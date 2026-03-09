# 🎨 VIRA Intelligent Assistant - Design Document

## 📋 Document Overview

**Feature**: VIRA (Voice Intelligence & Responsive Assistant)  
**Version**: 1.0  
**Status**: Design Phase  
**Target Platform**: Android 8.0+ (API 26+)  
**Primary Language**: Kotlin  
**Architecture**: MVVM + Clean Architecture

---

## 🎯 Design Goals

1. **Modularity**: Loosely coupled components for easy testing and maintenance
2. **Scalability**: Support for adding new features without major refactoring
3. **Performance**: Fast response times (<100ms rule-based, <2s AI-enhanced)
4. **Reliability**: Graceful degradation when services unavailable
5. **Security**: Secure storage of API keys and user data
6. **Testability**: High test coverage with clear separation of concerns

---

## 🏗️ System Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        Presentation Layer                        │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐         │
│  │  ChatView    │  │ QuickActions │  │ SettingsView │         │
│  │  (Compose)   │  │   (Compose)  │  │  (Compose)   │         │
│  └──────────────┘  └──────────────┘  └──────────────┘         │
│         ↕                  ↕                  ↕                  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │              ViewModel Layer (State Management)           │  │
│  │  • ChatViewModel  • QuickActionsViewModel                │  │
│  │  • SettingsViewModel                                     │  │
│  └──────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
                              ↕
┌─────────────────────────────────────────────────────────────────┐
│                         Domain Layer                             │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │                    Use Cases                              │  │
│  │  • ProcessMessageUseCase  • ManageTasksUseCase           │  │
│  │  • GetWeatherUseCase      • GetNewsUseCase               │  │
│  │  • ExecuteAndroidActionUseCase                           │  │
│  └──────────────────────────────────────────────────────────┘  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │                  Domain Models                            │  │
│  │  • Message  • Task  • Schedule  • WeatherData            │  │
│  └──────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
                              ↕
┌─────────────────────────────────────────────────────────────────┐
│                          Data Layer                              │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │                   Repositories                            │  │
│  │  • MessageRepository  • TaskRepository                   │  │
│  │  • WeatherRepository  • NewsRepository                   │  │
│  │  • ConfigRepository   • ContactRepository                │  │
│  └──────────────────────────────────────────────────────────┘  │
│         ↕                    ↕                    ↕              │
│  ┌─────────────┐  ┌──────────────┐  ┌──────────────────┐      │
│  │   Local     │  │   Remote     │  │     Android      │      │
│  │ Data Source │  │ Data Source  │  │   Integration    │      │
│  │  (SQLite)   │  │  (APIs)      │  │    Services      │      │
│  └─────────────┘  └──────────────┘  └──────────────────┘      │
└─────────────────────────────────────────────────────────────────┘
```


### Architecture Layers

#### 1. Presentation Layer
- **Technology**: Jetpack Compose for modern, declarative UI
- **Pattern**: MVVM with unidirectional data flow
- **State Management**: StateFlow for reactive state updates
- **Navigation**: Compose Navigation for screen transitions

#### 2. Domain Layer
- **Pure Kotlin**: No Android dependencies
- **Use Cases**: Single responsibility, testable business logic
- **Domain Models**: Immutable data classes
- **Interfaces**: Repository interfaces defined here

#### 3. Data Layer
- **Repository Pattern**: Single source of truth
- **Data Sources**: Local (Room), Remote (Retrofit), Android (System Services)
- **Mappers**: Convert between data models and domain models
- **Caching**: In-memory + persistent caching strategy

---

## 🧩 Core Components

### 1. Message Processing Engine

#### 1.1 MessageProcessor Interface

```kotlin
interface MessageProcessor {
    suspend fun processMessage(
        message: String,
        context: ConversationContext
    ): ProcessingResult
}

data class ConversationContext(
    val userId: String,
    val sessionId: String,
    val previousMessages: List<Message>,
    val currentTime: LocalDateTime,
    val location: Location?,
    val deviceState: DeviceState
)

sealed class ProcessingResult {
    data class RuleBased(
        val response: String,
        val action: Action?,
        val confidence: Float
    ) : ProcessingResult()
    
    data class AIEnhanced(
        val response: String,
        val action: Action?,
        val provider: AIProvider
    ) : ProcessingResult()
    
    data class Error(
        val message: String,
        val fallbackResponse: String?
    ) : ProcessingResult()
}
```

#### 1.2 Hybrid Processing Strategy

```kotlin
class HybridMessageProcessor(
    private val ruleBasedProcessor: RuleBasedProcessor,
    private val aiProcessor: AIProcessor,
    private val configRepository: ConfigRepository
) : MessageProcessor {
    
    override suspend fun processMessage(
        message: String,
        context: ConversationContext
    ): ProcessingResult {
        // Step 1: Try rule-based processing first
        val ruleResult = ruleBasedProcessor.process(message, context)
        
        if (ruleResult.confidence > CONFIDENCE_THRESHOLD) {
            return ProcessingResult.RuleBased(
                response = ruleResult.response,
                action = ruleResult.action,
                confidence = ruleResult.confidence
            )
        }
        
        // Step 2: Check if AI is configured
        val aiConfig = configRepository.getAIConfig()
        if (!aiConfig.isConfigured) {
            return ProcessingResult.Error(
                message = "AI not configured",
                fallbackResponse = generateHelpMessage()
            )
        }
        
        // Step 3: Use AI for complex queries
        return try {
            val aiResult = aiProcessor.process(message, context, aiConfig)
            ProcessingResult.AIEnhanced(
                response = aiResult.response,
                action = aiResult.action,
                provider = aiConfig.provider
            )
        } catch (e: Exception) {
            ProcessingResult.Error(
                message = e.message ?: "AI processing failed",
                fallbackResponse = ruleResult.response
            )
        }
    }
    
    companion object {
        private const val CONFIDENCE_THRESHOLD = 0.8f
    }
}
```


### 2. Rule-Based Pattern Matching Engine

#### 2.1 Pattern Matcher Architecture

```kotlin
data class CommandPattern(
    val id: String,
    val regex: Regex,
    val category: CommandCategory,
    val priority: Int,
    val handler: CommandHandler,
    val requiredPermissions: List<String> = emptyList()
)

enum class CommandCategory {
    TASK_MANAGEMENT,
    INFORMATION_QUERY,
    ANDROID_INTEGRATION,
    GREETING,
    SYSTEM_CONTROL,
    MEDIA_CONTROL,
    CONTACT_MANAGEMENT
}

interface CommandHandler {
    suspend fun handle(
        match: MatchResult,
        context: ConversationContext
    ): CommandResult
}

data class CommandResult(
    val response: String,
    val action: Action?,
    val confidence: Float,
    val speak: Boolean = false
)
```

#### 2.2 Pattern Registry

```kotlin
class PatternRegistry {
    private val patterns = mutableListOf<CommandPattern>()
    
    init {
        registerTaskPatterns()
        registerInfoPatterns()
        registerAndroidPatterns()
        registerGreetingPatterns()
        registerSystemPatterns()
        registerMediaPatterns()
        registerContactPatterns()
    }
    
    private fun registerTaskPatterns() {
        // Add task pattern
        patterns.add(CommandPattern(
            id = "add_task",
            regex = Regex(
                "\\b(tambah|add|buat|create|catat)\\s+(task|tugas|todo)\\s+(.+)",
                RegexOption.IGNORE_CASE
            ),
            category = CommandCategory.TASK_MANAGEMENT,
            priority = 10,
            handler = AddTaskHandler()
        ))
        
        // Complete task pattern
        patterns.add(CommandPattern(
            id = "complete_task",
            regex = Regex(
                "\\b(selesai|done|complete|finish)\\s+(task|tugas)\\s*(.+)?",
                RegexOption.IGNORE_CASE
            ),
            category = CommandCategory.TASK_MANAGEMENT,
            priority = 10,
            handler = CompleteTaskHandler()
        ))
        
        // List tasks pattern
        patterns.add(CommandPattern(
            id = "list_tasks",
            regex = Regex(
                "\\b(daftar|list|show|tampilkan)\\s+(task|tugas|todo)",
                RegexOption.IGNORE_CASE
            ),
            category = CommandCategory.TASK_MANAGEMENT,
            priority = 10,
            handler = ListTasksHandler()
        ))
    }
    
    private fun registerInfoPatterns() {
        // Weather pattern
        patterns.add(CommandPattern(
            id = "weather",
            regex = Regex(
                "\\b(cuaca|weather|suhu|temperature|hujan|rain)\\b",
                RegexOption.IGNORE_CASE
            ),
            category = CommandCategory.INFORMATION_QUERY,
            priority = 9,
            handler = WeatherHandler()
        ))
        
        // News pattern
        patterns.add(CommandPattern(
            id = "news",
            regex = Regex(
                "\\b(berita|news|headline|kabar)\\b",
                RegexOption.IGNORE_CASE
            ),
            category = CommandCategory.INFORMATION_QUERY,
            priority = 9,
            handler = NewsHandler()
        ))
    }
    
    private fun registerAndroidPatterns() {
        // Open app pattern
        patterns.add(CommandPattern(
            id = "open_app",
            regex = Regex(
                "\\b(buka|open|jalankan|launch)\\s+(aplikasi|app)?\\s*([\\w\\s]+)",
                RegexOption.IGNORE_CASE
            ),
            category = CommandCategory.ANDROID_INTEGRATION,
            priority = 10,
            handler = OpenAppHandler()
        ))
        
        // Send WhatsApp pattern
        patterns.add(CommandPattern(
            id = "send_whatsapp",
            regex = Regex(
                "\\b(kirim|send|chat)\\s+(pesan|message|whatsapp|wa)\\s+(ke|to)\\s+([\\w\\s]+)",
                RegexOption.IGNORE_CASE
            ),
            category = CommandCategory.CONTACT_MANAGEMENT,
            priority = 10,
            handler = SendWhatsAppHandler(),
            requiredPermissions = listOf(Manifest.permission.READ_CONTACTS)
        ))
        
        // Make call pattern
        patterns.add(CommandPattern(
            id = "make_call",
            regex = Regex(
                "\\b(telepon|call|hubungi)\\s+(ke|to)?\\s*([\\w\\s]+)",
                RegexOption.IGNORE_CASE
            ),
            category = CommandCategory.CONTACT_MANAGEMENT,
            priority = 10,
            handler = MakeCallHandler(),
            requiredPermissions = listOf(
                Manifest.permission.READ_CONTACTS,
                Manifest.permission.CALL_PHONE
            )
        ))
    }
    
    fun findMatch(input: String): PatternMatch? {
        return patterns
            .sortedByDescending { it.priority }
            .firstNotNullOfOrNull { pattern ->
                pattern.regex.find(input)?.let { match ->
                    PatternMatch(pattern, match)
                }
            }
    }
}

data class PatternMatch(
    val pattern: CommandPattern,
    val match: MatchResult
)
```


### 3. AI API Abstraction Layer

#### 3.1 AI Provider Interface

```kotlin
interface AIProvider {
    suspend fun generateResponse(
        prompt: String,
        context: ConversationContext,
        config: AIConfig
    ): AIResponse
    
    suspend fun validateApiKey(apiKey: String): ValidationResult
    
    fun getProviderName(): String
}

data class AIConfig(
    val provider: AIProviderType,
    val apiKey: String,
    val model: String,
    val temperature: Float = 0.9f,
    val maxTokens: Int = 1024,
    val isConfigured: Boolean = false
)

enum class AIProviderType {
    GROQ,
    GEMINI,
    OPENAI
}

data class AIResponse(
    val response: String,
    val action: Action?,
    val tokensUsed: Int,
    val latencyMs: Long
)

