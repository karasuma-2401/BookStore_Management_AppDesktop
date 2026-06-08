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

                        var scheduledShifts = await context.EmployeeShifts
                            .Include(es => es.Shift)
                            .Where(es => es.Status == "Scheduled")
                            .ToListAsync(stoppingToken);

                        bool changed = false;
                        foreach (var es in scheduledShifts)
                        {
                            var utcDate = DateTime.SpecifyKind(es.WorkDate, DateTimeKind.Utc);
                            var workDateVn = TimeZoneInfo.ConvertTimeFromUtc(utcDate, vnTimeZone).Date;
                            var shiftEndTime = workDateVn.Add(es.Shift?.EndTime ?? TimeSpan.Zero);

                            if (nowVn > shiftEndTime)
                            {
                                es.Status = "Absent";
                                es.IsPaid = false;
                                changed = true;
                            }
                        }

                        if (changed)
                        {
                            int updatedCount = await context.SaveChangesAsync(stoppingToken);
                            _logger.LogInformation($"[Auto-Scanner] Cap nhat thanh cong {updatedCount} ca vang mat.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[Auto-Scanner] Loi khi quet vang mat.");
                }

                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }
}