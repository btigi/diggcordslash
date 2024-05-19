using diggcordslash.Model;
using diggcordslash.Model.DiscordAPI;

namespace diggcordslash.Commands;

public class AboutCommand1 : ICommand
{
    [Command("iggAbout", 1, "See information about iggbot", "52ccc25d-2d54-4933-b454-c04b0d471236")]
    public async Task<Interaction> Command(Interaction interaction, IServiceProvider serviceProvider)
    {
        var i = new Interaction() { Type = InteractionType.ChannelMessageWithSource, data = new Data() { content = "<todo>" } };
        return await Task.FromResult(i);
    }
}