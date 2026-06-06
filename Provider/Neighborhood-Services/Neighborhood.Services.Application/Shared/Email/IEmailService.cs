using Neighborhood.Services.Domain.ProblemTypes;

namespace Neighborhood.Services.Application.Shared.Email
{
    public interface IEmailService
    {
        //General Method
        public  Task SendEmailAsync(EmailMessageDto message);

        //Email Verification
        public Task <EmailSendingResult>SendEmailVerificationEmailAsync(
            string EmailReceiver,
        string ConfirmationURL,
        string title = "Confirm your email",
        IEnumerable<EmailAttachmentDto>? emailAttachments = null!);

        //Password Reset
        public Task<EmailSendingResult> SendPasswordResetEmailAsync(
            string EmailReceiver,
       string PasswordResetURL,
       string title = "Reset Your Password",
       IEnumerable<EmailAttachmentDto>? emailAttachments = null!);

        //Booking Confirmed
        public Task <EmailSendingResult>SendBookingVerificationEmainAsync(
           string EmailReceiver,
    int BookingId,
    DateTime Time,
    string Problem,
    string TechnicianName,
    string title = "Booking is Verified",
    IEnumerable<EmailAttachmentDto>? emailAttachments = null!);

    }
}
