# 🤖 VIRA - Virtual Intelligent Responsive Assistant

<div align="center">

![VIRA Logo](https://img.shields.io/badge/VIRA-AI%20Assistant-8B5CF6?style=for-the-badge&logo=android&logoColor=white)

**Your Personal AI Assistant for Android**

[![Platform](https://img.shields.io/badge/Platform-Android-3DDC84?style=flat-square&logo=android&logoColor=white)](https://www.android.com/)
[![Framework](https://img.shields.io/badge/Framework-Uno%20Platform-512BD4?style=flat-square)](https://platform.uno/)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=.net&logoColor=white)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)](LICENSE)

</div>

---

## 📱 About VIRA

VIRA (Virtual Intelligent Responsive Assistant) is a modern, feature-rich AI assistant application for Android devices. Built with Uno Platform and .NET 8, VIRA provides a seamless conversational experience with advanced AI capabilities, voice synthesis, and a beautiful Material Design-inspired interface.

### ✨ Key Features

- 🤖 **Multi-AI Provider Support**
  - Groq API (Llama 3.3 70B) - Fast & Free
  - Google Gemini API (Gemini 2.0 Flash)
  - Easy switching between providers

- 🎤 **Advanced Voice Features**
  - ElevenLabs TTS integration with natural female voice (Rachel)
  - Voice toggle button (ON/OFF)
  - Background voice synthesis (non-blocking)
  - Speech recognition support

- 💬 **Rich Chat Experience**
  - Clean, modern Material Design UI
  - Real-time typing indicators
  - Message history with timestamps
  - Quick action buttons for common queries

- ⚙️ **Customizable Settings**
  - API provider selection
  - Voice output control
  - Dark mode support
  - Privacy mode options

- 📊 **Usage Statistics**
  - Conversation tracking
  - Question counter
  - Days active tracking

---

## 🚀 Quick Start

### Prerequisites

- **Android Device/Emulator**: Android 7.0 (API 24) or higher
- **.NET SDK**: 8.0 or higher
- **Android SDK**: Latest version
- **API Keys**:
  - Groq API Key (recommended) - [Get it here](https://console.groq.com/keys)
  - OR Gemini API Key - [Get it here](https://aistudio.google.com/apikey)
  - ElevenLabs API Key (optional, for voice) - [Get it here](https://elevenlabs.io/)

### Installation

#### Option 1: Download APK (Easiest)

1. Download the latest `VIRA-Release.apk` from releases
2. Transfer to your Android device
3. Enable "Install from Unknown Sources" in Settings
4. Tap the APK file to install
5. Open VIRA and enter your API key in Settings

#### Option 2: Build from Source

```bash
# Clone the repository
git clone https://github.com/yourusername/VIRA.git
cd VIRA

# Build the APK
bash build.sh

# Install to connected device/emulator
adb install -r VIRA-Release.apk
```

---

## 🛠️ Building from Source

### System Requirements

- **OS**: Linux, macOS, or Windows
- **.NET SDK**: 8.0+
- **Android SDK**: API 34+
- **Java**: JDK 11+

### Build Steps

1. **Install .NET SDK**
   ```bash
   # Ubuntu/Debian
   sudo apt install dotnet-sdk-8.0
   
   # macOS
   brew install dotnet
   
   # Windows
   # Download from https://dotnet.microsoft.com/download
   ```

2. **Install Android SDK**
   ```bash
   # Linux
   wget https://dl.google.com/android/repository/commandlinetools-linux-latest.zip
   unzip commandlinetools-linux-latest.zip -d ~/Android/Sdk
   
   # Or install Android Studio
   ```

3. **Clone and Build**
   ```bash
   git clone https://github.com/yourusername/VIRA.git
   cd VIRA
   
   # Restore dependencies
   dotnet restore
   
   # Build APK
   bash build.sh
   ```

4. **Install APK**
   ```bash
   # Via ADB
   adb install -r VIRA-Release.apk
   
   # Or transfer APK to device manually
   ```

---

## 📖 Usage Guide

### First Time Setup

1. **Launch VIRA** on your Android device
2. **Tap Settings** (⚙️ icon in top-right)
3. **Select AI Provider**:
   - Choose "Groq" (recommended - faster, more quota)
   - Or choose "Gemini"
4. **Enter API Key**:
   - Paste your Groq or Gemini API key
   - Tap "Save Configuration"
5. **Optional: Configure Voice**:
   - Enter ElevenLabs API key for voice output
   - Or leave default key for testing

### Using VIRA

#### Text Chat
1. Type your message in the input box
2. Tap the send button (➤)
3. Wait for VIRA's response
4. Response appears as text + voice (if enabled)

#### Voice Toggle
- **Green 🔊**: Voice output ON
- **Red 🔇**: Voice output OFF
- Tap to toggle

#### Quick Actions
Use quick action buttons for common queries:
- ☀️ Weather
- 📰 News
- 🔔 Reminders
- 🚗 Traffic
- ☕ Coffee
- 🎵 Music

---

## 🎤 Voice Configuration

### ElevenLabs TTS

VIRA uses ElevenLabs for high-quality text-to-speech:

**Default Voice**: Rachel (Female, Natural, Calm)
- Voice ID: `21m00Tcm4TlvDq8ikWAM`
- Free tier compatible
- 10,000 characters/month

### Alternative Voices

Edit `VIRA.Shared/Services/ElevenLabsTTSService.cs` to change voice:

```csharp
// Female Voices (Free Tier)
private const string VoiceId = "21m00Tcm4TlvDq8ikWAM"; // Rachel (current)
// private const string VoiceId = "EXAVITQu4vr4xnSDxMaL"; // Bella
// private const string VoiceId = "MF3mGyEYCl7XYWbV9V6O"; // Elli

// Male Voices (Free Tier)
// private const string VoiceId = "pNInz6obpgDQGcFmaJgB"; // Adam
// private const string VoiceId = "TxGEqnHWrfWFTfGW9XjX"; // Josh
```

---

## 🔧 Configuration

### API Keys

#### Groq API (Recommended)
- **Free Tier**: 30 requests/minute, 14,400 requests/day
- **Model**: Llama 3.3 70B Versatile
- **Speed**: Very fast (~2-3 seconds)
- **Get Key**: https://console.groq.com/keys

#### Gemini API
- **Free Tier**: 15 requests/minute, 1,500 requests/day
- **Model**: Gemini 2.0 Flash
- **Speed**: Fast (~3-4 seconds)
- **Get Key**: https://aistudio.google.com/apikey

#### ElevenLabs API (Optional)
- **Free Tier**: 10,000 characters/month
- **Voice**: Rachel (Natural Female)
- **Get Key**: https://elevenlabs.io/

### Settings Location

API keys are stored securely in Android SharedPreferences:
- File: `/data/data/com.vira.assistant/shared_prefs/vira_settings.xml`
- Keys are encrypted and stored locally on device
- Never transmitted except to respective API endpoints

---

## 🏗️ Architecture

### Technology Stack

- **Framework**: Uno Platform 5.0
- **Language**: C# (.NET 8)
- **UI**: Material Design 3
- **Target**: Android (net8.0-android)
- **Min SDK**: API 24 (Android 7.0)
- **Target SDK**: API 34 (Android 14)

### Project Structure

```
VIRA-UNO/
├── VIRA.Mobile/              # Android-specific code
│   ├── Activities/           # Android Activities
│   │   ├── MainActivity.cs   # Main chat interface
│   │   ├── SettingsActivity.cs
│   │   └── VoiceActiveActivity.cs
│   ├── Services/             # Platform services
│   │   └── AndroidVoiceService.cs
│   └── Utils/                # Utilities
│       ├── KeywordDetector.cs
│       ├── StatsTracker.cs
│       └── TimeGreeting.cs
├── VIRA.Shared/              # Shared code
│   ├── Models/               # Data models
│   ├── Services/             # Business logic
│   │   ├── GroqChatbotService.cs
│   │   ├── GeminiChatbotService.cs
│   │   └── ElevenLabsTTSService.cs
│   ├── ViewModels/           # MVVM ViewModels
│   └── Views/                # Shared UI components
└── build.sh                  # Build script
```

### Key Components

#### MainActivity.cs
- Main chat interface
- Message handling
- Voice toggle
- Quick actions

#### GroqChatbotService.cs
- Groq API integration
- Llama 3.3 70B model
- Message parsing

#### GeminiChatbotService.cs
- Google Gemini API integration
- Gemini 2.0 Flash model
- Response handling

#### ElevenLabsTTSService.cs
- ElevenLabs TTS integration
- Voice synthesis
- Audio streaming

#### AndroidVoiceService.cs
- Audio playback
- MediaPlayer management
- Speech recognition

---

## 🎨 UI/UX Features

### Design Principles

- **Material Design 3**: Modern, clean interface
- **Dark Theme**: Easy on the eyes
- **Smooth Animations**: Polished user experience
- **Responsive Layout**: Adapts to different screen sizes

### Color Palette

```
Primary:     #8B5CF6 (Purple)
Secondary:   #3B82F6 (Blue)
Background:  #0A1628 (Dark Blue)
Surface:     #1E293B (Dark Gray)
Success:     #22C55E (Green)
Error:       #EF4444 (Red)
```

### Typography

- **Headers**: Bold, 24-32sp
- **Body**: Regular, 14-16sp
- **Captions**: Light, 12sp

---

## 🔒 Privacy & Security

### Data Storage

- **Local Only**: All data stored on device
- **No Cloud Sync**: Messages not uploaded to servers
- **Encrypted Keys**: API keys stored securely
- **No Tracking**: No analytics or telemetry

### API Communication

- **HTTPS Only**: All API calls encrypted
- **No PII**: Personal information not sent to APIs
- **User Control**: Clear chat history anytime

### Permissions

```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.RECORD_AUDIO" />
```

- **Internet**: Required for API calls
- **Record Audio**: Optional, for voice input

---

## 🐛 Troubleshooting

### Common Issues

#### 1. "API Key not set" Error
**Solution**: Go to Settings → Enter your Groq/Gemini API key → Save

#### 2. Voice Not Working
**Solutions**:
- Check voice toggle is ON (green 🔊)
- Verify ElevenLabs API key in Settings
- Check device volume
- Ensure internet connection

#### 3. Slow Response
**Solutions**:
- Switch to Groq API (faster than Gemini)
- Check internet connection
- Verify API quota not exceeded

#### 4. Build Errors
**Solutions**:
```bash
# Clean build
dotnet clean
rm -rf VIRA.Mobile/bin VIRA.Mobile/obj
rm -rf VIRA.Shared/bin VIRA.Shared/obj

# Restore and rebuild
dotnet restore
bash build.sh
```

### Debug Logs

View logs via ADB:
```bash
# Real-time logs
adb logcat -s VIRA_MainActivity:I VIRA_Groq:I VIRA_ElevenLabs:I

# Save logs to file
adb logcat -d > vira_logs.txt
```

---

## 🤝 Contributing

Contributions are welcome! Please follow these guidelines:

1. **Fork the repository**
2. **Create a feature branch**
   ```bash
   git checkout -b feature/amazing-feature
   ```
3. **Commit your changes**
   ```bash
   git commit -m "Add amazing feature"
   ```
4. **Push to the branch**
   ```bash
   git push origin feature/amazing-feature
   ```
5. **Open a Pull Request**

### Code Style

- Follow C# coding conventions
- Use meaningful variable names
- Add comments for complex logic
- Write clean, maintainable code

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

### Technologies Used

- [Uno Platform](https://platform.uno/) - Cross-platform UI framework
- [.NET](https://dotnet.microsoft.com/) - Application framework
- [Groq](https://groq.com/) - Fast AI inference
- [Google Gemini](https://ai.google.dev/) - AI model
- [ElevenLabs](https://elevenlabs.io/) - Text-to-speech

### Inspiration

VIRA was inspired by modern AI assistants like ChatGPT, Google Assistant, and Siri, with a focus on privacy, customization, and open-source development.

---

## 📞 Support

### Get Help

- **Issues**: [GitHub Issues](https://github.com/yourusername/VIRA/issues)
- **Discussions**: [GitHub Discussions](https://github.com/yourusername/VIRA/discussions)
- **Email**: support@vira-assistant.com

### Useful Links

- [Groq API Documentation](https://console.groq.com/docs)
- [Gemini API Documentation](https://ai.google.dev/docs)
- [ElevenLabs API Documentation](https://elevenlabs.io/docs)
- [Uno Platform Documentation](https://platform.uno/docs)

---

## 🗺️ Roadmap

### Version 2.0 (Planned)

- [ ] Multi-language support (Indonesian, Spanish, French)
- [ ] Custom voice training
- [ ] Offline mode with local LLM
- [ ] Widget support
- [ ] Wear OS companion app
- [ ] Cloud sync (optional)
- [ ] Plugin system
- [ ] Custom themes

### Version 1.5 (In Progress)

- [x] Groq API integration
- [x] ElevenLabs TTS
- [x] Voice toggle
- [x] Background voice synthesis
- [ ] Image generation
- [ ] File attachments
- [ ] Export conversations

---

## 📊 Stats

- **Lines of Code**: ~5,000
- **Build Time**: ~60 seconds
- **APK Size**: ~32 MB
- **Min Android Version**: 7.0 (API 24)
- **Target Android Version**: 14 (API 34)

---

## 🌟 Star History

If you find VIRA useful, please consider giving it a star ⭐

[![Star History Chart](https://api.star-history.com/svg?repos=yourusername/VIRA&type=Date)](https://star-history.com/#yourusername/VIRA&Date)

---

<div align="center">

**Made with ❤️ by the VIRA Team**

[Website](https://vira-assistant.com) • [Documentation](https://docs.vira-assistant.com) • [Blog](https://blog.vira-assistant.com)

</div>
