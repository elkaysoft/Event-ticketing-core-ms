using ETS.Domain.Models.Postmark.Requests;
using ETS.Domain.Models.Postmark.Response;

namespace ETS.Domain.Contracts
{
    public interface IEmailService
    {
        Task<SendEmailResponse> SendEmail(SendEmailRequest request, CancellationToken cancellationToken);
    }
}
