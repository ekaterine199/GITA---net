using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutofacDemo
{
    public class SmsMessageService: IMessageService
    {
        public void SendMessage(string message)
        {
            Console.WriteLine($"SMS Sent: {message}");
        }

        
    }
}
