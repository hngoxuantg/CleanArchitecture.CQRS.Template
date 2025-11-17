namespace Project.Domain.Entities.Base
{
    public abstract class BaseEntity
    {
        public virtual int Id { get; set; }

        public virtual DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual int? CreateBy { get; set; }

        public virtual DateTime? UpdateAt { get; set; }

        public virtual int? UpdateBy { get; set; }

        public virtual void SetCreated(int? updatedBy)
        {
            CreatedAt = DateTime.UtcNow;
            CreateBy = updatedBy;
        }

        public virtual void SetUpdated(int? updatedBy)
        {
            UpdateAt = DateTime.UtcNow;
            UpdateBy = updatedBy;
        }
    }
}
