using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace Chat.Communication.CustomAnotations
{
    public class IsObjectIdListAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            try
            {
                if (value == null)
                    return false;

                List<string> values = value as List<string>;
                foreach (string val in values)
                {
                    ObjectId parsed = ObjectId.Parse(val);
                    if (parsed == null)
                        return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
