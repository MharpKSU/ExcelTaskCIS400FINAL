/* CScheduleGeneratorTests.cs
 * Author: Rod Howell
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ksu.Cis300.TaskScheduler.Tests
{
    /// <summary>
    /// Unit tests for the ScheduleGenerator class.
    /// </summary>
    public class DScheduleGeneratorTests
    {
        /// <summary>
        /// Checks that the given schedule satisfies Criterion 3, that all SchedulingDecisions
        /// properties are ordered, and that each SchedulingDecision is actually scheduled at 
        /// its Time property.
        /// </summary>
        /// <param name="sched">The schedule.</param>
        /// <param name="tasks">The tasks.</param>
        /// <param name="fn">The file containing the tasks.</param>
        /// <param name="msg">An error message if the requirements aren't satisfied.</param>
        /// <returns>Whether the task requirements are satisfied.</returns>
        private static bool MeetsTaskRequirements(SchedulingDecision?[,] sched, List<Task> tasks,
            string fn, out string msg)
        {
            msg = "";
            foreach (Task t in tasks)
            {
                foreach (TaskInstance i in t.Instances)
                {
                    if (i.SchedulingDecisions.Count != t.ExecutionTime)
                    {
                        msg = $"Task \"{t.Name}\" is not scheduled for {t.ExecutionTime} time units between {i.Available} and {i.Deadline}";
                        return false;
                    }
                    int prev = i.Available - 1;
                    foreach (SchedulingDecision d in i.SchedulingDecisions)
                    {
                        if (d.Time <= prev)
                        {
                            msg = $"The scheduling decisions for task \"{t.Name}\" arriving at {i.Available} are out of order or start before the arrival time.";
                            return false;
                        }
                        if (d.Time >= i.Deadline)
                        {
                            msg = $"Task \"{t.Name}\" arriving at {i.Available} is scheduled after its deadline.";
                            return false;
                        }
                        bool found = false;
                        for (int j = 0; j < sched.GetLength(1); j++)
                        {
                            if (sched[d.Time, j] == d)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            msg = $"The scheduling decisions for task \"{t.Name}\" arriving at {i.Available} don't match the schedule.";
                            return false;
                        }
                        prev = d.Time;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Checks that the given schedule satisfies Criteria 2, 4, and 5, and that each
        /// SchedulingDecision in the schedule has a correct Time property.
        /// </summary>
        /// <param name="sched">The schedule.</param>
        /// <param name="tasks">The tasks.</param>
        /// <param name="fn">The file containing the tasks.</param>
        /// <param name="msg">An error message if Criterion 4 isn't satisfied.</param>
        /// <returns>Whether the schedule satisfies Criterion 4.</returns>
        private static bool MeetsScheduleRequirements(SchedulingDecision?[,] sched, List<Task> tasks,
            string fn, out string msg)
        {
            msg = "";
            HashSet<TaskInstance> prev = new();
            for (int i = 0; i < sched.GetLength(0); i++)
            {
                HashSet<TaskInstance> running = new();
                bool hasNull = false;
                for (int j = 0; j < sched.GetLength(1); j++)
                {
                    SchedulingDecision? d = sched[i, j];
                    if (d == null)
                    {
                        hasNull = true;
                    }
                    else
                    {
                        TaskInstance inst = d.TaskInstance;
                        if (running.Contains(inst))
                        {
                            msg = $"For {fn}, two tasks are scheduled simultaneously at time {i}";
                            return false;
                        }
                        SchedulingDecision? prevDec = i == 0 ? null : sched[i - 1, j];
                        if (prev.Contains(inst) && (prevDec == null || prevDec.TaskInstance != inst))
                        {
                            msg = $"For {fn} the same instance of Task \"{inst.Task.Name}\" is scheduled at times {i - 1} and {i} on different processors.";
                            return false;
                        }
                        running.Add(inst);
                        if (d.Time != i)
                        {
                            msg = $"For {fn} the scheduling decision at time {i} on processor {j} has a time of {d.Time}";
                            return false;
                        }
                    }
                }
                if (hasNull)
                {
                    foreach (Task t in tasks)
                    {
                        TaskInstance inst = t.Instances[i / t.Period];
                        if (!running.Contains(inst) &&
                            inst.SchedulingDecisions[t.ExecutionTime - 1].Time > i)
                        {
                            msg = $"For {fn}, a processor is idle at time {i}, but task {t.Name} hasn't finished.";
                            return false;
                        }
                    }
                }
                prev = running;
            }
            return true;
        }



        /// <summary>
        /// Test that a correct schedule is produced on the file Testn.csv.
        /// </summary>
        /// <param name="n">The file number.</param>
        [Test, Category("A: GetSchedule Tests"), Timeout(5000)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void TestGetSchedule(int n)
        {
            string fn = $"Test{n}.csv";
            List<Task> tasks = ScheduleIO.ReadTasks($"../../../Resources/{fn}",
                out int nProc, out int superPeriod);
            SchedulingDecision?[,] sched = ScheduleGenerator.GetSchedule(tasks, nProc, superPeriod);
            Assert.That(MeetsTaskRequirements(sched, tasks, fn, out string msg) &&
                MeetsScheduleRequirements(sched, tasks, fn, out msg), Is.True, msg);
        }

        /// <summary>
        /// Tests GetBackgroundColors.
        /// </summary>
        [Test, Category("B: GetBackgroundColors Tests"), Timeout(1000)]
        public void TestGetBackgroundColors()
        {
            Color[] colors = ScheduleGenerator.GetBackgroundColors(5);
            Color[] expected = { Color.FromArgb(255, 26, 160, 42), Color.FromArgb(255, 78, 96, 128), 
                Color.FromArgb(255, 130, 32, 214), Color.FromArgb(255, 181, 225, 44), 
                Color.FromArgb(255, 233, 161, 130) };
            Assert.That(colors, Is.EquivalentTo(expected));
        }

        /// <summary>
        /// Tests GetForegroundColor.
        /// </summary>
        [Test, Category("C: GetForegroundColor Tests")]
        public void TestGetForegroundColor()
        {
            Assert.Multiple(() =>
            {
                Assert.That(ScheduleGenerator.GetForegroundColor(Color.FromArgb(255, 100, 180, 102)),
                    Is.EqualTo(Color.White));
                Assert.That(ScheduleGenerator.GetForegroundColor(Color.FromArgb(255, 102, 178, 103)),
                    Is.EqualTo(Color.Black));
            });
        }
    }
}
