using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VIRA.Shared.Models;
using System.Collections.Generic;

namespace VIRA.Shared.Views;

/// <summary>
/// Schedule card component that displays schedule items with colored indicators
/// matching the reference design
/// </summary>
public sealed partial class ScheduleCard : UserControl
{
    /// <summary>
    /// Dependency property for ScheduleItems
    /// </summary>
    public static readonly DependencyProperty ScheduleItemsProperty =
        DependencyProperty.Register(
            nameof(ScheduleItems),
            typeof(List<ScheduleItem>),
            typeof(ScheduleCard),
            new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the schedule items to display
    /// </summary>
    public List<ScheduleItem> ScheduleItems
    {
        get => (List<ScheduleItem>)GetValue(ScheduleItemsProperty);
        set => SetValue(ScheduleItemsProperty, value);
    }

    public ScheduleCard()
    {
        InitializeComponent();
    }
}
