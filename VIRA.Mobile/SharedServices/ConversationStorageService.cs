using Android.Content;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VIRA.Mobile.SharedModels;

namespace VIRA.Mobile.SharedServices;

/// <summary>
/// Service for persisting conversations to local storage using JSON files
/// </summary>
public class ConversationStorageService
{
    private const string TAG = "ConversationStorageService";
    private readonly string _conversationsDir;
    private readonly JsonSerializerSettings _jsonSettings;

    public ConversationStorageService(Context context)
    {
        // Use internal storage directory for conversations
        var filesDir = context.FilesDir?.AbsolutePath ?? throw new InvalidOperationException("FilesDir is null");
        _conversationsDir = Path.Combine(filesDir, "conversations");
        
        // Create conversations directory if it doesn't exist
        if (!Directory.Exists(_conversationsDir))
        {
            Directory.CreateDirectory(_conversationsDir);
            Android.Util.Log.Info(TAG, $"Created conversations directory: {_conversationsDir}");
        }

        // Configure JSON serialization settings
        _jsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc
        };
    }

    /// <summary>
    /// Gets the file path for a conversation
    /// </summary>
    private string GetConversationFile(string conversationId)
    {
        return Path.Combine(_conversationsDir, $"{conversationId}.json");
    }

    /// <summary>
    /// Serializes a conversation to JSON string
    /// </summary>
    private string SerializeConversation(Conversation conversation)
    {
        return JsonConvert.SerializeObject(conversation, _jsonSettings);
    }

    /// <summary>
    /// Deserializes a conversation from JSON string
    /// </summary>
    private Conversation? DeserializeConversation(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<Conversation>(json, _jsonSettings);
        }
        catch (JsonException ex)
        {
            Android.Util.Log.Error(TAG, $"Failed to deserialize conversation: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Saves a conversation to local storage
    /// </summary>
    public void SaveConversation(Conversation conversation)
    {
        try
        {
            var filePath = GetConversationFile(conversation.Id);
            var json = SerializeConversation(conversation);
            File.WriteAllText(filePath, json);
            Android.Util.Log.Info(TAG, $"Saved conversation {conversation.Id} to {filePath}");
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error(TAG, $"Failed to save conversation {conversation.Id}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Loads a conversation from local storage
    /// </summary>
    public Conversation? LoadConversation(string conversationId)
    {
        try
        {
            var filePath = GetConversationFile(conversationId);
            if (!File.Exists(filePath))
            {
                Android.Util.Log.Warn(TAG, $"Conversation file not found: {filePath}");
                return null;
            }

            var json = File.ReadAllText(filePath);
            var conversation = DeserializeConversation(json);
            
            if (conversation != null)
            {
                Android.Util.Log.Info(TAG, $"Loaded conversation {conversationId} with {conversation.GetMessageCount()} messages");
            }
            
            return conversation;
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error(TAG, $"Failed to load conversation {conversationId}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Loads all conversations from local storage
    /// </summary>
    public List<Conversation> LoadAllConversations()
    {
        var conversations = new List<Conversation>();
        
        try
        {
            if (!Directory.Exists(_conversationsDir))
            {
                Android.Util.Log.Warn(TAG, "Conversations directory does not exist");
                return conversations;
            }

            var files = Directory.GetFiles(_conversationsDir, "*.json");
            Android.Util.Log.Info(TAG, $"Found {files.Length} conversation files");

            foreach (var file in files)
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var conversation = DeserializeConversation(json);
                    
                    if (conversation != null)
                    {
                        conversations.Add(conversation);
                    }
                    else
                    {
                        Android.Util.Log.Warn(TAG, $"Skipping corrupted conversation file: {file}");
                    }
                }
                catch (Exception ex)
                {
                    Android.Util.Log.Error(TAG, $"Error loading conversation from {file}: {ex.Message}");
                    // Continue loading other conversations
                }
            }

            // Sort by UpdatedAt descending (most recent first)
            conversations = conversations.OrderByDescending(c => c.UpdatedAt).ToList();
            Android.Util.Log.Info(TAG, $"Successfully loaded {conversations.Count} conversations");
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error(TAG, $"Failed to load conversations: {ex.Message}");
        }

        return conversations;
    }

    /// <summary>
    /// Deletes a single conversation from local storage
    /// </summary>
    public void DeleteConversation(string conversationId)
    {
        try
        {
            var filePath = GetConversationFile(conversationId);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Android.Util.Log.Info(TAG, $"Deleted conversation {conversationId}");
            }
            else
            {
                Android.Util.Log.Warn(TAG, $"Conversation file not found for deletion: {filePath}");
            }
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error(TAG, $"Failed to delete conversation {conversationId}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Deletes all conversations from local storage
    /// </summary>
    public void DeleteAllConversations()
    {
        try
        {
            if (!Directory.Exists(_conversationsDir))
            {
                Android.Util.Log.Warn(TAG, "Conversations directory does not exist");
                return;
            }

            var files = Directory.GetFiles(_conversationsDir, "*.json");
            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    Android.Util.Log.Error(TAG, $"Failed to delete file {file}: {ex.Message}");
                }
            }

            Android.Util.Log.Info(TAG, $"Deleted {files.Length} conversation files");
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error(TAG, $"Failed to delete all conversations: {ex.Message}");
            throw;
        }
    }
}
