using CommunityToolkit.Mvvm.ComponentModel;
using MauiSample.Models.Permissions;
using Microsoft.AppCenter.Crashes;
using Plugin.Firebase.CloudMessaging;

namespace MauiSample.ViewModels;
public partial class MainPageViewModel(ILogger<MainPageViewModel> logger) : ObservableObject
{
    private readonly ILogger<MainPageViewModel> _logger = logger;

    [ObservableProperty]
    private string _token = string.Empty;

    public async Task LoadAsync()
    {
        try
        {
            if (!CrossFirebaseCloudMessaging.IsSupported)
            {
                _logger.LogWarning("Firebase messaging not supported on this device");
                return;
            }

            var status = await Permissions.CheckStatusAsync<NotificationPermission>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<NotificationPermission>();
            }
            await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
            var firebaseToken = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();

            if (!string.IsNullOrEmpty(firebaseToken))
            {
                _logger.LogInformation("Firebase token {token}", firebaseToken);

                Token = firebaseToken;
            }
            else
            {
                throw new InvalidOperationException("Firebase token could not be retrieved");
            }

        }
        catch (Exception ex)
        {
            Crashes.TrackError(ex);
            _logger.LogError(ex, "Error getting firebase token: {message}", ex.Message);
        }
    }
}
