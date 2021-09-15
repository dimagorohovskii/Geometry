using Geometry.Core.Objects;
using System;
using System.ComponentModel;

namespace Geometry.Core.MVVM.Model
{
    /// <summary>
    /// Модель, регулирующая логику поведения расположенных на холсте фигур
    /// </summary>
    public interface IGeometryCanvasModel : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Интервал в мс между обновлениями фиггур
        /// </summary>
        double UpdatingDelay { get; set; }
        /// <summary>
        /// Продолжительность последней перерисовки в мс
        /// </summary>
        long RedrawDuration { get; }

        /// <summary>
        /// Добавить фигуру на холст
        /// </summary>
        /// <param name="shape">Фигура, что будет добавлена на холст</param>
        void AddShape(GeometryShape shape);
    }
}
