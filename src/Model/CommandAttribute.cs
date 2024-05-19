namespace diggcordslash.Model;

[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute(string name, int type, string description, string identifier) : Attribute
{
    private readonly string name = name;
    private readonly int type = type;
    private readonly string description = description;
    private readonly Guid identifier = new(identifier);

    public string Name { get { return name; } }
    public int Type { get { return type; } }
    public string Description { get { return description; } }
    public Guid Identifier { get { return identifier; } }
}
