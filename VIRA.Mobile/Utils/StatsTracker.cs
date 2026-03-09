using Android.Content;

namespace VIRA.Mobile.Utils;

public static class StatsTracker
{
    private const string PREFS_NAME = "vira_stats";
    private const string KEY_CONVERSATIONS = "conversation_count";
    private const string KEY_QUESTIONS = "question_count";
    private const string KEY_FIRST_USE = "first_use_date";
    private const string KEY_LAST_USE = "last_use_date";
    
    public static void IncrementConversations(Context context)
    {
        var prefs = context.GetSharedPreferences(PREFS_NAME, FileCreationMode.Private);
        var editor = prefs?.Edit();
        var current = prefs?.GetInt(KEY_CONVERSATIONS, 0) ?? 0;
        editor?.PutInt(KEY_CONVERSATIONS, current + 1);
        editor?.PutLong(KEY_LAST_USE, DateTime.Now.Ticks);
        editor?.Apply();
        
        // Set first use date if not set
        if (prefs?.GetLong(KEY_FIRST_USE, 0) == 0)
        {
            editor?.PutLong(KEY_FIRST_USE, DateTime.Now.Ticks);
            editor?.Apply();
        }
    }
    
    public static void IncrementQuestions(Context context)
    {
        var prefs = context.GetSharedPreferences(PREFS_NAME, FileCreationMode.Private);
        var editor = prefs?.Edit();
        var current = prefs?.GetInt(KEY_QUESTIONS, 0) ?? 0;
        editor?.PutInt(KEY_QUESTIONS, current + 1);
        editor?.PutLong(KEY_LAST_USE, DateTime.Now.Ticks);
        editor?.Apply();
    }
    
    public static int GetConversationCount(Context context)
    {
        var prefs = context.GetSharedPreferences(PREFS_NAME, FileCreationMode.Private);
        return prefs?.GetInt(KEY_CONVERSATIONS, 0) ?? 0;
    }
    
    public static int GetQuestionCount(Context context)
    {
        var prefs = context.GetSharedPreferences(PREFS_NAME, FileCreationMode.Private);
        return prefs?.GetInt(KEY_QUESTIONS, 0) ?? 0;
    }
    
    public static int GetDaysActive(Context context)
    {
        var prefs = context.GetSharedPreferences(PREFS_NAME, FileCreationMode.Private);
        var firstUseTicks = prefs?.GetLong(KEY_FIRST_USE, 0) ?? 0;
        
        if (firstUseTicks == 0)
            return 0;
        
        var firstUse = new DateTime(firstUseTicks);
        var days = (DateTime.Now - firstUse).Days;
        return days > 0 ? days : 1; // At least 1 day if used today
    }
    
    public static void ClearStats(Context context)
    {
        var prefs = context.GetSharedPreferences(PREFS_NAME, FileCreationMode.Private);
        var editor = prefs?.Edit();
        editor?.Clear();
        editor?.Apply();
    }
    
    public static (int conversations, int questions, int daysActive) GetAllStats(Context context)
    {
        return (
            GetConversationCount(context),
            GetQuestionCount(context),
            GetDaysActive(context)
        );
    }
}
