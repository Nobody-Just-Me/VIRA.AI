# Design Document: VIRA Modern UI Redesign

## Overview

This design document specifies the implementation of a modern, visually stunning UI redesign for VIRA that matches the React/TypeScript reference design found in "AI Assistant Mobile UI" folder. The redesign will be implemented using C#/XAML with Uno Platform, featuring gradient backgrounds, smooth animations, rich content cards, and support for multiple AI providers (Groq, OpenAI, and Gemini).

The design follows a mobile-first approach with a dark theme (#101622 base), purple/indigo gradient accents, and glassmorphism effects. All components will be implemented to achieve 100% visual parity with the reference design while maintaining performance and accessibility standards.

## Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                      VIRA Application                        │
├─────────────────────────────────────────────────────────────┤
│  Presentation Layer (XAML Views + Code-Behind)              │
│  ├─ MainChatView (Chat Interface)                           │
│  ├─ VoiceModeView (Voice Interface with Waveform)           │
│  ├─ SettingsView (Provider Configuration)                   │
│  ├─ ChatHistorySidebar (Conversation History)               │
│  └─ ContentCards (Weather, News, Schedule, Traffic)         │
├─────────────────────────────────────────────────────────────┤
│  Business Logic Layer (Services)                            │
│  ├─ AIProviderManager (Multi-provider orchestration)        │
│  ├─ AnimationService (Smooth transitions)                   │
│  ├─ ThemeService (Color & styling management)               │
│  └─ QuickActionService (Action button handlers)             │
├─────────────────────────────────────────────────────────────┤
│  AI Provider Layer (Abstraction)                            │
│  ├─ IProviderService (Common interface)                     │
│  ├─ GroqProvider (Groq API implementation)                  │
│  ├─ OpenAIProvider (OpenAI API implementation)              │
│  └─ GeminiProvider (Gemini API implementation)              │
├─────────────────────────────────────────────────────────────┤
│  Data Layer                                                  │
│  ├─ ProviderConfigRepository (API keys, settings)           │
│  ├─ MessageRepository (Chat history)                        │
│  └─ PreferencesRepository (User preferences)                │
└─────────────────────────────────────────────────────────────┘
```

### Component Hierarchy

```
App
├─ MainChatView
│  ├─ HeaderComponent
│  │  ├─ GreetingText
│  │  ├─ StatusIndicator
│  │  ├─ MenuButton → ChatHistorySidebar
│  │  └─ SettingsButton → SettingsView
│  ├─ MessageList (ScrollViewer)
│  │  ├─ MessageBubble (User)
│  │  ├─ MessageBubble (AI)
│  │  ├─ WeatherCard
│  │  ├─ NewsCard
│  │  ├─ ScheduleCard
│  │  └─ TrafficCard
│  ├─ QuickActionBar
│  │  └─ QuickActionButton[]
│  └─ FloatingInputArea
│     ├─ NewChatButton
│     ├─ TextInput
│     ├─ VoiceButton → VoiceModeView
│     └─ SendButton
├─ VoiceModeView
│  ├─ VoiceHeader
│  ├─ TranscriptDisplay
│  ├─ WaveformVisualizer (40 bars)
│  └─ VoiceControls
└─ SettingsView
   ├─ ProfileHeader
   ├─ APIConfiguration
   │  ├─ ProviderSelector
   │  ├─ APIKeyInput
   │  └─ ModelSelector
   └─ PreferencesSection
```


## Components and Interfaces

### 1. AI Provider System

#### IProviderService Interface

```csharp
public interface IProviderService
{
    string ProviderName { get; }
    bool IsConfigured { get; }
    Task<bool> ValidateConnectionAsync();
    Task<string> SendMessageAsync(string message, List<Message> history);
    Task<Stream> SendMessageStreamAsync(string message, List<Message> history);
    List<string> GetAvailableModels();
}
```

#### AIProviderManager

```csharp
public class AIProviderManager
{
    private Dictionary<string, IProviderService> _providers;
    private string _activeProvider;
    private string _fallbackProvider;
    
    public async Task<string> SendMessageAsync(string message, List<Message> history)
    {
        try
        {
            var provider = _providers[_activeProvider];
            return await provider.SendMessageAsync(message, history);
        }
        catch (Exception ex)
        {
            // Fallback to secondary provider
            if (!string.IsNullOrEmpty(_fallbackProvider))
            {
                var fallback = _providers[_fallbackProvider];
                return await fallback.SendMessageAsync(message, history);
            }
            throw;
        }
    }
    
    public void SetActiveProvider(string providerName) { }
    public void SetFallbackProvider(string providerName) { }
    public List<string> GetConfiguredProviders() { }
}
```

#### Provider Implementations

```csharp
public class GroqProvider : IProviderService
{
    private string _apiKey;
    private string _model = "mixtral-8x7b-32768";
    
    public async Task<string> SendMessageAsync(string message, List<Message> history)
    {
        // Groq API implementation
        // Fast inference, low latency
    }
}

public class OpenAIProvider : IProviderService
{
    private string _apiKey;
    private string _model = "gpt-4";
    
    public async Task<Stream> SendMessageStreamAsync(string message, List<Message> history)
    {
        // OpenAI streaming implementation
    }
}

public class GeminiProvider : IProviderService
{
    private string _apiKey;
    private string _model = "gemini-2.0-flash";
    
    public async Task<string> SendMessageAsync(string message, List<Message> history)
    {
        // Gemini API implementation
        // Multimodal support
    }
}
```

### 2. Theme and Styling System

#### ThemeService

```csharp
public class ThemeService
{
    public static class Colors
    {
        // Base colors from reference design
        public const string Background = "#101622";
        public const string PurpleGradientStart = "#8b5cf6";
        public const string PurpleGradientEnd = "#7c3aed";
        public const string IndigoAccent = "#6366f1";
        public const string SemiTransparentWhite = "#FFFFFF14"; // 8% opacity
        
        // Message bubbles
        public const string UserMessageGradient = "#8b5cf6";
        public const string AIMessageBackground = "#FFFFFF0D"; // 5% opacity
        
        // Status colors
        public const string SuccessGreen = "#22c55e";
        public const string WarningYellow = "#eab308";
        public const string ErrorRed = "#ef4444";
        public const string InfoBlue = "#2094f3";
    }
    
    public static class Spacing
    {
        public const double Small = 8;
        public const double Medium = 16;
        public const double Large = 24;
        public const double XLarge = 32;
    }
    
    public static class BorderRadius
    {
        public const double Small = 8;
        public const double Medium = 16;
        public const double Large = 24;
        public const double XLarge = 32;
    }
}
```


#### XAML Resource Dictionary

```xml
<ResourceDictionary>
    <!-- Colors -->
    <Color x:Key="BackgroundColor">#101622</Color>
    <Color x:Key="PurpleGradientStart">#8b5cf6</Color>
    <Color x:Key="PurpleGradientEnd">#7c3aed</Color>
    
    <!-- Brushes -->
    <SolidColorBrush x:Key="BackgroundBrush" Color="{StaticResource BackgroundColor}"/>
    
    <LinearGradientBrush x:Key="PurpleGradientBrush" StartPoint="0,0" EndPoint="1,1">
        <GradientStop Color="{StaticResource PurpleGradientStart}" Offset="0"/>
        <GradientStop Color="{StaticResource PurpleGradientEnd}" Offset="1"/>
    </LinearGradientBrush>
    
    <LinearGradientBrush x:Key="AmbientGlowBrush" StartPoint="0,0" EndPoint="0,1">
        <GradientStop Color="#8b5cf6" Opacity="0.2" Offset="0"/>
        <GradientStop Color="Transparent" Offset="1"/>
    </LinearGradientBrush>
    
    <!-- Shadows -->
    <Shadow x:Key="MessageShadow" 
            BlurRadius="24" 
            Opacity="0.35" 
            Color="#8b5cf6" 
            Offset="0,8"/>
    
    <!-- Border Radius -->
    <CornerRadius x:Key="SmallRadius">8</CornerRadius>
    <CornerRadius x:Key="MediumRadius">16</CornerRadius>
    <CornerRadius x:Key="LargeRadius">24</CornerRadius>
    <CornerRadius x:Key="XLargeRadius">32</CornerRadius>
</ResourceDictionary>
```

### 3. Animation System

#### AnimationService

```csharp
public class AnimationService
{
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
    
    public static async Task PulseAsync(UIElement element)
    {
        var scaleAnimation = new DoubleAnimation
        {
            From = 1.0,
            To = 1.1,
            Duration = TimeSpan.FromMilliseconds(500),
            AutoReverse = true,
            RepeatBehavior = RepeatBehavior.Forever,
            EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
        };
        
        var transform = new ScaleTransform { CenterX = 0.5, CenterY = 0.5 };
        element.RenderTransform = transform;
        
        Storyboard.SetTarget(scaleAnimation, transform);
        Storyboard.SetTargetProperty(scaleAnimation, "ScaleX");
        
        var storyboard = new Storyboard();
        storyboard.Children.Add(scaleAnimation);
        storyboard.Begin();
    }
}
```

### 4. Message and Content Card Components

#### Message Model

```csharp
public class Message
{
    public int Id { get; set; }
    public MessageRole Role { get; set; }
    public string Text { get; set; }
    public MessageType Type { get; set; }
    public DateTime Timestamp { get; set; }
    
    // Rich content
    public List<ScheduleItem> Schedule { get; set; }
    public WeatherData Weather { get; set; }
    public List<NewsItem> NewsItems { get; set; }
    public List<TrafficRoute> TrafficData { get; set; }
}

public enum MessageRole { User, AI }
public enum MessageType { Text, Schedule, Weather, News, Reminder, Traffic }
```


#### MessageBubble Component (XAML)

```xml
<UserControl x:Class="VIRA.Shared.Views.MessageBubble">
    <Grid HorizontalAlignment="{x:Bind IsUser ? Right : Left}">
        <Border CornerRadius="16,16,16,2"
                Background="{x:Bind IsUser ? PurpleGradientBrush : AIMessageBrush}"
                Padding="16,14"
                MaxWidth="340">
            <Border.Shadow>
                <Shadow BlurRadius="24" Opacity="0.35" Color="#8b5cf6" Offset="0,8" 
                        Visibility="{x:Bind IsUser ? Visible : Collapsed}"/>
            </Border.Shadow>
            
            <StackPanel Spacing="8">
                <TextBlock Text="{x:Bind Message.Text}" 
                           TextWrapping="Wrap"
                           Foreground="{x:Bind IsUser ? White : #F1F5F9}"
                           FontSize="15"
                           LineHeight="22"/>
                
                <TextBlock Text="{x:Bind FormattedTime}" 
                           FontSize="10"
                           Foreground="#64748b"
                           HorizontalAlignment="Right"/>
            </StackPanel>
        </Border>
    </Grid>
</UserControl>
```

#### WeatherCard Component

```csharp
public class WeatherCard : UserControl
{
    public WeatherData Data { get; set; }
    
    public WeatherCard()
    {
        InitializeComponent();
    }
}
```

```xml
<UserControl x:Class="VIRA.Shared.Views.WeatherCard">
    <Border Background="#FFFFFF0D" 
            BorderBrush="#FFFFFF14" 
            BorderThickness="1"
            CornerRadius="16"
            Padding="16">
        <StackPanel Spacing="12">
            <TextBlock Text="{x:Bind Data.City}" 
                       FontSize="15" 
                       Foreground="#F1F5F9"/>
            
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="Auto"/>
                    <ColumnDefinition Width="*"/>
                </Grid.ColumnDefinitions>
                
                <TextBlock Text="{x:Bind Data.Temp}" 
                           FontSize="48" 
                           FontWeight="Bold"
                           Foreground="White"/>
                
                <StackPanel Grid.Column="1" 
                            VerticalAlignment="Center" 
                            Margin="16,0,0,0">
                    <TextBlock Text="{x:Bind Data.Condition}" 
                               FontSize="14" 
                               Foreground="#F1F5F9"/>
                    <TextBlock Text="{x:Bind Data.Humidity}" 
                               FontSize="12" 
                               Foreground="#94a3b8"/>
                    <TextBlock Text="{x:Bind Data.UV}" 
                               FontSize="12" 
                               Foreground="#94a3b8"/>
                </StackPanel>
            </Grid>
            
            <Border Background="#FFFFFF08" 
                    CornerRadius="8" 
                    Padding="12">
                <TextBlock Text="{x:Bind Data.Tomorrow}" 
                           FontSize="13" 
                           Foreground="#cbd5e1"/>
            </Border>
        </StackPanel>
    </Border>
</UserControl>
```

### 5. Voice Mode with Waveform Visualizer

#### WaveformVisualizer Component

```csharp
public class WaveformVisualizer : UserControl
{
    private const int BarCount = 40;
    private Rectangle[] _bars;
    private DispatcherTimer _animationTimer;
    private bool _isActive;
    private bool _isSpeaking;
    
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            UpdateAnimation();
        }
    }
    
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
        InitializeComponent();
        InitializeBars();
        
        _animationTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(50)
        };
        _animationTimer.Tick += AnimateBars;
    }
    
    private void InitializeBars()
    {
        _bars = new Rectangle[BarCount];
        var container = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 3,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        
        for (int i = 0; i < BarCount; i++)
        {
            var bar = new Rectangle
            {
                Width = 3,
                Height = 4,
                RadiusX = 2,
                RadiusY = 2,
                Fill = new SolidColorBrush(Colors.Gray)
            };
            
            _bars[i] = bar;
            container.Children.Add(bar);
        }
        
        Content = container;
    }
    
    private void AnimateBars(object sender, object e)
    {
        var center = BarCount / 2.0;
        
        for (int i = 0; i < BarCount; i++)
        {
            var dist = Math.Abs(i - center);
            var maxDist = BarCount / 2.0;
            var bellCurve = 1 - Math.Pow(dist / maxDist, 2);
            
            var baseHeight = _isSpeaking ? 40 : 25;
            var noise = Random.Shared.NextDouble() * baseHeight * 0.5;
            var signal = baseHeight * 0.5 + noise;
            var height = Math.Max(4, signal * bellCurve);
            
            _bars[i].Height = height;
            _bars[i].Fill = new SolidColorBrush(
                _isSpeaking ? Color.FromArgb(255, 139, 92, 246) : // Purple
                _isActive ? Color.FromArgb(255, 239, 68, 68) :    // Red
                Color.FromArgb(255, 226, 232, 240)                // Gray
            );
        }
    }
    
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
    
    private void ResetBars()
    {
        for (int i = 0; i < BarCount; i++)
        {
            _bars[i].Height = 4;
            _bars[i].Fill = new SolidColorBrush(Color.FromArgb(255, 226, 232, 240));
        }
    }
}
```


### 6. Chat History Sidebar

#### ChatHistorySidebar Component

```csharp
public class ChatHistorySidebar : UserControl
{
    public bool IsOpen { get; set; }
    public event EventHandler<string> ChatSelected;
    public event EventHandler NewChatRequested;
    public event EventHandler ClearHistoryRequested;
    
