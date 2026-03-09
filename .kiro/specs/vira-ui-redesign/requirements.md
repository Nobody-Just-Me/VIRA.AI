# Requirements Document: VIRA UI Redesign

## Introduction

This specification defines the requirements for redesigning VIRA's user interface to match the modern design from the "AI Assistant Mobile UI" reference implementation. The goal is to recreate the exact visual design, animations, and user experience using Uno Platform (C#/XAML) for Android, while maintaining VIRA's existing functionality and architecture.

The reference design features a dark theme with ambient gradients, smooth animations, modern typography, and an intuitive chat interface with voice capabilities. This redesign will transform VIRA from its current UI to a polished, production-ready assistant interface.

## Glossary

- **VIRA**: Voice Interactive Responsive Assistant - the AI assistant application
- **Uno_Platform**: Cross-platform UI framework using C#/XAML
- **Main_Chat_Screen**: Primary interface showing conversation history and input controls
- **Voice_Active_Screen**: Full-screen voice interaction mode with visual feedback
- **Message_Bubble**: Individual chat message container with role-specific styling
- **Quick_Action_Button**: Shortcut button for common queries (Weather, News, etc.)
- **Ambient_Background**: Layered gradient effects creating depth and atmosphere
- **Waveform_Visualizer**: Animated bars showing voice activity levels
- **Voice_Orb**: Circular animated element representing VIRA's voice state
- **Card_Component**: Specialized message content (Weather, News, Schedule, Traffic)
- **Chat_History_Sidebar**: Slide-in panel showing past conversations
- **Loading_Indicator**: Animated element showing AI processing state
- **Permission_Dialog**: Modal requesting user permissions (microphone, etc.)
- **Error_Snackbar**: Toast notification for errors and confirmations
- **Resource_Dictionary**: XAML file containing reusable styles and colors
- **Storyboard**: XAML animation definition
- **Gradient_Brush**: XAML brush with color gradients
- **Control_Template**: XAML template defining control appearance

## Requirements

### Requirement 1: Dark Theme Color System

**User Story:** As a user, I want a modern dark theme interface, so that the app is comfortable to use in low-light conditions and looks visually appealing.

#### Acceptance Criteria

