namespace Chat.Communication.ViewObjects.Utils
{
    public class ListAllEntityVO<T> where T : class
    {
        public List<T> Items { get; set; } = new List<T>();
        public long? TotalItems { get; set; } = null;
        public int? TotalPages { get; set; } = null;
        public bool? HasNextPage { get; set; } = null;
        public bool? HasPreviousPage { get; set; } = null;
    }
}
