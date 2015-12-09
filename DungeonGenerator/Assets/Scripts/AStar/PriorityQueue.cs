using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.AStar
{

    /// <summary> A generic priority queue implementation. </summary>
    /// <remarks> David Venegoni, Jan 02 2014. </remarks>
    /// <seealso> cref="T:System.Collections.Generic.IEnumerable{T}". </seealso>
    /// <seealso> cref="T:Utilities.Interfaces.IPriorityQueue{T}". </seealso>
    /// <typeparam name="T"> Generic type parameter, where T 
    ///                      must implement the IComparable interface. </typeparam>
    /// Further modified to fit in unity context.
    /// Date: 09/12/2015
    /// User: Mikkel Stolborg
    public class PriorityQueue<T> : IEnumerable<T>, IPriorityQueue<T>
        where T : IComparable<T>
    {
        #region Private Member Variables

        private readonly List<T> items; /* The items in the queue */

        #endregion

        #region Properties

        /// <summary> Gets the convention this priority queue uses to sort and insert items. </summary>
        /// <value> The ordering convention. </value>
        public PriorityConvention OrderingConvention= PriorityConvention.HighestPriorityInFront;

        /// <summary> Gets the number of items that are in the priority queue. </summary>
        /// <value> The number of items in the priority queue. </value>
        public Int32 Count
        {
            get { return items.Count; }
        }

        #endregion

        #region Constructors
        public PriorityQueue()
        {
            OrderingConvention = PriorityConvention.HighestPriorityInFront;
            items = new List<T>();
        }
        /// <summary> Initializes a new instance of the PriorityQueue class. </summary>
        /// <remarks> David Venegoni, Jan 02 2014. </remarks>
        /// <exception cref="ArgumentException"> Thrown when the convention is specified 
        ///                                      as PriorityConvention.None. </exception>
        /// <param name="convention">
        ///     (Optional) the convention to use when sorting and inserting items (this cannot be changed
        ///     after the priority queue is created).
        /// </param>
        public PriorityQueue(PriorityConvention convention)
        {
            if (convention == PriorityConvention.None)
            {
                throw new ArgumentException("No valid ordering convention was specified", "convention");
            }

            OrderingConvention = convention;
            items = new List<T>();
        }
        /// <summary>
        ///     Adds the items to the priority queue.  This method checks if the enumerable is null 
        ///     and only iterates of the items once.
        /// </summary>
        /// <remarks> David Venegoni, Jan 02 2014. </remarks>
        /// <param name="itemsToAdd"> An IEnumerable&lt;T&gt; of items to add to the priority queue. </param>
        public void AddRange(IEnumerable<T> itemsToAdd)
        {
            if (itemsToAdd == null)
            {
                return;
            }

            foreach (T item in itemsToAdd)
            {
                Insert(item);
            }
        }

        #endregion

        #region Public Methods

        /// <summary> Gets the enumerator for the priority queue. </summary>
        /// <remarks> David Venegoni, Jan 02 2014. </remarks>
        /// <returns> The enumerator for the priority queue. </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        /// <summary> Gets the enumerator for the priority queue. </summary>
        /// <remarks> David Venegoni, Jan 02 2014. </remarks>
        /// <returns> The enumerator for the priority queue. </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary> Adds an item to the priority queue. </summary>
        /// <remarks> David Venegoni, Jan 02 2014. </remarks>
        /// <param name="item"> The item to add. </param>
        public void Add(T item)
        {
            Insert(item);
        }

        /// <summary> Clears all the items from the priority queue. </summary>
        /// <remarks> David Venegoni, Jan 02 2014. </remarks>
        public int Clear()
        {
            return Clear(0);
        }
        /// <summary> Clears all the items starting at the specified start index. </summary>
        /// <remarks> David Venegoni, Jan 03 2014. </remarks>
        /// <param name="startIndex"> The start index. </param>
        /// <returns> The number of items that were removed from the priority queue. </returns>
        public Int32 Clear(Int32 startIndex)
        {
            //Int32 numberOfItems = items.Count - 1 - startIndex;
            //items.RemoveRange(startIndex, numberOfItems);
            return Clear(startIndex, Count - 1 - startIndex); ;
        }

        /// <summary> Clears the number of items specified by count 
        ///           from the priority queue starting at specified start index. </summary>
        /// <remarks> David Venegoni, Jan 03 2014. </remarks>
        /// <param name="startIndex"> The start index. </param>
        /// <param name="count">      Number of items to remove. </param>
        public int Clear(Int32 startIndex, Int32 count)
        {
            items.RemoveRange(startIndex, count);
            return count;
        }

        /// <summary> Clears all the items that satisfy the specified predicate function. </summary>
        /// <remarks> David Venegoni, Jan 03 2014. </remarks>
        /// <param name="predicateFunction"> The predicate function to use in determining 
        ///                                  which items should be removed. </param>
        /// <returns> The number of items that were removed from the priority queue. </returns>
        public Int32 ClearWhere(Func<T, Boolean> predicateFunction)
        {
            return items.RemoveAll(predicateFunction.Invoke);
        }

        /// <summary> Pops an item from the front of the queue. </summary>
        /// <remarks> David Venegoni, Jan 02 2014. </remarks>
        /// <exception cref="InvalidOperationException"> Thrown when no items exist 
        ///                                              in the priority queue. </exception>
        /// <returns> An item from the front of the queue. </returns>
        public T PopFront()
        {
            if (items.Count == 0)
            {
                throw new InvalidOperationException("No elements exist in the queue");
            }

            T item = items[0];
            items.RemoveAt(0);
            return item;
        }

        /// <summary> Pops an item from the back of the queue. </summary>
        /// <remarks> David Venegoni, Jan 02 2014. </remarks>
        /// <exception cref="InvalidOperationException"> Thrown when no items exist
        ///                                              in the priority queue. </exception>
        /// <returns> An item from the back of the queue. </returns>
        public T PopBack()
        {
            if (items.Count == 0)
            {
                throw new InvalidOperationException("No elements exist in the queue");
            }

            Int32 tail = items.Count - 1;
            T item = items[tail];
            items.RemoveAt(tail);
            return item;
        }

        /// <summary> Peeks at the item at the front queue without removing the item. </summary>
        /// <remarks> David Venegoni, Jan 02 2014. </remarks>
        /// <exception cref="InvalidOperationException"> Thrown when no items exist 
        ///                                              in the priority queue. </exception>
        /// <returns> The item at the front of the queue. </returns>
        public T PeekFront()
        {
            if (items.Count == 0)
            {
                throw new InvalidOperationException("No elements exist in the queue");
            }

            return items[0];
        }

        /// <summary> Peeks at the item at the back of the queue without removing the item. </summary>
        /// <remarks> David Venegoni, Jan 02 2014. </remarks>
        /// <exception cref="InvalidOperationException"> Thrown when no items exist 
        ///                                              in the priority queue. </exception>
        /// <returns> The item at the back of the queue. </returns>
        public T PeekBack()
        {
            if (items.Count == 0)
            {
                throw new InvalidOperationException("No elements exist in the queue");
            }

            return items[items.Count - 1];
        }

        /// <summary> Pops the specified number of items from the front of the queue. </summary>
        /// <remarks> David Venegoni, Jan 02 2014. </remarks>
        /// <exception cref="ArgumentException">
        ///     Thrown when the number of items to pop exceeds the number of items in the priority
        ///     queue.
        /// </exception>
        /// <param name="numberToPop"> Number of items to pop from the front of the queue. </param>
        /// <returns> The items from the front of the queue. </returns>
        public IEnumerable<T> PopFront(Int32 numberToPop)
        {
            if (numberToPop > items.Count)
            {
                throw new ArgumentException(@"The numberToPop exceeds the number 
                                            of elements in the queue", "numberToPop");
            }

            var poppedItems = new List<T>();
            while (poppedItems.Count < numberToPop)
            {
                poppedItems.Add(PopFront());
            }

            return poppedItems;
        }

        /// <summary> Pops the specified number of items from the back of the queue. </summary>
        /// <remarks> David Venegoni, Jan 02 2014. </remarks>
        /// <exception cref="ArgumentException">
        ///     Thrown when the number of items to pop exceeds the number of items in the priority
        ///     queue.
        /// </exception>
        /// <param name="numberToPop"> Number of items to pop from the back of the queue. </param>
        /// <returns> The items from the back of the queue. </returns>
        public IEnumerable<T> PopBack(Int32 numberToPop)
        {
            if (numberToPop > items.Count)
            {
                throw new ArgumentException(@"The numberToPop exceeds the number 
                                            of elements in the queue", "numberToPop");
            }

            var poppedItems = new List<T>();
            while (poppedItems.Count < numberToPop)
            {
                poppedItems.Add(PopBack());
            }

            return poppedItems;
        }

        /// <summary> Queries if the priority queue is empty. </summary>
        /// <remarks> David Venegoni, Jan 02 2014. </remarks>
        /// <returns> true if the priority queue empty, false if not. </returns>
        public Boolean IsEmpty()
        {
            return items.Count == 0;
        }

        #endregion

        #region Private Methods

        /// <summary> Inserts the given item into the queue. </summary>
        /// <remarks> David Venegoni, Jan 02 2014. </remarks>
        /// <param name="item"> The item to insert into the queue. </param>
        private void Insert(T item)
        {
            if (items.Count == 0)
            {
                items.Add(item);
            }
            else if (OrderingConvention == PriorityConvention.HighestPriorityInFront)
            {
                InsertAscending(item);
            }
            else
            {
                InsertDescending(item);
            }
        }

        /// <summary>
        ///     Inserts the specified item into the priority queue 
        ///     (using the PriorityConvention.HighestPriorityInFront convention).
        /// </summary>
        /// <remarks> David Venegoni, Jan 02 2014. </remarks>
        /// <param name="item"> The item to insert into the queue. </param>
        private void InsertAscending(T item)
        {
            T tail = items[items.Count - 1];
            Int32 comparedToTail = item.CompareTo(tail);

            if (comparedToTail <= 0) // Less or equal to the than the current minimum
            {
                items.Add(item);
            }
            else if (items.Count == 1)
            {
                /* 
                    * Must assume that since there is only one item
                    * in the list and that the function has reached 
                    * this point, that the current item is greater 
                    * than the item in the queue, so needs to be 
                    * inserted in front 
                    */
                items.Insert(0, item);
            }
            else
            {
                FindIndexAndInsertItemAscending(item);
            }
        }

        /// <summary>
        ///     Inserts the specified item into the priority queue 
        ///     (using the PriorityConvention.LowestPriorityInFront convention).
        /// </summary>
        /// <remarks> David Venegoni, Jan 02 2014. </remarks>
        /// <param name="item"> The item to insert into the queue. </param>
        private void InsertDescending(T item)
        {
            T tail = items[items.Count - 1];
            Int32 comparedToTail = item.CompareTo(tail);

            if (comparedToTail >= 0) // Greater than or equal to current maximum
            {
                items.Add(item);
            }
            else if (items.Count == 1) // See InsertAscending for explanation
            {
                items.Insert(0, item);
            }
            else
            {
                FindIndexAndInsertItemDescending(item);
            }
        }

        /// <summary>
        ///     Searches for the index where the given item should be place in the queue and, 
        ///     subsequently, inserts the item at the specified index.
        /// </summary>
        /// <remarks> David Venegoni, Jan 02 2014. </remarks>
        /// <param name="item"> The item to insert into the queue. </param>
        private void FindIndexAndInsertItemAscending(T item)
        {
            Int32 lowerBoundIndex = 0;
            Int32 upperBoundIndex = items.Count - 1;
            Int32 currentMedianIndex = upperBoundIndex / 2; // No need to floor, integers will always round towards 0

            /* 
                * Will determine which side of the median the current item should be placed, 
                * then updating the lower and upper bounds accordingly until the proper index 
                * is found, at which the point the item will be inserted.
                */
            while (true)
            {
                Int32 comparisonResult = item.CompareTo(items[currentMedianIndex]);
                switch (comparisonResult)
                {
                    case 1:
                        upperBoundIndex = currentMedianIndex;
                        break;
                    case -1:
                        lowerBoundIndex = currentMedianIndex;
                        break;
                    default:
                        FindIndexAndInsertItem(item,
                                                currentMedianIndex,
                                                PriorityConvention.HighestPriorityInFront);
                        return;
                }

                if (AreEndConditionsMet(item, lowerBoundIndex, upperBoundIndex, ref currentMedianIndex))
                {
                    break;
                }
            }
        }

        /// <summary>
        ///     Searches for the index where the given item should be place in the queue and, 
        ///     subsequently, inserts the item at the specified index.
        /// </summary>
        /// <remarks> David Venegoni, Jan 02 2014. </remarks>
        /// <param name="item"> The item to insert into the queue. </param>
        private void FindIndexAndInsertItemDescending(T item)
        {
            // See FindIndexAndInsertItemAscending for explanation
            Int32 lowerBoundIndex = 0;
            Int32 upperBoundIndex = items.Count - 1;
            Int32 currentMedianIndex = upperBoundIndex / 2;

            while (true)
            {
                Int32 comparisonResult = item.CompareTo(items[currentMedianIndex]);
                switch (comparisonResult)
                {
                    case 1:
                        lowerBoundIndex = currentMedianIndex;
                        break;
                    case -1:
                        upperBoundIndex = currentMedianIndex;
                        break;
                    default:
                        FindIndexAndInsertItem(item,
                                                currentMedianIndex,
                                                PriorityConvention.LowestPriorityInFront);
                        return;
                }

                if (AreEndConditionsMet(item, lowerBoundIndex, upperBoundIndex, ref currentMedianIndex))
                {
                    break;
                }
            }
        }

        /// <summary>
        ///     Searches for the index where the given item should be place in the queue and,
        ///     subsequently, inserts the item at the specified index.
        /// </summary>
        /// <remarks>
        ///     This method will be called when the specified item equals 
        ///     another item (can be more than one) within the queue.
        ///     David Venegoni, Jan 02 2014.
        /// </remarks>
        /// <param name="item">               The item to insert into the queue. </param>
        /// <param name="currentIndex">       The index in which to start at. </param>
        /// <param name="priorityConvention"> The priority convention to use when finding the index. </param>
        private void FindIndexAndInsertItem(T item, Int32 currentIndex, PriorityConvention priorityConvention)
        {
            Int32 currentPosition = currentIndex;
            Int32 condition = priorityConvention == PriorityConvention.HighestPriorityInFront ? 1 : -1;
            Boolean isLastElement = false;
            while (item.CompareTo(items[currentPosition]) != condition)
            {
                ++currentPosition;
                if (currentPosition < items.Count) // Make sure the index does not go out of range
                {
                    continue;
                }

                isLastElement = true;
                break;
            }

            if (isLastElement)
            {
                items.Add(item);
            }
            else
            {
                items.Insert(currentPosition, item);
            }
        }

        /// <summary>
        ///     Determines if the current lower bound, upper bound, and median index are 
        ///     at the end conditions, if not, the current median index is updated 
        ///     using the lower and upper bound indices.
        /// </summary>
        /// <remarks>
        ///     The end conditions are:  
        ///                             1)  Is the upper bound index 0?    
        ///                             2)  Is the lower bound index the last index in the queue?    
        ///                             3)  Is the new median index (calculated using lower and 
        ///                                 upper bound indices) the same as the current median index?
        ///     David Venegoni, Jan 02 2014.
        /// </remarks>
        /// <param name="item">               The item to insert if the end conditions are met. </param>
        /// <param name="lowerBoundIndex">    Zero-based index of the lower bound. </param>
        /// <param name="upperBoundIndex">    Zero-based index of the upper bound. </param>
        /// <param name="currentMedianIndex"> [in,out] The current median index. </param>
        /// <returns> true if end conditions met, false if not. </returns>
        private Boolean AreEndConditionsMet(T item, Int32 lowerBoundIndex,
                                                Int32 upperBoundIndex, ref Int32 currentMedianIndex)
        {
            if (upperBoundIndex == 0)
            {
                items.Insert(0, item);
                return true;
            }

            if (lowerBoundIndex == items.Count - 1)
            {
                items.Add(item);
                return true;
            }

            /* 
                * If the new median is the same as the old median, 
                * then this item's priority will always be +1 from 
                * the median's priority, not to mention that continuing 
                * to use that median will result in an infinite loop 
                */
            Int32 newMedianIndex = (upperBoundIndex + lowerBoundIndex) / 2;
            if (currentMedianIndex == newMedianIndex)
            {
                items.Insert(currentMedianIndex + 1, item);
                return true;
            }

            currentMedianIndex = newMedianIndex;
            return false;
        }

        #endregion
    }
}