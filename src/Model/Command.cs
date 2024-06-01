using System.Reflection;

namespace diggcordslash.Model;

public class Command
{
    public string Name { get; set; } = "";
    public int Type { get; set; }
    public string Description { get; set; } = "";
    public Guid Identifier { get; set; }
    public MethodInfo? Method { get; set; }
    public Type? Class { get; set; }
    public Option[]? Options { get; set; }
}