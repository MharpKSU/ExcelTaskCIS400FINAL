/* DScheduleIOTests.cs
 * Author: Rod Howell
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Ksu.Cis300.TaskScheduler.Tests
{
    /// <summary>
    /// Unit tests for ScheduleIO.
    /// </summary>
    public class CScheduleIOTests
    {
        /// <summary>
        /// The number of processors in each file Testn.csv.
        /// </summary>
        private static readonly int[] _numberOfProcessors = { 2, 5, 5, 5, 5, 5 };

        /// <summary>
        /// The super-period for each file Testn.csv.
        /// </summary>
        private static readonly int[] _superPeriods = { 6, 200, 200, 198, 192, 200 };

        /// <summary>
        /// The Message property for the exception thrown for each of the files Badn.csv.
        /// </summary>
        private static readonly string[] _messages =
        {
            "The number of processors must be at least 1 and at most 5.",
            "The number of processors must be at least 1 and at most 5.",
            "Line 9 has an execution time larger than its period.",
            "Line 3 has 4 fields.",
            "Line 4 has 2 fields.",
            "The task name in line 6 is too long.",
            "Line 8 has an execution time of 0.",
            "Line 6 has a period of 201.",
            "The task set is infeasible.",
            "The number of tasks is greater than 60.",
            "The super-period is greater than 200.",
            "The super-period is greater than 200.",
            "The number of processors is not given."
        };

        /// <summary>
        /// Tests the values of the out parameters returned by ReadTasks on the file Testn.csv.
        /// </summary>
        /// <param name="n">The number of the file to read.</param>
        [Test, Category("A: ReadTasks Tests"), Timeout(1000)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void TestReadTasksOutParameters(int n)
        {
            ScheduleIO.ReadTasks($"../../../Resources/Test{n}.csv", out int nProc, out int superPeriod);
            Assert.Multiple(() =>
            {
                Assert.That(nProc, Is.EqualTo(_numberOfProcessors[n]),
                    $"The first out parameter for Test{n}.csv should be {_numberOfProcessors[n]}.");
                Assert.That(superPeriod, Is.EqualTo(_superPeriods[n]),
                    $"The second out parameter for Test{n}.csv should be {_superPeriods[n]}.");
            });
        }

        /// <summary>
        /// Tests that reading Test0.csv returns the correct list of Tasks.
        /// </summary>
        [Test, Category("A: ReadTasks Tests"), Timeout(1000)]
        public void TestReadTasksTasks()
        {
            List<Task> tasks = 
                ScheduleIO.ReadTasks("../../../Resources/Test0.csv", out int _, out int _);
            Assert.That(tasks, Has.Count.EqualTo(4));
            Assert.Multiple(() =>
            {
                Assert.That(tasks[0].Name, Is.EqualTo("Task 1"),
                    "Element 0 of the returned list should have a Name of \"Task 1\"");
                Assert.That(tasks[3].Name, Is.EqualTo("Task 4"),
                    "Element 3 of the list returned should have a Name of \"Task 4\"");
                Assert.That(tasks[0].ExecutionTime, Is.EqualTo(1),
                    "Element 0 of the list returned should have an ExecutionTime of 1");
                Assert.That(tasks[2].ExecutionTime, Is.EqualTo(2),
                    "Element 2 of the list returned shold have an ExecutionTime of 2.");
                Assert.That(tasks[1].Period, Is.EqualTo(6),
                    "Element 1 of the list returned should have a Period of 6.");
                Assert.That(tasks[3].Period, Is.EqualTo(3),
                    "Element 3 of the list returned should have a Period of 3.");
                Assert.That(tasks[0].Index, Is.EqualTo(0),
                    "Element 0 of the list returned should have an Index of 0.");
                Assert.That(tasks[3].Index, Is.EqualTo(3),
                    "Element 3 of the list returned should have an Index of 3.");
                Assert.That(tasks[0].Instances, Has.Count.EqualTo(3),
                    "Element 0 of the list returned should have an Instances with 3 elements.");
                Assert.That(tasks[3].Instances, Has.Count.EqualTo(2),
                    "Element 3 of the list returned should have an Instances of 2 elements.");
            });
        }

        /// <summary>
        /// Tests that reading the file Badn.csv throws the proper exception with the correct message.
        /// </summary>
        /// <param name="n">The file number.</param>
        [Test, Category("A: ReadTasks Tests"), Timeout(1000)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(9)]
        [TestCase(10)]
        [TestCase(11)]
        [TestCase(12)]
        public void TestReadTasksExceptions(int n)
        {
            Exception e = Assert.Throws<IOException>(() =>
                ScheduleIO.ReadTasks($"../../../Resources/Bad{n}.csv", out int _, out int _),
                $"Reading the file Bad{n}.csv didn't throw an IOException.");
            Assert.That(e.Message, Is.EqualTo(_messages[n]),
                $"Reading the file Bad{n}.csv threw an IOException with an incorrect Message.");
        }
    }
}
