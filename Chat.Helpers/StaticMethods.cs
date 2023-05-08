using Chat.Communication.CustomExceptions;
using Chat.Communication.ViewObjects.User;
using Chat.Communication.ViewObjects.Utils;
using FluentResults;
using MongoDB.Bson;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Chat.Helpers
{
    public static class StaticMethods
    {
        public static string SerializeObject(object @object)
        {
            try
            {
                return JsonSerializer.Serialize(@object);
            }
            catch (Exception ex)
            {
                return $"Falha ao deserializar objeto: {ex.Message}";
            }
        }

        public static string ExtractResultMessage(Result result)
        {
            try
            {
                if (result.IsSuccess)
                    return result.Successes.FirstOrDefault().Message;
                else
                    return result.Errors.FirstOrDefault().Message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static void GetPaginationItems<T>(ref ListAllEntityVO<T> list, ref int? limit, ref int? page) where T : class
        {
            try
            {
                if (limit is null)
                    limit = -1;

                if (page is null)
                    page = -1;

                double countPages = list.TotalItems.Value / limit.Value;
                if (Math.Floor(countPages) < page)
                {
                    limit = (int)list.TotalItems;
                    page = 0;
                    countPages = 0;
                }
                list.TotalPages = int.Parse(countPages.ToString()) <= 0 ? 0 : int.Parse(countPages.ToString());
                list.HasNextPage = list.TotalPages > (page + 1);
                list.HasPreviousPage = page > 0;

                if (limit <= 0)
                    limit = (int)list.TotalItems;
                if (page <= 0)
                    page = 0;
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Falha ao realizar cálculo de paginação de itens", ex);
            }
        }

        public static string CryptPasswordMD5(string password, string salt)
        {
            try
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes($"{password}@{salt}");
                    byte[] hashBytes = md5.ComputeHash(inputBytes);

                    StringBuilder sb = new StringBuilder();
                    foreach (byte item in hashBytes)
                    {
                        sb.Append(item.ToString("x2"));
                    }
                    return sb.ToString();
                }
            }
            catch (Exception)
            {
                throw new ValidException($"Falha inesperada ao tratar a senha");
            }
        }

        public static string GetSaltGenerateHashPasswordUser(UserReturnVO user, string password)
        {
            return $"@{user.FirstName}_@{user.Username}_@{user.Email}@{password}";
        }

        public static string GetSaltGenerateHashPasswordUser(UserCreateVO user, string password)
        {
            return $"@{user.FirstName}_@{user.Username}_@{user.Email}@{password}";
        }

        public static string GenerateAlfaNumericRandom(int size)
        {
            try
            {
                string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
                Random random = new Random();
                string result = new string(
                    Enumerable.Repeat(chars, size)
                              .Select(s => s[random.Next(s.Length)])
                              .ToArray());

                return result;
            }
            catch (Exception ex)
            {
                throw new ValidException($"Falha ao gerar nova senha alfanumérica", ex);
            }
        }

        public static void IsObjectIdValid(string value)
        {
            try
            {
                ObjectId parsed = ObjectId.Parse(value);
                if (parsed == null)
                    throw new Exception();
            }
            catch (Exception ex)
            {
                throw new ValidException($"Id {value} informado inválido", ex);
            }
        }

        public static void IsObjectIdValid(List<string> values)
        {
            try
            {
                foreach (string value in values)
                {
                    ObjectId? parsed = null; 
                    try
                    {
                        parsed = ObjectId.Parse(value);
                    }
                    catch (Exception) { }

                    if (parsed == null)
                        throw new ValidException($"Id {value} informado inválido");
                }
            }
            catch (ValidException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new ValidException($"Algum id informado inválido", ex);
            }
        }
    }
}
