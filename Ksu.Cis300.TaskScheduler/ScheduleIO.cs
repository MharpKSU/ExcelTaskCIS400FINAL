//Maddie Harp
//ScheduleIO.cs
//reads in and writes schedule
namespace Ksu.Cis300.TaskScheduler
{
    /// <summary>
    /// handles reading and writing(for savce) for the schedule
    /// </summary>
    public static class ScheduleIO
    {
        /// <summary>
        /// mas length for name
        /// </summary>
        public const int MaxNameLength = 15;
        /// <summary>
        /// max period allowed
        /// </summary>
        private const int _maxPeriod = 200;
        /// <summary>
        /// max processors allowed to be used
        /// </summary>
        private const int _maxProcessors = 5;
        /// <summary>
        /// max tasks for schedule
        /// </summary>
        private const int _maxTasks = 60;
        /// <summary>
        /// minimum amount of processors that can be used
        /// </summary>
        private const int _minProcessors = 1;
        /// <summary>
        /// number of fields for schedule
        /// </summary>
        private const int _numberOfFields = 3;
        /// <summary>
        /// char that separates all the fields in file
        /// </summary>
        private const char _separator = ',';
        /// <summary>
        /// field where name is found
        /// </summary>
        private const int _nameField = 0;
        /// <summary>
        /// field where execution time is found
        /// </summary>
        private const int _executionTimeField = 1;
        /// <summary>
        /// fields where period is found
        /// </summary>
        private const int _periodField = 2;

        /// <summary>
        /// reads in the task schedule from file
        /// </summary>
        /// <param name="pathName">name of file being used</param>
        /// <param name="numProcessors">number of processors being used</param>
        /// <param name="superPeriod">super period for schedule</param>
        /// <returns>list of tasks from given file</returns>
        /// <exception cref="IOException">throws expection if error was found with file</exception>
        public static List<Task> ReadTasks(string pathName, out int numProcessors, out int superPeriod)
        {
            List<Task> tasks = new();
            superPeriod = 1;

            //reading in lines from file
            string[] lines = File.ReadAllLines(pathName);
            //error handling
            if (lines.Length == 0)
            {
                throw new IOException("The number of processors is not given.");
            }
            numProcessors = int.Parse(lines[0]);

            if (numProcessors < _minProcessors || numProcessors > _maxProcessors)
            {
                throw new IOException("The number of processors must be at least 1 and at most 5.");
            }

            for (int i = 1; i < lines.Length; i++)
            {
                string[] fields = lines[i].Split(_separator);
                if (fields.Length != _numberOfFields)
                {
                    throw new IOException("Line " + (i + 1) + " has " + fields.Length + " fields.");
                }

                string name = fields[_nameField];
                if (name.Length > MaxNameLength)
                {
                    throw new IOException("The task name in line " + (i + 1) + " is too long.");
                }

                int executionTime = int.Parse(fields[_executionTimeField]);
                if (executionTime < 1)
                {
                    throw new IOException("Line " + (i + 1) + " has an execution time of " + executionTime + ".");
                }
                int period = int.Parse(fields[_periodField]);
                if (executionTime > period)
                {
                    throw new IOException("Line " + (i + 1) + " has an execution time larger than its period.");
                }
                if (period > _maxPeriod)
                {
                    throw new IOException("Line " + (i + 1) + " has a period of " + period + ".");
                }

                superPeriod = SuperPeriod(superPeriod, period);

                tasks.Add(new Task(name, executionTime, period, tasks.Count));

            }

            if (!ValidateProcessor(tasks, numProcessors, superPeriod))
            {
                throw new IOException("The task set is infeasible.");
            }

            if (tasks.Count > 60)
            {
                throw new IOException("The number of tasks is greater than 60.");
            }
            //if no error was found with file, get the tasks from file
            foreach (Task task in tasks)
            {
                task.CreateInstances(superPeriod);
            }

            return tasks;
        }

        /// <summary>
        /// calculates the super period for schedule
        /// </summary>
        /// <param name="current">current super period</param>
        /// <param name="period">period</param>
        /// <returns>returns least common mulitple(the super period)</returns>
        /// <exception cref="IOException"></exception>
        private static int SuperPeriod(int current, int period)
        {
            int greatest = GCD(current, period);
            int least = (current / greatest) * period;

            if (least > _maxPeriod)
            {
                throw new IOException("The super-period is greater than 200.");
            }

            return least;
        }

        /// <summary>
        /// get the great common denomitor, used to calc super period
        /// </summary>
        /// <param name="current">current super period</param>
        /// <param name="period">period</param>
        /// <returns>finds greatest common denomitar</returns>
        private static int GCD(int current, int period)
        {
            while (period != 0)
            {
                int temp = period;
                period = current % period;
                current = temp;
            }

            return current;
        }

        /// <summary>
        /// makes sure file is valid
        /// </summary>
        /// <param name="tasks">the list of task from file</param>
        /// <param name="numProcessors">number of processors being used</param>
        /// <param name="superPeriod">super period from schedule</param>
        /// <returns>if processor valid</returns>
        private static bool ValidateProcessor(List<Task> tasks, int numProcessors, int superPeriod)
        {
            int total = 0;
            foreach (Task task in tasks)
            {
                total += (superPeriod / task.Period) * task.ExecutionTime;
            }

            return total <= (numProcessors * superPeriod);
        }

        /// <summary>
        /// write shcedule into a file
        /// </summary>
        /// <param name="schedule">schedule given</param>
        /// <param name="pathName">name of save file</param>
        public static void WriteSchedule(SchedulingDecision?[,] schedule, string pathName)
        {
            int superPeriod = schedule.GetLength(0);
            int numProcessors = schedule.GetLength(1);

            using (StreamWriter writer = new StreamWriter(pathName))
            {
                writer.Write("Time");
                for (int j = 1; j <= numProcessors; j++)
                {
                    writer.Write($",Processor {j}");
                }
                writer.WriteLine();


                for (int i = 0; i < superPeriod; i++)
                {
                    writer.Write(i);
                    for (int j = 0; j < numProcessors; j++)
                    {
                        writer.Write(",");
                        var decision = schedule[i, j];
                        if (decision != null)
                        {
                            writer.Write(decision.TaskInstance.Task.Name);
                        }
                    }
                    writer.WriteLine();
                }
            }
        }


    }
}
