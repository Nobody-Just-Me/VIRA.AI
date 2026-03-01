using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VIRA.Shared.ViewModels;

namespace VIRA.Shared.Views;

public sealed partial class MainPage : Page
{
    public MainChatViewModel ViewModel { get; }

    public MainPage()
    {
        this.InitializeComponent();
        
        // ViewModel will be injected via DI in production
        // For now, create a simple instance
        ViewModel = App.Current.Services.GetService<MainChatViewModel>();
    }

    private void OnMenuClick(object sender, RoutedEventArgs e)
    {
        // Show menu or clear chat confirmation
        ViewModel.ClearChatCommand.Execute(null);
    }

    private void OnSettingsClick(object sender, RoutedEventArgs e)
    {
        // Navigate to settings page
        Frame.Navigate(typeof(SettingsPage));
    }
}
