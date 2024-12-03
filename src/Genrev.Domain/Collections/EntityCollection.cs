using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.Collections
{

    /// <summary>
    /// Use this as a replacement for the usual Collection/ICollection types by EntityFramework.
    /// This allows for a read-only domain model collection while EF will use the unboxed version to interact.
    /// http://stackoverflow.com/questions/11191103/entity-framework-read-only-collections
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityCollection<T> : System.Collections.ObjectModel.Collection<T>
    {

        [Obsolete("Unboxing this collection is only allowed in the declarating class.", true)]
        public new void Add(T item) { }

        [Obsolete("Unboxing this collection is only allowed in the declarating class.", true)]
        public new void Clear() { }

        [Obsolete("Unboxing this collection is only allowed in the declarating class.", true)]
        public new void Insert(int index, T item) { }

        [Obsolete("Unboxing this collection is only allowed in the declarating class.", true)]
        public new void Remove(T item) { }

        [Obsolete("Unboxing this collection is only allowed in the declarating class.", true)]
        public new void RemoveAt(int index) { }

        public static implicit operator EntityCollection<T>(List<T> source)
        {
            var target = new EntityCollection<T>();
            foreach (var item in source)
            {
                ((System.Collections.ObjectModel.Collection<T>)target).Add(item); // unbox
            }
            return target;
        }

    }

    public static class EntityCollectionHelper
    {

        /// <summary>
        /// Handles the generic plumbing for adding an item to the collection.  Instantiates the collection if required.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection to add the item to</param>
        /// <param name="item">The item to add</param>
        /// <param name="allowDuplicate">true to allow duplicate items to be entered, false otherwise</param>
        /// <param name="errOnDuplicateAttempt">if duplicates not allowed, true to throw an error on attempted duplicate entry, false to return without error</param>
        /// <returns>/>Updated List that must be set in the calling method</returns>
        /// <exception cref="InvalidOperationException">Cannot enter duplicate item to collection</exception> 
        public static EntityCollection<T> AddItem<T>(EntityCollection<T> collection, T item, bool allowDuplicate = false, bool errOnDuplicateAttempt = false)
        {

            if (collection == null)
            {
                collection = new EntityCollection<T>();
            }

            if (!allowDuplicate)
            {
                if (collection.Contains(item))
                {
                    if (errOnDuplicateAttempt)
                    {
                        throw new InvalidOperationException("Cannot enter duplicate item to collection");
                    }
                    else
                    {
                        return collection;
                    }
                }
            }

            var list = collection.ToList();
            list.Add(item);

            return list;
        }

        public static EntityCollection<T> RemoveItem<T>(EntityCollection<T> collection, T item)
        {
            var list = collection.ToList();
            list.Remove(item);
            return list;
        }

    }

}
