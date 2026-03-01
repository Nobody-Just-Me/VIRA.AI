# 📱 Cara Build APK VIRA - Panduan Lengkap

## 🎯 Pilihan Build

Anda memiliki 3 pilihan untuk mendapatkan APK:

### ✅ Opsi 1: Build Otomatis dengan Script (TERMUDAH)
### ⚙️ Opsi 2: Build Manual via Visual Studio
### 🐳 Opsi 3: Build via Docker (Advanced)

---

## ✅ OPSI 1: Build Otomatis dengan Script (RECOMMENDED)

### Langkah 1: Install Prerequisites

#### Windows:
1. **Install .NET 8 SDK**
   - Download: https://dotnet.microsoft.com/download/dotnet/8.0
   - Jalankan installer
   - Restart terminal

2. **Install Visual Studio 2022 Community** (Gratis)
   - Download: https://visualstudio.microsoft.com/downloads/
   - Saat install, pilih workload:
     - ✅ Mobile development with .NET
     - ✅ .NET Multi-platform App UI development
   - Ini akan otomatis install Android SDK

3. **Verify Installation**
   ```cmd
   dotnet --version
   ```
   Harus muncul versi 8.0.x

#### Linux/Mac:
```bash
# Install .NET 8 SDK
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0

# Install Android SDK
# Via Android Studio atau manual download
```

### Langkah 2: Build APK

#### Windows:
```cmd
cd VIRA-UNO
build.bat
```

#### Linux/Mac:
```bash
cd VIRA-UNO
./build.sh
```

### Langkah 3: Install ke Device

APK akan tersimpan di: `VIRA-UNO/VIRA-Release.apk`

**Via USB:**
```bash
adb install -r VIRA-Release.apk
```

**Via Transfer File:**
1. Copy `VIRA-Release.apk` ke HP
2. Buka File Manager
3. Tap APK file
4. Allow "Install from Unknown Sources"
5. Install

---

## ⚙️ OPSI 2: Build Manual via Visual Studio

### Langkah 1: Open Project
1. Buka Visual Studio 2022
2. File → Open → Project/Solution
3. Pilih `VIRA-UNO/VIRA.sln`

### Langkah 2: Configure
1. Klik kanan `VIRA.Mobile` → Set as Startup Project
2. Toolbar: Pilih "Release" (bukan Debug)
3. Toolbar: Pilih "Any CPU"

### Langkah 3: Build
1. Build → Rebuild Solution
2. Tunggu hingga selesai (5-10 menit)

### Langkah 4: Publish
1. Klik kanan `VIRA.Mobile` → Publish
2. Pilih "Android" → "Android App Bundle"
3. Klik "Publish"

APK akan ada di:
`VIRA.Mobile\bin\Release\net8.0-android\publish\`

---

## 🐳 OPSI 3: Build via Docker (Advanced)

Jika Anda familiar dengan Docker:

```bash
# Build Docker image dengan semua tools
docker build -t vira-builder .

# Run build
docker run -v $(pwd):/app vira-builder

# APK akan tersimpan di ./output/
```

---

## 🔐 Setup API Key (PENTING!)

### ⚠️ JANGAN Hardcode API Key di Source Code!

**Cara yang BENAR:**

### Metode 1: Input di Aplikasi (PALING AMAN)
1. Install APK
2. Buka aplikasi VIRA
3. Tap icon Settings (⚙️) di kanan atas
4. Paste API Key Anda
5. Tap "Simpan API Key"
6. Tap "Test Koneksi" untuk verifikasi

### Metode 2: Environment Variable (untuk Development)
```bash
# Windows
set GEMINI_API_KEY=AIzaSy...

