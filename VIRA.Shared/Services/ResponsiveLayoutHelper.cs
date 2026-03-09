using Microsoft.UI.Xaml;
using System;

namespace VIRA.Shared.Services
{
    /// <summary>
    /// Helper for responsive layout adjustments
    /// </summary>
    public class ResponsiveLayoutHelper
    {
        public enum ScreenSize
        {
            Small,   // < 5.5"
            Medium,  // 5.5" - 6.5"
            Large    // > 6.5"
        }

        /// <summary>
        /// Gets the screen size category based on window width
        /// </summary>
        public static ScreenSize GetScreenSize(double width)
        {
            if (width < 360)
            {
                return ScreenSize.Small;
            }
            else if (width < 420)
            {
                return ScreenSize.Medium;
            }
            else
            {
                return ScreenSize.Large;
            }
        }

        /// <summary>
        /// Gets scaled font size based on screen size
        /// </summary>
        public static double GetScaledFontSize(double baseFontSize, ScreenSize screenSize)
        {
            return screenSize switch
            {
                ScreenSize.Small => baseFontSize * 0.9,
                ScreenSize.Medium => baseFontSize,
                ScreenSize.Large => baseFontSize * 1.1,
                _ => baseFontSize
            };
        }

        /// <summary>
        /// Gets scaled spacing based on screen size
        /// </summary>
        public static double GetScaledSpacing(double baseSpacing, ScreenSize screenSize)
        {
            return screenSize switch
            {
                ScreenSize.Small => baseSpacing * 0.85,
                ScreenSize.Medium => baseSpacing,
                ScreenSize.Large => baseSpacing * 1.15,
                _ => baseSpacing
            };
        }

        /// <summary>
        /// Adjusts layout for keyboard appearance
        /// </summary>
        public static void AdjustForKeyboard(FrameworkElement element, bool isKeyboardVisible, double keyboardHeight)
        {
            if (element == null) return;

            if (isKeyboardVisible)
            {
                // Move element up by keyboard height
                element.Margin = new Thickness(
                    element.Margin.Left,
                    element.Margin.Top,
                    element.Margin.Right,
                    keyboardHeight);
            }
            else
            {
                // Reset margin
                element.Margin = new Thickness(
                    element.Margin.Left,
                    element.Margin.Top,
                    element.Margin.Right,
                    0);
            }
        }

        /// <summary>
        /// Maintains aspect ratio for an element
        /// </summary>
        public static void MaintainAspectRatio(FrameworkElement element, double aspectRatio, double maxWidth)
        {
            if (element == null) return;

            var width = Math.Min(element.ActualWidth, maxWidth);
            var height = width / aspectRatio;

            element.Width = width;
            element.Height = height;
        }
    }
}
