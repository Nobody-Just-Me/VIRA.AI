# Implementation Plan: VIRA Modern UI Redesign

## Overview

This implementation plan breaks down the VIRA Modern UI Redesign into discrete coding tasks. The approach follows an incremental strategy: establish the foundation (theme system, providers), build core UI components (chat interface, message bubbles), add rich features (voice mode, content cards), and finally integrate everything with animations and polish.

Each task builds on previous work, ensuring no orphaned code. Testing tasks are marked as optional (*) to allow for faster MVP delivery while maintaining the option for comprehensive testing.

## Tasks

- [x] 1. Set up theme system and color resources
  - Create ThemeService.cs with all color constants from reference design (#101622, #8b5cf6, etc.)
  - Create XAML ResourceDictionary with colors, brushes, gradients, shadows, and border radius values
  - Add ambient glow gradient brushes for background effects
  - Test theme resources load correctly in app
  - _Requirements: 11.1, 11.2, 11.3, 11.4, 11.5, 11.6_

- [x]* 1.1 Write property test for theme color values
  - **Property 1**: For any theme color constant, the value should match the reference design specification
  - **Validates: Requirements 11.1**

- [x] 2. Implement multi-provider AI system
  - [x] 2.1 Create IProviderService interface with common methods (SendMessageAsync, ValidateConnectionAsync, GetAvailableModels)
    - _Requirements: 9.1, 9.2, 9.3_
  
  - [x] 2.2 Implement GroqProvider with Groq API integration
    - Add API key configuration
    - Implement message sending with Groq API format
    - Add model selection (mixtral-8x7b-32768)
    - _Requirements: 9.1_
  
  - [x] 2.3 Implement OpenAIProvider with streaming support
    - Add API key configuration
    - Implement SendMessageStreamAsync for streaming responses
    - Add model selection (gpt-4, gpt-3.5-turbo)
    - _Requirements: 9.2, 15.1_
  
  - [x] 2.4 Implement GeminiProvider with multimodal support
    - Add API key configuration
    - Implement message sending with Gemini API format
    - Add model selection (gemini-2.0-flash, gemini-pro)
    - _Requirements: 9.3, 15.2_
  
  - [x] 2.5 Create AIProviderManager for orchestration
    - Implement provider registration and selection
    - Add active provider routing logic
    - Implement fallback mechanism for provider failures
    - Add API key validation before saving
    - _Requirements: 9.4, 9.5, 9.6, 9.7_

- [x]* 2.6 Write property test for provider routing
  - **Property 25**: For any AI request when a provider is selected, the request should be routed to that specific provider's API
  - **Validates: Requirements 9.4**

- [x]* 2.7 Write property test for provider fallback
  - **Property 26**: For any provider request failure, the system should attempt to route the request to the configured fallback provider
  - **Validates: Requirements 9.5**

- [x]* 2.8 Write property test for API key isolation
  - **Property 27**: For any provider, its API key should be stored separately and not accessible to other providers
  - **Validates: Requirements 9.6**

- [x]* 2.9 Write property test for API key validation
  - **Property 28**: For any API key input, validation should occur before the key is saved to storage
  - **Validates: Requirements 9.7**

- [x] 3. Checkpoint - Ensure provider system works
  - Ensure all tests pass, ask the user if questions arise.


- [x] 4. Create animation system
  - [x] 4.1 Implement AnimationService with core animation methods
    - Add FadeInAsync method (280ms, cubic ease-out)
    - Add SlideInFromBottomAsync method (12px distance, 280ms)
    - Add PulseAsync method (500ms, sine ease, infinite repeat)
    - Add ScaleAnimation for button taps (200ms)
    - _Requirements: 12.3, 12.6_
  
  - [x] 4.2 Create reusable animation behaviors for XAML
    - Create FadeInBehavior for message entrance
    - Create SlideInBehavior for sidebar
    - Create PulseBehavior for voice button
    - _Requirements: 1.4, 12.3_

- [x]* 4.3 Write property test for message entrance animation
  - **Property 4**: For any new message added to the chat, an entrance animation should be triggered with fade-in and slide-up effects
  - **Validates: Requirements 1.4**

- [x]* 4.4 Write property test for screen transitions
  - **Property 34**: For any navigation between screens, a smooth fade or slide transition animation should be applied
  - **Validates: Requirements 12.3**

- [x] 5. Build message bubble component
  - [x] 5.1 Create Message model with all properties (Id, Role, Text, Type, Timestamp, rich content)
    - _Requirements: 1.1, 1.2, 1.3, 1.5_
  
  - [x] 5.2 Create MessageBubble UserControl with XAML layout
    - Implement user message styling (purple gradient #8b5cf6, shadow)
    - Implement AI message styling (semi-transparent white, backdrop blur)
    - Add rounded corners (16px with asymmetric corners)
    - Display timestamp below message
    - _Requirements: 1.2, 1.3, 1.5, 1.6_
  
  - [x] 5.3 Add message entrance animation to MessageBubble
    - Apply fade-in and slide-up animation when message appears
    - Use 280ms duration with cubic ease-out
    - _Requirements: 1.4_

- [x]* 5.4 Write property test for message bubble styling
  - **Property 1**: For any message in the chat, user messages should have purple gradient background with shadow, and AI messages should have semi-transparent white background with backdrop blur
  - **Validates: Requirements 1.2, 1.3**

- [x]* 5.5 Write property test for timestamp display
  - **Property 2**: For any message displayed in the chat, a timestamp should be visible below the message bubble
  - **Validates: Requirements 1.5**

- [x]* 5.6 Write property test for corner radius
  - **Property 3**: For any message bubble, the corner radius should match the design specification
  - **Validates: Requirements 1.6**

- [x] 6. Implement rich content cards
  - [x] 6.1 Create WeatherCard component
    - Create WeatherData model (City, Temp, Condition, Humidity, UV, Tomorrow)
    - Design XAML layout matching reference design
    - Display all weather fields with proper styling
    - _Requirements: 5.1_
  
  - [x] 6.2 Create NewsCard component
    - Create NewsItem model (Category, Title)
    - Design XAML layout with category badges
    - Display headlines with proper styling
    - _Requirements: 5.2_
  
  - [x] 6.3 Create ScheduleCard component
    - Create ScheduleItem model (Time, Title, Location, Color)
    - Design XAML layout with time-based list
    - Add colored indicators for each item
    - _Requirements: 5.3_
  
  - [x] 6.4 Create TrafficCard component
    - Create TrafficRoute model (Route, ETA, Status, Color)
    - Design XAML layout with route information
    - Add color-coded status indicators
    - _Requirements: 5.4_

- [x]* 6.5 Write property test for content card completeness
  - **Property 11**: For any rich content data (weather, news, schedule, traffic), the rendered card should display all required fields
  - **Validates: Requirements 5.1, 5.2, 5.3, 5.4**

- [x] 7. Build main chat interface
  - [x] 7.1 Create MainChatView with XAML layout
    - Add gradient background (#101622) with ambient effects
    - Create header with greeting, status indicator, menu and settings buttons
    - Add ScrollViewer for message list
    - Add floating input area at bottom
    - _Requirements: 1.1, 2.4, 2.5, 2.6, 2.7, 4.1, 4.2, 4.3, 4.5, 4.8_
  
  - [x] 7.2 Implement time-based greeting logic
    - Add GetGreeting() method that returns appropriate greeting based on time
    - "Good Morning" for 5:00 AM - 11:59 AM
    - "Good Afternoon" for 12:00 PM - 4:59 PM
    - "Good Evening" for 5:00 PM - 4:59 AM
    - _Requirements: 2.1, 2.2, 2.3_
  
  - [x] 7.3 Create QuickActionBar component
    - Create QuickAction model (Icon, Label, Color, Query)
    - Create QuickActionService with default actions (Weather, News, Reminders, Traffic, Coffee, Music)
    - Design horizontal scrollable layout
    - Apply semi-transparent background and borders
    - Add colored icons for each action
    - _Requirements: 3.1, 3.2, 3.4, 3.5_
  
  - [x] 7.4 Implement FloatingInputArea component
    - Add gradient background (dark to transparent)
    - Apply 32px border radius
    - Add new chat button (+) on left
    - Add text input in center
    - Add voice button (🎤) with purple background
    - Add send button (➤) with purple gradient
    - Add bottom handle indicator
    - _Requirements: 4.1, 4.2, 4.3, 4.4, 4.5, 4.8_
  
  - [x] 7.5 Wire up message sending functionality
    - Connect send button to AIProviderManager
    - Clear input after sending
    - Add message to chat list
    - Trigger AI response
    - _Requirements: 4.6_

- [x]* 7.6 Write property test for time-based greetings
  - **Property 5**: For any time of day, the greeting text should correctly display the appropriate greeting
  - **Validates: Requirements 2.1, 2.2, 2.3**

- [x]* 7.7 Write property test for header button navigation
  - **Property 6**: For any header button (menu or settings), tapping it should navigate to or open the corresponding interface
  - **Validates: Requirements 2.5, 2.6**

- [x]* 7.8 Write property test for quick action triggering
  - **Property 7**: For any quick action button, tapping it should trigger the corresponding action handler with the correct query
  - **Validates: Requirements 3.3**

- [x]* 7.9 Write property test for send button behavior
  - **Property 9**: For any non-empty text input, tapping the send button should submit the message and clear the input field
  - **Validates: Requirements 4.6**

- [x] 8. Checkpoint - Ensure main chat interface works
  - Ensure all tests pass, ask the user if questions arise.


- [ ] 9. Implement voice mode with waveform visualizer
  - [x] 9.1 Create WaveformVisualizer component
    - Initialize 40 Rectangle bars in horizontal layout
    - Implement bell curve height distribution
    - Add animation timer (50ms interval)
    - Implement bar height animation based on audio levels
    - Add state-based coloring (purple for idle, red for recording, purple for speaking)
    - _Requirements: 6.2, 6.9_
  
  - [x] 9.2 Create VoiceModeView with full-screen layout
    - Add ambient background effects
    - Create header with "VOICE MODE" title and "Live" indicator
    - Add transcript display area
    - Add WaveformVisualizer in center
    - Add large circular voice button (80px diameter)
    - Add voice controls (message, voice, volume buttons)
    - _Requirements: 6.1, 6.3_
  
  - [x] 9.3 Implement voice button state management
    - Add IsListening and IsSpeaking properties
    - Apply pulsing animation when listening
    - Change color to red when recording
    - Change color to purple when idle
    - _Requirements: 6.4, 6.5, 6.6_
  
  - [x] 9.4 Wire up speech recognition
    - Integrate with platform speech recognition API
    - Update transcript as speech is recognized
    - Display speaking indicator when AI responds
    - _Requirements: 6.7, 6.8_
  
  - [x] 9.5 Connect voice mode to main chat
    - Add navigation from FloatingInputArea voice button to VoiceModeView
    - Pass voice transcript back to main chat when complete
    - _Requirements: 4.7, 6.1_

- [x]* 9.6 Write property test for voice mode activation
  - **Property 10**: For any voice button tap, the system should navigate to and display the full-screen voice mode interface
  - **Validates: Requirements 4.7, 6.1**

- [x]* 9.7 Write property test for voice button state coloring
  - **Property 12**: For any voice mode state (idle, listening, recording), the voice button color should match the state
  - **Validates: Requirements 6.5, 6.6, 6.4**

- [x]* 9.8 Write property test for transcript updates
  - **Property 13**: For any recognized speech input, the transcript text should be displayed and updated in real-time
  - **Validates: Requirements 6.7**

- [x]* 9.9 Write property test for speaking indicator
  - **Property 14**: For any AI speech output, a speaking indicator with purple accent should be visible during the speech
  - **Validates: Requirements 6.8**

- [x]* 9.10 Write property test for waveform responsiveness
  - **Property 15**: For any audio input level change, the waveform visualizer bars should animate to reflect the audio amplitude
  - **Validates: Requirements 6.9**

- [ ] 10. Create chat history sidebar
  - [x] 10.1 Create ChatHistorySidebar component
    - Design XAML layout (280px width, left-aligned)
    - Add semi-transparent overlay
    - Add header with "Chat History" title
    - Add "New Chat" button at top
    - Add scrollable list of past conversations
    - Add "Clear History" button at bottom
    - _Requirements: 7.1, 7.2, 7.3, 7.4, 7.7_
  
  - [x] 10.2 Implement slide-in/out animations
    - Add ShowAsync method with slide-in from left (300ms)
    - Add HideAsync method with slide-out to left (300ms)
    - Use cubic easing functions
    - _Requirements: 7.1_
  
  - [x] 10.3 Wire up sidebar functionality
    - Connect menu button to show sidebar
    - Connect overlay tap to hide sidebar
    - Implement conversation loading on selection
    - Implement new chat creation
    - Implement clear history with confirmation
    - _Requirements: 7.5, 7.6_

- [x]* 10.4 Write property test for sidebar animation
  - **Property 16**: For any menu button tap, the chat history sidebar should slide in from the left with smooth animation
  - **Validates: Requirements 7.1**

- [x]* 10.5 Write property test for chat history display
  - **Property 17**: For any saved conversation sessions, they should all be displayed in the chat history sidebar list
  - **Validates: Requirements 7.2**

- [x]* 10.6 Write property test for conversation loading
  - **Property 18**: For any conversation selected from history, that conversation's messages should be loaded and the sidebar should close
  - **Validates: Requirements 7.5**

- [x]* 10.7 Write property test for overlay dismissal
  - **Property 19**: For any tap on the sidebar overlay, the sidebar should close with slide-out animation
  - **Validates: Requirements 7.6**

- [ ] 11. Implement dialogs and notifications
  - [x] 11.1 Create PermissionRationaleDialog component
    - Design dialog layout with explanation text
    - Add "Grant Permission" and "Cancel" buttons
    - Apply styling matching reference design
    - _Requirements: 8.1_
  
  - [x] 11.2 Create ContactDisambiguationDialog component
    - Design dialog with list of matching contacts
    - Display contact name, phone, and label for each
    - Add selection handling
    - _Requirements: 8.2_
  
  - [x] 11.3 Create notification system (ErrorSnackbar)
    - Create notification component with auto-dismiss (3 seconds)
    - Add error styling (red accent)
    - Add success styling (green accent)
    - Add info styling (blue accent)
    - Position at top of screen
    - _Requirements: 8.3, 8.4, 8.5_

- [x]* 11.4 Write property test for permission dialog
  - **Property 20**: For any action requiring permissions, a permission rationale dialog should be displayed
  - **Validates: Requirements 8.1**

- [x]* 11.5 Write property test for contact disambiguation
  - **Property 21**: For any contact name with multiple matches, a contact selection dialog should be displayed
  - **Validates: Requirements 8.2**

- [x]* 11.6 Write property test for error notifications
  - **Property 22**: For any error that occurs, an error notification should be displayed with error styling
  - **Validates: Requirements 8.3**

- [x]* 11.7 Write property test for success notifications
  - **Property 23**: For any successful action completion, a success notification should be displayed with success styling
  - **Validates: Requirements 8.4**

- [x]* 11.8 Write property test for notification auto-dismissal
  - **Property 24**: For any notification displayed, it should automatically dismiss after exactly 3 seconds
  - **Validates: Requirements 8.5**

- [ ] 12. Checkpoint - Ensure dialogs and notifications work
  - Ensure all tests pass, ask the user if questions arise.


- [ ] 13. Build settings and provider configuration UI
  - [x] 13.1 Create SettingsView with XAML layout
    - Add scrollable main content area
    - Create Vira profile header with avatar, stats, and PRO badge
    - Add section headers for organization
    - _Requirements: 10.1_
  
  - [x] 13.2 Implement API Configuration section
    - Add provider selector dropdown (Groq, OpenAI, Gemini)
    - Add API key input field for each provider with show/hide toggle
    - Add model selector dropdown for each provider
    - Add "Save Configuration" button
    - Add "Test Connection" button with status indicator
    - _Requirements: 10.2, 10.3, 10.4, 10.5, 10.6_
  
  - [x] 13.3 Implement Preferences section
    - Add theme selector (Light, Dark, System)
    - Add toggle switches for: Voice Output, Web Browsing, Haptics, Notifications
    - Add language selector dropdown
    - Add response style selector
    - _Requirements: 10.1_
  
  - [x] 13.4 Implement AI Behaviour section
    - Add Memory Mode toggle
    - Add Privacy Mode toggle
    - _Requirements: 10.1_
  
  - [x] 13.5 Wire up settings functionality
    - Connect provider selector to AIProviderManager
    - Implement API key validation and saving
    - Implement connection testing
    - Display active provider in chat header
    - Persist all settings across app restarts
    - _Requirements: 10.7, 10.8_

- [x]* 13.6 Write property test for provider UI elements
  - **Property 29**: For any configured provider, the settings page should display an API key input field and model selection dropdown
  - **Validates: Requirements 10.3, 10.4**

- [x]* 13.7 Write property test for provider status indication
  - **Property 30**: For any provider, a status indicator should display its current connection state
  - **Validates: Requirements 10.5**

- [x]* 13.8 Write property test for connection testing
  - **Property 31**: For any "Test Connection" button tap, the system should verify the provider connection and update the status indicator
  - **Validates: Requirements 10.6**

- [x]* 13.9 Write property test for active provider display
  - **Property 32**: For any active provider selection, the provider name should be displayed in the chat header
  - **Validates: Requirements 10.7**

- [x]* 13.10 Write property test for configuration persistence
  - **Property 33**: For any provider configuration saved, restarting the app and loading the configuration should produce equivalent settings (round-trip)
  - **Validates: Requirements 10.8**

- [x]* 13.11 Write property test for provider-specific configuration
  - **Property 40**: For any provider with unique capabilities, those configuration options should be exposed in the settings interface
  - **Validates: Requirements 15.4, 15.5**

- [ ] 14. Implement responsive layout and accessibility
  - [x] 14.1 Add responsive layout logic
    - Implement screen width change handlers
    - Add proportional layout adjustments
    - Maintain aspect ratios across screen sizes
    - Implement keyboard appearance handling for input area
    - Test on small (5"), medium (6"), and large (7"+) screen sizes
    - _Requirements: 13.1, 13.2, 13.3_
  
  - [x] 14.2 Add accessibility support
    - Add AutomationProperties.Name to all interactive elements
    - Ensure minimum touch target size of 44x44 pixels for all buttons
    - Verify color contrast ratios meet WCAG AA standards (4.5:1)
    - Add keyboard navigation support
    - _Requirements: 14.1, 14.3, 14.4_

- [x]* 14.3 Write property test for responsive layout
  - **Property 35**: For any screen width change, the layout should adjust proportionally while maintaining aspect ratios
  - **Validates: Requirements 13.1, 13.2**

- [x]* 14.4 Write property test for keyboard adjustment
  - **Property 36**: For any keyboard appearance event, the input area should adjust its position to remain visible
  - **Validates: Requirements 13.3**

- [x]* 14.5 Write property test for accessibility text
  - **Property 37**: For any interactive UI element, accessibility text should be defined
  - **Validates: Requirements 14.1**

- [x]* 14.6 Write property test for touch target sizing
  - **Property 38**: For any interactive element, the minimum touch target size should be at least 44x44 pixels
  - **Validates: Requirements 14.3**

- [x]* 14.7 Write property test for text contrast
  - **Property 39**: For any text element, the color contrast ratio should meet WCAG AA standards
  - **Validates: Requirements 14.4**

- [ ] 15. Implement error handling and recovery
  - [x] 15.1 Create ProviderErrorHandler
    - Handle network errors with user-friendly messages
    - Handle invalid API key errors with navigation to settings
    - Handle timeout errors with retry option
    - Handle generic errors with fallback
    - _Requirements: 9.5_
  
  - [x] 15.2 Create ErrorRecoveryService
    - Implement automatic fallback provider switching
    - Implement retry with exponential backoff
    - Add cache clearing for data errors
    - _Requirements: 9.5_
  
  - [x] 15.3 Wire up error handling throughout app
    - Connect provider errors to notification system
    - Add error states to UI components
    - Implement graceful degradation for animations
    - _Requirements: 8.3_

- [x]* 15.4 Write unit tests for error handling
  - Test network error handling
  - Test API key error handling
  - Test timeout error handling
  - Test fallback provider switching

- [ ] 16. Final integration and polish
  - [x] 16.1 Integrate all components into main app
    - Wire MainChatView to navigation
    - Connect VoiceModeView to navigation
    - Connect SettingsView to navigation
    - Ensure all navigation flows work correctly
    - _Requirements: All_
  
  - [x] 16.2 Add loading states and transitions
    - Add loading indicators for AI responses
    - Add skeleton screens for content cards
    - Ensure smooth transitions between all screens
    - _Requirements: 12.3_
  
  - [x] 16.3 Optimize performance
    - Enable virtualization for message list
    - Optimize animation performance
    - Test scrolling performance with 100+ messages
    - Verify 60 FPS during animations
    - _Requirements: 12.1, 12.2_
  
  - [x] 16.4 Visual verification against reference design
    - Compare colors, spacing, border radius, shadows
    - Verify gradient directions and color stops
    - Test on multiple screen sizes
    - Ensure 100% visual parity
    - _Requirements: 11.1, 11.2, 11.3, 11.4, 11.5, 11.6_

- [x]* 16.5 Write integration tests
  - Test complete message sending flow
  - Test provider switching flow
  - Test voice mode flow
  - Test settings configuration flow
  - Test chat history flow

- [ ] 17. Final checkpoint - Ensure all features work end-to-end
  - Ensure all tests pass, ask the user if questions arise.

## Notes

- Tasks marked with `*` are optional and can be skipped for faster MVP delivery
- Each task references specific requirements for traceability
- Checkpoints ensure incremental validation at key milestones
- Property tests validate universal correctness properties across all inputs
- Unit tests validate specific examples, edge cases, and integration points
- The implementation follows an incremental approach: foundation → core UI → rich features → integration
- All colors, spacing, and animations match the reference design exactly
- Multi-provider support is built from the ground up with fallback mechanisms
- Accessibility and responsive design are integrated throughout, not added as afterthoughts
