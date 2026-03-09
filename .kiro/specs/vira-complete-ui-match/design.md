# Design Document: VIRA Complete UI Match

## Overview

This design document specifies the technical implementation for achieving 100% UI match with the reference "uii vira" design and implementing multi-provider AI support in the VIRA Android application. The implementation will use Android native UI components (no XAML), leverage existing VIRA architecture, and maintain backward compatibility with all current features.

The design focuses on five main areas:
1. Chat History Sidebar with smooth animations
2. Enhanced Settings UI matching reference design
3. Multi-provider AI configuration (Gemini, Groq, OpenAI)
4. Main Chat UI updates for visual consistency
5. Data persistence for conversations and settings

## Architecture

### High-Level Architecture

The feature builds upon VIRA's existing architecture with these new components:

```
┌─────────────────────────────────────────────────────────────┐
│                        Presentation Layer                    │
├─────────────────────────────────────────────────────────────┤
│  MainActivity (existing)                                     │
│  ├─ ChatHistorySidebarFragment (NEW)                        │
│  ├─ MainChatFragment (UPDATED)                              │
│  └─ SettingsActivity (UPDATED)                              │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│                        ViewModel Layer                       │
├─────────────────────────────────────────────────────────────┤
│  MainChatViewModel (UPDATED)                                │
│  ├─ ConversationManager (NEW)                               │
│  └─ ProviderConfigManager (NEW)                             │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│                         Service Layer                        │
├─────────────────────────────────────────────────────────────┤
│  AIProviderManager (existing)                                │
│  ├─ GeminiProvider (existing)                               │
│  ├─ GroqProvider (existing)                                 │
│  └─ OpenAIProvider (existing)                               │
│                                                              │
│  ConversationStorageService (NEW)                           │
│  AnimationService (existing)                                │
│  PreferencesService (existing)                              │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│                          Data Layer                          │
├─────────────────────────────────────────────────────────────┤
│  SharedPreferences (Android)                                 │
│  Internal Storage (JSON files)                              │
└─────────────────────────────────────────────────────────────┘
```

### Design Principles

1. **Native Android UI**: Use Android Views, Fragments, and Material Design components
2. **Separation of Concerns**: UI, business logic, and data layers remain distinct
3. **Backward Compatibility**: All existing features continue to work
4. **Performance**: Smooth 60fps animations, efficient data loading
5. **Security**: Secure storage of API keys using Android best practices

## Components and Interfaces

### 1. ChatHistorySidebarFragment

A new Fragment that displays the conversation history sidebar.

```kotlin
class ChatHistorySidebarFragment : Fragment() {
    private lateinit var conversationManager: ConversationManager
    private lateinit var recyclerView: RecyclerView
    private lateinit var adapter: ConversationListAdapter
    
    // Public interface
    fun show()
    fun hide()
    fun refreshConversations()
    
    // Callbacks
    interface SidebarListener {
        fun onConversationSelected(conversationId: String)
        fun onNewChatRequested()
        fun onClearHistoryRequested()
    }
}
```

**Key Responsibilities:**
- Display list of conversations with titles and timestamps
- Handle user interactions (tap conversation, new chat, clear history)
- Animate slide in/out transitions
- Notify parent activity of user actions

### 2. ConversationManager

A new class managing conversation lifecycle and storage.

```kotlin
class ConversationManager(
    private val storageService: ConversationStorageService,
    private val context: Context
) {
    private var currentConversation: Conversation? = null
    private val conversations: MutableList<Conversation> = mutableListOf()
    
    // Public interface
    fun loadAllConversations(): List<Conversation>
    fun createNewConversation(): Conversation
    fun switchToConversation(conversationId: String): Conversation?
    fun addMessageToCurrentConversation(message: ChatMessage)
    fun deleteAllConversations()
    fun getCurrentConversation(): Conversation?
    fun saveCurrentConversation()
}
```

**Key Responsibilities:**
- Manage active conversation state
- Coordinate with storage service for persistence
- Provide conversation CRUD operations
- Track conversation metadata (title, timestamp, message count)

### 3. ConversationStorageService

A new service handling conversation persistence.

