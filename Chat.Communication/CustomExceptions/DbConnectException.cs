namespace Chat.Communication.CustomExceptions
{
    public class DbConnectException : ApplicationException
    {
        public DbConnectException(string message, Exception? ex = null) : base(message, ex) { }
    }
}
