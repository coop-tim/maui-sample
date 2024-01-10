using Foundation;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;

namespace MauiSample;
[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    private ILogger<AppDelegate>? _logger;

    protected override MauiApp CreateMauiApp()
    {
        var app = MauiProgram.CreateMauiApp();

        _logger = app.Services.GetRequiredService<ILogger<AppDelegate>>();

        var config = MauiProgram.GetService<IConfiguration>();
        var secret = config.GetSection("AppCenter:Secret")?.Value;
        if (!string.IsNullOrEmpty(secret))
        {
            AppCenter.Configure(secret);

            //AppCenter.LogLevel = Microsoft.AppCenter.LogLevel.Verbose;
            AppCenter.Start(typeof(Crashes), typeof(Analytics));
        }

        return app;
    }
}
