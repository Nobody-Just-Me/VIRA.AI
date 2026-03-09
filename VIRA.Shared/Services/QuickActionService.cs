using VIRA.Shared.Models;

namespace VIRA.Shared.Services;

public class QuickActionService
{
    private readonly List<QuickAction> _defaultActions;

    public QuickActionService()
    {
        _defaultActions = new List<QuickAction>
        {
            new QuickAction { Icon = "☀️", Label = "Weather" },
            new QuickAction { Icon = "📰", Label = "News" },
            new QuickAction { Icon = "📅", Label = "Reminders" },
            new QuickAction { Icon = "🚗", Label = "Traffic" },
            new QuickAction { Icon = "☕", Label = "Coffee" },
            new QuickAction { Icon = "🎵", Label = "Music" }
        };
    }

    public List<QuickAction> GetDefaultActions()
    {
        return _defaultActions;
    }

    public List<QuickAction> GetCustomActions()
    {
        // TODO: Load from preferences/storage
        return new List<QuickAction>();
    }

    public void SaveCustomActions(List<QuickAction> actions)
    {
        // TODO: Save to preferences/storage
    }
}
