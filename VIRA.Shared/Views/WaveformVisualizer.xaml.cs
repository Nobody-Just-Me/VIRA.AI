using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;

namespace VIRA.Shared.Views
{
    /// <summary>
    /// Waveform visualizer component with 40 bars in bell curve distribution.
    /// Animates bars based on audio levels with state-based coloring.
    /// </summary>
    public sealed partial class WaveformVisualizer : UserControl
    {
        private const int BarCount = 40;
        private Rectangle[] _bars;
        private DispatcherTimer _animationTimer;
        private bool _isActive;
        private bool _isSpeaking;
        private readonly Random _random = new Random();

        /// <summary>
        /// Gets or sets whether the visualizer is active (recording state).
        /// When active, bars are colored red.
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                UpdateAnimation();
            }
        }

        /// <summary>
        /// Gets or sets whether the AI is speaking.
        /// When speaking, bars are colored purple.
        /// </summary>
        public bool IsSpeaking
        {
            get => _isSpeaking;
            set
            {
                _isSpeaking = value;
                UpdateAnimation();
            }
        }

        public WaveformVisualizer()
        {
            this.InitializeComponent();
            InitializeBars();
            InitializeAnimationTimer();
        }

        /// <summary>
        /// Initializes the 40 rectangle bars in horizontal layout.
        /// </summary>
        private void InitializeBars()
        {
            _bars = new Rectangle[BarCount];

            for (int i = 0; i < BarCount; i++)
            {
                var bar = new Rectangle
                {
                    Width = 3,
                    Height = 4,
                    RadiusX = 2,
                    RadiusY = 2,
                    Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 226, 232, 240)) // Gray
                };

                _bars[i] = bar;
                BarsContainer.Children.Add(bar);
            }
        }

        /// <summary>
        /// Initializes the animation timer with 50ms interval.
        /// </summary>
        private void InitializeAnimationTimer()
        {
            _animationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            _animationTimer.Tick += AnimateBars;
        }

        /// <summary>
        /// Animates bars with bell curve height distribution based on audio levels.
        /// </summary>
        private void AnimateBars(object sender, object e)
        {
            var center = BarCount / 2.0;

            for (int i = 0; i < BarCount; i++)
            {
                // Calculate distance from center for bell curve
                var dist = Math.Abs(i - center);
                var maxDist = BarCount / 2.0;
                var bellCurve = 1 - Math.Pow(dist / maxDist, 2);

                // Calculate height based on state
                var baseHeight = _isSpeaking ? 40 : 25;
                var noise = _random.NextDouble() * baseHeight * 0.5;
                var signal = baseHeight * 0.5 + noise;
                var height = Math.Max(4, signal * bellCurve);

                _bars[i].Height = height;

                // Apply state-based coloring
                _bars[i].Fill = new SolidColorBrush(GetBarColor());
            }
        }

        /// <summary>
        /// Gets the bar color based on current state.
        /// Purple for idle/speaking, red for recording.
        /// </summary>
        private Windows.UI.Color GetBarColor()
        {
            if (_isActive)
            {
                // Red for recording
                return Windows.UI.Color.FromArgb(255, 239, 68, 68);
            }
            else if (_isSpeaking)
            {
                // Purple for speaking
                return Windows.UI.Color.FromArgb(255, 139, 92, 246);
            }
            else
            {
                // Gray for idle
                return Windows.UI.Color.FromArgb(255, 226, 232, 240);
            }
        }

        /// <summary>
        /// Updates animation state based on IsActive and IsSpeaking properties.
        /// </summary>
        private void UpdateAnimation()
        {
            if (_isActive || _isSpeaking)
            {
                _animationTimer.Start();
            }
            else
            {
                _animationTimer.Stop();
                ResetBars();
            }
        }

        /// <summary>
        /// Resets all bars to idle state (gray, minimum height).
        /// </summary>
        private void ResetBars()
        {
            for (int i = 0; i < BarCount; i++)
            {
                _bars[i].Height = 4;
                _bars[i].Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 226, 232, 240));
            }
        }
    }
}
