# VIRA - Quick Start Guide

## 🚀 Memulai dalam 5 Menit

### 1. Install Prerequisites

```bash
# Install Uno Platform templates
dotnet new install Uno.Templates
```

Pastikan sudah terinstall:
- Visual Studio 2022 (v17.8+)
- .NET 8 SDK
- Android SDK (API 21-34)

### 2. Clone & Build

```bash
cd VIRA-UNO
dotnet restore
dotnet build
```

### 3. Setup Gemini API Key

1. Dapatkan API Key: https://aistudio.google.com/app/apikey
2. Buka aplikasi VIRA di Android
3. Masuk ke Settings → Masukkan API Key → Simpan

### 4. Deploy ke Android

**Via Visual Studio:**
1. Buka `VIRA.sln`
2. Set `VIRA.Mobile` sebagai Startup Project
3. Pilih Android device/emulator
4. Tekan F5

**Via CLI:**
```bash
dotnet build VIRA.Mobile/VIRA.Mobile.csproj -f net8.0-android -t:Run
```

## 📱 Cara Menggunakan

### Chat dengan Teks
1. Ketik pesan di input box bawah
2. Tekan tombol Send (✈️)
3. Vira akan merespons dengan suara dan teks

### Chat dengan Suara
1. Tekan tombol Microphone (🎤)
2. Ucapkan pertanyaan Anda
3. Vira akan mendengar, memproses, dan menjawab

### Quick Actions
Gunakan tombol shortcut untuk perintah cepat:
- 🌤️ Cuaca - Cek cuaca hari ini
- 📰 Berita - Lihat berita terkini
- 📅 Jadwal - Tampilkan jadwal
- 🚗 Lalu Lintas - Cek kondisi jalan

## 🎨 Desain UI

Aplikasi menggunakan:
- **Dark Mode** dengan warna Slate (#101622)
- **Glassmorphism** untuk efek kaca buram
- **Gradient Ambient** untuk atmosfer modern
- **Smooth Animations** untuk transisi halus

## 🔧 Troubleshooting Cepat

**Build Error?**
```bash
dotnet clean
dotnet restore
dotnet build
```

**Voice tidak berfungsi?**
- Cek permission Microphone di Settings → Apps → VIRA

**API Error?**
- Verifikasi API Key di Settings
- Test koneksi dengan tombol "Test Koneksi"

## 📚 Dokumentasi Lengkap

Lihat `IMPLEMENTATION_GUIDE.md` untuk:
- Struktur proyek detail
- Customization guide
- Function calling implementation
- Advanced features

## 🎯 Fitur Utama

✅ Chat AI dengan Gemini 1.5 Flash
✅ Voice Input (Speech-to-Text)
✅ Voice Output (Text-to-Speech) suara perempuan Indonesia
✅ Quick Actions untuk perintah cepat
✅ Dark Mode dengan Glassmorphism
✅ Conversation History
✅ Typing Indicator
✅ Settings Management

## 🔜 Coming Soon

- Function Calling untuk cuaca real-time
- News API integration
- Reminder system
- Traffic updates
- Multi-language support

## 💡 Tips

1. **Hemat Baterai**: Matikan voice output jika tidak diperlukan
2. **Respons Cepat**: Gunakan Quick Actions untuk perintah umum
3. **Clear Chat**: Tap menu (☰) untuk clear conversation history
4. **API Quota**: Gemini API gratis memiliki limit, gunakan bijak

## 🆘 Butuh Bantuan?

- Baca `IMPLEMENTATION_GUIDE.md` untuk detail teknis
- Cek `VIRA Virtual Intelligent.txt` untuk konsep awal
- Lihat kode di `VIRA.Shared/` untuk referensi

---

**Selamat menggunakan VIRA! 🎉**
