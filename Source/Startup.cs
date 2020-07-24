using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Otc.AspNetCore.ApiBoot;
using Otc.Messaging.Subscriber.HW;
using System.Diagnostics.CodeAnalysis;

namespace SubscriberHostedWorker.Template
{
    public class Startup : SubscriberStartup<Pesquisa, MessageHandler>
    {
        protected override ApiMetadata ApiMetadata => new ApiMetadata()
        {
            Name = "SubscriberHostedWorker.Template",
            Description = "This project is based in Otc.Messaging.Subscriber.HW package " +
                "(https://github.com/OleConsignado/otc-messaging/).",
            DefaultApiVersion = "1.0"
        };

        public Startup(IConfiguration configuration)
            : base(configuration)
        {
        }

        //
        // IMPORTANT
        // You should not override ConfigureApiServices method,
        // implement ConfigureSubscriberServices in order to register worker dependencies.
        //

        /// <summary>
        /// Worker dependencies registration.
        /// </summary>
        /// <param name="services"></param>
        [ExcludeFromCodeCoverage]
        protected override void ConfigureSubscriberServices(
            IServiceCollection services)
        {
        }
    }
}
