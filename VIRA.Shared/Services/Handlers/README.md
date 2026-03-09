# Task Management Command Handlers

This directory contains command handlers for task management patterns in VIRA.

## Implemented Handlers

### 1. AddTaskHandler
**Pattern**: `\b(tambah|add|buat|create)\s+(task|tugas|todo)\s+(.+)`

**Examples**:
- "tambah task beli susu"
- "add task buy milk"
- "buat task meeting dengan client"
- "create todo finish report"

**Response**: Confirms task addition with task title

---

### 2. CompleteTaskHandler
**Pattern**: `\b(selesai|done|complete|finish)\s+(task|tugas)\s*(.*)`

**Examples**:
- "selesai task" (completes most recent task)
- "done task beli" (completes task containing "beli")
- "complete task meeting"
- "finish tugas report"

**Response**: Confirms task completion

---

### 3. ListTasksHandler
**Pattern**: `\b(daftar|list|show|tampilkan)\s+(task|tugas|todo)`

**Examples**:
- "daftar task"
- "list tasks"
- "show todo"
- "tampilkan tugas"

**Response**: Shows active and completed tasks with priority indicators

---

### 4. DeleteTaskHandler
**Pattern**: `\b(hapus|delete|remove|batalkan)\s+(task|tugas)\s+(.+)`

**Examples**:
- "hapus task beli"
- "delete task meeting"
- "remove todo report"
- "batalkan tugas client"

**Response**: Confirms task deletion

---

## Architecture

Each handler implements the `ICommandHandler` interface:

```csharp
public interface ICommandHandler
{
    Task<CommandResult> HandleAsync(Match match, ConversationContext context);
}
```

Handlers receive:
- **Match**: Regex match result with captured groups
- **ConversationContext**: User context, session info, previous messages

Handlers return:
- **CommandResult**: Response text, optional action, confidence score, speak flag

## Integration

Handlers are registered in `PatternRegistry` with:
- **ID**: Unique identifier
- **Regex Pattern**: Pattern to match user input
- **Category**: TASK_MANAGEMENT
- **Priority**: 10 (high priority)
- **Handler**: Instance of the handler class

## Testing

See `VIRA.Shared/Tests/TaskManagementTests.cs` for unit tests covering all handlers.

## Future Enhancements

- Task priority setting via voice commands
- Due date parsing from natural language
- Task categories and tags
- Recurring tasks
- Task reminders and notifications