sealed class ValidationResult {
    object Valid : ValidationResult()
    data class Invalid(val reason: String) : ValidationResult()
    data class NetworkError(val message: String) : ValidationResult()
}
```

#### 3.2 Groq Provider Implementation

```kotlin
class GroqProvider(
    private val httpClient: HttpClient
) : AIProvider {
    
    override suspend fun generateResponse(
        prompt: String,
        context: ConversationContext,
        config: AIConfig
    ): AIResponse {
        val startTime = System.currentTimeMillis()
        
        val request = GroqRequest(
            model = config.model,
            messages = buildMessages(prompt, context),
            temperature = config.temperature,
            maxTokens = config.maxTokens
        )
        
        val response = httpClient.post("https://api.groq.com/openai/v1/chat/completions") {
            header("Authorization", "Bearer ${config.apiKey}")
            header("Content-Type", "application/json")
            setBody(request)
        }.body<GroqResponse>()
        
        val latency = System.currentTimeMillis() - startTime
        
        return AIResponse(
            response = response.choices.first().message.content,
            action = extractAction(response.choices.first().message.content),
            tokensUsed = response.usage.totalTokens,
            latencyMs = latency
        )
    }
    
    override suspend fun validateApiKey(apiKey: String): ValidationResult {
        return try {
            val response = httpClient.get("https://api.groq.com/openai/v1/models") {
                header("Authorization", "Bearer $apiKey")
            }
            
            if (response.status.isSuccess()) {
                ValidationResult.Valid
            } else {
                ValidationResult.Invalid("Invalid API key")
            }
        } catch (e: Exception) {
            ValidationResult.NetworkError(e.message ?: "Network error")
        }
    }
    
    override fun getProviderName(): String = "Groq"
    
    private fun buildMessages(
        prompt: String,
        context: ConversationContext
    ): List<GroqMessage> {
        val messages = mutableListOf<GroqMessage>()
        
        // System message
        messages.add(GroqMessage(
            role = "system",
            content = buildSystemPrompt()
        ))
        
        // Previous messages (last 5 for context)
        context.previousMessages.takeLast(5).forEach { msg ->
            messages.add(GroqMessage(
                role = if (msg.isUser) "user" else "assistant",
                content = msg.content
            ))
        }
        
        // Current message
        messages.add(GroqMessage(
            role = "user",
            content = prompt
        ))
        
        return messages
    }
    
    private fun buildSystemPrompt(): String = """
        Kamu adalah VIRA (Voice Intelligence & Responsive Assistant), asisten AI pribadi yang cerdas dan membantu.
        
        Karakteristik:
        - Ramah, profesional, dan responsif
        - Berbicara dalam Bahasa Indonesia yang natural
        - Memberikan jawaban yang singkat dan jelas
        - Proaktif memberikan saran yang relevan
        
        Kemampuan:
        - Manajemen task dan reminder
        - Informasi cuaca, berita, jadwal
        - Kontrol sistem Android (buka app, kirim pesan, telepon)
        - Analisis dan rekomendasi berbasis konteks
        
        Format response:
        - Gunakan bahasa yang natural dan conversational
        - Jika perlu melakukan action, awali dengan [ACTION: action_type]
        - Contoh: [ACTION: add_task] Baik, saya sudah menambahkan task "Beli susu" ke daftar Anda.
    """.trimIndent()
    
    private fun extractAction(response: String): Action? {
        val actionRegex = Regex("\\[ACTION:\\s*(\\w+)\\]")
        val match = actionRegex.find(response) ?: return null
        
        val actionType = match.groupValues[1]
        return when (actionType) {
            "add_task" -> Action.AddTask
            "complete_task" -> Action.CompleteTask
            "open_app" -> Action.OpenApp
            "send_message" -> Action.SendMessage
            else -> null
        }
    }
}

data class GroqRequest(
    val model: String,
    val messages: List<GroqMessage>,
    val temperature: Float,
    @SerialName("max_tokens") val maxTokens: Int
)

data class GroqMessage(
    val role: String,
    val content: String
)

data class GroqResponse(
    val choices: List<GroqChoice>,
    val usage: GroqUsage
)

data class GroqChoice(
    val message: GroqMessage
)

data class GroqUsage(
    @SerialName("total_tokens") val totalTokens: Int
)
```


#### 3.3 Gemini Provider Implementation

```kotlin
class GeminiProvider(
    private val httpClient: HttpClient
) : AIProvider {
    
    override suspend fun generateResponse(
        prompt: String,
        context: ConversationContext,
        config: AIConfig
    ): AIResponse {
        val startTime = System.currentTimeMillis()
        
        val request = GeminiRequest(
            contents = buildContents(prompt, context),
            generationConfig = GeminiGenerationConfig(
                temperature = config.temperature,
                maxOutputTokens = config.maxTokens
            )
        )
        
        val url = "https://generativelanguage.googleapis.com/v1beta/models/${config.model}:generateContent?key=${config.apiKey}"
        
        val response = httpClient.post(url) {
            header("Content-Type", "application/json")
            setBody(request)
        }.body<GeminiResponse>()
        
        val latency = System.currentTimeMillis() - startTime
        
        return AIResponse(
            response = response.candidates.first().content.parts.first().text,
            action = extractAction(response.candidates.first().content.parts.first().text),
            tokensUsed = response.usageMetadata?.totalTokenCount ?: 0,
            latencyMs = latency
        )
    }
    
    override suspend fun validateApiKey(apiKey: String): ValidationResult {
        return try {
            val url = "https://generativelanguage.googleapis.com/v1beta/models?key=$apiKey"
            val response = httpClient.get(url)
            
            if (response.status.isSuccess()) {
                ValidationResult.Valid
            } else {
                ValidationResult.Invalid("Invalid API key")
            }
        } catch (e: Exception) {
            ValidationResult.NetworkError(e.message ?: "Network error")
        }
    }
    
    override fun getProviderName(): String = "Gemini"
    
    private fun buildContents(
        prompt: String,
        context: ConversationContext
    ): List<GeminiContent> {
        val contents = mutableListOf<GeminiContent>()
        
        // Add system instruction as first user message
        contents.add(GeminiContent(
            role = "user",
            parts = listOf(GeminiPart(text = buildSystemPrompt()))
        ))
        
        contents.add(GeminiContent(
            role = "model",
            parts = listOf(GeminiPart(text = "Baik, saya siap membantu sebagai VIRA."))
        ))
        
        // Previous messages
        context.previousMessages.takeLast(5).forEach { msg ->
            contents.add(GeminiContent(
                role = if (msg.isUser) "user" else "model",
                parts = listOf(GeminiPart(text = msg.content))
            ))
        }
        
        // Current message
        contents.add(GeminiContent(
            role = "user",
            parts = listOf(GeminiPart(text = prompt))
        ))
        
        return contents
    }
    
    private fun buildSystemPrompt(): String = """
        Kamu adalah VIRA (Voice Intelligence & Responsive Assistant), asisten AI pribadi yang cerdas dan membantu.
        
        Karakteristik:
        - Ramah, profesional, dan responsif
        - Berbicara dalam Bahasa Indonesia yang natural
        - Memberikan jawaban yang singkat dan jelas
        - Proaktif memberikan saran yang relevan
        
        Kemampuan:
        - Manajemen task dan reminder
        - Informasi cuaca, berita, jadwal
        - Kontrol sistem Android (buka app, kirim pesan, telepon)
        - Analisis dan rekomendasi berbasis konteks
        
        Format response:
        - Gunakan bahasa yang natural dan conversational
        - Jika perlu melakukan action, awali dengan [ACTION: action_type]
        - Contoh: [ACTION: add_task] Baik, saya sudah menambahkan task "Beli susu" ke daftar Anda.
    """.trimIndent()
    
    private fun extractAction(response: String): Action? {
        val actionRegex = Regex("\\[ACTION:\\s*(\\w+)\\]")
        val match = actionRegex.find(response) ?: return null
        
        val actionType = match.groupValues[1]
        return when (actionType) {
            "add_task" -> Action.AddTask
            "complete_task" -> Action.CompleteTask
            "open_app" -> Action.OpenApp
            "send_message" -> Action.SendMessage
            else -> null
        }
    }
}

data class GeminiRequest(
    val contents: List<GeminiContent>,
    val generationConfig: GeminiGenerationConfig
)

data class GeminiContent(
    val role: String,
    val parts: List<GeminiPart>
)

data class GeminiPart(
    val text: String
)

data class GeminiGenerationConfig(
    val temperature: Float,
    val maxOutputTokens: Int
)

data class GeminiResponse(
    val candidates: List<GeminiCandidate>,
    val usageMetadata: GeminiUsageMetadata?
)

data class GeminiCandidate(
    val content: GeminiContent
)

data class GeminiUsageMetadata(
    val totalTokenCount: Int
)
```


#### 3.4 OpenAI Provider Implementation

```kotlin
class OpenAIProvider(
    private val httpClient: HttpClient
) : AIProvider {
    
    override suspend fun generateResponse(
        prompt: String,
        context: ConversationContext,
        config: AIConfig
    ): AIResponse {
        val startTime = System.currentTimeMillis()
        
        val request = OpenAIRequest(
            model = config.model,
            messages = buildMessages(prompt, context),
            temperature = config.temperature,
            maxTokens = config.maxTokens
        )
        
        val response = httpClient.post("https://api.openai.com/v1/chat/completions") {
            header("Authorization", "Bearer ${config.apiKey}")
            header("Content-Type", "application/json")
            setBody(request)
        }.body<OpenAIResponse>()
        
        val latency = System.currentTimeMillis() - startTime
        
        return AIResponse(
            response = response.choices.first().message.content,
            action = extractAction(response.choices.first().message.content),
            tokensUsed = response.usage.totalTokens,
            latencyMs = latency
        )
    }
    
    override suspend fun validateApiKey(apiKey: String): ValidationResult {
        return try {
            val response = httpClient.get("https://api.openai.com/v1/models") {
                header("Authorization", "Bearer $apiKey")
            }
            
            if (response.status.isSuccess()) {
                ValidationResult.Valid
            } else {
                ValidationResult.Invalid("Invalid API key")
            }
        } catch (e: Exception) {
            ValidationResult.NetworkError(e.message ?: "Network error")
        }
    }
    
    override fun getProviderName(): String = "OpenAI"
    
    private fun buildMessages(
        prompt: String,
        context: ConversationContext
    ): List<OpenAIMessage> {
        val messages = mutableListOf<OpenAIMessage>()
        
        // System message
        messages.add(OpenAIMessage(
            role = "system",
            content = buildSystemPrompt()
        ))
        
        // Previous messages
        context.previousMessages.takeLast(5).forEach { msg ->
            messages.add(OpenAIMessage(
                role = if (msg.isUser) "user" else "assistant",
                content = msg.content
            ))
        }
        
        // Current message
        messages.add(OpenAIMessage(
            role = "user",
            content = prompt
        ))
        
        return messages
    }
    
    private fun buildSystemPrompt(): String = """
        Kamu adalah VIRA (Voice Intelligence & Responsive Assistant), asisten AI pribadi yang cerdas dan membantu.
        
        Karakteristik:
        - Ramah, profesional, dan responsif
        - Berbicara dalam Bahasa Indonesia yang natural
        - Memberikan jawaban yang singkat dan jelas
        - Proaktif memberikan saran yang relevan
        
        Kemampuan:
        - Manajemen task dan reminder
        - Informasi cuaca, berita, jadwal
        - Kontrol sistem Android (buka app, kirim pesan, telepon)
        - Analisis dan rekomendasi berbasis konteks
        
        Format response:
        - Gunakan bahasa yang natural dan conversational
        - Jika perlu melakukan action, awali dengan [ACTION: action_type]
        - Contoh: [ACTION: add_task] Baik, saya sudah menambahkan task "Beli susu" ke daftar Anda.
    """.trimIndent()
    
    private fun extractAction(response: String): Action? {
        val actionRegex = Regex("\\[ACTION:\\s*(\\w+)\\]")
        val match = actionRegex.find(response) ?: return null
        
        val actionType = match.groupValues[1]
        return when (actionType) {
            "add_task" -> Action.AddTask
            "complete_task" -> Action.CompleteTask
            "open_app" -> Action.OpenApp
            "send_message" -> Action.SendMessage
            else -> null
        }
    }
}

data class OpenAIRequest(
    val model: String,
    val messages: List<OpenAIMessage>,
    val temperature: Float,
    @SerialName("max_tokens") val maxTokens: Int
)

data class OpenAIMessage(
    val role: String,
    val content: String
)

data class OpenAIResponse(
    val choices: List<OpenAIChoice>,
    val usage: OpenAIUsage
)

data class OpenAIChoice(
    val message: OpenAIMessage
)

data class OpenAIUsage(
    @SerialName("total_tokens") val totalTokens: Int
)
```

#### 3.5 AI Provider Factory

```kotlin
class AIProviderFactory(
    private val httpClient: HttpClient
) {
    fun createProvider(type: AIProviderType): AIProvider {
        return when (type) {
            AIProviderType.GROQ -> GroqProvider(httpClient)
            AIProviderType.GEMINI -> GeminiProvider(httpClient)
            AIProviderType.OPENAI -> OpenAIProvider(httpClient)
        }
    }
}
```


### 4. Android Integration Services

#### 4.1 Android Action Executor

```kotlin
interface AndroidActionExecutor {
    suspend fun executeAction(action: AndroidAction): ActionResult
    fun checkPermissions(action: AndroidAction): PermissionStatus
}

