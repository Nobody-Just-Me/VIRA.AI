using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics.Drawables;
using Microsoft.Extensions.DependencyInjection;
using VIRA.Shared.Services;

namespace VIRA.Mobile.Activities;

[Activity(Label = "Settings", Theme = "@style/AppTheme")]
public class SettingsActivity : Activity
{
    private EditText? _apiKeyInput;
    private EditText? _elevenLabsKeyInput;
    private Spinner? _apiProviderSpinner;
    private TextView? _apiKeyLabel;
    private TextView? _apiKeyHint;
    private Switch? _voiceOutputSwitch;
    private Switch? _darkModeSwitch;
    private Switch? _webBrowsingSwitch;
    private Switch? _memoryModeSwitch;
    private Switch? _privacyModeSwitch;
    
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
        {
            Window?.SetStatusBarColor(Android.Graphics.Color.ParseColor("#F8FAFC"));
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M && Window?.DecorView != null)
            {
                Window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
            }
        }
        
        BuildUI();
        LoadSettings();
    }
    
    private void LoadSettings()
    {
        // Load API key and provider from preferences
        var prefs = GetSharedPreferences("vira_settings", FileCreationMode.Private);
        var provider = prefs?.GetString("ai_provider", "gemini");
        
        // Set provider spinner
        if (_apiProviderSpinner != null)
        {
            _apiProviderSpinner.SetSelection(provider == "groq" ? 1 : 0);
        }
        
        // Load API key based on provider
        string? apiKey = null;
        if (provider == "groq")
        {
            apiKey = prefs?.GetString("groq_api_key", null);
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
        
        // Load voice output setting
        var voiceOutputEnabled = prefs?.GetBoolean("voice_output_enabled", true) ?? true;
        if (_voiceOutputSwitch != null)
        {
            _voiceOutputSwitch.Checked = voiceOutputEnabled;
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
        scrollView.SetBackgroundColor(Android.Graphics.Color.ParseColor("#F8FAFC"));
        
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
        backButton.SetTextColor(Android.Graphics.Color.ParseColor("#1E293B"));
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
        title.SetTextColor(Android.Graphics.Color.ParseColor("#0F172A"));
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
        text.SetTextColor(Android.Graphics.Color.ParseColor("#94A3B8"));
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
        cardDrawable.SetColor(Android.Graphics.Color.White);
        cardDrawable.SetCornerRadius(32);
        card.Background = cardDrawable;
        card.SetPadding(48, 48, 48, 48);
        
        // API Provider label
        var providerLabel = new TextView(this)
        {
            Text = "API Provider",
            TextSize = 16
        };
        providerLabel.SetTextColor(Android.Graphics.Color.ParseColor("#1E293B"));
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
        spinnerDrawable.SetColor(Android.Graphics.Color.ParseColor("#F1F5F9"));
        spinnerDrawable.SetCornerRadius(24);
        _apiProviderSpinner.Background = spinnerDrawable;
        _apiProviderSpinner.SetPadding(48, 32, 48, 32);
        
        var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, 
            new[] { "Gemini", "Groq" });
        adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
        _apiProviderSpinner.Adapter = adapter;
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
        
        // API Key label
        var label = new TextView(this)
        {
            Text = "API Key",
            TextSize = 16
        };
        label.SetTextColor(Android.Graphics.Color.ParseColor("#1E293B"));
        card.AddView(label);
        
        // API Key input
        _apiKeyInput = new EditText(this)
        {
            Hint = "Paste your API key here...",
            InputType = Android.Text.InputTypes.TextVariationPassword,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 16
            }
        };
        var inputDrawable = new GradientDrawable();
        inputDrawable.SetColor(Android.Graphics.Color.ParseColor("#F1F5F9"));
        inputDrawable.SetCornerRadius(24);
        _apiKeyInput.Background = inputDrawable;
        _apiKeyInput.SetPadding(48, 32, 48, 32);
        card.AddView(_apiKeyInput);
        
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
        info.SetTextColor(Android.Graphics.Color.ParseColor("#94A3B8"));
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
                RightMargin = 8
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
                LeftMargin = 8
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
        
        card.AddView(buttonRow);
        
        // Spacer for ElevenLabs section
        var spacer2 = new View(this)
        {
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                48)
            {
                TopMargin = 24
            }
        };
        card.AddView(spacer2);
        
        // ElevenLabs TTS Section
        var elevenLabsLabel = new TextView(this)
        {
            Text = "🎤 Voice Output (ElevenLabs TTS)",
            TextSize = 16
        };
        elevenLabsLabel.SetTextColor(Android.Graphics.Color.ParseColor("#1E293B"));
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
        elevenLabsDesc.SetTextColor(Android.Graphics.Color.ParseColor("#64748B"));
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
        elevenLabsInputDrawable.SetColor(Android.Graphics.Color.ParseColor("#F1F5F9"));
        elevenLabsInputDrawable.SetCornerRadius(24);
        _elevenLabsKeyInput.Background = elevenLabsInputDrawable;
        _elevenLabsKeyInput.SetPadding(48, 32, 48, 32);
        card.AddView(_elevenLabsKeyInput);
        
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
        cardDrawable.SetColor(Android.Graphics.Color.White);
        cardDrawable.SetCornerRadius(32);
        card.Background = cardDrawable;
        card.SetPadding(48, 32, 48, 32);
        
        _voiceOutputSwitch = CreateToggleItem(card, "🔊", "Voice Output", "Read responses aloud", true);
        _darkModeSwitch = CreateToggleItem(card, "🌙", "Dark Mode", "Sync with system settings", false);
        _webBrowsingSwitch = CreateToggleItem(card, "🌐", "Use Web Browsing", "Allow Vira to search the internet", true);
        
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
        cardDrawable.SetColor(Android.Graphics.Color.White);
        cardDrawable.SetCornerRadius(32);
        card.Background = cardDrawable;
        card.SetPadding(48, 32, 48, 32);
        
        _memoryModeSwitch = CreateToggleItem(card, "🧠", "Memory Mode", "Remember context across sessions", true);
        _privacyModeSwitch = CreateToggleItem(card, "🛡️", "Privacy Mode", "Don't save conversation history", false);
        
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
        cardDrawable.SetColor(Android.Graphics.Color.White);
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
        cardDrawable.SetColor(Android.Graphics.Color.White);
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
        titleText.SetTextColor(Android.Graphics.Color.ParseColor("#1E293B"));
        textContainer.AddView(titleText);
        
        var subtitleText = new TextView(this)
        {
            Text = subtitle,
            TextSize = 12
        };
        subtitleText.SetTextColor(Android.Graphics.Color.ParseColor("#94A3B8"));
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
        titleText.SetTextColor(Android.Graphics.Color.ParseColor("#1E293B"));
        textContainer.AddView(titleText);
        
        if (!string.IsNullOrEmpty(subtitle))
        {
            var subtitleText = new TextView(this)
            {
                Text = subtitle,
                TextSize = 12
            };
            subtitleText.SetTextColor(Android.Graphics.Color.ParseColor("#94A3B8"));
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
        version.SetTextColor(Android.Graphics.Color.ParseColor("#94A3B8"));
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
        var provider = _apiProviderSpinner?.SelectedItemPosition == 1 ? "groq" : "gemini";
        
        if (string.IsNullOrEmpty(apiKey))
        {
            Toast.MakeText(this, "Please enter an API Key", ToastLength.Short)?.Show();
            return;
        }
        
        Android.Util.Log.Info("VIRA_Settings", "========================================");
        Android.Util.Log.Info("VIRA_Settings", $"💾 Saving Configuration");
        Android.Util.Log.Info("VIRA_Settings", $"   Provider: {provider}");
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
        else
        {
            editor?.PutString("gemini_api_key", apiKey);
        }
        
        editor?.PutString("ai_provider", provider);
        
        // Save ElevenLabs API key
        if (!string.IsNullOrEmpty(elevenLabsKey))
        {
            editor?.PutString("elevenlabs_api_key", elevenLabsKey);
        }
        
        // Save voice output setting
        if (_voiceOutputSwitch != null)
        {
            editor?.PutBoolean("voice_output_enabled", _voiceOutputSwitch.Checked);
        }
        
        var success = editor?.Commit();
        
        Android.Util.Log.Info("VIRA_Settings", $"✅ Configuration saved (success: {success})");
        Android.Util.Log.Info("VIRA_Settings", "========================================");
        
        var providerName = provider == "groq" ? "Groq (Llama 3.3 70B)" : "Gemini (gemini-2.0-flash)";
        var voiceStatus = !string.IsNullOrEmpty(elevenLabsKey) ? "Voice: Enabled ✅" : "Voice: Disabled (no key)";
        Toast.MakeText(this, $"✅ Configuration Saved!\n\nProvider: {providerName}\n{voiceStatus}\n\nKembali ke chat untuk test!", ToastLength.Long)?.Show();
    }
}
