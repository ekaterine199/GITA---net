using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjectDemo
{
    public class MessageModule : Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
            Bind<IMessageService>().To<EmailMessageService>();
        }
    }
}
