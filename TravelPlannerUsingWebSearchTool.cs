using Azure.AI.Projects;
using Azure.Identity;
using Google.Protobuf.Reflection;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using OpenAI.Responses;


string apiKey = "{YOUR_API_KEY}";

IChatClient chatClient = new OpenAIClient(apiKey).GetChatClient("gpt-4o-mini").AsIChatClient();
#pragma warning disable OPENAI001
var webSearchTool = new WebSearchTool().AsAITool();

AIAgent tourPlannerAgent = chatClient.AsAIAgent(
    instructions: "You can search the web. maximum 3 searches for a given keyword and give the response as a list, make sure to include the reference links/urls",
    tools: [webSearchTool]
);

AIAgent ticketPlannerAgent = chatClient.AsAIAgent(
    instructions: "You can search the web for air ticket prices for a given set of countries, take these countries from the previous agent's output, maximum 3 searches for each country and give the response as a list, make sure to include the reference links/urls",
    tools: [webSearchTool]
);

AIAgent itenaryPlannerAgent = chatClient.AsAIAgent(
    instructions: "You can search the web for things to do for a given set of countries, take these countries from the previous agent's output, maximum 3 searches for each country and give the response as a list, make sure to include the reference links/urls",
    tools: [webSearchTool]
);


#pragma warning restore OPENAI001




Workflow workflow = AgentWorkflowBuilder.BuildSequential(tourPlannerAgent, ticketPlannerAgent, itenaryPlannerAgent);


string userInput = "What are countries ideal for travelling on a budget in 2026?";

Console.WriteLine(userInput);
// Run the workflow
AgentResponse response = await workflow.AsAIAgent().RunAsync(
    userInput
);







Console.WriteLine(response.Text);
Console.Read();