sealed class AndroidAction {
    data class OpenApp(val packageName: String, val appName: String) : AndroidAction()
    data class SendWhatsApp(val contactName: String, val message: String) : AndroidAction()
    data class MakeCall(val contactName: String, val phoneNumber: String?) : AndroidAction()
    data class SendSMS(val contactName: String, val message: String) : AndroidAction()
    data class SearchGoogle(val query: String) : AndroidAction()
    data class SetAlarm(val hour: Int, val minute: Int, val label: String?) : AndroidAction()
    data class SetTimer(val durationSeconds: Int, val label: String?) : AndroidAction()
    data class TakePhoto() : AndroidAction()
    data class TakeScreenshot() : AndroidAction()
    data class SetVolume(val volumeType: VolumeType, val level: Int) : AndroidAction()
    data class SetBrightness(val level: Int) : AndroidAction()
    data class ToggleWiFi(val enable: Boolean) : AndroidAction()
    data class ToggleBluetooth(val enable: Boolean) : AndroidAction()
    data class ToggleFlashlight(val enable: Boolean) : AndroidAction()
    data class MediaControl(val action: MediaAction) : AndroidAction()
}

enum class VolumeType {
    MEDIA, RINGER, ALARM
}

enum class MediaAction {
    PLAY, PAUSE, NEXT, PREVIOUS
}

sealed class ActionResult {
    object Success : ActionResult()
    data class PermissionRequired(val permissions: List<String>) : ActionResult()
    data class Error(val message: String) : ActionResult()
}

sealed class PermissionStatus {
    object Granted : PermissionStatus()
    data class Required(val permissions: List<String>) : PermissionStatus()
}
```

#### 4.2 App Launcher Service

```kotlin
class AppLauncherService(
    private val context: Context,
    private val packageManager: PackageManager
) {
    private val appMappings = mapOf(
        "whatsapp" to "com.whatsapp",
        "wa" to "com.whatsapp",
        "instagram" to "com.instagram.android",
        "ig" to "com.instagram.android",
        "chrome" to "com.android.chrome",
        "browser" to "com.android.chrome",
        "gmail" to "com.google.android.gm",
        "maps" to "com.google.android.apps.maps",
        "youtube" to "com.google.android.youtube",
        "spotify" to "com.spotify.music",
        "telegram" to "org.telegram.messenger",
        "twitter" to "com.twitter.android",
        "facebook" to "com.facebook.katana",
        "messenger" to "com.facebook.orca",
        "tiktok" to "com.zhiliaoapp.musically",
        "shopee" to "com.shopee.id",
        "tokopedia" to "com.tokopedia.tkpd",
        "gojek" to "com.gojek.app",
        "grab" to "com.grabtaxi.passenger"
    )
    
    fun openApp(appName: String): ActionResult {
        val packageName = appMappings[appName.lowercase()] 
            ?: findAppByName(appName)
            ?: return ActionResult.Error("App '$appName' tidak ditemukan")
        
        return try {
            val intent = packageManager.getLaunchIntentForPackage(packageName)
            if (intent != null) {
                intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK)
                context.startActivity(intent)
                ActionResult.Success
            } else {
                ActionResult.Error("Tidak dapat membuka app '$appName'")
            }
        } catch (e: Exception) {
            ActionResult.Error("Error: ${e.message}")
        }
    }
    
    private fun findAppByName(appName: String): String? {
        val installedApps = packageManager.getInstalledApplications(PackageManager.GET_META_DATA)
        return installedApps.firstOrNull { app ->
            val label = packageManager.getApplicationLabel(app).toString()
            label.contains(appName, ignoreCase = true)
        }?.packageName
    }
    
    fun getInstalledApps(): List<AppInfo> {
        val installedApps = packageManager.getInstalledApplications(PackageManager.GET_META_DATA)
        return installedApps
            .filter { app ->
                packageManager.getLaunchIntentForPackage(app.packageName) != null
            }
            .map { app ->
                AppInfo(
                    packageName = app.packageName,
                    name = packageManager.getApplicationLabel(app).toString(),
                    icon = packageManager.getApplicationIcon(app)
                )
            }
            .sortedBy { it.name }
    }
}

data class AppInfo(
    val packageName: String,
    val name: String,
    val icon: Drawable
)
```


#### 4.3 Contact Manager Service

```kotlin
class ContactManagerService(
    private val context: Context,
    private val contentResolver: ContentResolver
) {
    fun findContact(name: String): ContactResult {
        if (!hasContactPermission()) {
            return ContactResult.PermissionRequired
        }
        
        val contacts = searchContacts(name)
        
        return when {
            contacts.isEmpty() -> ContactResult.NotFound(name)
            contacts.size == 1 -> ContactResult.Found(contacts.first())
            else -> ContactResult.Multiple(contacts)
        }
    }
    
    private fun searchContacts(query: String): List<Contact> {
        val contacts = mutableListOf<Contact>()
        val projection = arrayOf(
            ContactsContract.CommonDataKinds.Phone.CONTACT_ID,
            ContactsContract.CommonDataKinds.Phone.DISPLAY_NAME,
            ContactsContract.CommonDataKinds.Phone.NUMBER
        )
        
        val selection = "${ContactsContract.CommonDataKinds.Phone.DISPLAY_NAME} LIKE ?"
        val selectionArgs = arrayOf("%$query%")
        
        contentResolver.query(
            ContactsContract.CommonDataKinds.Phone.CONTENT_URI,
            projection,
            selection,
            selectionArgs,
            null
        )?.use { cursor ->
            val idIndex = cursor.getColumnIndex(ContactsContract.CommonDataKinds.Phone.CONTACT_ID)
            val nameIndex = cursor.getColumnIndex(ContactsContract.CommonDataKinds.Phone.DISPLAY_NAME)
            val numberIndex = cursor.getColumnIndex(ContactsContract.CommonDataKinds.Phone.NUMBER)
            
            while (cursor.moveToNext()) {
                contacts.add(Contact(
                    id = cursor.getLong(idIndex),
                    name = cursor.getString(nameIndex),
                    phoneNumber = cursor.getString(numberIndex)
                ))
            }
        }
        
        return contacts.distinctBy { it.id }
    }
    
    private fun hasContactPermission(): Boolean {
        return ContextCompat.checkSelfPermission(
            context,
            Manifest.permission.READ_CONTACTS
        ) == PackageManager.PERMISSION_GRANTED
    }
    
    fun sendWhatsApp(contact: Contact, message: String): ActionResult {
        return try {
            val intent = Intent(Intent.ACTION_VIEW).apply {
                data = Uri.parse("https://wa.me/${contact.phoneNumber.filter { it.isDigit() }}")
                putExtra("sms_body", message)
                setPackage("com.whatsapp")
            }
            context.startActivity(intent)
            ActionResult.Success
        } catch (e: Exception) {
            ActionResult.Error("WhatsApp tidak terinstall atau error: ${e.message}")
        }
    }
    
    fun makeCall(contact: Contact): ActionResult {
        if (!hasCallPermission()) {
            return ActionResult.PermissionRequired(listOf(Manifest.permission.CALL_PHONE))
        }
        
        return try {
            val intent = Intent(Intent.ACTION_CALL).apply {
                data = Uri.parse("tel:${contact.phoneNumber}")
            }
            context.startActivity(intent)
            ActionResult.Success
        } catch (e: Exception) {
            ActionResult.Error("Error melakukan panggilan: ${e.message}")
        }
    }
    
    fun sendSMS(contact: Contact, message: String): ActionResult {
        return try {
            val intent = Intent(Intent.ACTION_VIEW).apply {
                data = Uri.parse("sms:${contact.phoneNumber}")
                putExtra("sms_body", message)
            }
            context.startActivity(intent)
            ActionResult.Success
        } catch (e: Exception) {
            ActionResult.Error("Error mengirim SMS: ${e.message}")
        }
    }
    
    private fun hasCallPermission(): Boolean {
        return ContextCompat.checkSelfPermission(
            context,
            Manifest.permission.CALL_PHONE
        ) == PackageManager.PERMISSION_GRANTED
    }
}

data class Contact(
    val id: Long,
    val name: String,
    val phoneNumber: String
)

