using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjectDemo
{
    internal class EmailMessageService : IMessageService
    {
        public void SendMessage(string message)
        {
            Console.WriteLine($"Email Sent: {message}");
        }
    }
}
