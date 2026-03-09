using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using System;

namespace VIRA.Shared.Views
{
    /// <summary>
    /// Dialog for explaining permission rationale to users
    /// </summary>
    public class PermissionRationaleDialog
    {
        /// <summary>
        /// Shows a permission rationale dialog
        /// </summary>
        /// <param name="permissionName">Name of the permission (e.g., "Microphone", "Contacts")</param>
        /// <param name="rationale">Explanation of why the permission is needed</param>
        /// <param name="xamlRoot">XamlRoot for the dialog</param>
        /// <returns>True if user granted permission, false otherwise</returns>
        public static async System.Threading.Tasks.Task<bool> ShowAsync(
            string permissionName, 
            string rationale, 
            XamlRoot xamlRoot)
        {
            var dialog = new ContentDialog
            {
                Title = $"{permissionName} Permission Required",
                Content = CreateContent(rationale),
                PrimaryButtonText = "Grant Permission",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = xamlRoot
            };

            // Apply custom styling
            dialog.Background = new SolidColorBrush(
                Windows.UI.Color.FromArgb(255, 26, 31, 46)); // #1a1f2e
            dialog.Foreground = new SolidColorBrush(
                Windows.UI.Color.FromArgb(255, 241, 245, 249)); // #F1F5F9
            dialog.BorderBrush = new SolidColorBrush(
                Windows.UI.Color.FromArgb(36, 255, 255, 255)); // #FFFFFF14
            dialog.CornerRadius = new CornerRadius(16);

            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }

        private static UIElement CreateContent(string rationale)
        {
            var stackPanel = new StackPanel
            {
                Spacing = 16
            };

            // Icon
            var iconText = new TextBlock
            {
                Text = "🔒",
                FontSize = 48,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 8, 0, 0)
            };
            stackPanel.Children.Add(iconText);

            // Rationale text
            var rationaleText = new TextBlock
            {
                Text = rationale,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 15,
                LineHeight = 22,
                Foreground = new SolidColorBrush(
                    Windows.UI.Color.FromArgb(255, 203, 213, 225)), // #cbd5e1
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                MaxWidth = 400
            };
            stackPanel.Children.Add(rationaleText);

            return stackPanel;
        }
    }
}
