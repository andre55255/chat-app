namespace Chat.Communication.ViewObjects.EndPoints
{
    public class FilterEndpointVO
    {
        public string? Route { get; set; } = null;
        public string? Verb { get; set; } = null;
        public List<string>? Roles { get; set; } = null;
        public int? Page { get; set; } = null;
        public int? Limit { get; set; } = null;
    }
}
