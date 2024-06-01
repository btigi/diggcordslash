using diggcordslash.Model.DiscordAPI;

namespace diggcordslash.Model;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class OptionAttribute(string name, OptionType type, string description, bool required, string[]? choices = null) : Attribute
{
    private readonly string name = name;
    private readonly OptionType type = type;
    private readonly string description = description;
    private readonly bool required = required;
    private readonly string[]? choices = choices;

    public string Name { get { return name; } }
    public OptionType Type { get { return type; } }
    public string Description { get { return description; } }
    public bool Required { get { return required; } }
    public string[]? Choices { get { return choices; } }    
}