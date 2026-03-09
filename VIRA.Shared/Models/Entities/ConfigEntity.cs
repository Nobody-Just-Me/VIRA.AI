namespace VIRA.Shared.Models.Entities;

/// <summary>
/// Database entity for configuration key-value pairs
/// </summary>
public class ConfigEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public string? Category { get; set; }
}

/// <summary>
/// Database entity for quick actions
/// </summary>
public class QuickActionEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Label { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Command { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsDefault { get; set; }
    public string Category { get; set; } = "General";

    /// <summary>
    /// Convert to domain model
    /// </summary>
    public QuickAction ToDomain()
    {
        return new QuickAction
        {
            Id = Id,
            Label = Label,
            Icon = Icon,
            Command = Command,
            Order = Order,
            IsDefault = IsDefault,
            Category = Category
        };
    }

    /// <summary>
    /// Create from domain model
    /// </summary>
    public static QuickActionEntity FromDomain(QuickAction action)
    {
        return new QuickActionEntity
        {
            Id = action.Id,
            Label = action.Label,
            Icon = action.Icon,
            Command = action.Command,
            Order = action.Order,
            IsDefault = action.IsDefault,
            Category = action.Category
        };
    }
}
