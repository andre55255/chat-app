namespace Chat.Communication.ViewObjects.User
{
    public class FilterUserVO
    {
        public bool? IsBlocked { get; set; } = null;
        public string? Name { get; set; } = null;
        public string? Username { get; set; } = null;
        public string? Email { get; set; } = null;
        public List<string>? Roles { get; set; } = null;
        public int? Page { get; set; } = null;
        public int? Limit { get; set; } = null;
    }
}
