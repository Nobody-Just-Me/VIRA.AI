using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics.Drawables;
using Microsoft.Extensions.DependencyInjection;
using VIRA.Mobile.SharedServices;

namespace VIRA.Mobile.Activities;

[Activity(Label = "Settings", Theme = "@style/AppTheme")]
public class SettingsActivity : Activity
{
    private EditText? _apiKeyInput;
    private EditText? _elevenLabsKeyInput;
    private EditText? _voiceIdInput;
    private Spinner? _apiProviderSpinner;
    private Spinner? _modelSpinner;
    private TextView? _apiKeyLabel;
    private TextView? _apiKeyHint;
    private Switch? _voiceOutputSwitch;
    private Switch? _darkModeSwitch;
    private Switch? _webBrowsingSwitch;
    private Switch? _memoryModeSwitch;
    private Switch? _privacyModeSwitch;
    private Button? _showHideApiKeyButton;
    
    // Theme colors
    private string _bgPrimary = "#101622";
    private string _bgSecondary = "#1E293B";
    private string _bgCard = "#0DFFFFFF";
    private string _borderColor = "#1AFFFFFF";
    private string _textPrimary = "#FFFFFF";
    private string _textSecondary = "#94A3B8";
    private string _textTertiary = "#64748B";
    private string _statusBarColor = "#0A1628";
    
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        // Load theme colors first
        LoadThemeColors();
        
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
        {
            Window?.SetStatusBarColor(Android.Graphics.Color.ParseColor(_statusBarColor));
        }
        
        // Add screenshot protection for security
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
        {
            Window?.SetFlags(WindowManagerFlags.Secure, WindowManagerFlags.Secure);
        }
        
