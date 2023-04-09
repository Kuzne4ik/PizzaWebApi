using Hangfire;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PizzaWebApi.Core.Interfaces;

/// <summary>
/// Handfire run RecurringJobs
/// https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/fundamentals/host/hosted-services/samples/3.x/BackgroundTasksSample/Services/ConsumeScopedServiceHostedService.cs
/// </summary>
public class HandfireRegisterJobsHostedService : BackgroundService
{
    private readonly ILogger<HandfireRegisterJobsHostedService> _logger;

    public HandfireRegisterJobsHostedService(ILogger<HandfireRegisterJobsHostedService> logger)
    {
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //Get Reoport about last 24 hours Orders
        RegisterSend24HoursOrdersReportJob();
        return Task.CompletedTask;
    }

    private Task RegisterSend24HoursOrdersReportJob()
    {
        _logger.LogInformation($"Run {RegisterSend24HoursOrdersReportJob} ");

        try
        {
            RecurringJob.AddOrUpdate<ITelegramService>("neworderslast24hourstask", t => t.SendOrders24ReportAsync() , Cron.Daily(6));
        }
        catch (Exception ex)
        {
           _logger.LogError(ex, $"{nameof(RegisterSend24HoursOrdersReportJob)} failed");
        }
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Consume Scoped Service Hosted Service is stopping.");

        await base.StopAsync(stoppingToken);
    }
}