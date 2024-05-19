namespace diggcordslash.Model.DiscordAPI;

public class Member
{
    public User? User { get; set; }
    public string[]? Roles { get; set; }
    public object? Premium_since { get; set; }
    public string Permissions { get; set; } = "";
    public bool Pending { get; set; }
    public string Nick { get; set; } = "";
    public bool Mute { get; set; }
    public DateTime Joined_at { get; set; }
    public bool Is_pending { get; set; }
    public bool Deaf { get; set; }
}
