using MSL_APP.Data;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MSL_APP.Utility
{
    /// <summary>
    /// Main class to handle emails. Currently works with SendGrid. Ideally, this class will be replaced if Mohawk allows SMTP
    /// and outgoing mail from a particular server.
    /// 
    /// If SendGrid is used in deployment, the admin will need to set a machine level environment variable titled 'SENDGRID_API_KEY' with
    /// a key generated on their account. 
    /// </summary>
    public static class MSLEmailHandler
    {
        private static readonly string emailSource = "studentlicensesadmin@mohawkcollege.ca";
        private static readonly string emailFrom = "Administrator";

        /// <summary>
        /// Send email to confirm account via SendGrid. Requires the intended user and a callback URL to give the user
        /// to confirm the account.
        /// </summary>
        /// <param name="user">New user</param>
        /// <param name="callbackUrl">URL to confirm</param>
        /// <returns></returns>
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


        /// <summary>
        /// Send email for password resets via SendGrid. Requires the intended user and a callback URL to give to the user
        /// to actaully reset their account.
        /// </summary>
        /// <param name="user">User who forgot their password</param>
        /// <param name="callbackUrl">URL for reset</param>
        /// <returns></returns>
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
