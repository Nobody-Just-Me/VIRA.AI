# Implementation Plan: VIRA Complete UI Match

## Overview

This implementation plan breaks down the VIRA Complete UI Match feature into discrete coding tasks. The approach follows an incremental strategy: data layer first, then core services, then UI components, and finally integration. Each task builds on previous work, with testing integrated throughout to catch errors early.

## Tasks

- [x] 1. Set up data models and storage infrastructure
  - [x] 1.1 Create Conversation data model
    - Implement Conversation data class with id, title, timestamps, and messages list
    - Add helper methods: getDisplayTitle(), getFormattedTimestamp(), getMessageCount()
    - _Requirements: 1.2, 5.1, 5.2_
  
  - [x] 1.2 Create ProviderConfiguration data model
    - Implement ProviderConfiguration data class with provider, model, apiKey fields
    - Add companion object with SharedPreferences key constants
    - Add getApiKeyPrefKey() helper method
    - _Requirements: 3.1, 3.7, 3.8_
  
  - [x] 1.3 Create ThemePreference enum
    - Implement ThemePreference enum with LIGHT, DARK, SYSTEM values
    - Add companion object with preference key constant
    - _Requirements: 2.3_
  
  - [ ]* 1.4 Write property test for Conversation serialization
    - **Property 15: Conversation serialization round-trip**
    - **Validates: Requirements 5.6**

- [x] 2. Implement ConversationStorageService
  - [x] 2.1 Create ConversationStorageService class
    - Implement file-based storage using internal storage directory
    - Add Gson instance for JSON serialization
    - Create conversations directory on initialization
    - _Requirements: 5.1, 5.6_
  
  - [x] 2.2 Implement save and load methods
    - Implement saveConversation() with file I/O and error handling
    - Implement loadConversation() with JSON deserialization
    - Implement loadAllConversations() to scan directory
    - _Requirements: 5.1, 5.2_
  
  - [x] 2.3 Implement delete methods
    - Implement deleteConversation() to remove single conversation file
    - Implement deleteAllConversations() to clear directory
    - _Requirements: 1.5_
  
  - [ ]* 2.4 Write unit tests for storage error handling
    - Test corrupted JSON handling
    - Test storage unavailable scenarios
    - Test file I/O exceptions
    - _Requirements: 5.1, 5.6_
  
  - [ ]* 2.5 Write property test for conversation persistence
    - **Property 11: Message persistence**
    - **Validates: Requirements 5.1**

- [x] 3. Implement ConversationManager
  - [x] 3.1 Create ConversationManager class
    - Implement constructor with ConversationStorageService dependency
    - Add currentConversation and conversations list properties
    - _Requirements: 1.3, 5.3_
  
  - [x] 3.2 Implement conversation lifecycle methods
    - Implement loadAllConversations() to load from storage
    - Implement createNewConversation() with unique ID generation
    - Implement switchToConversation() with save-before-switch logic
    - Implement getCurrentConversation() getter
    - _Requirements: 1.3, 1.4, 5.3_
  
  - [x] 3.3 Implement message and deletion methods
    - Implement addMessageToCurrentConversation() with auto-save
    - Implement saveCurrentConversation() wrapper
    - Implement deleteAllConversations() with storage coordination
    - _Requirements: 1.5, 5.1_
  
  - [ ]* 3.4 Write property test for conversation switching
    - **Property 13: Conversation switching persistence**
    - **Validates: Requirements 5.3**
  
  - [ ]* 3.5 Write property test for conversation loading
    - **Property 12: Conversation loading and restoration**
    - **Validates: Requirements 5.2, 5.5**

