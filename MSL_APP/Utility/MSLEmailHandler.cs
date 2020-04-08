using MSL_APP.Data;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MSL_APP.Utility
{
    public static class MSLEmailHandler
    {
        private static readonly string emailSource = "studentlicensesadmin@mohawkcollege.ca";
        private static readonly string emailFrom = "Administrator";


        public async static Task<Response> SendConfirmationEmail(ApplicationUser user, string callbackUrl)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY", EnvironmentVariableTarget.Machine);
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(emailSource, emailFrom);
            var subject = "Confirm your Mohawk Student License account";
            var to = new EmailAddress(user.Email, user.FirstName + " " + user.LastName); //Input.Email
            var plainTextContent = $"Please confirm your account by copy and pasting this URL into your browser: {HtmlEncoder.Default.Encode(callbackUrl)}";
            var htmlContent = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            return await client.SendEmailAsync(msg);
        }


        public async static Task<Response> SendPasswordResetEmail(ApplicationUser user, string callbackUrl)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY", EnvironmentVariableTarget.Machine);
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(emailSource, emailFrom);
            var subject = "Reset Mohawk License System Password";
            var to = new EmailAddress(user.Email, user.FirstName + " " + user.LastName); //Input.Email
            var plainTextContent = $"Please reset your password by copying and pasting the following into your address bar: {HtmlEncoder.Default.Encode(callbackUrl)}";
            var htmlContent = $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            return await client.SendEmailAsync(msg);

        }

    }
}