```kotlin
class ConversationStorageService(private val context: Context) {
    private val conversationsDir: File
    private val gson: Gson
    
    // Public interface
    fun saveConversation(conversation: Conversation)
    fun loadConversation(conversationId: String): Conversation?
    fun loadAllConversations(): List<Conversation>
    fun deleteConversation(conversationId: String)
    fun deleteAllConversations()
    
    // Internal methods
    private fun getConversationFile(conversationId: String): File
    private fun serializeConversation(conversation: Conversation): String
    private fun deserializeConversation(json: String): Conversation
}
```

**Key Responsibilities:**
- Serialize/deserialize conversations to/from JSON
- Manage file I/O for conversation storage
- Handle storage errors gracefully
- Ensure data integrity

### 4. ProviderConfigManager

A new class managing AI provider configuration.

```kotlin
class ProviderConfigManager(private val preferencesService: PreferencesService) {
    
    data class ProviderConfig(
        val provider: AIProvider,
        val model: String,
        val apiKey: String
    )
    
    enum class AIProvider {
        GEMINI, GROQ, OPENAI
    }
    
    // Public interface
    fun saveProviderConfig(config: ProviderConfig)
    fun loadProviderConfig(): ProviderConfig?
    fun getAvailableModels(provider: AIProvider): List<String>
    fun validateApiKey(apiKey: String): Boolean
}
```

**Key Responsibilities:**
- Manage provider selection and API key storage
- Provide model lists for each provider
- Validate API key format
- Coordinate with PreferencesService for persistence

### 5. Updated SettingsActivity

Enhanced settings screen with new UI sections.

```kotlin
class SettingsActivity : AppCompatActivity() {
    private lateinit var providerConfigManager: ProviderConfigManager
    private lateinit var preferencesService: PreferencesService
    
    // UI Components
    private lateinit var profileHeader: View
    private lateinit var themeSelector: ThemeButtonGroup
    private lateinit var providerSpinner: Spinner
    private lateinit var modelSpinner: Spinner
    private lateinit var apiKeyInput: EditText
    private lateinit var apiKeyToggle: ImageButton
    private lateinit var saveButton: Button
    
    // Settings sections
    private fun setupProfileHeader()
    private fun setupThemeSelection()
    private fun setupTogglePreferences()
    private fun setupDropdownSelectors()
    private fun setupProviderConfiguration()
    private fun saveConfiguration()
}
```

**Key Responsibilities:**
- Display all settings in organized card sections
- Handle provider and model selection
- Manage API key input with show/hide toggle
- Save all settings to SharedPreferences
- Match reference UI design exactly

### 6. Updated MainChatFragment

Enhanced main chat view with new header and sidebar integration.

```kotlin
class MainChatFragment : Fragment() {
    private lateinit var viewModel: MainChatViewModel
    private lateinit var conversationManager: ConversationManager
    private lateinit var sidebarFragment: ChatHistorySidebarFragment
    
    // UI Components
    private lateinit var hamburgerButton: ImageButton
    private lateinit var settingsButton: ImageButton
    private lateinit var greetingText: TextView
    private lateinit var quickActionsContainer: LinearLayout
    private lateinit var messagesRecyclerView: RecyclerView
    
    // Methods
    private fun setupHeader()
    private fun setupQuickActions()
    private fun setupMessageList()
    private fun toggleSidebar()
    private fun loadConversation(conversationId: String)
}
```

**Key Responsibilities:**
- Display chat interface matching reference design
- Integrate sidebar toggle functionality
- Coordinate with ConversationManager for message history
- Maintain existing voice and TTS features

## Data Models

### Conversation

```kotlin
data class Conversation(
    val id: String = UUID.randomUUID().toString(),
    val title: String,
    val createdAt: Long = System.currentTimeMillis(),
    val updatedAt: Long = System.currentTimeMillis(),
    val messages: MutableList<ChatMessage> = mutableListOf()
) {
    fun getDisplayTitle(): String {
        return if (title.isNotEmpty()) title else "New Conversation"
    }
    
    fun getFormattedTimestamp(): String {
        val sdf = SimpleDateFormat("MMM dd, yyyy HH:mm", Locale.getDefault())
        return sdf.format(Date(updatedAt))
    }
    
    fun getMessageCount(): Int = messages.size
}
```