- [x] 4. Implement ProviderConfigManager
  - [x] 4.1 Create ProviderConfigManager class
    - Implement constructor with PreferencesService dependency
    - Define AIProvider enum (GEMINI, GROQ, OPENAI)
    - _Requirements: 3.1, 3.7_
  
  - [x] 4.2 Implement configuration persistence methods
    - Implement saveProviderConfig() to store in SharedPreferences
    - Implement loadProviderConfig() to retrieve from SharedPreferences
    - Store provider, model, and API key separately
    - _Requirements: 3.7, 3.8, 5.4_
  
  - [x] 4.3 Implement model listing and validation
    - Implement getAvailableModels() with hardcoded model lists per provider
    - Implement validateApiKey() for basic format validation
    - _Requirements: 3.2, 3.3, 3.4_
  
  - [ ]* 4.4 Write property test for provider configuration round-trip
    - **Property 10: Provider configuration round-trip**
    - **Validates: Requirements 3.7, 3.8**
  
  - [ ]* 4.5 Write property test for provider settings persistence
    - **Property 14: Provider settings persistence**
    - **Validates: Requirements 5.4**

- [x] 5. Checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.

- [x] 6. Create ChatHistorySidebarFragment UI
  - [x] 6.1 Create fragment layout XML
    - Design sidebar layout with RecyclerView for conversation list
    - Add "New Chat" and "Clear History" buttons at top
    - Add empty state view for no conversations
    - Apply Material Design styling matching reference
    - _Requirements: 1.2, 1.4, 1.5_
  
  - [x] 6.2 Create conversation list item layout
    - Design list item with title, timestamp, and message count
    - Add ripple effect for tap feedback
    - Apply spacing and typography from reference design
    - _Requirements: 1.2_
  
  - [x] 6.3 Implement ChatHistorySidebarFragment class
    - Create Fragment with ConversationManager dependency
    - Implement show() and hide() methods with slide animations
    - Set up RecyclerView with adapter
    - Implement SidebarListener interface for callbacks
    - _Requirements: 1.1, 1.6_
  
  - [x] 6.4 Implement ConversationListAdapter
    - Create RecyclerView adapter for conversation list
    - Implement ViewHolder with click listeners
    - Use DiffUtil for efficient updates
    - _Requirements: 1.2, 1.3_
  
  - [x] 6.5 Implement sidebar animations
    - Create slide-in animation from left edge (300ms with easing)
    - Create slide-out animation to left edge (300ms with easing)
    - Use ObjectAnimator for smooth transitions
    - _Requirements: 1.1, 1.6, 1.7, 6.1_
  
  - [ ]* 6.6 Write property test for sidebar visibility toggle
    - **Property 1: Sidebar visibility toggle**
    - **Validates: Requirements 1.1**
  
  - [ ]* 6.7 Write property test for conversation list completeness
    - **Property 2: Conversation list completeness**
    - **Validates: Requirements 1.2**
  
  - [ ]* 6.8 Write property test for animation timing
    - **Property 7: Animation timing**
    - **Validates: Requirements 1.7**

- [x] 7. Update SettingsActivity UI
  - [x] 7.1 Create new settings layout XML
    - Design profile header section with avatar, stats, version
    - Create card sections for different setting groups
    - Add theme selection with three visual buttons
    - Add toggle switches for all boolean preferences
    - Add dropdowns for Language and Response Style
    - _Requirements: 2.1, 2.3, 2.4, 2.5_
  
  - [x] 7.2 Create provider configuration section layout
    - Add provider dropdown (Gemini, Groq, OpenAI)
    - Add model dropdown (dynamic based on provider)
    - Add API key input field with show/hide toggle
    - Add save button with success feedback
    - _Requirements: 3.1, 3.5, 3.6, 3.7_
  
  - [x] 7.3 Update SettingsActivity class
    - Inject ProviderConfigManager and PreferencesService
    - Initialize all UI components
    - Set up profile header with app version
    - _Requirements: 2.1, 3.7_
  
  - [x] 7.4 Implement theme selection logic
    - Set up click listeners for theme buttons
    - Highlight selected theme button
    - Save theme preference on selection
    - Apply theme change immediately
    - _Requirements: 2.3, 2.6_
  
  - [x] 7.5 Implement toggle preferences
    - Set up listeners for all toggle switches
    - Save each preference change immediately to SharedPreferences
    - _Requirements: 2.4, 2.6_
  
  - [x] 7.6 Implement provider configuration logic
    - Set up provider dropdown listener to update model list
    - Set up model dropdown with dynamic options
    - Implement API key show/hide toggle
    - Implement save button with validation and feedback
    - Load saved configuration on activity start
    - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5, 3.6, 3.7, 3.8_
  
  - [ ]* 7.7 Write unit tests for settings UI examples
    - Test profile header displays correctly
    - Test three theme buttons exist
    - Test all toggle switches exist
    - Test provider dropdown has three options
    - Test model options update based on provider
    - _Requirements: 2.1, 2.3, 2.4, 3.1, 3.2, 3.3, 3.4_
  
  - [ ]* 7.8 Write property test for settings persistence
    - **Property 8: Settings persistence**
    - **Validates: Requirements 2.6**
  
  - [ ]* 7.9 Write property test for provider selection UI update
    - **Property 9: Provider selection UI update**
    - **Validates: Requirements 3.5**

