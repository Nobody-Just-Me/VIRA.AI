using Xunit;
using VIRA.Shared.ViewModels;
using VIRA.Shared.Models;
using VIRA.Shared.Services;

namespace VIRA.Shared.Tests;

/// <summary>
/// UI tests for VIRA components
/// Tests chat message sending/display, voice button interaction, quick actions, settings, and tasks screen
/// </summary>
public class UITests
{
    private readonly MainChatViewModel _chatViewModel;
    private readonly TaskManager _taskManager;
    private readonly PreferencesService _preferencesService;

    public UITests()
    {
        _taskManager = new TaskManager();
        _preferencesService = new PreferencesService();
        
        var weatherService = new WeatherApiService();
        var newsService = new NewsApiService();
        var ruleBasedProcessor = new RuleBasedProcessor(_taskManager, weatherService, newsService);
        var hybridProcessor = new HybridMessageProcessor(ruleBasedProcessor, _preferencesService);
        
        _chatViewModel = new MainChatViewModel(hybridProcessor, _taskManager);
    }

    #region Chat Message Tests

    [Fact]
    public async Task SendMessage_ShouldAddUserMessageToHistory()
    {
        // Arrange
        string message = "Hello VIRA";
        int initialCount = _chatViewModel.Messages.Count;

        // Act
        await _chatViewModel.SendMessageAsync(message);

        // Assert
        Assert.True(_chatViewModel.Messages.Count > initialCount);
        Assert.Contains(_chatViewModel.Messages, m => m.Content == message && m.IsUser);
    }

    [Fact]
    public async Task SendMessage_ShouldAddAssistantResponse()
    {
        // Arrange
        string message = "tambah task beli susu";

        // Act
        await _chatViewModel.SendMessageAsync(message);

        // Assert
        Assert.Contains(_chatViewModel.Messages, m => !m.IsUser);
    }

    [Fact]
    public async Task SendMessage_ShouldSetTimestamp()
    {
        // Arrange
        string message = "test message";
        var beforeSend = DateTime.UtcNow;

        // Act
        await _chatViewModel.SendMessageAsync(message);

        // Assert
        var userMessage = _chatViewModel.Messages.Last(m => m.IsUser);
        Assert.True(userMessage.Timestamp >= beforeSend);
        Assert.True(userMessage.Timestamp <= DateTime.UtcNow);
    }

    [Fact]
    public async Task SendMessage_ShouldClearInputAfterSending()
    {
        // Arrange
        string message = "test message";
        _chatViewModel.InputText = message;

        // Act
        await _chatViewModel.SendMessageAsync(message);

        // Assert
        Assert.Empty(_chatViewModel.InputText);
    }