### ChatMessage (existing, no changes needed)

```kotlin
data class ChatMessage(
    val id: String,
    val content: String,
    val isUser: Boolean,
    val timestamp: Long,
    val type: MessageType
)
```

### ProviderConfiguration

```kotlin
data class ProviderConfiguration(
    val provider: String, // "gemini", "groq", "openai"
    val model: String,
    val apiKey: String,
    val lastUpdated: Long = System.currentTimeMillis()
) {
    companion object {
        const val PREF_KEY_PROVIDER = "ai_provider"
        const val PREF_KEY_MODEL = "ai_model"
        const val PREF_KEY_API_KEY_PREFIX = "api_key_"
        
        fun getApiKeyPrefKey(provider: String): String {
            return "$PREF_KEY_API_KEY_PREFIX$provider"
        }
    }
}
```

### ThemePreference

```kotlin
enum class ThemePreference {
    LIGHT, DARK, SYSTEM;
    
    companion object {
        const val PREF_KEY = "theme_preference"
    }
}
```

## Correctness Properties


*A property is a characteristic or behavior that should hold true across all valid executions of a system—essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Chat History Sidebar Properties

**Property 1: Sidebar visibility toggle**
*For any* UI state, when the hamburger menu icon is tapped, the Chat_History_Sidebar should become visible with animation applied.
**Validates: Requirements 1.1**

**Property 2: Conversation list completeness**
*For any* set of saved conversations, when the Chat_History_Sidebar is displayed, all conversations should be rendered with their title and timestamp visible.
**Validates: Requirements 1.2**

**Property 3: Conversation loading**
*For any* conversation in the sidebar list, when tapped, the main chat view should display that conversation's complete message history.
**Validates: Requirements 1.3**

**Property 4: New chat creation**
*For any* UI state, when the "New Chat" button is tapped, a new conversation with zero messages should be created and the sidebar should close.
**Validates: Requirements 1.4**

**Property 5: History clearing**
*For any* set of conversations, when the "Clear History" button is confirmed, all conversations should be deleted and the conversation list should be empty.
**Validates: Requirements 1.5**

**Property 6: Outside tap closes sidebar**
*For any* tap location outside the Chat_History_Sidebar bounds, the sidebar should close with animation.
**Validates: Requirements 1.6**

**Property 7: Animation timing**
*For any* sidebar open or close animation, the transition should complete within 300ms.
**Validates: Requirements 1.7**

### Settings and Configuration Properties

**Property 8: Settings persistence**
*For any* setting change in the Settings_Activity, the new value should be immediately persisted to SharedPreferences and retrievable after app restart.
**Validates: Requirements 2.6**

**Property 9: Provider selection UI update**
*For any* AI provider selection (Gemini, Groq, OpenAI), the corresponding API key input field should be displayed.
**Validates: Requirements 3.5**

**Property 10: Provider configuration round-trip**
*For any* valid provider configuration (provider, model, API key), saving the configuration and restarting the application should restore the exact same configuration.
**Validates: Requirements 3.7, 3.8**

### Data Persistence Properties

**Property 11: Message persistence**
*For any* conversation and any new message, adding the message to the conversation should result in the updated conversation being saved to local storage immediately.
**Validates: Requirements 5.1**

**Property 12: Conversation loading and restoration**
*For any* set of saved conversations with one marked as active, restarting the application should load all conversations and restore the active conversation to the main chat view.
**Validates: Requirements 5.2, 5.5**

**Property 13: Conversation switching persistence**
*For any* two conversations A and B, when switching from A to B, conversation A should be persisted before conversation B is loaded.
**Validates: Requirements 5.3**

**Property 14: Provider settings persistence**
*For any* provider configuration change, the selected provider and API key should be persisted to SharedPreferences immediately.
**Validates: Requirements 5.4**

**Property 15: Conversation serialization round-trip**
*For any* valid conversation object, serializing to JSON and then deserializing should produce an equivalent conversation with the same id, title, timestamps, and messages.
**Validates: Requirements 5.6**

### Animation and Interaction Properties

