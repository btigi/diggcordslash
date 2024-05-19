using diggcordslash.Model;
using diggcordslash.Model.DiscordAPI;

namespace diggcordslash.Commands;

public class IEQuoteCommand : ICommand
{
    [Command("IEQuote", 1, "Get a random quote from an Infinity Engine game", "65903dba-2ecd-4e03-95cd-72c66be9116f")]
    public async Task<Interaction> Command(Interaction interaction, IServiceProvider serviceProvider)
    {
        var httpClientFactoryService = serviceProvider.GetService(typeof(IHttpClientFactory));
        if (httpClientFactoryService != null)
        {
            var httpClientFactory = (IHttpClientFactory)httpClientFactoryService;
            var url = $"https://strings.iimods.com/";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.SendAsync(request);
            var responseData = await response.Content.ReadAsStringAsync();

            var i = new Interaction() { Type = InteractionType.ChannelMessageWithSource, data = new Data() { content = responseData } };
            return i;
        }

        var defaultResult = new Interaction() { Type = InteractionType.ChannelMessageWithSource, data = new Data() { content = "Error" } };
        return defaultResult;
    }
}