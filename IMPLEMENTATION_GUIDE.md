# VIRA Implementation Guide

## Struktur Proyek yang Telah Dibuat

```
VIRA-UNO/
├── VIRA.Shared/                    # Shared code untuk semua platform
│   ├── Models/
│   │   ├── ChatMessage.cs          # Model pesan chat dengan tipe terstruktur
│   │   ├── DailyCommandIntent.cs   # Model untuk function calling
│   │   └── UserContext.cs          # Konteks pengguna dan perangkat
│   ├── Services/
│   │   ├── IGeminiService.cs       # Interface untuk Gemini API
│   │   ├── GeminiChatbotService.cs # Implementasi Gemini API client
│   │   └── IVoiceService.cs        # Interface untuk voice I/O
│   ├── ViewModels/
│   │   └── MainChatViewModel.cs    # ViewModel untuk chat (MVVM)
│   ├── Views/
│   │   ├── MainPage.xaml           # UI halaman chat utama
│   │   ├── MainPage.xaml.cs        # Code-behind MainPage
│   │   ├── SettingsPage.xaml       # UI halaman pengaturan
│   │   └── SettingsPage.xaml.cs    # Code-behind Settings
│   ├── Styles/
│   │   └── AppStyles.xaml          # Style dan tema aplikasi
│   ├── App.xaml                    # Application resources
│   ├── App.xaml.cs                 # App startup & DI configuration
│   └── VIRA.Shared.csproj
│
├── VIRA.Mobile/                    # Android-specific project
│   ├── Services/
│   │   └── AndroidVoiceService.cs  # Implementasi STT & TTS Android
│   ├── MainActivity.cs             # Entry point Android
│   ├── AndroidManifest.xml         # Permissions & app config
│   └── VIRA.Mobile.csproj
│
├── VIRA.sln                        # Solution file
└── README.md

```

## Langkah-Langkah Build & Deploy

### 1. Prerequisites

Pastikan Anda sudah menginstall:
- Visual Studio 2022 (17.8 atau lebih baru)
- .NET 8 SDK
- Uno Platform Solution Templates
- Android SDK (API Level 21-34)
- Java JDK 11 atau lebih baru

Install Uno Platform templates:
```bash
dotnet new install Uno.Templates
```

### 2. Setup Project

```bash
cd VIRA-UNO
dotnet restore
```

### 3. Konfigurasi Android SDK

Di Visual Studio:
1. Tools → Options → Xamarin → Android Settings
2. Pastikan Android SDK path sudah benar
3. Install Android SDK Platform 34 (atau sesuai target)

### 4. Build Project

```bash
# Build Shared project
dotnet build VIRA.Shared/VIRA.Shared.csproj

# Build Android project
dotnet build VIRA.Mobile/VIRA.Mobile.csproj -f net8.0-android
```

### 5. Deploy ke Device

#### Via Visual Studio:
1. Buka `VIRA.sln` di Visual Studio 2022
2. Set `VIRA.Mobile` sebagai Startup Project
3. Pilih device/emulator Android di toolbar
4. Tekan F5 untuk build & deploy

#### Via Command Line:
```bash
# Deploy ke device yang terhubung
dotnet build VIRA.Mobile/VIRA.Mobile.csproj -f net8.0-android -t:Run
```

### 6. Generate APK untuk Distribution

```bash
dotnet publish VIRA.Mobile/VIRA.Mobile.csproj -f net8.0-android -c Release
```

APK akan tersimpan di:
`VIRA.Mobile/bin/Release/net8.0-android/publish/`

## Konfigurasi Awal Aplikasi

### 1. Dapatkan Gemini API Key

1. Kunjungi: https://aistudio.google.com/app/apikey
2. Login dengan Google Account
3. Klik "Create API Key"
4. Copy API Key yang dihasilkan

### 2. Setup di Aplikasi

1. Buka aplikasi VIRA di Android
2. Tap icon Settings (⚙️) di kanan atas
3. Paste API Key di field "Gemini API Key"
4. Tap "Simpan API Key"
5. Tap "Test Koneksi" untuk verifikasi

## Fitur yang Sudah Diimplementasi

