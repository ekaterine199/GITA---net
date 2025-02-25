using InversionOfControl.Interfaces;

namespace InversionOfControl.Services
{
    public class EmailNotificationService : InotificationService
    {
        public void Notify(string message)
        {
            throw new NotImplementedException();
        }

        public void SendEmail(string message)
        {
            Console.WriteLine(message);
        }
    }
}
