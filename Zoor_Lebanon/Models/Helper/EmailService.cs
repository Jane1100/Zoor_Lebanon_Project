using Google.Apis.Auth.OAuth2;   // For GoogleCredential
using Google.Apis.Gmail.v1;      // For GmailService
using Google.Apis.Gmail.v1.Data; // For the Message class
using Google.Apis.Services;      // For BaseClientService
using MimeKit;                   // For constructing the email
using System.Text;               // For encoding


namespace Zoor_Lebanon_Booking_Platform.Models.Helper
{
    public static class EmailService
    {
        private static readonly string[] Scopes = { GmailService.Scope.GmailSend };
        private static readonly string ApplicationName = "Zoor Lebanon Booking Platform";

        public static void SendEmail(string recipient, string subject, string body)
        {
            // Path to your credentials JSON file
            var credentialPath = "C:\\Users\\Jane Abi Saad\\Documents\\USJ-CS\\Semestre5\\Projet_Info\\Zoor_Lebanon_Booking_Platform\\zoor-lebanon-444419-ddab87fe0160.json";
            var credential = GoogleCredential.FromFile(credentialPath).CreateScoped(Scopes);

            using (var service = new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            }))
            {
                // Create the MIME message
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Zoor Lebanon", "contactzoorlb@gmail.com")); // Replace with your sender email
                message.To.Add(new MailboxAddress("", recipient)); // Recipient's email
                message.Subject = subject;
                message.Body = new TextPart("plain")
                {
                    Text = body
                };

                // Encode message to base64url
                var rawMessage = new Message
                {
                    Raw = Base64UrlEncode(message.ToString())
                };

                // Send the email
                service.Users.Messages.Send(rawMessage, "me").Execute();
            }
        }

        private static string Base64UrlEncode(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }
    }

}
