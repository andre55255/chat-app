namespace Chat.Communication.CustomExceptions
{
    public class NotFoundException : ApplicationException
    {
        public NotFoundException(string message, Exception? ex = null) : base(message, ex) { }
    }
}
