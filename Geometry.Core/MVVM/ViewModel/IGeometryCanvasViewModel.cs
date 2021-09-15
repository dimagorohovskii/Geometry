using Geometry.Core.MVVM.ViewModel.Wraps;
using Geometry.Core.Objects;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

namespace Geometry.Core.MVVM.ViewModel
{
    /// <summary>
    /// Модель представления, отвечающая за отображение 
    /// </summary>
    public interface IGeometryCanvasViewModel : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Команда, создающая новую фигуру на холсте
        /// </summary>
        ICommand CreateShapeCommand { get; }
        /// <summary>
        /// Интервал в мс между обновлениями фиггур
        /// </summary>
        double UpdatingDelay { get; set; }
        /// <summary>
        /// Продолжительность последней перерисовки в мс
        /// </summary>
        long RedrawDuration { get; }
        /// <summary>
        /// Сколько объектов создавать за один клик
        /// </summary>
        uint CreatedPerClick { get; set; }
        /// <summary>
        /// Количество фигур на холсте
        /// </summary>
        int ShapesCount { get; }
        /// <summary>
        /// Текущий выбранный пользователем цвет для новых создаваемых фигур
        /// </summary>
        Color SelectedColor { get; set; }
        /// <summary>
        /// Стратегия поведения новых создаваемых фигур
        /// </summary>
        IShapeBehavior NewShapeBehavior { get; set; }
        /// <summary>
        /// Текущие параметры обрабатываемого холста
        /// </summary>
        CanvasSize CurrentCanvasSize { get; }
        /// <summary>
        /// Доступные пользователю для выбора фигуры
        /// </summary>
        ObservableCollection<ISelectionObject<Enum>> AvailableShapes { get; }
        /// <summary>
        /// Доступные пользователю для выбора модели поведения фигур
        /// </summary>
        ObservableCollection<ISelectionObject<IShapeBehavior>> AvailableShapesBehaviors { get; }
        /// <summary>
        /// Текущий набор фигур на холсте
        /// </summary>
        ObservableCollection<GeometryShape> ShapesOnCanvas { get; }
    }
}
