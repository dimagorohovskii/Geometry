using Geometry.Core.Objects;
using System;

namespace Geometry.GeometryBehaviors
{
    /// <summary>
    /// Примерная стратегия "падения" фигуры
    /// </summary>
    internal class FallBehavior : IShapeBehavior
    {
        private readonly CanvasSize _actualCanvasSize;
        private readonly Random _random;
        /// <summary>
        /// Сформировать стратегию падения фигуры
        /// </summary>
        /// <param name="random">База рандомого перемещения элемента при уходе за экран</param>
        /// <param name="canvasSize">Ссылка на объект с актуальными размерами холста</param>
        public FallBehavior(Random random, CanvasSize canvasSize)
        {
            _random = random ?? throw new ArgumentNullException(nameof(random));
            _actualCanvasSize = canvasSize ?? throw new ArgumentNullException(nameof(canvasSize));
        }
        public void Move(GeometryShape shape)
        {
            shape.Y += 10;
            if (shape.Y > _actualCanvasSize.Height)
            {
                shape.X = _random.Next((int)(_actualCanvasSize.Width - shape.Width));
                shape.Y = -shape.Height;
            }
        }
    }
}
