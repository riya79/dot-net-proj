using System.Globalization;

namespace LanguageWorker
{
    public class LanguageWorker : BackgroundService
    {
        private readonly ILogger<LanguageWorker> _logger;
        private string _currentCulture = "en-US"; 
        

        public LanguageWorker(ILogger<LanguageWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    SetCulture(_currentCulture);
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
        private void SetCulture(string cultureCode)
        {
            CultureInfo cultureInfo = new CultureInfo(cultureCode);
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
        }

        // Method to update the current culture from outside
        public void UpdateCulture(string newCultureCode)
        {
            _currentCulture = newCultureCode;
        }
    }
}
