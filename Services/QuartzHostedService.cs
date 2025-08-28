using Quartz.Spi;
using Quartz;

namespace Bundler.Services
{
    public class QuartzHostedService : IHostedService
    {
        private readonly ISchedulerFactory _factory;
        private readonly IServiceProvider _provider;
        private IScheduler? _scheduler;

        public QuartzHostedService(
            ISchedulerFactory factory,
            IServiceProvider provider)
        {
            _factory = factory;
            _provider = provider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _scheduler = await _factory.GetScheduler(cancellationToken);
            _scheduler.JobFactory = new JobFactory(_provider);
            await _scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_scheduler != null)
                await _scheduler.Shutdown(cancellationToken);
        }
    }
}
