using Geometry.Core.Objects;
using System;

namespace Geometry.GeometryBehaviors
{
    internal class FlyingBehavior : IShapeBehavior
    {
        private readonly CanvasSize _actualCanvasSize;
        private readonly Random _random;
        /// <summary>
        /// Сформировать стратегию пролета фигуры
        /// </summary>
        /// <param name="random">База рандомого перемещения элемента при уходе за экран</param>
        /// <param name="canvasSize">Ссылка на объект с актуальными размерами холста</param>
        public FlyingBehavior(Random random, CanvasSize canvasSize)
        {
            _random = random ?? throw new ArgumentNullException(nameof(random));
            _actualCanvasSize = canvasSize ?? throw new ArgumentNullException(nameof(canvasSize));
        }
        public void Move(GeometryShape shape)
        {
            shape.X += 10;
            if (shape.X > _actualCanvasSize.Width)
            {
                shape.Y = _random.Next((int)(_actualCanvasSize.Height - shape.Height));
                shape.X = -shape.Width;
            }
        }
    }
}
