
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Chat.Communication.CustomAnotations
{
    public class IsMatchRegexAttribute : ValidationAttribute
    {
        private string _regexStr;

        public IsMatchRegexAttribute(string regexStr)
        {
            _regexStr = regexStr;
        }

        public override bool IsValid(object? value)
        {
            if (value == null)
                return false;

            string valueStr = value as string;
            Regex regex = new Regex(_regexStr);

            return regex.IsMatch(valueStr);
        }
    }
}
