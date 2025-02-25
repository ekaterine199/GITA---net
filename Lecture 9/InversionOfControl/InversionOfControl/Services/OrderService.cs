using InversionOfControl.Interfaces;

namespace InversionOfControl.Services
{
    public class OrderService
    {
        //private readonly EmailNotificationService _notificationService;
        //private readonly SmsNotificationService _notificationService;
        //public OrderService()
        //{
        //    //_notificationService = new EmailNotificationService();
        //    _notificationService = new SmsNotificationService();
        //}

        private readonly InotificationService _notificationService;
        public OrderService(InotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void ProcessOrder()
        {
            Console.WriteLine("Processing order...");
            _notificationService.Notify("Your order has been processed.");
            //_notificationService.SendEmail("Your order has been processed.");
        }
    }
}