    public async Task ShowAsync()
    {
        IsOpen = true;
        Visibility = Visibility.Visible;
        
        // Slide in animation
        var slideAnimation = new DoubleAnimation
        {
            From = -300,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(300),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        
        var transform = new TranslateTransform();
        RenderTransform = transform;
        
        Storyboard.SetTarget(slideAnimation, transform);
        Storyboard.SetTargetProperty(slideAnimation, "X");
        
        var storyboard = new Storyboard();
        storyboard.Children.Add(slideAnimation);
        storyboard.Begin();
        
        await Task.Delay(300);
    }
    
    public async Task HideAsync()
    {
        var slideAnimation = new DoubleAnimation
        {
            From = 0,
            To = -300,
            Duration = TimeSpan.FromMilliseconds(300),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
        };
        
        var transform = RenderTransform as TranslateTransform;
        
        Storyboard.SetTarget(slideAnimation, transform);
        Storyboard.SetTargetProperty(slideAnimation, "X");
        
        var storyboard = new Storyboard();
        storyboard.Children.Add(slideAnimation);
        storyboard.Begin();
        
        await Task.Delay(300);
        IsOpen = false;
        Visibility = Visibility.Collapsed;
    }
}
```

```xml
<UserControl x:Class="VIRA.Shared.Views.ChatHistorySidebar">
    <Grid>
        <!-- Overlay -->
        <Border Background="#00000080" 
                Tapped="OnOverlayTapped"
                Visibility="{x:Bind IsOpen}"/>
        
