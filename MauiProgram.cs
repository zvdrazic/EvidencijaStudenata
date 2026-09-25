using Microsoft.Extensions.Logging;

namespace EvidencijaStudenata;

/// <summary>
/// Ulazna točka za konfiguraciju i pokretanje .NET MAUI aplikacije, odgovara
/// opisu iz poglavlja 4.3. završnog rada ("Arhitektura i struktura projekta").
/// </summary>
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
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}