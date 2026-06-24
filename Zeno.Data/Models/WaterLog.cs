namespace Zeno.Data.Models;

public class WaterLog
{
    public int Id { get; set; }
    public int Glasses { get; set; } = 0;
    public int Goal { get; set; } = 8;
    public DateTime Date { get; set; } = DateTime.UtcNow.Date;
}
