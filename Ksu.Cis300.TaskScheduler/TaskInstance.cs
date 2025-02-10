//Maddie Harp
//TaskInstance.cs
//creates task instances
namespace Ksu.Cis300.TaskScheduler
{
    /// <summary>
    /// creates task instances
    /// </summary>
    public class TaskInstance
    {
        /// <summary>
        /// Gets the Task for which this TaskInstance is an instance
        /// </summary>
        public Task Task { get;}
        /// <summary>
        /// Gets an int giving the time at which this instance becomes available
        /// </summary>
        public int Available { get; }
        /// <summary>
        /// Gets an int giving the deadline of this instance
        /// </summary>
        public int Deadline { get => Available + Task.Period; }
        /// <summary>
        ///  Gets a List giving the decisions for each unit of execution time for which this task has been scheduled
        /// </summary>
        public List<SchedulingDecision> SchedulingDecisions { get;}

        /// <summary>
        /// construtor
        /// </summary>
        /// <param name="t">task</param>
        /// <param name="available">time where instacne becomes available</param>
        public TaskInstance(Task t, int available) {
            SchedulingDecisions = new();
            Task = t;
            Available = available;
        }

    }
}
