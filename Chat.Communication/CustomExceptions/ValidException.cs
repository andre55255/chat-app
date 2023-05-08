namespace Chat.Communication.CustomExceptions
{
    public class ValidException : ApplicationException
    {
        public ValidException(string message, Exception? ex = null) : base(message, ex) { }
    }
}