sealed class ContactResult {
    data class Found(val contact: Contact) : ContactResult()
    data class Multiple(val contacts: List<Contact>) : ContactResult()
    data class NotFound(val query: String) : ContactResult()
    object PermissionRequired : ContactResult()
}
```


#### 4.4 System Control Service

```kotlin
class SystemControlService(
    private val context: Context
) {
    private val wifiManager: WifiManager by lazy {
        context.applicationContext.getSystemService(Context.WIFI_SERVICE) as WifiManager
    }
    
    private val bluetoothAdapter: BluetoothAdapter? by lazy {
        BluetoothAdapter.getDefaultAdapter()
    }
    
    private val audioManager: AudioManager by lazy {
        context.getSystemService(Context.AUDIO_SERVICE) as AudioManager
    }
    
    private val cameraManager: CameraManager by lazy {
        context.getSystemService(Context.CAMERA_SERVICE) as CameraManager
    }
    
    fun toggleWiFi(enable: Boolean): ActionResult {
        return try {
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.Q) {
                // Android 10+ requires user to manually toggle WiFi
                val intent = Intent(Settings.Panel.ACTION_WIFI)
                intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK)
                context.startActivity(intent)
                ActionResult.Success
            } else {
                @Suppress("DEPRECATION")
                wifiManager.isWifiEnabled = enable
                ActionResult.Success
            }
        } catch (e: Exception) {
            ActionResult.Error("Error mengatur WiFi: ${e.message}")
        }
    }
    
    fun toggleBluetooth(enable: Boolean): ActionResult {
        if (!hasBluetoothPermission()) {
            return ActionResult.PermissionRequired(listOf(
                Manifest.permission.BLUETOOTH,
                Manifest.permission.BLUETOOTH_ADMIN
            ))
        }
        
        return try {
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.S) {
                // Android 12+ requires user to manually toggle Bluetooth
                val intent = Intent(Settings.ACTION_BLUETOOTH_SETTINGS)
                intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK)
                context.startActivity(intent)
                ActionResult.Success
            } else {
                @Suppress("DEPRECATION")
                if (enable) {
                    bluetoothAdapter?.enable()
                } else {
                    bluetoothAdapter?.disable()
                }
                ActionResult.Success
            }
        } catch (e: Exception) {
            ActionResult.Error("Error mengatur Bluetooth: ${e.message}")
        }
    }
    
    fun toggleFlashlight(enable: Boolean): ActionResult {
        return try {
            val cameraId = cameraManager.cameraIdList[0]
            cameraManager.setTorchMode(cameraId, enable)
            ActionResult.Success
        } catch (e: Exception) {
            ActionResult.Error("Error mengatur flashlight: ${e.message}")
        }
    }
    
    fun setVolume(volumeType: VolumeType, level: Int): ActionResult {
        return try {
            val streamType = when (volumeType) {
                VolumeType.MEDIA -> AudioManager.STREAM_MUSIC
                VolumeType.RINGER -> AudioManager.STREAM_RING
                VolumeType.ALARM -> AudioManager.STREAM_ALARM
            }
            
            val maxVolume = audioManager.getStreamMaxVolume(streamType)
            val targetLevel = (level.coerceIn(0, 100) * maxVolume / 100)
            
            audioManager.setStreamVolume(streamType, targetLevel, 0)
            ActionResult.Success
        } catch (e: Exception) {
            ActionResult.Error("Error mengatur volume: ${e.message}")
        }
    }
    
    fun setBrightness(level: Int): ActionResult {
        if (!hasWriteSettingsPermission()) {
            return ActionResult.PermissionRequired(listOf(Manifest.permission.WRITE_SETTINGS))
        }
        
        return try {
            val brightness = (level.coerceIn(0, 100) * 255 / 100)
            Settings.System.putInt(
                context.contentResolver,
                Settings.System.SCREEN_BRIGHTNESS,
                brightness
            )
            ActionResult.Success
        } catch (e: Exception) {
            ActionResult.Error("Error mengatur brightness: ${e.message}")
        }
    }
    
    fun openSettings(): ActionResult {
        return try {
            val intent = Intent(Settings.ACTION_SETTINGS)
            intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK)
            context.startActivity(intent)
            ActionResult.Success
        } catch (e: Exception) {
            ActionResult.Error("Error membuka Settings: ${e.message}")
        }
    }
    
    private fun hasBluetoothPermission(): Boolean {
        return ContextCompat.checkSelfPermission(
            context,
            Manifest.permission.BLUETOOTH
        ) == PackageManager.PERMISSION_GRANTED
    }
    
    private fun hasWriteSettingsPermission(): Boolean {
        return if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            Settings.System.canWrite(context)
        } else {
            true
        }
    }
}
```


#### 4.5 Media Control Service

```kotlin
class MediaControlService(
    private val context: Context
) {
    private val audioManager: AudioManager by lazy {
        context.getSystemService(Context.AUDIO_SERVICE) as AudioManager
    }
    
    fun controlMedia(action: MediaAction): ActionResult {
        return try {
            val keyCode = when (action) {
                MediaAction.PLAY -> KeyEvent.KEYCODE_MEDIA_PLAY_PAUSE
                MediaAction.PAUSE -> KeyEvent.KEYCODE_MEDIA_PLAY_PAUSE
                MediaAction.NEXT -> KeyEvent.KEYCODE_MEDIA_NEXT
                MediaAction.PREVIOUS -> KeyEvent.KEYCODE_MEDIA_PREVIOUS
            }
            
            val downEvent = KeyEvent(KeyEvent.ACTION_DOWN, keyCode)
            val upEvent = KeyEvent(KeyEvent.ACTION_UP, keyCode)
            
            audioManager.dispatchMediaKeyEvent(downEvent)
            audioManager.dispatchMediaKeyEvent(upEvent)
            
            ActionResult.Success
        } catch (e: Exception) {
            ActionResult.Error("Error mengontrol media: ${e.message}")
        }
    }
    
    fun openMusicApp(appName: String = "spotify"): ActionResult {
        val packageName = when (appName.lowercase()) {
            "spotify" -> "com.spotify.music"
            "youtube music", "yt music" -> "com.google.android.apps.youtube.music"
            "apple music" -> "com.apple.android.music"
            "joox" -> "com.tencent.ibg.joox"
            else -> "com.spotify.music"
        }
        
        return try {
            val intent = context.packageManager.getLaunchIntentForPackage(packageName)
            if (intent != null) {
                intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK)
                context.startActivity(intent)
                ActionResult.Success
            } else {
                ActionResult.Error("App musik '$appName' tidak terinstall")
            }
        } catch (e: Exception) {
            ActionResult.Error("Error membuka app musik: ${e.message}")
        }
    }
}
```

#### 4.6 Search Service

```kotlin
class SearchService(
    private val context: Context
) {
    fun searchGoogle(query: String): ActionResult {
        return try {
            val intent = Intent(Intent.ACTION_WEB_SEARCH).apply {
                putExtra(SearchManager.QUERY, query)
                addFlags(Intent.FLAG_ACTIVITY_NEW_TASK)
            }
            context.startActivity(intent)
            ActionResult.Success
        } catch (e: Exception) {
            // Fallback to browser
            searchInBrowser(query)
        }
    }
    
    fun searchYouTube(query: String): ActionResult {
        return try {
            val intent = Intent(Intent.ACTION_SEARCH).apply {
                setPackage("com.google.android.youtube")
                putExtra("query", query)
                addFlags(Intent.FLAG_ACTIVITY_NEW_TASK)
            }
            context.startActivity(intent)
            ActionResult.Success
        } catch (e: Exception) {
            ActionResult.Error("YouTube tidak terinstall")
        }
    }
    
    fun openUrl(url: String): ActionResult {
        return try {
            val intent = Intent(Intent.ACTION_VIEW).apply {
                data = Uri.parse(url)
                addFlags(Intent.FLAG_ACTIVITY_NEW_TASK)
            }
            context.startActivity(intent)
            ActionResult.Success
        } catch (e: Exception) {
            ActionResult.Error("Error membuka URL: ${e.message}")
        }
    }
    
    private fun searchInBrowser(query: String): ActionResult {
        return try {
            val url = "https://www.google.com/search?q=${Uri.encode(query)}"
            val intent = Intent(Intent.ACTION_VIEW).apply {
                data = Uri.parse(url)
                addFlags(Intent.FLAG_ACTIVITY_NEW_TASK)
            }
            context.startActivity(intent)
            ActionResult.Success
        } catch (e: Exception) {
            ActionResult.Error("Error membuka browser: ${e.message}")
        }
    }
}
```

#### 4.7 Camera Service

```kotlin
class CameraService(
    private val context: Context
) {
    fun takePhoto(): ActionResult {
        if (!hasCameraPermission()) {
            return ActionResult.PermissionRequired(listOf(Manifest.permission.CAMERA))
        }
        
        return try {
            val intent = Intent(MediaStore.ACTION_IMAGE_CAPTURE)
            intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK)
            context.startActivity(intent)
            ActionResult.Success
        } catch (e: Exception) {
            ActionResult.Error("Error membuka kamera: ${e.message}")
        }
    }
    
    fun takeScreenshot(): ActionResult {
        // Note: Taking screenshot programmatically requires system permissions
        // This will guide user to take screenshot manually
        return ActionResult.Error(
            "Untuk screenshot, tekan tombol Power + Volume Down secara bersamaan"
        )
    }
    
    private fun hasCameraPermission(): Boolean {
        return ContextCompat.checkSelfPermission(
            context,
            Manifest.permission.CAMERA
        ) == PackageManager.PERMISSION_GRANTED
    }
}
```


### 5. Data Models and Storage

#### 5.1 Domain Models

```kotlin
// Message Model
data class Message(
    val id: String = UUID.randomUUID().toString(),
    val content: String,
    val isUser: Boolean,
    val timestamp: LocalDateTime = LocalDateTime.now(),
    val action: Action? = null,
    val metadata: MessageMetadata? = null
)

data class MessageMetadata(
    val processingType: ProcessingType,
    val latencyMs: Long,
    val confidence: Float? = null,
    val aiProvider: AIProviderType? = null
)

enum class ProcessingType {
    RULE_BASED,
    AI_ENHANCED,
    ERROR
}

// Task Model
data class Task(
    val id: String = UUID.randomUUID().toString(),
    val title: String,
    val description: String? = null,
    val priority: TaskPriority = TaskPriority.MEDIUM,
    val status: TaskStatus = TaskStatus.PENDING,
    val dueDate: LocalDateTime? = null,
    val createdAt: LocalDateTime = LocalDateTime.now(),
    val completedAt: LocalDateTime? = null,
    val tags: List<String> = emptyList()
)

enum class TaskPriority {
    LOW, MEDIUM, HIGH
}

enum class TaskStatus {
    PENDING, IN_PROGRESS, COMPLETED, CANCELLED
}

// Schedule Model
data class Schedule(
    val id: String = UUID.randomUUID().toString(),
    val title: String,
    val description: String? = null,
    val startTime: LocalDateTime,
    val endTime: LocalDateTime,
    val location: String? = null,
    val reminder: ReminderConfig? = null,
    val createdAt: LocalDateTime = LocalDateTime.now()
)

data class ReminderConfig(
    val minutesBefore: Int,
    val enabled: Boolean = true
)

// Weather Model
data class WeatherData(
    val location: String,
    val temperature: Double,
    val feelsLike: Double,
    val condition: String,
    val humidity: Int,
    val windSpeed: Double,
    val forecast: List<WeatherForecast>,
    val timestamp: LocalDateTime = LocalDateTime.now()
)

data class WeatherForecast(
    val date: LocalDate,
    val tempMin: Double,
    val tempMax: Double,
    val condition: String
)

// News Model
data class NewsArticle(
    val id: String,
    val title: String,
    val description: String,
    val url: String,
    val source: String,
    val category: String,
    val publishedAt: LocalDateTime,
    val imageUrl: String?
)

// Action Model
sealed class Action {
    object AddTask : Action()
    object CompleteTask : Action()
    object ListTasks : Action()
    data class OpenApp(val appName: String) : Action()
    data class SendMessage(val contactName: String, val message: String) : Action()
    data class MakeCall(val contactName: String) : Action()
    data class SearchWeb(val query: String) : Action()
    data class SetAlarm(val time: LocalTime) : Action()
    data class ControlMedia(val action: MediaAction) : Action()
}
```


#### 5.2 Database Schema (Room)

```kotlin
// Message Entity
@Entity(tableName = "messages")
data class MessageEntity(
    @PrimaryKey val id: String,
    @ColumnInfo(name = "content") val content: String,
    @ColumnInfo(name = "is_user") val isUser: Boolean,
    @ColumnInfo(name = "timestamp") val timestamp: Long,
    @ColumnInfo(name = "action_type") val actionType: String?,
    @ColumnInfo(name = "processing_type") val processingType: String,
    @ColumnInfo(name = "latency_ms") val latencyMs: Long,
    @ColumnInfo(name = "confidence") val confidence: Float?,
    @ColumnInfo(name = "ai_provider") val aiProvider: String?
)

@Dao
interface MessageDao {
    @Query("SELECT * FROM messages ORDER BY timestamp DESC LIMIT :limit")
    suspend fun getRecentMessages(limit: Int = 50): List<MessageEntity>
    
    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertMessage(message: MessageEntity)
    
    @Query("DELETE FROM messages")
    suspend fun clearAll()
    
    @Query("SELECT * FROM messages WHERE content LIKE '%' || :query || '%'")
    suspend fun searchMessages(query: String): List<MessageEntity>
}

// Task Entity
@Entity(tableName = "tasks")
data class TaskEntity(
    @PrimaryKey val id: String,
    @ColumnInfo(name = "title") val title: String,
    @ColumnInfo(name = "description") val description: String?,
    @ColumnInfo(name = "priority") val priority: String,
    @ColumnInfo(name = "status") val status: String,
    @ColumnInfo(name = "due_date") val dueDate: Long?,
    @ColumnInfo(name = "created_at") val createdAt: Long,
    @ColumnInfo(name = "completed_at") val completedAt: Long?,
    @ColumnInfo(name = "tags") val tags: String // JSON array
)

@Dao
interface TaskDao {
    @Query("SELECT * FROM tasks WHERE status != 'COMPLETED' ORDER BY priority DESC, due_date ASC")
    suspend fun getActiveTasks(): List<TaskEntity>
    
    @Query("SELECT * FROM tasks WHERE status = 'COMPLETED' ORDER BY completed_at DESC LIMIT 20")
    suspend fun getCompletedTasks(): List<TaskEntity>
    
    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertTask(task: TaskEntity)
    
    @Update
    suspend fun updateTask(task: TaskEntity)
    
    @Delete
    suspend fun deleteTask(task: TaskEntity)
    
    @Query("SELECT * FROM tasks WHERE id = :taskId")
    suspend fun getTaskById(taskId: String): TaskEntity?
}

// Schedule Entity
@Entity(tableName = "schedules")
data class ScheduleEntity(
    @PrimaryKey val id: String,
    @ColumnInfo(name = "title") val title: String,
    @ColumnInfo(name = "description") val description: String?,
    @ColumnInfo(name = "start_time") val startTime: Long,
    @ColumnInfo(name = "end_time") val endTime: Long,
    @ColumnInfo(name = "location") val location: String?,
    @ColumnInfo(name = "reminder_minutes") val reminderMinutes: Int?,
    @ColumnInfo(name = "reminder_enabled") val reminderEnabled: Boolean,
    @ColumnInfo(name = "created_at") val createdAt: Long
)

@Dao
interface ScheduleDao {
    @Query("SELECT * FROM schedules WHERE start_time >= :startOfDay AND start_time < :endOfDay ORDER BY start_time ASC")
    suspend fun getSchedulesForDay(startOfDay: Long, endOfDay: Long): List<ScheduleEntity>
    
    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertSchedule(schedule: ScheduleEntity)
    
    @Update
    suspend fun updateSchedule(schedule: ScheduleEntity)
    
    @Delete
    suspend fun deleteSchedule(schedule: ScheduleEntity)
    
    @Query("SELECT * FROM schedules WHERE start_time > :currentTime ORDER BY start_time ASC LIMIT 5")
    suspend fun getUpcomingSchedules(currentTime: Long): List<ScheduleEntity>
}

// Config Entity
@Entity(tableName = "config")
data class ConfigEntity(
    @PrimaryKey val key: String,
    @ColumnInfo(name = "value") val value: String
)

@Dao
interface ConfigDao {
    @Query("SELECT * FROM config WHERE key = :key")
    suspend fun getConfig(key: String): ConfigEntity?
    
    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun setConfig(config: ConfigEntity)
    
