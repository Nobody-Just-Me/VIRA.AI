namespace VIRA.Shared.Models;

public class UserContext
{
    public DateTime CurrentTime { get; set; } = DateTime.Now;
    public string DeviceType { get; set; } = "Android";
    public int BatteryPercent { get; set; } = 100;
    public bool IsInternetConnected { get; set; } = true;
    public string LocationContext { get; set; } = "Surabaya, Indonesia";
}
