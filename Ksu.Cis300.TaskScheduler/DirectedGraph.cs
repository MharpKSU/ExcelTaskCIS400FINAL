/* DirectedGraph.cs
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
    /// An implementation of a directed graph using both adjacency lists and a dictionary version
    /// of an adjacency matrix.
    /// </summary>
    /// <typeparam name="TNode">The type of the nodes.</typeparam>
    /// <typeparam name="TEdgeData">The type of the values on the edges.</typeparam>
    public class DirectedGraph<TNode, TEdgeData> where TNode : notnull
    {
        /// <summary>
        /// The adjacency lists for the nodes in the graph.
        /// </summary>
        private Dictionary<TNode, LinkedListCell<TNode>?> _adjacencyLists = new();

        /// <summary>
        /// The data associated with each edge in the graph.
        /// </summary>
        private Dictionary<(TNode, TNode), TEdgeData> _edges = new();

        /// <summary>
        /// Gets an enumerable collection of the nodes.
        /// </summary>
        public IEnumerable<TNode> Nodes => _adjacencyLists.Keys;

        /// <summary>
        /// Gets the number of nodes in the graph.
        /// </summary>
        public int NodeCount => _adjacencyLists.Count;

        /// <summary>
        /// Gets the number of edges in the graph.
        /// </summary>
        public int EdgeCount => _edges.Count;

        /// <summary>
        /// Adds the given node to the graph.
        /// </summary>
        /// <param name="node">The node to add.</param>
        public void AddNode(TNode node)
        {
            _adjacencyLists.Add(node, null);
        }

        /// <summary>
        /// Tries to get the value associated with the given edge.
        /// </summary>
        /// <param name="source">The source node of the edge.</param>
        /// <param name="dest">The destination node of the edge.</param>
        /// <param name="value">The value associated with the edge, or the default value if
        /// the edge is not in the graph.</param>
        /// <returns>Whether the edge is in the graph.</returns>
        public bool TryGetEdge(TNode source, TNode dest, out TEdgeData? value)
        {
            return _edges.TryGetValue((source, dest), out value);
        }

        /// <summary>
        /// Determines whether the given node is in the graph.
        /// </summary>
        /// <param name="node">The node to look for.</param>
        /// <returns>Whether node is in the graph.</returns>
        public bool ContainsNode(TNode node)
        {
            return _adjacencyLists.ContainsKey(node);
        }

        /// <summary>
        /// Determines whether there is an edge from the given source node to the given destination node.
        /// </summary>
        /// <param name="source">The source node.</param>
        /// <param name="dest">The destination node.</param>
        /// <returns>Whether there is an edge from source to dest.</returns>
        public bool ContainsEdge(TNode source, TNode dest)
        {
            return _edges.ContainsKey((source, dest));
        }

        /// <summary>
        /// Adds an edge from the given source node to the given destination node with the
        /// given data value attached. If either node is null, throws an ArgumentNullException.
        /// If the nodes are the same or if the edge already exists, throws an ArgumentException.
        /// </summary>
        /// <param name="source">The source node.</param>
        /// <param name="dest">The destination node.</param>
        /// <param name="value">The data value to associate with the edge.</param>
        public void AddEdge(TNode source, TNode dest, TEdgeData value)
        {
            if (source == null || dest == null)
            {
                throw new ArgumentNullException();
            }
            if (source.Equals(dest))
            {
                throw new ArgumentException();
            }
            _edges.Add((source, dest), value);
            _adjacencyLists.TryGetValue(source, out LinkedListCell<TNode>? list);
            LinkedListCell<TNode> cell = new(dest, list);
            list = cell;
            _adjacencyLists[source] = list;
            if (!ContainsNode(dest))
            {
                AddNode(dest);
            }
        }

        /// <summary>
        /// Gets an enumerable collection of the outgoing edges from the given node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>An enumerable collection of the outgoing edges from node.</returns>
        public IEnumerable<Edge<TNode, TEdgeData>> OutgoingEdges(TNode node)
        {
            LinkedListCell<TNode>? list = _adjacencyLists[node];
            while (list != null)
            {
                yield return new Edge<TNode, TEdgeData>(node, list.Data, _edges[(node, list.Data)]);
                list = list.Next;
            }
        }

        public TEdgeData this[TNode source, TNode destination] {
            get 
            {
                if (TryGetEdge(source, destination, out TEdgeData? edge))
                {
                    return edge!;
                }
                else { 
                    throw new KeyNotFoundException();
                }
            }
            set 
            {
                if (TryGetEdge(source, destination, out TEdgeData? edge))
                {
                    _edges[(source,destination)] = value;
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }
        }

    }
}
