using Geometry.Core.MVVM.ViewModel.Wraps;
using Geometry.Core.Objects;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Geometry.View.Converters
{
    internal class BehaviorPresenterToBehaviorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ISelectionObject<IShapeBehavior> behaviorContainer))
            {
                return null;
            }
            else
            {
                return behaviorContainer.BindedObject;
            }
        }
    }
}
