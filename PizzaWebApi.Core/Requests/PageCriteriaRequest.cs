namespace PizzaWebApi.Core.Requests
{
    /// <summary>
    /// Тело запроса для получения списка объектов
    /// </summary>
    /// <remarks>
    /// Валидатор <see cref="Validators.PageCriteriaRequestValidator"/>
    /// </remarks>
    public class PageCriteriaRequest
    {
        /// <summary>
        /// Максимальная величина размера страницы выборки
        /// </summary>
        const int MAX_PAGE_SIZE = 50;
        private int _page;
        private int _pageSize;

        public int Page
        {
            get => _page;
            set => _page = value < 1 ? 1 : value;
        }
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1 || value > MAX_PAGE_SIZE ? MAX_PAGE_SIZE : value;
        }

        public int Skip => (Page - 1) * PageSize;
    }
}
