using Core.Utilities.Config;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.AzureAI;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Azure.AI.Projects;
using Azure.Identity;

IKernelBuilder builder = KernelBuilderProvider.CreateKernelWithChatCompletion();
builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));
Kernel kernel = builder.Build();

var connectionString = AISettingsProvider.GetSettings().AIFoundryProject.ConnectionString;
var deployment = AISettingsProvider.GetSettings().AIFoundryProject.DeploymentName;

var projectClient = new AIProjectClient(connectionString, new DefaultAzureCredential());

var clientProvider =  AzureAIClientProvider.FromConnectionString(connectionString, new AzureCliCredential());
AgentsClient client = clientProvider.Client.GetAgentsClient();

// Initialize the Conference Organizer agent
var organizerAgent = await client.CreateAgentAsync(
    deployment,
    "Conference Organizer Agent",
    instructions:
            """
            You are part of the organizer team for the GoodTech developer conference, and is in charge of evaluating proposals for conference talks.
            The goal is to determing if a give submission proposal is good enough to qualify for the conference.1. 
            Follow these steps:
            1. Evaluate the proposal, taking the following guidelines and requirements into account:
                - Is the topic relevant to the conference?
                - Is the proposal clear, concise and engaging?
                - The proposal must NOT be longer than 100 words.
                - It has to contain the term 'AI Overlords'
            2. If the proposal matches all the guidelines and requirements, state that it is approved with the sentence 'Looks good to me!'.
            3. Otherwise, respond with ONE suggestion in a friendly helpful way, how the proposal can be improved to meet the guidelines and requirements. Never include examples. 
            4. The developer will then send a new, updated, proposal that you will evaluate, following the same steps.            
            
            *Very important instruction*: Only response with a single suggested change in each response.
            """,
    tools:
    [
        // Add tools here
    ]);

var softwareDeveloperAgent = await client.CreateAgentAsync(
    deployment,
    "Software Developer Agent",
    instructions:
            """
            You are an experienced software developer that loves to share your knowledge by speaking at various tech conferences
            The goal is to write and refine a proposal for the GoodTech developer conference. 
            Make sure to write a both detailed and engaging proposal that is relevant to the conference.
            Use the following structure:
            - Title: The title of the talk
            - Abstract: A short summary of the talk
            - Outline: A brief outline of the talk
            - Key takeaways: What the audience will learn 
            Only provide a single proposal per response.
            You're laser focused on the goal at hand.
            Don't waste time with chit chat.
            Consider suggestions when refining an idea.

            **Important** Only incorporate ONE suggested improvement at a time.
            """,
    tools:
    [
        // Add tools here
    ]);


var organizerAIAgent = new AzureAIAgent(organizerAgent, client)
{
    Kernel = kernel,
    Name = organizerAgent.Value.Name,    
};

var softwareDeveloperAIAgent = new AzureAIAgent(softwareDeveloperAgent, client)
{
    Kernel = kernel,
    Name = softwareDeveloperAgent.Value.Name
};

AgentGroupChat chat = new AgentGroupChat(softwareDeveloperAIAgent, organizerAIAgent)
    {
    
        ExecutionSettings =
            new()
            {
                TerminationStrategy =
                    new ApprovalTerminationStrategy()
                    {
                        // Only the conference organizer may approve.
                        Agents = [organizerAIAgent],
                        // Limit total number of turns
                        MaximumIterations = 10,
                    }
            }
    };

// Invoke chat and display messages.
string input = "Write a proposal with the following topic: Developing AI Agents with Semantic Kernel";
chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, input));

Console.WriteLine($"###{AuthorRole.User}:");
Console.WriteLine($"'{input}'###");

await foreach (var content in chat.InvokeAsync())
{
    Console.WriteLine($"\n\n\n### {content.AuthorName.ToUpper() ?? "*"}:");
    Console.WriteLine($"'{content.Content}'");
}
