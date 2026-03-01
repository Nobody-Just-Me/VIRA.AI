# VIRA - Virtual Intelligent Responsive Assistant

<div align="center">

![VIRA Logo](https://via.placeholder.com/150x150/6366F1/FFFFFF?text=V)

**Asisten Pribadi AI Cerdas untuk Android**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Uno Platform](https://img.shields.io/badge/Uno%20Platform-5.1-7C4DFF?logo=uno)](https://platform.uno/)
[![Gemini API](https://img.shields.io/badge/Gemini-1.5%20Flash-4285F4?logo=google)](https://ai.google.dev/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

[Quick Start](#-quick-start) • [Features](#-fitur-utama) • [Documentation](#-dokumentasi) • [Screenshots](#-screenshots)

</div>

---

## 📖 Tentang VIRA

VIRA (Virtual Intelligent Responsive Assistant) adalah aplikasi asisten pribadi AI yang dirancang khusus untuk Android, menggabungkan kekuatan Google Gemini AI dengan antarmuka modern yang elegan. Dibangun menggunakan Uno Platform dan C#, VIRA menawarkan pengalaman interaksi yang natural melalui teks dan suara.

### 🎯 Mengapa VIRA?

- **🧠 AI Cerdas**: Powered by Gemini 1.5 Flash untuk respons cepat dan akurat
- **🎤 Voice-First**: Interaksi hands-free dengan Speech-to-Text dan Text-to-Speech
- **🎨 Modern UI**: Dark mode dengan glassmorphism design yang memukau
- **🔒 Privacy**: API Key disimpan lokal, data tidak dibagikan
- **⚡ Performa**: Optimized untuk Android, ringan dan responsif
- **🇮🇩 Bahasa Indonesia**: Dukungan penuh untuk bahasa Indonesia

## ✨ Fitur Utama

### Core Features
- ✅ **Chat AI Cerdas** - Percakapan natural dengan Gemini AI
- ✅ **Voice Input** - Speech-to-Text untuk input hands-free
- ✅ **Voice Output** - Text-to-Speech dengan suara perempuan Indonesia
- ✅ **Quick Actions** - Shortcut untuk perintah yang sering digunakan
- ✅ **Conversation History** - Menyimpan konteks percakapan
- ✅ **Typing Indicator** - Animasi saat AI sedang berpikir

### UI/UX Features
- 🎨 **Dark Mode** - Tema gelap yang nyaman di mata
- 💎 **Glassmorphism** - Efek kaca buram yang modern
- 🌊 **Smooth Animations** - Transisi halus dan responsif
- 📱 **Mobile-First** - Dioptimalkan untuk layar sentuh
- 🎭 **Ambient Effects** - Background gradient yang dinamis

### AI Capabilities
- 🤖 **Natural Language** - Memahami bahasa natural Indonesia
- 🧠 **Context Aware** - Mengingat konteks percakapan
- 🎯 **Intent Detection** - Mendeteksi jenis permintaan (cuaca, berita, dll)
- 📊 **Structured Responses** - Respons terstruktur untuk data kompleks

## 🚀 Quick Start

### Prerequisites

```bash
# Install .NET 8 SDK
https://dotnet.microsoft.com/download/dotnet/8.0

# Install Uno Platform templates
dotnet new install Uno.Templates

# Verify installation
dotnet --version  # Should show 8.0.x
```

**Requirements:**
- Visual Studio 2022 (v17.8 atau lebih baru)
- Android SDK (API Level 21-34)
- Java JDK 11+

### Installation

```bash
# Clone repository
git clone https://github.com/yourusername/vira-uno.git
cd VIRA-UNO

# Restore dependencies
dotnet restore

# Build project
dotnet build
```

### Setup Gemini API Key

1. Dapatkan API Key gratis: [Google AI Studio](https://aistudio.google.com/app/apikey)
2. Buka aplikasi VIRA di Android
3. Tap icon Settings (⚙️)
4. Masukkan API Key → Simpan
5. Test koneksi

### Deploy ke Android

**Via Visual Studio:**
1. Buka `VIRA.sln`
2. Set `VIRA.Mobile` sebagai Startup Project
3. Pilih Android device/emulator
4. Tekan `F5`

**Via Command Line:**
```bash
dotnet build VIRA.Mobile/VIRA.Mobile.csproj -f net8.0-android -t:Run
```

## 🏗️ Arsitektur

### Teknologi Stack

| Layer | Technology |
|-------|-----------|
| **Framework** | Uno Platform 5.1 (WinUI 3 / XAML) |
| **Language** | C# (.NET 8) |
| **AI Engine** | Google Gemini API (gemini-1.5-flash) |
| **Voice STT** | Android SpeechRecognizer |
| **Voice TTS** | Android TextToSpeech |
| **Architecture** | MVVM (CommunityToolkit.Mvvm) |
| **DI Container** | Microsoft.Extensions.DependencyInjection |

### Struktur Proyek

```
VIRA-UNO/
├── VIRA.Shared/                    # Shared code untuk semua platform
│   ├── Models/                     # Data models
│   │   ├── ChatMessage.cs
│   │   ├── DailyCommandIntent.cs
│   │   └── UserContext.cs
│   ├── Services/                   # Business logic
│   │   ├── IGeminiService.cs
│   │   ├── GeminiChatbotService.cs
│   │   └── IVoiceService.cs
│   ├── ViewModels/                 # MVVM ViewModels
│   │   └── MainChatViewModel.cs
│   ├── Views/                      # XAML Pages
│   │   ├── MainPage.xaml
│   │   └── SettingsPage.xaml
│   ├── Styles/                     # UI Styles
│   │   └── AppStyles.xaml
│   └── Converters/                 # Value converters
│       └── BoolToVisibilityConverter.cs
│
├── VIRA.Mobile/                    # Android-specific
│   ├── Services/
│   │   └── AndroidVoiceService.cs
│   ├── MainActivity.cs
│   └── AndroidManifest.xml
│
├── VIRA.sln                        # Solution file
├── README.md
├── QUICK_START.md
├── IMPLEMENTATION_GUIDE.md
├── UI_DESIGN_MAPPING.md
└── IMPLEMENTATION_CHECKLIST.md
```

## 📚 Dokumentasi

| Dokumen | Deskripsi |
|---------|-----------|
| [QUICK_START.md](QUICK_START.md) | Panduan memulai dalam 5 menit |
| [IMPLEMENTATION_GUIDE.md](IMPLEMENTATION_GUIDE.md) | Panduan implementasi lengkap |
| [UI_DESIGN_MAPPING.md](UI_DESIGN_MAPPING.md) | Mapping desain React → XAML |
| [IMPLEMENTATION_CHECKLIST.md](IMPLEMENTATION_CHECKLIST.md) | Progress dan checklist |

## 🎨 Screenshots

<div align="center">

| Main Chat | Voice Active | Settings |
|-----------|--------------|----------|
| ![Chat](https://via.placeholder.com/250x500/101622/FFFFFF?text=Chat+Screen) | ![Voice](https://via.placeholder.com/250x500/101622/FFFFFF?text=Voice+Screen) | ![Settings](https://via.placeholder.com/250x500/101622/FFFFFF?text=Settings) |

</div>

## 🎯 Roadmap

### ✅ v1.0 (Current)
- [x] Core chat functionality
- [x] Voice input/output
- [x] Dark mode UI
- [x] Settings management

### 🔄 v1.1 (In Progress)
- [ ] Function calling (Weather, News, Traffic)
- [ ] Reminder system
- [ ] Export chat history
- [ ] Multi-language support

### 🔮 v2.0 (Planned)
- [ ] Image input (Vision API)
- [ ] Voice commands (wake word)
- [ ] Widget support
- [ ] Cloud sync

## 🤝 Contributing

Contributions are welcome! Please read our [Contributing Guidelines](CONTRIBUTING.md) first.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **UI Design**: Inspired by [AI Assistant Mobile UI](https://www.figma.com/design/KP3EbhbFDqNuCMIjnIf9bM/AI-Assistant-Mobile-UI)
- **AI Engine**: Powered by [Google Gemini API](https://ai.google.dev/)
- **Framework**: Built with [Uno Platform](https://platform.uno/)
- **Icons**: [Segoe MDL2 Assets](https://docs.microsoft.com/en-us/windows/apps/design/style/segoe-ui-symbol-font)

## 📞 Support

- 📧 Email: support@vira-app.com
- 💬 Discord: [Join our community](https://discord.gg/vira)
- 🐛 Issues: [GitHub Issues](https://github.com/yourusername/vira-uno/issues)
- 📖 Docs: [Documentation](https://docs.vira-app.com)

## ⭐ Star History

[![Star History Chart](https://api.star-history.com/svg?repos=yourusername/vira-uno&type=Date)](https://star-history.com/#yourusername/vira-uno&Date)

---

<div align="center">

**Made with ❤️ for Android developers**

[Website](https://vira-app.com) • [Documentation](https://docs.vira-app.com) • [Blog](https://blog.vira-app.com)

</div>