**Property 16: Sidebar animation timing and easing**
*For any* sidebar open or close action, the animation should use easing and complete within 300ms.
**Validates: Requirements 6.1**

**Property 17: Interactive element feedback**
*For any* interactive UI element, when tapped, visual feedback should be provided within 100ms.
**Validates: Requirements 6.2**

**Property 18: Sidebar overlay blocking**
*For any* UI element behind the sidebar overlay, when the sidebar is animating or open, that element should not respond to tap events.
**Validates: Requirements 6.4**

**Property 19: Animation performance**
*For any* animation sequence, the frame rate should maintain at least 60 frames per second.
**Validates: Requirements 6.5**

### Backward Compatibility Properties

**Property 20: Feature preservation**
*For any* existing feature (voice input, TTS, message processing, Android integrations), after UI updates, the feature should continue to function with the same behavior as before the update.
**Validates: Requirements 7.1, 7.2, 7.3, 7.4**

### Security Properties

**Property 21: API key masking and toggle**
*For any* API key input field, the key should be masked by default, and toggling the show/hide button should alternate between masked and visible states.
**Validates: Requirements 8.2, 8.3**

## Error Handling

### Conversation Storage Errors

**File I/O Failures:**
- If conversation save fails due to storage unavailability, queue the save operation for retry
- Display a non-intrusive notification to the user about sync status
- Maintain in-memory conversation state until successful save
- Log error details for debugging

**Deserialization Errors:**
- If conversation JSON is corrupted, skip that conversation and continue loading others
- Log the corrupted conversation ID for investigation
- Display a warning to user about unavailable conversations
- Provide option to clear corrupted data

**Storage Quota Exceeded:**
- Implement conversation limit (e.g., 100 conversations maximum)
- When limit reached, prompt user to delete old conversations
- Provide automatic cleanup option for conversations older than 30 days
- Display storage usage in settings

### API Configuration Errors

**Invalid API Key:**
- Validate API key format before saving
- Display clear error message for invalid format
- Prevent saving invalid configuration
- Provide link to provider documentation for key format

**Provider Connection Failures:**
- When first message fails, display error with provider name
- Offer to open settings to check API key
- Suggest switching to alternative provider
- Cache last successful provider as fallback

**Missing Configuration:**
- On app first launch, detect missing API configuration
- Display welcome screen with setup instructions
- Guide user through provider selection and API key entry
- Validate configuration before allowing chat

### UI State Errors

**Sidebar Animation Interruption:**
- If sidebar animation is interrupted (e.g., rapid taps), cancel current animation
- Start new animation from current position
- Prevent animation queue buildup
- Ensure final state matches user intent

**Conversation Loading Failure:**
- If conversation fails to load, display error in chat view
- Offer to retry loading
- Provide option to create new conversation
- Log error for debugging

**Memory Pressure:**
- Limit number of messages loaded per conversation (e.g., last 100)
- Implement pagination for older messages
- Clear message cache when switching conversations
- Monitor memory usage and trim caches proactively

## Testing Strategy

### Dual Testing Approach

This feature requires both unit tests and property-based tests for comprehensive coverage:

**Unit Tests** focus on:
- Specific UI examples (settings screen has correct buttons, provider dropdown has 3 options)
- Edge cases (empty conversation list, corrupted JSON, missing API key)
- Error conditions (storage failure, invalid input, network errors)
- Integration points (Fragment lifecycle, SharedPreferences, file I/O)

**Property-Based Tests** focus on:
- Universal properties across all inputs (any conversation can be saved/loaded, any setting persists)
- Round-trip properties (serialize/deserialize, save/load configuration)
- Invariants (conversation count after operations, message order preservation)
- Comprehensive input coverage through randomization

### Property-Based Testing Configuration

**Framework:** Use Kotest Property Testing for Kotlin
- Minimum 100 iterations per property test
- Each test tagged with: `Feature: vira-complete-ui-match, Property N: [property text]`
- Each correctness property implemented as a single property-based test

