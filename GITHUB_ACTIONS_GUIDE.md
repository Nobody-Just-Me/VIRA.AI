# 🚀 Build APK dengan GitHub Actions (TERMUDAH!)

## ✨ Keuntungan Metode Ini

- ✅ **TIDAK PERLU** install Android SDK di komputer Anda
- ✅ **TIDAK PERLU** install Visual Studio
- ✅ **TIDAK PERLU** setup apapun di lokal
- ✅ Build otomatis di cloud (GitHub servers)
- ✅ Download APK langsung dari browser
- ✅ **GRATIS** untuk public repositories

## 📋 Langkah-Langkah (5 Menit!)

### 1. Push Code ke GitHub

```bash
cd VIRA-UNO

# Initialize git (jika belum)
git init

# Add all files
git add .

# Commit
git commit -m "Initial commit - VIRA AI Assistant"

# Create repository di GitHub (via browser):
# https://github.com/new
# Nama: vira-android-app

# Add remote dan push
git remote add origin https://github.com/YOUR_USERNAME/vira-android-app.git
git branch -M main
git push -u origin main
```

### 2. Trigger Build

Setelah push, GitHub Actions akan otomatis:
1. Detect workflow file (`.github/workflows/build-apk.yml`)
2. Start build process
3. Compile APK
4. Upload sebagai artifact

**Atau trigger manual:**
1. Buka repository di GitHub
2. Klik tab "Actions"
3. Pilih "Build VIRA APK"
4. Klik "Run workflow"
5. Klik "Run workflow" lagi

### 3. Download APK

Setelah build selesai (~10-15 menit):

1. Buka tab "Actions" di repository
2. Klik workflow run yang baru saja selesai
3. Scroll ke bawah ke section "Artifacts"
4. Download "VIRA-Release-APK"
5. Extract ZIP file
6. Transfer APK ke Android device

## 📱 Install APK ke Device

### Via USB (ADB):
```bash
adb install -r VIRA-Release.apk
```

### Via File Transfer:
1. Copy APK ke HP via USB/Bluetooth/Cloud
2. Buka File Manager di HP
3. Tap file APK
4. Allow "Install from Unknown Sources" jika diminta
5. Tap "Install"

## 🔄 Update Aplikasi

Setiap kali Anda push perubahan ke GitHub:
```bash
git add .
git commit -m "Update: fitur baru"
git push
```

GitHub Actions akan otomatis build APK baru!

## 🏷️ Create Release (Optional)

Untuk membuat release resmi dengan APK yang bisa di-download publik:

```bash
# Tag version
git tag v1.0.0
git push origin v1.0.0
```

GitHub Actions akan otomatis create release dengan APK attached!

## 📊 Monitor Build Progress

1. Buka repository di GitHub
2. Klik tab "Actions"
3. Lihat real-time progress:
   - ✅ Checkout code
   - ✅ Setup .NET
   - ✅ Setup Java
   - ✅ Install Uno Templates
   - ✅ Restore dependencies
   - ✅ Build APK
   - ✅ Upload artifact

## ⏱️ Estimasi Waktu

| Tahap | Waktu |
|-------|-------|
| Setup environment | 2-3 min |
| Restore dependencies | 2-3 min |
| Build APK | 5-8 min |
| Upload artifact | 1 min |
| **Total** | **10-15 min** |

## 🔒 Keamanan API Key

**PENTING:** Jangan commit API Key ke GitHub!

API Key akan diinput oleh user saat pertama kali buka aplikasi:
1. Install APK
2. Buka VIRA
3. Tap Settings
4. Input Gemini API Key
5. Simpan

## 💡 Tips

### Build Lebih Cepat
- Gunakan cache untuk dependencies
- Build hanya saat push ke branch tertentu

### Notifikasi
GitHub akan kirim email saat build selesai atau gagal

### Private Repository
Jika repository private, GitHub Actions tetap gratis untuk:
- 2,000 menit/bulan (Free plan)
- Unlimited untuk public repos

## 🐛 Troubleshooting

### Build Failed?
1. Cek tab "Actions" untuk error log
2. Klik workflow run yang failed
3. Expand step yang error
4. Copy error message

### APK Tidak Muncul?
- Pastikan build success (✅ hijau)
- Scroll ke bawah sampai section "Artifacts"
- Refresh page jika perlu

### Tidak Bisa Push ke GitHub?
```bash
# Setup Git credentials
git config --global user.name "Your Name"
git config --global user.email "your@email.com"

# Use Personal Access Token
# Generate di: https://github.com/settings/tokens
```

## 🎉 Selesai!

Dengan metode ini, Anda:
- ✅ Tidak perlu setup Android SDK lokal
- ✅ Tidak perlu install tools berat
- ✅ Bisa build dari mana saja (hanya perlu browser)
- ✅ APK selalu tersedia untuk download

**Ini adalah cara TERMUDAH untuk build APK VIRA!** 🚀
