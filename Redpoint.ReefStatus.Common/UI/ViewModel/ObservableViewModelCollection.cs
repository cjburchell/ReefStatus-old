// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableViewModelCollection.cs" company="Redpoint Apps">
//   2009
// </copyright>
// <summary>
//   A collection of Observable view models
// </summary>
// --------------------------------------------------------------------------------------------------------------------





namespace RedPoint.ReefStatus.Common.UI.ViewModel
{
	
	using System;
	using System.Collections.ObjectModel;

	using System.ComponentModel;
	using System.Windows.Threading;
	
    /// <summary>
    /// A collection of Observable view models
    /// </summary>
    /// <typeparam name="T">An View Model</typeparam>
    public class ObservableViewModelCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        protected override void InsertItem(int index, T item)
        {
            dispatcher.Invoke(new Action(() => BaseInsertItem(index, item)));
            item.PropertyChanged += this.ItemPropertyChanged;
        }

        /// <summary>
        /// Bases the insert item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        private void BaseInsertItem(int index, T item)
        {
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            T item = this[index];
            item.PropertyChanged -= this.ItemPropertyChanged;
            base.RemoveItem(index);
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index.</param>
        protected override void SetItem(int index, T item)
        {
            var oldItem = this[index];
            oldItem.PropertyChanged -= this.ItemPropertyChanged;
            this.dispatcher.Invoke(new Action(() => base.SetItem(index, item)));
            item.PropertyChanged += this.ItemPropertyChanged;
        }

        /// <summary>
        /// Items the property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            int index = IndexOf((T)sender);
            this[index] = this[index];
        }
    }
}