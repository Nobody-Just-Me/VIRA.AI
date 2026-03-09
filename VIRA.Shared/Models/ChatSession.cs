using System;
using System.Collections.Generic;

namespace VIRA.Shared.Models
{
    /// <summary>
    /// Represents a chat conversation session
    /// </summary>
    public class ChatSession
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public List<Message> Messages { get; set; } = new List<Message>();
        
        /// <summary>
        /// Formatted timestamp for display
        /// </summary>
        public string FormattedTimestamp
        {
            get
            {
                var now = DateTime.Now;
                var diff = now - Timestamp;
                
                if (diff.TotalMinutes < 60)
                {
                    return $"{(int)diff.TotalMinutes}m ago";
                }
                else if (diff.TotalHours < 24)
                {
                    return $"{(int)diff.TotalHours}h ago";
                }
                else if (diff.TotalDays < 7)
                {
                    return $"{(int)diff.TotalDays}d ago";
                }
                else
                {
                    return Timestamp.ToString("MMM dd");
                }
            }
        }
    }
}
