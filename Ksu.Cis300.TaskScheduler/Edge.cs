/* Edge.cs
 * Author: Rod Howell
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ksu.Cis300.TaskScheduler
{
    /// <summary>
    /// A single edge of a directed graph.
    /// <typeparam name="TNode">The type of the nodes.</typeparam>
    /// <typeparam name="TEdgeData">The type of the data associated with an edge.</typeparam>
    /// </summary>
    public readonly struct Edge<TNode, TEdgeData>
    {
        /// <summary>
        /// Gets the source node.
        /// </summary>
        public TNode Source { get; }

        /// <summary>
        /// Gets the destination node.
        /// </summary>
        public TNode Destination { get; }

        /// <summary>
        /// Gets the data associated with the edge.
        /// </summary>
        public TEdgeData Data { get; }

        /// <summary>
        /// Constructs an edge from the given source node to the given destination node having
        /// the given data.
        /// </summary>
        /// <param name="source">The source node.</param>
        /// <param name="dest">The destination node.</param>
        /// <param name="data">The data associated with the edge.</param>
        public Edge(TNode source, TNode dest, TEdgeData data)
        {
            Source = source;
            Destination = dest;
            Data = data;
        }
    }
}
