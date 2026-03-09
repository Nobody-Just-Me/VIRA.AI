namespace VIRA.Shared.Models;

/// <summary>
/// Base class for all VIRA errors
/// </summary>
public abstract class ViraError
{
    public string Message { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

/// <summary>
/// Network-related errors
/// </summary>
public class NetworkError : ViraError
{
    public NetworkErrorType Type { get; set; }
    public string? Url { get; set; }
    public int? StatusCode { get; set; }

    public NetworkError(NetworkErrorType type, string message)
    {
        Type = type;
        Message = message;
        Code = $"NET_{(int)type:D3}";
    }
}

public enum NetworkErrorType
{
    NoConnection = 1,
    Timeout = 2,
    ServerError = 3,
    Unauthorized = 4,
    NotFound = 5,
    RateLimited = 6,
    Unknown = 99
}

/// <summary>
/// Permission-related errors
/// </summary>
public class PermissionError : ViraError
{
    public PermissionErrorType Type { get; set; }
    public List<string> RequiredPermissions { get; set; } = new();
    public bool IsPermanentlyDenied { get; set; }

    public PermissionError(PermissionErrorType type, string message, List<string>? permissions = null)
    {
        Type = type;
        Message = message;
        Code = $"PERM_{(int)type:D3}";
        RequiredPermissions = permissions ?? new List<string>();
    }
}

public enum PermissionErrorType
{
    MicrophoneDenied = 1,
    ContactsDenied = 2,
    PhoneDenied = 3,
    CameraDenied = 4,
    StorageDenied = 5,
    LocationDenied = 6,
    NotificationDenied = 7,
    Unknown = 99
}

/// <summary>
/// Data-related errors
/// </summary>
public class DataError : ViraError
{
    public DataErrorType Type { get; set; }
    public string? EntityType { get; set; }
    public string? EntityId { get; set; }

    public DataError(DataErrorType type, string message)
    {
        Type = type;
        Message = message;
        Code = $"DATA_{(int)type:D3}";
    }
}

public enum DataErrorType
{
    NotFound = 1,
    InvalidFormat = 2,
    CorruptedData = 3,
    StorageFull = 4,
    SaveFailed = 5,
    LoadFailed = 6,
    DeleteFailed = 7,
    Unknown = 99
}

/// <summary>
/// Processing-related errors
/// </summary>
public class ProcessingError : ViraError
{
    public ProcessingErrorType Type { get; set; }
    public string? InputText { get; set; }
    public string? Provider { get; set; }

    public ProcessingError(ProcessingErrorType type, string message)
    {
        Type = type;
        Message = message;
        Code = $"PROC_{(int)type:D3}";
    }
}

public enum ProcessingErrorType
{
    PatternMatchFailed = 1,
    AINotConfigured = 2,
    AIRequestFailed = 3,
    InvalidResponse = 4,
    TimeoutError = 5,
    RateLimitExceeded = 6,
    InvalidApiKey = 7,
    Unknown = 99
}

/// <summary>
/// Voice-related errors
/// </summary>
public class VoiceError : ViraError
{
    public VoiceErrorType Type { get; set; }

    public VoiceError(VoiceErrorType type, string message)
    {
        Type = type;
        Message = message;
        Code = $"VOICE_{(int)type:D3}";
    }
}

public enum VoiceErrorType
{
    NotAvailable = 1,
    RecognitionFailed = 2,
    NoSpeechDetected = 3,
    TTSFailed = 4,
    AudioPlaybackFailed = 5,
    MicrophoneUnavailable = 6,
    Unknown = 99
}

/// <summary>
/// Error messages in Indonesian
/// </summary>
public static class ErrorMessages
{
    // Network errors
    public static string NoConnection => "❌ Tidak ada koneksi internet. Periksa koneksi Anda dan coba lagi.";
    public static string Timeout => "⏱️ Koneksi timeout. Server membutuhkan waktu terlalu lama untuk merespons.";
    public static string ServerError => "🔧 Server sedang mengalami masalah. Coba lagi nanti.";
    public static string Unauthorized => "🔒 API key tidak valid atau tidak memiliki akses.";
    public static string NotFound => "🔍 Resource tidak ditemukan.";
    public static string RateLimited => "⚠️ Terlalu banyak permintaan. Tunggu sebentar dan coba lagi.";

    // Permission errors
    public static string MicrophoneDenied => "🎤 Izin mikrofon diperlukan untuk fitur suara. Aktifkan di pengaturan.";
    public static string ContactsDenied => "📇 Izin kontak diperlukan untuk fitur ini. Aktifkan di pengaturan.";
    public static string PhoneDenied => "📞 Izin telepon diperlukan untuk melakukan panggilan. Aktifkan di pengaturan.";
    public static string CameraDenied => "📷 Izin kamera diperlukan untuk fitur ini. Aktifkan di pengaturan.";
    public static string StorageDenied => "💾 Izin penyimpanan diperlukan untuk fitur ini. Aktifkan di pengaturan.";

    // Data errors
    public static string DataNotFound => "📋 Data tidak ditemukan.";
    public static string InvalidFormat => "⚠️ Format data tidak valid.";
    public static string SaveFailed => "💾 Gagal menyimpan data. Coba lagi.";
    public static string LoadFailed => "📂 Gagal memuat data.";

    // Processing errors
    public static string AINotConfigured => "⚙️ AI belum dikonfigurasi. Silakan tambahkan API key di pengaturan.";
    public static string AIRequestFailed => "🤖 Gagal memproses permintaan AI. Coba lagi.";
    public static string InvalidApiKey => "🔑 API key tidak valid. Periksa kembali API key Anda.";
    public static string PatternMatchFailed => "❓ Maaf, saya tidak mengerti perintah Anda. Coba dengan kata lain.";

    // Voice errors
    public static string VoiceNotAvailable => "🎤 Fitur suara tidak tersedia di perangkat ini.";
    public static string RecognitionFailed => "🎤 Gagal mengenali suara. Coba lagi.";
    public static string NoSpeechDetected => "🔇 Tidak ada suara yang terdeteksi. Pastikan mikrofon berfungsi.";
    public static string TTSFailed => "🔊 Gagal memutar suara. Coba lagi.";

    // Generic errors
    public static string UnknownError => "❌ Terjadi kesalahan yang tidak diketahui.";
    public static string TryAgain => "Silakan coba lagi.";
    public static string ContactSupport => "Jika masalah berlanjut, hubungi dukungan.";

    /// <summary>
    /// Get user-friendly error message
    /// </summary>
    public static string GetMessage(ViraError error)
    {
        return error switch
        {
            NetworkError ne => ne.Type switch
            {
                NetworkErrorType.NoConnection => NoConnection,
                NetworkErrorType.Timeout => Timeout,
                NetworkErrorType.ServerError => ServerError,
                NetworkErrorType.Unauthorized => Unauthorized,
                NetworkErrorType.NotFound => NotFound,
                NetworkErrorType.RateLimited => RateLimited,
                _ => $"{UnknownError} (Network)"
            },
            PermissionError pe => pe.Type switch
            {
                PermissionErrorType.MicrophoneDenied => MicrophoneDenied,
                PermissionErrorType.ContactsDenied => ContactsDenied,
                PermissionErrorType.PhoneDenied => PhoneDenied,
                PermissionErrorType.CameraDenied => CameraDenied,
                PermissionErrorType.StorageDenied => StorageDenied,
                _ => $"{UnknownError} (Permission)"
            },
            DataError de => de.Type switch
            {
                DataErrorType.NotFound => DataNotFound,
                DataErrorType.InvalidFormat => InvalidFormat,
                DataErrorType.SaveFailed => SaveFailed,
                DataErrorType.LoadFailed => LoadFailed,
                _ => $"{UnknownError} (Data)"
            },
            ProcessingError pe => pe.Type switch
            {
                ProcessingErrorType.AINotConfigured => AINotConfigured,
                ProcessingErrorType.AIRequestFailed => AIRequestFailed,
                ProcessingErrorType.InvalidApiKey => InvalidApiKey,
                ProcessingErrorType.PatternMatchFailed => PatternMatchFailed,
                _ => $"{UnknownError} (Processing)"
            },
            VoiceError ve => ve.Type switch
            {
                VoiceErrorType.NotAvailable => VoiceNotAvailable,
                VoiceErrorType.RecognitionFailed => RecognitionFailed,
                VoiceErrorType.NoSpeechDetected => NoSpeechDetected,
                VoiceErrorType.TTSFailed => TTSFailed,
                _ => $"{UnknownError} (Voice)"
            },
            _ => UnknownError
        };
    }

    /// <summary>
    /// Get actionable suggestion for error
    /// </summary>
    public static string GetActionSuggestion(ViraError error)
    {
        return error switch
        {
            NetworkError => "Periksa koneksi internet Anda dan coba lagi.",
            PermissionError pe => pe.IsPermanentlyDenied 
                ? "Buka Pengaturan > Aplikasi > VIRA > Izin untuk mengaktifkan."
                : "Izinkan akses saat diminta.",
            DataError => "Coba restart aplikasi atau hapus cache.",
            ProcessingError pe when pe.Type == ProcessingErrorType.AINotConfigured 
                => "Buka Pengaturan dan tambahkan API key.",
            ProcessingError => "Coba dengan perintah yang lebih sederhana.",
            VoiceError => "Pastikan mikrofon berfungsi dan tidak digunakan aplikasi lain.",
            _ => TryAgain
        };
    }
}
