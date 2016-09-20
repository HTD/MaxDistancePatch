using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Woof.Core.Collections {

    public static class ObservableCollectionExtensions {

        /// <summary>
        /// Removes all items from the collection which satisfy the predicate.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="collection">Collection to delete from.</param>
        /// <param name="predicate">Remove condition.</param>
        public static void Remove<T>(this ObservableCollection<T> collection, Func<T, bool> predicate) {
            foreach (var item in collection.Where(predicate).ToArray()) collection.Remove(item);
        }

    }

}