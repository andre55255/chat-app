namespace Chat.Communication.CustomExceptions
{
    public class EmailException : ApplicationException
    {
        public EmailException(string message, Exception ex = null) : base(message, ex) { }
    }
}
