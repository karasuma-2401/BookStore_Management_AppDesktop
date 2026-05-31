using BookStoreManagement.API.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStoreManagement.API.BackgroundJobs
{
    public class AttendanceScannerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AttendanceScannerService> _logger;

        public AttendanceScannerService(IServiceScopeFactory scopeFactory, ILogger<AttendanceScannerService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Attendance Scanner Background Job is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                        DateTime nowUtc = DateTime.UtcNow;
                        TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

                        DateTime nowVn = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, vnTimeZone);

                        DateTime startOfTodayVn = nowVn.Date;

                        DateTime thresholdUtc = TimeZoneInfo.ConvertTimeToUtc(startOfTodayVn, vnTimeZone);
                        int updatedCount = await context.EmployeeShifts
                            .Where(es => es.WorkDate < thresholdUtc && es.Status == "Scheduled")
                            .ExecuteUpdateAsync(setters => setters
                                .SetProperty(e => e.Status, "Absent")
                                .SetProperty(e => e.IsPaid, false),
                                stoppingToken);

                        if (updatedCount > 0)
                        {
                            _logger.LogInformation($"[Auto-Scanner] Cap nhat thanh cong {updatedCount} ca vang mat.");
                        }
                    }
                }
                catch (Exception ex)
                {

                    _logger.LogError(ex, "[Auto-Scanner] Loi khi quet vang mat.");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}