using Geometry.Core.Objects;
using System.Windows.Media;

namespace Geometry.GeometryShapes
{
    /// <summary>
    /// Класс, описывающий модель эллипса
    /// </summary>
    internal class GeometryEllipse : GeometryShape
    {
        public GeometryEllipse(double x, double y, double width, double height, Color color) : base(x, y, width, height, color)
        {
        }
    }
}
