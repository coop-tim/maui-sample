using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Plugin.Firebase.CloudMessaging;

namespace MauiSample;
[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public static bool AppCenterConfigured { get; set; }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        var intent = this.Intent;

        HandleIntent(Intent);
        CreateNotificationChannelIfNeeded();

        base.OnCreate(savedInstanceState);

    }

    protected override void OnNewIntent(Intent? intent)
    {
        base.OnNewIntent(intent);
        HandleIntent(intent);
    }

    private static void HandleIntent(Intent? intent)
    {
        if (intent is not null)
        {
            FirebaseCloudMessagingImplementation.OnNewIntent(intent);
        }
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
#if ANDROID23_0_OR_GREATER
#pragma warning disable CA1416 // Validate platform compatibility
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
#pragma warning restore CA1416 // Validate platform compatibility
#endif
    }

    private void CreateNotificationChannelIfNeeded()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            CreateNotificationChannel();
        }
    }

    private void CreateNotificationChannel()
    {
        var channelId = $"{PackageName}.general";
#if ANDROID26_0_OR_GREATER
        var notificationManager = GetSystemService(NotificationService) as NotificationManager;
        if (notificationManager is not null)
        {
            if ((int)Build.VERSION.SdkInt > 26)
            {
#pragma warning disable CA1416 // Validate platform compatibility
                var channel = new NotificationChannel(channelId, "General", NotificationImportance.Default);
                notificationManager.CreateNotificationChannel(channel);
#pragma warning restore CA1416 // Validate platform compatibility
            }
        }

        FirebaseCloudMessagingImplementation.ChannelId = channelId;
#endif
        //FirebaseCloudMessagingImplementation.SmallIconRef = Resource.Drawable.ic_push_small;
    }
}
