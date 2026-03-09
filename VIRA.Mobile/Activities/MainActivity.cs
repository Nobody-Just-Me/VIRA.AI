using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics.Drawables;
using Microsoft.Extensions.DependencyInjection;
using VIRA.Mobile.SharedServices;
using VIRA.Mobile.ViewModels;
using VIRA.Mobile.SharedModels;
using VIRA.Mobile.Utils;
using VIRA.Mobile.Models;
using VIRA.Mobile.Views;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace VIRA.Mobile.Activities;

[Activity(
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden,
    WindowSoftInputMode = SoftInput.AdjustResize,
    Theme = "@style/AppTheme",
    Label = "VIRA")]
public class MainActivity : Activity
{
    private const string TAG = "VIRA_MainActivity";
    private LinearLayout? _chatLayout;
    private ScrollView? _scrollView;
    private EditText? _inputBox;
    private Button? _sendButton;
    private Button? _voiceButton;
    private Button? _plusButton;
    private TextView? _greetingText;
    private TextView? _statusText;
    private HorizontalScrollView? _quickActionsScroll;
    private MainChatViewModel? _viewModel;
    private ITTSService? _ttsService;
    private Services.AndroidVoiceService? _androidVoiceService;
    private bool _isTyping = false;
    private bool _voiceOutputEnabled = true;
    
    // Theme colors
    private string _bgPrimary = "#101622";
    private string _bgSecondary = "#1E293B";
    private string _bgCard = "#0DFFFFFF";
    private string _borderColor = "#1AFFFFFF";
    private string _textPrimary = "#FFFFFF";
    private string _textSecondary = "#94A3B8";
    private string _textTertiary = "#64748B";
    private string _statusBarColor = "#0A1628";
    
    // Sidebar and conversation management
    private ChatHistorySidebar? _sidebar;
    private ConversationManager? _conversationManager;
    private ConversationStorageService? _storageService;
    private FrameLayout? _rootLayout;
    
    private List<VIRA.Mobile.Models.QuickAction> _quickActions = new()
    {
        new VIRA.Mobile.Models.QuickAction("☀️", "Weather", "weather"),
        new VIRA.Mobile.Models.QuickAction("📰", "News", "news"),
        new VIRA.Mobile.Models.QuickAction("🔔", "Reminders", "reminder"),
        new VIRA.Mobile.Models.QuickAction("🚗", "Traffic", "traffic"),
        new VIRA.Mobile.Models.QuickAction("☕", "Coffee", "coffee"),
        new VIRA.Mobile.Models.QuickAction("🎵", "Music", "music")
    };
    
    protected override void OnResume()
    {
        base.OnResume();
        
        Android.Util.Log.Info(TAG, "========================================");
        Android.Util.Log.Info(TAG, "OnResume - Reloading API Key and Theme");
        Android.Util.Log.Info(TAG, "========================================");
        
        // Reload theme colors
        LoadThemeColors();
        
        // Reload API key when returning from Settings
        // This ensures new API key is always loaded
        ReloadApiKey();
        
        // Rebuild UI to apply theme changes
        if (_rootLayout != null)
        {
            _rootLayout.RemoveAllViews();
            BuildUI();
            
            // Reload current conversation messages after UI rebuild
            var currentConversation = _conversationManager?.GetCurrentConversation();
            if (currentConversation != null && currentConversation.Messages.Count > 0)
            {
                Android.Util.Log.Info(TAG, $"🔄 Reloading {currentConversation.Messages.Count} messages from current conversation");
                
                // Display all messages from current conversation
                foreach (var msg in currentConversation.Messages)
                {
                    if (msg.Role == ChatMessageRole.User)
                    {
                        AddUserMessage(msg.Content);
                    }
                    else
                    {
                        AddBotMessage(msg.Content);
                    }
                }
                
                ScrollToBottom();
                Android.Util.Log.Info(TAG, "✅ Messages reloaded successfully");
            }
            else
            {
                // No messages - show welcome message
                Android.Util.Log.Info(TAG, "📝 No messages in conversation - showing welcome message");
                AddBotMessage($"{TimeGreeting.GetGreetingEmoji()} {TimeGreeting.GetGreeting()}! Saya Vira, asisten AI pribadi Anda. Ada yang bisa saya bantu?");
            }
        }
    }
    
