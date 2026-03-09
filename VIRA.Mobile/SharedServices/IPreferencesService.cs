namespace VIRA.Mobile.SharedServices;

public interface IPreferencesService
{
    string GetPreference(string key, string defaultValue = "");
    void SetPreference(string key, string value);
    bool GetBoolPreference(string key, bool defaultValue = false);
    void SetBoolPreference(string key, bool value);
}
