using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using XamlListView = Microsoft.UI.Xaml.Controls.ListView;
using XamlOrientation = Microsoft.UI.Xaml.Controls.Orientation;

namespace VIRA.Shared.Views
{
    /// <summary>
    /// Contact information for disambiguation
    /// </summary>
    public class ContactInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
    }

    /// <summary>
    /// Dialog for disambiguating between multiple matching contacts
    /// </summary>
    public class ContactDisambiguationDialog
    {
        /// <summary>
        /// Shows a contact disambiguation dialog
        /// </summary>
        /// <param name="contactName">Name of the contact being searched</param>
        /// <param name="matches">List of matching contacts</param>
        /// <param name="xamlRoot">XamlRoot for the dialog</param>
        /// <returns>Selected contact or null if cancelled</returns>
        public static async System.Threading.Tasks.Task<ContactInfo?> ShowAsync(
            string contactName,
            List<ContactInfo> matches,
            XamlRoot xamlRoot)
        {
            ContactInfo? selectedContact = null;

            var dialog = new ContentDialog
            {
                Title = $"Multiple contacts found for \"{contactName}\"",
                Content = CreateContent(matches, contact => selectedContact = contact),
                PrimaryButtonText = "Select",
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
            
            return result == ContentDialogResult.Primary ? selectedContact : null;
        }

        private static UIElement CreateContent(List<ContactInfo> matches, Action<ContactInfo> onContactSelected)
        {
            var stackPanel = new StackPanel
            {
                Spacing = 12,
                MaxWidth = 400
            };

            // Instruction text
            var instructionText = new TextBlock
            {
                Text = "Please select which contact you meant:",
                FontSize = 14,
                Foreground = new SolidColorBrush(
                    Windows.UI.Color.FromArgb(255, 148, 163, 184)), // #94a3b8
                Margin = new Thickness(0, 0, 0, 8)
            };
            stackPanel.Children.Add(instructionText);

            // Contact list
            var listView = new XamlListView
            {
                SelectionMode = ListViewSelectionMode.Single,
                MaxHeight = 300
            };

            foreach (var contact in matches)
            {
                var contactItem = CreateContactItem(contact);
                listView.Items.Add(contactItem);
            }

            listView.SelectionChanged += (s, e) =>
            {
                if (listView.SelectedIndex >= 0 && listView.SelectedIndex < matches.Count)
                {
                    onContactSelected(matches[listView.SelectedIndex]);
                }
            };

            // Select first item by default
            if (matches.Any())
            {
                listView.SelectedIndex = 0;
                onContactSelected(matches[0]);
            }

            stackPanel.Children.Add(listView);

            return stackPanel;
        }

        private static Border CreateContactItem(ContactInfo contact)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(
                    Windows.UI.Color.FromArgb(13, 255, 255, 255)), // #FFFFFF0D
                BorderBrush = new SolidColorBrush(
                    Windows.UI.Color.FromArgb(20, 255, 255, 255)), // #FFFFFF14
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(12),
                Margin = new Thickness(0, 4)
            };

            var stackPanel = new StackPanel
            {
                Spacing = 4
            };

            // Contact name
            var nameText = new TextBlock
            {
                Text = contact.Name,
                FontSize = 15,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                Foreground = new SolidColorBrush(
                    Windows.UI.Color.FromArgb(255, 241, 245, 249)) // #F1F5F9
            };
            stackPanel.Children.Add(nameText);

            // Phone and label
            var phonePanel = new StackPanel
            {
                Orientation = XamlOrientation.Horizontal,
                Spacing = 8
            };

            var phoneText = new TextBlock
            {
                Text = contact.Phone,
                FontSize = 13,
                Foreground = new SolidColorBrush(
                    Windows.UI.Color.FromArgb(255, 203, 213, 225)) // #cbd5e1
            };
            phonePanel.Children.Add(phoneText);

            if (!string.IsNullOrEmpty(contact.Label))
            {
                var labelBorder = new Border
                {
                    Background = new SolidColorBrush(
                        Windows.UI.Color.FromArgb(255, 139, 92, 246)), // #8b5cf6
                    CornerRadius = new CornerRadius(4),
                    Padding = new Thickness(6, 2)
                };

                var labelText = new TextBlock
                {
                    Text = contact.Label,
                    FontSize = 11,
                    Foreground = new SolidColorBrush(
                        Windows.UI.Color.FromArgb(255, 255, 255, 255))
                };

                labelBorder.Child = labelText;
                phonePanel.Children.Add(labelBorder);
            }

            stackPanel.Children.Add(phonePanel);
            border.Child = stackPanel;

            return border;
        }
    }
}
