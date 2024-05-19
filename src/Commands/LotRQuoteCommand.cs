using diggcordslash.Model;
using diggcordslash.Model.Commands.LotRCommand;
using diggcordslash.Model.DiscordAPI;
using System.Net.Http.Headers;

namespace diggcordslash.Commands;

// https://the-one-api.dev/documentation
public class LotRQuoteCommand : ICommand
{
    [Command("LotRQuote", 1, "Get a random Lord of the Rings quote", "2b1e781e-5d6f-421c-888a-ee443f994058")]
    public async Task<Interaction> Command(Interaction interaction, IServiceProvider serviceProvider)
    {
        var httpClientFactoryService = serviceProvider.GetService(typeof(IHttpClientFactory));
        if (httpClientFactoryService != null)
        {
            var httpClientFactory = (IHttpClientFactory)httpClientFactoryService;
            var url = $"https://the-one-api.dev/v2/quote/";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var httpClient = httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "PLXrj4fydHnazcbX3aGb");
            var response = await httpClient.SendAsync(request);
            var responseData = await response.Content.ReadFromJsonAsync<LotRQuoteResponse>();

            if (responseData != null && responseData.Docs != null && responseData.Docs.Length > 0)
            {
                var random = new Random();
                var randomNumber = random.Next(0, responseData.Docs.Length);
                var r = responseData.Docs[randomNumber];
                var result = new Interaction() { Type = InteractionType.ChannelMessageWithSource, data = new Data() { content = r.Dialog } };
                return result;
            }
        }

        var defaultResult = new Interaction() { Type = InteractionType.ChannelMessageWithSource, data = new Data() { content = "API is not currently available" } };
        return defaultResult;
    }
}