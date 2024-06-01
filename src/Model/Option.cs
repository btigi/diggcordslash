namespace diggcordslash.Model;

public class Option()
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int Type { get; set; }
    public Choice[] Choices { get; set; } = [];
}
