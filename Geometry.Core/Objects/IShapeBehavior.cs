namespace Geometry.Core.Objects
{
    /// <summary>
    /// Интерфейс, реализующий стратегиию простейшего поведения конкретной фигуры
    /// </summary>
    public interface IShapeBehavior
    {
        /// <summary>
        /// Сделать шаг фигурой
        /// </summary>
        /// <param name="shape">Фигура, выполняющая шаг</param>
        void Move(GeometryShape shape);
    }
}
