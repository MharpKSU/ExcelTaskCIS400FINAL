/* LinkedListCell.cs
 * Author: Rod Howell
 */
namespace Ksu.Cis300.TaskScheduler
{
    /// <summary>
    /// A single cell of a generic linked list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list</typeparam>
    public class LinkedListCell<T>
    {
        /// <summary>
        /// Gets or sets the data in the cell.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Gets or sets the next cell in the list.
        /// </summary>
        public LinkedListCell<T>? Next { get; set; }

        /// <summary>
        /// Constructs a new cell with the given data and next cell.
        /// </summary>
        /// <param name="data">The data in the cell.</param>
        /// <param name="next">The next cell in the list.</param>
        public LinkedListCell(T data, LinkedListCell<T>? next)
        {
            Data = data;
            Next = next;
        }   
    }
}