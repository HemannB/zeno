namespace Zeno.Data.Models;

public class PomodoroSession
{
    public int Id { get; set; }
    public int? TaskId { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public bool WasCompleted { get; set; } = false;
}
