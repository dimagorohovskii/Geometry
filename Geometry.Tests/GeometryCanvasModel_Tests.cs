using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Media;
using Geometry.Core.Objects;
using Geometry.GeometryBehaviors;
using Geometry.GeometryShapes;
using Geometry.Model;
using Geometry.Tests.TestObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Timer = System.Timers.Timer;

namespace Geometry.Tests
{
    [TestClass]
    public class GeometryCanvasModel_Tests
    {
        [TestMethod("Проверка корректности отработки утилизации модели")]
        [Description("Запускаем модель в работу и, дождавшись старта фоновых процессов, утилизируем. Все фоновые процессы должны завершиться.")]
        [TestCategory("IDisposable")]
        public void CorrectDisposeTest()
        {
            Task shapesMovingTask = null;
            Timer shapesMovingDelay;
            using (GeometryCanvasModel testingModel = new GeometryCanvasModel(1))
            {
                PrivateObject testingModelPrivateAccess = new PrivateObject(testingModel);
                shapesMovingDelay = testingModelPrivateAccess.GetField("_shapesMovingDelay") as Timer;
                FillModelByRandomShapes(testingModel, 10);
                shapesMovingTask = WaitForBackgroundRedrawingTask(testingModelPrivateAccess);
            }

            Thread.Sleep(1);
            Assert.ThrowsException<ObjectDisposedException>(shapesMovingDelay.Start, "Таймер не утилизирован");
            Assert.IsTrue(shapesMovingTask == null || shapesMovingTask.IsCanceled || shapesMovingTask.IsCompleted, "Фоновые задачи не завершены");
        }

        [TestMethod("Проверка допустимости множественного вызова Dispose")]
        [Description("Dispose должно быть допустимо вызывать множество раз без исключений.")]
        [TestCategory("IDisposable")]
        public void MultyDisposingTest()
        {
            GeometryCanvasModel testingModel = new GeometryCanvasModel(1);
            for (int i = 0; i < 500; i++)
            {
                testingModel.Dispose();
            }
        }


        [TestMethod("Проверка процесса разрешения перегрузок перерисовки")]
        [Description("Если перерисовка не успевает завершить обработку фигур за время задержки между перерисовками, следующий процесс перерисовки должен стартовать сразу" +
            "после завершения предыдущего, а отсчет задержки продолжится с момента старта нового процесса перерисовки")]
        [TestCategory("Redrawing")]
        public void OverloadResolvingTest()
        {
            const int delayBetweenRedrawings = 1;
            //Допустимая процентная погрешность для сравнения времени выполнения задачи и времени между двумя задачами
            //Если значения этих времен расходятся менее, чем на процент погрешности, принимается, что алгоритм отработал корректно
            const double acceptableErrorOfValues = 0.1;
            //Допустимый процент пробуксовок, когда время выполнения задачи расходится с задержкой между задачами более чем на допустимую
            //временную погрешность. Если процент пробуксовок превышает допустимый, тест считается проваленным
            const double acceptableSlippage = 0.1;

            IShapeBehavior shapeBehavior = new ThreadSleeepBehavior(delayBetweenRedrawings);
            GeometryShape testShape = new GeometryEllipse(0, 0, 10, 10, Colors.Red);
            testShape.SetBehavior(shapeBehavior);
            IList<GeometryShape> testShapes = new List<GeometryShape>()
            {
                testShape
            };

            List<RedrawingCycleInfo> redrawingCycleInfos;

            using (GeometryCanvasModel testingModel = new GeometryCanvasModel(delayBetweenRedrawings))
            {
                redrawingCycleInfos = MeasureRedrawParameters(testingModel, testShapes, 1000);
            }

            int correctBehavior = 0, slip = 0;
            foreach (RedrawingCycleInfo info in redrawingCycleInfos)
            {
                double difference = (info.DelayBetweenTasks - info.TaskDuration).TotalMilliseconds;
                double delay = info.DelayBetweenTasks.TotalMilliseconds;
                double error = Math.Abs(difference / delay);
                if (error < acceptableErrorOfValues)
                {
                    correctBehavior++;
                }
                else
                {
                    slip++;
                }
            }

            double actualSlip = (double)slip / correctBehavior;
            Assert.IsTrue(acceptableSlippage > actualSlip, $"Актуальный процент пробуксовок {actualSlip} выше допустимого {acceptableSlippage}.");
        }

