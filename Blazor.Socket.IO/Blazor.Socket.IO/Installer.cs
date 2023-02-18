using Microsoft.Extensions.DependencyInjection;

namespace Blazor.Socket.IO
{
    public static class Installer
    {
        public static IServiceCollection AddBlazorSocketIo(this IServiceCollection serviceProvider)
        {
            serviceProvider.AddScoped<JsCallback>();

            return serviceProvider;
        }
    }
}
