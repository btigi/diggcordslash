using diggcordslash.Model;
using diggcordslash.Model.DiscordAPI;

namespace diggcordslash.Commands;

public class Kharacter : ICommand
{
    [Command("Kharacter", 1, "Get a random karacter", "4d08cdfd-fa6d-4d67-8b61-187f33af0271")]
    public async Task<Interaction> Command(Interaction interaction, IServiceProvider serviceProvider)
    {
        var kharacters = new string[]
        {
            "Donald bae", "Sora", "Mansex", "Goofy", "The Hydra's Back", "Danny Devito", "Roxas", "Aqua", "Terra", "Ventus", "Minne", "Mickey", "Evil Merlin"
        };

        var random = new Random();
        var kharacter = random.Next(0, kharacters.Length);

        var i = new Interaction() { Type = InteractionType.ChannelMessageWithSource, data = new Data() { content = $"{interaction?.member?.User?.Username} - your kharacter is {kharacters[kharacter]}" } };
        return await Task.FromResult(i);
    }
}