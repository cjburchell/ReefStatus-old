namespace RedPoint.ReefStatus.Common.UI.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using System.Windows.Threading;

    /// <summary>
    /// Safe Observable collection
    /// </summary>
    /// <typeparam name="T">the type of the collection</typeparam>
    public class SafeObservableCollection<T> : ObservableCollection<T>
    {
        /// <summary>
        /// The Dispatcher thread
        /// </summary>
        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        protected override void InsertItem(int index, T item)
        {
            this.dispatcher.Invoke(() => base.InsertItem(index, item));
        }

        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            this.dispatcher.Invoke(() => base.RemoveItem(index));
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index.</param>
        protected override void SetItem(int index, T item)
        {
            this.dispatcher.Invoke(() => base.SetItem(index, item));
        }
    }
}
