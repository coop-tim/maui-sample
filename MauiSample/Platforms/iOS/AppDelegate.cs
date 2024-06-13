using Foundation;

namespace MauiSample;
[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    private ILogger<AppDelegate>? _logger;

    protected override MauiApp CreateMauiApp()
    {
        var app = MauiProgram.CreateMauiApp();

        _logger = app.Services.GetRequiredService<ILogger<AppDelegate>>();

        return app;
    }
}
