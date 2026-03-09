using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;

namespace VIRA.Shared.Behaviors;

/// <summary>
/// Behavior that fades in an element when it loads
/// </summary>
public class FadeInBehavior : Behavior<FrameworkElement>
{
    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(
            nameof(Duration),
            typeof(double),
            typeof(FadeInBehavior),
            new PropertyMetadata(280.0));

    public double Duration
    {
        get => (double)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
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
        await Services.AnimationService.FadeInAsync(AssociatedObject, Duration);
    }
}
