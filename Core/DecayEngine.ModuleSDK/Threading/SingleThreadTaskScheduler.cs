using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DecayEngine.ModuleSDK.Threading
{
    public class SingleThreadTaskScheduler : TaskScheduler
    {
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return Enumerable.Empty<Task>();
        }

        protected override void QueueTask(Task task)
        {
            TryExecuteTask(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            TryExecuteTask(task);
            return true;
        }
    }
}