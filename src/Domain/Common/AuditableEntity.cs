namespace PlovCenter.Domain.Common;

public abstract class AuditableEntity
{
    public Guid Id { get; protected set; }

    public DateTime CreatedUtc { get; protected set; }

    public DateTime UpdatedUtc { get; protected set; }

    protected void InitializeAudit(DateTime utcNow)
    {
        Id = Guid.NewGuid();
        CreatedUtc = utcNow;
        UpdatedUtc = utcNow;
    }

    protected void Touch(DateTime utcNow)
    {
        UpdatedUtc = utcNow;
    }
}
