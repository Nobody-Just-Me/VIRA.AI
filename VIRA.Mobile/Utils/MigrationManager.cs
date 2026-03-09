using Android.Content;
using VIRA.Mobile.SharedModels;
using VIRA.Mobile.SharedServices;
using System;
using System.Collections.Generic;

namespace VIRA.Mobile.Utils;

/// <summary>
/// Manages data migration for existing users upgrading to new versions
/// </summary>
public class MigrationManager
{
    private const string PREF_KEY_VERSION = "app_version";
    private const string PREF_KEY_FIRST_LAUNCH = "first_launch";
    private const string CURRENT_VERSION = "2.4.0";
    
    private readonly Context _context;
    private readonly ISharedPreferences _prefs;
    private readonly ConversationStorageService _storageService;
    
    public MigrationManager(Context context, ConversationStorageService storageService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        _prefs = context.GetSharedPreferences("vira_settings", FileCreationMode.Private)!;
    }
    
    /// <summary>
    /// Checks if this is the first launch of the app
    /// </summary>
    public bool IsFirstLaunch()
    {
        return _prefs.GetBoolean(PREF_KEY_FIRST_LAUNCH, true);
    }
    
    /// <summary>
    /// Checks if onboarding should be shown
    /// </summary>
    public bool ShouldShowOnboarding()
    {
        // Show onboarding if it hasn't been completed yet
        return !_prefs.GetBoolean("onboarding_completed", false);
    }
    
    /// <summary>
    /// Checks if migration is needed based on version
    /// </summary>
    public bool NeedsMigration()
    {
        var savedVersion = _prefs.GetString(PREF_KEY_VERSION, null);
        return savedVersion != CURRENT_VERSION;
    }
    
    /// <summary>
    /// Performs migration from old version to current version
    /// </summary>
    public void PerformMigration()
    {
        var savedVersion = _prefs.GetString(PREF_KEY_VERSION, null);
        
        Android.Util.Log.Info("VIRA_Migration", "========================================");
        Android.Util.Log.Info("VIRA_Migration", $"🔄 Starting Migration");
        Android.Util.Log.Info("VIRA_Migration", $"   From Version: {savedVersion ?? "None (new install)"}");
        Android.Util.Log.Info("VIRA_Migration", $"   To Version: {CURRENT_VERSION}");
        
        try
        {
            // If no saved version, this is a new install or very old version
            if (string.IsNullOrEmpty(savedVersion))
            {
                MigrateFromLegacy();
            }
            else
            {
                // Version-specific migrations can be added here
                // For example: if (savedVersion == "2.3.0") { MigrateFrom230(); }
            }
            
            // Mark migration as complete
            var editor = _prefs.Edit();
            editor?.PutString(PREF_KEY_VERSION, CURRENT_VERSION);
            editor?.PutBoolean(PREF_KEY_FIRST_LAUNCH, false);
            editor?.Apply();
            
            Android.Util.Log.Info("VIRA_Migration", "✅ Migration completed successfully");
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error("VIRA_Migration", $"❌ Migration failed: {ex.Message}");
            Android.Util.Log.Error("VIRA_Migration", ex.StackTrace ?? "No stack trace");
        }
        finally
        {
            Android.Util.Log.Info("VIRA_Migration", "========================================");
        }
    }
    
    /// <summary>
    /// Migrates data from legacy versions (pre-2.4.0)
    /// </summary>
    private void MigrateFromLegacy()
    {
        Android.Util.Log.Info("VIRA_Migration", "📦 Migrating from legacy version...");
        
        // 1. Check if there are any existing conversations
        var existingConversations = _storageService.LoadAllConversations();
        
        if (existingConversations.Count == 0)
        {
            // No existing data, create a welcome conversation
            CreateWelcomeConversation();
        }
        else
        {
            Android.Util.Log.Info("VIRA_Migration", $"   Found {existingConversations.Count} existing conversations");
        }
        
        // 2. Migrate settings to new format if needed
        MigrateSettings();
        
        Android.Util.Log.Info("VIRA_Migration", "✅ Legacy migration complete");
    }
    
