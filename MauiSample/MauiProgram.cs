using System.Reflection;
using Plugin.Firebase.CloudMessaging;
using Plugin.Firebase.Analytics;
using Microsoft.Maui.LifecycleEvents;

#if IOS
using Plugin.Firebase.Core.Platforms.iOS;
using UIKit;
#elif ANDROID
using Plugin.Firebase.Core.Platforms.Android;
#endif

namespace MauiSample;
public static class MauiProgram
{
    static IServiceProvider? _serviceProvider;

    public static TService GetService<TService>()
        where TService : notnull
        => _serviceProvider is not null ? (_serviceProvider.GetRequiredService<TService>()) : throw new Exception("Service provider not registered yet");

    public static MauiApp CreateMauiApp()
    {
        using var stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("MauiSample.appsettings.json") ?? throw new Exception("appsettings.json not found");

        var configBuilder = new ConfigurationBuilder()
            .AddJsonStream(stream);

#if ANDROID
        using var androidStream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("MauiSample.appsettings.Android.json") ?? throw new Exception("appsettings.Android.json not found");
        configBuilder = configBuilder.AddJsonStream(androidStream);
#endif
#if IOS
        using var iosStream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("MauiSample.appsettings.Ios.json") ?? throw new Exception("appsettings.Ios.json not found");
        configBuilder = configBuilder.AddJsonStream(iosStream);
#endif
#if !DEBUG
        using var releaseStream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("MauiSample.appsettings.Release.json") ?? throw new Exception("appsettings.Release.json not found");
        configBuilder = configBuilder.AddJsonStream(releaseStream);
#endif
#if DEBUG
        using var debugStream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("MauiSample.appsettings.Debug.json") ?? throw new Exception("appsettings.Debug.json not found");
        configBuilder = configBuilder.AddJsonStream(debugStream);
#endif
        var config = configBuilder.Build();

        var builder = MauiApp.CreateBuilder();

        builder.Configuration.AddConfiguration(config);

        builder
            .UseMauiApp<App>()
            .RegisterFirebaseServices()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif

        builder.Services
            .AddSingleton<AppShell>()
            .AddViews()
            .AddViewModels()
            .AddCustomServices(config);

        var app = builder.Build();
        _serviceProvider = app.Services;

        return app;
    }

    private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
    {
        builder.ConfigureLifecycleEvents(events => {
#if IOS
            events.AddiOS(iOS => iOS.WillFinishLaunching((app, launchOptions) => {
                CrossFirebase.Initialize();
                FirebaseCloudMessagingImplementation.Initialize();
                return true;
            }));
#elif ANDROID
            events.AddAndroid(android => android.OnCreate((activity, _) =>
            {
                CrossFirebase.Initialize(activity);
                FirebaseAnalyticsImplementation.Initialize(activity);
            }));
#endif
        });

        CrossFirebaseCloudMessaging.Current.NotificationTapped += Current_NotificationTapped;


        return builder;
    }

    private static void Current_NotificationTapped(object? sender, Plugin.Firebase.CloudMessaging.EventArgs.FCMNotificationTappedEventArgs e)
    {
        var notificationData = e.Notification.Data;
        if (notificationData.ContainsKey("link"))
        {
            //Do something with the link
        }
    }
}
