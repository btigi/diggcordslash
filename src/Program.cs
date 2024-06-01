using diggcordslash.Model;
using diggcordslash.Model.DiscordAPI;
using diggcordslash.Commands;
using NSec.Cryptography;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using diggcordslash.Helpers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

var ApplicationId = Environment.GetEnvironmentVariable("SlashBotApplicationId");
var BotToken = Environment.GetEnvironmentVariable("SlashBotBotToken");
var DiscordPublicKey = Environment.GetEnvironmentVariable("SlashBotDiscordPublicKey");
var SecurityId = Environment.GetEnvironmentVariable("SlashBotSecurityId");

if (String.IsNullOrEmpty(ApplicationId) || String.IsNullOrEmpty(BotToken) || String.IsNullOrEmpty(DiscordPublicKey) || String.IsNullOrEmpty(SecurityId))
{
    throw new InvalidOperationException("Missing configuration environment variables missing");
}

app.MapGet("listavailablecommands", (string securityId) =>
{
    if (securityId != SecurityId)
        return Results.Unauthorized();

    var commands = GetCommands(includeReflectionInformation: false);
    return Results.Ok(commands);
});

app.MapGet("registercommand", async (string securityId, Guid identifier, string? guildId, IHttpClientFactory httpClientFactory) =>
{
    if (securityId != SecurityId)
        return Results.Unauthorized();

    var commands = GetCommands();
    var command = commands.Find(f => f.Identifier == identifier);
    if (command != null)
    {
        var commandToSend = new DiscordCommand
        {
            Name = command.Name.ToLower(),
            Description = command.Description,
            Type = command.Type,
            Options = command.Options
        };

        var url = $"https://discord.com/api/v10/applications/{ApplicationId}/commands";
        if (!String.IsNullOrEmpty(guildId))
        {
            url = $"https://discord.com/api/v10/applications/{ApplicationId}/guilds/{guildId}/commands";
        }
        return Results.Ok(await MakeRequest(HttpMethod.Post, url, JsonContent.Create(commandToSend), httpClientFactory));
    }

    return Results.NotFound();
});

app.MapGet("listregisteredcommands", async (string securityId, string? guildId, IHttpClientFactory httpClientFactory) =>
{
    if (securityId != SecurityId)
        return Results.Unauthorized();

    var url = $"https://discord.com/api/v10/applications/{ApplicationId}/commands";
    if (!string.IsNullOrEmpty(guildId))
    {
        url = $"https://discord.com/api/v10/applications/{ApplicationId}/guilds/{guildId}/commands";
    }
    return Results.Ok(await MakeRequest(HttpMethod.Get, url, null, httpClientFactory));
});

app.MapGet("deletecommand", async (string securityId, string identifier, string? guildId, IHttpClientFactory httpClientFactory) =>
{
    if (securityId != SecurityId)
        return Results.Unauthorized();

    var url = $"https://discord.com/api/v10/applications/{ApplicationId}/commands/{identifier}";
    if (!String.IsNullOrEmpty(guildId))
    {
        url = $"https://discord.com/api/v10/applications/{ApplicationId}/guilds/{guildId}/commands/{identifier}";
    }
    return Results.Ok(await MakeRequest(HttpMethod.Delete, url, null, httpClientFactory));
});

app.MapPost("interactions", async (HttpContext context) =>
{
    var (isValid, interaction) = await TryGetInteraction(context);
    if (!isValid)
    {
        return Results.Unauthorized();
    }

    var command = GetCommands().Find(f => String.Equals(Convert.ToString(f.Name), interaction?.data?.name, StringComparison.InvariantCultureIgnoreCase));
    if (command != null && command.Class != null && command.Method != null)
    {
        var instance = Activator.CreateInstance(command.Class);

        if (instance is ICommand executableCommand)
        {
            var result = await executableCommand.Command(interaction, app.Services);
            return Results.Json(result);
        }
    }

    if ((int)interaction.Type == 1)
    {
        return Results.Json(new Pong() { Type = InteractionType.Pong });
    }

    var i = new Interaction() { Type = InteractionType.ChannelMessageWithSource, data = new Data() { content = "iggbot saw an unknown command" } };
    return Results.Json(i);
});

app.Run();