        <!-- Sidebar -->
        <Border Width="280" 
                HorizontalAlignment="Left"
                Background="#1a1f2e"
                BorderBrush="#FFFFFF14"
                BorderThickness="0,0,1,0">
            <StackPanel>
                <!-- Header -->
                <Border Background="#8b5cf6" Padding="20,16">
                    <TextBlock Text="Chat History" 
                               FontSize="18" 
                               FontWeight="SemiBold"
                               Foreground="White"/>
                </Border>
                
                <!-- New Chat Button -->
                <Button Content="+ New Chat" 
                        Margin="16,16,16,8"
                        Background="#FFFFFF14"
                        Foreground="White"
                        CornerRadius="12"
                        Click="OnNewChatClick"/>
                
                <!-- Chat List -->
                <ScrollViewer>
                    <ItemsRepeater ItemsSource="{x:Bind ChatHistory}">
                        <ItemsRepeater.ItemTemplate>
                            <DataTemplate>
                                <Border Margin="8,4" 
                                        Background="#FFFFFF08"
                                        CornerRadius="8"
                                        Padding="12"
                                        Tapped="OnChatItemTapped">
                                    <StackPanel>
                                        <TextBlock Text="{Binding Title}" 
                                                   FontSize="14" 
                                                   Foreground="White"/>
                                        <TextBlock Text="{Binding Timestamp}" 
                                                   FontSize="11" 
                                                   Foreground="#94a3b8"/>
                                    </StackPanel>
                                </Border>
                            </DataTemplate>
                        </ItemsRepeater.ItemTemplate>
                    </ItemsRepeater>
                </ScrollViewer>
                