        /// <summary>
        /// Замерить временные параметры перерисовки на тестовых данных
        /// </summary>
        /// <param name="testingModel">Тестируемая модель</param>
        /// <param name="testShapes">Тестовые фигуры</param>
        /// <param name="iterationsCount">Количество замеров</param>
        /// <returns>Список промежутков между стартами перерисовки и времен выполнения перерисовки</returns>
        private static List<RedrawingCycleInfo> MeasureRedrawParameters(GeometryCanvasModel testingModel, IEnumerable<GeometryShape> testShapes, int iterationsCount)
        {
            TimeSpan
                    taskStartTime = default,
                    delayBetweenTasks = default;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            PrivateObject testingModelPrivateAccess = new PrivateObject(testingModel);
            foreach (GeometryShape shape in testShapes)
            {
                testingModel.AddShape(shape);
            }

            Task task = WaitForBackgroundRedrawingTask(testingModelPrivateAccess);
            task.Wait(); //Ожидаем окончания задачи, поскольку отслеживание могло начаться не с начала

            List<RedrawingCycleInfo> redrawingCycleInfos = new List<RedrawingCycleInfo>();
            for (int i = 0; i < iterationsCount; i++)
            {
                task = WaitForBackgroundRedrawingTask(testingModelPrivateAccess);
                TimeSpan previousTaskStartTime = taskStartTime;
                taskStartTime = stopwatch.Elapsed;
                if (i != 0)
                {
                    delayBetweenTasks = taskStartTime - previousTaskStartTime;
                }

                task.Wait();
                TimeSpan taskFinishTime = stopwatch.Elapsed;
                if (i != 0)
                {
                    TimeSpan taskDuration = taskFinishTime - taskStartTime;
                    redrawingCycleInfos.Add(new RedrawingCycleInfo(delayBetweenTasks, taskDuration));
                }
            }

            stopwatch.Stop();
            return redrawingCycleInfos;
        }

        /// <summary>
        /// Дождаться старта фоновой задачи перерисовки
        /// </summary>
        /// <param name="modelPrivateObject">Обертка модели, для которой будет ожидаться старт задачи</param>
        /// <returns>Начатая задача</returns>
        private static Task WaitForBackgroundRedrawingTask(PrivateObject modelPrivateObject)
        {
            Task shapesMovingTask = null;
            while (shapesMovingTask == null)
            {
                shapesMovingTask = modelPrivateObject.GetField("_shapesMovingTask") as Task;
            }

            return shapesMovingTask;
        }

        /// <summary>
        /// Заполнить модель случайными фигурами
        /// </summary>
        /// <param name="model">Заполняемая модель</param>
        /// <param name="shapesCount">Количество фигур, что будет добавлено в модель</param>
        private static void FillModelByRandomShapes(GeometryCanvasModel model, int shapesCount)
        {
            ShapesFactory factory = new ShapesFactory();
            GeometryShapesTypes[] availableTypes =
                factory.GetShapesSelectors()
                .Select(s => s.BindedObject)
                .OfType<GeometryShapesTypes>()
                .ToArray();
            if (availableTypes.Length == 0)
            {
                throw new InvalidOperationException($"Поскольку допустимых для создания объектов по шаблонам {typeof(GeometryShapesTypes).FullName} нет, выполнить операцию невозможно");
            }

            Random random = new Random();
            for (int i = 0; i < shapesCount; i++)
            {
                GeometryShape shape = factory.FormGeometry(availableTypes[random.Next(availableTypes.Length)], Colors.Red);
                model.AddShape(shape);
            }
        }
    }
}
