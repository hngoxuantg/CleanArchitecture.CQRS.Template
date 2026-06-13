namespace Project.Domain.Entities.Base
{
    public abstract class BaseEntity
    {
        public virtual int Id { get; set; }

        public virtual DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual int? CreatedBy { get; set; }

        public virtual DateTime? UpdatedAt { get; set; }

        public virtual int? UpdatedBy { get; set; }

        public virtual void SetCreated(int? createdBy)
        {
            CreatedAt = DateTime.UtcNow;
            CreatedBy = createdBy;
        }

        public virtual void SetUpdated(int? updatedBy)
        {
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy;
        }
    }
}
