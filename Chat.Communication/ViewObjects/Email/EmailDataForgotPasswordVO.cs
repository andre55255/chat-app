namespace Chat.Communication.ViewObjects.Email
{
    public class EmailDataForgotPasswordVO
    {
        public List<string> Recipients { get; set; }
        public string NewPassword { get; set; }
    }
}
