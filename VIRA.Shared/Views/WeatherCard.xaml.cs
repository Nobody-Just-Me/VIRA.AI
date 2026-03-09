using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VIRA.Shared.Models;

namespace VIRA.Shared.Views;

/// <summary>
/// Weather card component that displays weather information in a card format
/// matching the reference design
/// </summary>
public sealed partial class WeatherCard : UserControl
{
    /// <summary>
    /// Dependency property for WeatherData
    /// </summary>
    public static readonly DependencyProperty DataProperty =
        DependencyProperty.Register(
            nameof(Data),
            typeof(WeatherData),
            typeof(WeatherCard),
            new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the weather data to display
    /// </summary>
    public WeatherData Data
    {
        get => (WeatherData)GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    public WeatherCard()
    {
        InitializeComponent();
    }
}
