using diggcordslash.Model.DiscordAPI;

namespace diggcordslash.Model;

public class Interaction
{
    public InteractionType Type { get; set; }
    public string token { get; set; } = "";
    public Member? member { get; set; }
    public string id { get; set; } = "";
    public string guild_id { get; set; } = "";
    public string app_permissions { get; set; } = "";
    public string guild_locale { get; set; } = "";
    public string locale { get; set; } = "";
    public Data? data { get; set; }
    public string channel_id { get; set; } = "";
}
