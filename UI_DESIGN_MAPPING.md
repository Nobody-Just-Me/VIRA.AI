# UI Design Mapping: React → Uno Platform XAML

Dokumen ini menjelaskan bagaimana desain UI dari AI Assistant Mobile UI (React/TypeScript) diadaptasi ke VIRA (Uno Platform/XAML).

## 🎨 Color Palette Mapping

| React (Tailwind) | Uno Platform XAML | Usage |
|------------------|-------------------|-------|
| `bg-[#101622]` | `#101622` (SlateBackground) | Background utama |
| `bg-[#1E293B]` | `#1E293B` (SlateCard) | Card & bubble AI |
| `bg-[#256AF4]` | `#256AF4` (PrimaryBlue) | User bubble & buttons |
| `text-slate-400` | `#94A3B8` (SlateTextSecondary) | Secondary text |
| `text-white` | `#F8FAFC` (SlateText) | Primary text |
| `bg-[#6366F1]` | `#6366F1` (PrimaryPurple) | Accent color |

## 📐 Layout Structure Comparison

### MainChat Page

#### React (MainChat.tsx)
```tsx
<div className="h-screen bg-[#101622]">
  <header>...</header>
  <main className="flex-1 overflow-y-auto">
    {messages.map(msg => <MessageBubble />)}
  </main>
  <div className="absolute bottom-0">
    <QuickActions />
    <InputBox />
  </div>
</div>
```

#### Uno Platform (MainPage.xaml)
```xml
<Grid Background="#101622">
  <Grid.RowDefinitions>
    <RowDefinition Height="Auto"/>      <!-- Header -->
    <RowDefinition Height="*"/>         <!-- Chat Area -->
    <RowDefinition Height="Auto"/>      <!-- Input Area -->
  </Grid.RowDefinitions>
  
  <Grid Grid.Row="0"><!-- Header --></Grid>
  <ScrollViewer Grid.Row="1"><!-- Messages --></ScrollViewer>
  <Grid Grid.Row="2"><!-- Input + Quick Actions --></Grid>
</Grid>
```

## 🧩 Component Mapping

### 1. Message Bubbles

#### React
```tsx
<motion.div className={`
  ${isUser ? 'bg-[#256af4]' : 'bg-white/5 backdrop-blur-md'}
  rounded-t-2xl rounded-br-2xl rounded-bl-[2px]
`}>
  <p>{msg.text}</p>
</motion.div>
```

#### Uno Platform
```xml
<Border Background="#256AF4"
        CornerRadius="16,16,2,16"
        Padding="16,12">
  <TextBlock Text="{x:Bind Content}"/>
</Border>
```

**Adaptasi:**
- `motion.div` → `Border` dengan `BackgroundTransition`
- `backdrop-blur-md` → `Opacity="0.9"` (XAML tidak support blur langsung)
- `rounded-t-2xl` → `CornerRadius="16,16,16,2"` (TopLeft, TopRight, BottomRight, BottomLeft)

### 2. Quick Actions

#### React
```tsx
<button className="
  bg-white/5 backdrop-blur-md 
  border border-white/10 
  px-3.5 py-1.5 rounded-full
">
  <Sun className="w-3.5 h-3.5" />
  <span>Weather</span>
</button>
```

#### Uno Platform
```xml
<Button Style="{StaticResource QuickActionButtonStyle}"
        Content="🌤️ Cuaca"
        Command="{x:Bind ViewModel.ExecuteQuickActionCommand}"
        CommandParameter="Bagaimana cuaca hari ini?"/>

<!-- Style Definition -->
<Style x:Key="QuickActionButtonStyle" TargetType="Button">
  <Setter Property="Background" Value="#FFFFFF0D"/>
  <Setter Property="BorderBrush" Value="#FFFFFF1A"/>
  <Setter Property="CornerRadius" Value="20"/>
</Style>
```

**Adaptasi:**
- Lucide icons → Unicode emoji atau FontIcon
- Tailwind classes → XAML Style dengan Setters
- `onClick` → `Command` binding (MVVM pattern)

### 3. Input Box

#### React
```tsx
<div className="backdrop-blur-md bg-[#101622]/60 rounded-[32px]">
  <button><Plus /></button>
  <input placeholder="Ask Vira..." />
  <button><Mic /></button>
  <button><Send /></button>
</div>
```

#### Uno Platform
```xml
<Grid Background="#101622" Opacity="0.6" CornerRadius="32">
  <Grid.ColumnDefinitions>
    <ColumnDefinition Width="Auto"/>  <!-- New Chat -->
    <ColumnDefinition Width="*"/>     <!-- Input -->
    <ColumnDefinition Width="Auto"/>  <!-- Mic -->
    <ColumnDefinition Width="Auto"/>  <!-- Send -->
  </Grid.ColumnDefinitions>
  
  <Button Grid.Column="0"><!-- Plus --></Button>
  <TextBox Grid.Column="1" PlaceholderText="Tanya Vira..."/>
  <Button Grid.Column="2"><!-- Mic --></Button>
  <Button Grid.Column="3"><!-- Send --></Button>
</Grid>
```

### 4. Typing Indicator

#### React
```tsx
<motion.div
  initial={{ opacity: 0, y: 8 }}
  animate={{ opacity: 1, y: 0 }}
>
  {[0, 0.2, 0.4].map(delay => (
    <motion.div
      animate={{ opacity: [0.3, 1, 0.3], y: [0, -3, 0] }}
      transition={{ duration: 0.8, repeat: Infinity, delay }}
    />
  ))}
</motion.div>
```

