using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SendMailFunction;

[assembly: FunctionsStartup(typeof(Startup))]

namespace SendMailFunction {
    public class Startup : FunctionsStartup {
        public override void Configure(IFunctionsHostBuilder builder) {
            builder.Services.AddSingleton<IEmailSender, EmailSender>();
        }
    }
}
