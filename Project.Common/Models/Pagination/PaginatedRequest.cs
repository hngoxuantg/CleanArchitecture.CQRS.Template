using System.ComponentModel.DataAnnotations;

namespace Project.Common.Models.Pagination
{
    public abstract class PaginatedRequest
    {
        public int _pageNumber = 1;
        public int _pageSize = 12;
        public virtual int PageNumber
        {
            get => _pageNumber;
            set
            {
                if (value >= 1)
                    _pageNumber = value;
            }
        }
        public virtual int PageSize
        {
            get => _pageSize;
            set
            {
                if (value >= 1 && value <= 100)
                    _pageSize = value;
            }
        }
        [MaxLength(50, ErrorMessage = "Cannot exceed 50 characters")]
        public virtual string? Search { get; set; }
    }
}
