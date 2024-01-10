#if IOS
using UserNotifications;
#endif

namespace MauiSample.Models.Permissions;
#if ANDROID
public class NotificationPermission : Microsoft.Maui.ApplicationModel.Permissions.BasePlatformPermission
{

    public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
        new List<(string androidPermission, bool isRuntime)>
        {
#if ANDROID21_0_OR_GREATER
#pragma warning disable CA1416 // Validate platform compatibility
            (Android.Manifest.Permission.PostNotifications, true)
#pragma warning restore CA1416 // Validate platform compatibility
#endif
        }.ToArray();
}
#elif IOS
public class NotificationPermission : Microsoft.Maui.ApplicationModel.Permissions.BasePermission
{
    public override Task<PermissionStatus> CheckStatusAsync()
    {
        var taskCompletionSource = new TaskCompletionSource<PermissionStatus>();

        UNUserNotificationCenter.Current.GetNotificationSettings((settings) =>
        {
#pragma warning disable CA1416 // Validate platform compatibility
            var status = settings.AuthorizationStatus switch
            {
                UNAuthorizationStatus.Authorized => PermissionStatus.Granted,
                UNAuthorizationStatus.Denied => PermissionStatus.Denied,
#if IOS11_0_OR_GREATER
                UNAuthorizationStatus.Ephemeral => PermissionStatus.Granted,
#endif
                UNAuthorizationStatus.NotDetermined => PermissionStatus.Unknown,
#if IOS12_0_OR_GREATER
                UNAuthorizationStatus.Provisional => PermissionStatus.Granted,
#endif
                _ => PermissionStatus.Unknown
            };
#pragma warning restore CA1416 // Validate platform compatibility

            taskCompletionSource.SetResult(status);
        });

        return taskCompletionSource.Task;
    }

    public override Task<PermissionStatus> RequestAsync()
    {
        var taskCompletionSource = new TaskCompletionSource<PermissionStatus>();
        UNUserNotificationCenter.Current.RequestAuthorization(
            UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
            (granted, error) => {
                if (!granted)
                {
                    taskCompletionSource.SetResult(PermissionStatus.Denied);
                }
                else
                {
                    taskCompletionSource.SetResult(PermissionStatus.Granted);
                }
            });

        return taskCompletionSource.Task;
    }

    public override void EnsureDeclared()
    {
    }

    public override bool ShouldShowRationale()
    {
        return false;
    }
}
#endif
