using Geometry.Core.MVVM.ViewModel.Wraps;

namespace Geometry.View.Wraps
{
    internal class SelectionObject<T> : ISelectionObject<T>
    {
        public string UserFriendlyName { get; }

        public T BindedObject { get; }

        public SelectionObject(string userFriendlyName, T bindedObject)
        {
            UserFriendlyName = userFriendlyName;
            BindedObject = bindedObject;
        }
    }
}
