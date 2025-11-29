using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.Text;
using static CommonUtilities.ColoredConsole;

namespace AzureOpenAIShared.Middleware;

/// <summary>
/// Middleware for logging function call details.
/// </summary>
public static class FunctionCallMiddleware
{
    /// <summary>
    /// Middleware that logs function call details including the function name and arguments.
    /// </summary>
    /// <param name="callingAgent">The AI agent making the function call.</param>
    /// <param name="context">The function invocation context containing function and argument information.</param>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the function invocation.</returns>
    public static async ValueTask<object?> OnFunctionCall(
        AIAgent callingAgent,
        FunctionInvocationContext context,
        Func<FunctionInvocationContext, CancellationToken, ValueTask<object?>> next,
        CancellationToken cancellationToken)
    {
        StringBuilder functionCallDetails = new();
        functionCallDetails.Append($"- Tool Call: '{context.Function.Name}'");
        
        if (context.Arguments.Count > 0)
        {
            functionCallDetails.Append($" (Args: {string.Join(", ", context.Arguments.Select(x => $"[{x.Key} = {x.Value}]"))})");
        }

        WriteSecondaryLogLine(functionCallDetails.ToString());

        return await next(context, cancellationToken);
    }
}

