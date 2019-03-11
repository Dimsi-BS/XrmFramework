namespace Model.Sdk.Queries
{
    public class PagingInfo
    {
        public int PageNumber { get; set; } = 1;

        public int? PageSize { get; set; }

        public string PagingCookie { get; set; }
    }
}
