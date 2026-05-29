using Agents.Agents;
using Agents.Workflow;
using Microsoft.Extensions.AI;
using System.Text.RegularExpressions;

namespace Agents.Utility
{
    public class AIHelpers
    {
        public static async Task<AgentResult?> ToolCallUtility(ChatMessage assistantMessage, IList<AITool> tools, List<ChatMessage> messages, bool approvalToCallTool = false)
        {
            // Find tool calls
            List<FunctionCallContent> toolCalls =
                [.. assistantMessage.Contents.OfType<FunctionCallContent>()];

            // check if question
            Match match = Regex.Match(assistantMessage.Text, "<Question>(.*?)</Question>", RegexOptions.Singleline);
            if (approvalToCallTool || match.Success)
            {
                return new AgentResult()
                {
                    ApprovalRequired = match.Success | toolCalls.Count > 0,
                    Messages = [.. messages]
                };
            }

            List<AIContent> toolResults = [];

            // Execute all requested tools
            foreach (FunctionCallContent toolCall in toolCalls)
            {
                AITool tool = tools.First(t => t.Name == toolCall.Name);
                AIFunction function = (AIFunction)tool;
                object? result = await function.InvokeAsync(new AIFunctionArguments(toolCall.Arguments));
                toolResults.Add(new FunctionResultContent(toolCall.CallId, result));
            }

            // Single tool message
            if (toolResults.Count > 0)
            {
                messages.Add(new ChatMessage(ChatRole.Tool, toolResults));
            }

            return null;
        }

        public static AgentOutput ConvertToAgentOutput(AgentResult agentResut, string agentName)
        {
            AgentOutput agentOutput = new()
            {
                AgentName = agentName,
            };

            foreach (ChatMessage message in agentResut.Messages)
            {
                List<ToolCall> tools = [];

                List<FunctionCallContent> toolCalls =
                    [.. message.Contents.OfType<FunctionCallContent>()];
                List<FunctionResultContent> executedToolCalls =
                    [.. message.Contents.OfType<FunctionResultContent>()];

                foreach (FunctionCallContent toolCall in toolCalls)
                {
                    tools.Add(new()
                    {
                        ToolCallId = toolCall.CallId,
                        ToolName = toolCall.Name,
                        Arguments = toolCall.Arguments,
                    });
                }

                foreach (FunctionResultContent toolCall in executedToolCalls)
                {
                    tools.Add(new()
                    {
                        ToolCallId = toolCall.CallId,
                        Result = toolCall.Result,
                        Exception = toolCall.Exception,
                    });
                }

                agentOutput.Chat.Add(new()
                {
                    Role = message.Role.Value,
                    Text = message.Text,
                    IsApprovalRequired = false,
                    ToolCalls = [.. tools]
                });
            }

            return agentOutput;
        }
        public static List<ChatMessage> ConvertToAgentResult(AgentOutput agentResut)
        {
            List<ChatMessage> chats = [];
            foreach (AgentChatMessage prevChat in agentResut.Chat)
            {
                List<AIContent> contents = [];

                // Text
                if (!string.IsNullOrWhiteSpace(prevChat.Text))
                {
                    contents.Add(
                        new TextContent(prevChat.Text));
                }

                // Tool calls/results
                foreach (ToolCall toolCall in prevChat.ToolCalls)
                {
                    // Assistant requesting tool
                    if (prevChat.Role == "assistant")
                    {
                        contents.Add(
                            new FunctionCallContent(
                                toolCall.ToolCallId,
                                toolCall.ToolName,
                                toolCall.Arguments));
                    }

                    // Tool returning result
                    if (prevChat.Role == "tool")
                    {
                        contents.Add(
                            new FunctionResultContent(
                                toolCall.ToolCallId,
                                toolCall.Result));
                    }
                }

                chats.Add(new ChatMessage(
                    new ChatRole(prevChat.Role),
                    contents));
            }

            return chats;
        }

        public static List<string> FindAgentsToExclude()
        {
            string? socket = Environment.GetEnvironmentVariable("CONTAINER_SOCKET");
            string? host = Environment.GetEnvironmentVariable("KUBE_HOST");
            string? token = Environment.GetEnvironmentVariable("KUBE_TOKEN");

            List<string> excludeAgents = [];
            if (host == null || token == null)
            {
                excludeAgents.Add("InfrastructureAgent");
                excludeAgents.Add("DeploymentAgent");
            }

            if (socket == null)
            {
                excludeAgents.Add("DockerWriteAgent");
                excludeAgents.Add("DockerReadAgent");
            }

            return excludeAgents;
        }

        public static string FindRunTimeEnvironment()
        {
            string? runtime = Environment.GetEnvironmentVariable("CONTAINER_RUNTIME");
            string? socket = Environment.GetEnvironmentVariable("CONTAINER_SOCKET");
            string? host = Environment.GetEnvironmentVariable("KUBE_HOST");
            string? token = Environment.GetEnvironmentVariable("KUBE_TOKEN");

            List<string> result = [];
            if (host != null && token != null)
            {
                result.Add("Kubernetes");
            }

            if (socket != null)
            {
                if(runtime?.ToLower() == "podman")
                {
                    result.Add("Podman");
                }
                else
                {
                    result.Add("Docker");
                }
            }

            return string.Join(", ", result);
        }
    }
}
