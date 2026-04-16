namespace ETS.Domain.Entities
{
    public abstract class BaseEntity
    {
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
