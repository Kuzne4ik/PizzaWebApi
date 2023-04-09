namespace PizzaWebApi.Core.Requests
{
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
