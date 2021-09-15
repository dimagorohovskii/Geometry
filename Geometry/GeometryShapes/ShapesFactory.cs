using Geometry.Core.Objects;
using Geometry.GeometryBehaviors;
using Geometry.View.Wraps;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Geometry.GeometryShapes
{
    /// <summary>
    /// Формирует селекторы фигур и сами фигуры по заданным шаблонам
    /// </summary>
    internal class ShapesFactory
    {
        /// <summary>
        /// Получить набор допустимых для обработки фабрикой типов фигур
        /// </summary>
        /// <returns>Набор типов фигур, обернутый в селекторы</returns>
        internal IList<SelectionObject<Enum>> GetShapesSelectors()
        {
            return new List<SelectionObject<Enum>>()
            {
                new SelectionObject<Enum>("Круг", GeometryShapesTypes.Circle),
                new SelectionObject<Enum>("Эллипс", GeometryShapesTypes.Ellipse),
                new SelectionObject<Enum>("Прямоугольник", GeometryShapesTypes.Rectangle),
                new SelectionObject<Enum>("Квадрат", GeometryShapesTypes.Square)
            };
        }

        /// <summary>
        /// Получить набор допустимых для выбора моделей поведения фигур
        /// </summary>
        /// <param name="actualSize">Ссылка на вспомогательный объект с актуальными размерами холста</param>
        /// <returns>Набор моделей поведения фигур, обернутый в селекторы</returns>
        internal IList<SelectionObject<IShapeBehavior>> GetBehaviorsSelectors(CanvasSize actualSize)
        {
            if (actualSize == null)
            {
                throw new ArgumentNullException(nameof(actualSize));
            }

            Random random = new Random();
            return new List<SelectionObject<IShapeBehavior>>()
            {
                new SelectionObject<IShapeBehavior>("Падение", new FallBehavior(random, actualSize)),
                new SelectionObject<IShapeBehavior>("Пролет", new FlyingBehavior(random, actualSize))
            };
        }

        /// <summary>
        /// Сформировать выбранную фигуру по одному из заготовленных шаблонов
        /// </summary>
        /// <param name="type">Тип формируемой фигуры</param>
        /// <param name="color">Цвет, которым будет закращена фигура</param>
        /// <returns>Фигура, сформированная по заданному шаблону</returns>
        internal GeometryShape FormGeometry(GeometryShapesTypes type, Color color)
        {
            switch (type)
            {
                case GeometryShapesTypes.Circle:
                    return new GeometryEllipse(10, 10, 10, 10, color);
                case GeometryShapesTypes.Ellipse:
                    return new GeometryEllipse(10, 10, 10, 20, color);
                case GeometryShapesTypes.Rectangle:
                    return new GeometryRectangle(10, 10, 10, 20, color);
                case GeometryShapesTypes.Square:
                    return new GeometryRectangle(10, 10, 10, 10, color);
                default:
                    throw new ArgumentException($"Для фигуры {type} отсутствует шаблон");
            }
        }
    }
}
