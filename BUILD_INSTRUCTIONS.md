# Panduan Build APK VIRA

## Prerequisites Installation

### 1. Install .NET 8 SDK
```bash
# Download dari:
https://dotnet.microsoft.com/download/dotnet/8.0

# Verify installation
dotnet --version
```

### 2. Install Visual Studio 2022
Download dari: https://visualstudio.microsoft.com/downloads/

**Workloads yang harus diinstall:**
- ✅ .NET Multi-platform App UI development
- ✅ Mobile development with .NET
- ✅ .NET desktop development

### 3. Install Uno Platform Templates
```bash
dotnet new install Uno.Templates
```

### 4. Install Android SDK
Via Visual Studio Installer:
- Tools → Android → Android SDK (API 21-34)
- Tools → Android → Android SDK Build-Tools
- Tools → Android → Android Emulator

## Build Steps

### Step 1: Open Project
```bash
cd VIRA-UNO
# Buka VIRA.sln di Visual Studio 2022
```

### Step 2: Restore Dependencies
```bash
dotnet restore
```

### Step 3: Build Debug APK (untuk testing)
```bash
dotnet build VIRA.Mobile/VIRA.Mobile.csproj -f net8.0-android -c Debug
```

APK akan ada di:
`VIRA.Mobile/bin/Debug/net8.0-android/com.vira.assistant-Signed.apk`

### Step 4: Build Release APK (untuk distribusi)
```bash
dotnet publish VIRA.Mobile/VIRA.Mobile.csproj -f net8.0-android -c Release
```

APK akan ada di:
`VIRA.Mobile/bin/Release/net8.0-android/publish/com.vira.assistant-Signed.apk`

## Setup API Key (AMAN)

### Cara 1: Input di Aplikasi (Recommended)
1. Install APK ke device
2. Buka aplikasi
3. Tap Settings
4. Masukkan API Key
5. Tap Simpan

### Cara 2: Environment Variable (untuk development)
```bash
# Windows
set GEMINI_API_KEY=your_api_key_here

# Linux/Mac
export GEMINI_API_KEY=your_api_key_here
```

Lalu update `GeminiChatbotService.cs`:
```csharp
private string _apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? string.Empty;
```

### Cara 3: Secure Configuration File (Advanced)
Buat file `appsettings.json` (tidak di-commit ke git):
```json
{
  "GeminiApiKey": "your_api_key_here"
}
```

Add ke `.gitignore`:
```
appsettings.json
*.user
```

## Troubleshooting

### Error: "Android SDK not found"
**Solution:**
1. Open Visual Studio
2. Tools → Options → Xamarin → Android Settings
3. Set Android SDK Location
4. Install missing SDK components

### Error: "Java JDK not found"
**Solution:**
1. Install JDK 11 atau 17
2. Set JAVA_HOME environment variable
3. Restart Visual Studio

### Error: "Build failed with code 1"
**Solution:**
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

### Error: "Unable to find package Uno.WinUI"
**Solution:**
```bash
# Clear NuGet cache
dotnet nuget locals all --clear
dotnet restore
```

## Deploy ke Device

### Via USB (ADB)
```bash
# Enable USB Debugging di device
# Connect via USB

# Install APK
adb install VIRA.Mobile/bin/Debug/net8.0-android/com.vira.assistant-Signed.apk

# Check logs
adb logcat | grep VIRA
```

### Via Visual Studio
1. Connect device via USB
2. Enable USB Debugging
3. Select device di toolbar
4. Press F5 (Run)

## Signing APK untuk Release

### Generate Keystore
```bash
keytool -genkey -v -keystore vira-release.keystore -alias vira -keyalg RSA -keysize 2048 -validity 10000
```

### Sign APK
```bash
jarsigner -verbose -sigalg SHA256withRSA -digestalg SHA-256 -keystore vira-release.keystore VIRA.Mobile/bin/Release/net8.0-android/com.vira.assistant.apk vira
```

### Verify Signature
```bash
jarsigner -verify -verbose -certs VIRA.Mobile/bin/Release/net8.0-android/com.vira.assistant.apk
```

## Optimasi APK Size

### Enable ProGuard (Code Shrinking)
Edit `VIRA.Mobile.csproj`:
```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <AndroidEnableProguard>true</AndroidEnableProguard>
  <AndroidLinkMode>Full</AndroidLinkMode>
  <AndroidUseSharedRuntime>false</AndroidUseSharedRuntime>
</PropertyGroup>
```

### Enable R8 (Advanced Optimization)
```xml
<PropertyGroup>
  <AndroidEnableR8>true</AndroidEnableR8>
</PropertyGroup>
```

## Testing Checklist

- [ ] Build berhasil tanpa error
- [ ] APK terinstall di device
- [ ] Aplikasi bisa dibuka
- [ ] Input API Key berfungsi
- [ ] Chat dengan AI berfungsi
- [ ] Voice input berfungsi
- [ ] Voice output berfungsi
- [ ] Quick Actions berfungsi
- [ ] Settings tersimpan
- [ ] Tidak ada crash

## Estimasi Waktu

- Setup environment: 30-60 menit (first time)
- Build Debug APK: 2-5 menit
- Build Release APK: 5-10 menit
- Testing: 10-15 menit

**Total: ~1-2 jam untuk first time build**

## Alternative: GitHub Actions (CI/CD)

Jika Anda ingin automated build, saya bisa buatkan GitHub Actions workflow yang akan build APK otomatis setiap kali push code.

## Need Help?

Jika mengalami kesulitan, saya bisa:
1. ✅ Membuat script automation untuk build
2. ✅ Membuat Docker container dengan semua tools
3. ✅ Membuat GitHub Actions workflow
4. ✅ Troubleshoot error messages
5. ✅ Optimize build configuration

Silakan tanyakan jika ada yang tidak jelas!
