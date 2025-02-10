//Maddie Harp
//SchedulingDecision.cs
//creates scheduling decisions
namespace Ksu.Cis300.TaskScheduler
{
    /// <summary>
    /// creates scheduling decision
    /// </summary>
    public class SchedulingDecision
    {
        /// <summary>
        /// Gets a TaskInstance giving the instance for which this decision was made
        /// </summary>
        public TaskInstance TaskInstance { get; }
        /// <summary>
        /// Gets or sets an int giving the time at which the task instance is scheduled
        /// </summary>
        public int Time { get; set; }
        /// <summary>
        /// Gets the execution time completed by the task instance prior to this scheduling decision
        /// </summary>
        public int ExecutionTimeCompleted { get; }
        /// <summary>
        /// Gets the next SchedulingDecision? for the TaskInstance
        /// </summary>
        public SchedulingDecision? NextDecision { 
            get {
                if (ExecutionTimeCompleted + 1 < TaskInstance.SchedulingDecisions.Count)
                {
                    return TaskInstance.SchedulingDecisions[ExecutionTimeCompleted + 1];
                }
                else {
                    return null;
                }
            } 
        }

        public SchedulingDecision(TaskInstance i, int t, int e) { 
            TaskInstance = i;
            Time = t;
            ExecutionTimeCompleted = e; 
        }
    }
}