                <!-- Clear History -->
                <Button Content="Clear History" 
                        Margin="16"
                        Background="Transparent"
                        Foreground="#ef4444"
                        BorderBrush="#ef4444"
                        BorderThickness="1"
                        CornerRadius="12"
                        Click="OnClearHistoryClick"/>
            </StackPanel>
        </Border>
    </Grid>
</UserControl>
```

### 7. Quick Action System

#### QuickAction Model

```csharp
public class QuickAction
{
    public string Icon { get; set; }
    public string Label { get; set; }
    public string Color { get; set; }
    public string Query { get; set; }
}
```

#### QuickActionService

```csharp
public class QuickActionService
{
    private static readonly List<QuickAction> DefaultActions = new()
    {
        new QuickAction { Icon = "☀️", Label = "Weather", Color = "#eab308", Query = "What's the weather today?" },
        new QuickAction { Icon = "📰", Label = "News", Color = "#2094f3", Query = "Show me today's news" },
        new QuickAction { Icon = "🔔", Label = "Reminders", Color = "#ef4444", Query = "Show my reminders" },
        new QuickAction { Icon = "🚗", Label = "Traffic", Color = "#22c55e", Query = "Check traffic conditions" },
        new QuickAction { Icon = "☕", Label = "Coffee", Color = "#d97706", Query = "Order my usual coffee" },
        new QuickAction { Icon = "🎵", Label = "Music", Color = "#a855f7", Query = "Play some focus music" }
    };
    
