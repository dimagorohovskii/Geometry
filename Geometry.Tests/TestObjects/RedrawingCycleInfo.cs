using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry.Tests.TestObjects
{
    /// <summary>
    /// информация о цикле перерисовки
    /// </summary>
    internal struct RedrawingCycleInfo
    {
        /// <summary>
        /// Время, проходящее между стартами двух циклов перерисовки
        /// </summary>
        public TimeSpan DelayBetweenTasks { get; set; }
        /// <summary>
        /// Продолжительность цикла перерисовки
        /// </summary>
        public TimeSpan TaskDuration { get; set; }
        public RedrawingCycleInfo(TimeSpan delayBetweenTasks, TimeSpan taskDuration)
        {
            DelayBetweenTasks = delayBetweenTasks;
            TaskDuration = taskDuration;
        }

        public override string ToString()
        {
            return $"Между стартами: {DelayBetweenTasks}; Продолжительность: {TaskDuration}";
        }
    }
}
