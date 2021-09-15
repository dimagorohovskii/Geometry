using Geometry.Core.MVVM;
using Geometry.Core.MVVM.Model;
using Geometry.Core.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Geometry.Model
{
    /// <summary>
    /// Модель, через равные промежутки времени перемещающая все вверенные ей фигуры по холсту
    /// </summary>
    internal class GeometryCanvasModel : BindableBase, IGeometryCanvasModel
    {
        #region Константы
        /// <summary>
        /// Задержка в милисекундах между проверками модели на наличие перегрузок перерисовкой
        /// </summary>
        private const int OVERLOAD_CHECKING_DELAY = 5;
        #endregion
        #region Поля
        private readonly object _shapesAccessSync = new object();
        private readonly object _shapesMovingTaskAccessSync = new object();
        private readonly object _disposingLock = new object();
        /// <summary>
        /// Загеристрированные на модель фигуры
        /// </summary>
        private readonly IList<GeometryShape> _shapes;
        /// <summary>
        /// Таймер, отсчитывающий задержку между перемещениями
        /// </summary>
        private readonly Timer _shapesMovingDelay;
        /// <summary>
        /// Текущая фоновая задача по перемещению фигур
        /// </summary>
        private Task _shapesMovingTask;
        /// <summary>
        /// Токен, ответственный за отмену текущей фоновой задачи по перерисовке фигур
        /// </summary>
        private CancellationTokenSource _shapesMovingCancellationTolenSource;
        private bool _isDisposed = false;
        private long _lastRedrawDuration;
        #endregion
        #region Свойства
        public double UpdatingDelay
        {
            get
            {
                lock (_disposingLock)
                {
                    if (_isDisposed)
                    {
                        throw new ObjectDisposedException(nameof(GeometryCanvasModel));
                    }

                    return _shapesMovingDelay.Interval;
                }
            }

            set
            {
                lock (_disposingLock)
                {
                    if (_isDisposed)
                    {
                        throw new ObjectDisposedException(nameof(GeometryCanvasModel));
                    }

                    if (_shapesMovingDelay.Interval == value)
                    {
                        return;
                    }

                    _shapesMovingDelay.Interval = value;
                    OnPropertyChanged(nameof(UpdatingDelay));
                }
            }
        }

        public long RedrawDuration
        {
            get => _lastRedrawDuration;

            protected set => SetValue(value, ref _lastRedrawDuration, nameof(RedrawDuration));
        }
        #endregion
        #region Конструкторы и деструкторы
        /// <summary>
        /// Создать модель, перемещающую все фигуры с указанным интервалом
        /// </summary>
        /// <param name="movingInterval">Интервал в мс, через который будут перемещаться фигуры</param>
        public GeometryCanvasModel(double movingInterval)
        {
            _shapes = new List<GeometryShape>();
            _shapesMovingDelay = new Timer(movingInterval)
            {
                AutoReset = true
            };

            _shapesMovingDelay.Elapsed += OnDelayElapsed;
            _shapesMovingDelay.Start();
        }
        ~GeometryCanvasModel()
        {
            Dispose(false);
        }
        #endregion
        #region Методы
        #region Публичные методы
        public void AddShape(GeometryShape shape)
        {
            lock (_shapesAccessSync)
            {
                if (shape == null)
                {
                    throw new ArgumentNullException(nameof(shape));
                }

                if (!_shapes.Contains(shape))
                {
                    _shapes.Add(shape);
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
        #region Приватные методы
        /// <summary>
        /// Действия, выполняемые по окончании задержки между перерисовками фигур
        /// </summary>
        /// <param name="sender">Таймер, оповестивший об окончании времени задежки</param>
        /// <param name="e">Данные таймера</param>
        private void OnDelayElapsed(object sender, ElapsedEventArgs e)
        {
            if (CanStartNewTask())
            {
                RunShapesMovingTask();
            }
            else
            {
                ResolveShapesOverload(OVERLOAD_CHECKING_DELAY);
            }
        }



        /// <summary>
        /// Запустить фоновую задачу по перемещению всех фигур. Замещает текущую фоновую задачу
        /// </summary>
        private void RunShapesMovingTask()
        {
            lock (_shapesMovingTaskAccessSync)
            {
                CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
                CancellationToken token = cancelTokenSource.Token;
                Task formed = Task.Run(() => BacgroundShapesMovingTaskStrategy(token), token);
                _shapesMovingTask = formed;
                _shapesMovingCancellationTolenSource = cancelTokenSource;
            }
        }

        /// <summary>
        /// Разрешение проблемы перегрузки отрисовки
        /// </summary>
        /// <remarks>Если модель не успевает обновить все фигуры за интервал между перерисовками, этот метод
        /// приостанавливает цикл, начиная новую перерисовку только, когда завершится последняя перерисовка</remarks>
        /// <param name="checkInterval">Интервал в мс, с которым происходит проверка допустимости новой перерисовки</param>
        private void ResolveShapesOverload(int checkInterval)
        {
            _shapesMovingDelay.Stop();
            while (!CanStartNewTask())
            {
                Thread.Sleep(checkInterval);
                if (_isDisposed)
                {
                    return;
                }
            }

            DoIfNotDisposing(() =>
            {
                RunShapesMovingTask();
                _shapesMovingDelay.Start();
            });
        }

        /// <summary>
        /// Действия, выполняемый за один проход фоновой задачи по перемещению всех фигур
        /// </summary>
        /// <param name="token">Токен, отвечающий за отмену операций перерисовки</param>
        private void BacgroundShapesMovingTaskStrategy(CancellationToken token)
        {
#if DEBUG
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
#endif
            MoveAllShapes(token);
            lock (_shapesMovingTaskAccessSync)
            {
                _shapesMovingTask = null;
                _shapesMovingCancellationTolenSource = null;
            }

#if DEBUG
            stopwatch.Stop();
            RedrawDuration = stopwatch.ElapsedMilliseconds;
#endif
        }

        /// <summary>
        /// Выполнить перемещение каждой фигуры по их стратегиям поведения
        /// </summary>
        /// <param name="token">Токен, отвечающий за отмену операций перерисовки</param>
        private void MoveAllShapes(CancellationToken token)
        {
            lock (_shapesAccessSync)
            {
                foreach (GeometryShape shape in _shapes)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    shape.ImplementBehavior();
                }
            }
        }
        /// <summary>
        /// Проверить, завершено ли выполнение последней фоновой операции перерисовки
        /// </summary>
        /// <returns>True, если допустимо начать новую задачу. Иначе - False</returns>
        private bool CanStartNewTask()
        {
            lock (_shapesMovingTaskAccessSync)
            {
                return
                    _shapesMovingTask == null ||
                    _shapesMovingTask.IsCanceled ||
                    _shapesMovingTask.IsFaulted ||
                    _shapesMovingTask.IsCompleted;
            }
        }
        #endregion
        #region Защищенные методы
        /// <summary>
        /// Выполнить действие только, если не производится утилизация модели
        /// </summary>
        /// <param name="action">Выполняемое действие</param>
        protected void DoIfNotDisposing(Action action)
        {
            lock (_disposingLock)
            {
                if (!_isDisposed)
                {
                    action.Invoke();
                }
            }
        }


        protected virtual void Dispose(bool disposing)
        {
            lock (_disposingLock)
            {
                if (!_isDisposed)
                {
                    if (disposing)
                    {
                        _shapesMovingDelay.Stop();
                        _shapesMovingDelay.Elapsed -= OnDelayElapsed;
                        if (_shapesMovingCancellationTolenSource != null)
                        {
                            _shapesMovingCancellationTolenSource.Cancel();
                        }

                        _shapesMovingDelay.Dispose();
                    }

                    _isDisposed = true;
                }
            }
        }
        #endregion
        #endregion
    }
}