    private void LoadThemeColors()
    {
        var prefs = GetSharedPreferences("vira_settings", FileCreationMode.Private);
        var theme = prefs?.GetString("theme_preference", "dark") ?? "dark";
        
        Android.Util.Log.Info(TAG, $"🎨 Loading theme: {theme}");
        
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
            
            Android.Util.Log.Info(TAG, "✅ Light theme colors loaded");
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
                
                Android.Util.Log.Info(TAG, "✅ System theme (dark) colors loaded");
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
                
                Android.Util.Log.Info(TAG, "✅ System theme (light) colors loaded");
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
            
            Android.Util.Log.Info(TAG, "✅ Dark theme colors loaded");
        }
    }
    
    protected override void OnPause()
    {
        base.OnPause();
    }
    
    private void ReloadApiKey()
    {
        try
        {
            var prefs = GetSharedPreferences("vira_settings", FileCreationMode.Private);
            var provider = prefs?.GetString("ai_provider", "gemini");
            
            Android.Util.Log.Info(TAG, $"📖 Reading configuration from SharedPreferences...");
            Android.Util.Log.Info(TAG, $"   Provider: {provider}");
            
            // Get API key based on provider
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
            
            if (!string.IsNullOrEmpty(apiKey))
            {
                Android.Util.Log.Info(TAG, $"✅ Found API Key (length: {apiKey.Length})");
                Android.Util.Log.Info(TAG, $"   First 10 chars: {apiKey.Substring(0, Math.Min(10, apiKey.Length))}...");
                
                // Recreate ViewModel with new API key and provider
                var httpClient = new HttpClient();
                IGeminiService chatService;
                
                if (provider == "groq")
                {
                    Android.Util.Log.Info(TAG, "🚀 Using Groq API (Llama 3.3 70B)");
                    chatService = new GroqChatbotService(httpClient);
                }
                else if (provider == "openai")
                {
                    Android.Util.Log.Info(TAG, "🤖 Using OpenAI API (gpt-4o-mini)");
                    chatService = new OpenAIChatbotService(httpClient);
                }
                else
                {
                    Android.Util.Log.Info(TAG, "🤖 Using Gemini API (gemini-2.0-flash)");
                    chatService = new GeminiChatbotService(httpClient);
                }
                
                chatService.SetApiKey(apiKey);
                
                // Reload TTS API key
                var elevenLabsKey = prefs?.GetString("elevenlabs_api_key", null);
                if (!string.IsNullOrEmpty(elevenLabsKey) && _ttsService != null)
                {
                    _ttsService.SetApiKey(elevenLabsKey);
                    Android.Util.Log.Info(TAG, $"✅ ElevenLabs API Key reloaded (length: {elevenLabsKey.Length})");
                }
                
                if (_androidVoiceService == null)
                {
                    _androidVoiceService = new Services.AndroidVoiceService();
                }
                
                var messageProcessor = CreateMessageProcessor(chatService);
                _viewModel = new MainChatViewModel(messageProcessor, chatService, _androidVoiceService, null);
                
                Android.Util.Log.Info(TAG, $"✅ ViewModel recreated with {provider} provider");
                
                // Update status text
                if (_statusText != null)
                {
                    _statusText.Text = "Vira AI";
                }
            }
            else
            {
                Android.Util.Log.Warn(TAG, $"⚠️ No API Key found for provider: {provider}");
            }
        }
        catch (System.Exception ex)
        {
            Android.Util.Log.Error(TAG, $"❌ Error reloading API Key: {ex.Message}");
            Android.Util.Log.Error(TAG, $"   Stack trace: {ex.StackTrace}");
        }
    }
    
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        Android.Util.Log.Info(TAG, "========================================");
        Android.Util.Log.Info(TAG, "OnCreate started - VIRA MainActivity");
        Android.Util.Log.Info(TAG, "========================================");
        
        // Initialize ViewModel directly without relying on Shared.App
        try
        {
            Android.Util.Log.Info(TAG, "Step 1: Creating HttpClient...");
            var httpClient = new HttpClient();
            
            Android.Util.Log.Info(TAG, "Step 2: Loading settings from SharedPreferences...");
            var prefs = GetSharedPreferences("vira_settings", FileCreationMode.Private);
            var provider = prefs?.GetString("ai_provider", "gemini");
            
            // Load voice output setting (default: TRUE for first time)
            // Check if this is first launch (no preference set yet)
            if (!prefs.Contains("voice_output_enabled"))
            {
                // First launch - enable voice by default
                _voiceOutputEnabled = true;
                var editor = prefs?.Edit();
                editor?.PutBoolean("voice_output_enabled", true);
                editor?.Apply();
                Android.Util.Log.Info(TAG, "✅ First launch - Voice output enabled by default");
            }
            else
            {
                // Load saved preference - respect user's choice
                _voiceOutputEnabled = prefs.GetBoolean("voice_output_enabled", true);
                Android.Util.Log.Info(TAG, $"✅ Loaded voice preference from storage: {_voiceOutputEnabled}");
            }
            Android.Util.Log.Info(TAG, $"🔊 Voice output current state: {_voiceOutputEnabled}");
            
            Android.Util.Log.Info(TAG, $"Step 3: Creating ChatService ({provider})...");
            
            // Get API key based on provider
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
            
            // Create ChatService based on provider
            IGeminiService chatService;
            
            if (provider == "groq")
            {
                Android.Util.Log.Info(TAG, "🚀 Using Groq API (Llama 3.3 70B)");
                chatService = new GroqChatbotService(httpClient);
            }
            else if (provider == "openai")
            {
                Android.Util.Log.Info(TAG, "🤖 Using OpenAI API (gpt-4o-mini)");
                chatService = new OpenAIChatbotService(httpClient);
            }
            else
            {
                Android.Util.Log.Info(TAG, "🤖 Using Gemini API (gemini-2.0-flash)");
                chatService = new GeminiChatbotService(httpClient);
            }
            
            if (!string.IsNullOrEmpty(apiKey))
            {
                chatService.SetApiKey(apiKey);
                Android.Util.Log.Info(TAG, $"✅ API Key loaded successfully (length: {apiKey.Length})");
            }
            else
            {
                Android.Util.Log.Warn(TAG, $"⚠️ No API Key found for {provider} - user needs to set it in Settings");
            }
            
            Android.Util.Log.Info(TAG, "Step 4: Creating VoiceService...");
            _androidVoiceService = new Services.AndroidVoiceService();
            
            Android.Util.Log.Info(TAG, "Step 5: Creating TTS Service...");
            var ttsHttpClient = new HttpClient();
            _ttsService = new ElevenLabsTTSService(ttsHttpClient);
            
            // Load ElevenLabs API key
            var elevenLabsKey = prefs?.GetString("elevenlabs_api_key", null);
            
            if (!string.IsNullOrEmpty(elevenLabsKey))
            {
                _ttsService.SetApiKey(elevenLabsKey);
                Android.Util.Log.Info(TAG, $"✅ ElevenLabs API Key loaded (length: {elevenLabsKey.Length})");
                
                // Load and set Voice ID
                var voiceId = prefs?.GetString("elevenlabs_voice_id", "21m00Tcm4TlvDq8ikWAM");
                _ttsService.SetVoiceId(voiceId);
                Android.Util.Log.Info(TAG, $"✅ ElevenLabs Voice ID set: {voiceId}");
            }
            else
            {
                Android.Util.Log.Warn(TAG, "⚠️ No ElevenLabs API Key found - voice output disabled");
                Android.Util.Log.Info(TAG, "💡 User needs to add ElevenLabs API key in Settings for voice output");
            }
            
            Android.Util.Log.Info(TAG, "Step 6: Creating ViewModel...");
            var messageProcessor = CreateMessageProcessor(chatService);
            _viewModel = new MainChatViewModel(messageProcessor, chatService, _androidVoiceService, null);
            
            Android.Util.Log.Info(TAG, $"✅ ViewModel initialized successfully with {provider} provider");
        }
        catch (System.Exception ex)
        {
            Android.Util.Log.Error(TAG, $"❌ Error initializing ViewModel: {ex.Message}");
            Android.Util.Log.Error(TAG, $"Stack trace: {ex.StackTrace}");
        }
        
        // Initialize conversation management
        Android.Util.Log.Info(TAG, "Step 7: Initializing conversation management...");
        _storageService = new ConversationStorageService(this);
        _conversationManager = new ConversationManager(_storageService, this);
        
        // Step 8: Run migration if needed
        Android.Util.Log.Info(TAG, "Step 8: Checking for migration...");
        var migrationManager = new MigrationManager(this, _storageService);
        
        // Check if onboarding should be shown
        if (migrationManager.ShouldShowOnboarding())
        {
            Android.Util.Log.Info(TAG, "🎉 First launch - showing onboarding");
            var onboardingIntent = new Intent(this, typeof(OnboardingActivity));
            StartActivity(onboardingIntent);
        }
        
        if (migrationManager.IsFirstLaunch())
        {
            Android.Util.Log.Info(TAG, "🎉 First launch detected - running migration");
            migrationManager.PerformMigration();
        }
        else if (migrationManager.NeedsMigration())
        {
            Android.Util.Log.Info(TAG, $"🔄 Version change detected - running migration from {migrationManager.GetSavedVersion()} to {migrationManager.GetCurrentVersion()}");
            migrationManager.PerformMigration();
        }
        else
        {
            Android.Util.Log.Info(TAG, $"✅ No migration needed - version {migrationManager.GetCurrentVersion()}");
        }
        
        // Load existing conversations (after migration)
        _conversationManager.LoadAllConversations();
        
        // Create or load default conversation
        var currentConversation = _conversationManager.GetCurrentConversation();
        if (currentConversation == null)
        {
            _conversationManager.CreateNewConversation();
            Android.Util.Log.Info(TAG, "✅ Created new default conversation");
        }
        else
        {
            Android.Util.Log.Info(TAG, $"✅ Loaded existing conversation: {currentConversation.Id}");
        }
        
        // Request permissions
        if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
        {
            RequestPermissions(new[] { 
                Android.Manifest.Permission.RecordAudio,
                Android.Manifest.Permission.Internet
            }, 1);
        }

        // Set status bar color based on theme
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
        {
            Window?.SetStatusBarColor(Android.Graphics.Color.ParseColor(_statusBarColor));
        }
        
        BuildUI();
        
        // Add welcome message only on first launch
        AddBotMessage($"{TimeGreeting.GetGreetingEmoji()} {TimeGreeting.GetGreeting()}! Saya Vira, asisten AI pribadi Anda. Ada yang bisa saya bantu?");
        
        Android.Util.Log.Info(TAG, "OnCreate completed");
    }
    
    private void BuildUI()
    {
        try
        {
            // Root layout - FrameLayout to hold main UI and sidebar overlay
            _rootLayout = new FrameLayout(this)
            {
                LayoutParameters = new ViewGroup.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.MatchParent)
            };
            
            // Main layout
            var mainLayout = new LinearLayout(this)
            {
                Orientation = Orientation.Vertical,
                LayoutParameters = new ViewGroup.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.MatchParent)
            };
            mainLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor(_bgPrimary));
            
            // Header
            var header = CreateHeader();
            mainLayout.AddView(header);
            
            // Chat area
            _scrollView = new ScrollView(this)
            {
                LayoutParameters = new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    0)
                {
                    Weight = 1
                }
            };
            
            _chatLayout = new LinearLayout(this)
            {
                Orientation = Orientation.Vertical,
                LayoutParameters = new ViewGroup.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent)
            };
            _chatLayout.SetPadding(32, 32, 32, 32);
            
            _scrollView.AddView(_chatLayout);
            mainLayout.AddView(_scrollView);
            
            // Quick Actions
            var quickActions = CreateQuickActions();
            mainLayout.AddView(quickActions);
            
            // Input area
            var inputLayout = CreateInputArea();
            mainLayout.AddView(inputLayout);
            
            // Add main layout to root
            _rootLayout.AddView(mainLayout);
            
            // Initialize and add sidebar
            if (_conversationManager != null)
            {
                _sidebar = new ChatHistorySidebar(this, _conversationManager);
                _sidebar.SetListener(new SidebarListener(this));
                _rootLayout.AddView(_sidebar);
                Android.Util.Log.Info(TAG, "✅ Sidebar initialized and added to layout");
            }
            
            SetContentView(_rootLayout);
        }
        catch (System.Exception ex)
        {
            Android.Util.Log.Error(TAG, $"Error creating UI: {ex.Message}");
        }
    }
    
    private LinearLayout CreateHeader()
    {
        var header = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
        };
        
        var headerDrawable = new GradientDrawable();
        headerDrawable.SetColor(Android.Graphics.Color.ParseColor(_bgSecondary));
        header.Background = headerDrawable;
        header.SetPadding(24, 16, 24, 24); // Reduced top padding from 48 to 16
        
        // Top row with menu and settings
        var topRow = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
        };
        topRow.SetGravity(GravityFlags.CenterVertical);
        
        // Menu button - smaller size
        var menuButton = new Button(this)
        {
            Text = "≡",
            LayoutParameters = new LinearLayout.LayoutParams(
                80,  // Reduced from 100
                80)  // Reduced from 100
        };
        menuButton.SetBackgroundColor(Android.Graphics.Color.Transparent);
        menuButton.SetTextColor(Android.Graphics.Color.ParseColor(_textSecondary));
        menuButton.TextSize = 24; // Reduced from 28
        menuButton.SetPadding(0, 0, 0, 0);
        menuButton.Click += OnMenuClick;
        topRow.AddView(menuButton);
        
        // Spacer
        var spacer = new View(this)
        {
            LayoutParameters = new LinearLayout.LayoutParams(0, 0) { Weight = 1 }
        };
        topRow.AddView(spacer);
        
        // Settings button - smaller size
        var settingsButton = new Button(this)
        {
            Text = "⚙",
            LayoutParameters = new LinearLayout.LayoutParams(
                80,  // Reduced from 100
                80)  // Reduced from 100
        };
        settingsButton.SetBackgroundColor(Android.Graphics.Color.Transparent);
        settingsButton.SetTextColor(Android.Graphics.Color.ParseColor(_textSecondary));
        settingsButton.TextSize = 20; // Reduced from 24
        settingsButton.SetPadding(0, 0, 0, 0);
        settingsButton.Click += OnSettingsClick;
        topRow.AddView(settingsButton);
        
        header.AddView(topRow);
        
        // Greeting text
        _greetingText = new TextView(this)
        {
            Text = TimeGreeting.GetGreeting(),
            TextSize = 28, // Slightly smaller
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 12
            }
        };
        _greetingText.SetTextColor(Android.Graphics.Color.ParseColor(_textPrimary));
        _greetingText.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        header.AddView(_greetingText);
        
        // Status row - simplified without voice toggle
        var statusRow = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 12
            }
        };
        statusRow.SetGravity(GravityFlags.CenterVertical);
        
        // Status indicator (purple dot)
        var statusDot = new View(this)
        {
            LayoutParameters = new LinearLayout.LayoutParams(12, 12) // Slightly smaller
            {
                RightMargin = 12
            }
        };
        var dotDrawable = new GradientDrawable();
        dotDrawable.SetShape(ShapeType.Oval);
        dotDrawable.SetColor(Android.Graphics.Color.ParseColor("#8B5CF6"));
        statusDot.Background = dotDrawable;
        statusRow.AddView(statusDot);
        
        _statusText = new TextView(this)
        {
            Text = "Vira AI",
            TextSize = 13,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
        };
        _statusText.SetTextColor(Android.Graphics.Color.ParseColor(_textSecondary));
        statusRow.AddView(_statusText);
        
        header.AddView(statusRow);
        
        return header;
    }
    
    private HorizontalScrollView CreateQuickActions()
    {
        _quickActionsScroll = new HorizontalScrollView(this)
        {
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 16,
                BottomMargin = 16
            }
        };
        _quickActionsScroll.HorizontalScrollBarEnabled = false;
        
        var container = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
        };
        container.SetPadding(32, 0, 32, 0);
        
        foreach (var action in _quickActions)
        {
            var chip = CreateQuickActionChip(action);
            container.AddView(chip);
        }
        
        _quickActionsScroll.AddView(container);
        return _quickActionsScroll;
    }
    
    private Button CreateQuickActionChip(VIRA.Mobile.Models.QuickAction action)
    {
        var chip = new Button(this)
        {
            Text = $"{action.Icon} {action.Label}",
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
            {
                RightMargin = 16
            }
        };
        
        var chipDrawable = new GradientDrawable();
        chipDrawable.SetColor(Android.Graphics.Color.ParseColor(_bgCard));
        chipDrawable.SetCornerRadius(60);
        chipDrawable.SetStroke(2, Android.Graphics.Color.ParseColor(_borderColor));
        chip.Background = chipDrawable;
        chip.SetTextColor(Android.Graphics.Color.ParseColor(_textPrimary));
        chip.TextSize = 14;
        chip.SetPadding(48, 24, 48, 24);
        chip.SetAllCaps(false);
        
        chip.Click += (s, e) => OnQuickActionClick(action.Action);
        
        return chip;
    }
    
    private LinearLayout CreateInputArea()
    {
        var inputLayout = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                BottomMargin = 32,
                LeftMargin = 32,
                RightMargin = 32
            }
        };
        
        var inputContainerDrawable = new GradientDrawable();
        inputContainerDrawable.SetColor(Android.Graphics.Color.ParseColor(_bgCard));
        inputContainerDrawable.SetCornerRadius(96); // 32dp equivalent
        inputContainerDrawable.SetStroke(2, Android.Graphics.Color.ParseColor(_borderColor));
        inputLayout.Background = inputContainerDrawable;
        
        inputLayout.SetPadding(16, 16, 16, 16);
        inputLayout.SetGravity(GravityFlags.CenterVertical);
        
        // Plus button
        _plusButton = new Button(this)
        {
            Text = "+",
            LayoutParameters = new LinearLayout.LayoutParams(
                120,
                120)
        };
        var plusDrawable = new GradientDrawable();
        plusDrawable.SetColor(Android.Graphics.Color.ParseColor(_bgCard));
        plusDrawable.SetCornerRadius(60);
        _plusButton.Background = plusDrawable;
        _plusButton.SetTextColor(Android.Graphics.Color.ParseColor(_textSecondary));
        _plusButton.TextSize = 28;
        _plusButton.Click += OnPlusClick;
        inputLayout.AddView(_plusButton);
        
        // Input box
        _inputBox = new EditText(this)
        {
            Hint = "Ask Vira anything...",
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                120)
            {
                Weight = 1,
                LeftMargin = 16,
                RightMargin = 16
            }
        };
        var inputDrawable = new GradientDrawable();
        inputDrawable.SetColor(Android.Graphics.Color.Transparent);
        _inputBox.Background = inputDrawable;
        _inputBox.SetTextColor(Android.Graphics.Color.ParseColor(_textPrimary));
        _inputBox.SetHintTextColor(Android.Graphics.Color.ParseColor(_textTertiary));
        _inputBox.SetPadding(48, 0, 48, 0);
        _inputBox.Gravity = GravityFlags.CenterVertical;
        _inputBox.TextChanged += OnInputTextChanged;
        inputLayout.AddView(_inputBox);
        
        // Voice button
        _voiceButton = new Button(this)
        {
            Text = "🎤",
            LayoutParameters = new LinearLayout.LayoutParams(
                120,
                120)
            {
                RightMargin = 8
            }
        };
        var voiceDrawable = new GradientDrawable();
        voiceDrawable.SetColor(Android.Graphics.Color.ParseColor(_bgCard));
        voiceDrawable.SetCornerRadius(60);
        _voiceButton.Background = voiceDrawable;
        _voiceButton.SetTextColor(Android.Graphics.Color.ParseColor(_textPrimary));
        _voiceButton.TextSize = 24;
        _voiceButton.Click += OnVoiceButtonClick;
        inputLayout.AddView(_voiceButton);
        
        // Send button
        _sendButton = new Button(this)
        {
            Text = "➤",
            LayoutParameters = new LinearLayout.LayoutParams(
                120,
                120)
        };
        var sendDrawable = new GradientDrawable();
        sendDrawable.SetOrientation(GradientDrawable.Orientation.LeftRight);
        sendDrawable.SetColors(new int[] { Android.Graphics.Color.ParseColor("#8B5CF6"), Android.Graphics.Color.ParseColor("#A855F7") });
        sendDrawable.SetCornerRadius(60);
        _sendButton.Background = sendDrawable;
        _sendButton.SetTextColor(Android.Graphics.Color.White);
        _sendButton.TextSize = 24;
        _sendButton.Enabled = false;
        _sendButton.Click += OnSendButtonClick;
        inputLayout.AddView(_sendButton);
        
        return inputLayout;
    }
    
    private void OnInputTextChanged(object? sender, Android.Text.TextChangedEventArgs e)
    {
        if (_sendButton != null)
        {
            var hasText = !string.IsNullOrWhiteSpace(_inputBox?.Text);
            _sendButton.Enabled = hasText;
            
            var sendDrawable = new GradientDrawable();
            sendDrawable.SetColor(Android.Graphics.Color.ParseColor(hasText ? "#3B82F6" : "#1E293B"));
            sendDrawable.SetCornerRadius(60);
            _sendButton.Background = sendDrawable;
        }
    }
    
    private void AddUserMessage(string text)
    {
        var card = CreateMessageCard(text, true, DateTime.Now);
        _chatLayout?.AddView(card);
        ScrollToBottom();
    }
    
    private void AddBotMessage(string text)
    {
        var card = CreateMessageCard(text, false, DateTime.Now);
        _chatLayout?.AddView(card);
        ScrollToBottom();
    }
    
    private void AddBotMessageWithCard(ChatMessage message)
    {
        View? card = null;
        
        // CRITICAL: ALWAYS display message.Content from API
        // Rich cards use mock data which doesn't match API response
        // So we ALWAYS display the actual API response text instead
        
        Android.Util.Log.Info(TAG, $"📝 AddBotMessageWithCard called");
        Android.Util.Log.Info(TAG, $"   Message Type: {message.Type}");
        Android.Util.Log.Info(TAG, $"   Content: {message.Content}");
        
        // ALWAYS use text display for ALL message types
        // This ensures API response is shown, not mock data
        card = CreateMessageCard(message.Content, false, message.Timestamp);
        
        if (card != null)
        {
            _chatLayout?.AddView(card);
            ScrollToBottom();
        }
    }
    
    private void ShowTypingIndicator()
    {
        if (_isTyping) return;
        _isTyping = true;
        
        var typingCard = CreateTypingIndicator();
        typingCard.Tag = "typing_indicator";
        _chatLayout?.AddView(typingCard);
        ScrollToBottom();
    }
    
    private void HideTypingIndicator()
    {
        _isTyping = false;
        
        if (_chatLayout == null) return;
        
        for (int i = 0; i < _chatLayout.ChildCount; i++)
        {
            var child = _chatLayout.GetChildAt(i);
            if (child?.Tag?.ToString() == "typing_indicator")
            {
                _chatLayout.RemoveView(child);
                break;
            }
        }
    }
    
    private View CreateTypingIndicator()
    {
        var container = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                BottomMargin = 32
            }
        };
        container.SetGravity(GravityFlags.Left);
        
        var card = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
        };
        
        var cardDrawable = new GradientDrawable();
        cardDrawable.SetColor(Android.Graphics.Color.ParseColor(_bgCard));
        cardDrawable.SetStroke(2, Android.Graphics.Color.ParseColor(_borderColor));
        cardDrawable.SetCornerRadius(48);
        card.Background = cardDrawable;
        card.SetPadding(48, 32, 48, 32);
        card.SetGravity(GravityFlags.Center);
        
        var typingText = new TextView(this)
        {
            Text = "●  ●  ●",
            TextSize = 20
        };
        typingText.SetTextColor(Android.Graphics.Color.ParseColor(_textTertiary));
        
        card.AddView(typingText);
        container.AddView(card);
        
        return container;
    }
    
    private View CreateMessageCard(string text, bool isUser, DateTime timestamp)
    {
        var cardContainer = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                BottomMargin = 32
            }
        };
        cardContainer.SetGravity(isUser ? GravityFlags.Right : GravityFlags.Left);
        
        // Label (Vira or You)
        var label = new TextView(this)
        {
            Text = isUser ? "You" : "Vira",
            TextSize = 11,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
            {
                BottomMargin = 4
            }
        };
        label.SetTextColor(Android.Graphics.Color.ParseColor(_textSecondary));
        cardContainer.AddView(label);
        
        var card = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
        };
        
        var cardDrawable = new GradientDrawable();
        if (isUser)
        {
            cardDrawable.SetOrientation(GradientDrawable.Orientation.LeftRight);
            cardDrawable.SetColors(new int[] { Android.Graphics.Color.ParseColor("#8B5CF6"), Android.Graphics.Color.ParseColor("#A855F7") });
            cardDrawable.SetCornerRadii(new float[] { 48, 48, 48, 48, 4, 4, 48, 48 }); // rounded-t-2xl (16dp->~48px), rounded-br-[2px], rounded-bl-2xl
        }
        else
        {
            cardDrawable.SetColor(Android.Graphics.Color.ParseColor(_bgCard));
            cardDrawable.SetStroke(2, Android.Graphics.Color.ParseColor(_borderColor));
            cardDrawable.SetCornerRadii(new float[] { 48, 48, 48, 48, 48, 48, 4, 4 }); // rounded-t-2xl, rounded-br-2xl, rounded-bl-[2px]
        }
        
        card.Background = cardDrawable;
        card.SetPadding(48, 36, 48, 36);
        
        var textView = new TextView(this)
        {
            Text = text,
            TextSize = 16,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
        };
        textView.SetTextColor(Android.Graphics.Color.ParseColor(_textPrimary));
        textView.SetMaxWidth(800);
        
        card.AddView(textView);
        cardContainer.AddView(card);
        
        // Timestamp
        var timeText = new TextView(this)
        {
            Text = timestamp.ToString("HH:mm"),
            TextSize = 11,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 8
            }
        };
        timeText.SetTextColor(Android.Graphics.Color.ParseColor(_textTertiary));
        cardContainer.AddView(timeText);
        
        return cardContainer;
    }
    
    private async void OnSendButtonClick(object? sender, System.EventArgs e)
    {
        var message = _inputBox?.Text?.Trim();
        if (string.IsNullOrEmpty(message)) return;
        
        Android.Util.Log.Info(TAG, "========================================");
        Android.Util.Log.Info(TAG, $"📤 Sending message: {message}");
        Android.Util.Log.Info(TAG, "========================================");
        
        // Clear input
        if (_inputBox != null)
            _inputBox.Text = "";
        
        // Add user message
        AddUserMessage(message);
        
        // Save user message to conversation
        var userChatMessage = new ChatMessage
        {
            Content = message,
            Role = ChatMessageRole.User,
            Timestamp = DateTime.Now,
            Type = MessageType.Text
        };
        _conversationManager?.AddMessageToCurrentConversation(userChatMessage);
        
        // Track question
        StatsTracker.IncrementQuestions(this);
        Android.Util.Log.Info(TAG, "✅ Question tracked");
        
        // Show typing indicator
        ShowTypingIndicator();
        Android.Util.Log.Info(TAG, "✅ Typing indicator shown");
        
        // Disable send button
        if (_sendButton != null)
            _sendButton.Enabled = false;
        
        try
        {
            // Check if ViewModel is available
            if (_viewModel == null)
            {
                Android.Util.Log.Error(TAG, "❌ ViewModel is null!");
                HideTypingIndicator();
                AddBotMessage("Maaf, terjadi kesalahan sistem. Silakan restart aplikasi.");
                return;
            }
            
            Android.Util.Log.Info(TAG, "✅ ViewModel is available");
            
            // Detect intent
            var intent = KeywordDetector.DetectIntent(message);
            Android.Util.Log.Info(TAG, $"🎯 Detected intent: {intent}");
            
            // Set InputText in ViewModel
            _viewModel.InputText = message;
            Android.Util.Log.Info(TAG, $"✅ Set ViewModel.InputText = {message}");
            
            // Send to API and wait for response
            Android.Util.Log.Info(TAG, "📡 Calling ViewModel.SendMessageAsync...");
            await _viewModel.SendMessageAsync();
            Android.Util.Log.Info(TAG, "✅ SendMessageAsync completed");
            
            // Hide typing indicator
            HideTypingIndicator();
            
            // Get response
            var lastMessage = _viewModel.Messages.LastOrDefault();
            if (lastMessage != null && lastMessage.Role == ChatMessageRole.Assistant)
            {
                Android.Util.Log.Info(TAG, "========================================");
                Android.Util.Log.Info(TAG, $"✅ Got response from API");
                Android.Util.Log.Info(TAG, $"   Message Count in ViewModel: {_viewModel.Messages.Count}");
                Android.Util.Log.Info(TAG, $"   Last Message Role: {lastMessage.Role}");
                Android.Util.Log.Info(TAG, $"   Last Message Type: {lastMessage.Type}");
                Android.Util.Log.Info(TAG, $"   Content length: {lastMessage.Content.Length}");
                Android.Util.Log.Info(TAG, $"   Content FULL TEXT:");
                Android.Util.Log.Info(TAG, $"   >>> {lastMessage.Content}");
                Android.Util.Log.Info(TAG, "========================================");
                
                // CRITICAL FIX: Use the SAME content for both display and voice
                // This ensures text displayed matches what is spoken
                var responseContent = lastMessage.Content;
                
                // Double-check content is not empty
                if (string.IsNullOrWhiteSpace(responseContent))
                {
                    Android.Util.Log.Error(TAG, "❌ ERROR: Response content is empty!");
                    responseContent = "Maaf, saya tidak mendapat respons yang valid dari API.";
                }
                
                Android.Util.Log.Info(TAG, "📝 DISPLAYING TEXT:");
                Android.Util.Log.Info(TAG, $"   >>> {responseContent}");
                
                // Display the message with the correct content
                AddBotMessageWithCard(lastMessage);
                
                // Save bot message to conversation
                _conversationManager?.AddMessageToCurrentConversation(lastMessage);
                
                // Track conversation
                StatsTracker.IncrementConversations(this);
                Android.Util.Log.Info(TAG, "✅ Conversation tracked");
                
                Android.Util.Log.Info(TAG, "🎤 STARTING VOICE SYNTHESIS (BACKGROUND):");
                Android.Util.Log.Info(TAG, $"   >>> {responseContent}");
                
                // Synthesize and play speech in background (fire-and-forget)
                // This allows text to appear immediately without waiting for TTS
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await SynthesizeAndPlaySpeechAsync(responseContent);
                    }
                    catch (Exception ex)
                    {
                        Android.Util.Log.Error(TAG, $"❌ Background TTS error: {ex.Message}");
                    }
                });
            }
            else
            {
                Android.Util.Log.Warn(TAG, "⚠️ No response from ViewModel");
                if (lastMessage != null)
                {
                    Android.Util.Log.Warn(TAG, $"   Last message role: {lastMessage.Role}");
                    Android.Util.Log.Warn(TAG, $"   Last message content: {lastMessage.Content}");
                }
                else
                {
                    Android.Util.Log.Warn(TAG, "   lastMessage is NULL");
                }
                AddBotMessage("Maaf, saya tidak mendapat respons. Pastikan API Key sudah diatur di Settings.");
            }
        }
        catch (System.Exception ex)
        {
            HideTypingIndicator();
            Android.Util.Log.Error(TAG, "========================================");
            Android.Util.Log.Error(TAG, $"❌ ERROR sending message: {ex.Message}");
            Android.Util.Log.Error(TAG, $"❌ Exception type: {ex.GetType().Name}");
            Android.Util.Log.Error(TAG, $"❌ Stack trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Android.Util.Log.Error(TAG, $"❌ Inner exception: {ex.InnerException.Message}");
            }
            Android.Util.Log.Error(TAG, "========================================");
            AddBotMessage($"Maaf, terjadi kesalahan: {ex.Message}. Pastikan API Key Gemini sudah diatur di Settings.");
        }
        finally
        {
            // Re-enable send button if there's text
            if (_sendButton != null && !string.IsNullOrWhiteSpace(_inputBox?.Text))
            {
                _sendButton.Enabled = true;
            }
            Android.Util.Log.Info(TAG, "========================================");
            Android.Util.Log.Info(TAG, "OnSendButtonClick completed");
            Android.Util.Log.Info(TAG, "========================================");
        }
    }
    
    private async Task SynthesizeAndPlaySpeechAsync(string text)
    {
        try
        {
            Android.Util.Log.Info(TAG, "========================================");
            Android.Util.Log.Info(TAG, "🎤 SynthesizeAndPlaySpeechAsync called");
            Android.Util.Log.Info(TAG, $"   Original text length: {text.Length}");
            Android.Util.Log.Info(TAG, $"   Original text preview: {text.Substring(0, Math.Min(100, text.Length))}...");
            
            // Check if voice output is enabled (button toggle)
            if (!_voiceOutputEnabled)
            {
                Android.Util.Log.Info(TAG, "🔇 Voice output disabled by user (toggle button)");
                Android.Util.Log.Info(TAG, "========================================");
                return;
            }
            
            // Clean text for speech (remove markdown, emojis, etc)
            var cleanText = CleanTextForSpeech(text);
            
            Android.Util.Log.Info(TAG, $"   Cleaned text length: {cleanText.Length}");
            Android.Util.Log.Info(TAG, $"   Cleaned text preview: {cleanText.Substring(0, Math.Min(100, cleanText.Length))}...");
            
            // Use Android native TTS with Indonesian female voice
            Android.Util.Log.Info(TAG, "🎤 Using Android native TTS with Indonesian female voice...");
            
            try
            {
                // Create TTS with initialization listener
                var ttsInitialized = false;
                Android.Speech.Tts.TextToSpeech? ttsInstance = null;
                
                var tts = new Android.Speech.Tts.TextToSpeech(this, new TtsInitListener((status) =>
                {
                    if (status == Android.Speech.Tts.OperationResult.Success)
                    {
                        ttsInitialized = true;
                        Android.Util.Log.Info(TAG, "✅ TTS initialized successfully");
                    }
                    else
                    {
                        Android.Util.Log.Error(TAG, $"❌ TTS initialization failed: {status}");
                    }
                }));
                
                ttsInstance = tts;
                
                // Wait for TTS to initialize
                var maxWait = 20; // 2 seconds max
                while (!ttsInitialized && maxWait > 0)
                {
                    await Task.Delay(100);
                    maxWait--;
                }
                
                if (!ttsInitialized)
                {
                    Android.Util.Log.Error(TAG, "❌ TTS initialization timeout");
                    return;
                }
                
                // Set Indonesian locale - FORCE Indonesian language
                var indonesianLocale = new Java.Util.Locale("id", "ID");
                
                // Check if Indonesian is available
                var localeResult = tts.IsLanguageAvailable(indonesianLocale);
                Android.Util.Log.Info(TAG, $"🌐 Indonesian language availability: {localeResult}");
                
                if (localeResult == Android.Speech.Tts.LanguageAvailableResult.MissingData ||
                    localeResult == Android.Speech.Tts.LanguageAvailableResult.NotSupported)
                {
                    // Indonesian not available - prompt user to install
                    Android.Util.Log.Warn(TAG, "⚠️ Indonesian TTS not available - prompting user to install");
                    
                    RunOnUiThread(() =>
                    {
                        new AlertDialog.Builder(this)
                            .SetTitle("Bahasa Indonesia Tidak Tersedia")
                            .SetMessage("TTS bahasa Indonesia belum terinstall. Apakah Anda ingin menginstall sekarang?")
                            .SetPositiveButton("Install", (s, e) =>
                            {
                                // Open TTS settings to install Indonesian
                                var installIntent = new Intent();
                                installIntent.SetAction(Android.Speech.Tts.TextToSpeech.Engine.ActionInstallTtsData);
                                StartActivity(installIntent);
                            })
                            .SetNegativeButton("Nanti", (s, e) => { })
                            .Show();
                    });
                    
                    return;
                }
                
                // Set Indonesian language
                var setLangResult = tts.SetLanguage(indonesianLocale);
                
                if (setLangResult == Android.Speech.Tts.LanguageAvailableResult.Available ||
                    setLangResult == Android.Speech.Tts.LanguageAvailableResult.CountryAvailable ||
                    setLangResult == Android.Speech.Tts.LanguageAvailableResult.CountryVarAvailable)
                {
                    Android.Util.Log.Info(TAG, $"✅ Indonesian language set successfully: {setLangResult}");
                }
                else
                {
                    Android.Util.Log.Warn(TAG, $"⚠️ Failed to set Indonesian language: {setLangResult}");
                }
                
                // Set pitch higher for female voice (1.0 = normal, 1.2 = slightly higher/feminine)
                tts.SetPitch(1.2f);
                Android.Util.Log.Info(TAG, "✅ Pitch set to 1.2 for female voice");
                
                // Set speech rate (0.9 = slightly slower for clarity in Indonesian)
                tts.SetSpeechRate(0.9f);
                Android.Util.Log.Info(TAG, "✅ Speech rate set to 0.9 for clarity");
                
                // Create speech parameters to force Indonesian
                var speechParams = new Dictionary<string, string>();
                speechParams[Android.Speech.Tts.TextToSpeech.Engine.KeyParamUtteranceId] = "vira_tts";
                
                // Speak the text with Indonesian language
                var speakResult = tts.Speak(cleanText, Android.Speech.Tts.QueueMode.Flush, null, "vira_tts");
                
                if (speakResult == Android.Speech.Tts.OperationResult.Success)
                {
                    Android.Util.Log.Info(TAG, "✅ Android native TTS (Indonesian female voice) playback started");
                    Android.Util.Log.Info(TAG, $"   Speaking in Indonesian: {cleanText.Substring(0, Math.Min(50, cleanText.Length))}...");
                }
                else
                {
                    Android.Util.Log.Error(TAG, $"❌ TTS speak failed: {speakResult}");
                }
            }
            catch (Exception nativeTtsEx)
            {
                Android.Util.Log.Error(TAG, $"❌ Android native TTS failed: {nativeTtsEx.Message}");
                Android.Util.Log.Error(TAG, $"   Stack trace: {nativeTtsEx.StackTrace}");
            }
            
            Android.Util.Log.Info(TAG, "========================================");
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error(TAG, $"❌ Error in TTS: {ex.Message}");
            Android.Util.Log.Info(TAG, "========================================");
            // Don't throw - TTS is optional, continue without voice
        }
    }
    
    // TTS initialization listener helper class
    private class TtsInitListener : Java.Lang.Object, Android.Speech.Tts.TextToSpeech.IOnInitListener
    {
        private readonly Action<Android.Speech.Tts.OperationResult> _onInit;
        
        public TtsInitListener(Action<Android.Speech.Tts.OperationResult> onInit)
        {
            _onInit = onInit;
        }
        
        public void OnInit(Android.Speech.Tts.OperationResult status)
        {
            _onInit?.Invoke(status);
        }
    }
    
    private string CleanTextForSpeech(string text)
    {
        // Remove markdown formatting
        text = System.Text.RegularExpressions.Regex.Replace(text, @"\*\*(.*?)\*\*", "$1");
        text = System.Text.RegularExpressions.Regex.Replace(text, @"\*(.*?)\*", "$1");
        text = System.Text.RegularExpressions.Regex.Replace(text, @"__(.*?)__", "$1");
        text = System.Text.RegularExpressions.Regex.Replace(text, @"_(.*?)_", "$1");
        
        // Remove URLs
        text = System.Text.RegularExpressions.Regex.Replace(text, @"https?://\S+", "");
        
        // Remove emojis (most TTS don't handle them well)
        text = System.Text.RegularExpressions.Regex.Replace(text, @"[\u00A0-\u9999\uD800-\uDBFF\uDC00-\uDFFF\uE000-\uF8FF]+", "");
        
        // Remove extra whitespace
        text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ").Trim();
        
        // Limit length (ElevenLabs free tier has limits)
        if (text.Length > 500)
        {
            text = text.Substring(0, 500) + "...";
        }
        
        return text;
    }
    
    private void OnVoiceToggleClick(object? sender, System.EventArgs e)
    {
        // Toggle voice output
        _voiceOutputEnabled = !_voiceOutputEnabled;
        
        Android.Util.Log.Info(TAG, "========================================");
        Android.Util.Log.Info(TAG, $"🎤 Voice toggle clicked!");
        Android.Util.Log.Info(TAG, $"   New state: {_voiceOutputEnabled}");
        
        // Save to preferences
        var prefs = GetSharedPreferences("vira_settings", FileCreationMode.Private);
        var editor = prefs?.Edit();
        editor?.PutBoolean("voice_output_enabled", _voiceOutputEnabled);
        editor?.Apply();
        
        Android.Util.Log.Info(TAG, $"✅ Saved to preferences: voice_output_enabled = {_voiceOutputEnabled}");
        
        // Show toast
        var status = _voiceOutputEnabled ? "AKTIF 🔊" : "NONAKTIF 🔇";
        Toast.MakeText(this, $"Suara: {status}", ToastLength.Short)?.Show();
        
        Android.Util.Log.Info(TAG, $"✅ Voice toggle completed - Voice is now {(_voiceOutputEnabled ? "ON" : "OFF")}");
        Android.Util.Log.Info(TAG, "========================================");
    }
    
    
    private void OnQuickActionClick(string action)
    {
        var message = action switch
        {
            "weather" => "What's the weather like today?",
            "news" => "Show me today's news",
            "reminder" => "Show my reminders",
            "traffic" => "How's the traffic?",
            "coffee" => "Order my usual coffee",
            "music" => "Play my focus playlist",
            _ => ""
        };
        
        if (!string.IsNullOrEmpty(message) && _inputBox != null)
        {
            _inputBox.Text = message;
            OnSendButtonClick(null, System.EventArgs.Empty);
        }
    }
    
    private void OnVoiceButtonClick(object? sender, System.EventArgs e)
    {
        var intent = new Intent(this, typeof(VoiceActiveActivity));
        StartActivityForResult(intent, 100);
    }
    
    private void OnPlusClick(object? sender, System.EventArgs e)
    {
        new AlertDialog.Builder(this)
            .SetTitle("Clear Chat")
            .SetMessage("Are you sure you want to clear all conversations?")
            .SetPositiveButton("Clear", (s, e) =>
            {
                _chatLayout?.RemoveAllViews();
                AddBotMessage($"{TimeGreeting.GetGreetingEmoji()} {TimeGreeting.GetGreeting()}! I'm Vira, your personal AI assistant. How can I help you today?");
            })
            .SetNegativeButton("Cancel", (s, e) => { })
            .Show();
    }
    
    private void OnMenuClick(object? sender, System.EventArgs e)
    {
        // Toggle sidebar visibility
        if (_sidebar != null)
        {
            _sidebar.Show();
            Android.Util.Log.Info(TAG, "📂 Sidebar opened");
        }
    }
    
    private void OnSettingsClick(object? sender, System.EventArgs e)
    {
        var intent = new Intent(this, typeof(SettingsActivity));
        StartActivity(intent);
    }
    
    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        
        if (requestCode == 100 && resultCode == Result.Ok && data != null)
        {
            var transcription = data.GetStringExtra("transcription");
            if (!string.IsNullOrEmpty(transcription) && _inputBox != null)
            {
                _inputBox.Text = transcription;
                OnSendButtonClick(null, System.EventArgs.Empty);
            }
        }
    }
    
    private void ScrollToBottom()
    {
        _scrollView?.Post(() => _scrollView.FullScroll(FocusSearchDirection.Down));
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        
        if (requestCode == 1)
        {
            Android.Util.Log.Info(TAG, "Permissions result received");
        }
    }
    
    // Rich Card Creation Methods
    
    private View CreateWeatherCard(ChatMessage message)
    {
        var container = CreateCardContainer(false);
        
        var card = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
        };
        
        var cardDrawable = new GradientDrawable();
        cardDrawable.SetColor(Android.Graphics.Color.ParseColor("#0DFFFFFF")); cardDrawable.SetStroke(2, Android.Graphics.Color.ParseColor("#1AFFFFFF"));
        cardDrawable.SetCornerRadius(48);
        card.Background = cardDrawable;
        card.SetPadding(48, 32, 48, 32);
        
        // Title
        var title = new TextView(this)
        {
            Text = "🌤 Cuaca Hari Ini",
            TextSize = 18
        };
        title.SetTextColor(Android.Graphics.Color.White);
        title.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        card.AddView(title);
        
        if (message.Weather != null)
        {
            // Temperature
            var temp = new TextView(this)
            {
                Text = message.Weather.Temp,
                TextSize = 48,
                LayoutParameters = new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.WrapContent,
                    ViewGroup.LayoutParams.WrapContent)
                {
                    TopMargin = 16
                }
            };
            temp.SetTextColor(Android.Graphics.Color.White);
            temp.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
            card.AddView(temp);
            
            // Condition
            var condition = new TextView(this)
            {
                Text = message.Weather.Condition,
                TextSize = 16
            };
            condition.SetTextColor(Android.Graphics.Color.ParseColor("#94A3B8"));
            card.AddView(condition);
            
            // Details
            var details = new LinearLayout(this)
            {
                Orientation = Orientation.Horizontal,
                LayoutParameters = new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent)
                {
                    TopMargin = 24
                }
            };
            
            details.AddView(CreateWeatherDetail("💧", "Kelembaban", message.Weather.Humidity));
            details.AddView(CreateWeatherDetail("☀️", "UV Index", message.Weather.UV));
            
            card.AddView(details);
            
            // Tomorrow
            var tomorrow = new TextView(this)
            {
                Text = $"Besok: {message.Weather.Tomorrow}",
                TextSize = 14,
                LayoutParameters = new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.WrapContent,
                    ViewGroup.LayoutParams.WrapContent)
                {
                    TopMargin = 16
                }
            };
            tomorrow.SetTextColor(Android.Graphics.Color.ParseColor("#64748B"));
            card.AddView(tomorrow);
        }
        
        container.AddView(card);
        return container;
    }
    
    private LinearLayout CreateWeatherDetail(string icon, string label, string value)
    {
        var detail = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1
            }
        };
        
        var iconText = new TextView(this)
        {
            Text = icon,
            TextSize = 24
        };
        detail.AddView(iconText);
        
        var labelText = new TextView(this)
        {
            Text = label,
            TextSize = 12
        };
        labelText.SetTextColor(Android.Graphics.Color.ParseColor("#64748B"));
        detail.AddView(labelText);
        
        var valueText = new TextView(this)
        {
            Text = value,
            TextSize = 16
        };
        valueText.SetTextColor(Android.Graphics.Color.White);
        valueText.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        detail.AddView(valueText);
        
        return detail;
    }
    
    private View CreateScheduleCard(ChatMessage message)
    {
        var container = CreateCardContainer(false);
        
        var card = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
        };
        
        var cardDrawable = new GradientDrawable();
        cardDrawable.SetColor(Android.Graphics.Color.ParseColor("#0DFFFFFF")); cardDrawable.SetStroke(2, Android.Graphics.Color.ParseColor("#1AFFFFFF"));
        cardDrawable.SetCornerRadius(48);
        card.Background = cardDrawable;
        card.SetPadding(48, 32, 48, 32);
        
        // Title
        var title = new TextView(this)
        {
            Text = "📅 Jadwal Hari Ini",
            TextSize = 18
        };
        title.SetTextColor(Android.Graphics.Color.White);
        title.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        card.AddView(title);
        
        if (message.Schedule != null)
        {
            foreach (var item in message.Schedule)
            {
                var scheduleItem = CreateScheduleItem(item);
                card.AddView(scheduleItem);
            }
        }
        
        container.AddView(card);
        return container;
    }
    
    private LinearLayout CreateScheduleItem(ScheduleItem item)
    {
        var itemLayout = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 24
            }
        };
        itemLayout.SetGravity(GravityFlags.CenterVertical);
        
        // Color indicator
        var indicator = new View(this)
        {
            LayoutParameters = new LinearLayout.LayoutParams(8, 80)
            {
                RightMargin = 24
            }
        };
        var indicatorDrawable = new GradientDrawable();
        indicatorDrawable.SetColor(Android.Graphics.Color.ParseColor(item.Color));
        indicatorDrawable.SetCornerRadius(4);
        indicator.Background = indicatorDrawable;
        itemLayout.AddView(indicator);
        
        // Content
        var content = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1
            }
        };
        
        var time = new TextView(this)
        {
            Text = item.Time,
            TextSize = 14
        };
        time.SetTextColor(Android.Graphics.Color.ParseColor("#64748B"));
        content.AddView(time);
        
        var titleText = new TextView(this)
        {
            Text = item.Title,
            TextSize = 16
        };
        titleText.SetTextColor(Android.Graphics.Color.White);
        titleText.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        content.AddView(titleText);
        
        var location = new TextView(this)
        {
            Text = item.Location,
            TextSize = 14
        };
        location.SetTextColor(Android.Graphics.Color.ParseColor("#94A3B8"));
        content.AddView(location);
        
        itemLayout.AddView(content);
        
        return itemLayout;
    }
    
    private View CreateNewsCard(ChatMessage message)
    {
        var container = CreateCardContainer(false);
        
        var card = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
        };
        
        var cardDrawable = new GradientDrawable();
        cardDrawable.SetColor(Android.Graphics.Color.ParseColor("#0DFFFFFF")); cardDrawable.SetStroke(2, Android.Graphics.Color.ParseColor("#1AFFFFFF"));
        cardDrawable.SetCornerRadius(48);
        card.Background = cardDrawable;
        card.SetPadding(48, 32, 48, 32);
        
        // Title
        var title = new TextView(this)
        {
            Text = "📰 Berita Terkini",
            TextSize = 18
        };
        title.SetTextColor(Android.Graphics.Color.White);
        title.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        card.AddView(title);
        
        if (message.NewsItems != null)
        {
            foreach (var item in message.NewsItems)
            {
                var newsItem = CreateNewsItem(item);
                card.AddView(newsItem);
            }
        }
        
        container.AddView(card);
        return container;
    }
    
    private LinearLayout CreateNewsItem(NewsItem item)
    {
        var itemLayout = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 24
            }
        };
        
        var category = new TextView(this)
        {
            Text = item.Category,
            TextSize = 12
        };
        category.SetTextColor(Android.Graphics.Color.ParseColor("#3B82F6"));
        itemLayout.AddView(category);
        
        var titleText = new TextView(this)
        {
            Text = item.Title,
            TextSize = 16,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 8
            }
        };
        titleText.SetTextColor(Android.Graphics.Color.White);
        itemLayout.AddView(titleText);
        
        return itemLayout;
    }
    
    private View CreateTrafficCard(ChatMessage message)
    {
        var container = CreateCardContainer(false);
        
        var card = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
        };
        
        var cardDrawable = new GradientDrawable();
        cardDrawable.SetColor(Android.Graphics.Color.ParseColor("#0DFFFFFF")); cardDrawable.SetStroke(2, Android.Graphics.Color.ParseColor("#1AFFFFFF"));
        cardDrawable.SetCornerRadius(48);
        card.Background = cardDrawable;
        card.SetPadding(48, 32, 48, 32);
        
        // Title
        var title = new TextView(this)
        {
            Text = "🚗 Kondisi Lalu Lintas",
            TextSize = 18
        };
        title.SetTextColor(Android.Graphics.Color.White);
        title.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        card.AddView(title);
        
        if (message.TrafficData != null)
        {
            foreach (var route in message.TrafficData)
            {
                var routeItem = CreateTrafficRoute(route);
                card.AddView(routeItem);
            }
        }
        
        container.AddView(card);
        return container;
    }
    
    private LinearLayout CreateTrafficRoute(TrafficRoute route)
    {
        var itemLayout = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 24
            }
        };
        itemLayout.SetGravity(GravityFlags.CenterVertical);
        
        // Content
        var content = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1
            }
        };
        
        var routeText = new TextView(this)
        {
            Text = route.Route,
            TextSize = 16
        };
        routeText.SetTextColor(Android.Graphics.Color.White);
        routeText.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        content.AddView(routeText);
        
        var status = new TextView(this)
        {
            Text = route.Status,
            TextSize = 14
        };
        status.SetTextColor(Android.Graphics.Color.ParseColor(route.Color));
        content.AddView(status);
        
        itemLayout.AddView(content);
        
        // ETA
        var eta = new TextView(this)
        {
            Text = route.ETA,
            TextSize = 20
        };
        eta.SetTextColor(Android.Graphics.Color.White);
        eta.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        itemLayout.AddView(eta);
        
        return itemLayout;
    }
    
    private View CreateReminderCard(ChatMessage message)
    {
        var container = CreateCardContainer(false);
        
        var card = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
        };
        
        var cardDrawable = new GradientDrawable();
        cardDrawable.SetColor(Android.Graphics.Color.ParseColor("#0DFFFFFF")); cardDrawable.SetStroke(2, Android.Graphics.Color.ParseColor("#1AFFFFFF"));
        cardDrawable.SetCornerRadius(48);
        card.Background = cardDrawable;
        card.SetPadding(48, 32, 48, 32);
        
        // Title
        var title = new TextView(this)
        {
            Text = "🔔 Pengingat Aktif",
            TextSize = 18
        };
        title.SetTextColor(Android.Graphics.Color.White);
        title.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        card.AddView(title);
        
        if (message.Reminders != null)
        {
            foreach (var reminder in message.Reminders)
            {
                var reminderItem = CreateReminderItem(reminder);
                card.AddView(reminderItem);
            }
        }
        
        container.AddView(card);
        return container;
    }
    
    private LinearLayout CreateReminderItem(ReminderItem reminder)
    {
        var itemLayout = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                TopMargin = 24
            }
        };
        itemLayout.SetGravity(GravityFlags.CenterVertical);
        
        // Checkbox
        var checkbox = new TextView(this)
        {
            Text = reminder.IsCompleted ? "✅" : "⬜",
            TextSize = 20,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
            {
                RightMargin = 24
            }
        };
        itemLayout.AddView(checkbox);
        
        // Content
        var content = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                0,
                ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1
            }
        };
        
        var time = new TextView(this)
        {
            Text = reminder.Time,
            TextSize = 14
        };
        time.SetTextColor(Android.Graphics.Color.ParseColor("#64748B"));
        content.AddView(time);
        
        var titleText = new TextView(this)
        {
            Text = reminder.Title,
            TextSize = 16
        };
        titleText.SetTextColor(reminder.IsCompleted 
            ? Android.Graphics.Color.ParseColor("#64748B") 
            : Android.Graphics.Color.White);
        if (reminder.IsCompleted)
        {
            titleText.PaintFlags = titleText.PaintFlags | Android.Graphics.PaintFlags.StrikeThruText;
        }
        content.AddView(titleText);
        
        itemLayout.AddView(content);
        
        return itemLayout;
    }
    
    private View CreateCoffeeCard(ChatMessage message)
    {
        var container = CreateCardContainer(false);
        
        var card = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
        };
        
        var cardDrawable = new GradientDrawable();
        cardDrawable.SetColor(Android.Graphics.Color.ParseColor("#0DFFFFFF")); cardDrawable.SetStroke(2, Android.Graphics.Color.ParseColor("#1AFFFFFF"));
        cardDrawable.SetCornerRadius(48);
        card.Background = cardDrawable;
        card.SetPadding(48, 32, 48, 32);
        
        // Title
        var title = new TextView(this)
        {
            Text = "☕ Pesanan Kopi",
            TextSize = 18
        };
        title.SetTextColor(Android.Graphics.Color.White);
        title.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        card.AddView(title);
        
        if (message.Coffee != null)
        {
            // Order details
            var orderText = new TextView(this)
            {
                Text = $"{message.Coffee.Size} {message.Coffee.Type}",
                TextSize = 20,
                LayoutParameters = new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.WrapContent,
                    ViewGroup.LayoutParams.WrapContent)
                {
                    TopMargin = 16
                }
            };
            orderText.SetTextColor(Android.Graphics.Color.White);
            orderText.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
            card.AddView(orderText);
            
            // Location
            var location = new TextView(this)
            {
                Text = $"📍 {message.Coffee.Location}",
                TextSize = 14,
                LayoutParameters = new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.WrapContent,
                    ViewGroup.LayoutParams.WrapContent)
                {
                    TopMargin = 8
                }
            };
            location.SetTextColor(Android.Graphics.Color.ParseColor("#94A3B8"));
            card.AddView(location);
            
            // ETA and Price
            var details = new LinearLayout(this)
            {
                Orientation = Orientation.Horizontal,
                LayoutParameters = new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent)
                {
                    TopMargin = 24
                }
            };
            
            var eta = new TextView(this)
            {
                Text = $"⏱ {message.Coffee.ETA}",
                TextSize = 14,
                LayoutParameters = new LinearLayout.LayoutParams(
                    0,
                    ViewGroup.LayoutParams.WrapContent)
                {
                    Weight = 1
                }
            };
            eta.SetTextColor(Android.Graphics.Color.ParseColor("#3B82F6"));
            details.AddView(eta);
            
            var price = new TextView(this)
            {
                Text = message.Coffee.Price,
                TextSize = 16
            };
            price.SetTextColor(Android.Graphics.Color.White);
            price.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
            details.AddView(price);
            
            card.AddView(details);
            
            // Confirmation
            var confirm = new TextView(this)
            {
                Text = "✅ Pesanan dikonfirmasi",
                TextSize = 14,
                LayoutParameters = new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.WrapContent,
                    ViewGroup.LayoutParams.WrapContent)
                {
                    TopMargin = 16
                }
            };
            confirm.SetTextColor(Android.Graphics.Color.ParseColor("#22C55E"));
            card.AddView(confirm);
        }
        
        container.AddView(card);
        return container;
    }
    
    private View CreateMusicCard(ChatMessage message)
    {
        var container = CreateCardContainer(false);
        
        var card = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
        };
        
        var cardDrawable = new GradientDrawable();
        cardDrawable.SetColor(Android.Graphics.Color.ParseColor("#0DFFFFFF")); cardDrawable.SetStroke(2, Android.Graphics.Color.ParseColor("#1AFFFFFF"));
        cardDrawable.SetCornerRadius(48);
        card.Background = cardDrawable;
        card.SetPadding(48, 32, 48, 32);
        
        // Title
        var title = new TextView(this)
        {
            Text = "🎵 Sedang Diputar",
            TextSize = 18
        };
        title.SetTextColor(Android.Graphics.Color.White);
        title.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        card.AddView(title);
        
        if (message.Music != null)
        {
            // Playlist
            var playlist = new TextView(this)
            {
                Text = message.Music.Playlist,
                TextSize = 14,
                LayoutParameters = new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.WrapContent,
                    ViewGroup.LayoutParams.WrapContent)
                {
                    TopMargin = 16
                }
            };
            playlist.SetTextColor(Android.Graphics.Color.ParseColor("#8B5CF6"));
            card.AddView(playlist);
            
            // Current song
            var song = new TextView(this)
            {
                Text = message.Music.CurrentSong,
                TextSize = 20,
                LayoutParameters = new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.WrapContent,
                    ViewGroup.LayoutParams.WrapContent)
                {
                    TopMargin = 8
                }
            };
            song.SetTextColor(Android.Graphics.Color.White);
            song.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
            card.AddView(song);
            
            // Artist
            var artist = new TextView(this)
            {
                Text = message.Music.Artist,
                TextSize = 16,
                LayoutParameters = new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.WrapContent,
                    ViewGroup.LayoutParams.WrapContent)
                {
                    TopMargin = 4
                }
            };
            artist.SetTextColor(Android.Graphics.Color.ParseColor("#94A3B8"));
            card.AddView(artist);
            
            // Total songs
            var total = new TextView(this)
            {
                Text = $"{message.Music.TotalSongs} lagu dalam playlist",
                TextSize = 14,
                LayoutParameters = new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.WrapContent,
                    ViewGroup.LayoutParams.WrapContent)
                {
                    TopMargin = 16
                }
            };
            total.SetTextColor(Android.Graphics.Color.ParseColor("#64748B"));
            card.AddView(total);
        }
        
        container.AddView(card);
        return container;
    }
    
    private LinearLayout CreateCardContainer(bool isUser)
    {
        var cardContainer = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                BottomMargin = 32
            }
        };
        cardContainer.SetGravity(isUser ? GravityFlags.Right : GravityFlags.Left);
        
        // Label (Vira or You)
        var label = new TextView(this)
        {
            Text = isUser ? "You" : "Vira",
            TextSize = 12,
            LayoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
            {
                BottomMargin = 12
            }
        };
        label.SetTextColor(Android.Graphics.Color.ParseColor("#64748B"));
        cardContainer.AddView(label);
        
        return cardContainer;
    }
    
    private HybridMessageProcessor CreateMessageProcessor(IGeminiService chatService)
    {
        // Create all required dependencies
        var httpClient = new HttpClient();
        var taskManager = new TaskManager();
        var weatherService = new WeatherApiService(httpClient);
        var newsService = new NewsApiService(httpClient);
        var patternRegistry = new PatternRegistry(taskManager, weatherService, newsService);
        var ruleProcessor = new RuleBasedProcessor(patternRegistry);
        var prefsService = new PreferencesService();
        
        return new HybridMessageProcessor(ruleProcessor, chatService, chatService, chatService, prefsService);
    }
    
    // Sidebar listener implementation
    private class SidebarListener : Java.Lang.Object, ChatHistorySidebar.ISidebarListener
    {
        private readonly MainActivity _activity;
        
        public SidebarListener(MainActivity activity)
        {
            _activity = activity;
        }
        
        public void OnConversationSelected(string conversationId)
        {
            Android.Util.Log.Info(TAG, $"📂 Conversation selected: {conversationId}");
            _activity.LoadConversation(conversationId);
        }
        
        public void OnNewChatRequested()
        {
            Android.Util.Log.Info(TAG, "📂 New chat requested");
            _activity.CreateNewChat();
        }
        
        public void OnClearHistoryRequested()
        {
            Android.Util.Log.Info(TAG, "📂 Clear history requested");
            _activity.ShowClearHistoryConfirmation();
        }
    }
    
    private void LoadConversation(string conversationId)
    {
        try
        {
            var conversation = _conversationManager?.SwitchToConversation(conversationId);
            if (conversation != null)
            {
                // Clear current chat display
                _chatLayout?.RemoveAllViews();
                
                // Load and display messages from conversation
                foreach (var msg in conversation.Messages)
                {
                    if (msg.Role == ChatMessageRole.User)
                    {
                        AddUserMessage(msg.Content);
                    }
                    else
                    {
                        AddBotMessage(msg.Content);
                    }
                }
                
                ScrollToBottom();
                Android.Util.Log.Info(TAG, $"✅ Loaded conversation with {conversation.Messages.Count} messages");
            }
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error(TAG, $"❌ Error loading conversation: {ex.Message}");
            Toast.MakeText(this, "Gagal memuat percakapan", ToastLength.Short)?.Show();
        }
    }
    
    private void CreateNewChat()
    {
        try
        {
            _conversationManager?.CreateNewConversation();
            
            // Clear current chat display
            _chatLayout?.RemoveAllViews();
            
            // Add welcome message
            AddBotMessage($"{TimeGreeting.GetGreetingEmoji()} {TimeGreeting.GetGreeting()}! Saya Vira, asisten AI pribadi Anda. Ada yang bisa saya bantu?");
            
            Android.Util.Log.Info(TAG, "✅ Created new chat");
            Toast.MakeText(this, "Percakapan baru dibuat", ToastLength.Short)?.Show();
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error(TAG, $"❌ Error creating new chat: {ex.Message}");
        }
    }
    
    private void ShowClearHistoryConfirmation()
    {
        new AlertDialog.Builder(this)
            .SetTitle("Hapus Semua Riwayat")
            .SetMessage("Apakah Anda yakin ingin menghapus semua riwayat percakapan? Tindakan ini tidak dapat dibatalkan.")
            .SetPositiveButton("Hapus", (s, e) =>
            {
                try
                {
                    _conversationManager?.DeleteAllConversations();
                    
                    // Clear current chat display
                    _chatLayout?.RemoveAllViews();
                    
                    // Create new conversation
                    CreateNewChat();
                    
                    // Refresh sidebar
                    _sidebar?.RefreshConversations();
                    
                    Android.Util.Log.Info(TAG, "✅ Cleared all conversation history");
                    Toast.MakeText(this, "Semua riwayat dihapus", ToastLength.Short)?.Show();
                }
                catch (Exception ex)
                {
                    Android.Util.Log.Error(TAG, $"❌ Error clearing history: {ex.Message}");
                    Toast.MakeText(this, "Gagal menghapus riwayat", ToastLength.Short)?.Show();
                }
            })
            .SetNegativeButton("Batal", (s, e) => { })
            .Show();
    }
}
