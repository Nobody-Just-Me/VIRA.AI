using System.Text.RegularExpressions;
using VIRA.Shared.Models;

namespace VIRA.Shared.Services.Handlers;

/// <summary>
/// Handler for sending WhatsApp messages
/// Pattern: "kirim|send whatsapp ke [contact name]"
/// </summary>
public class SendWhatsAppHandler : ICommandHandler
{
    public async Task<CommandResult> HandleAsync(Match match, ConversationContext context)
    {
        // Extract contact name from the match
        // Pattern groups: 1=action (kirim/send), 2=message type (whatsapp/wa), 3=ke/to, 4=contact name
        string contactName = match.Groups.Count > 4 
            ? match.Groups[4].Value.Trim() 
            : string.Empty;

        if (string.IsNullOrWhiteSpace(contactName))
        {
            return new CommandResult(
                response: "Maaf, ke siapa Anda ingin mengirim WhatsApp? Coba lagi dengan format: 'kirim whatsapp ke [nama kontak]'",
                confidence: 0.5f,
                speak: true
            );
        }

        // Create response with placeholder action
        // The actual Android implementation will be done in task 6.3
        string response = $"💬 Membuka WhatsApp untuk mengirim pesan ke {contactName}... (Fitur ini akan diimplementasikan di task 6.3)";

        return await Task.FromResult(new CommandResult(
            response: response,
            action: new { Type = "send_whatsapp", ContactName = contactName },
            confidence: 1.0f,
            speak: true
        ));
    }
}
