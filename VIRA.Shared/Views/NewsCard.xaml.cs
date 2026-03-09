using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VIRA.Shared.Models;
using System.Collections.Generic;

namespace VIRA.Shared.Views;

/// <summary>
/// News card component that displays news items with category badges
/// matching the reference design
/// </summary>
public sealed partial class NewsCard : UserControl
{
    /// <summary>
    /// Dependency property for NewsItems
    /// </summary>
    public static readonly DependencyProperty NewsItemsProperty =
        DependencyProperty.Register(
            nameof(NewsItems),
            typeof(List<NewsItem>),
            typeof(NewsCard),
            new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the news items to display
    /// </summary>
    public List<NewsItem> NewsItems
    {
        get => (List<NewsItem>)GetValue(NewsItemsProperty);
        set => SetValue(NewsItemsProperty, value);
    }

    public NewsCard()
    {
        InitializeComponent();
    }
}
