using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;


string apiKey = "{OPENAPI_APIKEY}";

IChatClient chatClient = new OpenAIClient(apiKey).GetChatClient("gpt-4o-mini").AsIChatClient();

AIAgent agent = new ChatClientAgent(chatClient, new ChatClientAgentOptions
{
    Name = "BookingAgent",
  
    ChatOptions = new ChatOptions
    {
        Instructions = """
            You are a helpful assistant for allowing customers to book badminton courts.
            You have access to tools that can provide you with the date, available badminton slots, , available badminton courts and book a badminton court.
            Use these tools to gather information and suggest the best slot for a given date.
            if customer fail to provide his email address ask for it, as it is required for booking a court.
            Once you successfully complete a booking please send me text 'Booking_Done'
            """,
        Tools = [
           AIFunctionFactory.Create(BookBadmintonCourt),
           AIFunctionFactory.Create(GetAvailableBadmintonSlots),
           AIFunctionFactory.Create(GetCourts),
           AIFunctionFactory.Create(GetCurrentDate)
       ]
       
    }
});









string userInput = "What are the courts free for today? if any family court is available please book for me. ";
Console.WriteLine(userInput);
AgentResponse response = await agent.RunAsync(userInput);

while (!response.Text.Contains("Booking_Done"))
{
    Console.WriteLine(response);
    userInput += " " + Console.ReadLine();
    response = await agent.RunAsync(userInput);
}


Console.Read();


[Description("Returns a list of badminton court slots for a given date, each with an int courtId, startDateTime, endDateTime, Description.")]
static List<SlotResult> GetAvailableBadmintonSlots(
    [Description("The date to get Courts available for a date in format YYYY-MM-DD.")] string date)
{
    Console.WriteLine($"[Tool] Getting Courts available for '{date}'.");

    return
     [
         new(1,date+" 10:00:00 AM", date+" 11:00:00 AM","Normal Court"),
         new(2,date+" 10:00:00 PM", date+" 11:00:00 PM","Family Court"),
         new(3,date+" 11:00:00 AM", date+" 12:00:00 PM","Normal Court"),
     ];
}

[Description("Returns a list of badminton court slots for a given date, each with an int courtId, startDateTime, endDateTime, Description.")]
static bool BookBadmintonCourt(
    [Description("The id of court being booked")] int courtId,
    [Description("The startDatetime for the booking")] string startDateTime,
    [Description("The endDatetime for the booking")] string endDateTime,
    [Description("The email address of the person booking the court")] string emailaddress)
{
    Console.WriteLine($"[Tool] Booking court '{courtId}' from '{startDateTime}' to '{endDateTime}' for '{emailaddress}'.");
    return true;
}

[Description("Returns a list of badminton courts for a given date, each with an ID, name, and active status.")]
static List<CourtResult> GetCourts()
{
    Console.WriteLine($"[Tool] Getting Courts.");
    return
    [
        new(1,"Court 1", true),
        new(2,"Court 2", false),
        new(3,"Court 3", true)
    ];
}


[Description("Gets the current date from the system and returns as a string in format YYYY-MM-DD.")]
static string GetCurrentDate()
{
    Console.WriteLine("[Tool] Getting current date.");

    return DateTime.Now.ToString("yyyy-MM-dd");
}

record SlotResult(int courtId, string startDateTime, string endDateTime, string Description);

record CourtResult(int id, string Name, bool IsActive);















