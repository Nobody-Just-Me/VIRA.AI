using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;

namespace VIRA.Shared.Behaviors;

/// <summary>
/// Behavior that applies pulsing animation to an element (for voice button)
/// </summary>
public class PulseBehavior : Behavior<FrameworkElement>
{
    public static readonly DependencyProperty IsActiveProperty =
        DependencyProperty.Register(
            nameof(IsActive),
            typeof(bool),
            typeof(PulseBehavior),
            new PropertyMetadata(false, OnIsActiveChanged));

    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PulseBehavior behavior && behavior.AssociatedObject != null)
        {
            if ((bool)e.NewValue)
            {
                Services.AnimationService.PulseAsync(behavior.AssociatedObject);
            }
            else
            {
                Services.AnimationService.StopPulse(behavior.AssociatedObject);
            }
        }
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        if (IsActive)
        {
            Services.AnimationService.PulseAsync(AssociatedObject);
        }
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        Services.AnimationService.StopPulse(AssociatedObject);
    }
}
