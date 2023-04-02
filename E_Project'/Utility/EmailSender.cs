using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace E_Project_.Utility
{
    public class EmailSender : IEmailSender
    {
        //    public Task SendEmailAsync(string email, string subject, string htmlMessage)
        //    {
        //        return Execute( email,  subject, htmlMessage);
        //    }

        //    public async Task Execute(string email, string subject, string body)
        //    {
        //        MailjetClient client = new MailjetClient("63fa16d27a0dc88311510eecbb47d87d", "e690ec78ad8520dbb4d234436fb04041");

        //        MailjetRequest request = new MailjetRequest
        //        {
        //            Resource = Send.Resource,
        //        }
        //         .Property(Send.Messages, new JArray {
        // new JObject {
        //  {
        //   "From",
        //   new JObject {
        //    {"Email", "khaleledibrahim@protonmail.com"},
        //    {"Name", "khaled"}
        //   }
        //  }, {
        //   "To",
        //   new JArray {
        //    new JObject {
        //     {
        //      "Email",
        //      // Email Variable in Function Execute 
        //     email
        //     }, {
        //      "Name",
        //      "KhaledOrg"
        //     }
        //    }
        //   }
        //  }, {
        //   "Subject",
        //   //subject Variable in Function Execute 
        //   subject
        //  },  {
        //   "HTMLPart",
        //  //  Body 
        //  body

        //  } 
        // }
        //         });
        //      await client.PostAsync(request);
        //    }
        //}


        ///  Here We Can Impelement send Email 
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            string FromEmail = "khaledibrahimahmedali@outlook.com";
            string FromPass="";

            var message = new MailMessage();
            message.From = new MailAddress(FromEmail);
            message.Subject = subject;
            message.To.Add(email) ;
            message.Body = $"<html><body>{htmlMessage}</body></html>";
            message.IsBodyHtml = true;



            // Email Provider Gmail
            var client = new SmtpClient("smtp-mail.outlook.com" )
            {
                Port= 587,
                Credentials = new NetworkCredential(FromEmail, FromPass),
                EnableSsl = true
            };
            client.Send(message);
        }
    }
}
