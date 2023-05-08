using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Chat.Communication.CustomAnotations
{
    public class IsValidPasswordAttribute : ValidationAttribute
    {
        public IsValidPasswordAttribute()
        {
        }

        public override bool IsValid(object? value)
        {
            try
            {
                if (value == null)
                    return false;

                string valueStr = value as string;
                if (string.IsNullOrEmpty(valueStr))
                    return false;

                bool hasDigit = valueStr.Where(x => Char.IsDigit(x)).Any();
                bool hasLower = valueStr.Where(x => Char.IsLower(x)).Any();
                bool hasUpper = valueStr.Where(x => Char.IsUpper(x)).Any();

                return hasDigit && hasLower && hasUpper;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