- [x] 8. Update MainChatFragment UI
  - [x] 8.1 Update main chat layout XML
    - Update header with centered greeting text
    - Add hamburger menu button on left
    - Keep settings button on right
    - Update quick actions styling to match reference
    - Update message bubble styling to match reference
    - _Requirements: 4.1, 4.2_
  
  - [x] 8.2 Update MainChatFragment class
    - Add ConversationManager dependency
    - Add reference to ChatHistorySidebarFragment
    - Update header setup with new layout
    - _Requirements: 4.1, 4.2_
  
  - [x] 8.3 Implement sidebar integration
    - Add hamburger button click listener to toggle sidebar
    - Implement outside-tap detection to close sidebar
    - Add overlay view that blocks interaction when sidebar open
    - _Requirements: 1.1, 1.6, 6.4_
  
  - [x] 8.4 Implement conversation loading
    - Implement loadConversation() method
    - Clear current messages and load selected conversation
    - Update UI to show conversation messages
    - _Requirements: 1.3, 5.5_
  
  - [x] 8.5 Update message handling to use ConversationManager
    - Modify message sending to add to current conversation
    - Ensure messages are auto-saved via ConversationManager
    - _Requirements: 5.1_
  
  - [ ]* 8.6 Write property test for conversation loading
    - **Property 3: Conversation loading**
    - **Validates: Requirements 1.3**
  
  - [ ]* 8.7 Write property test for new chat creation
    - **Property 4: New chat creation**
    - **Validates: Requirements 1.4**
  
  - [ ]* 8.8 Write property test for outside tap closes sidebar
    - **Property 6: Outside tap closes sidebar**
    - **Validates: Requirements 1.6**
  
  - [ ]* 8.9 Write property test for sidebar overlay blocking
    - **Property 18: Sidebar overlay blocking**
    - **Validates: Requirements 6.4**

- [x] 9. Checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.

- [x] 10. Implement remaining interaction properties
  - [x] 10.1 Add interactive element feedback
    - Add ripple effects to all buttons and tappable elements
    - Ensure feedback appears within 100ms of tap
    - _Requirements: 6.2_
  
  - [x] 10.2 Implement history clearing with confirmation
    - Add confirmation dialog for "Clear History" button
    - Implement deletion logic via ConversationManager
    - Update UI after deletion
    - _Requirements: 1.5_
  
  - [ ]* 10.3 Write property test for history clearing
    - **Property 5: History clearing**
    - **Validates: Requirements 1.5**
  
  - [ ]* 10.4 Write property test for interactive element feedback
    - **Property 17: Interactive element feedback**
    - **Validates: Requirements 6.2**

- [x] 11. Implement security features
  - [x] 11.1 Add API key masking
    - Set API key input to password type by default
    - Implement show/hide toggle to switch input type
    - _Requirements: 8.2, 8.3_
  
  - [x] 11.2 Add screenshot protection
    - Set FLAG_SECURE on SettingsActivity window
    - Prevent API keys from appearing in screenshots
    - _Requirements: 8.4_
  
  - [x] 11.3 Implement secure storage
    - Use SharedPreferences MODE_PRIVATE for API keys
    - Consider EncryptedSharedPreferences if available
    - _Requirements: 8.1_
  
  - [ ]* 11.4 Write unit tests for security features
    - Test API key field is masked by default
    - Test FLAG_SECURE is set on settings activity
    - Test SharedPreferences uses MODE_PRIVATE
    - _Requirements: 8.1, 8.2, 8.4_
  
  - [ ]* 11.5 Write property test for API key masking toggle
    - **Property 21: API key masking and toggle**
    - **Validates: Requirements 8.2, 8.3**

