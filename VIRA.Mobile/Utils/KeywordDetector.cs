using System;
using System.Linq;

namespace VIRA.Mobile.Utils;

public static class KeywordDetector
{
    public static string DetectIntent(string message)
    {
        var lower = message.ToLower();
        
        // Weather keywords
        if (lower.Contains("weather") || lower.Contains("cuaca") || 
            lower.Contains("temperature") || lower.Contains("suhu"))
            return "weather";
            
        // Schedule keywords
        if (lower.Contains("schedule") || lower.Contains("jadwal") || 
            lower.Contains("meeting") || lower.Contains("agenda"))
            return "schedule";
            
        // News keywords
        if (lower.Contains("news") || lower.Contains("berita") || 
            lower.Contains("headline"))
            return "news";
            
        // Traffic keywords
        if (lower.Contains("traffic") || lower.Contains("macet") || 
            lower.Contains("jalan") || lower.Contains("route"))
            return "traffic";
            
        // Reminder keywords
        if (lower.Contains("reminder") || lower.Contains("ingat") || 
            lower.Contains("pengingat"))
            return "reminder";
            
        // Coffee keywords
        if (lower.Contains("coffee") || lower.Contains("kopi") || 
            lower.Contains("caffeine"))
            return "coffee";
            
        // Music keywords
        if (lower.Contains("music") || lower.Contains("lagu") || 
            lower.Contains("song") || lower.Contains("play"))
            return "music";
            
        // Greeting keywords
        if (lower.Contains("hello") || lower.Contains("halo") || 
            lower.Contains("hi") || lower.Contains("hey"))
            return "greeting";
            
        // Thanks keywords
        if (lower.Contains("thanks") || lower.Contains("thank") || 
            lower.Contains("terima kasih"))
            return "thanks";
            
        return "general";
    }
}