    @Query("DELETE FROM config WHERE key = :key")
    suspend fun deleteConfig(key: String)
}

// Database
@Database(
    entities = [
        MessageEntity::class,
        TaskEntity::class,
        ScheduleEntity::class,
        ConfigEntity::class
    ],
    version = 1,
    exportSchema = false
)
abstract class ViraDatabase : RoomDatabase() {
    abstract fun messageDao(): MessageDao
    abstract fun taskDao(): TaskDao
    abstract fun scheduleDao(): ScheduleDao
    abstract fun configDao(): ConfigDao
}
```


#### 5.3 Secure Storage for API Keys

```kotlin
class SecureStorageManager(
    private val context: Context
) {
    private val keyStore: KeyStore = KeyStore.getInstance("AndroidKeyStore").apply {
        load(null)
    }
    
    private val encryptedPrefs: SharedPreferences by lazy {
        val masterKey = MasterKey.Builder(context)
            .setKeyScheme(MasterKey.KeyScheme.AES256_GCM)
            .build()
        
        EncryptedSharedPreferences.create(
            context,
            "vira_secure_prefs",
            masterKey,
            EncryptedSharedPreferences.PrefKeyEncryptionScheme.AES256_SIV,
            EncryptedSharedPreferences.PrefValueEncryptionScheme.AES256_GCM
        )
    }
    
    fun saveApiKey(provider: AIProviderType, apiKey: String) {
        encryptedPrefs.edit()
            .putString("api_key_${provider.name}", apiKey)
            .apply()
    }
    
    fun getApiKey(provider: AIProviderType): String? {
        return encryptedPrefs.getString("api_key_${provider.name}", null)
    }
    
    fun deleteApiKey(provider: AIProviderType) {
        encryptedPrefs.edit()
            .remove("api_key_${provider.name}")
            .apply()
    }
    
    fun saveAIConfig(config: AIConfig) {
        saveApiKey(config.provider, config.apiKey)
        encryptedPrefs.edit()
            .putString("ai_provider", config.provider.name)
            .putString("ai_model", config.model)
            .putFloat("ai_temperature", config.temperature)
            .putInt("ai_max_tokens", config.maxTokens)
            .putBoolean("ai_configured", config.isConfigured)
            .apply()
    }
    
    fun getAIConfig(): AIConfig? {
        val providerName = encryptedPrefs.getString("ai_provider", null) ?: return null
        val provider = AIProviderType.valueOf(providerName)
        val apiKey = getApiKey(provider) ?: return null
        
        return AIConfig(
            provider = provider,
            apiKey = apiKey,
            model = encryptedPrefs.getString("ai_model", getDefaultModel(provider))!!,
            temperature = encryptedPrefs.getFloat("ai_temperature", 0.9f),
            maxTokens = encryptedPrefs.getInt("ai_max_tokens", 1024),
            isConfigured = encryptedPrefs.getBoolean("ai_configured", false)
        )
    }
    
    fun clearAIConfig() {
        AIProviderType.values().forEach { provider ->
            deleteApiKey(provider)
        }
        encryptedPrefs.edit()
            .remove("ai_provider")
            .remove("ai_model")
            .remove("ai_temperature")
            .remove("ai_max_tokens")
            .remove("ai_configured")
            .apply()
    }
    
    private fun getDefaultModel(provider: AIProviderType): String {
        return when (provider) {
            AIProviderType.GROQ -> "llama-3.3-70b-versatile"
            AIProviderType.GEMINI -> "gemini-2.0-flash"
            AIProviderType.OPENAI -> "gpt-4o-mini"
        }
    }
}
```

---

## 🔐 Security Implementation

### API Key Security

1. **Storage**: Android Keystore + EncryptedSharedPreferences
2. **Transmission**: HTTPS only, no logging
3. **Access**: Only accessible by app process
4. **Validation**: Keys validated before storage
5. **Deletion**: Secure deletion when user removes key

### Permission Handling Strategy

```kotlin
class PermissionManager(
    private val activity: ComponentActivity
) {
    private val permissionLauncher = activity.registerForActivityResult(
        ActivityResultContracts.RequestMultiplePermissions()
    ) { permissions ->
        permissions.entries.forEach { entry ->
            val permission = entry.key
            val isGranted = entry.value
            
            if (isGranted) {
                onPermissionGranted(permission)
            } else {
                onPermissionDenied(permission)
            }
        }
    }
    
    fun requestPermissions(permissions: List<String>, rationale: String) {
        val permissionsToRequest = permissions.filter { permission ->
            ContextCompat.checkSelfPermission(
                activity,
                permission
            ) != PackageManager.PERMISSION_GRANTED
        }
        
        if (permissionsToRequest.isEmpty()) {
            // All permissions already granted
            onAllPermissionsGranted()
            return
        }
        
        // Show rationale if needed
        val shouldShowRationale = permissionsToRequest.any { permission ->
            ActivityCompat.shouldShowRequestPermissionRationale(activity, permission)
        }
        
        if (shouldShowRationale) {
            showPermissionRationale(rationale) {
                permissionLauncher.launch(permissionsToRequest.toTypedArray())
            }
        } else {
            permissionLauncher.launch(permissionsToRequest.toTypedArray())
        }
    }
    
    fun checkPermission(permission: String): Boolean {
        return ContextCompat.checkSelfPermission(
            activity,
            permission
        ) == PackageManager.PERMISSION_GRANTED
    }
    
    private fun showPermissionRationale(rationale: String, onAccept: () -> Unit) {
        // Show dialog explaining why permission is needed
        AlertDialog.Builder(activity)
            .setTitle("Izin Diperlukan")
            .setMessage(rationale)
            .setPositiveButton("Izinkan") { _, _ -> onAccept() }
            .setNegativeButton("Tidak") { dialog, _ -> dialog.dismiss() }
            .show()
    }
    
    private fun onPermissionGranted(permission: String) {
        // Handle permission granted
    }
    
    private fun onPermissionDenied(permission: String) {
        // Show message about limited functionality
        showPermissionDeniedMessage(permission)
    }
    
    private fun onAllPermissionsGranted() {
        // All permissions granted, proceed with action
    }
    
    private fun showPermissionDeniedMessage(permission: String) {
        val message = when (permission) {
            Manifest.permission.READ_CONTACTS -> 
                "Tanpa izin kontak, VIRA tidak dapat mengenali nama kontak untuk panggilan atau pesan."
            Manifest.permission.CALL_PHONE -> 
                "Tanpa izin panggilan, VIRA tidak dapat melakukan panggilan telepon otomatis."
            Manifest.permission.CAMERA -> 
                "Tanpa izin kamera, VIRA tidak dapat mengambil foto."
            else -> 
                "Beberapa fitur mungkin tidak tersedia tanpa izin ini."
        }
        
        Toast.makeText(activity, message, Toast.LENGTH_LONG).show()
    }
}
```


---

## 🎨 UI Layer Design

### Screen Structure

VIRA memiliki 4 layar utama:

1. **ChatScreen** (Main): Layar utama untuk interaksi dengan VIRA
2. **SettingsScreen**: Konfigurasi AI provider, API keys, preferences
3. **TasksScreen**: Manajemen task dan reminder
4. **HistoryScreen**: Riwayat percakapan dan analytics

### Navigation Graph

```
ChatScreen (Start Destination)
    ├─→ SettingsScreen
    ├─→ TasksScreen
    └─→ HistoryScreen
```

### Jetpack Compose Architecture

```kotlin
// Main Screen
@Composable
fun ViraScreen(
    viewModel: ViraViewModel = hiltViewModel()
) {
    val uiState by viewModel.uiState.collectAsState()
    
    Scaffold(
        topBar = { ViraTopBar(onSettingsClick = { /* navigate to settings */ }) },
        bottomBar = { ViraInputBar(onSendMessage = viewModel::sendMessage) }
    ) { paddingValues ->
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(paddingValues)
        ) {
            QuickActionsPanel(
                actions = uiState.quickActions,
                onActionClick = viewModel::executeQuickAction
            )
            
            ChatMessageList(
                messages = uiState.messages,
                modifier = Modifier.weight(1f)
            )
        }
    }
}

// ViewModel
@HiltViewModel
class ViraViewModel @Inject constructor(
    private val processMessageUseCase: ProcessMessageUseCase,
    private val getQuickActionsUseCase: GetQuickActionsUseCase,
    private val executeActionUseCase: ExecuteActionUseCase
) : ViewModel() {
    
    private val _uiState = MutableStateFlow(ViraUiState())
    val uiState: StateFlow<ViraUiState> = _uiState.asStateFlow()
    
    init {
        loadQuickActions()
        loadRecentMessages()
    }
    
    fun sendMessage(text: String) {
        viewModelScope.launch {
            // Add user message
            val userMessage = Message(
                content = text,
                isUser = true
            )
            _uiState.update { it.copy(
                messages = it.messages + userMessage,
                isProcessing = true
            )}
            
            // Process message
            val context = buildContext()
            val result = processMessageUseCase(text, context)
            
            // Add assistant response
            val assistantMessage = when (result) {
                is ProcessingResult.RuleBased -> Message(
                    content = result.response,
                    isUser = false,
                    action = result.action,
                    metadata = MessageMetadata(
                        processingType = ProcessingType.RULE_BASED,
                        latencyMs = 0,
                        confidence = result.confidence
                    )
                )
                is ProcessingResult.AIEnhanced -> Message(
                    content = result.response,
                    isUser = false,
                    action = result.action,
                    metadata = MessageMetadata(
                        processingType = ProcessingType.AI_ENHANCED,
                        latencyMs = 0,
                        aiProvider = result.provider
                    )
                )
                is ProcessingResult.Error -> Message(
                    content = result.fallbackResponse ?: "Maaf, saya tidak mengerti.",
                    isUser = false,
                    metadata = MessageMetadata(
                        processingType = ProcessingType.ERROR,
                        latencyMs = 0
                    )
                )
            }
            
            _uiState.update { it.copy(
                messages = it.messages + assistantMessage,
                isProcessing = false
            )}
            
            // Execute action if present
            assistantMessage.action?.let { action ->
                executeActionUseCase(action)
            }
        }
    }
    
    fun executeQuickAction(action: QuickAction) {
        sendMessage(action.command)
    }
    
    private fun loadQuickActions() {
        viewModelScope.launch {
            val actions = getQuickActionsUseCase()
            _uiState.update { it.copy(quickActions = actions) }
        }
    }
    
    private fun loadRecentMessages() {
        // Load from database
    }
    
    private fun buildContext(): ConversationContext {
        return ConversationContext(
            userId = "default_user",
            sessionId = UUID.randomUUID().toString(),
            previousMessages = _uiState.value.messages.takeLast(5),
            currentTime = LocalDateTime.now(),
            location = null,
            deviceState = DeviceState()
        )
    }
}

data class ViraUiState(
    val messages: List<Message> = emptyList(),
    val quickActions: List<QuickAction> = emptyList(),
    val isProcessing: Boolean = false,
    val error: String? = null
)

data class QuickAction(
    val id: String,
    val label: String,
    val icon: ImageVector,
    val command: String
)
```

### UI Components

```kotlin
@Composable
fun ChatMessageList(
    messages: List<Message>,
    modifier: Modifier = Modifier
) {
    LazyColumn(
        modifier = modifier,
        reverseLayout = true,
        contentPadding = PaddingValues(16.dp),
        verticalArrangement = Arrangement.spacedBy(8.dp)
    ) {
        items(messages.reversed()) { message ->
            ChatMessageItem(message = message)
        }
    }
}

