# 📱 Cara Termudah Mendapatkan APK VIRA

## ✨ Metode GitHub Actions (TIDAK PERLU INSTALL APAPUN!)

### ⏱️ Total Waktu: 15-20 Menit
### 💰 Biaya: GRATIS
### 🛠️ Setup Lokal: TIDAK PERLU

---

## 📋 Langkah 1: Buat Repository di GitHub (2 menit)

1. **Buka browser** dan kunjungi: https://github.com/new

2. **Isi form:**
   - Repository name: `vira-android-app`
   - Description: `VIRA - AI Assistant for Android with Gemini`
   - Pilih: **Public** (atau Private jika mau)
   - ⚠️ **JANGAN** centang "Add a README file"
   - ⚠️ **JANGAN** pilih .gitignore atau license
   - Klik **"Create repository"**

3. **Copy URL repository** yang muncul
   - Akan terlihat seperti: `https://github.com/USERNAME/vira-android-app.git`
   - Ganti `USERNAME` dengan username GitHub Anda

---

## 📋 Langkah 2: Push Code ke GitHub (3 menit)

Buka terminal dan jalankan:

```bash
cd ~/Test\ ai/VIRA-UNO

# Add remote (ganti USERNAME dengan username GitHub Anda)
git remote add origin https://github.com/USERNAME/vira-android-app.git

# Rename branch ke main
git branch -M main

# Push ke GitHub
git push -u origin main
```

**Jika diminta username/password:**
- Username: username GitHub Anda
- Password: Gunakan **Personal Access Token** (bukan password biasa)
  - Buat token di: https://github.com/settings/tokens
  - Klik "Generate new token (classic)"
  - Pilih scope: `repo`
  - Copy token dan paste sebagai password

---

## 📋 Langkah 3: Monitor Build (10-15 menit)

Setelah push berhasil:

1. **Buka repository di browser:**
   ```
   https://github.com/USERNAME/vira-android-app
   ```

2. **Klik tab "Actions"** (di bagian atas)

3. **Lihat workflow "Build VIRA APK"** sedang berjalan
   - Status: 🟡 Kuning (sedang berjalan)
   - Tunggu sampai: ✅ Hijau (selesai)
   - Atau: ❌ Merah (error - beritahu saya jika ini terjadi)

4. **Progress yang akan Anda lihat:**
   ```
   ✓ Checkout code          (30 detik)
   ✓ Setup .NET             (1 menit)
   ✓ Setup Java JDK         (1 menit)
   ✓ Install Uno Templates  (2 menit)
   ✓ Restore dependencies   (3 menit)
   ✓ Build APK              (5-8 menit)
   ✓ Upload APK             (1 menit)
   ```

---

## 📋 Langkah 4: Download APK (1 menit)

Setelah build selesai (✅ hijau):

1. **Klik workflow run** yang baru saja selesai

2. **Scroll ke bawah** sampai section **"Artifacts"**

3. **Klik "VIRA-Release-APK"** untuk download

4. **Extract file ZIP** yang di-download
   - Akan ada file `.apk` di dalamnya

5. **Transfer APK ke HP Android Anda**
   - Via USB
   - Via Bluetooth
   - Via Google Drive/Dropbox
   - Via WhatsApp (kirim ke diri sendiri)

---

## 📋 Langkah 5: Install di HP (2 menit)

1. **Buka File Manager** di HP

2. **Cari file APK** yang sudah ditransfer

3. **Tap file APK**

4. **Jika muncul warning "Install from Unknown Sources":**
   - Tap "Settings"
   - Enable "Allow from this source"
   - Kembali dan tap APK lagi

5. **Tap "Install"**

6. **Tap "Open"** setelah instalasi selesai

---

## 📋 Langkah 6: Setup API Key (1 menit)

Saat pertama kali buka aplikasi:

1. **Tap icon Settings** (⚙️) di kanan atas

2. **Paste Gemini API Key** Anda di field yang tersedia

3. **Tap "Simpan API Key"**

4. **Tap "Test Koneksi"** untuk verifikasi

5. **Kembali ke chat** dan mulai berbicara dengan Vira!

---

## 🎉 Selesai!

Anda sekarang punya aplikasi VIRA di HP Android Anda!

---

## 🔄 Update Aplikasi di Masa Depan

Jika Anda ingin update fitur atau fix bug:

```bash
cd ~/Test\ ai/VIRA-UNO

# Edit file yang ingin diubah
# ...

# Commit dan push
git add .
git commit -m "Update: deskripsi perubahan"
git push
```

GitHub Actions akan otomatis build APK baru!

---

## 📊 Perbandingan Metode

| Metode | Setup | Build | Download | Total |
|--------|-------|-------|----------|-------|
| **GitHub Actions** | 5 min | 10-15 min | 0 GB | ✅ **15-20 min** |
| Install Android SDK | 30 min | 5-10 min | 1-2 GB | 35-40 min |
| Docker | 10 min | 20-30 min | 3-4 GB | 30-40 min |
| Visual Studio | 60 min | 5-10 min | 5-10 GB | 65-70 min |

**GitHub Actions adalah PEMENANGNYA! 🏆**

---

## ❓ FAQ

### Q: Apakah GitHub Actions gratis?
**A:** Ya! Gratis untuk public repositories. Private repos dapat 2,000 menit/bulan gratis.

### Q: Berapa lama build APK?
**A:** 10-15 menit untuk build pertama. Build berikutnya bisa lebih cepat (~8-10 menit).

### Q: Apakah saya perlu install Android SDK?
**A:** TIDAK! Semua build dilakukan di server GitHub.

### Q: Apakah API Key aman?
**A:** Ya! API Key TIDAK di-commit ke GitHub. User input sendiri saat pertama kali buka aplikasi.

### Q: Bagaimana jika build gagal?
**A:** Cek tab Actions → klik workflow yang failed → lihat error log. Beritahu saya error-nya dan saya akan bantu fix.

### Q: Bisakah saya share APK ke orang lain?
**A:** Ya! APK bisa di-share. Tapi setiap user harus input API Key Gemini mereka sendiri.

### Q: Apakah bisa build untuk iOS?
**A:** Uno Platform support iOS, tapi perlu Mac untuk build. Untuk sekarang fokus ke Android dulu.

---

## 💡 Tips Pro

1. **Bookmark halaman Actions:**
   ```
   https://github.com/USERNAME/vira-android-app/actions
   ```
   Untuk cepat cek status build

2. **Enable email notifications:**
   GitHub akan email Anda saat build selesai atau gagal

3. **Create releases:**
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```
   APK akan otomatis attached ke release!

4. **Download APK langsung dari Releases:**
   ```
   https://github.com/USERNAME/vira-android-app/releases
   ```
   Lebih mudah untuk share ke orang lain

---

## 🆘 Butuh Bantuan?

Jika ada masalah di langkah manapun:

1. **Screenshot error message**
2. **Copy paste error log**
3. **Beritahu saya di mana stuck**

Saya siap membantu! 😊

---

**Selamat mencoba! Semoga berhasil! 🚀**
