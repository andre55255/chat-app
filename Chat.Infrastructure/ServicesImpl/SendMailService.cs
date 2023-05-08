using Chat.Communication.CustomExceptions;
using Chat.Communication.ViewObjects.Email;
using Chat.Core.ServicesInterface;
using Chat.Helpers;
using FluentResults;
using System.Net.Mail;
using System.Net;
using Chat.Communication.ViewObjects.APISettings;
using Chat.Communication.ViewObjects.User;

namespace Chat.Infrastructure.ServicesImpl
{
    public class SendMailService : ISendMailService
    {
        private readonly IAPISettingsService _apiSettingsService;
        private readonly ILogService _logService;

        public SendMailService(IAPISettingsService apiSettingsService, ILogService logService)
        {
            _apiSettingsService = apiSettingsService;
            _logService = logService;
        }

        public async Task<Result> SendMailForgotPasswordAsync(EmailDataForgotPasswordVO data, UserReturnVO user)
        {
            try
            {
                Result valid = await ValidData(data, user, true);
                if (valid.IsFailed)
                    return valid;

                SendMailVO active = new SendMailVO();
                active.Body = LoadTemplate(ConstantsEmail.TemplateResetAccount);
                active.Recipients = data.Recipients;
                active.Subject = ConstantsEmail.SubjectResetAccount + $"{user.FirstName} {user.LastName}";

                active.Body = active.Body.Replace("[[NAME]]", $"{user.FirstName} {user.LastName}");
                active.Body = active.Body.Replace("[[PASS]]", data.NewPassword);

                Result result = await SendMailAsync(active);
                return result;
            }
            catch (ValidException ex)
            {
                throw new EmailException(ex.Message, ex);
            }
            catch (EmailException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex, 
                    $"Falha inesperada ao enviar email de redefinição de senha, {user.Username}. {StaticMethods.SerializeObject(data)}", 
                    this.GetType().ToString());

                throw new EmailException($"Falha inesperada ao enviar email de redefinição de senha, {user.Username}", ex);
            }
        }

        private string LoadTemplate(string nameTemplate)
        {
            try
            {
                string filepath = Path.Combine(Path.GetFullPath(DirectoriesName.EmailTemplates), nameTemplate);
                using (StreamReader reader = new StreamReader(filepath))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (EmailException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                _logService.WriteAsync(ex, 
                    $"Falha ao buscar template para envio de email, nome: {nameTemplate}",
                    this.GetType().ToString());

                throw new EmailException($"Falha ao buscar template para envio de email, nome: {nameTemplate}");
            }
        }

        private async Task<Result> ValidData(EmailDataForgotPasswordVO data, UserReturnVO user = null, bool isUserValid = false)
        {
            try
            {
                if (isUserValid && user is null)
                    throw new EmailException($"Usuário não informado para envio de email");

                if (data.Recipients is null || data.Recipients.Count <= 0)
                    throw new EmailException($"Destinatários não informados para envio de email");

                return Result.Ok();
            }
            catch (EmailException ex)
            {
                await _logService.WriteAsync(ex, ex.Message, this.GetType().ToString());
                throw ex;
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex, $"Falha inesperada ao validar dados para envio de email", this.GetType().ToString());
                throw new EmailException($"Falha inesperada ao validar dados para envio de email", ex);
            }
        }

        private async Task<ConfigurationMailVO> GetCredentailsSendMailAsync()
        {
            try
            {
                AppSettingsVO settings = _apiSettingsService.GetInfoAppSettings();
                string smtpHost = settings!.Email!.Server!;
                if (smtpHost is null || string.IsNullOrEmpty(smtpHost))
                    throw new EmailException($"Host smtp para envio de email não encontrado");

                int? smtpPort = settings!.Email!.Port;
                if (smtpPort is null)
                    throw new EmailException($"Porta de smtp para envio de email não encontrada");

                string loginEmail = settings!.Email!.Login!;
                if (loginEmail is null || string.IsNullOrEmpty(loginEmail))
                    throw new EmailException($"Login para envio de email não encontrado");

                string passEmail = settings!.Email!.Pass!;
                if (passEmail is null || string.IsNullOrEmpty(passEmail))
                    throw new EmailException($"Senha para envio de email não encontrada");

                return new ConfigurationMailVO
                {
                    SmtpHost = smtpHost,
                    SmtpPort = smtpPort.Value,
                    EmailLogin = loginEmail,
                    EmailPass = passEmail
                };
            }
            catch (EmailException ex)
            {
                await _logService.WriteAsync(ex, ex.Message, this.GetType().ToString());
                throw new EmailException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex, $"Falha ao credenciais para envio de email", this.GetType().ToString());
                throw new EmailException($"Falha ao credenciais para envio de email", ex);
            }
        }

        private async Task<Result> SendMailAsync(SendMailVO sendMail)
        {
            MailMessage mail = new MailMessage();
            try
            {
                ConfigurationMailVO configs = await GetCredentailsSendMailAsync();
                mail.From = new MailAddress(configs.EmailLogin);

                foreach (string recipients in sendMail.Recipients)
                {
                    mail.To.Add(recipients);
                }
                mail.Subject = sendMail.Subject;
                mail.Body = sendMail.Body;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.Normal;
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
                if (sendMail.Attachments != null)
                {
                    foreach (Attachment attch in sendMail.Attachments)
                    {
                        mail.Attachments.Add(attch);
                    }
                }

                SmtpClient client = new SmtpClient();
                client.Credentials = new NetworkCredential(configs.EmailLogin, configs.EmailPass);
                client.Host = configs.SmtpHost;
                client.Port = configs.SmtpPort;
                client.EnableSsl = true;

                await client.SendMailAsync(mail);
                return Result.Ok();
            }
            catch (EmailException ex)
            {
                throw ex;
            }
            catch (NotFoundException ex)
            {
                throw new EmailException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                await _logService.WriteAsync(ex, $"Falha no método de disparar email", this.GetType().ToString());
                throw new EmailException($"Falha no método de disparar email", ex);
            }
            finally
            {
                mail.Dispose();
            }
        }
    }
}