# Linux/Mac
export GEMINI_API_KEY=AIzaSy...
```

Lalu update `GeminiChatbotService.cs`:
```csharp
private string _apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? string.Empty;
```

### Metode 3: Secure Config File (Advanced)
Buat `VIRA.Shared/appsettings.json`:
```json
{
  "GeminiApiKey": "AIzaSy..."
}
```

**PENTING:** Tambahkan ke `.gitignore`:
```
appsettings.json
*.user
secrets.json
```

---

## 🐛 Troubleshooting

### Error: "Android SDK not found"
**Solusi:**
1. Install Visual Studio dengan workload "Mobile development with .NET"
2. Atau download Android SDK manual: https://developer.android.com/studio
3. Set environment variable:
   ```
   ANDROID_HOME=C:\Users\YourName\AppData\Local\Android\Sdk
   ```

### Error: ".NET SDK not found"
**Solusi:**
1. Download .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0
2. Install dan restart terminal
3. Verify: `dotnet --version`

### Error: "Java JDK not found"
**Solusi:**
1. Download JDK 11 atau 17: https://adoptium.net/
2. Install
3. Set JAVA_HOME:
   ```
   JAVA_HOME=C:\Program Files\Java\jdk-17
   ```

### Error: "Build failed with code 1"
**Solusi:**
```bash
# Clean dan rebuild
dotnet clean
dotnet restore
dotnet build
```

### Error: "Unable to find package Uno.WinUI"
**Solusi:**
```bash
# Clear NuGet cache
dotnet nuget locals all --clear
dotnet restore
```

---

## 📊 Estimasi Waktu & Ukuran

| Tahap | Waktu | Catatan |
|-------|-------|---------|
| Install Prerequisites | 30-60 min | First time only |
| Download Dependencies | 5-10 min | First time only |
| Build APK | 5-10 min | Subsequent builds faster |
| **Total First Time** | **40-80 min** | |
| **Subsequent Builds** | **5-10 min** | |

**Ukuran APK:** ~15-25 MB (tergantung optimasi)

---

## ✅ Checklist Sebelum Build

- [ ] .NET 8 SDK terinstall
- [ ] Visual Studio 2022 dengan Android workload
- [ ] Android SDK terinstall (API 21-34)
- [ ] Java JDK 11+ terinstall
- [ ] Internet connection (untuk download dependencies)
- [ ] Minimal 5GB free disk space

---

## 🚀 Quick Start (TL;DR)

Jika semua prerequisites sudah terinstall:

```bash
# Clone/navigate to project
cd VIRA-UNO

# Windows
build.bat

# Linux/Mac
./build.sh

# Install
adb install -r VIRA-Release.apk
```

---

## 💡 Tips & Tricks

### Mempercepat Build
1. Gunakan SSD untuk project location
2. Disable antivirus saat build
3. Close aplikasi lain yang berat
4. Gunakan Release configuration (bukan Debug)

### Mengecilkan Ukuran APK
Edit `VIRA.Mobile.csproj`:
```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <AndroidLinkMode>Full</AndroidLinkMode>
  <AndroidEnableProguard>true</AndroidEnableProguard>
  <AndroidUseSharedRuntime>false</AndroidUseSharedRuntime>
</PropertyGroup>
```

### Testing di Emulator
1. Buka Android Studio
2. Tools → AVD Manager
3. Create Virtual Device
4. Start emulator
5. Run: `adb install VIRA-Release.apk`

---

## 📞 Butuh Bantuan?

Jika mengalami kesulitan:

1. **Cek log error** - Copy paste error message
2. **Cek prerequisites** - Pastikan semua terinstall
3. **Clean & rebuild** - `dotnet clean && dotnet build`
4. **Restart IDE** - Kadang Visual Studio perlu restart
5. **Tanya saya** - Saya siap membantu troubleshoot!

---

## 🎉 Setelah Build Berhasil

1. ✅ Install APK ke device
2. ✅ Buka aplikasi VIRA
3. ✅ Masuk ke Settings
4. ✅ Input Gemini API Key
5. ✅ Test koneksi
6. ✅ Mulai chat dengan Vira!

**Selamat! Anda berhasil build VIRA! 🎊**
