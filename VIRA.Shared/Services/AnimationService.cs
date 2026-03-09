using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;

namespace VIRA.Shared.Services;

/// <summary>
/// Provides smooth animation methods for UI elements.
/// All animations match the reference design timing and easing functions.
/// </summary>
public class AnimationService
{
    /// <summary>
    /// Fades in an element with cubic ease-out (280ms)
    /// </summary>
    public static async Task FadeInAsync(UIElement element, double duration = 280)
    {
        var animation = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(duration),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        
        Storyboard.SetTarget(animation, element);
        Storyboard.SetTargetProperty(animation, "Opacity");
        
        var storyboard = new Storyboard();
        storyboard.Children.Add(animation);
        storyboard.Begin();
        
        await Task.Delay((int)duration);
    }
    
    /// <summary>
    /// Slides in an element from bottom with fade (280ms, 12px distance)
    /// </summary>
    public static async Task SlideInFromBottomAsync(UIElement element, double distance = 12)
    {
        var translateAnimation = new DoubleAnimation
        {
            From = distance,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(280),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        
        var opacityAnimation = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(280)
        };
        
        var transform = new TranslateTransform();
        element.RenderTransform = transform;
        
        Storyboard.SetTarget(translateAnimation, transform);
        Storyboard.SetTargetProperty(translateAnimation, "Y");
        Storyboard.SetTarget(opacityAnimation, element);
        Storyboard.SetTargetProperty(opacityAnimation, "Opacity");
        
        var storyboard = new Storyboard();
        storyboard.Children.Add(translateAnimation);
        storyboard.Children.Add(opacityAnimation);
        storyboard.Begin();
        
        await Task.Delay(280);
    }
    
    /// <summary>
    /// Applies pulsing animation to an element (500ms, sine ease, infinite repeat)
    /// </summary>
    public static void PulseAsync(UIElement element)
    {
        var scaleXAnimation = new DoubleAnimation
        {
            From = 1.0,
            To = 1.1,
            Duration = TimeSpan.FromMilliseconds(500),
            AutoReverse = true,
            RepeatBehavior = RepeatBehavior.Forever,
            EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
        };
        
        var scaleYAnimation = new DoubleAnimation
        {
            From = 1.0,
            To = 1.1,
            Duration = TimeSpan.FromMilliseconds(500),
            AutoReverse = true,
            RepeatBehavior = RepeatBehavior.Forever,
            EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
        };
        
        var transform = new ScaleTransform 
        { 
            CenterX = 0.5, 
            CenterY = 0.5 
        };
        element.RenderTransform = transform;
        
        Storyboard.SetTarget(scaleXAnimation, transform);
        Storyboard.SetTargetProperty(scaleXAnimation, "ScaleX");
        Storyboard.SetTarget(scaleYAnimation, transform);
        Storyboard.SetTargetProperty(scaleYAnimation, "ScaleY");
        
        var storyboard = new Storyboard();
        storyboard.Children.Add(scaleXAnimation);
        storyboard.Children.Add(scaleYAnimation);
        storyboard.Begin();
    }
    
    /// <summary>
    /// Stops pulsing animation on an element
    /// </summary>
    public static void StopPulse(UIElement element)
    {
        if (element.RenderTransform is ScaleTransform transform)
        {
            transform.ScaleX = 1.0;
            transform.ScaleY = 1.0;
        }
    }
    
    /// <summary>
    /// Applies scale animation for button taps (200ms)
    /// </summary>
    public static async Task ScaleAnimationAsync(UIElement element, double fromScale = 1.0, double toScale = 0.95)
    {
        var scaleXAnimation = new DoubleAnimation
        {
            From = fromScale,
            To = toScale,
            Duration = TimeSpan.FromMilliseconds(100),
            AutoReverse = true,
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        
        var scaleYAnimation = new DoubleAnimation
        {
            From = fromScale,
            To = toScale,
            Duration = TimeSpan.FromMilliseconds(100),
            AutoReverse = true,
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        
        var transform = element.RenderTransform as ScaleTransform;
        if (transform == null)
        {
            transform = new ScaleTransform { CenterX = 0.5, CenterY = 0.5 };
            element.RenderTransform = transform;
        }
        
        Storyboard.SetTarget(scaleXAnimation, transform);
        Storyboard.SetTargetProperty(scaleXAnimation, "ScaleX");
        Storyboard.SetTarget(scaleYAnimation, transform);
        Storyboard.SetTargetProperty(scaleYAnimation, "ScaleY");
        
        var storyboard = new Storyboard();
        storyboard.Children.Add(scaleXAnimation);
        storyboard.Children.Add(scaleYAnimation);
        storyboard.Begin();
        
        await Task.Delay(200);
    }
    
    /// <summary>
    /// Slides in an element from left (300ms, for sidebar)
    /// </summary>
    public static async Task SlideInFromLeftAsync(UIElement element, double distance = 300)
    {
        var slideAnimation = new DoubleAnimation
        {
            From = -distance,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(300),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        
        var transform = new TranslateTransform();
        element.RenderTransform = transform;
        
        Storyboard.SetTarget(slideAnimation, transform);
        Storyboard.SetTargetProperty(slideAnimation, "X");
        
        var storyboard = new Storyboard();
        storyboard.Children.Add(slideAnimation);
        storyboard.Begin();
        
        await Task.Delay(300);
    }
    
    /// <summary>
    /// Slides out an element to left (300ms, for sidebar)
    /// </summary>
    public static async Task SlideOutToLeftAsync(UIElement element, double distance = 300)
    {
        var slideAnimation = new DoubleAnimation
        {
            From = 0,
            To = -distance,
            Duration = TimeSpan.FromMilliseconds(300),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
        };
        
        var transform = element.RenderTransform as TranslateTransform;
        if (transform == null)
        {
            transform = new TranslateTransform();
            element.RenderTransform = transform;
        }
        
        Storyboard.SetTarget(slideAnimation, transform);
        Storyboard.SetTargetProperty(slideAnimation, "X");
        
        var storyboard = new Storyboard();
        storyboard.Children.Add(slideAnimation);
        storyboard.Begin();
        
        await Task.Delay(300);
    }
}