1. THE Color_System SHALL define a primary background color of #101622 (dark blue-gray)
2. THE Color_System SHALL define a primary accent color of #8b5cf6 (purple)
3. THE Color_System SHALL define secondary accent colors including #25d1f4 (cyan), #a855f7 (light purple), and #c084fc (lighter purple)
4. THE Color_System SHALL define text colors: white (#ffffff) for primary text, #e2e8f0 (slate-100) for secondary text, and #94a3b8 (slate-400) for tertiary text
5. THE Color_System SHALL define semantic colors: #22c55e (green) for success, #ef4444 (red) for errors, #eab308 (yellow) for warnings
6. THE Color_System SHALL define opacity levels: 5% white for subtle backgrounds, 8% white for borders, 10% white for hover states
7. THE Color_System SHALL be implemented as XAML Resource Dictionary entries accessible throughout the application
8. THE Color_System SHALL support all UI components without hardcoded color values

### Requirement 2: Ambient Background Effects

**User Story:** As a user, I want subtle animated background effects, so that the interface feels alive and premium.

#### Acceptance Criteria

1. THE Main_Chat_Screen SHALL display three layered gradient circles as background elements
2. WHEN the Main_Chat_Screen is visible, THE first gradient circle SHALL be positioned at top-left with purple color (#8b5cf6) at 20% opacity with 50px blur
3. WHEN the Main_Chat_Screen is visible, THE second gradient circle SHALL be positioned at bottom-right with light purple color (#a855f7) at 10% opacity with 60px blur
4. WHEN the Main_Chat_Screen is visible, THE third gradient circle SHALL be positioned at center-left with lighter purple color (#c084fc) at 10% opacity with 40px blur
5. THE gradient circles SHALL remain static (no animation) to maintain performance
6. THE gradient circles SHALL be rendered behind all other UI elements using z-index/Canvas.ZIndex
7. THE Voice_Active_Screen SHALL display animated gradient circles that pulse with scale and opacity changes
8. THE ambient effects SHALL not interfere with text readability or touch interactions

### Requirement 3: Main Chat Screen Layout

**User Story:** As a user, I want a clean chat interface with clear sections, so that I can easily read messages and interact with the assistant.

#### Acceptance Criteria

1. THE Main_Chat_Screen SHALL display a header bar at the top with menu button, title, and settings button
2. THE Main_Chat_Screen SHALL display a scrollable message area in the center showing conversation history
3. THE Main_Chat_Screen SHALL display a floating input area at the bottom with gradient background
4. THE header bar SHALL have a semi-transparent background (#101622 at 60% opacity) with backdrop blur effect
5. THE header bar SHALL display "Good Morning/Afternoon/Evening" based on current time
6. THE header bar SHALL display "Vira AI" subtitle with a blue dot indicator (#6366f1)
7. THE message area SHALL have bottom padding of 200px to prevent content from being hidden by the floating input
8. THE floating input area SHALL have a gradient background from solid #101622 to transparent
9. THE floating input area SHALL remain fixed at the bottom during scrolling
10. THE Main_Chat_Screen SHALL auto-scroll to the latest message when new messages arrive

### Requirement 4: Message Bubble Styling

**User Story:** As a user, I want visually distinct message bubbles for my messages and AI responses, so that I can easily follow the conversation.

#### Acceptance Criteria

1. WHEN a message is from the user, THE Message_Bubble SHALL have a purple background (#8b5cf6) with white text
2. WHEN a message is from the user, THE Message_Bubble SHALL be aligned to the right side with rounded corners: top-left 16px, top-right 16px, bottom-left 16px, bottom-right 2px
3. WHEN a message is from the user, THE Message_Bubble SHALL have a purple shadow (0px 8px 24px -4px rgba(139,92,246,0.35))
4. WHEN a message is from AI, THE Message_Bubble SHALL have a semi-transparent white background (white at 5% opacity) with slate-100 text (#e2e8f0)
5. WHEN a message is from AI, THE Message_Bubble SHALL be aligned to the left side with rounded corners: top-left 16px, top-right 16px, bottom-left 2px, bottom-right 16px
6. WHEN a message is from AI, THE Message_Bubble SHALL have a subtle border (white at 8% opacity) and backdrop blur effect
7. THE Message_Bubble SHALL display sender label ("You" or "Vira") above the bubble in 11px slate-400 text
8. THE Message_Bubble SHALL display timestamp below the bubble in 10px slate-600 text
9. THE Message_Bubble SHALL have padding of 14px horizontal and 14px vertical
10. THE Message_Bubble SHALL animate in with fade and slide-up effect (opacity 0→1, translateY 12→0, scale 0.96→1) over 280ms
11. THE Message_Bubble SHALL support maximum width of 85% of screen width
12. THE Message_Bubble SHALL support bold text formatting using **text** markdown syntax

### Requirement 5: Quick Action Buttons

**User Story:** As a user, I want quick access to common queries, so that I can get information faster without typing.

#### Acceptance Criteria

1. THE Main_Chat_Screen SHALL display a horizontal scrollable row of Quick_Action_Buttons above the input field
2. THE Quick_Action_Buttons SHALL include: Weather (sun icon, yellow), News (newspaper icon, blue), Reminders (bell icon, red), Traffic (car icon, green), Coffee (coffee icon, amber), Music (music icon, purple)
3. WHEN a Quick_Action_Button is displayed, THE button SHALL have a semi-transparent white background (white at 5% opacity) with white border at 10% opacity
4. WHEN a Quick_Action_Button is displayed, THE button SHALL show an icon (14px size) with role-specific color and label text (13px, slate-300)
5. WHEN a Quick_Action_Button is tapped, THE button SHALL scale to 93% size with spring animation
6. WHEN a Quick_Action_Button is tapped, THE system SHALL send the associated query to the chat
7. THE Quick_Action_Buttons row SHALL be horizontally scrollable without showing scrollbar
8. THE Quick_Action_Buttons SHALL have 8px spacing between them
9. THE Quick_Action_Buttons SHALL have rounded corners of 16px (full pill shape)
10. THE Quick_Action_Buttons SHALL have padding of 14px horizontal and 6px vertical

### Requirement 6: Input Area Components

**User Story:** As a user, I want an intuitive input area with voice and text options, so that I can communicate with VIRA in my preferred way.

#### Acceptance Criteria

1. THE input area SHALL display a rounded container (32px border radius) with semi-transparent background and border
2. THE input area SHALL contain a "New Chat" button (plus icon) on the left side
3. THE input area SHALL contain a text input field in the center with placeholder "Ask Vira anything..."
4. THE input area SHALL contain a voice button (microphone icon) and send button (send icon) on the right side
5. WHEN the voice button is displayed, THE button SHALL be 40px × 40px with semi-transparent white background (white at 5% opacity)
6. WHEN the send button is displayed, THE button SHALL be 40px × 40px with purple gradient background (#8b5cf6) and purple shadow
7. WHEN the send button is disabled (empty input), THE button SHALL have 40% opacity
8. WHEN the voice or send button is tapped, THE button SHALL scale to 90% size with spring animation
9. THE text input field SHALL have transparent background, slate-200 text color, and slate-500 placeholder color
10. THE text input field SHALL support Enter key to send message
11. THE input area container SHALL have padding of 6px and gap of 6px between elements
12. THE input area SHALL display a bottom handle (112px wide, 4px tall, white at 15% opacity, rounded) centered below it

### Requirement 7: Voice Active Screen

**User Story:** As a user, I want a dedicated voice mode with visual feedback, so that I can interact with VIRA hands-free.

#### Acceptance Criteria

1. WHEN the voice button is tapped, THE system SHALL navigate to the Voice_Active_Screen
2. THE Voice_Active_Screen SHALL display a dark background (#101f22) with animated gradient overlays
3. THE Voice_Active_Screen SHALL display a header with close button, status indicator, and menu button
4. THE Voice_Active_Screen SHALL display a Voice_Orb in the center with animated rings and glow effects
5. THE Voice_Active_Screen SHALL display a Waveform_Visualizer below the Voice_Orb
6. THE Voice_Active_Screen SHALL display transcribed text in real-time as the user speaks
7. THE Voice_Active_Screen SHALL display a status label (LISTENING, PROCESSING, or DONE) in the header
8. WHEN the voice state is "listening", THE status indicator SHALL be cyan (#25d1f4) and pulse with animation
9. WHEN the voice state is "processing", THE status indicator SHALL be purple (#a855f7) and pulse with animation
10. WHEN the voice state is "done", THE status indicator SHALL be green (#22c55e) without animation
11. WHEN the voice state is "done", THE Voice_Active_Screen SHALL display a "Send to Vira" button with purple gradient
12. WHEN the close button is tapped, THE system SHALL navigate back to Main_Chat_Screen
13. WHEN the "Send to Vira" button is tapped, THE system SHALL send the transcript to Main_Chat_Screen and navigate back

### Requirement 8: Voice Orb Animation

**User Story:** As a user, I want engaging visual feedback during voice interaction, so that I know VIRA is listening and processing.

#### Acceptance Criteria

1. THE Voice_Orb SHALL be a circular element 192px in diameter with gradient background
2. THE Voice_Orb SHALL display a "V" logo in the center with gradient text effect (cyan to light purple)
3. THE Voice_Orb SHALL have two concentric ring borders (272px and 292px diameter) with semi-transparent colors
4. WHEN the voice state is "listening" or "processing", THE Voice_Orb SHALL scale with animation (1.0 → 1.04 → 1.0) over 2 seconds, repeating
5. WHEN the voice state is "listening" or "processing", THE outer rings SHALL scale and fade with animation
6. WHEN the voice state is "listening" or "processing", THE Voice_Orb SHALL have an outer glow effect that pulses
7. WHEN the voice state is "processing", THE "V" logo SHALL pulse opacity (1.0 → 0.4 → 1.0) over 1.2 seconds, repeating
8. WHEN the voice state is "idle", THE Voice_Orb SHALL remain static without animations
9. THE Voice_Orb SHALL have a gradient background from cyan (rgba(37,209,244,0.1)) to purple (rgba(168,85,247,0.1))
10. THE Voice_Orb SHALL have a subtle border (white at 10% opacity) and backdrop blur effect

### Requirement 9: Waveform Visualizer

**User Story:** As a user, I want to see audio levels visualized, so that I have feedback that my voice is being captured.

#### Acceptance Criteria

1. THE Waveform_Visualizer SHALL display 40 vertical bars arranged horizontally
2. THE Waveform_Visualizer bars SHALL have 3px spacing between them and 4px width each
3. WHEN the voice state is "listening", THE bars SHALL animate with random heights following a bell curve distribution (center bars taller)
4. WHEN the voice state is "listening", THE bars SHALL be colored red (#ef4444)
5. WHEN the voice state is "processing" (AI speaking), THE bars SHALL animate with taller random heights
6. WHEN the voice state is "processing" (AI speaking), THE bars SHALL be colored purple (#a855f7)
7. WHEN the voice state is "idle", THE bars SHALL have minimal static height (4px) with bell curve distribution
8. WHEN the voice state is "idle", THE bars SHALL be colored gray (#e5e7eb)
9. THE bars SHALL update at 50ms intervals (20 times per second) when active
10. THE bars SHALL have rounded ends (full border radius)
11. THE bars SHALL animate height changes with spring physics (stiffness 300, damping 20)

### Requirement 10: Card Components

**User Story:** As a user, I want rich visual cards for weather, news, schedule, and traffic information, so that I can quickly scan important data.

#### Acceptance Criteria

1. WHEN AI responds with weather data, THE system SHALL display a Weather_Card component
2. THE Weather_Card SHALL display city name, temperature, condition with emoji, humidity, UV index, and tomorrow's forecast
3. THE Weather_Card SHALL have a semi-transparent background with rounded corners and proper spacing
4. WHEN AI responds with news data, THE system SHALL display a News_Card component
5. THE News_Card SHALL display multiple news items with category emoji and title
6. THE News_Card SHALL have consistent spacing between news items (12px)
7. WHEN AI responds with schedule data, THE system SHALL display a Schedule_Card component
8. THE Schedule_Card SHALL display multiple schedule items with time, title, location, and color-coded indicator
9. THE Schedule_Card SHALL have rounded item containers with semi-transparent backgrounds
10. WHEN AI responds with traffic data, THE system SHALL display a Traffic_Card component
11. THE Traffic_Card SHALL display multiple routes with route name, ETA, status, and color-coded status indicator
12. THE Traffic_Card SHALL have consistent layout with route info on left and ETA on right
13. ALL card components SHALL use 15px font size for primary text and 13px for secondary text
14. ALL card components SHALL have proper padding (12px) and spacing (12px between items)

### Requirement 11: Loading Indicator

**User Story:** As a user, I want to see when VIRA is processing my request, so that I know the system is working.

#### Acceptance Criteria

1. WHEN AI is processing a response, THE system SHALL display a Loading_Indicator
2. THE Loading_Indicator SHALL be positioned on the left side like an AI message
3. THE Loading_Indicator SHALL display a sparkle icon in a circular gradient badge (purple to indigo)
4. THE Loading_Indicator SHALL display the text "Vira is thinking..." in 11px uppercase slate-400 text
5. THE Loading_Indicator SHALL display 4 animated vertical bars in a semi-transparent container
6. THE bars SHALL animate height (6px → 16px → 6px) and opacity (0.4 → 1.0 → 0.4) with staggered delays
7. THE bars SHALL have gradient colors from cyan (#25d1f4) to purple (#a855f7)
8. THE bars SHALL have 4px spacing between them and 6px width each
9. THE Loading_Indicator SHALL animate in with fade and slide-up effect (opacity 0→1, translateY 10→0, scale 0.95→1)
10. THE Loading_Indicator SHALL be removed when AI response is ready

### Requirement 12: Chat History Sidebar

**User Story:** As a user, I want to access my past conversations, so that I can continue previous discussions or review history.

#### Acceptance Criteria

1. WHEN the menu button is tapped, THE system SHALL display the Chat_History_Sidebar
2. THE Chat_History_Sidebar SHALL slide in from the left side with animation
3. THE Chat_History_Sidebar SHALL have a semi-transparent dark background with backdrop blur
4. THE Chat_History_Sidebar SHALL display a header with "Chat History" title and close button
5. THE Chat_History_Sidebar SHALL display a "New Chat" button at the top
6. THE Chat_History_Sidebar SHALL display a list of past conversations grouped by date (Today, Yesterday, Last 7 Days, Last 30 Days)
7. WHEN a past conversation is tapped, THE system SHALL load that conversation and close the sidebar
8. THE Chat_History_Sidebar SHALL display a "Clear History" button at the bottom with red text
9. WHEN the "Clear History" button is tapped, THE system SHALL show a confirmation and clear all history
10. WHEN the close button or backdrop is tapped, THE Chat_History_Sidebar SHALL slide out and close
11. THE Chat_History_Sidebar SHALL have a width of 85% of screen width
12. THE Chat_History_Sidebar SHALL display conversation titles with truncation if too long

### Requirement 13: Permission Dialogs

**User Story:** As a user, I want clear explanations when permissions are needed, so that I understand why VIRA needs access.

#### Acceptance Criteria

1. WHEN a permission is required, THE system SHALL display a Permission_Dialog
2. THE Permission_Dialog SHALL have a semi-transparent dark background with rounded corners
3. THE Permission_Dialog SHALL display an icon representing the permission type (microphone, contacts, etc.)
4. THE Permission_Dialog SHALL display a title explaining what permission is needed
5. THE Permission_Dialog SHALL display a description explaining why the permission is needed
6. THE Permission_Dialog SHALL display two buttons: "Cancel" and "Grant Permission"
7. WHEN the "Grant Permission" button is tapped, THE system SHALL request the Android permission
8. WHEN the "Cancel" button is tapped, THE Permission_Dialog SHALL close without requesting permission
9. THE Permission_Dialog SHALL have a backdrop that dims the background content
10. THE Permission_Dialog SHALL animate in with fade and scale effect

### Requirement 14: Error Notifications

**User Story:** As a user, I want to be notified of errors and confirmations, so that I know when something goes wrong or succeeds.

#### Acceptance Criteria

1. WHEN an error occurs, THE system SHALL display an Error_Snackbar at the bottom of the screen
2. THE Error_Snackbar SHALL have a semi-transparent dark background with red accent for errors
3. THE Error_Snackbar SHALL have a semi-transparent dark background with green accent for success messages
4. THE Error_Snackbar SHALL display a title and description text
5. THE Error_Snackbar SHALL display an icon (X for errors, checkmark for success)
6. THE Error_Snackbar SHALL automatically dismiss after 4 seconds
7. THE Error_Snackbar SHALL be dismissible by swiping down
8. THE Error_Snackbar SHALL animate in from bottom with slide-up effect
9. THE Error_Snackbar SHALL animate out with slide-down and fade effect
10. THE Error_Snackbar SHALL have rounded corners (16px) and proper padding (16px)

### Requirement 15: Typography System

**User Story:** As a developer, I want a consistent typography system, so that all text is readable and follows the design system.

#### Acceptance Criteria

1. THE Typography_System SHALL define a base font size of 16px
2. THE Typography_System SHALL use a sans-serif font family (Segoe UI on Windows, Roboto on Android)
3. THE Typography_System SHALL define heading sizes: H1 (24px), H2 (20px), H3 (18px), H4 (16px)
4. THE Typography_System SHALL define body text size of 15px with 1.5 line height
5. THE Typography_System SHALL define small text size of 13px for secondary information
6. THE Typography_System SHALL define tiny text size of 11px for labels and timestamps
7. THE Typography_System SHALL define font weights: normal (400), medium (500), semibold (600), bold (700)
8. THE Typography_System SHALL be implemented as XAML styles accessible throughout the application
9. ALL text elements SHALL use the Typography_System styles instead of inline font properties

### Requirement 16: Animation System

**User Story:** As a user, I want smooth animations throughout the app, so that interactions feel polished and responsive.

#### Acceptance Criteria

1. THE Animation_System SHALL define standard durations: fast (150ms), normal (280ms), slow (400ms)
2. THE Animation_System SHALL define standard easing functions: ease-out, ease-in-out, spring
3. WHEN a button is tapped, THE button SHALL scale to 92-95% size with spring animation
4. WHEN a screen transitions, THE screen SHALL fade in with 300ms duration
5. WHEN a dialog appears, THE dialog SHALL scale from 0.9 to 1.0 and fade in over 280ms
6. WHEN a message appears, THE message SHALL slide up and fade in over 280ms
7. WHEN the sidebar opens, THE sidebar SHALL slide in from left over 300ms with ease-out
8. WHEN content scrolls, THE scroll SHALL be smooth without janky frame drops
9. THE Animation_System SHALL maintain 60 FPS performance on target Android devices
10. THE Animation_System SHALL use XAML Storyboards for complex animations

### Requirement 17: Responsive Layout

**User Story:** As a user, I want the app to work well on different screen sizes, so that I can use it on various Android devices.

#### Acceptance Criteria

1. THE layout SHALL adapt to screen widths from 360px to 480px (common Android phone sizes)
2. THE layout SHALL use relative sizing (percentages, flex) instead of fixed pixel values where appropriate
3. THE message bubbles SHALL have maximum width of 85% to prevent overly wide text on large screens
4. THE input area SHALL have maximum width of 600px and center on wider screens
5. THE Quick_Action_Buttons SHALL scroll horizontally on narrow screens
6. THE Chat_History_Sidebar SHALL have width of 85% on narrow screens and maximum 400px on wider screens
7. THE Voice_Orb SHALL scale proportionally on different screen sizes
8. THE header elements SHALL have appropriate spacing on different screen widths
9. THE card components SHALL stack vertically and expand to full width
10. THE layout SHALL be tested on screens from 5 inches to 6.7 inches diagonal

### Requirement 18: Accessibility

**User Story:** As a user with accessibility needs, I want the app to be usable with assistive technologies, so that I can access all features.

#### Acceptance Criteria

1. ALL interactive elements SHALL have minimum touch target size of 44×44 pixels
2. ALL buttons SHALL have accessible names describing their function
3. ALL text SHALL have minimum contrast ratio of 4.5:1 against background (WCAG AA)
4. THE app SHALL support Android TalkBack screen reader
5. THE app SHALL support dynamic text sizing (user can increase font size in system settings)
6. ALL form inputs SHALL have associated labels
7. ALL icons SHALL have text alternatives for screen readers
8. THE focus order SHALL follow logical reading order (top to bottom, left to right)
9. THE app SHALL not rely solely on color to convey information
10. THE app SHALL support keyboard navigation where applicable

### Requirement 19: Performance

**User Story:** As a user, I want the app to be fast and responsive, so that I can interact without delays or lag.

#### Acceptance Criteria

1. THE Main_Chat_Screen SHALL render initial view within 500ms of navigation
2. THE message list SHALL scroll smoothly at 60 FPS with 100+ messages
3. THE Voice_Active_Screen SHALL start within 300ms of button tap
4. THE animations SHALL maintain 60 FPS on mid-range Android devices (Snapdragon 600 series or equivalent)
5. THE app memory usage SHALL not exceed 150MB during normal operation
6. THE gradient effects SHALL use GPU acceleration to avoid CPU overhead
7. THE Waveform_Visualizer SHALL update at 20 FPS without dropping frames
8. THE image assets SHALL be optimized for size and loading speed
9. THE app SHALL not block the UI thread during AI processing
10. THE app startup time SHALL be under 2 seconds on target devices

### Requirement 20: XAML Resource Organization

**User Story:** As a developer, I want well-organized XAML resources, so that the UI is maintainable and consistent.

#### Acceptance Criteria

1. THE project SHALL have a Colors.xaml Resource_Dictionary defining all color values
2. THE project SHALL have a Brushes.xaml Resource_Dictionary defining all gradient brushes
3. THE project SHALL have a Styles.xaml Resource_Dictionary defining all control styles
4. THE project SHALL have an Animations.xaml Resource_Dictionary defining reusable storyboards
5. THE project SHALL have a Typography.xaml Resource_Dictionary defining all text styles
6. ALL Resource_Dictionaries SHALL be merged in App.xaml for global access
7. THE color values SHALL use x:Key naming convention: PrimaryBackgroundColor, AccentPurpleColor, etc.
8. THE gradient brushes SHALL use x:Key naming convention: PurpleGradientBrush, CyanPurpleGradientBrush, etc.
9. THE styles SHALL use x:Key naming convention: MessageBubbleUserStyle, MessageBubbleAIStyle, etc.
10. THE resources SHALL not contain unused or duplicate definitions