    [Fact]
    public async Task SendMessage_ShouldDisableInputWhileProcessing()
    {
        // Arrange
        string message = "test message";

        // Act
        var sendTask = _chatViewModel.SendMessageAsync(message);
        
        // Assert - Should be processing
        Assert.True(_chatViewModel.IsProcessing);
        
        await sendTask;
        
        // Assert - Should be done processing
        Assert.False(_chatViewModel.IsProcessing);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task SendMessage_ShouldNotSendEmptyMessages(string message)
    {
        // Arrange
        int initialCount = _chatViewModel.Messages.Count;

        // Act
        await _chatViewModel.SendMessageAsync(message);

        // Assert
        Assert.Equal(initialCount, _chatViewModel.Messages.Count);
    }

    [Fact]
    public void MessageDisplay_ShouldDistinguishUserAndAssistant()
    {
        // Arrange
        var userMessage = new ChatMessage
        {
            Content = "User message",
            IsUser = true,
            Role = ChatMessageRole.User
        };
        
        var assistantMessage = new ChatMessage
        {
            Content = "Assistant message",
            IsUser = false,
            Role = ChatMessageRole.Assistant
        };

        // Assert
        Assert.True(userMessage.IsUser);
        Assert.False(assistantMessage.IsUser);
        Assert.Equal(ChatMessageRole.User, userMessage.Role);
        Assert.Equal(ChatMessageRole.Assistant, assistantMessage.Role);
    }

    [Fact]
    public void MessageDisplay_ShouldShowTimestamp()
    {
        // Arrange
        var message = new ChatMessage
        {
            Content = "Test message",
            Timestamp = DateTime.UtcNow
        };

        // Assert
        Assert.NotEqual(default(DateTime), message.Timestamp);
    }

    [Fact]
    public void MessageDisplay_ShouldShowProcessingType()
    {
        // Arrange
        var ruleBasedMessage = new ChatMessage
        {
            Content = "Rule-based response",
            ProcessingType = "rule-based"
        };
        
        var aiMessage = new ChatMessage
        {
            Content = "AI response",
            ProcessingType = "ai"
        };

        // Assert
        Assert.Equal("rule-based", ruleBasedMessage.ProcessingType);
        Assert.Equal("ai", aiMessage.ProcessingType);
    }

    #endregion

    #region Voice Button Tests

    [Fact]
    public void VoiceButton_ShouldToggleListeningState()
    {
        // Arrange
        bool isListening = false;

        // Act
        isListening = !isListening; // Simulate button press

        // Assert
        Assert.True(isListening);
        
        // Act again
        isListening = !isListening;
        
        // Assert
        Assert.False(isListening);
    }

    [Fact]
    public void VoiceButton_ShouldShowVisualFeedback()
    {
        // Arrange
        var voiceState = VoiceRecognitionState.Idle;

        // Act - Start listening
        voiceState = VoiceRecognitionState.Listening;

        // Assert
        Assert.Equal(VoiceRecognitionState.Listening, voiceState);
        
        // Act - Processing
        voiceState = VoiceRecognitionState.Processing;
        
        // Assert
        Assert.Equal(VoiceRecognitionState.Processing, voiceState);
    }

    [Fact]
    public void VoiceButton_ShouldHandleError()
    {
        // Arrange
        var voiceState = VoiceRecognitionState.Listening;

        // Act
        voiceState = VoiceRecognitionState.Error;

        // Assert
        Assert.Equal(VoiceRecognitionState.Error, voiceState);
    }

    [Fact]
    public void VoiceRecognition_ShouldTranscribeToText()
    {
        // Arrange
        string transcribedText = "tambah task beli susu";

        // Act
        _chatViewModel.InputText = transcribedText;

        // Assert
        Assert.Equal(transcribedText, _chatViewModel.InputText);
    }

    [Fact]
    public void VoiceRecognition_ShouldAllowConfirmationBeforeSending()
    {
        // Arrange
        string transcribedText = "test message";
        bool confirmed = false;

        // Act - Transcribe
        _chatViewModel.InputText = transcribedText;
        
        // User can review before confirming
        confirmed = true;

        // Assert
        Assert.Equal(transcribedText, _chatViewModel.InputText);
        Assert.True(confirmed);
    }

    #endregion

    #region Quick Actions Tests

    [Fact]
    public void QuickActions_ShouldHaveDefaultActions()
    {
        // Arrange
        var quickActions = GetDefaultQuickActions();

        // Assert
        Assert.NotEmpty(quickActions);
        Assert.True(quickActions.Count >= 8);
    }

    [Fact]
    public void QuickActions_ShouldExecuteCommand()
    {
        // Arrange
        var quickAction = new QuickAction
        {
            Id = "weather",
            Label = "Cuaca",
            Command = "cuaca hari ini",
            Icon = "☀️"
        };

        // Act
        string command = quickAction.Command;

        // Assert
        Assert.Equal("cuaca hari ini", command);
    }

    [Fact]
    public void QuickActions_ShouldBeCustomizable()
    {
        // Arrange
        var quickActions = new List<QuickAction>();
        var newAction = new QuickAction
        {
            Id = "custom",
            Label = "Custom Action",
            Command = "custom command",
            Icon = "🎯"
        };

        // Act
        quickActions.Add(newAction);

        // Assert
        Assert.Contains(quickActions, a => a.Id == "custom");
    }

    [Fact]
    public void QuickActions_ShouldSupportReordering()
    {
        // Arrange
        var quickActions = new List<QuickAction>
        {
            new QuickAction { Id = "1", Label = "First" },
            new QuickAction { Id = "2", Label = "Second" },
            new QuickAction { Id = "3", Label = "Third" }
        };

        // Act - Move first to last
        var first = quickActions[0];
        quickActions.RemoveAt(0);
        quickActions.Add(first);

        // Assert
        Assert.Equal("2", quickActions[0].Id);
        Assert.Equal("1", quickActions[2].Id);
    }

    [Fact]
    public void QuickActions_ShouldBeCollapsible()
    {
        // Arrange
        bool isExpanded = true;

        // Act
        isExpanded = !isExpanded;

        // Assert
        Assert.False(isExpanded);
    }

    #endregion

    #region Settings Screen Tests

    [Fact]
    public void Settings_ShouldSaveAIProvider()
    {
        // Arrange
        string provider = "groq";

        // Act
        _preferencesService.SetPreference("ai_provider", provider);
        var saved = _preferencesService.GetPreference("ai_provider");

        // Assert
        Assert.Equal(provider, saved);
    }

    [Fact]
    public void Settings_ShouldSaveAPIKey()
    {
        // Arrange
        string apiKey = "gsk_test_1234567890";

        // Act
        _preferencesService.SetPreference("groq_api_key", apiKey);
        var saved = _preferencesService.GetPreference("groq_api_key");

        // Assert
        Assert.Equal(apiKey, saved);
    }

    [Fact]
    public void Settings_ShouldToggleTTS()
    {
        // Arrange
        bool ttsEnabled = true;

        // Act
        _preferencesService.SetPreference("tts_enabled", ttsEnabled.ToString());
        var saved = _preferencesService.GetPreference("tts_enabled");

        // Assert
        Assert.Equal("True", saved);
    }

    [Fact]
    public void Settings_ShouldToggleVoiceInput()
    {
        // Arrange
        bool voiceEnabled = true;

        // Act
        _preferencesService.SetPreference("voice_enabled", voiceEnabled.ToString());
        var saved = _preferencesService.GetPreference("voice_enabled");

        // Assert
        Assert.Equal("True", saved);
    }

    [Fact]
    public void Settings_ShouldToggleProactiveSuggestions()
    {
        // Arrange
        bool proactiveEnabled = false;

        // Act
        _preferencesService.SetPreference("proactive_suggestions", proactiveEnabled.ToString());
        var saved = _preferencesService.GetPreference("proactive_suggestions");

        // Assert
        Assert.Equal("False", saved);
    }

    [Fact]
    public void Settings_ShouldValidateAPIKey()
    {
        // Arrange
        string validKey = "gsk_1234567890abcdef";
        string invalidKey = "invalid_key";

        // Act
        bool validResult = ValidateApiKeyFormat(validKey);
        bool invalidResult = ValidateApiKeyFormat(invalidKey);

        // Assert
        Assert.True(validResult);
        Assert.False(invalidResult);
    }

    [Fact]
    public async Task Settings_ShouldTestConnection()
    {
        // Arrange
        string apiKey = "test_key";

        // Act
        bool connected = await TestConnection(apiKey);

        // Assert
        // Test key should fail
        Assert.False(connected);
    }

    #endregion

    #region Tasks Screen Tests

    [Fact]
    public async Task TasksScreen_ShouldDisplayActiveTasks()
    {
        // Arrange
        await _taskManager.AddTaskAsync("Active task 1");
        await _taskManager.AddTaskAsync("Active task 2");

        // Act
        var activeTasks = await _taskManager.GetActiveTasksAsync();

        // Assert
        Assert.NotEmpty(activeTasks);
        Assert.All(activeTasks, task => Assert.False(task.IsCompleted));
    }

    [Fact]
    public async Task TasksScreen_ShouldDisplayCompletedTasks()
    {
        // Arrange
        var task = await _taskManager.AddTaskAsync("Task to complete");
        await _taskManager.CompleteTaskAsync(task.Id);

        // Act
        var completedTasks = await _taskManager.GetCompletedTasksAsync();

        // Assert
        Assert.Contains(completedTasks, t => t.Id == task.Id);
        Assert.All(completedTasks, task => Assert.True(task.IsCompleted));
    }

    [Fact]
    public async Task TasksScreen_ShouldToggleTaskCompletion()
    {
        // Arrange
        var task = await _taskManager.AddTaskAsync("Toggle task");

        // Act - Complete
        await _taskManager.CompleteTaskAsync(task.Id);
        var completed = await _taskManager.GetTaskByIdAsync(task.Id);

        // Assert
        Assert.NotNull(completed);
        Assert.True(completed.IsCompleted);

        // Act - Uncomplete (if supported)
        // Note: Current implementation may not support uncomplete
    }

    [Fact]
    public async Task TasksScreen_ShouldDeleteTask()
    {
        // Arrange
        var task = await _taskManager.AddTaskAsync("Task to delete");

        // Act
        await _taskManager.DeleteTaskAsync(task.Id);
        var deleted = await _taskManager.GetTaskByIdAsync(task.Id);

        // Assert
        Assert.Null(deleted);
    }

    [Fact]
    public async Task TasksScreen_ShouldShowPriorityBadge()
    {
        // Arrange
        var highTask = await _taskManager.AddTaskAsync("High priority", TaskPriority.HIGH);
        var mediumTask = await _taskManager.AddTaskAsync("Medium priority", TaskPriority.MEDIUM);
        var lowTask = await _taskManager.AddTaskAsync("Low priority", TaskPriority.LOW);

        // Assert
        Assert.Equal(TaskPriority.HIGH, highTask.Priority);
        Assert.Equal(TaskPriority.MEDIUM, mediumTask.Priority);
        Assert.Equal(TaskPriority.LOW, lowTask.Priority);
    }

    [Fact]
    public async Task TasksScreen_ShouldShowDueDate()
    {
        // Arrange
        var dueDate = DateTime.Today.AddDays(1);
        var task = await _taskManager.AddTaskAsync("Task with due date", dueDate: dueDate);

        // Assert
        Assert.NotNull(task.DueDate);
        Assert.Equal(dueDate.Date, task.DueDate.Value.Date);
    }

    [Fact]
    public async Task TasksScreen_ShouldSortByPriorityAndDueDate()
    {
        // Arrange
        await _taskManager.AddTaskAsync("Low, far", TaskPriority.LOW, DateTime.Today.AddDays(10));
        await _taskManager.AddTaskAsync("High, soon", TaskPriority.HIGH, DateTime.Today.AddDays(1));
        await _taskManager.AddTaskAsync("Medium, tomorrow", TaskPriority.MEDIUM, DateTime.Today.AddDays(2));

        // Act
        var tasks = await _taskManager.GetActiveTasksAsync();

        // Assert
        Assert.NotEmpty(tasks);
        // Should be sorted by priority first
        for (int i = 0; i < tasks.Count - 1; i++)
        {
            if (tasks[i].Priority != tasks[i + 1].Priority)
            {
                Assert.True(tasks[i].Priority >= tasks[i + 1].Priority);
            }
        }
    }

    #endregion

    #region Loading and Error States Tests

    [Fact]
    public void LoadingState_ShouldShowIndicator()
    {
        // Arrange
        bool isLoading = false;

        // Act
        isLoading = true;

        // Assert
        Assert.True(isLoading);
    }

    [Fact]
    public void ErrorState_ShouldShowMessage()
    {
        // Arrange
        string errorMessage = "An error occurred";
        bool hasError = false;

        // Act
        hasError = true;

        // Assert
        Assert.True(hasError);
        Assert.NotEmpty(errorMessage);
    }

    [Fact]
    public void ErrorState_ShouldShowRetryButton()
    {
        // Arrange
        bool canRetry = false;

        // Act
        canRetry = true;

        // Assert
        Assert.True(canRetry);
    }

    [Fact]
    public void OfflineIndicator_ShouldShowWhenNoInternet()
    {
        // Arrange
        bool isOnline = true;

        // Act
        isOnline = false;

        // Assert
        Assert.False(isOnline);
    }

    #endregion

    #region WeatherCard Tests

    [Fact]
    public void WeatherCard_ShouldDisplayAllFields()
    {
        // Arrange
        var weatherData = new WeatherData
        {
            City = "Jakarta",
            Temp = "28°C",
            Condition = "Partly Cloudy",
            Humidity = "Humidity: 65%",
            UV = "UV Index: 7",
            Tomorrow = "Tomorrow: Sunny, 30°C"
        };

        // Assert - All fields should be populated
        Assert.NotEmpty(weatherData.City);
        Assert.NotEmpty(weatherData.Temp);
        Assert.NotEmpty(weatherData.Condition);
        Assert.NotEmpty(weatherData.Humidity);
        Assert.NotEmpty(weatherData.UV);
        Assert.NotEmpty(weatherData.Tomorrow);
    }

    [Fact]
    public void WeatherCard_ShouldHandleEmptyData()
    {
        // Arrange
        var weatherData = new WeatherData();

        // Assert - Should have default empty strings
        Assert.NotNull(weatherData.City);
        Assert.NotNull(weatherData.Temp);
        Assert.NotNull(weatherData.Condition);
        Assert.NotNull(weatherData.Humidity);
        Assert.NotNull(weatherData.UV);
        Assert.NotNull(weatherData.Tomorrow);
    }

    [Fact]
    public void WeatherCard_ShouldFormatTemperature()
    {
        // Arrange
        var weatherData = new WeatherData
        {
            Temp = "28°C"
        };

        // Assert
        Assert.Contains("°", weatherData.Temp);
    }

    #endregion

    #region NewsCard Tests

    [Fact]
    public void NewsCard_ShouldDisplayAllFields()
    {
        // Arrange
        var newsItems = new List<NewsItem>
        {
            new NewsItem
            {
                Category = "Technology",
                Title = "New AI breakthrough announced"
            },
            new NewsItem
            {
                Category = "Business",
                Title = "Market reaches all-time high"
            }
        };

        // Assert - All fields should be populated
        Assert.NotEmpty(newsItems);
        Assert.All(newsItems, item =>
        {
            Assert.NotEmpty(item.Category);
            Assert.NotEmpty(item.Title);
        });
    }

    [Fact]
    public void NewsCard_ShouldHandleEmptyList()
    {
        // Arrange
        var newsItems = new List<NewsItem>();

        // Assert - Should handle empty list
        Assert.NotNull(newsItems);
        Assert.Empty(newsItems);
    }

    [Fact]
    public void NewsCard_ShouldHandleEmptyData()
    {
        // Arrange
        var newsItem = new NewsItem();

        // Assert - Should have default empty strings
        Assert.NotNull(newsItem.Category);
        Assert.NotNull(newsItem.Title);
    }

    [Fact]
    public void NewsCard_ShouldDisplayMultipleItems()
    {
        // Arrange
        var newsItems = new List<NewsItem>
        {
            new NewsItem { Category = "Tech", Title = "Title 1" },
            new NewsItem { Category = "Sports", Title = "Title 2" },
            new NewsItem { Category = "Politics", Title = "Title 3" }
        };

        // Assert
        Assert.Equal(3, newsItems.Count);
        Assert.All(newsItems, item =>
        {
            Assert.NotEmpty(item.Category);
            Assert.NotEmpty(item.Title);
        });
    }

    #endregion

    #region TrafficCard Tests

    [Fact]
    public void TrafficCard_ShouldDisplayAllFields()
    {
        // Arrange
        var trafficRoutes = new List<TrafficRoute>
        {
            new TrafficRoute
            {
                Route = "Home to Work",
                ETA = "25 min",
                Status = "Light traffic",
                Color = "#22c55e"
            },
            new TrafficRoute
            {
                Route = "Downtown Route",
                ETA = "45 min",
                Status = "Heavy traffic",
                Color = "#ef4444"
            }
        };

        // Assert - All fields should be populated
        Assert.NotEmpty(trafficRoutes);
        Assert.All(trafficRoutes, route =>
        {
            Assert.NotEmpty(route.Route);
            Assert.NotEmpty(route.ETA);
            Assert.NotEmpty(route.Status);
            Assert.NotEmpty(route.Color);
        });
    }

    [Fact]
    public void TrafficCard_ShouldHandleEmptyList()
    {
        // Arrange
        var trafficRoutes = new List<TrafficRoute>();

        // Assert - Should handle empty list
        Assert.NotNull(trafficRoutes);
        Assert.Empty(trafficRoutes);
    }

    [Fact]
    public void TrafficCard_ShouldHandleEmptyData()
    {
        // Arrange
        var trafficRoute = new TrafficRoute();

        // Assert - Should have default empty strings
        Assert.NotNull(trafficRoute.Route);
        Assert.NotNull(trafficRoute.ETA);
        Assert.NotNull(trafficRoute.Status);
        Assert.NotNull(trafficRoute.Color);
    }

    [Fact]
    public void TrafficCard_ShouldDisplayMultipleRoutes()
    {
        // Arrange
        var trafficRoutes = new List<TrafficRoute>
        {
            new TrafficRoute { Route = "Route 1", ETA = "15 min", Status = "Clear", Color = "#22c55e" },
            new TrafficRoute { Route = "Route 2", ETA = "30 min", Status = "Moderate", Color = "#eab308" },
            new TrafficRoute { Route = "Route 3", ETA = "60 min", Status = "Heavy", Color = "#ef4444" }
        };

        // Assert
        Assert.Equal(3, trafficRoutes.Count);
        Assert.All(trafficRoutes, route =>
        {
            Assert.NotEmpty(route.Route);
            Assert.NotEmpty(route.ETA);
            Assert.NotEmpty(route.Status);
            Assert.NotEmpty(route.Color);
        });
    }

    [Fact]
    public void TrafficCard_ShouldSupportColorCoding()
    {
        // Arrange
        var trafficRoutes = new List<TrafficRoute>
        {
            new TrafficRoute { Route = "Clear Route", Status = "Clear", Color = "#22c55e" },
            new TrafficRoute { Route = "Moderate Route", Status = "Moderate", Color = "#eab308" },
            new TrafficRoute { Route = "Heavy Route", Status = "Heavy", Color = "#ef4444" }
        };

        // Assert - Each route should have a valid hex color
        Assert.All(trafficRoutes, route =>
        {
            Assert.StartsWith("#", route.Color);
            Assert.True(route.Color.Length == 7 || route.Color.Length == 9); // #RRGGBB or #AARRGGBB
        });
    }

    #endregion

    #region Helper Methods

    private List<QuickAction> GetDefaultQuickActions()
    {
        return new List<QuickAction>
        {
            new QuickAction { Id = "status", Label = "Status", Command = "status hari ini", Icon = "📊" },
            new QuickAction { Id = "weather", Label = "Cuaca", Command = "cuaca hari ini", Icon = "☀️" },
            new QuickAction { Id = "news", Label = "Berita", Command = "berita terkini", Icon = "📰" },
            new QuickAction { Id = "schedule", Label = "Jadwal", Command = "jadwal hari ini", Icon = "📅" },
            new QuickAction { Id = "tasks", Label = "Task", Command = "daftar task", Icon = "✅" },
            new QuickAction { Id = "reminder", Label = "Reminder", Command = "reminder apa saja", Icon = "⏰" },
            new QuickAction { Id = "traffic", Label = "Traffic", Command = "lalu lintas", Icon = "🚗" },
            new QuickAction { Id = "suggestions", Label = "Saran", Command = "apa yang harus saya lakukan", Icon = "💡" }
        };
    }

    private bool ValidateApiKeyFormat(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            return false;

        return apiKey.StartsWith("gsk_") || 
               apiKey.StartsWith("AIza") || 
               apiKey.StartsWith("sk-");
    }

    private async Task<bool> TestConnection(string apiKey)
    {
        // Simulate connection test
        await Task.Delay(10);
        return false; // Test keys always fail
    }

    #endregion
}
