namespace PizzaWebApi.Core.Response
{
    public class SearchResult<T>
    {
        public SearchResult(IEnumerable<T> results, int total)
        {
            Results = results;
            Total = total;
        }

        public IEnumerable<T> Results { get; set; }
        public int Total { get; set; }
    }
}
