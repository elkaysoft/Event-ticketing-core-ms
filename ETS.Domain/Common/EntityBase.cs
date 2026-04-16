using System.ComponentModel.DataAnnotations;

namespace ETS.Domain.Common
{
    public abstract class EntityBase
    {
        private readonly List<IDomainEvent> _domainEvents = [];

        [MaxLength(150)]
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastModifiedAt { get; set; }
        [MaxLength(150)]
        public string? LastModifiedBy { get; set; }
        public bool IsDeleted { get; set; }

        public IReadOnlyList<IDomainEvent> GetDomainEvents() => _domainEvents.ToList();
        public void ClearDomainEvent() => _domainEvents.Clear();

        protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    }
}
