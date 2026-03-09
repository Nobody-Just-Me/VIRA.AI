# Design Document: VIRA UI Redesign

## Overview

This design document specifies the technical implementation for redesigning VIRA's user interface using Uno Platform (C#/XAML) to match the modern design from the "AI Assistant Mobile UI" reference. The implementation will recreate the exact visual design, animations, and user experience while maintaining VIRA's existing service architecture.

The design follows a component-based architecture with reusable XAML resources, custom controls, and animation storyboards. All UI components will be implemented as UserControls or custom controls inheriting from Uno Platform base classes.

### Key Design Principles

1. **Resource-Driven Styling**: All colors, brushes, and styles defined in XAML Resource Dictionaries
2. **Component Reusability**: Shared UI elements implemented as reusable UserControls
3. **Animation Performance**: GPU-accelerated animations using CompositionAnimations where possible
4. **MVVM Pattern**: ViewModels for all screens with data binding
5. **Responsive Layout**: Adaptive layouts using Grid, StackPanel, and relative sizing

## Architecture

### Project Structure

```
VIRA.Shared/
├── Resources/
│   ├── Colors.xaml              # Color definitions
│   ├── Brushes.xaml             # Gradient and solid brushes
│   ├── Typography.xaml          # Text styles
│   ├── Styles.xaml              # Control styles
│   └── Animations.xaml          # Reusable storyboards
├── Views/
│   ├── MainChatView.xaml        # Main chat screen
│   ├── VoiceActiveView.xaml     # Voice interaction screen
│   └── Components/
│       ├── MessageBubble.xaml   # Chat message component
│       ├── QuickActionBar.xaml  # Quick action buttons
│       ├── ChatInputArea.xaml   # Input controls
│       ├── VoiceOrb.xaml        # Animated voice orb
│       ├── WaveformVisualizer.xaml  # Audio waveform
│       ├── WeatherCard.xaml     # Weather display
│       ├── NewsCard.xaml        # News display
│       ├── ScheduleCard.xaml    # Schedule display
│       ├── LoadingIndicator.xaml    # Typing indicator
│       ├── ChatHistorySidebar.xaml  # History panel
│       └── PermissionDialog.xaml    # Permission request
├── ViewModels/
│   ├── MainChatViewModel.cs     # Main chat logic
│   └── VoiceActiveViewModel.cs  # Voice mode logic
└── Controls/
    └── AmbientBackground.cs     # Custom gradient background
```


### Technology Stack

