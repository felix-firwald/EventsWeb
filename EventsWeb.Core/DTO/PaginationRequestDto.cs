using System.ComponentModel.DataAnnotations;

namespace EventsWeb.Core.DTO
{
    /// <summary>
    /// Represents pagination info user sends to server
    /// </summary>
    public sealed class PaginationRequestDto
    {
        /// <summary>
        /// Curent page index (starts from 1 to N)
        /// </summary>
        public int CurrentPage { get; }

        /// <summary>
        /// Page size (elements per page)
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Number of elements which will be skipped
        /// </summary>
        public int SkipCount
        {
            get
            {
                return this.PageSize * (this.CurrentPage - 1);
            }
        }
        public PaginationRequestDto(int currentPage, int pageSize)
        {
            if (currentPage < 1)
            {
                throw new ValidationException("Параметр номера страницы не может быть меньше единицы");
            }
            if (pageSize < 1)
            {
                throw new ValidationException("Параметр размера страницы не может быть меньше единицы");
            }
            this.CurrentPage = currentPage;
            this.PageSize = pageSize;
        }
    }
}