        BuildUI();
        LoadSettings();
    }
    
    private void LoadThemeColors()
    {
        var prefs = GetSharedPreferences("vira_settings", FileCreationMode.Private);
        var theme = prefs?.GetString("theme_preference", "dark") ?? "dark";
        
        if (theme == "light")
        {
            // Light theme colors
            _bgPrimary = "#F8FAFC";
            _bgSecondary = "#E2E8F0";
            _bgCard = "#FFFFFF";
            _borderColor = "#CBD5E1";
            _textPrimary = "#0F172A";
            _textSecondary = "#475569";
            _textTertiary = "#94A3B8";
            _statusBarColor = "#E2E8F0";
        }
        else if (theme == "system")
        {
            // Check system theme
            var uiMode = Resources?.Configuration?.UiMode;
            var isSystemDark = (uiMode & Android.Content.Res.UiMode.NightMask) == Android.Content.Res.UiMode.NightYes;
            
            if (isSystemDark)
            {
                // Dark theme colors (default)
                _bgPrimary = "#101622";
                _bgSecondary = "#1E293B";
                _bgCard = "#0DFFFFFF";
                _borderColor = "#1AFFFFFF";
                _textPrimary = "#FFFFFF";
                _textSecondary = "#94A3B8";
                _textTertiary = "#64748B";
                _statusBarColor = "#0A1628";
            }
            else
            {
                // Light theme colors
                _bgPrimary = "#F8FAFC";
                _bgSecondary = "#E2E8F0";
                _bgCard = "#FFFFFF";
                _borderColor = "#CBD5E1";
                _textPrimary = "#0F172A";
                _textSecondary = "#475569";
                _textTertiary = "#94A3B8";
                _statusBarColor = "#E2E8F0";
            }
        }
        else
        {
            // Dark theme colors (default)
            _bgPrimary = "#101622";
            _bgSecondary = "#1E293B";
            _bgCard = "#0DFFFFFF";
            _borderColor = "#1AFFFFFF";
            _textPrimary = "#FFFFFF";
            _textSecondary = "#94A3B8";
            _textTertiary = "#64748B";
            _statusBarColor = "#0A1628";
        }
    }
    
    private void LoadSettings()
    {
        // Load API key and provider from preferences
        var prefs = GetSharedPreferences("vira_settings", FileCreationMode.Private);
        var provider = prefs?.GetString("ai_provider", "gemini");
        
        // Set provider spinner
        if (_apiProviderSpinner != null)
        {
            var selection = provider switch
            {
                "groq" => 1,
                "openai" => 2,
                _ => 0 // gemini
            };
            _apiProviderSpinner.SetSelection(selection);
        }
        
        // Update model dropdown based on provider
        if (_modelSpinner != null)
        {
            var models = provider switch
            {
                "groq" => new[] { "Llama 3.3 70B", "Mixtral 8x7B" },
                "openai" => new[] { "GPT-4o-mini", "GPT-4o", "GPT-4 Turbo" },
                _ => new[] { "Flash", "Pro", "Ultra" }
            };
            
            var modelAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, models);
            modelAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            _modelSpinner.Adapter = modelAdapter;
            
            // Load saved model selection
            var savedModel = prefs?.GetString($"{provider}_model", null);
            if (!string.IsNullOrEmpty(savedModel))
            {
                var modelIndex = Array.IndexOf(models, savedModel);
                if (modelIndex >= 0)
                {
                    _modelSpinner.SetSelection(modelIndex);
                }
            }
        }
        
        // Load API key based on provider
        string? apiKey = null;
        if (provider == "groq")
        {
            apiKey = prefs?.GetString("groq_api_key", null);
        }
        else if (provider == "openai")
        {
            apiKey = prefs?.GetString("openai_api_key", null);
        }
        else
        {
            apiKey = prefs?.GetString("gemini_api_key", null);
        }
        
        if (!string.IsNullOrEmpty(apiKey) && _apiKeyInput != null)
        {
            _apiKeyInput.Text = apiKey;
        }
        
        // Load ElevenLabs API key
        var elevenLabsKey = prefs?.GetString("elevenlabs_api_key", null);
        if (!string.IsNullOrEmpty(elevenLabsKey) && _elevenLabsKeyInput != null)
        {
            _elevenLabsKeyInput.Text = elevenLabsKey;
        }
        
        // Load ElevenLabs Voice ID
        var voiceId = prefs?.GetString("elevenlabs_voice_id", "21m00Tcm4TlvDq8ikWAM");
        if (!string.IsNullOrEmpty(voiceId) && _voiceIdInput != null)
        {
            _voiceIdInput.Text = voiceId;
        }
        
        // Load voice output setting
        var voiceOutputEnabled = prefs?.GetBoolean("voice_output_enabled", true) ?? true;
        if (_voiceOutputSwitch != null)
        {
            _voiceOutputSwitch.Checked = voiceOutputEnabled;
        }

        if (_darkModeSwitch != null)
        {
            _darkModeSwitch.Checked = prefs?.GetBoolean("dark_mode_enabled", false) ?? false;
        }

        if (_webBrowsingSwitch != null)
        {
            _webBrowsingSwitch.Checked = prefs?.GetBoolean("web_browsing_enabled", true) ?? true;
        }

        if (_memoryModeSwitch != null)
        {
            _memoryModeSwitch.Checked = prefs?.GetBoolean("memory_mode_enabled", true) ?? true;
        }

        if (_privacyModeSwitch != null)
        {
            _privacyModeSwitch.Checked = prefs?.GetBoolean("privacy_mode_enabled", false) ?? false;
        }
    }
    
    private void BuildUI()
    {
        var scrollView = new ScrollView(this)
        {
            LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent)
        };
        scrollView.SetBackgroundColor(Android.Graphics.Color.ParseColor(_bgPrimary));
        
        var mainLayout = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
        };
        
        // Header
        mainLayout.AddView(CreateHeader());
        
        // Profile Card
        mainLayout.AddView(CreateProfileCard());
        
        // API Configuration Section
        mainLayout.AddView(CreateSectionTitle("API CONFIGURATION"));
        mainLayout.AddView(CreateAPIConfigSection());
        
        // Preferences Section
        mainLayout.AddView(CreateSectionTitle("PREFERENCES"));
        mainLayout.AddView(CreateThemeAppearanceSection());
        mainLayout.AddView(CreatePreferencesSection());
        
        // AI Behaviour Section
        mainLayout.AddView(CreateSectionTitle("AI BEHAVIOUR"));
        mainLayout.AddView(CreateAIBehaviourSection());
        
        // Data Section
        mainLayout.AddView(CreateSectionTitle("DATA"));
        mainLayout.AddView(CreateDataSection());
        
        // Support Section
        mainLayout.AddView(CreateSectionTitle("SUPPORT & ACCOUNT"));
        mainLayout.AddView(CreateSupportSection());
        
        // Footer
        mainLayout.AddView(CreateFooter());
        
        scrollView.AddView(mainLayout);
        SetContentView(scrollView);
    }
    
    private LinearLayout CreateHeader()
    {
        var header = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
        };
        header.SetPadding(32, 48, 32, 32);
        header.SetGravity(GravityFlags.CenterVertical);
        
        var backButton = new Button(this)
        {
            Text = "←",
            LayoutParameters = new LinearLayout.LayoutParams(
                100,
                100)
        };
        backButton.SetBackgroundColor(Android.Graphics.Color.Transparent);
        backButton.SetTextColor(Android.Graphics.Color.ParseColor(_textPrimary));
        backButton.TextSize = 28;
        backButton.Click += (s, e) => Finish();
        header.AddView(backButton);
        
        var title = new TextView(this)
        {
            Text = "Settings",
            TextSize = 24,
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1,
                LeftMargin = 16
            }
        };
        title.SetTextColor(Android.Graphics.Color.ParseColor(_textPrimary));
        title.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        header.AddView(title);
        
        return header;
    }
    
    private LinearLayout CreateProfileCard()
    {
        var card = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                LeftMargin = 32,
                RightMargin = 32,
                BottomMargin = 32
            }
        };
        
        var cardDrawable = new GradientDrawable();
        var colors = new int[] {
            Android.Graphics.Color.ParseColor("#6366F1"),
            Android.Graphics.Color.ParseColor("#8B5CF6")
        };
        cardDrawable.SetColors(colors);
        cardDrawable.SetCornerRadius(48);
        card.Background = cardDrawable;
        card.SetPadding(48, 48, 48, 48);
        
        // Avatar and title row
        var topRow = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
        };
        topRow.SetGravity(GravityFlags.CenterVertical);
        
        // Avatar
        var avatar = new TextView(this)
        {
            Text = "V",
            TextSize = 32,
            LayoutParameters = new LinearLayout.LayoutParams(
                120,
                120)
        };
        avatar.SetTextColor(Android.Graphics.Color.White);
        avatar.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        avatar.Gravity = GravityFlags.Center;
        var avatarDrawable = new GradientDrawable();
        avatarDrawable.SetColor(Android.Graphics.Color.ParseColor("#8B5CF6"));
        avatarDrawable.SetCornerRadius(60);
        avatar.Background = avatarDrawable;
        topRow.AddView(avatar);
        
        // Title and badge
        var titleContainer = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1,
                LeftMargin = 32
            }
        };
        
        var nameRow = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal
        };
        
        var name = new TextView(this)
        {
            Text = "Vira ",
            TextSize = 24
        };
        name.SetTextColor(Android.Graphics.Color.White);
        name.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        nameRow.AddView(name);
        
        var proBadge = new TextView(this)
        {
            Text = " PRO ",
            TextSize = 12,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
            {
                LeftMargin = 16
            }
        };
        proBadge.SetTextColor(Android.Graphics.Color.White);
        proBadge.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        proBadge.Gravity = GravityFlags.Center;
        var badgeDrawable = new GradientDrawable();
        badgeDrawable.SetColor(Android.Graphics.Color.ParseColor("#A78BFA"));
        badgeDrawable.SetCornerRadius(12);
        proBadge.Background = badgeDrawable;
        proBadge.SetPadding(24, 8, 24, 8);
        nameRow.AddView(proBadge);
        
        titleContainer.AddView(nameRow);
        
        var subtitle = new TextView(this)
        {
            Text = "Your Personal AI Assistant",
            TextSize = 14
        };
        subtitle.SetTextColor(Android.Graphics.Color.ParseColor("#C4B5FD"));
        titleContainer.AddView(subtitle);
        
        var rating = new TextView(this)
        {
            Text = "⭐⭐⭐⭐⭐ v2.4.0",
            TextSize = 12
        };
        rating.SetTextColor(Android.Graphics.Color.ParseColor("#E9D5FF"));
        titleContainer.AddView(rating);
        
        topRow.AddView(titleContainer);
        
        // Active badge
        var activeBadge = new TextView(this)
        {
            Text = "Active",
            TextSize = 12,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
        };
        activeBadge.SetTextColor(Android.Graphics.Color.White);
        activeBadge.Gravity = GravityFlags.Center;
        var activeDrawable = new GradientDrawable();
        activeDrawable.SetColor(Android.Graphics.Color.ParseColor("#10B981"));
        activeDrawable.SetCornerRadius(20);
        activeBadge.Background = activeDrawable;
        activeBadge.SetPadding(32, 16, 32, 16);
        topRow.AddView(activeBadge);
        
        card.AddView(topRow);
        
        // Stats row - Get real stats
        var stats = Utils.StatsTracker.GetAllStats(this);
        
        var statsRow = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 48
            }
        };
        
        statsRow.AddView(CreateStatItem(stats.conversations.ToString("N0"), "Conversations"));
        statsRow.AddView(CreateStatItem(stats.questions.ToString("N0"), "Questions"));
        statsRow.AddView(CreateStatItem(stats.daysActive.ToString(), "Days Active"));
        
        card.AddView(statsRow);
        
        return card;
    }
    
    private LinearLayout CreateStatItem(string value, string label)
    {
        var item = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1
            }
        };
        item.SetGravity(GravityFlags.Center);
        
        var valueText = new TextView(this)
        {
            Text = value,
            TextSize = 20
        };
        valueText.SetTextColor(Android.Graphics.Color.White);
        valueText.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        item.AddView(valueText);
        
        var labelText = new TextView(this)
        {
            Text = label,
            TextSize = 12
        };
        labelText.SetTextColor(Android.Graphics.Color.ParseColor("#C4B5FD"));
        item.AddView(labelText);
        
        return item;
    }
    
    private TextView CreateSectionTitle(string title)
    {
        var text = new TextView(this)
        {
            Text = title,
            TextSize = 12,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                LeftMargin = 48,
                RightMargin = 48,
                TopMargin = 32,
                BottomMargin = 16
            }
        };
        text.SetTextColor(Android.Graphics.Color.ParseColor(_textSecondary));
        text.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        return text;
    }
    
    private LinearLayout CreateAPIConfigSection()
    {
        var section = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                LeftMargin = 32,
                RightMargin = 32
            }
        };
        
        var card = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
        };
        var cardDrawable = new GradientDrawable();
        cardDrawable.SetColor(Android.Graphics.Color.ParseColor(_bgCard));
        cardDrawable.SetStroke(2, Android.Graphics.Color.ParseColor(_borderColor));
        cardDrawable.SetCornerRadius(32);
        card.Background = cardDrawable;
        card.SetPadding(48, 48, 48, 48);
        
        // API Provider label
        var providerLabel = new TextView(this)
        {
            Text = "API Provider",
            TextSize = 16
        };
        providerLabel.SetTextColor(Android.Graphics.Color.ParseColor(_textPrimary));
        card.AddView(providerLabel);
        
        // API Provider spinner
        _apiProviderSpinner = new Spinner(this)
        {
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 16
            }
        };
        var spinnerDrawable = new GradientDrawable();
        spinnerDrawable.SetColor(Android.Graphics.Color.ParseColor(_bgSecondary));
        spinnerDrawable.SetCornerRadius(24);
        _apiProviderSpinner.Background = spinnerDrawable;
        _apiProviderSpinner.SetPadding(48, 32, 48, 32);
        
        var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, 
            new[] { "Gemini", "Groq", "OpenAI" });
        adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
        _apiProviderSpinner.Adapter = adapter;
        _apiProviderSpinner.ItemSelected += OnProviderSelected;
        card.AddView(_apiProviderSpinner);
        
        // Spacer
        var spacer = new View(this)
        {
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                32)
            {
                TopMargin = 16
            }
        };
        card.AddView(spacer);
        
        // Model label
        var modelLabel = new TextView(this)
        {
            Text = "Model",
            TextSize = 16
        };
        modelLabel.SetTextColor(Android.Graphics.Color.ParseColor(_textPrimary));
        card.AddView(modelLabel);
        
        // Model spinner
        _modelSpinner = new Spinner(this)
        {
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 16
            }
        };
        var modelSpinnerDrawable = new GradientDrawable();
        modelSpinnerDrawable.SetColor(Android.Graphics.Color.ParseColor(_bgSecondary));
        modelSpinnerDrawable.SetCornerRadius(24);
        _modelSpinner.Background = modelSpinnerDrawable;
        _modelSpinner.SetPadding(48, 32, 48, 32);
        card.AddView(_modelSpinner);
        
        // Spacer
        var spacer2 = new View(this)
        {
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                32)
            {
                TopMargin = 16
            }
        };
        card.AddView(spacer2);
        
        // API Key label
        var label = new TextView(this)
        {
            Text = "API Key",
            TextSize = 16
        };
        label.SetTextColor(Android.Graphics.Color.ParseColor(_textPrimary));
        card.AddView(label);
        
        // API Key input container with show/hide button
        var apiKeyContainer = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 16
            }
        };
        
        // API Key input
        _apiKeyInput = new EditText(this)
        {
            Hint = "Paste your API key here...",
            InputType = Android.Text.InputTypes.TextVariationPassword,
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1
            }
        };
        var inputDrawable = new GradientDrawable();
        inputDrawable.SetColor(Android.Graphics.Color.ParseColor(_bgSecondary));
        inputDrawable.SetCornerRadius(24);
        _apiKeyInput.Background = inputDrawable;
        _apiKeyInput.SetPadding(48, 32, 48, 32);
        apiKeyContainer.AddView(_apiKeyInput);
        
        // Show/Hide button
        _showHideApiKeyButton = new Button(this)
        {
            Text = "👁",
            LayoutParameters = new LinearLayout.LayoutParams(
                120,
                ViewGroup.LayoutParams.WrapContent)
            {
                LeftMargin = 16
            }
        };
        var showHideDrawable = new GradientDrawable();
        showHideDrawable.SetColor(Android.Graphics.Color.ParseColor("#E0E7FF"));
        showHideDrawable.SetCornerRadius(24);
        _showHideApiKeyButton.Background = showHideDrawable;
        _showHideApiKeyButton.SetTextColor(Android.Graphics.Color.ParseColor("#4F46E5"));
        _showHideApiKeyButton.Click += OnShowHideApiKey;
        apiKeyContainer.AddView(_showHideApiKeyButton);
        
        card.AddView(apiKeyContainer);
        
        // Info text
        var info = new TextView(this)
        {
            Text = "Your key is stored securely on your device",
            TextSize = 12,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 16
            }
        };
        info.SetTextColor(Android.Graphics.Color.ParseColor(_textSecondary));
        card.AddView(info);
        
        // Get API Key buttons
        var buttonRow = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 16
            }
        };
        
        var geminiButton = new Button(this)
        {
            Text = "Get Gemini Key",
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1,
                RightMargin = 4
            }
        };
        var geminiDrawable = new GradientDrawable();
        geminiDrawable.SetColor(Android.Graphics.Color.ParseColor("#E0E7FF"));
        geminiDrawable.SetCornerRadius(20);
        geminiButton.Background = geminiDrawable;
        geminiButton.SetTextColor(Android.Graphics.Color.ParseColor("#4F46E5"));
        geminiButton.SetAllCaps(false);
        geminiButton.TextSize = 12;
        geminiButton.Click += (s, e) =>
        {
            var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://aistudio.google.com/apikey"));
            StartActivity(intent);
        };
        buttonRow.AddView(geminiButton);
        
        var groqButton = new Button(this)
        {
            Text = "Get Groq Key ⭐",
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1,
                LeftMargin = 4,
                RightMargin = 4
            }
        };
        var groqDrawable = new GradientDrawable();
        groqDrawable.SetColor(Android.Graphics.Color.ParseColor("#DCFCE7"));
        groqDrawable.SetCornerRadius(20);
        groqButton.Background = groqDrawable;
        groqButton.SetTextColor(Android.Graphics.Color.ParseColor("#16A34A"));
        groqButton.SetAllCaps(false);
        groqButton.TextSize = 12;
        groqButton.Click += (s, e) =>
        {
            var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://console.groq.com/keys"));
            StartActivity(intent);
        };
        buttonRow.AddView(groqButton);
        
        var openaiButton = new Button(this)
        {
            Text = "Get OpenAI Key",
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1,
                LeftMargin = 4
            }
        };
        var openaiDrawable = new GradientDrawable();
        openaiDrawable.SetColor(Android.Graphics.Color.ParseColor("#FEF3C7"));
        openaiDrawable.SetCornerRadius(20);
        openaiButton.Background = openaiDrawable;
        openaiButton.SetTextColor(Android.Graphics.Color.ParseColor("#D97706"));
        openaiButton.SetAllCaps(false);
        openaiButton.TextSize = 12;
        openaiButton.Click += (s, e) =>
        {
            var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://platform.openai.com/api-keys"));
            StartActivity(intent);
        };
        buttonRow.AddView(openaiButton);
        
        card.AddView(buttonRow);
        
        // Spacer for ElevenLabs section
        var spacer3 = new View(this)
        {
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                48)
            {
                TopMargin = 24
            }
        };
        card.AddView(spacer3);
        
        // ElevenLabs TTS Section
        var elevenLabsLabel = new TextView(this)
        {
            Text = "🎤 Voice Output (ElevenLabs TTS)",
            TextSize = 16
        };
        elevenLabsLabel.SetTextColor(Android.Graphics.Color.ParseColor(_textPrimary));
        elevenLabsLabel.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        card.AddView(elevenLabsLabel);
        
        var elevenLabsDesc = new TextView(this)
        {
            Text = "Ultra-realistic AI voice for VIRA responses",
            TextSize = 12,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 8
            }
        };
        elevenLabsDesc.SetTextColor(Android.Graphics.Color.ParseColor(_textSecondary));
        card.AddView(elevenLabsDesc);
        
        // ElevenLabs API Key input
        _elevenLabsKeyInput = new EditText(this)
        {
            Hint = "Paste ElevenLabs API key here...",
            InputType = Android.Text.InputTypes.TextVariationPassword,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 16
            }
        };
        var elevenLabsInputDrawable = new GradientDrawable();
        elevenLabsInputDrawable.SetColor(Android.Graphics.Color.ParseColor(_bgSecondary));
        elevenLabsInputDrawable.SetCornerRadius(24);
        _elevenLabsKeyInput.Background = elevenLabsInputDrawable;
        _elevenLabsKeyInput.SetPadding(48, 32, 48, 32);
        card.AddView(_elevenLabsKeyInput);
        
        // Voice ID label
        var voiceIdLabel = new TextView(this)
        {
            Text = "Voice ID (Optional)",
            TextSize = 14,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 24
            }
        };
        voiceIdLabel.SetTextColor(Android.Graphics.Color.ParseColor(_textPrimary));
        card.AddView(voiceIdLabel);
        
        // Voice ID input
        _voiceIdInput = new EditText(this)
        {
            Hint = "Default: 21m00Tcm4TlvDq8ikWAM (Rachel)",
            Text = "21m00Tcm4TlvDq8ikWAM",
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 8
            }
        };
        var voiceIdInputDrawable = new GradientDrawable();
        voiceIdInputDrawable.SetColor(Android.Graphics.Color.ParseColor(_bgSecondary));
        voiceIdInputDrawable.SetCornerRadius(24);
        _voiceIdInput.Background = voiceIdInputDrawable;
        _voiceIdInput.SetPadding(48, 32, 48, 32);
        card.AddView(_voiceIdInput);
        
        // Voice ID info
        var voiceIdInfo = new TextView(this)
        {
            Text = "💡 Find voices at: elevenlabs.io/app/voice-library\nExample: U3dExJoUNcmTY5H6GMuG",
            TextSize = 11,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 8
            }
        };
        voiceIdInfo.SetTextColor(Android.Graphics.Color.ParseColor(_textTertiary));
        card.AddView(voiceIdInfo);
        
        // Get ElevenLabs Key button
        var elevenLabsButton = new Button(this)
        {
            Text = "Get ElevenLabs Key (Free) 🎤",
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 16
            }
        };
        var elevenLabsButtonDrawable = new GradientDrawable();
        elevenLabsButtonDrawable.SetColor(Android.Graphics.Color.ParseColor("#F3E8FF"));
        elevenLabsButtonDrawable.SetCornerRadius(20);
        elevenLabsButton.Background = elevenLabsButtonDrawable;
        elevenLabsButton.SetTextColor(Android.Graphics.Color.ParseColor("#7C3AED"));
        elevenLabsButton.SetAllCaps(false);
        elevenLabsButton.TextSize = 14;
        elevenLabsButton.Click += (s, e) =>
        {
            var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://elevenlabs.io/"));
            StartActivity(intent);
        };
        card.AddView(elevenLabsButton);
        
        // Save button
        var saveButton = new Button(this)
        {
            Text = "Save Configuration",
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 32
            }
        };
        var saveDrawable = new GradientDrawable();
        saveDrawable.SetColor(Android.Graphics.Color.ParseColor("#3B82F6"));
        saveDrawable.SetCornerRadius(24);
        saveButton.Background = saveDrawable;
        saveButton.SetTextColor(Android.Graphics.Color.White);
        saveButton.SetAllCaps(false);
        saveButton.TextSize = 16;
        saveButton.SetPadding(0, 32, 0, 32);
        saveButton.Click += OnSaveConfig;
        card.AddView(saveButton);
        
        section.AddView(card);
        return section;
    }
    
    private LinearLayout CreateThemeAppearanceSection()
    {
        var section = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                LeftMargin = 32,
                RightMargin = 32,
                BottomMargin = 24
            }
        };
        
        var card = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
        };
        var cardDrawable = new GradientDrawable();
        cardDrawable.SetColor(Android.Graphics.Color.ParseColor("#0DFFFFFF"));
        cardDrawable.SetStroke(2, Android.Graphics.Color.ParseColor("#1AFFFFFF"));
        cardDrawable.SetCornerRadius(32);
        card.Background = cardDrawable;
        card.SetPadding(48, 48, 48, 48);
        
        // Header with icon and title
        var headerRow = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                BottomMargin = 16
            }
        };
        headerRow.SetGravity(GravityFlags.CenterVertical);
        
        // Icon
        var iconView = new TextView(this)
        {
            Text = "🌓",
            TextSize = 24,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
            {
                RightMargin = 16
            }
        };
        headerRow.AddView(iconView);
        
        // Title and subtitle
        var titleContainer = new LinearLayout(this)
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
            Text = "Theme Appearance",
            TextSize = 16
        };
        titleText.SetTextColor(Android.Graphics.Color.ParseColor(_textPrimary));
        titleText.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        titleContainer.AddView(titleText);
        
        var subtitleText = new TextView(this)
        {
            Text = "Choose how Vira looks",
            TextSize = 13
        };
        subtitleText.SetTextColor(Android.Graphics.Color.ParseColor(_textSecondary));
        titleContainer.AddView(subtitleText);
        
        headerRow.AddView(titleContainer);
        card.AddView(headerRow);
        
        // Theme buttons row
        var themeRow = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 16
            }
        };
        
        // Load current theme
        var prefs = GetSharedPreferences("vira_settings", FileCreationMode.Private);
        var currentTheme = prefs?.GetString("theme_preference", "light") ?? "light";
        
        // Light theme button
        var lightButton = CreateThemeButton("☀️", "Light", currentTheme == "light");
        lightButton.Click += (s, e) => OnThemeSelected("light");
        themeRow.AddView(lightButton);
        
        // Dark theme button
        var darkButton = CreateThemeButton("🌙", "Dark", currentTheme == "dark");
        darkButton.Click += (s, e) => OnThemeSelected("dark");
        themeRow.AddView(darkButton);
        
        // System theme button
        var systemButton = CreateThemeButton("🌐", "System", currentTheme == "system");
        systemButton.Click += (s, e) => OnThemeSelected("system");
        themeRow.AddView(systemButton);
        
        card.AddView(themeRow);
        section.AddView(card);
        return section;
    }
    
    private LinearLayout CreateThemeButton(string icon, string label, bool isSelected)
    {
        var button = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1,
                RightMargin = 12
            }
        };
        button.SetGravity(GravityFlags.Center);
        button.SetPadding(24, 32, 24, 32);
        button.Clickable = true;
        button.Focusable = true;
        
        // Set background based on selection
        var buttonDrawable = new GradientDrawable();
        if (isSelected)
        {
            buttonDrawable.SetColor(Android.Graphics.Color.ParseColor("#3B82F6"));
            buttonDrawable.SetStroke(3, Android.Graphics.Color.ParseColor("#60A5FA"));
        }
        else
        {
            buttonDrawable.SetColor(Android.Graphics.Color.ParseColor("#1E293B"));
            buttonDrawable.SetStroke(2, Android.Graphics.Color.ParseColor("#334155"));
        }
        buttonDrawable.SetCornerRadius(24);
        button.Background = buttonDrawable;
        
        // Icon
        var iconText = new TextView(this)
        {
            Text = icon,
            TextSize = 28,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
            {
                BottomMargin = 8
            }
        };
        button.AddView(iconText);
        
        // Label
        var labelText = new TextView(this)
        {
            Text = label,
            TextSize = 14
        };
        labelText.SetTextColor(isSelected ? Android.Graphics.Color.White : Android.Graphics.Color.ParseColor("#94A3B8"));
        labelText.SetTypeface(null, isSelected ? Android.Graphics.TypefaceStyle.Bold : Android.Graphics.TypefaceStyle.Normal);
        button.AddView(labelText);
        
        return button;
    }
    
    private void OnThemeSelected(string theme)
    {
        // Save theme preference
        var prefs = GetSharedPreferences("vira_settings", FileCreationMode.Private);
        var editor = prefs?.Edit();
        editor?.PutString("theme_preference", theme);
        editor?.Apply();
        
        // Show toast
        var themeLabel = theme switch
        {
            "light" => "Light ☀️",
            "dark" => "Dark 🌙",
            "system" => "System 🌐",
            _ => theme
        };
        Toast.MakeText(this, $"Theme: {themeLabel}", ToastLength.Short)?.Show();
        
        // Rebuild UI to reflect changes
        SetContentView(null);
        BuildUI();
        LoadSettings();
    }
    
    private LinearLayout CreatePreferencesSection()
    {
        var section = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                LeftMargin = 32,
                RightMargin = 32
            }
        };
        
        var card = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
        };
        var cardDrawable = new GradientDrawable();
        cardDrawable.SetColor(Android.Graphics.Color.ParseColor(_bgCard));
        cardDrawable.SetStroke(2, Android.Graphics.Color.ParseColor(_borderColor));
        cardDrawable.SetCornerRadius(32);
        card.Background = cardDrawable;
        card.SetPadding(48, 32, 48, 32);
        
        _voiceOutputSwitch = CreateToggleItem(card, "🔊", "Voice Output", "Read responses aloud", true);
        _webBrowsingSwitch = CreateToggleItem(card, "🌐", "Use Web Browsing", "Allow Vira to search the internet", true);
        
        // Add Haptics toggle
        var hapticsSwitch = CreateToggleItem(card, "⚡", "Haptics", "Vibrate on interaction", true);
        
        // Add Notifications toggle
        var notificationsSwitch = CreateToggleItem(card, "🔔", "Notifications", "Reminders & updates from Vira", true);
        
        section.AddView(card);
        return section;
    }
    
    private LinearLayout CreateAIBehaviourSection()
    {
        var section = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                LeftMargin = 32,
                RightMargin = 32
            }
        };
        
        var card = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
        };
        var cardDrawable = new GradientDrawable();
        cardDrawable.SetColor(Android.Graphics.Color.ParseColor(_bgCard));
        cardDrawable.SetStroke(2, Android.Graphics.Color.ParseColor(_borderColor));
        cardDrawable.SetCornerRadius(32);
        card.Background = cardDrawable;
        card.SetPadding(48, 32, 48, 32);
        
        _memoryModeSwitch = CreateToggleItem(card, "🧠", "Memory Mode", "Remember context across sessions", true);
        _privacyModeSwitch = CreateToggleItem(card, "🛡️", "Privacy Mode", "Don't save conversation history", false);
        
        // Add Language dropdown
        card.AddView(CreateDropdownItem("🌍", "Language", "English", new[] { "English", "Bahasa Indonesia", "Español", "Français", "Deutsch", "日本語", "中文" }));
        
        // Add Response Style dropdown
        card.AddView(CreateDropdownItem("💬", "Response Style", "Balanced", new[] { "Concise", "Balanced", "Detailed", "Creative", "Professional" }));
        
        section.AddView(card);
        return section;
    }
    
    private LinearLayout CreateDataSection()
    {
        var section = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                LeftMargin = 32,
                RightMargin = 32
            }
        };
        
        var card = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
        };
        var cardDrawable = new GradientDrawable();
        cardDrawable.SetColor(Android.Graphics.Color.ParseColor(_bgCard));
        cardDrawable.SetStroke(2, Android.Graphics.Color.ParseColor(_borderColor));
        cardDrawable.SetCornerRadius(32);
        card.Background = cardDrawable;
        card.SetPadding(48, 32, 48, 32);
        
        var clearButton = CreateActionItem("🗑️", "Clear Chat History", "Remove all conversations");
        clearButton.Click += (s, e) =>
        {
            new AlertDialog.Builder(this)
                .SetTitle("Clear Chat History")
                .SetMessage("Are you sure you want to delete all conversations and reset statistics?")
                .SetPositiveButton("Clear", (s2, e2) =>
                {
                    Utils.StatsTracker.ClearStats(this);
                    Toast.MakeText(this, "Chat history and statistics cleared", ToastLength.Short)?.Show();
                    
                    // Rebuild UI to refresh stats
                    BuildUI();
                    LoadSettings();
                })
                .SetNegativeButton("Cancel", (s2, e2) => { })
                .Show();
        };
        card.AddView(clearButton);
        
        section.AddView(card);
        return section;
    }
    
    private LinearLayout CreateSupportSection()
    {
        var section = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                LeftMargin = 32,
                RightMargin = 32
            }
        };
        
        var card = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
        };
        var cardDrawable = new GradientDrawable();
        cardDrawable.SetColor(Android.Graphics.Color.ParseColor(_bgCard));
        cardDrawable.SetStroke(2, Android.Graphics.Color.ParseColor(_borderColor));
        cardDrawable.SetCornerRadius(32);
        card.Background = cardDrawable;
        card.SetPadding(48, 32, 48, 32);
        
        card.AddView(CreateActionItem("❓", "Help & Support", "FAQs, tutorials, contact us"));
        card.AddView(CreateActionItem("⭐", "Rate Vira", "Share your experience"));
        
        var signOut = CreateActionItem("🚪", "Sign Out", "");
        signOut.Click += (s, e) => Finish();
        card.AddView(signOut);
        
        section.AddView(card);
        return section;
    }
    
    private Switch CreateToggleItem(LinearLayout parent, string icon, string title, string subtitle, bool defaultValue)
    {
        var item = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 16,
                BottomMargin = 16
            }
        };
        item.SetGravity(GravityFlags.CenterVertical);
        
        var iconText = new TextView(this)
        {
            Text = icon,
            TextSize = 24,
            LayoutParameters = new LinearLayout.LayoutParams(
                80,
                80)
        };
        iconText.Gravity = GravityFlags.Center;
        item.AddView(iconText);
        
        var textContainer = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1,
                LeftMargin = 16
            }
        };
        
        var titleText = new TextView(this)
        {
            Text = title,
            TextSize = 16
        };
        titleText.SetTextColor(Android.Graphics.Color.ParseColor(_textPrimary));
        textContainer.AddView(titleText);
        
        var subtitleText = new TextView(this)
        {
            Text = subtitle,
            TextSize = 12
        };
        subtitleText.SetTextColor(Android.Graphics.Color.ParseColor(_textSecondary));
        textContainer.AddView(subtitleText);
        
        item.AddView(textContainer);
        
        var toggle = new Switch(this)
        {
            Checked = defaultValue
        };
        item.AddView(toggle);
        
        parent.AddView(item);
        return toggle;
    }
    
    private LinearLayout CreateActionItem(string icon, string title, string subtitle)
    {
        var item = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 16,
                BottomMargin = 16
            }
        };
        item.SetGravity(GravityFlags.CenterVertical);
        item.Clickable = true;
        item.Focusable = true;
        
        var iconText = new TextView(this)
        {
            Text = icon,
            TextSize = 24,
            LayoutParameters = new LinearLayout.LayoutParams(
                80,
                80)
        };
        iconText.Gravity = GravityFlags.Center;
        item.AddView(iconText);
        
        var textContainer = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1,
                LeftMargin = 16
            }
        };
        
        var titleText = new TextView(this)
        {
            Text = title,
            TextSize = 16
        };
        titleText.SetTextColor(Android.Graphics.Color.ParseColor(_textPrimary));
        textContainer.AddView(titleText);
        
        if (!string.IsNullOrEmpty(subtitle))
        {
            var subtitleText = new TextView(this)
            {
                Text = subtitle,
                TextSize = 12
            };
            subtitleText.SetTextColor(Android.Graphics.Color.ParseColor(_textSecondary));
            textContainer.AddView(subtitleText);
        }
        
        item.AddView(textContainer);
        
        var arrow = new TextView(this)
        {
            Text = "›",
            TextSize = 24
        };
        arrow.SetTextColor(Android.Graphics.Color.ParseColor("#CBD5E1"));
        item.AddView(arrow);
        
        return item;
    }
    
    private LinearLayout CreateDropdownItem(string icon, string title, string currentValue, string[] options)
    {
        var item = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 16,
                BottomMargin = 16
            }
        };
        item.SetGravity(GravityFlags.CenterVertical);
        
        var iconText = new TextView(this)
        {
            Text = icon,
            TextSize = 24,
            LayoutParameters = new LinearLayout.LayoutParams(
                80,
                80)
        };
        iconText.Gravity = GravityFlags.Center;
        item.AddView(iconText);
        
        var textContainer = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1,
                LeftMargin = 16
            }
        };
        
        var titleText = new TextView(this)
        {
            Text = title,
            TextSize = 16
        };
        titleText.SetTextColor(Android.Graphics.Color.ParseColor(_textPrimary));
        textContainer.AddView(titleText);
        
        item.AddView(textContainer);
        
        // Value and arrow container
        var valueContainer = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
        };
        valueContainer.SetGravity(GravityFlags.CenterVertical);
        
        var valueText = new TextView(this)
        {
            Text = currentValue,
            TextSize = 14,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
            {
                RightMargin = 8
            }
        };
        valueText.SetTextColor(Android.Graphics.Color.ParseColor(_textSecondary));
        valueContainer.AddView(valueText);
        
        var arrow = new TextView(this)
        {
            Text = "›",
            TextSize = 24
        };
        arrow.SetTextColor(Android.Graphics.Color.ParseColor(_textTertiary));
        valueContainer.AddView(arrow);
        
        item.AddView(valueContainer);
        
        // Make clickable to show dialog
        item.Clickable = true;
        item.Focusable = true;
        item.Click += (s, e) =>
        {
            new AlertDialog.Builder(this)
                .SetTitle($"Select {title}")
                .SetItems(options, (s2, e2) =>
                {
                    var selectedOption = options[e2.Which];
                    valueText.Text = selectedOption;
                    
                    // Save to preferences
                    var prefs = GetSharedPreferences("vira_settings", FileCreationMode.Private);
                    var editor = prefs?.Edit();
                    editor?.PutString(title.ToLower().Replace(" ", "_"), selectedOption);
                    editor?.Apply();
                    
                    Toast.MakeText(this, $"{title}: {selectedOption}", ToastLength.Short)?.Show();
                })
                .Show();
        };
        
        return item;
    }
    
    private LinearLayout CreateFooter()
    {
        var footer = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 64,
                BottomMargin = 64
            }
        };
        footer.SetGravity(GravityFlags.Center);
        
        var logo = new TextView(this)
        {
            Text = "🟣 Vira AI",
            TextSize = 16
        };
        logo.SetTextColor(Android.Graphics.Color.ParseColor("#8B5CF6"));
        logo.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        footer.AddView(logo);
        
        var version = new TextView(this)
        {
            Text = "Version 2.4.0 (Build 1024)",
            TextSize = 12,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 8
            }
        };
        version.SetTextColor(Android.Graphics.Color.ParseColor(_textSecondary));
        footer.AddView(version);
        
        var tagline = new TextView(this)
        {
            Text = "Made with ⭐ by Kiro Team",
            TextSize = 11,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 4
            }
        };
        tagline.SetTextColor(Android.Graphics.Color.ParseColor("#CBD5E1"));
        footer.AddView(tagline);
        
        return footer;
    }
    
    private void OnSaveConfig(object? sender, System.EventArgs e)
    {
        var apiKey = _apiKeyInput?.Text?.Trim();
        var elevenLabsKey = _elevenLabsKeyInput?.Text?.Trim();
        var provider = _apiProviderSpinner?.SelectedItemPosition switch
        {
            1 => "groq",
            2 => "openai",
            _ => "gemini"
        };
        
        // Get selected model
        var selectedModel = _modelSpinner?.SelectedItem?.ToString();
        
        if (string.IsNullOrEmpty(apiKey))
        {
            Toast.MakeText(this, "Please enter an API Key", ToastLength.Short)?.Show();
            return;
        }
        
        Android.Util.Log.Info("VIRA_Settings", "========================================");
        Android.Util.Log.Info("VIRA_Settings", $"💾 Saving Configuration");
        Android.Util.Log.Info("VIRA_Settings", $"   Provider: {provider}");
        Android.Util.Log.Info("VIRA_Settings", $"   Model: {selectedModel}");
        Android.Util.Log.Info("VIRA_Settings", $"   API Key: {apiKey.Substring(0, Math.Min(10, apiKey.Length))}... (length: {apiKey.Length})");
        
        if (!string.IsNullOrEmpty(elevenLabsKey))
        {
            Android.Util.Log.Info("VIRA_Settings", $"   ElevenLabs Key: {elevenLabsKey.Substring(0, Math.Min(10, elevenLabsKey.Length))}... (length: {elevenLabsKey.Length})");
        }
        
        // Save to preferences
        var prefs = GetSharedPreferences("vira_settings", FileCreationMode.Private);
        var editor = prefs?.Edit();
        
        // Save API key to provider-specific key
        if (provider == "groq")
        {
            editor?.PutString("groq_api_key", apiKey);
        }
        else if (provider == "openai")
        {
            editor?.PutString("openai_api_key", apiKey);
        }
        else
        {
            editor?.PutString("gemini_api_key", apiKey);
        }
        
        editor?.PutString("ai_provider", provider);
        
        // Save model selection
        if (!string.IsNullOrEmpty(selectedModel))
        {
            editor?.PutString($"{provider}_model", selectedModel);
        }
        
        // Save ElevenLabs API key
        if (!string.IsNullOrEmpty(elevenLabsKey))
        {
            editor?.PutString("elevenlabs_api_key", elevenLabsKey);
        }
        
        // Save ElevenLabs Voice ID
        var voiceId = _voiceIdInput?.Text?.Trim();
        if (!string.IsNullOrEmpty(voiceId))
        {
            editor?.PutString("elevenlabs_voice_id", voiceId);
            Android.Util.Log.Info("VIRA_Settings", $"   Voice ID: {voiceId}");
        }
        else
        {
            // Use default if empty
            editor?.PutString("elevenlabs_voice_id", "21m00Tcm4TlvDq8ikWAM");
            Android.Util.Log.Info("VIRA_Settings", "   Voice ID: Using default (Rachel)");
        }
        
        // Save voice output setting
        if (_voiceOutputSwitch != null)
        {
            editor?.PutBoolean("voice_output_enabled", _voiceOutputSwitch.Checked);
        }

        if (_darkModeSwitch != null)
        {
            editor?.PutBoolean("dark_mode_enabled", _darkModeSwitch.Checked);
        }

        if (_webBrowsingSwitch != null)
        {
            editor?.PutBoolean("web_browsing_enabled", _webBrowsingSwitch.Checked);
        }

        if (_memoryModeSwitch != null)
        {
            editor?.PutBoolean("memory_mode_enabled", _memoryModeSwitch.Checked);
        }

        if (_privacyModeSwitch != null)
        {
            editor?.PutBoolean("privacy_mode_enabled", _privacyModeSwitch.Checked);
        }
        
        var success = editor?.Commit();
        
        Android.Util.Log.Info("VIRA_Settings", $"✅ Configuration saved (success: {success})");
        Android.Util.Log.Info("VIRA_Settings", "========================================");
        
        var providerName = provider switch
        {
            "groq" => $"Groq ({selectedModel})",
            "openai" => $"OpenAI ({selectedModel})",
            _ => $"Gemini ({selectedModel})"
        };
        var voiceStatus = !string.IsNullOrEmpty(elevenLabsKey) ? "Voice: Enabled ✅" : "Voice: Disabled (no key)";
        Toast.MakeText(this, $"✅ Configuration Saved!\n\nProvider: {providerName}\n{voiceStatus}\n\nKembali ke chat untuk test!", ToastLength.Long)?.Show();
    }
    
    private void OnProviderSelected(object? sender, AdapterView.ItemSelectedEventArgs e)
    {
        // Update model dropdown based on provider selection
        if (_modelSpinner == null) return;
        
        var models = e.Position switch
        {
            1 => new[] { "Llama 3.3 70B", "Mixtral 8x7B" }, // Groq
            2 => new[] { "GPT-4o-mini", "GPT-4o", "GPT-4 Turbo" }, // OpenAI
            _ => new[] { "Flash", "Pro", "Ultra" } // Gemini
        };
        
        var modelAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, models);
        modelAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
        _modelSpinner.Adapter = modelAdapter;
    }
    
    private void OnShowHideApiKey(object? sender, System.EventArgs e)
    {
        if (_apiKeyInput == null || _showHideApiKeyButton == null) return;
        
        // Toggle input type
        if (_apiKeyInput.InputType == Android.Text.InputTypes.TextVariationPassword)
        {
            // Show password
            _apiKeyInput.InputType = Android.Text.InputTypes.ClassText;
            _showHideApiKeyButton.Text = "🙈";
        }
        else
        {
            // Hide password
            _apiKeyInput.InputType = Android.Text.InputTypes.TextVariationPassword;
            _showHideApiKeyButton.Text = "👁";
        }
        
        // Move cursor to end
        _apiKeyInput.SetSelection(_apiKeyInput.Text?.Length ?? 0);
    }
}
