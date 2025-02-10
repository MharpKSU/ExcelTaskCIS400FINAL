/* BTaskTests.cs
 * Author: Rod Howell
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ksu.Cis300.TaskScheduler.Tests
{
    /// <summary>
    /// Unit tests for the Task, TaskInstance, and SchedulingDecision classes.
    /// </summary>
    public class BTaskTests
    {
        /// <summary>
        /// Tests that Task.Name is defined as a property with no set accessor.
        /// </summary>
        [Test, Category("A: Task Property Definitions"), Timeout(1000)]
        public void TestTaskNameDefinition()
        {
            Type type = typeof(Task);
            PropertyInfo? info = type.GetProperty("Name");
            Assert.That(info, Is.Not.Null, "Name is not defined as a property.");
            Assert.That(info.CanWrite, Is.False, "Name must not contain a set accessor");
        }

        /// <summary>
        /// Tests that Task.ExecutionTime is defined as a property with no set accessor.
        /// </summary>
        [Test, Category("A: Task Property Definitions"), Timeout(1000)]
        public void TestTaskExecutionTimeDefinition()
        {
            Type type = typeof(Task);
            PropertyInfo? info = type.GetProperty("ExecutionTime");
            Assert.That(info, Is.Not.Null, "ExecutionTime is not defined as a property.");
            Assert.That(info.CanWrite, Is.False, "ExecutionTime must not contain a set accessor");
        }

        /// <summary>
        /// Tests that Task.Period is defined as a property with no set accessor.
        /// </summary>
        [Test, Category("A: Task Property Definitions"), Timeout(1000)]
        public void TestTaskPeriodDefinition()
        {
            Type type = typeof(Task);
            PropertyInfo? info = type.GetProperty("Period");
            Assert.That(info, Is.Not.Null, "Period is not defined as a property.");
            Assert.That(info.CanWrite, Is.False, "Period must not contain a set accessor");
        }

        /// <summary>
        /// Tests that Task.Index is defined as a property with no set accessor.
        /// </summary>
        [Test, Category("A: Task Property Definitions"), Timeout(1000)]
        public void TestTaskIndexDefinition()
        {
            Type type = typeof(Task);
            PropertyInfo? info = type.GetProperty("Index");
            Assert.That(info, Is.Not.Null, "Index is not defined as a property.");
            Assert.That(info.CanWrite, Is.False, "Index must not contain a set accessor");
        }

        /// <summary>
        /// Test the Task constructor and other initialization code.
        /// </summary>
        [Test, Category("B: Task Initialization"), Timeout(1000)]
        public void TestConstructTask()
        {
            Task t = new("Task", 4, 8, 2);
            Assert.Multiple(() =>
            {
                Assert.That(t.Name, Is.EqualTo("Task"),
                    "The Task constructor doesn't correctly initialize the Name property.");
                Assert.That(t.ExecutionTime, Is.EqualTo(4),
                    "The Task constructor doesn't correctly initialize the ExecutionTime property.");
                Assert.That(t.Period, Is.EqualTo(8),
                    "The Task constructor doesn't correctly initialize the Period property.");
                Assert.That(t.Index, Is.EqualTo(2),
                    "The Task constructor doesn't correctly initialize the Index property.");
                Assert.That(t.Instances, Is.Not.Null.And.EquivalentTo(Array.Empty<Task>()),
                    "Task's Instances property isn't initialized correctly.");
            });
        }

        /// <summary>
        /// Tests that TaskInstance.Task is defined as a property with no set accessor.
        /// </summary>
        [Test, Category("C: TaskInstance Property Definitions"), Timeout(1000)]
        public void TestTaskInstanceTaskDefinition()
        {
            Type type = typeof(TaskInstance);
            PropertyInfo? info = type.GetProperty("Task");
            Assert.That(info, Is.Not.Null, "Task is not defined as a property.");
            Assert.That(info.CanWrite, Is.False, "Task must not contain a set accessor");
        }

        /// <summary>
        /// Tests that TaskInstance.Available is defined as a property with no set accessor.
        /// </summary>
        [Test, Category("C: TaskInstance Property Definitions"), Timeout(1000)]
        public void TestTaskInstanceAvailableDefinition()
        {
            Type type = typeof(TaskInstance);
            PropertyInfo? info = type.GetProperty("Available");
            Assert.That(info, Is.Not.Null, "Available is not defined as a property.");
            Assert.That(info.CanWrite, Is.False, "Available must not contain a set accessor");
        }

        /// <summary>
        /// Tests that TaskInstance.Deadline is defined as a property with no set accessor.
        /// </summary>
        [Test, Category("C: TaskInstance Property Definitions"), Timeout(1000)]
        public void TestTaskInstanceDeadlineDefinition()
        {
            Type type = typeof(TaskInstance);
            PropertyInfo? info = type.GetProperty("Deadline");
            Assert.That(info, Is.Not.Null, "Deadline is not defined as a property.");
            Assert.That(info.CanWrite, Is.False, "Deadline must not contain a set accessor");
        }

        /// <summary>
        /// Tests that TaskInstance.SchedulingDecisions is defined as a property with no set accessor.
        /// </summary>
        [Test, Category("C: TaskInstance Property Definitions"), Timeout(1000)]
        public void TestTaskInstanceSchedulingDecisionsDefinition()
        {
            Type type = typeof(TaskInstance);
            PropertyInfo? info = type.GetProperty("SchedulingDecisions");
            Assert.That(info, Is.Not.Null, "SchedulingDecisions is not defined as a property.");
            Assert.That(info.CanWrite, Is.False, "SchedulingDecisions must not contain a set accessor");
        }

        /// <summary>
        /// Tests the TaskInstance constructor and other initialization code.
        /// </summary>
        [Test, Category("D: TaskInstance Initialization"), Timeout(1000)]
        public void TestConstructTaskInstance()
        {
            Task t = new("Task", 4, 8, 2);
            TaskInstance i = new(t, 16);
            Assert.Multiple(() =>
            {
                Assert.That(t, Is.EqualTo(i.Task),
                    "The TaskInstance constructor doesn't correctly initialize the Task property.");
                Assert.That(i.Available, Is.EqualTo(16),
                    "The TaskInstance constructor doesn't correctly initialize the Available property.");
                Assert.That(i.Deadline, Is.EqualTo(24),
                    "The TaskInstance.Deadline property isn't computed correctly.");
                Assert.That(i.SchedulingDecisions, Is.Not.Null.And.EquivalentTo(Array.Empty<SchedulingDecision>()),
                    "TaskInstance's SchedulingDecisions property isn't initialized correctly.");
            });
        }

        /// <summary>
        /// Tests the CreateInstances properties of Task.
        /// </summary>
        [Test, Category("E: Task.CreateInstances"), Timeout(1000)]
        public void TestTaskCreateInstances()
        {
            Task t = new("Task", 4, 8, 3);
            t.CreateInstances(32);
            List<TaskInstance> list = t.Instances;
            List<Task> tasks = new();
            List<int> avail = new();
            foreach (TaskInstance i in list)
            {
                tasks.Add(i.Task);
                avail.Add(i.Available);
            }
            Assert.Multiple(() =>
            {
                Assert.That(list, Has.Count.EqualTo(4),
                    "CreateInstances creates a list of the wrong size.");
                Assert.That(tasks, Is.EquivalentTo(new Task[] { t, t, t, t }),
                    "CreateIntances doesn't correctly set the Task properties.");
                Assert.That(avail, Is.Ordered.And.EquivalentTo(new int[] { 0, 8, 16, 24 }),
                    "CreateInstances doesn't correctly set the Available properties.");
            });
        }

        /// <summary>
        /// Tests that SchedulingDecision.TaskInstance is defined as a property with no set accessor.
        /// </summary>
        [Test, Category("F: SchedulingDecision Property Definitions"), Timeout(1000)]
        public void TestSchedulingDecsionTaskInstanceDefinition()
        {
            Type type = typeof(SchedulingDecision);
            PropertyInfo? info = type.GetProperty("TaskInstance");
            Assert.That(info, Is.Not.Null, "TaskInstance is not defined as a property.");
            Assert.That(info.CanWrite, Is.False, "TaskInstance must not contain a set accessor");
        }

        /// <summary>
        /// Tests that SchedulingDecision.Time is defined as a property with a set accessor.
        /// </summary>
        [Test, Category("F: SchedulingDecision Property Definitions"), Timeout(1000)]
        public void TestSchedulingDecsionTimeDefinition()
        {
            Type type = typeof(SchedulingDecision);
            PropertyInfo? info = type.GetProperty("Time");
            Assert.That(info, Is.Not.Null, "Time is not defined as a property.");
            Assert.That(info.CanWrite, Is.True, "Time must contain a set accessor");
        }

        /// <summary>
        /// Tests that SchedulingDecision.ExecutionTimeCompleted is defined as a property with no set 
        /// accessor.
        /// </summary>
        [Test, Category("F: SchedulingDecision Property Definitions"), Timeout(1000)]
        public void TestSchedulingDecsionExecutionTimeCompletedDefinition()
        {
            Type type = typeof(SchedulingDecision);
            PropertyInfo? info = type.GetProperty("ExecutionTimeCompleted");
            Assert.That(info, Is.Not.Null, "ExecutionTimeCompleted is not defined as a property.");
            Assert.That(info.CanWrite, Is.False,
                "ExecutionTimeCompleted must not contain a set accessor");
        }

        /// <summary>
        /// Tests that SchedulingDecision.NextDecision is defined as a property with no set 
        /// accessor.
        /// </summary>
        [Test, Category("F: SchedulingDecision Property Definitions"), Timeout(1000)]
        public void TestSchedulingDecsionNextDecisionDefinition()
        {
            Type type = typeof(SchedulingDecision);
            PropertyInfo? info = type.GetProperty("NextDecision");
            Assert.That(info, Is.Not.Null, "NextDecision is not defined as a property.");
            Assert.That(info.CanWrite, Is.False,
                "NextDecision must not contain a set accessor");
        }

        /// <summary>
        /// Tests the SchedulingDecision constructor.
        /// </summary>
        [Test, Category("G: SchedulingDecision Initialization"), Timeout(1000)]
        public void TestConstructSchedulingDecision()
        {
            TaskInstance t = new(new Task("Task", 4, 8, 0), 16);
            SchedulingDecision d = new(t, 18, 1);
            Assert.Multiple(() =>
            {
                Assert.That(t, Is.EqualTo(d.TaskInstance),
                    "The SchedulingDecision constructor doesn't correctly initialize the TaskInstance property.");
                Assert.That(d.Time, Is.EqualTo(18),
                    "The SchedulingDecision constructor doesn't correctly initialize the Time property.");
                Assert.That(d.ExecutionTimeCompleted, Is.EqualTo(1),
                    "The SchedulingDecision's constructor doesn't correctly initialize the ExecutionTimeCompleted property.");
            });
        }

        /// <summary>
        /// Tests the SchedulingDecision.NextDecision property.
        /// </summary>
        [Test, Category("H: SchedulingDecision.NextDecision")]
        public void TestSchedulingDecisionNextDecision()
        {
            Task t = new("Task", 3, 8, 4);
            TaskInstance inst = new(t, 16);
            SchedulingDecision d1 = new(inst, 17, 0);
            inst.SchedulingDecisions.Add(d1);
            SchedulingDecision d2 = new(inst, 20, 1);
            inst.SchedulingDecisions.Add(d2);
            SchedulingDecision d3 = new(inst, 21, 2);
            inst.SchedulingDecisions.Add(d3);
            Assert.Multiple(() =>
            {
                Assert.That(d1.NextDecision, Is.EqualTo(d2),
                    "d1.NextDecision should be d2");
                Assert.That(d2.NextDecision, Is.EqualTo(d3),
                    "d2.NextDecision should be d3");
                Assert.That(d3.NextDecision, Is.Null,
                    "d3.NextDecision should be null.");
            });
        }
    }
}