@Composable
fun ChatMessageItem(message: Message) {
    Row(
        modifier = Modifier.fillMaxWidth(),
        horizontalArrangement = if (message.isUser) Arrangement.End else Arrangement.Start
    ) {
        Card(
            colors = CardDefaults.cardColors(
                containerColor = if (message.isUser) 
                    MaterialTheme.colorScheme.primaryContainer 
                else 
                    MaterialTheme.colorScheme.secondaryContainer
            ),
            modifier = Modifier.widthIn(max = 280.dp)
        ) {
            Column(
                modifier = Modifier.padding(12.dp)
            ) {
                Text(
                    text = message.content,
                    style = MaterialTheme.typography.bodyMedium
                )
                
                Spacer(modifier = Modifier.height(4.dp))
                
                Text(
                    text = message.timestamp.format(DateTimeFormatter.ofPattern("HH:mm")),
                    style = MaterialTheme.typography.labelSmall,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
                
                // Show metadata for debugging
                message.metadata?.let { metadata ->
                    Text(
                        text = "${metadata.processingType} • ${metadata.latencyMs}ms",
                        style = MaterialTheme.typography.labelSmall,
                        color = MaterialTheme.colorScheme.onSurfaceVariant
                    )
                }
            }
        }
    }
}

@Composable
fun QuickActionsPanel(
    actions: List<QuickAction>,
    onActionClick: (QuickAction) -> Unit
) {
    LazyRow(
        contentPadding = PaddingValues(horizontal = 16.dp, vertical = 8.dp),
        horizontalArrangement = Arrangement.spacedBy(8.dp)
    ) {
        items(actions) { action ->
            QuickActionButton(
                action = action,
                onClick = { onActionClick(action) }
            )
        }
    }
}

@Composable
fun QuickActionButton(
    action: QuickAction,
    onClick: () -> Unit
) {
    OutlinedButton(
        onClick = onClick,
        contentPadding = PaddingValues(horizontal = 16.dp, vertical = 8.dp)
    ) {
        Icon(
            imageVector = action.icon,
            contentDescription = action.label,
            modifier = Modifier.size(18.dp)
        )
        Spacer(modifier = Modifier.width(8.dp))
        Text(text = action.label)
    }
}

@Composable
fun ViraInputBar(
    onSendMessage: (String) -> Unit
) {
    var text by remember { mutableStateOf("") }
    var isRecording by remember { mutableStateOf(false) }
    
    Surface(
        tonalElevation = 3.dp
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .padding(8.dp),
            verticalAlignment = Alignment.CenterVertically
        ) {
            IconButton(
                onClick = { isRecording = !isRecording }
            ) {
                Icon(
                    imageVector = if (isRecording) Icons.Default.Stop else Icons.Default.Mic,
                    contentDescription = "Voice input",
                    tint = if (isRecording) MaterialTheme.colorScheme.error else MaterialTheme.colorScheme.primary
                )
            }
            
            OutlinedTextField(
                value = text,
                onValueChange = { text = it },
                modifier = Modifier.weight(1f),
                placeholder = { Text("Tanya VIRA...") },
                maxLines = 3,
                keyboardOptions = KeyboardOptions(
                    imeAction = ImeAction.Send
                ),
                keyboardActions = KeyboardActions(
                    onSend = {
                        if (text.isNotBlank()) {
                            onSendMessage(text)
                            text = ""
                        }
                    }
                )
            )
            
            IconButton(
                onClick = {
                    if (text.isNotBlank()) {
                        onSendMessage(text)
                        text = ""
                    }
                },
                enabled = text.isNotBlank()
            ) {
                Icon(
                    imageVector = Icons.AutoMirrored.Filled.Send,
                    contentDescription = "Send"
                )
            }
        }
    }
}
```

### Additional UI Screens

#### 1. Settings Screen

```kotlin
@Composable
fun SettingsScreen(
    viewModel: SettingsViewModel = hiltViewModel(),
    onNavigateBack: () -> Unit
) {
    val uiState by viewModel.uiState.collectAsState()
    
    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Pengaturan") },
                navigationIcon = {
                    IconButton(onClick = onNavigateBack) {
                        Icon(Icons.Default.ArrowBack, "Back")
                    }
                }
            )
        }
    ) { paddingValues ->
        LazyColumn(
            modifier = Modifier
                .fillMaxSize()
                .padding(paddingValues)
        ) {
            // AI Provider Section
            item {
                SettingsSection(title = "AI Provider") {
                    AIProviderSelector(
                        selectedProvider = uiState.aiConfig?.provider,
                        onProviderSelected = viewModel::selectProvider
                    )
                    
                    if (uiState.aiConfig?.provider != null) {
                        APIKeyInput(
                            provider = uiState.aiConfig!!.provider,
                            currentKey = uiState.aiConfig!!.apiKey,
                            onKeyChanged = viewModel::updateApiKey,
                            onValidate = viewModel::validateApiKey
                        )
                        
                        ModelSelector(
                            provider = uiState.aiConfig!!.provider,
                            selectedModel = uiState.aiConfig!!.model,
                            onModelSelected = viewModel::selectModel
                        )
                    }
                }
            }
            
            // Voice Settings Section
            item {
                SettingsSection(title = "Suara") {
                    SwitchPreference(
                        title = "Text-to-Speech",
                        subtitle = "Aktifkan respons suara",
                        checked = uiState.ttsEnabled,
                        onCheckedChange = viewModel::toggleTTS
                    )
                    
                    SwitchPreference(
                        title = "Voice Input",
                        subtitle = "Aktifkan input suara",
                        checked = uiState.voiceInputEnabled,
                        onCheckedChange = viewModel::toggleVoiceInput
                    )
                }
            }
            
            // Notification Settings
            item {
                SettingsSection(title = "Notifikasi") {
                    SwitchPreference(
                        title = "Proactive Suggestions",
                        subtitle = "Saran otomatis dari VIRA",
                        checked = uiState.proactiveSuggestionsEnabled,
                        onCheckedChange = viewModel::toggleProactiveSuggestions
                    )
                    
                    SwitchPreference(
                        title = "Task Reminders",
                        subtitle = "Pengingat untuk task",
                        checked = uiState.taskRemindersEnabled,
                        onCheckedChange = viewModel::toggleTaskReminders
                    )
                }
            }
            
            // Privacy Section
            item {
                SettingsSection(title = "Privasi") {
                    TextButton(
                        onClick = viewModel::clearHistory,
                        modifier = Modifier.fillMaxWidth()
                    ) {
                        Text("Hapus Riwayat Percakapan")
                    }
                    
                    TextButton(
                        onClick = viewModel::clearAllData,
                        modifier = Modifier.fillMaxWidth()
                    ) {
                        Text("Hapus Semua Data", color = MaterialTheme.colorScheme.error)
                    }
                }
            }
            
            // About Section
            item {
                SettingsSection(title = "Tentang") {
                    InfoRow("Versi", "1.0.0")
                    InfoRow("Build", "2024.01.01")
                }
            }
        }
    }
}

@Composable
fun AIProviderSelector(
    selectedProvider: AIProviderType?,
    onProviderSelected: (AIProviderType) -> Unit
) {
    Column {
        Text("Pilih AI Provider", style = MaterialTheme.typography.titleSmall)
        Spacer(modifier = Modifier.height(8.dp))
        
        AIProviderType.values().forEach { provider ->
            Row(
                modifier = Modifier
                    .fillMaxWidth()
                    .clickable { onProviderSelected(provider) }
                    .padding(vertical = 8.dp),
                verticalAlignment = Alignment.CenterVertically
            ) {
                RadioButton(
                    selected = selectedProvider == provider,
                    onClick = { onProviderSelected(provider) }
                )
                Spacer(modifier = Modifier.width(8.dp))
                Column {
                    Text(provider.name, style = MaterialTheme.typography.bodyLarge)
                    Text(
                        getProviderDescription(provider),
                        style = MaterialTheme.typography.bodySmall,
                        color = MaterialTheme.colorScheme.onSurfaceVariant
                    )
                }
            }
        }
    }
}

@Composable
fun APIKeyInput(
    provider: AIProviderType,
    currentKey: String,
    onKeyChanged: (String) -> Unit,
    onValidate: () -> Unit
) {
    var key by remember { mutableStateOf(currentKey) }
    var showKey by remember { mutableStateOf(false) }
    
    Column {
        Text("API Key", style = MaterialTheme.typography.titleSmall)
        Spacer(modifier = Modifier.height(8.dp))
        
        OutlinedTextField(
            value = key,
            onValueChange = { key = it },
            modifier = Modifier.fillMaxWidth(),
            placeholder = { Text("Masukkan API key ${provider.name}") },
            visualTransformation = if (showKey) VisualTransformation.None else PasswordVisualTransformation(),
            trailingIcon = {
                IconButton(onClick = { showKey = !showKey }) {
                    Icon(
                        imageVector = if (showKey) Icons.Default.VisibilityOff else Icons.Default.Visibility,
                        contentDescription = "Toggle visibility"
                    )
                }
            }
        )
        
        Spacer(modifier = Modifier.height(8.dp))
        
        Row {
            Button(
                onClick = {
                    onKeyChanged(key)
                    onValidate()
                },
                modifier = Modifier.weight(1f)
            ) {
                Text("Simpan & Validasi")
            }
        }
    }
}
```

#### 2. Tasks Screen

```kotlin
@Composable
fun TasksScreen(
    viewModel: TasksViewModel = hiltViewModel(),
    onNavigateBack: () -> Unit
) {
    val uiState by viewModel.uiState.collectAsState()
    var showAddDialog by remember { mutableStateOf(false) }
    
    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Task & Reminder") },
                navigationIcon = {
                    IconButton(onClick = onNavigateBack) {
                        Icon(Icons.Default.ArrowBack, "Back")
                    }
                }
            )
        },
        floatingActionButton = {
            FloatingActionButton(onClick = { showAddDialog = true }) {
                Icon(Icons.Default.Add, "Add task")
            }
        }
    ) { paddingValues ->
        LazyColumn(
            modifier = Modifier
                .fillMaxSize()
                .padding(paddingValues)
        ) {
            // Active Tasks
            item {
                Text(
                    "Task Aktif",
                    style = MaterialTheme.typography.titleMedium,
                    modifier = Modifier.padding(16.dp)
                )
            }
            
            items(uiState.activeTasks) { task ->
                TaskItem(
                    task = task,
                    onToggleComplete = { viewModel.toggleTaskComplete(task.id) },
                    onDelete = { viewModel.deleteTask(task.id) }
                )
            }
            
            // Completed Tasks
            if (uiState.completedTasks.isNotEmpty()) {
                item {
                    Text(
                        "Task Selesai",
                        style = MaterialTheme.typography.titleMedium,
                        modifier = Modifier.padding(16.dp)
                    )
                }
                
                items(uiState.completedTasks) { task ->
                    TaskItem(
                        task = task,
                        onToggleComplete = { viewModel.toggleTaskComplete(task.id) },
                        onDelete = { viewModel.deleteTask(task.id) }
                    )
                }
            }
        }
    }
    
    if (showAddDialog) {
        AddTaskDialog(
            onDismiss = { showAddDialog = false },
            onAdd = { title, priority, dueDate ->
                viewModel.addTask(title, priority, dueDate)
                showAddDialog = false
            }
        )
    }
}