**Example Test Structure:**
```kotlin
@Test
fun `Property 15 - Conversation serialization round-trip`() = runTest {
    // Feature: vira-complete-ui-match, Property 15: Conversation serialization round-trip
    checkAll(100, Arb.conversation()) { conversation ->
        val json = storageService.serializeConversation(conversation)
        val deserialized = storageService.deserializeConversation(json)
        
        deserialized.id shouldBe conversation.id
        deserialized.title shouldBe conversation.title
        deserialized.messages shouldBe conversation.messages
    }
}
```

### Test Coverage Requirements

**Unit Test Coverage:**
- All UI components (Fragments, Activities, Adapters)
- All data models (Conversation, ProviderConfiguration)
- All service classes (ConversationStorageService, ConversationManager, ProviderConfigManager)
- Error handling paths
- Edge cases (empty states, boundary conditions)

**Property Test Coverage:**
- All 21 correctness properties from design document
- Each property maps to specific requirements
- Focus on data integrity, persistence, and behavior consistency

**Integration Test Coverage:**
- End-to-end conversation flow (create, save, load, switch)
- Provider configuration flow (select, configure, save, use)
- Sidebar interaction flow (open, select conversation, close)
- Settings persistence flow (change setting, restart, verify)

### Testing Tools

- **JUnit 5**: Unit test framework
- **Kotest**: Property-based testing framework
- **MockK**: Mocking framework for Kotlin
- **Espresso**: UI testing framework for Android
- **Robolectric**: Android unit testing without emulator

### Manual Testing Checklist

While automated tests cover functional correctness, manual testing is required for:
- Visual design match with reference UI (spacing, colors, typography)
- Animation smoothness and feel
- Touch interaction responsiveness
- Accessibility (screen reader, touch target sizes)
- Different screen sizes and orientations
- Theme switching visual correctness

## Implementation Notes

### Android Native UI Components

All UI will be implemented using Android native components:
- **Fragments**: ChatHistorySidebarFragment, MainChatFragment
- **RecyclerView**: Conversation list, message list
- **Material Components**: Cards, buttons, switches, text fields
- **ConstraintLayout**: Flexible layouts matching reference design
- **DrawerLayout**: NOT used (custom sidebar implementation for more control)
- **Animations**: ObjectAnimator, ValueAnimator for smooth transitions

### Performance Considerations

**Conversation Loading:**
- Load conversations asynchronously on background thread
- Use coroutines for non-blocking I/O
- Cache conversation metadata (title, timestamp) separately from full messages
- Implement lazy loading for message history

**UI Rendering:**
- Use RecyclerView with ViewHolder pattern for efficient list rendering
- Implement DiffUtil for efficient list updates
- Avoid nested RecyclerViews
- Use ConstraintLayout to flatten view hierarchy

**Animation Performance:**
- Use hardware acceleration for animations
- Avoid animating complex layouts
- Use translation/alpha animations (GPU-accelerated)
- Profile animations with GPU rendering tools

### Security Best Practices

**API Key Storage:**
- Store in SharedPreferences with MODE_PRIVATE
- Consider using EncryptedSharedPreferences for additional security
- Never log API keys
- Clear API keys from memory after use

**UI Security:**
- Set FLAG_SECURE on API key input fields to prevent screenshots
- Mask API keys by default with password input type
- Clear clipboard after API key paste
- Implement auto-lock for sensitive screens

### Migration Strategy

**Existing Users:**
- Detect first launch of new version
- Migrate existing settings to new format if needed
- Create default conversation from existing message history
- Preserve all existing preferences

**New Users:**
- Display onboarding flow for provider setup
- Provide sample conversation or tutorial
- Guide through settings configuration
- Offer quick setup with recommended defaults

## Dependencies

### Existing Dependencies (No Changes)
- Kotlin Coroutines
- AndroidX Lifecycle
- AndroidX RecyclerView
- Material Components
- Gson (for JSON serialization)

### New Dependencies (If Needed)
- Kotest Property Testing: `io.kotest:kotest-property:5.8.0`
- EncryptedSharedPreferences: `androidx.security:security-crypto:1.1.0-alpha06`

### No XAML Dependencies
- All existing XAML code will be replaced with Android native Views
- Remove any Xamarin.Forms dependencies
- Use pure Kotlin/Java for all UI code
