using Microsoft.AspNetCore.Components;

namespace Web.Components.Pages.Chat;

public partial class ChatSuggestions : ComponentBase
{
    private readonly static string Prompt = """
        Suggest up to 3 follow-up questions that I could ask you to help me complete my task.
        Each suggestion must be a complete sentence, maximum 6 words.
        Each suggestion must be phrased as something that I (the user) would ask you (the assistant) in response to your previous message,
        for example 'Show product count by country', 'List all the active products', or 'Draw a chart for units in stock'.
        """;
}