@Composable
fun TaskItem(
    task: Task,
    onToggleComplete: () -> Unit,
    onDelete: () -> Unit
) {
    Card(
        modifier = Modifier
            .fillMaxWidth()
            .padding(horizontal = 16.dp, vertical = 4.dp)
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .padding(12.dp),
            verticalAlignment = Alignment.CenterVertically
        ) {
            Checkbox(
                checked = task.status == TaskStatus.COMPLETED,
                onCheckedChange = { onToggleComplete() }
            )
            
            Column(modifier = Modifier.weight(1f)) {
                Text(
                    text = task.title,
                    style = MaterialTheme.typography.bodyLarge,
                    textDecoration = if (task.status == TaskStatus.COMPLETED) 
                        TextDecoration.LineThrough else null
                )
                
                task.dueDate?.let { dueDate ->
                    Text(
                        text = dueDate.format(DateTimeFormatter.ofPattern("dd MMM yyyy, HH:mm")),
                        style = MaterialTheme.typography.bodySmall,
                        color = MaterialTheme.colorScheme.onSurfaceVariant
                    )
                }
                
                // Priority badge
                PriorityBadge(priority = task.priority)
            }
            
            IconButton(onClick = onDelete) {
                Icon(Icons.Default.Delete, "Delete", tint = MaterialTheme.colorScheme.error)
            }
        }
    }
}
```

#### 3. History Screen

```kotlin
@Composable
fun HistoryScreen(
    viewModel: HistoryViewModel = hiltViewModel(),
    onNavigateBack: () -> Unit
) {
    val uiState by viewModel.uiState.collectAsState()
    var searchQuery by remember { mutableStateOf("") }
    
    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Riwayat") },
                navigationIcon = {
                    IconButton(onClick = onNavigateBack) {
                        Icon(Icons.Default.ArrowBack, "Back")
                    }
                },
                actions = {
                    IconButton(onClick = viewModel::clearHistory) {
                        Icon(Icons.Default.Delete, "Clear history")
                    }
                }
            )
        }
    ) { paddingValues ->
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(paddingValues)
        ) {
            // Search bar
            OutlinedTextField(
                value = searchQuery,
                onValueChange = {
                    searchQuery = it
                    viewModel.searchMessages(it)
                },
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(16.dp),
                placeholder = { Text("Cari percakapan...") },
                leadingIcon = {
                    Icon(Icons.Default.Search, "Search")
                }
            )
            
            // Message history
            LazyColumn(
                modifier = Modifier.weight(1f)
            ) {
                items(uiState.messages) { message ->
                    HistoryMessageItem(message = message)
                }
            }
        }
    }
}
```

### UI Components Summary

**Layar yang Sudah Dirancang**:
1. ✅ ChatScreen - Layar utama chat dengan VIRA
2. ✅ SettingsScreen - Konfigurasi AI, suara, notifikasi, privasi
3. ✅ TasksScreen - Manajemen task dan reminder
4. ✅ HistoryScreen - Riwayat percakapan dengan search

**Komponen UI yang Sudah Dirancang**:
1. ✅ ChatMessageList - Daftar pesan chat
2. ✅ ChatMessageItem - Item pesan individual
3. ✅ QuickActionsPanel - Panel quick action buttons
4. ✅ ViraInputBar - Input bar dengan voice dan text
5. ✅ AIProviderSelector - Pemilih AI provider
6. ✅ APIKeyInput - Input API key dengan validasi
7. ✅ TaskItem - Item task dengan checkbox dan priority
8. ✅ AddTaskDialog - Dialog untuk menambah task
9. ✅ SettingsSection - Section untuk grouping settings
10. ✅ SwitchPreference - Toggle switch untuk settings

**Yang Masih Perlu Ditambahkan** (Optional/Future):
1. ⏳ OnboardingScreen - Tutorial pertama kali
2. ⏳ PermissionExplanationDialog - Penjelasan permission
3. ⏳ WeatherCard - Rich card untuk cuaca
4. ⏳ NewsCard - Rich card untuk berita
5. ⏳ ScheduleCard - Rich card untuk jadwal
6. ⏳ AnalyticsScreen - Statistik dan insights
7. ⏳ QuickActionCustomizer - Customize quick actions
8. ⏳ VoiceWaveformAnimation - Animasi saat recording

Semua UI utama sudah dirancang dengan Jetpack Compose. Yang masih optional adalah rich cards dan analytics yang bisa ditambahkan di fase berikutnya.
```


---

## ✅ Correctness Properties

A property is a characteristic or behavior that should hold true across all valid executions of a system—essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.

### Property 1: TTS Triggering for Important Messages
*For any* message marked with `speak=true`, the system SHALL trigger text-to-speech playback
**Validates: Requirements AC-1.4**

### Property 2: Task Creation from Commands
*For any* valid task creation command (voice or text), the system SHALL create a task entry in the database with the specified title
**Validates: Requirements AC-2.1**

### Property 3: Today's Task Filtering
*For any* query for today's tasks, the system SHALL return only tasks with due dates matching the current date
**Validates: Requirements AC-2.2**

### Property 4: Task Completion Updates
*For any* task marked as complete, the system SHALL update its status to COMPLETED and set the completedAt timestamp
**Validates: Requirements AC-2.3**

### Property 5: Reminder Generation for Due Tasks
*For any* task with a due date and reminder enabled, the system SHALL generate a reminder notification at the specified time before the due date
**Validates: Requirements AC-2.4**

### Property 6: Weather Pattern Matching
*For any* input containing weather-related keywords (cuaca, weather, suhu, hujan), the system SHALL match the weather query pattern
**Validates: Requirements AC-3.1**

### Property 7: News Category Filtering
*For any* news query with a specified category, the system SHALL return only articles from that category
**Validates: Requirements AC-3.2**

### Property 8: Schedule Retrieval for Current Day
*For any* schedule query for today, the system SHALL return only appointments with start times within the current day
**Validates: Requirements AC-3.4**

### Property 9: Response Formatting Consistency
*For any* information response (weather, news, schedule), the system SHALL include structured formatting elements (title, content, timestamp)
**Validates: Requirements AC-3.5**

### Property 10: Time-Based Greeting Selection
*For any* greeting request, the system SHALL select a greeting appropriate to the current time of day (pagi for 05:00-11:00, siang for 11:00-15:00, sore for 15:00-18:00, malam for 18:00-05:00)
**Validates: Requirements AC-4.1**

### Property 11: Incomplete Task Reminders
*For any* task with status PENDING or IN_PROGRESS at end of day (20:00), the system SHALL include it in the evening reminder
**Validates: Requirements AC-4.3**

### Property 12: App Launch Pattern Matching
*For any* input matching the open app pattern, the system SHALL extract the app name and trigger the app launcher service
**Validates: Requirements AC-11.1**

### Property 13: WhatsApp Message Parsing
*For any* WhatsApp send command, the system SHALL correctly parse both the contact name and message content
**Validates: Requirements AC-11.2**

### Property 14: Call Intent Triggering
*For any* call command with a valid contact, the system SHALL trigger the phone call intent with the correct phone number
**Validates: Requirements AC-11.3**

### Property 15: Search Query Extraction
*For any* search command, the system SHALL extract the search query and trigger the appropriate search intent
**Validates: Requirements AC-11.4**

### Property 16: Alarm Time Parsing
*For any* alarm command with time specification, the system SHALL correctly parse the hour and minute values
**Validates: Requirements AC-11.5**

### Property 17: Media Control Command Mapping
*For any* media control command (play, pause, next, previous), the system SHALL map it to the correct media key event
**Validates: Requirements AC-11.6**

### Property 18: Volume Level Normalization
*For any* volume command with a percentage level (0-100), the system SHALL normalize it to the device's volume range
**Validates: Requirements AC-11.8**

### Property 19: Permission Request Triggering
*For any* action requiring permissions that are not granted, the system SHALL trigger a permission request before attempting the action
**Validates: Requirements AC-11.9**

### Property 20: Permission Denial Handling
*For any* action where required permissions are denied, the system SHALL return an error result with an appropriate message
**Validates: Requirements AC-11.10**

### Property 21: Contact Name Extraction
*For any* command containing a contact reference, the system SHALL extract the contact name for lookup
**Validates: Requirements AC-12.2**

### Property 22: WhatsApp Contact Resolution
*For any* WhatsApp command with a contact name, the system SHALL resolve the name to a contact and trigger WhatsApp with the correct phone number
**Validates: Requirements AC-12.3**

### Property 23: Ambiguous Contact Handling
*For any* contact search returning multiple matches, the system SHALL return a Multiple result with all matching contacts
**Validates: Requirements AC-12.6**

### Property 24: Fuzzy Contact Matching
*For any* contact search with partial or misspelled name, the system SHALL find contacts where the display name contains the search query (case-insensitive)
**Validates: Requirements AC-12.7**

### Property 25: API Key Secure Storage
*For any* API key saved for a provider, the system SHALL store it in encrypted shared preferences and retrieve the same value
**Validates: Requirements AC-16.2**

### Property 26: API Key Validation
*For any* API key submitted for validation, the system SHALL make a test API call and return Valid only if the call succeeds
**Validates: Requirements AC-16.3**

### Property 27: API Connection Status
*For any* configured AI provider, the system SHALL report connection status as connected if API key is valid and network is available
**Validates: Requirements AC-16.6**

### Property 28: Invalid API Key Error Messages
*For any* invalid API key, the system SHALL return a ValidationResult.Invalid with a descriptive error message
**Validates: Requirements AC-16.7**

### Property 29: AI Fallback to Rule-Based
*For any* message where AI processing fails or is unavailable, the system SHALL fall back to rule-based processing
**Validates: Requirements AC-16.10**

### Property 30: Pattern Priority Ordering
*For any* input matching multiple patterns, the system SHALL select the pattern with the highest priority value
**Validates: Requirements (Hybrid Processing)**

### Property 31: Confidence Threshold Enforcement
*For any* rule-based match with confidence below 0.8, the system SHALL attempt AI processing if configured
**Validates: Requirements (Hybrid Processing)**

### Property 32: Message History Context
*For any* AI request, the system SHALL include the last 5 messages as context for conversation continuity
**Validates: Requirements (Hybrid Processing)**


---

## 🚨 Error Handling

### Error Categories

#### 1. Network Errors
```kotlin
sealed class NetworkError : Exception() {
    object NoConnection : NetworkError()
    object Timeout : NetworkError()
    data class HttpError(val code: Int, val message: String) : NetworkError()
    data class ApiError(val provider: AIProviderType, val message: String) : NetworkError()
}
```

**Handling Strategy**:
- Show user-friendly error message
- Fallback to cached data if available
- Fallback to rule-based processing for AI errors
- Retry with exponential backoff for transient errors

#### 2. Permission Errors
```kotlin
sealed class PermissionError : Exception() {
    data class NotGranted(val permissions: List<String>) : PermissionError()
    data class PermanentlyDenied(val permission: String) : PermissionError()
}
```

**Handling Strategy**:
- Show permission rationale dialog
- Provide alternative actions if permission denied
- Guide user to settings if permanently denied
- Graceful degradation of features

#### 3. Data Errors
```kotlin
sealed class DataError : Exception() {
    object NotFound : DataError()
    object InvalidFormat : DataError()
    data class DatabaseError(val message: String) : DataError()
}
```

**Handling Strategy**:
- Log error for debugging
- Show user-friendly error message
- Attempt data recovery if possible
- Clear corrupted data if necessary

#### 4. Processing Errors
```kotlin
sealed class ProcessingError : Exception() {
    object PatternNotMatched : ProcessingError()
    object AINotConfigured : ProcessingError()
    data class AIProcessingFailed(val reason: String) : ProcessingError()
    data class ActionExecutionFailed(val action: Action, val reason: String) : ProcessingError()
}
```

**Handling Strategy**:
- Fallback to rule-based processing
- Show help message for unrecognized commands
- Guide user to configure AI if needed
- Provide alternative actions if execution fails

### Error Response Templates

```kotlin
object ErrorMessages {
    const val NO_INTERNET = "Tidak ada koneksi internet. Beberapa fitur mungkin tidak tersedia."
    const val AI_NOT_CONFIGURED = "AI belum dikonfigurasi. Silakan tambahkan API key di Settings."
    const val PERMISSION_REQUIRED = "Izin diperlukan untuk fitur ini. Silakan berikan izin di Settings."
    const val COMMAND_NOT_UNDERSTOOD = "Maaf, saya tidak mengerti perintah tersebut. Ketik 'bantuan' untuk melihat perintah yang tersedia."
    const val ACTION_FAILED = "Maaf, terjadi kesalahan saat menjalankan perintah. Silakan coba lagi."
    const val CONTACT_NOT_FOUND = "Kontak tidak ditemukan. Pastikan nama kontak sudah benar."
    const val APP_NOT_FOUND = "Aplikasi tidak ditemukan atau tidak terinstall."
    const val API_KEY_INVALID = "API key tidak valid. Silakan periksa kembali API key Anda."
}
```

---

## 🧪 Testing Strategy

### Unit Testing

**Coverage Target**: >80% code coverage for core logic

**Test Categories**:

1. **Pattern Matching Tests**
   - Test all 60+ regex patterns
   - Test pattern priority ordering
   - Test edge cases (empty input, special characters)
   - Test ambiguous patterns

2. **Message Processing Tests**
   - Test rule-based processing flow
   - Test AI processing flow
   - Test hybrid processing decision logic
   - Test confidence threshold enforcement
   - Test fallback mechanisms

3. **Data Layer Tests**
   - Test CRUD operations for all entities
   - Test database queries and filters
   - Test data mappers (entity ↔ domain)
   - Test secure storage operations

4. **Android Integration Tests**
   - Test intent creation for all actions
   - Test permission checking logic
   - Test contact lookup and fuzzy matching
   - Test app launcher service
   - Test system control operations

5. **AI Provider Tests**
   - Test request building for each provider
   - Test response parsing
   - Test API key validation
   - Test error handling
   - Test provider factory

**Testing Framework**: JUnit 5 + MockK + Turbine (for Flow testing)