- [x] 12. Implement backward compatibility verification
  - [x] 12.1 Verify voice input functionality
    - Test that voice input still works with new UI
    - Ensure voice button is accessible
    - Verify voice processing pipeline unchanged
    - _Requirements: 7.1_
  
  - [x] 12.2 Verify TTS functionality
    - Test that TTS still works with new UI
    - Ensure TTS settings are preserved
    - Verify TTS playback unchanged
    - _Requirements: 7.2_
  
  - [x] 12.3 Verify message processing
    - Test that all message types are processed correctly
    - Verify pattern matching still works
    - Ensure AI provider integration unchanged
    - _Requirements: 7.3_
  
  - [x] 12.4 Verify Android integrations
    - Test permissions still work
    - Verify intents and deep links
    - Ensure notifications unchanged
    - _Requirements: 7.4_
  
  - [ ]* 12.5 Write property test for feature preservation
    - **Property 20: Feature preservation**
    - **Validates: Requirements 7.1, 7.2, 7.3, 7.4**

- [x] 13. Implement migration for existing users
  - [x] 13.1 Create migration utility
    - Detect first launch of new version
    - Create default conversation from existing messages if any
    - Migrate existing settings to new format
    - _Requirements: 5.2_
  
  - [x] 13.2 Add onboarding for new users
    - Display provider setup screen on first launch
    - Guide user through API key configuration
    - Create welcome conversation
    - _Requirements: 3.7, 3.8_

- [x] 14. Performance optimization and animation tuning
  - [x] 14.1 Optimize conversation loading
    - Implement async loading with coroutines
    - Add loading indicators for long operations
    - Cache conversation metadata separately
    - _Requirements: 5.2_
  
  - [x] 14.2 Optimize RecyclerView performance
    - Implement DiffUtil for efficient updates
    - Use ViewHolder pattern correctly
    - Avoid nested RecyclerViews
    - _Requirements: 1.2_
  
  - [x] 14.3 Profile and optimize animations
    - Use GPU profiling tools to verify 60fps
    - Optimize animation complexity if needed
    - Use hardware acceleration
    - _Requirements: 6.5_
  
  - [ ]* 14.4 Write property test for animation performance
    - **Property 19: Animation performance**
    - **Validates: Requirements 6.5**
  
  - [ ]* 14.5 Write property test for sidebar animation timing and easing
    - **Property 16: Sidebar animation timing and easing**
    - **Validates: Requirements 6.1**

- [x] 15. Final integration and testing
  - [x] 15.1 Integration test: Complete conversation flow
    - Test create conversation → add messages → save → load → switch
    - Verify data persistence across app restarts
    - _Requirements: 1.3, 1.4, 5.1, 5.2, 5.3_
  
  - [x] 15.2 Integration test: Provider configuration flow
    - Test select provider → enter API key → save → restart → verify loaded
    - Test switching between providers
    - _Requirements: 3.1, 3.7, 3.8_
  
  - [x] 15.3 Integration test: Sidebar interaction flow
    - Test open sidebar → select conversation → close sidebar → verify loaded
    - Test new chat → clear history flows
    - _Requirements: 1.1, 1.3, 1.4, 1.5, 1.6_
  
  - [ ]* 15.4 Run all property-based tests
    - Execute all 21 property tests with 100 iterations each
    - Verify all properties pass
    - Fix any failures discovered

- [x] 16. Final checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.

## Notes

- Tasks marked with `*` are optional and can be skipped for faster MVP
- Each task references specific requirements for traceability
- Checkpoints ensure incremental validation
- Property tests validate universal correctness properties
- Unit tests validate specific examples and edge cases
- The implementation uses Android native UI (Kotlin) with no XAML dependencies
- All existing VIRA features (voice, TTS, message processing) must continue working
