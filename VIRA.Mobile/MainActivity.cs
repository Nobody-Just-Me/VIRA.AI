using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace VIRA.Mobile;

[Activity(
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden,
    WindowSoftInputMode = SoftInput.AdjustResize,
    Theme = "@style/AppTheme")]
public class MainActivity : Microsoft.UI.Xaml.ApplicationActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        // Request microphone permission at runtime (Android 6.0+)
        if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
        {
            RequestPermissions(new[] { Android.Manifest.Permission.RecordAudio }, 1);
        }

        // Set status bar color to match app theme
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
        {
            Window?.SetStatusBarColor(Android.Graphics.Color.ParseColor("#101622"));
        }
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        
        // Handle speech recognition result
        if (requestCode == 10)
        {
            var voiceService = Shared.App.Current.Services.GetService(typeof(Shared.Services.IVoiceService)) 
                as Services.AndroidVoiceService;
            voiceService?.HandleActivityResult(requestCode, resultCode, data);
        }
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        
        if (requestCode == 1)
        {
            if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
            {
                // Microphone permission granted
                System.Diagnostics.Debug.WriteLine("Microphone permission granted");
            }
            else
            {
                // Permission denied
                System.Diagnostics.Debug.WriteLine("Microphone permission denied");
            }
        }
    }
}
