using Geometry.GeometryShapes;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Geometry.View.DataTemplateSelectors
{
    internal class ShapesCanvasDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            if (item == null || element == null)
            {
                return null;
            }

            if (item is GeometryEllipse)
            {
                return element.FindResource("ellipseTemplate") as DataTemplate;
            }
            else if (item is GeometryRectangle)
            {
                return element.FindResource("rectangleTemplate") as DataTemplate;
            }
            else
            {
                throw new ArgumentException($"Для объекта {item.GetType().FullName} отсутствует шаблон");
            }
        }
    }
}
