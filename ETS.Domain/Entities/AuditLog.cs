namespace ETS.Domain.Entities
{
    public class AuditLog
    {
        protected AuditLog() { }

        public AuditLog(string tableName,
            string columnName,
            string? keyValue,
            string? oldValue,
            string? newValue,
            DateTime changedAt,
            string? changedBy,
            string eventType)
        {
            TableName = tableName;
            ColumnName = columnName;
            KeyValue = keyValue;
            OldValue = oldValue;
            NewValue = newValue;
            ChangedAt = changedAt;
            ChangedBy = changedBy;
            EventType = eventType;
        }

        public long Id { get; set; }
        public string TableName { get; set; } = string.Empty;
        public string ColumnName { get; set; } = string.Empty;
        public string? KeyValue { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? ChangedBy { get; set; }
        public string EventType { get; set; } = string.Empty;
    }
}
