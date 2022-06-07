namespace HoaMokuBot.Domain.Contracts
{
    using Victoria.Responses.Search;

    public class SearchParameters
    {
        public SearchParameters(SearchType searchType, string searchText)
        {
            this.SearchType = searchType;
            this.SearchText = searchText;
        }

        public SearchType SearchType { get; }

        public string SearchText { get; }
    }
}
