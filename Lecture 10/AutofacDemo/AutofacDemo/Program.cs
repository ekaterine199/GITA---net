using Autofac;

namespace AutofacDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 1️⃣ Autofac-ის კონტეინერის შექმნა
            var builder = new ContainerBuilder();

            // 2️⃣ IMessageService-ის რეგისტრაცია (შეგვიძლია ჩავანაცვლოთ EmailMessageService -> SmsMessageService)
            builder.RegisterType<EmailMessageService>().As<IMessageService>();

            // 3️⃣ კონტეინერის აგება
            var container = builder.Build();

            // 4️⃣ IMessageService-ის მოთხოვნა
            using (var scope = container.BeginLifetimeScope())
            {
                var messageService = scope.Resolve<IMessageService>();

                // 5️⃣ მეთოდის გამოძახება
                messageService.SendMessage("Hello from Autofac!");
            }

        }
    }
}
