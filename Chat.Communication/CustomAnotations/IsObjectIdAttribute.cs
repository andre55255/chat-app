using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace Chat.Communication.CustomAnotations
{
    public class IsObjectIdAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            try
            {
                if (value == null)
                    return false;

                string valueStr = value as string;
                ObjectId parsed = ObjectId.Parse(valueStr);
                return parsed != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
