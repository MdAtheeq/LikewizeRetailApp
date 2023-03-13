using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EASendMail;
using LikewizeRetailApp.Models.Email;

namespace LikewizeRetailApp.Controllers
{
    public class EmailController : ApiController
    {
        public IHttpActionResult SendMail(EmailClass ec)
        {
            string subject = ec.Subject;
            string body = ec.Body;
            string to = ec.To;


            SmtpMail oMail = new SmtpMail("TryIt");


            oMail.From = "atheeqcontender@gmail.com";


            oMail.To = to;
            oMail.Subject = subject;
            oMail.TextBody = body;

            SmtpServer oServer = new SmtpServer("smtp.gmail.com");
            oServer.User = "atheeqcontender@gmail.com";
            oServer.Password = "losackowjiyrdzgh";

            oServer.Port = 587;

            // detect SSL/TLS automatically
            oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;


            SmtpClient oSmtp = new SmtpClient();
            oSmtp.SendMail(oServer, oMail);
            return Ok();
           
        }
    }
}
