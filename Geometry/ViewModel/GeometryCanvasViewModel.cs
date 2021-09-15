using Geometry.Core.MVVM;
using Geometry.Core.MVVM.Model;
using Geometry.Core.MVVM.ViewModel;
using Geometry.Core.MVVM.ViewModel.Wraps;
using Geometry.Core.Objects;
using Geometry.GeometryShapes;
using Geometry.View.Wraps;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace Geometry.ViewModel
{
    /// <summary>
    /// Модель представления, собирающая параметры формирования фигур и наносящая созданные фигуры на холст
    /// </summary>
    internal class GeometryCanvasViewModel : BindableBase, IGeometryCanvasViewModel
    {
        #region Поля
        private readonly IGeometryCanvasModel _model;
        private readonly ShapesFactory _shapesFactory;
        private Color _selectedColor;
        private IShapeBehavior _newShapeBehavior;
        private bool _isDisposed;
        private uint _createPerClick;
        #endregion
        #region Свойства
        public ICommand CreateShapeCommand { get; }

        public Color SelectedColor
        {
            get => _selectedColor;

            set => SetValue(value, ref _selectedColor);
        }

        public ObservableCollection<ISelectionObject<Enum>> AvailableShapes { get; }

        public ObservableCollection<ISelectionObject<IShapeBehavior>> AvailableShapesBehaviors { get; }

        public ObservableCollection<GeometryShape> ShapesOnCanvas { get; }

        public IShapeBehavior NewShapeBehavior
        {
            get => _newShapeBehavior;

            set => SetValue(value, ref _newShapeBehavior);
        }

        public double UpdatingDelay
        {
            get => _model.UpdatingDelay;

            set
            {
                if (_model.UpdatingDelay == value)
                {
                    return;
                }

                _model.UpdatingDelay = value;
                OnPropertyChanged(nameof(UpdatingDelay));
            }
        }

        public CanvasSize CurrentCanvasSize { get; }

        public long RedrawDuration => _model.RedrawDuration;

        public uint CreatedPerClick
        {
            get => _createPerClick;

            set => SetValue(value, ref _createPerClick, nameof(CreatedPerClick));
        }

        public int ShapesCount => ShapesOnCanvas.Count;
        #endregion
        #region Конструкторы и деструкторы
        public GeometryCanvasViewModel(IGeometryCanvasModel model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _model.PropertyChanged += OnModelPropertyChanged;
            _shapesFactory = new ShapesFactory();
            CurrentCanvasSize = new CanvasSize();
            AvailableShapes = new ObservableCollection<ISelectionObject<Enum>>();
            AvailableShapesBehaviors = new ObservableCollection<ISelectionObject<IShapeBehavior>>();
            ShapesOnCanvas = new ObservableCollection<GeometryShape>();
            CreateShapeCommand = new RelayCommand<GeometryShapesTypes>(CreateShape);
            BaseDataInit();
        }

        ~GeometryCanvasViewModel()
        {
            Dispose(false);
        }
        #endregion
        #region Методы
        #region Публичные методы
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
        #region Приватные методы
        /// <summary>
        /// Заполнение свойств базовыми данными
        /// </summary>
        private void BaseDataInit()
        {
            SelectedColor = Colors.Red;
            CreatedPerClick = 1;
            foreach (SelectionObject<Enum> shapeType in _shapesFactory.GetShapesSelectors())
            {
                AvailableShapes.Add(shapeType);
            }

            foreach (SelectionObject<IShapeBehavior> shapeBehavior in _shapesFactory.GetBehaviorsSelectors(CurrentCanvasSize))
            {
                AvailableShapesBehaviors.Add(shapeBehavior);
            }

            if (AvailableShapesBehaviors.Count != 0)
            {
                NewShapeBehavior = AvailableShapesBehaviors.First().BindedObject;
            }
        }
        /// <summary>
        /// Создание на холсте новой фигуры указанного типа с текущим выбранным поведением заданное количество раз
        /// </summary>
        /// <param name="newShapeType">Тип новой создаваемой фигуры</param>
        private void CreateShape(GeometryShapesTypes newShapeType)
        {
            GeometryShapesTypes currentType = newShapeType;
            Color currentColor = SelectedColor;
            IShapeBehavior currentBehavior = NewShapeBehavior;
            uint creations = CreatedPerClick;
            for (uint i = 0; i < creations; i++)
            {
                GeometryShape shape = _shapesFactory.FormGeometry(currentType, currentColor);
                shape.SetBehavior(currentBehavior);
                ShapesOnCanvas.Add(shape);
                _model.AddShape(shape);
            }

            OnPropertyChanged(nameof(ShapesCount));
        }
        /// <summary>
        /// Перехватчик измененных свойств модели
        /// </summary>
        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_model.RedrawDuration):
                    OnPropertyChanged(nameof(RedrawDuration));
                    break;
            }
        }
        #endregion
        #region Защищенные методы
        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _model.PropertyChanged -= OnModelPropertyChanged;
                    _model.Dispose();
                }

                _isDisposed = true;
            }
        }
        #endregion
        #endregion
    }
}
