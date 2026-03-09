namespace VIRA.Shared.Models;

/// <summary>
/// Represents a quick action button for common commands
/// </summary>
public class QuickAction
{
    /// <summary>
    /// Unique identifier for the quick action
    /// </summary>
    public string Id { get; set; }
    
    /// <summary>
    /// Display label for the button
    /// </summary>
    public string Label { get; set; } = string.Empty;
    
    /// <summary>
    /// Icon identifier (emoji or icon name)
    /// </summary>
    public string Icon { get; set; } = string.Empty;
    
    /// <summary>
    /// Command to execute when clicked
    /// </summary>
    public string Command { get; set; } = string.Empty;
    
    /// <summary>
    /// Display order (lower numbers appear first)
    /// </summary>
    public int Order { get; set; }
    
    /// <summary>
    /// Whether this is a default (non-deletable) action
    /// </summary>
    public bool IsDefault { get; set; }
    
    /// <summary>
    /// Category for grouping actions
    /// </summary>
    public string Category { get; set; } = "General";
    
    /// <summary>
    /// Color for the quick action button (hex format)
    /// </summary>
    public string Color { get; set; } = "#8B5CF6"; // Default purple
    
    /// <summary>
    /// Query text to execute (same as Command for compatibility)
    /// </summary>
    public string Query
    {
        get => Command;
        set => Command = value;
    }

    public QuickAction()
    {
        Id = Guid.NewGuid().ToString();
        IsDefault = false;
        Order = 0;
    }

    public QuickAction(string label, string icon, string command, bool isDefault = false)
    {
        Id = Guid.NewGuid().ToString();
        Label = label;
        Icon = icon;
        Command = command;
        IsDefault = isDefault;
        Order = 0;
    }
}

/// <summary>
/// Service for managing quick actions
/// </summary>
public class QuickActionService
{
    private readonly List<QuickAction> _actions;

    public QuickActionService()
    {
        _actions = new List<QuickAction>();
        InitializeDefaultActions();
    }

    /// <summary>
    /// Initialize default quick actions
    /// </summary>
    private void InitializeDefaultActions()
    {
        var defaultActions = new[]
        {
            new QuickAction("Daftar Task", "📋", "daftar task", true) { Order = 1, Category = "Tasks" },
            new QuickAction("Cuaca", "🌤️", "cuaca", true) { Order = 2, Category = "Info" },
            new QuickAction("Berita", "📰", "berita", true) { Order = 3, Category = "Info" },
            new QuickAction("Jam", "🕐", "jam berapa", true) { Order = 4, Category = "Info" },
            new QuickAction("Baterai", "🔋", "baterai", true) { Order = 5, Category = "System" },
            new QuickAction("Jadwal", "📅", "jadwal", true) { Order = 6, Category = "Tasks" }
        };

        foreach (var action in defaultActions)
        {
            _actions.Add(action);
        }
    }

    /// <summary>
    /// Get all quick actions sorted by order
    /// </summary>
    public List<QuickAction> GetAllActions()
    {
        return _actions.OrderBy(a => a.Order).ToList();
    }

    /// <summary>
    /// Get actions by category
    /// </summary>
    public List<QuickAction> GetActionsByCategory(string category)
    {
        return _actions
            .Where(a => a.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .OrderBy(a => a.Order)
            .ToList();
    }

    /// <summary>
    /// Add a new quick action
    /// </summary>
    public QuickAction AddAction(string label, string icon, string command, string category = "General")
    {
        var action = new QuickAction(label, icon, command, false)
        {
            Category = category,
            Order = _actions.Count
        };
        
        _actions.Add(action);
        return action;
    }

    /// <summary>
    /// Update an existing quick action
    /// </summary>
    public bool UpdateAction(string id, string? label = null, string? icon = null, string? command = null, string? category = null)
    {
        var action = _actions.FirstOrDefault(a => a.Id == id);
        if (action == null || action.IsDefault)
        {
            return false; // Cannot update default actions
        }

        if (label != null) action.Label = label;
        if (icon != null) action.Icon = icon;
        if (command != null) action.Command = command;
        if (category != null) action.Category = category;

        return true;
    }

    /// <summary>
    /// Delete a quick action
    /// </summary>
    public bool DeleteAction(string id)
    {
        var action = _actions.FirstOrDefault(a => a.Id == id);
        if (action == null || action.IsDefault)
        {
            return false; // Cannot delete default actions
        }

        _actions.Remove(action);
        return true;
    }

    /// <summary>
    /// Reorder quick actions
    /// </summary>
    public void ReorderActions(List<string> orderedIds)
    {
        for (int i = 0; i < orderedIds.Count; i++)
        {
            var action = _actions.FirstOrDefault(a => a.Id == orderedIds[i]);
            if (action != null)
            {
                action.Order = i;
            }
        }
    }

    /// <summary>
    /// Get action by ID
    /// </summary>
    public QuickAction? GetActionById(string id)
    {
        return _actions.FirstOrDefault(a => a.Id == id);
    }

    /// <summary>
    /// Get all categories
    /// </summary>
    public List<string> GetCategories()
    {
        return _actions
            .Select(a => a.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToList();
    }

    /// <summary>
    /// Reset to default actions
    /// </summary>
    public void ResetToDefaults()
    {
        _actions.Clear();
        InitializeDefaultActions();
    }
}
