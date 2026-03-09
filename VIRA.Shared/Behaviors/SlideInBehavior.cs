using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;

namespace VIRA.Shared.Behaviors;

/// <summary>
/// Behavior that slides in an element from bottom when it loads
/// </summary>
public class SlideInBehavior : Behavior<FrameworkElement>
{
    public static readonly DependencyProperty DistanceProperty =
        DependencyProperty.Register(
            nameof(Distance),
            typeof(double),
            typeof(SlideInBehavior),
            new PropertyMetadata(12.0));

    public double Distance
    {
        get => (double)GetValue(DistanceProperty);
        set => SetValue(DistanceProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Loaded += OnLoaded;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.Loaded -= OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await Services.AnimationService.SlideInFromBottomAsync(AssociatedObject, Distance);
    }
}
