using Camera.MAUI;
using Microsoft.Extensions.Logging;
using Trak_IT.Scripts;
namespace Trak_IT
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCameraView()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Poppins-SemiBold.ttf", "PoppinsSemibold");
                    fonts.AddFont("Poppins-Medium.ttf", "PoppinsMedium");
                    fonts.AddFont("Poppins-Bold.ttf", "PoppinsBold");
                });
            builder.Services.AddSingleton<LocalDBServer>();
            builder.Services.AddSingleton<UsersProfile>();
            builder.Services.AddTransient<Register>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<MainPage>();
#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
