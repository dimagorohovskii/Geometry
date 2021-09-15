using Geometry.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Geometry.Tests.TestObjects
{
    /// <summary>
    /// Тестовая модель поведения, обработка которого занимает указанное время
    /// </summary>
    internal class ThreadSleeepBehavior : IShapeBehavior
    {
        private readonly int _movingDuration;
        /// <summary>
        /// Создать модель поведения, обработка которой в холостую отрабатывает заданное время
        /// </summary>
        /// <param name="movingDuration">Время в мс холостой отработки</param>
        public ThreadSleeepBehavior(int movingDuration)
        {
            if (movingDuration < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(movingDuration));
            }

            _movingDuration = movingDuration;
        }
        public void Move(GeometryShape shape)
        {
            Thread.Sleep(_movingDuration);
        }
    }
}
