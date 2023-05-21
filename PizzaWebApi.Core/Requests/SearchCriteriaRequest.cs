namespace PizzaWebApi.Core.Requests
{
    /// <summary>
    /// Тело запроса для получения списка объектов по ключевому слову
    /// </summary>
    /// <remarks>
    /// Валидатор <see cref="Validators.SearchCriteriaRequestValidator"/>
    /// </remarks>
    public class SearchCriteriaRequest : PageCriteriaRequest
    {
        private string _keyword;

        public string Keyword
        {
            get => _keyword;
            set
            {
                if(value == null)
                    _keyword = string.Empty;
                else
                    _keyword = value.Trim();
            }
        }
    }
}
