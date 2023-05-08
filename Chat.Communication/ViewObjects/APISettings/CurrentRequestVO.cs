namespace Chat.Communication.ViewObjects.APISettings
{
    public class CurrentRequestVO
    {
        public string? BaseUrl { get; set; }
        public CurrentUserVO? CurrentUser { get; set; }
    }

    public class CurrentUserVO
    {
        public string? Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public List<string>? Roles { get; set; }
    }
}
