using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Enables calling an async method as "fire and forget" without needing the await.
        /// From https://stackoverflow.com/a/22630057
        /// </summary>
        public static void Forget(this Task task)
        {
            task.ContinueWith(
                t => { Log.Instance.log(t.Exception.Message, LogSeverity.Error); },
                TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}