    /// <summary>
    /// Creates a welcome conversation for new users
    /// </summary>
    private void CreateWelcomeConversation()
    {
        Android.Util.Log.Info("VIRA_Migration", "   Creating welcome conversation...");
        
        var welcomeConversation = new Conversation
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Welcome to VIRA",
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Messages = new List<ChatMessage>
            {
                new ChatMessage
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = "👋 Hello! I'm VIRA, your Voice Intelligent Responsive Assistant.\n\n" +
                             "I can help you with:\n" +
                             "• Answering questions\n" +
                             "• Managing tasks\n" +
                             "• Getting weather updates\n" +
                             "• Reading news\n" +
                             "• And much more!\n\n" +
                             "To get started, make sure you've configured your AI provider in Settings. " +
                             "Then just ask me anything!",
                    Role = ChatMessageRole.Assistant,
                    Timestamp = DateTime.UtcNow,
                    Type = MessageType.Text
                }
            }
        };
        
        _storageService.SaveConversation(welcomeConversation);
        
        // Mark this as the active conversation
        var editor = _prefs.Edit();
        editor?.PutString("active_conversation_id", welcomeConversation.Id);
        editor?.Apply();
        
        Android.Util.Log.Info("VIRA_Migration", "   ✅ Welcome conversation created");
    }
    
    /// <summary>
    /// Migrates settings from old format to new format
    /// </summary>
    private void MigrateSettings()
    {
        Android.Util.Log.Info("VIRA_Migration", "   Migrating settings...");
        
        var editor = _prefs.Edit();
        
        // Check if API provider is set, if not set default
        var provider = _prefs.GetString("ai_provider", null);
        if (string.IsNullOrEmpty(provider))
        {
            editor?.PutString("ai_provider", "gemini");
            Android.Util.Log.Info("VIRA_Migration", "   Set default provider: gemini");
        }
        
        // Check if theme preference is set, if not set default
        var theme = _prefs.GetString("theme_preference", null);
        if (string.IsNullOrEmpty(theme))
        {
            editor?.PutString("theme_preference", "dark");
            Android.Util.Log.Info("VIRA_Migration", "   Set default theme: dark");
        }
        
        // Ensure voice output is enabled by default if not set
        if (!_prefs.Contains("voice_output_enabled"))
        {
            editor?.PutBoolean("voice_output_enabled", true);
            Android.Util.Log.Info("VIRA_Migration", "   Set default voice output: enabled");
        }
        
        // Ensure web browsing is enabled by default if not set
        if (!_prefs.Contains("web_browsing_enabled"))
        {
            editor?.PutBoolean("web_browsing_enabled", true);
            Android.Util.Log.Info("VIRA_Migration", "   Set default web browsing: enabled");
        }
        
        // Ensure memory mode is enabled by default if not set
        if (!_prefs.Contains("memory_mode_enabled"))
        {
            editor?.PutBoolean("memory_mode_enabled", true);
            Android.Util.Log.Info("VIRA_Migration", "   Set default memory mode: enabled");
        }
        
        // Ensure privacy mode is disabled by default if not set
        if (!_prefs.Contains("privacy_mode_enabled"))
        {
            editor?.PutBoolean("privacy_mode_enabled", false);
            Android.Util.Log.Info("VIRA_Migration", "   Set default privacy mode: disabled");
        }
        
        editor?.Apply();
        
        Android.Util.Log.Info("VIRA_Migration", "   ✅ Settings migration complete");
    }
    
    /// <summary>
    /// Gets the saved app version
    /// </summary>
    public string? GetSavedVersion()
    {
        return _prefs.GetString(PREF_KEY_VERSION, null);
    }
    
    /// <summary>
    /// Gets the current app version
    /// </summary>
    public string GetCurrentVersion()
    {
        return CURRENT_VERSION;
    }
}
