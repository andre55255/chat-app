namespace Chat.Communication.ViewObjects.APIResponse
{
    public class APIResponseVO
    {
        public string? Message { get; set; }
        public object? Object { get; set; }
        public bool Success { get; set; }

        public APIResponseVO()
        {
        }
        public APIResponseVO(string? message, object? @object, bool success)
        {
            Message = message;
            Object = @object;
            Success = success;
        }

        public static APIResponseVO Ok(string? message = null, object? obj = null)
        {
            return new APIResponseVO(message, obj, true);
        }

        public static APIResponseVO Fail(string? message = null, object? obj = null)
        {
            return new APIResponseVO(message, obj, false);
        }
    }
}
