/* AGraphTests.cs
 * Author: Rod Howell
 */
namespace Ksu.Cis300.TaskScheduler.Tests
{
    /// <summary>
    /// Unit tests for the Graph class.
    /// </summary>
    public class AGraphTests
    {
        /// <summary>
        /// Tests that the indexer throws the proper exceptions when the edge isn't in the graph.
        /// </summary>
        [Test, Category("A: Graph Indexer"), Timeout(1000)]
        public void TestIndexerExceptions()
        {
            DirectedGraph<string, int> g = new();
            Assert.Multiple(() =>
            {
                Assert.Throws<KeyNotFoundException>(() =>
                {
                    int i = g["a", "b"];
                },
                "The indexer's get accessor doesn't throw a KeyNotFoundException when the edge isn't in the graph.");
                Assert.Throws<KeyNotFoundException>(() =>
                {
                    g["a", "b"] = 1;
                },
                "The indexer's set accessor doesn't throw a KeyNotFoundException when the edge isn't in the graph");
            });
        }

        /// <summary>
        /// Tests that the indexer works correctly.
        /// </summary>
        [Test, Category("A: Graph Indexer"), Timeout(1000)]
        public void TestIndexer()
        {
            DirectedGraph<string, int> g = new();
            g.AddEdge("a", "b", 2);
            g["a", "b"] = 10;
            Assert.That(g["a", "b"], Is.EqualTo(10));
        }
    }
}