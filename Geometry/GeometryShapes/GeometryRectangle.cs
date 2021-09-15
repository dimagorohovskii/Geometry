using Geometry.Core.Objects;
using System.Windows.Media;

namespace Geometry.GeometryShapes
{
    /// <summary>
    /// Класс, описывающий модель прямоугольника
    /// </summary>
    internal class GeometryRectangle : GeometryShape
    {
        public GeometryRectangle(double x, double y, double width, double height, Color color) : base(x, y, width, height, color)
        {
        }
    }
}
