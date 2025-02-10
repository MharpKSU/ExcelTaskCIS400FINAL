//Maddie Harp
//Task.cs
//creates task
namespace Ksu.Cis300.TaskScheduler
{

    public class Task
    {
        /// <summary>
        /// Gets a string giving the name of the task
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets an int giving the execution time of the tas
        /// </summary>
        public int ExecutionTime { get;}
        /// <summary>
        /// Gets an int giving the period of the task
        /// </summary>
        public int Period { get; }
        /// <summary>
        /// Gets an int giving the index of the task
        /// </summary>
        public int Index { get;}
        /// <summary>
        ///  Gets a list giving the instances of this task
        /// </summary>
        public List<TaskInstance> Instances { get; set;}

        public Task(string name, int executionTime, int period, int index) {
            Name = name;
            ExecutionTime = executionTime;
            Period = period;
            Index = index;
            Instances = new();
        }

        /// <summary>
        /// clears the Instances property, fills it with all of the task instances contained within superperiod
        /// </summary>
        /// <param name="superPeriod">super period from schedule</param>
        public void CreateInstances(int superPeriod) {
            Instances = new();
            for (int i = 0; i < superPeriod; i += Period) {
                Instances.Add(new TaskInstance(this, i));
            }
        }
    }
}
