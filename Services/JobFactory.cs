using Quartz.Spi;
using Quartz;

namespace Bundler.Services
{
    public class JobFactory : IJobFactory
    {
        private readonly IServiceProvider _provider;
        public JobFactory(IServiceProvider provider) => _provider = provider;
        public IJob? NewJob(
            TriggerFiredBundle bundle,
            IScheduler scheduler) =>
            _provider.GetService(bundle.JobDetail.JobType)
                as IJob;
        public void ReturnJob(IJob job) { }
    }
}

