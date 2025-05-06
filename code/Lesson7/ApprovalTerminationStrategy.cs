using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;

public class ApprovalTerminationStrategy : TerminationStrategy
{
    // Terminate when the final message contains the phrase "Looks good to me!"
    protected override Task<bool> ShouldAgentTerminateAsync(Agent agent, IReadOnlyList<ChatMessageContent> history, CancellationToken cancellationToken)
        => Task.FromResult(history[history.Count - 1].Content?.Contains("Looks good to me!", StringComparison.OrdinalIgnoreCase) ?? false);
}