async Task<(bool isValid, Interaction interaction)> TryGetInteraction(HttpContext context)
{
    context.Request.EnableBuffering();

    string? signature = context.Request.Headers["x-signature-ed25519"];
    string? timestamp = context.Request.Headers["x-signature-timestamp"];

    if (signature != null && long.TryParse(timestamp, out long timestampAsLong) && context.Request.ContentLength > 0)
    {
        var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
        var interaction = JsonSerializer.Deserialize<Interaction>(body, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true, Converters = { new IntToStringConverter() } });

        if (interaction != null && IsValidDiscordSignature(signature, timestampAsLong, body))
        {
            return (true, interaction);
        }
    }
    return (false, new Interaction());
}

bool IsValidDiscordSignature(string signature, long timestamp, string body)
{
    var algorithm = SignatureAlgorithm.Ed25519;
    var publicKey = PublicKey.Import(algorithm, HexStringToByteArray(DiscordPublicKey), KeyBlobFormat.RawPublicKey);
    var data = Encoding.UTF8.GetBytes(timestamp + body);
    return algorithm.Verify(publicKey, data, HexStringToByteArray(signature));
}

byte[] HexStringToByteArray(string hexstring)
{
    var length = hexstring.Length;
    var bytes = new byte[length / 2];

    for (int i = 0; i < length; i += 2)
    {
        bytes[i / 2] = Convert.ToByte(hexstring.Substring(i, 2), 16);
    }

    return bytes;
}

List<Command> GetCommands(bool includeReflectionInformation = true)
{
    var commands = new List<Command>();
    var types = Assembly.GetExecutingAssembly()
        .GetTypes()
        .Where(type => typeof(ICommand).IsAssignableFrom(type) && !type.IsInterface);

    foreach (var type in types)
    {
        var requiredMethod = type.GetMethod("Command");
        if (requiredMethod != null)
        {
            var nameAttributes = requiredMethod.GetCustomAttributes(typeof(CommandAttribute), false);

            Command? command = null;
            if (nameAttributes.Length > 0)
            {
                command = new Command()
                {
                    Name = ((CommandAttribute)nameAttributes[0]).Name,
                    Type = ((CommandAttribute)nameAttributes[0]).Type,
                    Description = ((CommandAttribute)nameAttributes[0]).Description,
                    Identifier = ((CommandAttribute)nameAttributes[0]).Identifier,
                    Class = includeReflectionInformation ? type : null,
                    Method = includeReflectionInformation ? requiredMethod : null
                };
            }

            var optionAttributes = requiredMethod.GetCustomAttributes(typeof(OptionAttribute), false);

            if (command != null && optionAttributes.Length > 0)
            {
                command.Options = new diggcordslash.Model.Option[optionAttributes.Length];
                for (int h = 0; h < optionAttributes.Length; h++)
                {
                    var name = ((OptionAttribute)optionAttributes[h]).Name;
                    var paramType = ((OptionAttribute)optionAttributes[h]).Type;
                    var description = ((OptionAttribute)optionAttributes[h]).Description;
                    var choices = ((OptionAttribute)optionAttributes[h]).Choices;

                    command.Options[h] = new diggcordslash.Model.Option()
                    {
                        Name = name.ToLower(),
                        Type = (int)paramType,
                        Description = description
                    };

                    if (choices != null)
                    {
                        for (int i = 0; i < choices.Length; i++)
                        {
                            command.Options[h].Choices = new Choice[choices.Length];

                            for (int j = 0; j < choices.Length; j++)
                            {
                                var parts = choices[j].Split("|");
                                command.Options[h].Choices[j] = new Choice()
                                {
                                    Name = parts[0],
                                    Value = parts[1]
                                };
                            }
                        }
                    }
                }
            }

            if (command != null)
            {
                commands.Add(command);
            }
        }
    }
    return commands;
}

async Task<string?> MakeRequest(HttpMethod httpMethod, string url, JsonContent? content, IHttpClientFactory httpClientFactory)
{
    var request = new HttpRequestMessage(httpMethod, url);
    if (content != null)
    {
        request.Content = content;
    }
    var httpClient = httpClientFactory.CreateClient();
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", BotToken);
    var response = await httpClient.SendAsync(request);
    var responseData = await response.Content.ReadAsStringAsync();
    return responseData;
}