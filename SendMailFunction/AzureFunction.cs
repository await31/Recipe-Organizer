using BusinessObjects.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Repositories;
using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Host;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

public class AzureFunction {

    private readonly IEmailSender _emailSender;
    public class OrchestratorInput {
        public DateTimeOffset ExecutionDateTime { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public int MealPlanId { get; set; }
        public string MealPlanTitle { get; set; }
        public string MealPlanDate { get; set; }
        public DateTime DueTime { get; set; }
    }

    public AzureFunction(IEmailSender emailSender) {
        _emailSender = emailSender;
    }

    [FunctionName("EmailSenderFunction")]
    public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
    [DurableClient] IDurableOrchestrationClient starter, TraceWriter log) {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);

        int mealPlanId = data?.mealPlanId;
        DateTimeOffset executionDateTime = data?.executionDateTime;
        string email = data?.email;
        string mealPlanTitle = data?.mealPlanTitle;
        string mealPlanDate = data?.mealPlanDate;
        string username = data?.username;
        //string timezone = data?.timezone;

        var input = new OrchestratorInput {
            Email = email,
            Username = username,
            MealPlanId = mealPlanId,
            MealPlanTitle = mealPlanTitle,
            MealPlanDate = mealPlanDate,
            DueTime = executionDateTime.DateTime.AddHours(-14)
        };

        //var cronExpression = $"{localExecutionDateTimeUtc.Minute} {localExecutionDateTimeUtc.Hour} {localExecutionDateTimeUtc.Day} {localExecutionDateTimeUtc.Month} * {localExecutionDateTimeUtc.Year}";
        //log.Info(cronExpression);
        log.Info("Email will be sent at: "+executionDateTime.DateTime.AddHours(-14).ToString());
        await starter.StartNewAsync("SendMailOrchestrator", input);
        return new OkObjectResult("Email will be sent successfully.");
    }   

    [FunctionName("SendMailOrchestrator")]
    public async Task SendMailOrchestrator(
    [OrchestrationTrigger] IDurableOrchestrationContext context) {

        var input = context.GetInput<OrchestratorInput>();
        var dueTime = input.DueTime;
        await context.CreateTimer(dueTime, CancellationToken.None);
        //await context.CreateTimer(dueTime, CancellationToken.None);
        await SendMailTask(input.Email, input.Username, input.MealPlanId, input.MealPlanTitle, input.MealPlanDate);
    }

    public async Task SendMailTask(string email, string username, int mealPlanId, string mealPlanTitle, string mealPlanDate) {
        var subject = "Meal plan reminder";
        string body = @"<html> <head> <style> body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; } .container { max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ccc; border-radius: 5px; } .header { background-color: #f5f5f5; padding: 10px; text-align: center; } .content { padding: 10px; text-align: center; } .footer { background-color: #f5f5f5; padding: 10px; text-align: center; } ul { list-style: none; } </style> </head> <body> <div class='container'> <div class='header'> <h1>Meal Plan Reminder</h1> </div> <div class='content' style='margin-bottom: 3%; margin-top: 1%'> <h2>Dear " + username + @", you have a plan on " + mealPlanDate + @"</h2> <h3>Title: " + mealPlanTitle + @"</h3> <a href='https://cookez.azurewebsites.net/MealPlan/Details/" + mealPlanId + @"' target='_blank' style='margin-top: 4%; border: solid 1px #3498db; border-radius: 5px; box-sizing: border-box; cursor: pointer; display: inline-block; font-size: 14px; font-weight: bold; margin: 0; padding: 12px 25px; text-decoration: none; text-transform: capitalize; background-color: #3498db; border-color: #3498db; color: #ffffff;'>View detail</a> </div> <div class='footer'> <p>We hope you enjoy your meal!</p> <p>Best regards,</p> <p>Cookez Team</p> </div> </div> </body> </html>";
        await _emailSender.SendEmailAsync(email, subject, body);
    }
    
}
