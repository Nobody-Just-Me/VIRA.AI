using System.Text.RegularExpressions;
using VIRA.Shared.Models;

namespace VIRA.Shared.Services.Handlers;

/// <summary>
/// Handler for media playback control
/// Pattern: "putar|play|pause|next musik"
/// </summary>
public class MediaControlHandler : ICommandHandler
{
    public async Task<CommandResult> HandleAsync(Match match, ConversationContext context)
    {
        // Extract media action from the match
        // Pattern groups: 1=action (putar/play/pause/next/previous), 2=optional media type (musik/music/lagu)
        string action = match.Groups.Count > 1 
            ? match.Groups[1].Value.Trim().ToLower() 
            : string.Empty;

        // Map action to media control type
        string mediaAction;
        string emoji;
        string responseText;

        if (action.Contains("putar") || action.Contains("play"))
        {
            mediaAction = "play";
            emoji = "▶️";
            responseText = "Memutar musik";
        }
        else if (action.Contains("pause") || action.Contains("jeda"))
        {
            mediaAction = "pause";
            emoji = "⏸️";
            responseText = "Menjeda musik";
        }
        else if (action.Contains("next") || action.Contains("lanjut") || action.Contains("selanjutnya"))
        {
            mediaAction = "next";
            emoji = "⏭️";
            responseText = "Memutar lagu berikutnya";
        }
        else if (action.Contains("previous") || action.Contains("sebelum") || action.Contains("kembali"))
        {
            mediaAction = "previous";
            emoji = "⏮️";
            responseText = "Memutar lagu sebelumnya";
        }
        else
        {
            return new CommandResult(
                response: "Maaf, saya tidak mengerti perintah media tersebut. Coba: 'putar musik', 'pause musik', 'next lagu', atau 'previous lagu'",
                confidence: 0.5f,
                speak: true
            );
        }

        // Create response with placeholder action
        // The actual Android implementation will be done in task 6.5
        string response = $"{emoji} {responseText}... (Fitur ini akan diimplementasikan di task 6.5)";

        return await Task.FromResult(new CommandResult(
            response: response,
            action: new { Type = "media_control", MediaAction = mediaAction },
            confidence: 1.0f,
            speak: true
        ));
    }
}
