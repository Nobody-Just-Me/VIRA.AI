# 🔧 Solusi Build APK VIRA

## ⚠️ Masalah Saat Ini

Anda tidak memiliki Android SDK yang diperlukan untuk build APK. Ada 3 solusi:

---

## ✅ SOLUSI 1: Install Android SDK (Recommended untuk Development)

### Langkah 1: Install Android SDK
```bash
cd VIRA-UNO
./install-android-sdk.sh
```

Script ini akan:
- Download Android Command Line Tools (~500 MB)
- Install Android SDK Platform 34
- Install Build Tools
- Setup environment variables

**Waktu:** ~20-30 menit (tergantung internet)
**Ukuran Download:** ~1-2 GB

### Langkah 2: Apply Environment
```bash
source ~/.bashrc
```

### Langkah 3: Build APK
```bash
./build.sh
```

APK akan tersimpan di `VIRA-Release.apk`

---

## 🐳 SOLUSI 2: Build dengan Docker (Recommended untuk Quick Build)

Jika Anda sudah punya Docker installed:

```bash
cd VIRA-UNO
./docker-build.sh
```

**Keuntungan:**
- ✅ Tidak perlu install Android SDK di sistem Anda
- ✅ Semua dependencies ada di container
- ✅ Reproducible builds
- ✅ Tidak mengotori sistem Anda

**Kekurangan:**
- ❌ Perlu Docker installed
- ❌ First build lambat (~20-30 menit)
- ❌ Ukuran image besar (~3-4 GB)

### Install Docker (jika belum ada):
```bash
# Ubuntu/Debian
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
sudo usermod -aG docker $USER
# Logout dan login lagi
```

---

## 💻 SOLUSI 3: Build di Visual Studio (Windows)

Jika Anda punya akses ke Windows PC:

### Langkah 1: Install Visual Studio 2022
- Download: https://visualstudio.microsoft.com/downloads/
- Pilih workload: "Mobile development with .NET"
- Ini akan otomatis install Android SDK

### Langkah 2: Open Project
1. Copy folder `VIRA-UNO` ke Windows PC
2. Buka `VIRA.sln` di Visual Studio
3. Wait for restore (first time ~5-10 menit)

### Langkah 3: Build
1. Set `VIRA.Mobile` as Startup Project
2. Select "Release" configuration
3. Build → Publish
4. APK akan ada di `VIRA.Mobile\bin\Release\net8.0-android\`

---

## 🌐 SOLUSI 4: GitHub Actions (Automated CI/CD)

Saya bisa buatkan GitHub Actions workflow yang akan build APK otomatis di cloud setiap kali Anda push code.

**Keuntungan:**
- ✅ Build di cloud (GitHub servers)
- ✅ Tidak perlu setup lokal
- ✅ Otomatis setiap push
- ✅ Download APK dari Artifacts

**Cara Setup:**
1. Push code ke GitHub repository
2. GitHub Actions akan otomatis build
3. Download APK dari Actions → Artifacts

Mau saya buatkan workflow file-nya?

---

## 📊 Perbandingan Solusi

| Solusi | Waktu Setup | Waktu Build | Ukuran Download | Kesulitan |
|--------|-------------|-------------|-----------------|-----------|
| **Android SDK** | 30 min | 5-10 min | 1-2 GB | Medium |
| **Docker** | 5 min | 20-30 min (first) | 3-4 GB | Easy |
| **Visual Studio** | 60 min | 5-10 min | 5-10 GB | Easy |
| **GitHub Actions** | 10 min | 15-20 min | 0 GB | Very Easy |

---

## 🎯 Rekomendasi Saya

### Untuk Anda (Linux User):

**Jika punya Docker:**
```bash
cd VIRA-UNO
./docker-build.sh
```

**Jika tidak punya Docker:**
```bash
cd VIRA-UNO
./install-android-sdk.sh
source ~/.bashrc
./build.sh
```

### Untuk Development Jangka Panjang:
Install Android SDK (Solusi 1) - Anda akan sering build

### Untuk One-Time Build:
Docker (Solusi 2) atau GitHub Actions (Solusi 4)

---

## ❓ FAQ

### Q: Berapa lama total waktu untuk build pertama kali?
**A:** 
- Dengan Android SDK: ~40-50 menit (install 30 min + build 10 min)
- Dengan Docker: ~25-35 menit (build image 20 min + compile 5 min)
- Dengan GitHub Actions: ~20 menit (otomatis di cloud)

### Q: Apakah saya harus install Android Studio?
**A:** Tidak. Command line tools sudah cukup. Android Studio opsional.

### Q: Apakah bisa build di cloud tanpa setup lokal?
**A:** Ya! Gunakan GitHub Actions (Solusi 4). Saya bisa buatkan workflow-nya.

### Q: Ukuran APK final berapa?
**A:** ~15-25 MB (tergantung optimasi)

### Q: Apakah perlu API Key untuk build?
**A:** Tidak. API Key diinput saat aplikasi pertama kali dibuka.

---

## 🚀 Quick Decision Tree

```
Punya Docker? 
├─ Ya → Gunakan docker-build.sh (TERCEPAT)
└─ Tidak
   ├─ Punya Windows PC? 
   │  └─ Ya → Build di Visual Studio (TERMUDAH)
   └─ Tidak
      ├─ Mau setup development environment?
      │  └─ Ya → Install Android SDK (TERBAIK untuk long-term)
      └─ Tidak
         └─ Gunakan GitHub Actions (PALING MUDAH, no setup)
```

---

## 💡 Mau Saya Bantu?

Pilih salah satu dan saya akan guide step-by-step:

1. **"Install Android SDK"** - Saya akan guide install SDK
2. **"Setup Docker"** - Saya akan guide setup Docker build
3. **"GitHub Actions"** - Saya akan buatkan workflow file
4. **"Cari alternatif lain"** - Saya akan carikan solusi lain

Mana yang Anda pilih?
