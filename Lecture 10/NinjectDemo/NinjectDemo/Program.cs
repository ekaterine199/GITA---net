using Ninject;

namespace NinjectDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            IKernel kernel = new StandardKernel(new MessageModule());

            IMessageService messageService = kernel.Get<IMessageService>();

            messageService.SendMessage("Hello from Ninject!");


        }
    }
}