- **UI Framework**: Uno Platform 5.x (C#/XAML)
- **Target Platform**: Android (API 21+)
- **Animation**: Uno.UI.Composition APIs + XAML Storyboards
- **Data Binding**: MVVM with INotifyPropertyChanged
- **Navigation**: Uno.Extensions.Navigation
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection

## Components and Interfaces

### 1. Color System (Colors.xaml)

The color system defines all color values as StaticResource entries in a ResourceDictionary.

**Color Definitions:**

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
    <!-- Background Colors -->
    <Color x:Key="PrimaryBackgroundColor">#101622</Color>
    <Color x:Key="VoiceBackgroundColor">#101f22</Color>
    <Color x:Key="CardBackgroundColor">#FFFFFF</Color>
    
    <!-- Accent Colors -->
    <Color x:Key="AccentPurpleColor">#8b5cf6</Color>
    <Color x:Key="AccentCyanColor">#25d1f4</Color>
    <Color x:Key="AccentLightPurpleColor">#a855f7</Color>
    <Color x:Key="AccentLighterPurpleColor">#c084fc</Color>
    <Color x:Key="AccentIndigoColor">#6366f1</Color>
    
    <!-- Text Colors -->
    <Color x:Key="TextPrimaryColor">#FFFFFF</Color>
    <Color x:Key="TextSecondaryColor">#e2e8f0</Color>
    <Color x:Key="TextTertiaryColor">#94a3b8</Color>
    <Color x:Key="TextMutedColor">#64748b</Color>
    <Color x:Key="TextDimColor">#475569</Color>
    
    <!-- Semantic Colors -->
    <Color x:Key="SuccessColor">#22c55e</Color>
    <Color x:Key="ErrorColor">#ef4444</Color>
    <Color x:Key="WarningColor">#eab308</Color>
    
    <!-- Opacity Variants (for overlays) -->
    <Color x:Key="WhiteOverlay5">#0DFFFFFF</Color>   <!-- 5% white -->
    <Color x:Key="WhiteOverlay8">#14FFFFFF</Color>   <!-- 8% white -->
    <Color x:Key="WhiteOverlay10">#1AFFFFFF</Color>  <!-- 10% white -->
    <Color x:Key="WhiteOverlay15">#26FFFFFF</Color>  <!-- 15% white -->
    <Color x:Key="WhiteOverlay20">#33FFFFFF</Color>  <!-- 20% white -->
    
    <!-- Solid Color Brushes -->
    <SolidColorBrush x:Key="PrimaryBackgroundBrush" Color="{StaticResource PrimaryBackgroundColor}"/>
    <SolidColorBrush x:Key="AccentPurpleBrush" Color="{StaticResource AccentPurpleColor}"/>
    <SolidColorBrush x:Key="TextPrimaryBrush" Color="{StaticResource TextPrimaryColor}"/>
    <!-- ... additional brushes ... -->
</ResourceDictionary>
```


### 2. Gradient Brushes (Brushes.xaml)

Gradient brushes for backgrounds, buttons, and effects.

**Brush Definitions:**

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
    <!-- Purple Gradient (for send button, voice orb) -->
    <LinearGradientBrush x:Key="PurpleGradientBrush" StartPoint="0,0" EndPoint="1,1">
        <GradientStop Color="#8b5cf6" Offset="0"/>
        <GradientStop Color="#7c3aed" Offset="1"/>
    </LinearGradientBrush>
    
    <!-- Indigo-Purple Gradient (for action buttons) -->
    <LinearGradientBrush x:Key="IndigoPurpleGradientBrush" StartPoint="0,0" EndPoint="1,1">
        <GradientStop Color="#6366f1" Offset="0"/>
        <GradientStop Color="#9333ea" Offset="1"/>
    </LinearGradientBrush>
    
    <!-- Cyan-Purple Gradient (for voice orb, loading indicator) -->
    <LinearGradientBrush x:Key="CyanPurpleGradientBrush" StartPoint="0,0" EndPoint="1,1">
        <GradientStop Color="#25d1f4" Offset="0"/>
        <GradientStop Color="#c084fc" Offset="1"/>
    </LinearGradientBrush>
    
    <!-- Ambient Gradient 1 (purple glow) -->
    <RadialGradientBrush x:Key="AmbientPurpleGlowBrush" Center="0.5,0.5" RadiusX="0.5" RadiusY="0.5">
        <GradientStop Color="#338b5cf6" Offset="0"/>  <!-- 20% opacity -->
        <GradientStop Color="#008b5cf6" Offset="1"/>  <!-- 0% opacity -->
    </RadialGradientBrush>
    
    <!-- Ambient Gradient 2 (light purple glow) -->
    <RadialGradientBrush x:Key="AmbientLightPurpleGlowBrush" Center="0.5,0.5" RadiusX="0.5" RadiusY="0.5">
        <GradientStop Color="#1Aa855f7" Offset="0"/>  <!-- 10% opacity -->
        <GradientStop Color="#00a855f7" Offset="1"/>  <!-- 0% opacity -->
    </RadialGradientBrush>
    
    <!-- Input Area Gradient (bottom fade) -->
    <LinearGradientBrush x:Key="InputAreaGradientBrush" StartPoint="0,0" EndPoint="0,1">
        <GradientStop Color="#00101622" Offset="0"/>  <!-- transparent -->
        <GradientStop Color="#F2101622" Offset="0.3"/>  <!-- 95% opacity -->
        <GradientStop Color="#101622" Offset="1"/>  <!-- solid -->
    </LinearGradientBrush>
    
    <!-- Voice Orb Inner Gradient -->
    <LinearGradientBrush x:Key="VoiceOrbInnerGradientBrush" StartPoint="0,0" EndPoint="1,1">
        <GradientStop Color="#4025d1f4" Offset="0"/>  <!-- 25% cyan -->
        <GradientStop Color="#40a855f7" Offset="1"/>  <!-- 25% purple -->
    </LinearGradientBrush>
    
    <!-- Voice Orb Outer Gradient -->
    <LinearGradientBrush x:Key="VoiceOrbOuterGradientBrush" StartPoint="0,0" EndPoint="1,1">
        <GradientStop Color="#1A25d1f4" Offset="0"/>  <!-- 10% cyan -->
        <GradientStop Color="#1Aa855f7" Offset="1"/>  <!-- 10% purple -->
    </LinearGradientBrush>
</ResourceDictionary>
```


### 3. Typography System (Typography.xaml)

Text styles for consistent typography throughout the app.

**Style Definitions:**

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
    <!-- Base Font Family -->
    <FontFamily x:Key="PrimaryFontFamily">Segoe UI, Roboto, sans-serif</FontFamily>
    
    <!-- Heading Styles -->
    <Style x:Key="H1TextStyle" TargetType="TextBlock">
        <Setter Property="FontFamily" Value="{StaticResource PrimaryFontFamily}"/>
        <Setter Property="FontSize" Value="24"/>
        <Setter Property="FontWeight" Value="Medium"/>
        <Setter Property="LineHeight" Value="36"/>
        <Setter Property="Foreground" Value="{StaticResource TextPrimaryBrush}"/>
    </Style>
    
    <Style x:Key="H2TextStyle" TargetType="TextBlock">
        <Setter Property="FontFamily" Value="{StaticResource PrimaryFontFamily}"/>
        <Setter Property="FontSize" Value="20"/>
        <Setter Property="FontWeight" Value="Medium"/>
        <Setter Property="LineHeight" Value="30"/>
        <Setter Property="Foreground" Value="{StaticResource TextPrimaryBrush}"/>
    </Style>
    
    <Style x:Key="H3TextStyle" TargetType="TextBlock">
        <Setter Property="FontFamily" Value="{StaticResource PrimaryFontFamily}"/>
        <Setter Property="FontSize" Value="18"/>
        <Setter Property="FontWeight" Value="Medium"/>
        <Setter Property="LineHeight" Value="27"/>
        <Setter Property="Foreground" Value="{StaticResource TextPrimaryBrush}"/>
    </Style>
    
    <!-- Body Text Styles -->
    <Style x:Key="BodyTextStyle" TargetType="TextBlock">
        <Setter Property="FontFamily" Value="{StaticResource PrimaryFontFamily}"/>
        <Setter Property="FontSize" Value="15"/>
        <Setter Property="FontWeight" Value="Normal"/>
        <Setter Property="LineHeight" Value="22.5"/>
        <Setter Property="Foreground" Value="{StaticResource TextSecondaryBrush}"/>
    </Style>
    
    <Style x:Key="SmallTextStyle" TargetType="TextBlock">
        <Setter Property="FontFamily" Value="{StaticResource PrimaryFontFamily}"/>
        <Setter Property="FontSize" Value="13"/>
        <Setter Property="FontWeight" Value="Normal"/>
        <Setter Property="LineHeight" Value="19.5"/>
        <Setter Property="Foreground" Value="{StaticResource TextTertiaryBrush}"/>
    </Style>
    
    <Style x:Key="TinyTextStyle" TargetType="TextBlock">
        <Setter Property="FontFamily" Value="{StaticResource PrimaryFontFamily}"/>
        <Setter Property="FontSize" Value="11"/>
        <Setter Property="FontWeight" Value="Normal"/>
        <Setter Property="LineHeight" Value="16.5"/>
        <Setter Property="Foreground" Value="{StaticResource TextMutedBrush}"/>
    </Style>
    
    <!-- Label Styles -->
    <Style x:Key="LabelTextStyle" TargetType="TextBlock">
        <Setter Property="FontFamily" Value="{StaticResource PrimaryFontFamily}"/>
        <Setter Property="FontSize" Value="11"/>
        <Setter Property="FontWeight" Value="SemiBold"/>
        <Setter Property="CharacterSpacing" Value="30"/>  <!-- 0.3em letter-spacing -->
        <Setter Property="TextTransform" Value="Uppercase"/>
        <Setter Property="Foreground" Value="{StaticResource TextTertiaryBrush}"/>
    </Style>
    
    <!-- Message Bubble Text -->
    <Style x:Key="MessageTextStyle" TargetType="TextBlock">
        <Setter Property="FontFamily" Value="{StaticResource PrimaryFontFamily}"/>
        <Setter Property="FontSize" Value="15"/>
        <Setter Property="FontWeight" Value="Normal"/>
        <Setter Property="LineHeight" Value="22.5"/>
        <Setter Property="TextWrapping" Value="Wrap"/>
    </Style>
    
    <!-- Timestamp Text -->
    <Style x:Key="TimestampTextStyle" TargetType="TextBlock">
        <Setter Property="FontFamily" Value="{StaticResource PrimaryFontFamily}"/>
        <Setter Property="FontSize" Value="10"/>
        <Setter Property="FontWeight" Value="Normal"/>
        <Setter Property="Foreground" Value="{StaticResource TextDimBrush}"/>
    </Style>
</ResourceDictionary>
```


### 4. Control Styles (Styles.xaml)

Reusable styles for buttons, containers, and other controls.

**Button Styles:**

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
    <!-- Icon Button Style (for header buttons) -->
    <Style x:Key="IconButtonStyle" TargetType="Button">
        <Setter Property="Width" Value="36"/>
        <Setter Property="Height" Value="36"/>
        <Setter Property="Background" Value="Transparent"/>
        <Setter Property="BorderThickness" Value="0"/>
        <Setter Property="CornerRadius" Value="12"/>
        <Setter Property="Padding" Value="0"/>
        <Setter Property="HorizontalContentAlignment" Value="Center"/>
        <Setter Property="VerticalContentAlignment" Value="Center"/>
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="Button">
                    <Grid Background="{TemplateBinding Background}" 
                          CornerRadius="{TemplateBinding CornerRadius}">
                        <VisualStateManager.VisualStateGroups>
                            <VisualStateGroup x:Name="CommonStates">
                                <VisualState x:Name="Normal"/>
                                <VisualState x:Name="PointerOver">
                                    <VisualState.Setters>
                                        <Setter Target="RootGrid.Background" Value="{StaticResource WhiteOverlay5Brush}"/>
                                    </VisualState.Setters>
                                </VisualState>
                                <VisualState x:Name="Pressed">
                                    <VisualState.Setters>
                                        <Setter Target="RootGrid.Scale" Value="0.92,0.92,1"/>
                                    </VisualState.Setters>
                                </VisualState>
                            </VisualStateGroup>
                        </VisualStateManager.VisualStateGroups>
                        <ContentPresenter x:Name="ContentPresenter"
                                        HorizontalAlignment="{TemplateBinding HorizontalContentAlignment}"
                                        VerticalAlignment="{TemplateBinding VerticalContentAlignment}"/>
                    </Grid>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>
    
    <!-- Quick Action Button Style -->
    <Style x:Key="QuickActionButtonStyle" TargetType="Button">
        <Setter Property="Background" Value="{StaticResource WhiteOverlay5Brush}"/>
        <Setter Property="BorderBrush" Value="{StaticResource WhiteOverlay10Brush}"/>
        <Setter Property="BorderThickness" Value="1"/>
        <Setter Property="CornerRadius" Value="16"/>
        <Setter Property="Padding" Value="14,6"/>
        <Setter Property="Height" Value="32"/>
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="Button">
                    <Border Background="{TemplateBinding Background}"
                            BorderBrush="{TemplateBinding BorderBrush}"
                            BorderThickness="{TemplateBinding BorderThickness}"
                            CornerRadius="{TemplateBinding CornerRadius}"
                            Padding="{TemplateBinding Padding}">
                        <VisualStateManager.VisualStateGroups>
                            <VisualStateGroup x:Name="CommonStates">
                                <VisualState x:Name="Normal"/>
                                <VisualState x:Name="PointerOver">
                                    <VisualState.Setters>
                                        <Setter Target="RootBorder.Background" Value="{StaticResource WhiteOverlay10Brush}"/>
                                    </VisualState.Setters>
                                </VisualState>
                                <VisualState x:Name="Pressed">
                                    <VisualState.Setters>
                                        <Setter Target="RootBorder.Scale" Value="0.93,0.93,1"/>
                                    </VisualState.Setters>
                                </VisualState>
                            </VisualStateGroup>
                        </VisualStateManager.VisualStateGroups>
                        <StackPanel Orientation="Horizontal" Spacing="6">
                            <ContentPresenter/>
                        </StackPanel>
                    </Border>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>
    
    <!-- Primary Action Button (Send, Voice) -->
    <Style x:Key="PrimaryActionButtonStyle" TargetType="Button">
        <Setter Property="Width" Value="40"/>
        <Setter Property="Height" Value="40"/>
        <Setter Property="Background" Value="{StaticResource PurpleGradientBrush}"/>
        <Setter Property="BorderThickness" Value="0"/>
        <Setter Property="CornerRadius" Value="20"/>
        <Setter Property="Padding" Value="0"/>
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="Button">
                    <Grid>
                        <Ellipse Fill="{TemplateBinding Background}">
                            <Ellipse.Shadow>
                                <ThemeShadow/>
                            </Ellipse.Shadow>
                        </Ellipse>
                        <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"/>
                        <VisualStateManager.VisualStateGroups>
                            <VisualStateGroup x:Name="CommonStates">
                                <VisualState x:Name="Normal"/>
                                <VisualState x:Name="Pressed">
                                    <VisualState.Setters>
                                        <Setter Target="RootGrid.Scale" Value="0.90,0.90,1"/>
                                    </VisualState.Setters>
                                </VisualState>
                                <VisualState x:Name="Disabled">
                                    <VisualState.Setters>
                                        <Setter Target="RootGrid.Opacity" Value="0.4"/>
                                    </VisualState.Setters>
                                </VisualState>
                            </VisualStateGroup>
                        </VisualStateManager.VisualStateGroups>
                    </Grid>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>
</ResourceDictionary>
```


### 5. Message Bubble Component

The MessageBubble is a UserControl that displays chat messages with role-specific styling.

**MessageBubble.xaml:**

```xml
<UserControl x:Class="VIRA.Shared.Views.Components.MessageBubble"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    
    <Grid x:Name="RootGrid" MaxWidth="340" HorizontalAlignment="{x:Bind Alignment, Mode=OneWay}">
        <!-- Sender Label -->
        <TextBlock x:Name="SenderLabel" 
                   Text="{x:Bind SenderName, Mode=OneWay}"
                   Style="{StaticResource TinyTextStyle}"
                   Margin="4,0,0,4"
                   HorizontalAlignment="{x:Bind LabelAlignment, Mode=OneWay}"/>
        
        <!-- Message Container -->
        <Border x:Name="MessageContainer"
                Background="{x:Bind BackgroundBrush, Mode=OneWay}"
                BorderBrush="{x:Bind BorderBrush, Mode=OneWay}"
                BorderThickness="{x:Bind BorderThickness, Mode=OneWay}"
                CornerRadius="{x:Bind CornerRadius, Mode=OneWay}"
                Padding="14"
                Margin="0,20,0,0">
            <Border.Shadow>
                <ThemeShadow/>
            </Border.Shadow>
            
            <!-- Message Content -->
            <ContentPresenter Content="{x:Bind MessageContent, Mode=OneWay}"/>
        </Border>
        
        <!-- Timestamp -->
        <TextBlock x:Name="TimestampLabel"
                   Text="{x:Bind Timestamp, Mode=OneWay}"
                   Style="{StaticResource TimestampTextStyle}"
                   Margin="4,4,0,0"
                   VerticalAlignment="Bottom"
                   HorizontalAlignment="{x:Bind LabelAlignment, Mode=OneWay}"/>
    </Grid>
</UserControl>
```

**MessageBubble.xaml.cs:**

```csharp
public sealed partial class MessageBubble : UserControl
{
    public static readonly DependencyProperty MessageRoleProperty =
        DependencyProperty.Register(nameof(MessageRole), typeof(MessageRole), 
            typeof(MessageBubble), new PropertyMetadata(MessageRole.AI, OnMessageRoleChanged));
    
    public MessageRole MessageRole
    {
        get => (MessageRole)GetValue(MessageRoleProperty);
        set => SetValue(MessageRoleProperty, value);
    }
    
    public string SenderName => MessageRole == MessageRole.User ? "You" : "Vira";
    
    public HorizontalAlignment Alignment => 
        MessageRole == MessageRole.User ? HorizontalAlignment.Right : HorizontalAlignment.Left;
    
    public HorizontalAlignment LabelAlignment => 
        MessageRole == MessageRole.User ? HorizontalAlignment.Right : HorizontalAlignment.Left;
    
    public Brush BackgroundBrush => MessageRole == MessageRole.User 
        ? (Brush)Application.Current.Resources["AccentPurpleBrush"]
        : new SolidColorBrush(Color.FromArgb(0x0D, 0xFF, 0xFF, 0xFF)); // 5% white
    
    public Brush BorderBrush => MessageRole == MessageRole.User
        ? new SolidColorBrush(Colors.Transparent)
        : new SolidColorBrush(Color.FromArgb(0x14, 0xFF, 0xFF, 0xFF)); // 8% white
    
    public Thickness BorderThickness => MessageRole == MessageRole.User 
        ? new Thickness(0) 
        : new Thickness(1);
    
    public CornerRadius CornerRadius => MessageRole == MessageRole.User
        ? new CornerRadius(16, 16, 16, 2)  // top-left, top-right, bottom-right, bottom-left
        : new CornerRadius(16, 16, 2, 16);
    
    private static void OnMessageRoleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (MessageBubble)d;
        control.AnimateIn();
    }
    
    private async void AnimateIn()
    {
        // Entrance animation: fade + slide up + scale
        var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
        var visual = ElementCompositionPreview.GetElementVisual(RootGrid);
        
        var fadeAnimation = compositor.CreateScalarKeyFrameAnimation();
        fadeAnimation.InsertKeyFrame(0f, 0f);
        fadeAnimation.InsertKeyFrame(1f, 1f);
        fadeAnimation.Duration = TimeSpan.FromMilliseconds(280);
        
        var slideAnimation = compositor.CreateVector3KeyFrameAnimation();
        slideAnimation.InsertKeyFrame(0f, new Vector3(0, 12, 0));
        slideAnimation.InsertKeyFrame(1f, new Vector3(0, 0, 0));
        slideAnimation.Duration = TimeSpan.FromMilliseconds(280);
        
        var scaleAnimation = compositor.CreateVector3KeyFrameAnimation();
        scaleAnimation.InsertKeyFrame(0f, new Vector3(0.96f, 0.96f, 1f));
        scaleAnimation.InsertKeyFrame(1f, new Vector3(1f, 1f, 1f));
        scaleAnimation.Duration = TimeSpan.FromMilliseconds(280);
        
        visual.StartAnimation("Opacity", fadeAnimation);
        visual.StartAnimation("Offset", slideAnimation);
        visual.StartAnimation("Scale", scaleAnimation);
    }
}

public enum MessageRole
{
    User,
    AI
}
```

