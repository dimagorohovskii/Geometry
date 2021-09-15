namespace Geometry.Core.MVVM.ViewModel.Wraps
{
    /// <summary>
    /// Обобщенный интерфейс модели графического объекта выборки, позволяющей сделать выбор в пользу привязанного к ней объекта
    /// </summary>
    /// <typeparam name="T">Тип привязанного к кнопке объекта</typeparam>
    public interface ISelectionObject<T>
    {
        /// <summary>
        /// Пояснительная подпись кнопки, отображаемая пользователю
        /// </summary>
        string UserFriendlyName { get; }
        /// <summary>
        /// Объект, в пользу которого объекь плхвлдяеь сделать выбор
        /// </summary>
        T BindedObject { get; }
    }
}
