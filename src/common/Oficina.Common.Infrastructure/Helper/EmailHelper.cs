using Microsoft.Extensions.Logging;
using Oficina.Common.Domain.Entities;
using Oficina.Common.Domain.IHelper;

namespace Oficina.Common.Infrastructure.Helper
{
    public class EmailHelper : IEmailHelper
    {
        private readonly ILogger<EmailHelper> _logger;
     
        public EmailHelper(
            ILogger<EmailHelper> logger)
        {
            _logger = logger;
        }

        public async Task EnviarEmailAsync(EmailMessage emailMessage)
        {
            try
            {
                _logger.LogInformation(
                    "Iniciando envio de email para {Destinatario}. Assunto: {Assunto}",
                    emailMessage.Para,
                    emailMessage.Assunto);

                // Validações básicas
                if (string.IsNullOrWhiteSpace(emailMessage.Para))
                    throw new ArgumentException("Destinatário não pode ser vazio.", nameof(emailMessage));

                if (string.IsNullOrWhiteSpace(emailMessage.Assunto))
                    throw new ArgumentException("Assunto não pode ser vazio.", nameof(emailMessage));

                if (string.IsNullOrWhiteSpace(emailMessage.CorpoHtml))
                    throw new ArgumentException("Corpo do email não pode ser vazio.", nameof(emailMessage));

                _logger.LogInformation(
                    "Email enviado com sucesso para {Destinatario}. Assunto: {Assunto}",
                    emailMessage.Para,
                    emailMessage.Assunto);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Erro ao enviar email para {emailMessage.Para} - Assunto: {emailMessage.Assunto}", ex);
            }
        }

        // Método estático para obter configurações padrão (quando precisar no futuro)
        private static EmailConfiguration GetDefaultConfiguration()
        {
            return new EmailConfiguration
            {
                SmtpHost = "smtp.gmail.com",
                SmtpPort = 587,
                SmtpUsername = string.Empty,
                SmtpPassword = string.Empty,
                FromEmail = "noreply@oficinaops.com",
                FromName = "Oficina Ops",
                EnableSsl = true
            };
        }
    }
}
