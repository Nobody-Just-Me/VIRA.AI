using Android.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VIRA.Mobile.SharedModels;

namespace VIRA.Mobile.SharedServices;

/// <summary>
/// Manages conversation lifecycle and coordinates with storage service
/// </summary>
public class ConversationManager
{
    private const string TAG = "ConversationManager";
    private readonly ConversationStorageService _storageService;
    private readonly Context _context;
    
    private Conversation? _currentConversation;
    private List<Conversation> _conversations;
    private Dictionary<string, ConversationMetadata> _metadataCache;

    public ConversationManager(ConversationStorageService storageService, Context context)
    {
        _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _conversations = new List<Conversation>();
        _metadataCache = new Dictionary<string, ConversationMetadata>();
    }

    /// <summary>
    /// Loads all conversations from storage asynchronously
    /// </summary>
    public async Task<List<Conversation>> LoadAllConversationsAsync()
    {
        try
        {
            return await Task.Run(() =>
            {
                _conversations = _storageService.LoadAllConversations();
                
                // Build metadata cache for quick access
                _metadataCache.Clear();
                foreach (var conv in _conversations)
                {
                    _metadataCache[conv.Id] = new ConversationMetadata
                    {
                        Id = conv.Id,
                        Title = conv.Title,
                        CreatedAt = conv.CreatedAt,
                        UpdatedAt = conv.UpdatedAt,
                        MessageCount = conv.Messages.Count
                    };
                }
                
                Android.Util.Log.Info(TAG, $"Loaded {_conversations.Count} conversations asynchronously");
                return _conversations;
            });
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error(TAG, $"Failed to load conversations: {ex.Message}");
            _conversations = new List<Conversation>();
            return _conversations;
        }
    }

    /// <summary>
    /// Loads all conversations from storage (synchronous version for backward compatibility)
    /// </summary>
    public List<Conversation> LoadAllConversations()
    {
        try
        {
            _conversations = _storageService.LoadAllConversations();
            
            // Build metadata cache for quick access
            _metadataCache.Clear();
            foreach (var conv in _conversations)
            {
                _metadataCache[conv.Id] = new ConversationMetadata
                {
                    Id = conv.Id,
                    Title = conv.Title,
                    CreatedAt = conv.CreatedAt,
                    UpdatedAt = conv.UpdatedAt,
                    MessageCount = conv.Messages.Count
                };
            }
            
            Android.Util.Log.Info(TAG, $"Loaded {_conversations.Count} conversations");
            return _conversations;
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error(TAG, $"Failed to load conversations: {ex.Message}");
            _conversations = new List<Conversation>();
            return _conversations;
        }
    }
    
    /// <summary>
    /// Gets conversation metadata without loading full message history
    /// </summary>
    public List<ConversationMetadata> GetConversationMetadata()
    {
        return _metadataCache.Values.OrderByDescending(m => m.UpdatedAt).ToList();
    }

    /// <summary>
    /// Creates a new conversation
    /// </summary>
    public Conversation CreateNewConversation()
    {
        var conversation = new Conversation
        {
            Id = Guid.NewGuid().ToString(),
            Title = "New Conversation",
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Messages = new List<ChatMessage>()
        };

        _currentConversation = conversation;
        _conversations.Insert(0, conversation); // Add to beginning of list
        
        // Add to metadata cache
        _metadataCache[conversation.Id] = new ConversationMetadata
        {
            Id = conversation.Id,
            Title = conversation.Title,
            CreatedAt = conversation.CreatedAt,
            UpdatedAt = conversation.UpdatedAt,
            MessageCount = 0
        };
        
        // Save immediately
        _storageService.SaveConversation(conversation);
        
        Android.Util.Log.Info(TAG, $"Created new conversation: {conversation.Id}");
        return conversation;
    }

    /// <summary>
    /// Switches to a different conversation (saves current first)
    /// </summary>
    public Conversation? SwitchToConversation(string conversationId)
    {
        try
        {
            // Save current conversation before switching
            if (_currentConversation != null)
            {
                SaveCurrentConversation();
            }

            // Load the requested conversation
            var conversation = _conversations.FirstOrDefault(c => c.Id == conversationId);
            if (conversation == null)
            {
                // Try loading from storage if not in memory
                conversation = _storageService.LoadConversation(conversationId);
                if (conversation != null)
                {
                    _conversations.Add(conversation);
                }
            }

            if (conversation != null)
            {
                _currentConversation = conversation;
                Android.Util.Log.Info(TAG, $"Switched to conversation: {conversationId}");
            }
            else
            {
                Android.Util.Log.Warn(TAG, $"Conversation not found: {conversationId}");
            }

            return conversation;
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error(TAG, $"Failed to switch conversation: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Gets the current conversation
    /// </summary>
    public Conversation? GetCurrentConversation()
    {
        return _currentConversation;
    }

    /// <summary>
    /// Adds a message to the current conversation and saves immediately
    /// </summary>
    public void AddMessageToCurrentConversation(ChatMessage message)
    {
        if (_currentConversation == null)
        {
            Android.Util.Log.Warn(TAG, "No current conversation - creating new one");
            CreateNewConversation();
        }

        if (_currentConversation != null)
        {
            _currentConversation.Messages.Add(message);
            _currentConversation.UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            
            // Auto-generate title from first user message if title is default
            if (_currentConversation.Title == "New Conversation" && message.Role == ChatMessageRole.User && !string.IsNullOrEmpty(message.Content))
            {
                _currentConversation.Title = message.Content.Length > 50 
                    ? message.Content.Substring(0, 47) + "..." 
                    : message.Content;
            }

            // Update metadata cache
            if (_metadataCache.ContainsKey(_currentConversation.Id))
            {
                _metadataCache[_currentConversation.Id].Title = _currentConversation.Title;
                _metadataCache[_currentConversation.Id].UpdatedAt = _currentConversation.UpdatedAt;
                _metadataCache[_currentConversation.Id].MessageCount = _currentConversation.Messages.Count;
            }

            // Save immediately after adding message
            SaveCurrentConversation();
            
            Android.Util.Log.Info(TAG, $"Added message to conversation {_currentConversation.Id}");
        }
    }

    /// <summary>
    /// Saves the current conversation to storage
    /// </summary>
    public void SaveCurrentConversation()
    {
        if (_currentConversation != null)
        {
            try
            {
                _storageService.SaveConversation(_currentConversation);
                Android.Util.Log.Info(TAG, $"Saved current conversation: {_currentConversation.Id}");
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error(TAG, $"Failed to save current conversation: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Deletes all conversations from storage and memory
    /// </summary>
    public void DeleteAllConversations()
    {
        try
        {
            _storageService.DeleteAllConversations();
            _conversations.Clear();
            _currentConversation = null;
            Android.Util.Log.Info(TAG, "Deleted all conversations");
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error(TAG, $"Failed to delete all conversations: {ex.Message}");
            throw;
        }
    }
}
