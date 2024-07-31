using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmailServiceLibrary.Model;

namespace EmailServiceLibrary.Services
{
    public interface IEmailSender
    {
        /*void SendEmail(Message message);*/
        Task SendEmailAsync(Message message);
    }

}