### ✅ Core Features
- [x] Chat interface dengan Gemini AI
- [x] Dark mode dengan glassmorphism design
- [x] Quick Actions untuk perintah cepat
- [x] Message history dengan scroll
- [x] Typing indicator saat AI memproses
- [x] Settings page untuk API Key management

### ✅ Voice Features
- [x] Speech-to-Text (Android SpeechRecognizer)
- [x] Text-to-Speech dengan suara perempuan Indonesia
- [x] Voice input button di chat interface
- [x] Auto-speak AI responses

### ✅ AI Capabilities
- [x] Natural language understanding
- [x] Context-aware responses
- [x] System prompt untuk identitas Vira
- [x] Conversation history (last 10 messages)
- [x] Message type detection (weather, news, schedule, etc.)

## Customization & Enhancement

### Mengubah Warna Tema

Edit `VIRA.Shared/Styles/AppStyles.xaml`:
```xml
<Color x:Key="PrimaryBlue">#256AF4</Color>  <!-- Ubah warna utama -->
<Color x:Key="SlateBackground">#101622</Color>  <!-- Ubah background -->
```

### Menambah Quick Action Baru

Edit `VIRA.Shared/Views/MainPage.xaml`, tambahkan button di section Quick Actions:
```xml
<Button Content="🎵 Musik" 
        Style="{StaticResource QuickActionButtonStyle}"
        Command="{x:Bind ViewModel.ExecuteQuickActionCommand}"
        CommandParameter="Putar musik santai"/>
```

### Mengintegrasikan API External

Untuk menambahkan function calling (cuaca, berita, dll):

1. Buat service baru di `VIRA.Shared/Services/`
2. Register di `App.xaml.cs` → `ConfigureServices()`
3. Inject ke `GeminiChatbotService` atau `MainChatViewModel`
4. Update parsing logic di `ParseResponse()`

### Menyesuaikan System Prompt

Edit `GeminiChatbotService.cs` → method `GetSystemPrompt()`:
```csharp
private string GetSystemPrompt()
{
    return @"Nama Anda adalah Vira...
    [Tambahkan instruksi custom di sini]";
}
```

## Troubleshooting

### Build Errors

**Error: "Android SDK not found"**
- Solution: Install Android SDK via Visual Studio Installer

**Error: "Java JDK not found"**
- Solution: Install JDK 11+ dan set JAVA_HOME environment variable

### Runtime Issues

**Voice tidak berfungsi:**
- Pastikan permission RECORD_AUDIO sudah granted
- Cek di Settings → Apps → VIRA → Permissions

**API Error 400:**
- Verifikasi API Key sudah benar
- Pastikan API Key memiliki akses ke Gemini API
- Cek quota di Google AI Studio

**TTS tidak berbunyi:**
- Pastikan volume device tidak mute
- Cek bahasa Indonesia sudah terinstall di Android TTS settings

## Testing di Huawei Mate 20 Pro

### Spesifikasi Device
- OS: Android 10 (EMUI 10)
- RAM: 6GB
- Processor: Kirin 980

### Known Issues
- Huawei devices mungkin tidak memiliki Google Play Services
- Solusi: Pastikan Gemini API endpoint tidak bergantung pada GMS

### Performance Tips
- Gunakan `gemini-1.5-flash` untuk response cepat
- Limit conversation history ke 10 pesan terakhir
- Enable hardware acceleration di AndroidManifest

## Next Steps

### Fase 4: Function Calling (Belum Diimplementasi)

Untuk mengimplementasikan function calling:

1. **Weather API Integration**
   - Daftar di OpenWeatherMap atau WeatherAPI
   - Buat `WeatherService.cs`
   - Update `GeminiChatbotService` untuk call API saat diperlukan

2. **News API Integration**
   - Gunakan NewsAPI.org
   - Parse dan format berita untuk UI

3. **Reminder System**
   - Integrate dengan Android AlarmManager
   - Simpan reminders di local storage

4. **Traffic Data**
   - Integrate Google Maps API atau Waze API
   - Real-time traffic updates

## Resources

- [Uno Platform Docs](https://platform.uno/docs/)
- [Gemini API Docs](https://ai.google.dev/docs)
- [Android Speech API](https://developer.android.com/reference/android/speech/package-summary)
- [MVVM Toolkit](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)

## Support

Untuk pertanyaan atau issue, buat issue di repository atau hubungi developer.
