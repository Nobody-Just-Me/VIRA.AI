using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VIRA.Shared.Models;
using System.Collections.Generic;

namespace VIRA.Shared.Views;

/// <summary>
/// Traffic card component that displays traffic routes with color-coded status indicators
/// matching the reference design
/// </summary>
public sealed partial class TrafficCard : UserControl
{
    /// <summary>
    /// Dependency property for TrafficRoutes
    /// </summary>
    public static readonly DependencyProperty TrafficRoutesProperty =
        DependencyProperty.Register(
            nameof(TrafficRoutes),
            typeof(List<TrafficRoute>),
            typeof(TrafficCard),
            new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the traffic routes to display
    /// </summary>
    public List<TrafficRoute> TrafficRoutes
    {
        get => (List<TrafficRoute>)GetValue(TrafficRoutesProperty);
        set => SetValue(TrafficRoutesProperty, value);
    }

    public TrafficCard()
    {
        InitializeComponent();
    }
}
