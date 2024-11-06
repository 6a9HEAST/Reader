using Microsoft.Extensions.Logging;
using Reader.Services;
using Reader.Views;
using Reader.Models;
using Reader.ViewModels;

namespace Reader
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .Services.AddSingleton<IDataStore<Book>, BookDataStore>()
                .AddTransient<MainViewModel>()
                .AddTransient<ReadViewModel>()
                .AddSingleton<MainView>()
                .AddSingleton<ReadView>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
