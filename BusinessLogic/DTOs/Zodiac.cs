namespace BusinessLogic.DTOs;

public class ZodiacModel
{
    public Zodiac[] Signs { get; set; } = null!;
}

public class Zodiac
{
    public string Name { get; set; } = null!;
    public string[] Compatible { get; set; } = null!;
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Description { get; set; } = null!;
}
