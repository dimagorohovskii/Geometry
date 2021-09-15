using Geometry.Core.MVVM.ViewModel;
using Geometry.Model;
using Geometry.ViewModel;
using System;
using System.Windows.Controls;

namespace Geometry.View
{
    /// <summary>
    /// Логика взаимодействия для GeometryCanvas.xaml
    /// </summary>
    public partial class GeometryCanvas : UserControl, IDisposable
    {
        #region Поля
        private readonly IGeometryCanvasViewModel _viewModel;
        private bool _isDisposed = false;
        #endregion

        #region Конструкторы и деструкторы
        public GeometryCanvas()
        {
            InitializeComponent();
            Dispatcher.ShutdownStarted += OnDispatcherShutdownStarted;
            IGeometryCanvasViewModel viewModel = new GeometryCanvasViewModel(
                new GeometryCanvasModel(1));
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        ~GeometryCanvas()
        {
            Dispose(false);
        }
        #endregion

        #region Методы
        #region Публичные
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
        #region Приватные
        private void Canvas_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            _viewModel.CurrentCanvasSize.Width = e.NewSize.Width;
            _viewModel.CurrentCanvasSize.Height = e.NewSize.Height;
        }
        private void OnDispatcherShutdownStarted(object sender, EventArgs e)
        {
            Dispose();
        }
        #endregion
        #region Защищенные
        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Dispatcher.ShutdownStarted -= OnDispatcherShutdownStarted;
                    _viewModel.Dispose();
                }

                _isDisposed = true;
            }
        }
        #endregion
        #endregion
    }
}