    public List<QuickAction> GetQuickActions()
    {
        return DefaultActions;
    }
}
```


## Data Models

### Provider Configuration

```csharp
public class ProviderConfig
{
    public string ProviderName { get; set; }
    public string ApiKey { get; set; }
    public string SelectedModel { get; set; }
    public bool IsActive { get; set; }
    public DateTime LastValidated { get; set; }
}

public class ProviderStatus
{
    public string ProviderName { get; set; }
    public bool IsConnected { get; set; }
    public string StatusMessage { get; set; }
    public DateTime LastChecked { get; set; }
}
```

### Rich Content Models

```csharp
public class WeatherData
{
    public string City { get; set; }
    public string Temp { get; set; }
    public string Condition { get; set; }
    public string Humidity { get; set; }
    public string UV { get; set; }
    public string Tomorrow { get; set; }
}

public class NewsItem
{
    public string Category { get; set; }
    public string Title { get; set; }
}

public class ScheduleItem
{
    public string Time { get; set; }
    public string Title { get; set; }
    public string Location { get; set; }
    public string Color { get; set; }
}

public class TrafficRoute
{
    public string Route { get; set; }
    public string ETA { get; set; }
    public string Status { get; set; }
    public string Color { get; set; }
}
```

### Chat History

```csharp
public class ChatSession
{
    public string Id { get; set; }
    public string Title { get; set; }
    public DateTime Timestamp { get; set; }
    public List<Message> Messages { get; set; }
}
```

## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system—essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

Before writing the correctness properties, I need to analyze the acceptance criteria from the requirements document to determine which are testable as properties.


### Property Reflection

After analyzing all acceptance criteria, I've identified the following redundancies and consolidations:

**Redundancies to eliminate:**
- Properties 2.1, 2.2, 2.3 (greeting time ranges) can be combined into one comprehensive property about time-based greetings
- Properties 6.5, 6.6 (voice button colors) can be combined into one property about state-based coloring
- Properties 5.1, 5.2, 5.3, 5.4 (content cards) all follow the same pattern and can be generalized
- Properties 1.2, 1.3 (message styling) both test message bubble styling and can be combined
- Properties 10.3, 10.4 (provider UI elements) follow the same pattern for all providers

**Properties to keep as separate:**
- Provider routing (9.4) and fallback (9.5) are distinct behaviors
- Animation triggering (1.4) and transition application (12.3) serve different purposes
- Sidebar opening (7.1) and closing (7.6) are inverse operations worth testing separately

**Final property count after consolidation:** Approximately 35 unique, non-redundant properties

### Correctness Properties

Property 1: Message bubble styling consistency
*For any* message in the chat, user messages should have purple gradient background (#8b5cf6) with shadow, and AI messages should have semi-transparent white background with backdrop blur
**Validates: Requirements 1.2, 1.3**

Property 2: Message timestamp display
*For any* message displayed in the chat, a timestamp should be visible below the message bubble
**Validates: Requirements 1.5**

Property 3: Message corner radius consistency
*For any* message bubble, the corner radius should match the design specification (16px with asymmetric corners)
**Validates: Requirements 1.6**

Property 4: Message entrance animation
*For any* new message added to the chat, an entrance animation should be triggered with fade-in and slide-up effects
**Validates: Requirements 1.4**

Property 5: Time-based greeting accuracy
*For any* time of day, the greeting text should correctly display "Good Morning" (5:00-11:59), "Good Afternoon" (12:00-16:59), or "Good Evening" (17:00-4:59)
**Validates: Requirements 2.1, 2.2, 2.3**

Property 6: Header button navigation
*For any* header button (menu or settings), tapping it should navigate to or open the corresponding interface (sidebar or settings page)
**Validates: Requirements 2.5, 2.6**

Property 7: Quick action triggering
*For any* quick action button, tapping it should trigger the corresponding action handler with the correct query
**Validates: Requirements 3.3**

Property 8: Quick action button styling
*For any* quick action button, it should have semi-transparent background with border and category-appropriate colored icon
**Validates: Requirements 3.4**

Property 9: Send button behavior
*For any* non-empty text input, tapping the send button should submit the message and clear the input field
**Validates: Requirements 4.6**

Property 10: Voice mode activation
*For any* voice button tap, the system should navigate to and display the full-screen voice mode interface
**Validates: Requirements 4.7, 6.1**

Property 11: Content card field completeness
*For any* rich content data (weather, news, schedule, traffic), the rendered card should display all required fields specified for that content type
**Validates: Requirements 5.1, 5.2, 5.3, 5.4**

Property 12: Voice button state coloring
*For any* voice mode state (idle, listening, recording), the voice button color should match the state (purple for idle, red for recording, animated for listening)
**Validates: Requirements 6.5, 6.6, 6.4**

Property 13: Transcript display updates
*For any* recognized speech input, the transcript text should be displayed and updated in real-time
**Validates: Requirements 6.7**

Property 14: Speaking indicator visibility
*For any* AI speech output, a speaking indicator with purple accent should be visible during the speech
**Validates: Requirements 6.8**

Property 15: Waveform audio responsiveness
*For any* audio input level change, the waveform visualizer bars should animate to reflect the audio amplitude
**Validates: Requirements 6.9**

Property 16: Sidebar slide animation
*For any* menu button tap, the chat history sidebar should slide in from the left with smooth animation
**Validates: Requirements 7.1**

Property 17: Chat history display
*For any* saved conversation sessions, they should all be displayed in the chat history sidebar list
**Validates: Requirements 7.2**

Property 18: Conversation loading
*For any* conversation selected from history, that conversation's messages should be loaded and the sidebar should close
**Validates: Requirements 7.5**

Property 19: Overlay dismissal
*For any* tap on the sidebar overlay, the sidebar should close with slide-out animation
**Validates: Requirements 7.6**

Property 20: Permission dialog display
*For any* action requiring permissions, a permission rationale dialog should be displayed before requesting the permission
**Validates: Requirements 8.1**

Property 21: Contact disambiguation
*For any* contact name with multiple matches, a contact selection dialog should be displayed with all matching contacts
**Validates: Requirements 8.2**

Property 22: Error notification display
*For any* error that occurs, an error notification should be displayed with error styling (red accent)
**Validates: Requirements 8.3**

Property 23: Success notification display
*For any* successful action completion, a success notification should be displayed with success styling (green accent)
**Validates: Requirements 8.4**

Property 24: Notification auto-dismissal
*For any* notification displayed, it should automatically dismiss after exactly 3 seconds
**Validates: Requirements 8.5**

Property 25: Provider request routing
*For any* AI request when a provider is selected, the request should be routed to that specific provider's API
**Validates: Requirements 9.4**

Property 26: Provider fallback mechanism
*For any* provider request failure, the system should attempt to route the request to the configured fallback provider
**Validates: Requirements 9.5**

Property 27: API key isolation
*For any* provider, its API key should be stored separately and not accessible to other providers
**Validates: Requirements 9.6**

Property 28: API key validation
*For any* API key input, validation should occur before the key is saved to storage
**Validates: Requirements 9.7**

Property 29: Provider UI elements
*For any* configured provider, the settings page should display an API key input field and model selection dropdown for that provider
**Validates: Requirements 10.3, 10.4**

Property 30: Provider status indication
*For any* provider, a status indicator should display its current connection state (connected/disconnected)
**Validates: Requirements 10.5**

Property 31: Connection testing
*For any* "Test Connection" button tap, the system should verify the provider connection and update the status indicator
**Validates: Requirements 10.6**

Property 32: Active provider display
*For any* active provider selection, the provider name should be displayed in the chat header
**Validates: Requirements 10.7**

Property 33: Configuration persistence (Round-trip)
*For any* provider configuration saved, restarting the app and loading the configuration should produce equivalent settings
**Validates: Requirements 10.8**

Property 34: Screen transition animations
*For any* navigation between screens, a smooth fade or slide transition animation should be applied
**Validates: Requirements 12.3**

Property 35: Responsive layout adaptation
*For any* screen width change, the layout should adjust proportionally while maintaining aspect ratios
**Validates: Requirements 13.1, 13.2**

Property 36: Keyboard layout adjustment
*For any* keyboard appearance event, the input area should adjust its position to remain visible above the keyboard
**Validates: Requirements 13.3**

Property 37: Accessibility text presence
*For any* interactive UI element, accessibility text (AutomationProperties.Name) should be defined
**Validates: Requirements 14.1**

Property 38: Touch target sizing
*For any* interactive element, the minimum touch target size should be at least 44x44 pixels
**Validates: Requirements 14.3**

Property 39: Text contrast compliance
*For any* text element, the color contrast ratio between text and background should meet WCAG AA standards (4.5:1 for normal text)
**Validates: Requirements 14.4**

Property 40: Provider-specific configuration
*For any* provider with unique capabilities, those configuration options should be exposed in the settings interface
**Validates: Requirements 15.4, 15.5**


## Error Handling

### Provider Error Handling

```csharp
public class ProviderErrorHandler
{
    public async Task<string> HandleProviderError(Exception ex, string providerName)
    {
        if (ex is HttpRequestException)
        {
            // Network error
            return "Unable to connect to AI service. Please check your internet connection.";
        }
        else if (ex is UnauthorizedAccessException)
        {
            // Invalid API key
            return $"Invalid API key for {providerName}. Please check your settings.";
        }
        else if (ex is TaskCanceledException)
        {
            // Timeout
            return "Request timed out. Please try again.";
        }
        else
        {
            // Generic error
            return $"An error occurred with {providerName}. Please try again later.";
        }
    }
}
```

### UI Error States

1. **Network Errors**: Display error notification with retry option
2. **API Key Errors**: Navigate to settings with error message
3. **Provider Unavailable**: Automatically attempt fallback provider
4. **Animation Errors**: Gracefully degrade to non-animated state
5. **Data Loading Errors**: Display placeholder with error message

### Error Recovery Strategies

```csharp
public class ErrorRecoveryService
{
    public async Task<bool> AttemptRecovery(Exception ex)
    {
        // Try fallback provider
        if (ex is ProviderException)
        {
            return await TryFallbackProvider();
        }
        
        // Retry with exponential backoff
        if (ex is HttpRequestException)
        {
            return await RetryWithBackoff();
        }
        
        // Clear cache and retry
        if (ex is DataException)
        {
            ClearCache();
            return await RetryOperation();
        }
        
        return false;
    }
}
```

## Testing Strategy

### Dual Testing Approach

This project will use both unit tests and property-based tests to ensure comprehensive coverage:

**Unit Tests**: Focus on specific examples, edge cases, and integration points
- Specific UI component rendering
- Provider API integration examples
- Error handling scenarios
- Navigation flows
- Animation trigger verification

**Property-Based Tests**: Verify universal properties across all inputs
- Message styling consistency across all messages
- Time-based greeting accuracy for all times
- Provider routing for all providers
- Content card completeness for all data types
- Accessibility compliance for all interactive elements

### Property-Based Testing Configuration

**Library**: Use FsCheck for C# property-based testing
**Iterations**: Minimum 100 iterations per property test
**Tagging**: Each property test must reference its design document property

Tag format:
```csharp
[Property(DisplayName = "Feature: vira-modern-ui-redesign, Property 1: Message bubble styling consistency")]
public Property MessageBubbleStyling()
{
    // Test implementation
}
```

### Test Organization

```
VIRA.Shared.Tests/
├── UITests/
│   ├── MessageBubbleTests.cs (Unit + Property)
│   ├── HeaderComponentTests.cs (Unit)
│   ├── QuickActionTests.cs (Property)
│   ├── VoiceModeTests.cs (Unit + Property)
│   └── ContentCardTests.cs (Property)
├── ProviderTests/
│   ├── GroqProviderTests.cs (Unit)
│   ├── OpenAIProviderTests.cs (Unit)
│   ├── GeminiProviderTests.cs (Unit)
│   ├── ProviderManagerTests.cs (Property)
│   └── FallbackTests.cs (Property)
├── AnimationTests/
│   ├── AnimationServiceTests.cs (Unit)
│   └── TransitionTests.cs (Property)
└── AccessibilityTests/
    ├── TouchTargetTests.cs (Property)
    ├── ContrastTests.cs (Property)
    └── AccessibilityTextTests.cs (Property)
```

### Key Test Scenarios

**Unit Test Examples:**
1. Verify weather card displays all fields with sample data
2. Test Groq provider sends request with correct API format
3. Verify sidebar slides in when menu button is tapped
4. Test error notification appears on provider failure
5. Verify voice button navigates to voice mode

**Property Test Examples:**
1. For all messages, verify correct styling is applied
2. For all times of day, verify correct greeting is displayed
3. For all providers, verify requests are routed correctly
4. For all content types, verify cards display all required fields
5. For all interactive elements, verify touch targets are adequate

### Performance Testing

While not part of automated tests, manual performance verification should include:
- 60 FPS during scrolling with 100+ messages
- Smooth animations without frame drops
- Voice mode waveform updates at 50ms intervals
- Provider response time < 2 seconds for typical queries
- App startup time < 1 second

### Visual Regression Testing

To ensure 100% visual parity with the reference design:
1. Take screenshots of reference React app
2. Take screenshots of C#/XAML implementation
3. Use image comparison tools to verify pixel-perfect matching
4. Focus on: colors, spacing, border radius, shadows, gradients
5. Test on multiple screen sizes (5", 6", 7"+)

## Implementation Notes

### Uno Platform Considerations

1. **Platform-Specific Code**: Use conditional compilation for platform-specific features
2. **Resource Management**: Use ResourceDictionary for shared styles
3. **Performance**: Use virtualization for long message lists
4. **Animations**: Prefer Composition API for smooth 60 FPS animations
5. **Accessibility**: Use AutomationProperties for screen reader support

### Migration from Existing VIRA

1. **Preserve Data**: Migrate existing chat history and preferences
2. **Gradual Rollout**: Implement feature flags for gradual UI transition
3. **Backward Compatibility**: Maintain existing API contracts
4. **Testing**: Run both old and new UI in parallel during transition
5. **User Feedback**: Collect feedback on new UI before full rollout

### Color Extraction from Reference Design

All colors have been extracted from the React/TypeScript reference:
- Background: #101622
- Purple gradient: #8b5cf6 → #7c3aed
- Indigo accent: #6366f1
- Semi-transparent white: rgba(255,255,255,0.05-0.14)
- Success: #22c55e
- Warning: #eab308
- Error: #ef4444
- Info: #2094f3

### Animation Timing

All animations match the reference design:
- Message entrance: 280ms with cubic ease-out
- Sidebar slide: 300ms with cubic ease
- Button tap: 200ms scale animation
- Waveform update: 50ms interval
- Notification dismiss: 3000ms delay
- Voice button pulse: 500ms with sine ease