**Example Unit Test**:
```kotlin
class PatternRegistryTest {
    private lateinit var registry: PatternRegistry
    
    @BeforeEach
    fun setup() {
        registry = PatternRegistry()
    }
    
    @Test
    fun `should match add task pattern`() {
        val input = "tambah task beli susu"
        val match = registry.findMatch(input)
        
        assertNotNull(match)
        assertEquals("add_task", match?.pattern?.id)
        assertEquals("beli susu", match?.match?.groupValues?.get(3))
    }
    
    @Test
    fun `should match weather pattern with various keywords`() {
        val inputs = listOf("cuaca hari ini", "weather", "suhu berapa", "hujan ga")
        
        inputs.forEach { input ->
            val match = registry.findMatch(input)
            assertNotNull(match, "Failed to match: $input")
            assertEquals("weather", match?.pattern?.id)
        }
    }
    
    @Test
    fun `should prioritize higher priority patterns`() {
        val input = "buka whatsapp" // Could match both "open_app" and "send_whatsapp"
        val match = registry.findMatch(input)
        
        assertNotNull(match)
        // Should match the higher priority pattern
        assertTrue(match!!.pattern.priority >= 9)
    }
}
```

### Property-Based Testing

**Framework**: Kotest Property Testing

**Configuration**: Minimum 100 iterations per property test

**Property Test Examples**:

```kotlin
class MessageProcessingPropertyTest : StringSpec({
    "Property 29: AI fallback to rule-based" {
        checkAll(Arb.string(1..100)) { input ->
            val processor = HybridMessageProcessor(
                ruleBasedProcessor = mockRuleBasedProcessor(),
                aiProcessor = mockFailingAIProcessor(),
                configRepository = mockConfigRepository()
            )
            
            val result = processor.processMessage(input, mockContext())
            
            // Should fall back to rule-based when AI fails
            result shouldBe instanceOf<ProcessingResult.RuleBased>()
        }
    }
    
    "Property 30: Pattern priority ordering" {
        checkAll(
            Arb.list(Arb.commandPattern(), 2..10)
        ) { patterns ->
            val registry = PatternRegistry()
            patterns.forEach { registry.addPattern(it) }
            
            val input = patterns.first().regex.pattern
            val match = registry.findMatch(input)
            
            // Should match the highest priority pattern
            match?.pattern?.priority shouldBe patterns.maxOf { it.priority }
        }
    }
    
    "Property 18: Volume level normalization" {
        checkAll(Arb.int(0..100)) { level ->
            val systemControl = SystemControlService(mockContext())
            val maxVolume = 15 // Typical Android max volume
            
            val normalized = (level * maxVolume / 100)
            
            // Normalized value should be within device range
            normalized shouldBeInRange 0..maxVolume
        }
    }
})

// Custom Arbitraries
fun Arb.Companion.commandPattern(): Arb<CommandPattern> = arbitrary {
    CommandPattern(
        id = Arb.string(5..20).bind(),
        regex = Regex(Arb.string(10..50).bind()),
        category = Arb.enum<CommandCategory>().bind(),
        priority = Arb.int(1..10).bind(),
        handler = mockHandler()
    )
}
```

### Integration Testing

**Test Scenarios**:

1. **End-to-End Message Flow**
   - User input → Pattern matching → Action execution → Response
   - Test with various command types
   - Test with AI enabled and disabled
   - Test error scenarios

2. **Database Integration**
   - Test Room database operations
   - Test data persistence across app restarts
   - Test migration scenarios

3. **Android System Integration**
   - Test actual intent launching (on emulator)
   - Test permission request flow
   - Test system service interactions

**Framework**: AndroidX Test + Espresso

### UI Testing

**Framework**: Jetpack Compose Testing

**Test Categories**:

1. **Component Tests**
   - Test individual composables
   - Test state updates
   - Test user interactions

2. **Screen Tests**
   - Test complete screen flows
   - Test navigation
   - Test error states

**Example UI Test**:
```kotlin
class ChatScreenTest {
    @get:Rule
    val composeTestRule = createComposeRule()
    
    @Test
    fun sendMessage_shouldDisplayInChat() {
        val viewModel = mockViewModel()
        
        composeTestRule.setContent {
            ViraScreen(viewModel = viewModel)
        }
        
        // Type message
        composeTestRule.onNodeWithTag("input_field")
            .performTextInput("cuaca hari ini")
        
        // Click send
        composeTestRule.onNodeWithTag("send_button")
            .performClick()
        
        // Verify message appears
        composeTestRule.onNodeWithText("cuaca hari ini")
            .assertIsDisplayed()
    }
}
```

### Performance Testing

**Metrics to Track**:
- Message processing latency (rule-based: <100ms, AI: <2s)
- Database query performance (<50ms)
- UI rendering performance (60 FPS)
- Memory usage (<150MB)
- Battery consumption (<5% per hour)

**Tools**: Android Profiler, Benchmark library

### Test Coverage Requirements

- **Unit Tests**: >80% code coverage
- **Integration Tests**: All critical user flows
- **Property Tests**: All correctness properties
- **UI Tests**: All user-facing screens
- **Performance Tests**: All performance-critical operations


---

## 📦 Dependencies and Technology Stack

### Core Dependencies

```kotlin
// Kotlin
kotlin = "1.9.20"
kotlinx-coroutines = "1.7.3"
kotlinx-serialization = "1.6.0"

// Android
android-gradle-plugin = "8.1.2"
androidx-core-ktx = "1.12.0"
androidx-lifecycle = "2.6.2"
androidx-activity-compose = "1.8.0"

// Jetpack Compose
compose-bom = "2023.10.01"
compose-ui = "1.5.4"
compose-material3 = "1.1.2"
compose-navigation = "2.7.5"

// Dependency Injection
hilt = "2.48"
hilt-navigation-compose = "1.1.0"

// Database
room = "2.6.0"

// Network
ktor-client = "2.3.5"
ktor-client-android = "2.3.5"
ktor-client-content-negotiation = "2.3.5"
ktor-serialization-kotlinx-json = "2.3.5"

// Security
androidx-security-crypto = "1.1.0-alpha06"

// Testing
junit = "5.10.0"
mockk = "1.13.8"
kotest = "5.7.2"
turbine = "1.0.0"
androidx-test = "1.5.0"
espresso = "3.5.1"
```

### Architecture Components

- **MVVM**: ViewModel + StateFlow for state management
- **Clean Architecture**: Domain, Data, Presentation layers
- **Dependency Injection**: Hilt for DI
- **Reactive Programming**: Kotlin Coroutines + Flow
- **UI**: Jetpack Compose
- **Navigation**: Compose Navigation
- **Database**: Room
- **Network**: Ktor Client
- **Security**: EncryptedSharedPreferences + Android Keystore

---

## 🔄 Data Flow Diagrams

### Message Processing Flow

```
User Input (Voice/Text)
        ↓
  Input Validation
        ↓
  Pattern Matching (Rule-Based)
        ↓
    Match Found? ──Yes──→ Execute Handler ──→ Generate Response
        ↓ No                                         ↓
    Check AI Config                            Execute Action
        ↓                                            ↓
    Configured? ──No──→ Show Error            Update UI State
        ↓ Yes                                        ↓
    Send to AI API                            Save to Database
        ↓                                            ↓
    Parse Response                            Trigger TTS (if needed)
        ↓                                            ↓
    Extract Action                            Return to User
        ↓
    Execute Action
        ↓
    Generate Response
        ↓
    Update UI State
        ↓
    Save to Database
        ↓
    Trigger TTS (if needed)
        ↓
    Return to User
```

### Android Action Execution Flow

```
Action Request
        ↓
  Check Permissions
        ↓
    Granted? ──No──→ Request Permissions ──→ Wait for User
        ↓ Yes                                      ↓
  Determine Action Type                      Granted? ──No──→ Show Error
        ↓                                          ↓ Yes
  ┌──────┴──────┬──────────┬──────────┐          ↓
  ↓             ↓          ↓          ↓      Execute Action
Open App   Contact    System    Media
  ↓          Action    Control   Control
  ↓             ↓          ↓          ↓
Launch     Lookup    Toggle    Send Key
Intent     Contact   Setting   Event
  ↓             ↓          ↓          ↓
  └──────┬──────┴──────────┴──────────┘
         ↓
    Return Result
         ↓
    Update UI
         ↓
    Show Feedback
```

---

## 🎯 Performance Optimization

### Caching Strategy

1. **In-Memory Cache**
   - Recent messages (last 50)
   - Quick actions
   - User preferences
   - Contact list (with TTL)

2. **Persistent Cache**
   - Weather data (1 hour TTL)
   - News articles (30 minutes TTL)
   - AI responses for common queries
   - App list (updated on app install/uninstall)

3. **Cache Invalidation**
   - Time-based expiration
   - Manual refresh by user
   - On data update events

### Lazy Loading

- Load messages on demand (pagination)
- Load contacts only when needed
- Defer AI provider initialization until first use
- Lazy inject dependencies

### Background Processing

- Use WorkManager for scheduled tasks
- Process reminders in background
- Sync data in background
- Clean up old data periodically

### Memory Management

- Use weak references for large objects
- Clear message history after threshold
- Compress images before caching
- Use Paging library for large lists

---

## 🚀 Deployment and Configuration

### Build Variants

```kotlin
android {
    buildTypes {
        debug {
            applicationIdSuffix = ".debug"
            debuggable = true
            minifyEnabled = false
        }
        
        release {
            minifyEnabled = true
            shrinkResources = true
            proguardFiles(
                getDefaultProguardFile("proguard-android-optimize.txt"),
                "proguard-rules.pro"
            )
        }
    }
    
    flavorDimensions += "version"
    productFlavors {
        create("free") {
            dimension = "version"
            applicationIdSuffix = ".free"
        }
        
        create("pro") {
            dimension = "version"
            applicationIdSuffix = ".pro"
        }
    }
}
```

### ProGuard Rules

```proguard
# Keep data classes
-keep class com.vira.data.** { *; }
-keep class com.vira.domain.** { *; }

# Keep Ktor
-keep class io.ktor.** { *; }

# Keep Room
-keep class * extends androidx.room.RoomDatabase
-keep @androidx.room.Entity class *

# Keep Hilt
-keep class dagger.hilt.** { *; }
-keep class javax.inject.** { *; }
```

### Environment Configuration

```kotlin
object AppConfig {
    const val DATABASE_NAME = "vira_database"
    const val DATABASE_VERSION = 1
    
    const val MESSAGE_HISTORY_LIMIT = 50
    const val CONTEXT_MESSAGE_LIMIT = 5
    
    const val CONFIDENCE_THRESHOLD = 0.8f
    
    const val WEATHER_CACHE_TTL_MINUTES = 60
    const val NEWS_CACHE_TTL_MINUTES = 30
    
    const val AI_TIMEOUT_SECONDS = 30
    const val NETWORK_TIMEOUT_SECONDS = 15
    
    const val MAX_RETRY_ATTEMPTS = 3
    const val RETRY_DELAY_MS = 1000L
}
```

---

## 📝 Implementation Notes

### Key Design Decisions

1. **Hybrid Processing**: Combine rule-based and AI for best performance and reliability
2. **Offline-First**: Core features work without internet
3. **Security-First**: API keys stored securely, no logging of sensitive data
4. **Modular Architecture**: Easy to add/remove features
5. **User-Configurable AI**: Support multiple providers, user brings their own API key

### Future Enhancements

1. **Multi-User Support**: Multiple user profiles
2. **Cloud Sync**: Optional cloud backup and sync
3. **Voice Cloning**: Personalized TTS voice
4. **Smart Home Integration**: Control IoT devices
5. **Calendar Integration**: Google Calendar sync
6. **Location-Based Reminders**: Trigger reminders based on location
7. **Habit Tracking**: Track and analyze user habits
8. **Team Features**: Shared tasks and schedules

### Known Limitations

1. **Voice Recognition**: Depends on Android's speech recognition quality
2. **AI Latency**: Response time depends on API provider and network
3. **Permission Restrictions**: Some features require user permissions
4. **Android Version**: Some features require newer Android versions
5. **External Dependencies**: Weather, news, AI APIs may have rate limits

---

## 📚 References

### Android Documentation
- [Jetpack Compose](https://developer.android.com/jetpack/compose)
- [Room Database](https://developer.android.com/training/data-storage/room)
- [Android Keystore](https://developer.android.com/training/articles/keystore)
- [Permissions](https://developer.android.com/guide/topics/permissions/overview)

### API Documentation
- [Groq API](https://console.groq.com/docs)
- [Google Gemini API](https://ai.google.dev/docs)
- [OpenAI API](https://platform.openai.com/docs)

### Testing Resources
- [Kotest Property Testing](https://kotest.io/docs/proptest/property-based-testing.html)
- [Compose Testing](https://developer.android.com/jetpack/compose/testing)

---

**Document Version**: 1.0  
**Last Updated**: 2024  
**Status**: Ready for Review  
**Next Step**: Create tasks.md for implementation plan
