namespace Zeno.Data.Models;

public enum Priority { Low, Medium, High }
public enum Recurrence { None, Daily, Weekly, Monthly }

public class ZenoTask
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool IsCompleted { get; set; } = false;
    public bool IsFavorite { get; set; } = false;
    public Priority Priority { get; set; } = Priority.Medium;
    public Recurrence Recurrence { get; set; } = Recurrence.None;
    public int? ProjectId { get; set; }
    public DateTime? DueDate { get; set; }
    public TimeSpan? DueTime { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
