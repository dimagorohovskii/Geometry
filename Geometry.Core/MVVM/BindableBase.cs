using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Geometry.Core.MVVM
{
    /// <summary>
    /// Вспомогательный класс, реализующий основные элементы, формирующие щаблон MVVM
    /// </summary>
    public abstract class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Установить значение в выбранную переменную и вызвать <seealso cref="PropertyChanged"/>, если значение было изменено
        /// </summary>
        /// <typeparam name="T">Тип присваеваемого значения</typeparam>
        /// <param name="value">Присваеваемое значение</param>
        /// <param name="variable">Переменная, которой будет присвоено значение</param>
        /// <param name="propertyName">Значение свойства, что следует обновить</param>
        protected virtual void SetValue<T>(T value, ref T variable, [CallerMemberName] string propertyName = "")
        {
            if (variable != null && variable.Equals(value))
            {
                return;
            }

            variable = value;
            OnPropertyChanged(propertyName);
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
