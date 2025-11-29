using Microsoft.Agents.AI;
using Microsoft.Agents.AI.OpenAI;
using System.Reflection;
using System.Linq;
using Microsoft.Extensions.AI;

namespace AzureOpenAIShared;

/// <summary>
/// Extension methods for creating AI tools from class instances.
/// </summary>
public static class ToolExtensions
{
    /// <summary>
    /// Creates AITool objects from all public methods of the given instance.
    /// </summary>
    /// <typeparam name="T">The type of the instance containing the tool methods.</typeparam>
    /// <param name="instance">The instance containing the tool methods.</param>
    /// <returns>A list of AITool objects.</returns>
    public static List<AITool> CreateAIToolsFromInstance<T>(this T instance) where T : class
    {
        if (instance == null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        var tools = new List<AITool>();
        var type = typeof(T);

        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => !m.IsSpecialName);

        foreach (var method in methods)
        {
            var function = AIFunctionFactory.Create(method, instance);
            
            if (method.IsDefined(typeof(DangerAttribute), inherit: false))
            {
                var approvalRequiredFunction = new ApprovalRequiredAIFunction(function);
                tools.Add(approvalRequiredFunction);
            }
            else
            {
                tools.Add(function);
            }
        }

        return tools;
    }
}