#### Uno Platform
```xml
<Border Visibility="{x:Bind ViewModel.IsTyping, Converter={StaticResource BoolToVisibilityConverter}}">
  <StackPanel Orientation="Horizontal">
    <Ellipse Width="6" Height="6">
      <Ellipse.RenderTransform>
        <TranslateTransform x:Name="Dot1"/>
      </Ellipse.RenderTransform>
    </Ellipse>
    <!-- Animate via Storyboard in code-behind -->
  </StackPanel>
</Border>
```

**Adaptasi:**
- Framer Motion → XAML Storyboard animations
- `animate` prop → `DoubleAnimation` in Resources
- Visibility controlled by ViewModel property

## 🎭 Animation Differences

### React (Framer Motion)
```tsx
<motion.div
  initial={{ opacity: 0, y: 12 }}
  animate={{ opacity: 1, y: 0 }}
  transition={{ duration: 0.28 }}
/>
```

### Uno Platform (XAML)
```xml
<Border>
  <Border.Transitions>
    <TransitionCollection>
      <EntranceThemeTransition/>
    </TransitionCollection>
  </Border.Transitions>
</Border>
```

**Catatan:**
- Uno Platform memiliki animasi built-in yang lebih terbatas
- Untuk animasi kompleks, gunakan `Storyboard` di code-behind
- `BackgroundTransition` untuk smooth color changes

## 🔄 State Management

### React (useState)
```tsx
const [messages, setMessages] = useState<Message[]>([]);
const [isTyping, setIsTyping] = useState(false);

const sendMessage = () => {
  setMessages([...messages, newMsg]);
  setIsTyping(true);
};
```

### Uno Platform (MVVM)
```csharp
public partial class MainChatViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isTyping = false;
    
    public ObservableCollection<ChatMessage> Messages { get; } = new();
    
    [RelayCommand]
    private async Task SendMessageAsync()
    {
        Messages.Add(newMsg);
        IsTyping = true;
    }
}
```

**Adaptasi:**
- `useState` → `ObservableProperty` attribute
- Direct state mutation → Property setters with INotifyPropertyChanged
- `onClick` → `RelayCommand` attribute

## 🎨 Glassmorphism Effect

### React
```tsx
className="bg-white/5 backdrop-blur-md border border-white/10"
```

### Uno Platform
```xml
<Border Background="#FFFFFF0D"
        BorderBrush="#FFFFFF1A"
        BorderThickness="1">
  <!-- XAML doesn't support backdrop-filter -->
  <!-- Use Opacity and layering instead -->
</Border>
```

**Workaround:**
- Gunakan `Opacity` untuk transparansi
- Layer multiple borders untuk depth effect
- Acrylic brush (Windows only, tidak di Android)

## 📱 Responsive Design

### React
```tsx
className="max-w-[85%] px-4"
```

### Uno Platform
```xml
<Border MaxWidth="320" Padding="16,12">
  <!-- Fixed pixel values instead of percentages -->
  <!-- Use Grid with * for responsive columns -->
</Border>
```

## 🔤 Typography

| React (Tailwind) | Uno Platform | Notes |
|------------------|--------------|-------|
| `text-[15px]` | `FontSize="15"` | Direct mapping |
| `font-semibold` | `FontWeight="SemiBold"` | Enum values |
| `leading-relaxed` | `LineHeight="1.5"` | Relative values |
| `tracking-tight` | `CharacterSpacing="-20"` | Different units |

## 🎯 Key Differences Summary

| Aspect | React/Tailwind | Uno Platform XAML |
|--------|----------------|-------------------|
| **Styling** | Utility classes | Styles & Resources |
| **State** | Hooks (useState) | MVVM (ObservableProperty) |
| **Events** | onClick, onChange | Command binding |
| **Animation** | Framer Motion | Storyboard/Transitions |
| **Icons** | Lucide React | FontIcon/Emoji |
| **Layout** | Flexbox/Grid | Grid/StackPanel |
| **Blur** | backdrop-filter | Not supported (use Opacity) |

## 💡 Best Practices

1. **Use Styles**: Define reusable styles in `AppStyles.xaml` instead of inline properties
2. **MVVM Pattern**: Keep UI logic in ViewModels, not code-behind
3. **Data Binding**: Use `{x:Bind}` for better performance than `{Binding}`
4. **Commands**: Use `RelayCommand` instead of event handlers
5. **Converters**: Create value converters for complex bindings (e.g., BoolToVisibility)

## 🔧 Tools for Conversion

- **Tailwind → XAML**: Manual mapping using color codes
- **React Components → XAML**: Identify component hierarchy, convert to Grid/StackPanel
- **Framer Motion → Storyboard**: Recreate animations using DoubleAnimation
- **Icons**: Use FontIcon with Segoe MDL2 Assets or Unicode emoji

## 📚 Resources

- [Uno Platform Docs](https://platform.uno/docs/)
- [XAML Controls Gallery](https://github.com/microsoft/Xaml-Controls-Gallery)
- [WinUI 3 Gallery](https://www.microsoft.com/store/productId/9P3JFPWWDZRC)
- [Tailwind to XAML Converter](https://github.com/your-tool-here)

---

Dengan mapping ini, desain UI dari AI Assistant Mobile dapat diadaptasi dengan baik ke Uno Platform sambil mempertahankan estetika dan user experience yang sama.
