using Geometry.Core.MVVM;
using System;
using System.Windows.Media;

namespace Geometry.Core.Objects
{
    /// <summary>
    /// Обобщенный класс, представляющий геометрическую фигуру, описывающий общие правила её поведения на холсте
    /// </summary>
    public abstract class GeometryShape : BindableBase
    {
        #region Поля
        /// <summary>
        /// Установленное поведение текущей фигуры
        /// </summary>
        private IShapeBehavior _shapeBehavior;
        private readonly object _shapeBehaviorAccessSync = new object();
        #region Поля, скрытые свойствами
        private double _x, _y, _width, _height;
        private Color _shapeColor;
        #endregion
        #endregion
        #region Свойства
        /// <summary>
        /// Координата фигуры по оси OX
        /// </summary>
        public double X
        {
            get => _x;

            set
            {
                if (!IsDoubleAllowable(value))
                {
                    throw new ArgumentException("Недопустимое значение", nameof(X));
                }

                SetValue(value, ref _x, nameof(X));
            }
        }
        /// <summary>
        /// Координата фигуры по оси OY 
        /// </summary>
        public double Y
        {
            get => _y;

            set
            {
                if (!IsDoubleAllowable(value))
                {
                    throw new ArgumentException("Недопустимое значение", nameof(Y));
                }

                SetValue(value, ref _y, nameof(Y));
            }
        }
        /// <summary>
        /// Ширина фигуры
        /// </summary>
        public double Width
        {
            get => _width;

            protected set
            {
                if (!IsDoubleAllowable(value))
                {
                    throw new ArgumentException("Недопустимое значение", nameof(Width));
                }

                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Width));
                }

                SetValue(value, ref _width, nameof(Width));
            }
        }
        /// <summary>
        /// Высота фигуры
        /// </summary>
        public double Height
        {
            get => _height;

            protected set
            {
                if (!IsDoubleAllowable(value))
                {
                    throw new ArgumentException("Недопустимое значение", nameof(Height));
                }

                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Height));
                }

                SetValue(value, ref _height, nameof(Height));
            }
        }
        /// <summary>
        /// Цвет установленной фигуры
        /// </summary>
        public Color ShapeColor
        {
            get => _shapeColor;

            protected set => SetValue(value, ref _shapeColor, nameof(ShapeColor));
        }
        #endregion
        #region Конструкторы и деструкторы
        /// <summary>
        /// Сформировать фигуру по заданным параметрам
        /// </summary>
        /// <param name="x">Начальная координата фигуры по оси OX</param>
        /// <param name="y">Начальная координата фигуры по оси OY</param>
        /// <param name="width">Начальная ширина фигуры</param>
        /// <param name="height">Начальная высота фигуры</param>
        /// <param name="color">Начальный цвет фигуры</param>
        public GeometryShape(double x, double y, double width, double height, Color color)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            ShapeColor = color;
        }
        #endregion
        #region Методы
        #region Публичные методы
        /// <summary>
        /// Сигнал фигуре действовать по заданной стратегии поведения
        /// </summary>
        public void ImplementBehavior()
        {
            lock (_shapeBehaviorAccessSync)
            {
                if (_shapeBehavior != null)
                {
                    _shapeBehavior.Move(this);
                }
            }
        }
        /// <summary>
        /// Установить фигуре новую стратегию поведения
        /// </summary>
        /// <param name="shapeBehavior">Новая стратегия поведения фигуры</param>
        public void SetBehavior(IShapeBehavior shapeBehavior)
        {
            lock (_shapeBehaviorAccessSync)
            {
                _shapeBehavior = shapeBehavior;
            }
        }
        #endregion
        #region Приватные методы
        /// <summary>
        /// Является ли число корректным
        /// </summary>
        /// <param name="value">Проверяется ли число</param>
        /// <returns>False, если число NaN или бесконечное, иначе True</returns>
        private static bool IsDoubleAllowable(double value)
        {
            return !(double.IsInfinity(value) || double.IsNaN(value));
        }
        #endregion
        #endregion
    }
}
