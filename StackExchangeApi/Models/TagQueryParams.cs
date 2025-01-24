namespace StackExchangeApi.Models
{
    public class TagQueryParams
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string OrderBy { get; set; } = string.Empty;
        public string ThenOrderBy { get; set; } = string.Empty;
        public bool IsDescending { get; set; } = false;
    }
}
