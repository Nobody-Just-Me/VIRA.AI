using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using VIRA.Shared.Services;

namespace VIRA.Shared.Views
{
    public sealed partial class SettingsView : Page
    {
        private readonly AIProviderManager _providerManager;
        private readonly PreferencesService _preferencesService;
        private bool _isKeyVisible = false;
        private string _currentProvider = "groq";

        public SettingsView()
        {
            this.InitializeComponent();
            
            // Initialize services
            var storageManager = new SecureStorageManager();
            var httpClient = new HttpClient();
            _providerManager = new AIProviderManager(storageManager);
            _preferencesService = new PreferencesService();
            
            // Register providers
            _providerManager.RegisterProvider(new GroqProvider(httpClient, storageManager));
            _providerManager.RegisterProvider(new OpenAIProvider(httpClient, storageManager));
            _providerManager.RegisterProvider(new GeminiProvider(httpClient, storageManager));
            
            LoadSettings();
        }

        private void LoadSettings()
        {
            // Load active provider
            var activeProvider = _preferencesService.GetActiveProvider();
            if (!string.IsNullOrEmpty(activeProvider))
            {
                _currentProvider = activeProvider.ToLower();
                SelectProviderInComboBox(activeProvider);
            }
            else
            {
                ProviderSelector.SelectedIndex = 0; // Default to Groq
            }
            
            // Load models for current provider
            UpdateModelList();
            
            // Load preferences
            VoiceOutputToggle.IsOn = _preferencesService.GetVoiceOutputEnabled();
            WebBrowsingToggle.IsOn = _preferencesService.GetWebBrowsingEnabled();
            HapticsToggle.IsOn = _preferencesService.GetHapticsEnabled();
            NotificationsToggle.IsOn = _preferencesService.GetNotificationsEnabled();
            MemoryModeToggle.IsOn = _preferencesService.GetMemoryModeEnabled();
            PrivacyModeToggle.IsOn = _preferencesService.GetPrivacyModeEnabled();
            
            // Update status
            UpdateConnectionStatus();
        }

