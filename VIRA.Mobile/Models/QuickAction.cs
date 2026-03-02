namespace VIRA.Mobile.Models;

public class QuickAction
{
    public string Icon { get; set; } = "";
    public string Label { get; set; } = "";
    public string Action { get; set; } = "";
    
    public QuickAction(string icon, string label, string action)
    {
        Icon = icon;
        Label = label;
        Action = action;
    }
}
