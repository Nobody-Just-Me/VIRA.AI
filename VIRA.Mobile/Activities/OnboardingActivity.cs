using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics.Drawables;

namespace VIRA.Mobile.Activities;

[Activity(Label = "Welcome to VIRA", Theme = "@style/AppTheme")]
public class OnboardingActivity : Activity
{
    private int _currentStep = 0;
    private LinearLayout? _contentContainer;
    private Button? _nextButton;
    private Button? _skipButton;
    private LinearLayout? _dotsContainer;
    
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
        {
            Window?.SetStatusBarColor(Android.Graphics.Color.ParseColor("#101622"));
        }
        
        BuildUI();
        ShowStep(_currentStep);
    }
    
    private void BuildUI()
    {
        var rootLayout = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent)
        };
        rootLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#101622"));
        rootLayout.SetPadding(48, 48, 48, 48);
        
        // Content container (will be replaced for each step)
        _contentContainer = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                0)
            {
                Weight = 1
            }
        };
        _contentContainer.SetGravity(GravityFlags.Center);
        rootLayout.AddView(_contentContainer);
        
        // Dots indicator
        _dotsContainer = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                BottomMargin = 32
            }
        };
        _dotsContainer.SetGravity(GravityFlags.Center);
        
        for (int i = 0; i < 3; i++)
        {
            var dot = new View(this)
            {
                LayoutParameters = new LinearLayout.LayoutParams(24, 24)
                {
                    LeftMargin = 8,
                    RightMargin = 8
                }
            };
            var dotDrawable = new GradientDrawable();
            dotDrawable.SetShape(ShapeType.Oval);
            dotDrawable.SetColor(Android.Graphics.Color.ParseColor("#334155"));
            dot.Background = dotDrawable;
            _dotsContainer.AddView(dot);
        }
        
        rootLayout.AddView(_dotsContainer);
        
        // Buttons container
        var buttonsContainer = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
        };
        
        // Skip button
        _skipButton = new Button(this)
        {
            Text = "Skip",
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1,
                RightMargin = 16
            }
        };
        var skipDrawable = new GradientDrawable();
        skipDrawable.SetColor(Android.Graphics.Color.ParseColor("#1E293B"));
        skipDrawable.SetCornerRadius(24);
        _skipButton.Background = skipDrawable;
        _skipButton.SetTextColor(Android.Graphics.Color.White);
        _skipButton.SetAllCaps(false);
        _skipButton.TextSize = 16;
        _skipButton.SetPadding(0, 32, 0, 32);
        _skipButton.Click += (s, e) => FinishOnboarding();
        buttonsContainer.AddView(_skipButton);
        
        // Next button
        _nextButton = new Button(this)
        {
            Text = "Next",
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1
            }
        };
        var nextDrawable = new GradientDrawable();
        nextDrawable.SetColor(Android.Graphics.Color.ParseColor("#3B82F6"));
        nextDrawable.SetCornerRadius(24);
        _nextButton.Background = nextDrawable;
        _nextButton.SetTextColor(Android.Graphics.Color.White);
        _nextButton.SetAllCaps(false);
        _nextButton.TextSize = 16;
        _nextButton.SetPadding(0, 32, 0, 32);
        _nextButton.Click += OnNextClicked;
        buttonsContainer.AddView(_nextButton);
        
        rootLayout.AddView(buttonsContainer);
        
        SetContentView(rootLayout);
    }
    
    private void ShowStep(int step)
    {
        if (_contentContainer == null || _dotsContainer == null || _nextButton == null) return;
        
        // Clear content
        _contentContainer.RemoveAllViews();
        
        // Update dots
        for (int i = 0; i < _dotsContainer.ChildCount; i++)
        {
            var dot = _dotsContainer.GetChildAt(i);
            if (dot != null)
            {
                var dotDrawable = new GradientDrawable();
                dotDrawable.SetShape(ShapeType.Oval);
                dotDrawable.SetColor(Android.Graphics.Color.ParseColor(i == step ? "#3B82F6" : "#334155"));
                dot.Background = dotDrawable;
            }
        }
        
        // Show content for current step
        switch (step)
        {
            case 0:
                ShowWelcomeStep();
                _nextButton.Text = "Next";
                break;
            case 1:
                ShowFeaturesStep();
                _nextButton.Text = "Next";
                break;
            case 2:
                ShowSetupStep();
                _nextButton.Text = "Get Started";
                break;
        }
    }
    
    private void ShowWelcomeStep()
    {
        if (_contentContainer == null) return;
        
        // Icon
        var icon = new TextView(this)
        {
            Text = "🟣",
            TextSize = 80,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
            {
                BottomMargin = 48
            }
        };
        _contentContainer.AddView(icon);
        
        // Title
        var title = new TextView(this)
        {
            Text = "Welcome to VIRA",
            TextSize = 32,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
            {
                BottomMargin = 24
            }
        };
        title.SetTextColor(Android.Graphics.Color.White);
        title.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        title.Gravity = GravityFlags.Center;
        _contentContainer.AddView(title);
        
        // Subtitle
        var subtitle = new TextView(this)
        {
            Text = "Your Voice Intelligent Responsive Assistant",
            TextSize = 18,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
            {
                BottomMargin = 16
            }
        };
        subtitle.SetTextColor(Android.Graphics.Color.ParseColor("#94A3B8"));
        subtitle.Gravity = GravityFlags.Center;
        _contentContainer.AddView(subtitle);
        
        // Description
        var description = new TextView(this)
        {
            Text = "VIRA is your personal AI assistant powered by advanced language models. " +
                   "Get answers, manage tasks, and stay informed - all through natural conversation.",
            TextSize = 16,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
        };
        description.SetTextColor(Android.Graphics.Color.ParseColor("#CBD5E1"));
        description.Gravity = GravityFlags.Center;
        description.SetPadding(32, 0, 32, 0);
        _contentContainer.AddView(description);
    }
    
    private void ShowFeaturesStep()
    {
        if (_contentContainer == null) return;
        
        // Title
        var title = new TextView(this)
        {
            Text = "What VIRA Can Do",
            TextSize = 28,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
            {
                BottomMargin = 48
            }
        };
        title.SetTextColor(Android.Graphics.Color.White);
        title.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        _contentContainer.AddView(title);
        
        // Features list
        var features = new[]
        {
            ("💬", "Natural Conversations", "Chat naturally with AI powered by Gemini, Groq, or OpenAI"),
            ("🎤", "Voice Interaction", "Speak your questions and hear responses with realistic AI voice"),
            ("📰", "Stay Informed", "Get weather updates, news, and real-time information"),
            ("✅", "Task Management", "Create, track, and complete tasks with AI assistance"),
            ("🌐", "Multi-Provider", "Choose your preferred AI provider and model")
        };
        
        foreach (var (icon, featureTitle, description) in features)
        {
            var featureCard = CreateFeatureCard(icon, featureTitle, description);
            _contentContainer.AddView(featureCard);
        }
    }
    
    private LinearLayout CreateFeatureCard(string icon, string title, string description)
    {
        var card = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                BottomMargin = 24
            }
        };
        card.SetPadding(32, 24, 32, 24);
        
        var cardDrawable = new GradientDrawable();
        cardDrawable.SetColor(Android.Graphics.Color.ParseColor("#1E293B"));
        cardDrawable.SetCornerRadius(24);
        card.Background = cardDrawable;
        
        // Icon
        var iconText = new TextView(this)
        {
            Text = icon,
            TextSize = 32,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
            {
                RightMargin = 24
            }
        };
        card.AddView(iconText);
        
        // Text container
        var textContainer = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1
            }
        };
        
        var titleText = new TextView(this)
        {
            Text = title,
            TextSize = 16
        };
        titleText.SetTextColor(Android.Graphics.Color.White);
        titleText.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        textContainer.AddView(titleText);
        
        var descText = new TextView(this)
        {
            Text = description,
            TextSize = 14
        };
        descText.SetTextColor(Android.Graphics.Color.ParseColor("#94A3B8"));
        textContainer.AddView(descText);
        
        card.AddView(textContainer);
        
        return card;
    }
    
    private void ShowSetupStep()
    {
        if (_contentContainer == null) return;
        
        // Icon
        var icon = new TextView(this)
        {
            Text = "🔑",
            TextSize = 64,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
            {
                BottomMargin = 32
            }
        };
        _contentContainer.AddView(icon);
        
        // Title
        var title = new TextView(this)
        {
            Text = "One More Thing...",
            TextSize = 28,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
            {
                BottomMargin = 24
            }
        };
        title.SetTextColor(Android.Graphics.Color.White);
        title.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        _contentContainer.AddView(title);
        
        // Description
        var description = new TextView(this)
        {
            Text = "To use VIRA, you'll need to configure an AI provider API key. " +
                   "Don't worry - we'll guide you through it!\n\n" +
                   "You can choose from:\n" +
                   "• Gemini (Google)\n" +
                   "• Groq (Fast & Free)\n" +
                   "• OpenAI (GPT-4)\n\n" +
                   "Tap 'Get Started' to open Settings and configure your API key.",
            TextSize = 16,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
        };
        description.SetTextColor(Android.Graphics.Color.ParseColor("#CBD5E1"));
        description.SetPadding(32, 0, 32, 0);
        _contentContainer.AddView(description);
        
        // Info card
        var infoCard = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 32
            }
        };
        infoCard.SetPadding(32, 24, 32, 24);
        
        var infoDrawable = new GradientDrawable();
        infoDrawable.SetColor(Android.Graphics.Color.ParseColor("#1E3A8A"));
        infoDrawable.SetCornerRadius(24);
        infoCard.Background = infoDrawable;
        
        var infoText = new TextView(this)
        {
            Text = "💡 Tip: We recommend Groq for fast, free responses!",
            TextSize = 14
        };
        infoText.SetTextColor(Android.Graphics.Color.ParseColor("#BFDBFE"));
        infoCard.AddView(infoText);
        
        _contentContainer.AddView(infoCard);
    }
    
    private void OnNextClicked(object? sender, System.EventArgs e)
    {
        _currentStep++;
        
        if (_currentStep >= 3)
        {
            // Last step - open settings
            FinishOnboarding();
            OpenSettings();
        }
        else
        {
            ShowStep(_currentStep);
        }
    }
    
    private void FinishOnboarding()
    {
        // Mark onboarding as complete
        var prefs = GetSharedPreferences("vira_settings", FileCreationMode.Private);
        var editor = prefs?.Edit();
        editor?.PutBoolean("onboarding_completed", true);
        editor?.Apply();
        
        Finish();
    }
    
    private void OpenSettings()
    {
        var intent = new Intent(this, typeof(SettingsActivity));
        StartActivity(intent);
    }
}