        private void SelectProviderInComboBox(string providerName)
        {
            var normalizedName = providerName.ToLower();
            for (int i = 0; i < ProviderSelector.Items.Count; i++)
            {
                if (ProviderSelector.Items[i] is ComboBoxItem item)
                {
                    var tag = item.Tag?.ToString()?.ToLower();
                    if (tag == normalizedName)
                    {
                        ProviderSelector.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void UpdateModelList()
        {
            ModelSelector.Items.Clear();
            
            var models = GetModelsForProvider(_currentProvider);
            foreach (var model in models)
            {
                ModelSelector.Items.Add(new ComboBoxItem { Content = model });
            }
            
            if (ModelSelector.Items.Count > 0)
            {
                ModelSelector.SelectedIndex = 0;
            }
        }

        private List<string> GetModelsForProvider(string provider)
        {
            return provider.ToLower() switch
            {
                "groq" => new List<string> 
                { 
                    "mixtral-8x7b-32768",
                    "llama2-70b-4096",
                    "gemma-7b-it"
                },
                "openai" => new List<string> 
                { 
                    "gpt-4",
                    "gpt-4-turbo",
                    "gpt-3.5-turbo"
                },
                "gemini" => new List<string> 
                { 
                    "gemini-2.0-flash",
                    "gemini-pro",
                    "gemini-pro-vision"
                },
                _ => new List<string>()
            };
        }

        private void UpdateConnectionStatus()
        {
            var provider = _providerManager.GetProvider(_currentProvider);
            if (provider != null && provider.IsConfigured)
            {
                StatusIndicator.Fill = new SolidColorBrush(
                    Windows.UI.Color.FromArgb(255, 34, 197, 94)); // Green
                StatusText.Text = "Connected";
                StatusText.Foreground = new SolidColorBrush(
                    Windows.UI.Color.FromArgb(255, 34, 197, 94));
            }
            else
            {
                StatusIndicator.Fill = new SolidColorBrush(
                    Windows.UI.Color.FromArgb(255, 148, 163, 184)); // Gray
                StatusText.Text = "Not configured";
                StatusText.Foreground = new SolidColorBrush(
                    Windows.UI.Color.FromArgb(255, 148, 163, 184));
            }
        }

        private void OnBackClick(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void OnProviderSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProviderSelector.SelectedItem is ComboBoxItem item)
            {
                _currentProvider = item.Tag?.ToString()?.ToLower() ?? "groq";
                UpdateModelList();
                UpdateConnectionStatus();
                
                // Load API key for selected provider if available
                var provider = _providerManager.GetProvider(_currentProvider);
                // Note: For security, we don't display the actual API key
                ApiKeyInput.Password = string.Empty;
            }
        }

        private void OnShowHideKeyClick(object sender, RoutedEventArgs e)
        {
            _isKeyVisible = !_isKeyVisible;
            ShowHideKeyButton.Content = _isKeyVisible ? "🙈" : "👁️";
            
            // Note: In a production app, you'd toggle between PasswordBox and TextBox
            // For now, this is a placeholder
        }

        private async void OnTestConnectionClick(object sender, RoutedEventArgs e)
        {
            TestConnectionButton.IsEnabled = false;
            StatusText.Text = "Testing connection...";
            StatusIndicator.Fill = new SolidColorBrush(
                Windows.UI.Color.FromArgb(255, 234, 179, 8)); // Yellow
            
            try
            {
                var provider = _providerManager.GetProvider(_currentProvider);
                if (provider != null)
                {
                    var isValid = await provider.ValidateConnectionAsync();
                    
                    if (isValid)
                    {
                        StatusIndicator.Fill = new SolidColorBrush(
                            Windows.UI.Color.FromArgb(255, 34, 197, 94)); // Green
                        StatusText.Text = "Connection successful!";
                        StatusText.Foreground = new SolidColorBrush(
                            Windows.UI.Color.FromArgb(255, 34, 197, 94));
                    }
                    else
                    {
                        StatusIndicator.Fill = new SolidColorBrush(
                            Windows.UI.Color.FromArgb(255, 239, 68, 68)); // Red
                        StatusText.Text = "Connection failed";
                        StatusText.Foreground = new SolidColorBrush(
                            Windows.UI.Color.FromArgb(255, 239, 68, 68));
                    }
                }
            }
            catch (Exception ex)
            {
                StatusIndicator.Fill = new SolidColorBrush(
                    Windows.UI.Color.FromArgb(255, 239, 68, 68)); // Red
                StatusText.Text = $"Error: {ex.Message}";
                StatusText.Foreground = new SolidColorBrush(
                    Windows.UI.Color.FromArgb(255, 239, 68, 68));
            }
            finally
            {
                TestConnectionButton.IsEnabled = true;
            }
        }

        private async void OnSaveConfigClick(object sender, RoutedEventArgs e)
        {
            SaveConfigButton.IsEnabled = false;
            
            try
            {
                // Save API key if provided
                var apiKey = ApiKeyInput.Password;
                if (!string.IsNullOrWhiteSpace(apiKey))
                {
                    var provider = _providerManager.GetProvider(_currentProvider);
                    if (provider != null)
                    {
                        // Validate API key before saving
                        // In production, this would call the provider's validation method
                        await _preferencesService.SaveApiKeyAsync(_currentProvider, apiKey);
                    }
                }
                
                // Save selected model
                if (ModelSelector.SelectedItem is ComboBoxItem modelItem)
                {
                    var model = modelItem.Content?.ToString();
                    if (!string.IsNullOrEmpty(model))
                    {
                        _preferencesService.SaveSelectedModel(_currentProvider, model);
                    }
                }
                
                // Save active provider
                _preferencesService.SaveActiveProvider(_currentProvider);
                _providerManager.SetActiveProvider(_currentProvider);
                
                // Save preferences
                _preferencesService.SaveVoiceOutputEnabled(VoiceOutputToggle.IsOn);
                _preferencesService.SaveWebBrowsingEnabled(WebBrowsingToggle.IsOn);
                _preferencesService.SaveHapticsEnabled(HapticsToggle.IsOn);
                _preferencesService.SaveNotificationsEnabled(NotificationsToggle.IsOn);
                _preferencesService.SaveMemoryModeEnabled(MemoryModeToggle.IsOn);
                _preferencesService.SavePrivacyModeEnabled(PrivacyModeToggle.IsOn);
                
                // Show success message
                var dialog = new ContentDialog
                {
                    Title = "Settings Saved",
                    Content = "Your settings have been saved successfully.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                
                await dialog.ShowAsync();
                
                UpdateConnectionStatus();
            }
            catch (Exception ex)
            {
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"Failed to save settings: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                
                await dialog.ShowAsync();
            }
            finally
            {
                SaveConfigButton.IsEnabled = true;
            }
        }
    }
}
