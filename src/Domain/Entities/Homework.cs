namespace Domain.Entities;

public class Homework
{
    private Homework() { }

    public Homework(string title, DateTime expirationTime, string? description = null)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        ExpirationTime = expirationTime;
        IsDone = false;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public DateTime ExpirationTime { get; private set; }
    public bool IsDone { get; private set; }

    public void MarkAsDone() => IsDone = true;
}

