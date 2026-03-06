using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Propla
{
    /// <summary>
    /// ObservableObject
    /// </summary>
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロパティセット
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingStore"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <param name="onChange"></param>
        protected void SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChange = null)
        {
            if (Equals(backingStore, value))
                return;
            backingStore = value;
            onChange?.Invoke();
            OnPropertyChanged(propertyName);
            return;
        }
        /// <summary>
        /// プロパティ変更
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// プロパティセット
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingStore"></param>
        /// <param name="backingStorePropertyName"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <param name="onChanged"></param>
        // 値クラスを内部に持つViewModelを作る場合
        protected void SetProperty<T>(object backingStore, string backingStorePropertyName, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
        {
            var pi = backingStore.GetType().GetProperty(backingStorePropertyName);
            if (pi == null)
                return;
            if (pi.PropertyType != value.GetType())
                return;
            pi.SetMethod.Invoke(backingStore, new object[] { value });
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return;
        }

    }
}
