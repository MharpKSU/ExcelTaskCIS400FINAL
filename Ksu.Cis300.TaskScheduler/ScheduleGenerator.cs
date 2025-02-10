//Maddie Harp
//ScheduleGenerator.cs
//generators schedule using given file
namespace Ksu.Cis300.TaskScheduler
{
    /// <summary>
    /// generators the schdule with given file
    /// </summary>
    public static class ScheduleGenerator
    {
        /// <summary>
        /// rgb value for the first background color
        /// </summary>
        private const int _firstColor = 0x1aa02a;
        /// <summary>
        /// rgb value for the increment
        /// </summary>
        private const int _colorIncrement = 0x33c056;
        /// <summary>
        /// the mask for converting an rgb value to an argb value
        /// </summary>
        private const uint _argbMask = 0xff000000;
        /// <summary>
        /// minimum sum of red, blue, and green components for which a black foreground should be used
        /// </summary>
        private const int _blackForegroundThreshold = 383;

        /// <summary>
        /// gets/creates schedule
        /// </summary>
        /// <param name="givenTasks">list of tasks given by file</param>
        /// <param name="processors">num of processors being used</param>
        /// <param name="superPeriod">superPeriod of schedule/tasks</param>
        /// <returns>the created final schedule for given file</returns>
        public static SchedulingDecision?[,] GetSchedule(List<Task> givenTasks, int processors, int superPeriod)
        {
            DirectedGraph<object, int> flowNetwork = CreateFlowNetwork(givenTasks, processors, superPeriod);

            MaxFlow(flowNetwork, "source", "sink");

            HashSet<SchedulingDecision>[] rawSchedule = RawSchedule(flowNetwork, superPeriod, "sink");

            List<TaskInstance>[] arrivals = GetArrivals(givenTasks, superPeriod);

            RearrangeIdles(rawSchedule, processors, arrivals);

            return FinalSchedule(rawSchedule, processors, superPeriod);

        }
        /// <summary>
        /// finds colors for tasks to display
        /// </summary>
        /// <param name="num">number of colors needed</param>
        /// <returns>Color[ ] of this size containing the background colors to use for the tasks</returns>
        public static Color[] GetBackgroundColors(int num)
        {
            Color[] colors = new Color[num];
            int currentColor = _firstColor;

            for (int i = 0; i < num; i++)
            {
                uint argb = _argbMask | (uint)currentColor;
                colors[i] = Color.FromArgb((int)argb);
                currentColor = (currentColor + _colorIncrement) & 0xFFFFFF;
            }

            return colors;
        }
        /// <summary>
        /// finds what foreground color to use with background
        /// </summary>
        /// <param name="background">color of background</param>
        /// <returns>a Color giving the foreground color to use with the given background color</returns>
        public static Color GetForegroundColor(Color background)
        {
            int r = background.R;
            int g = background.G;
            int b = background.B;
            int sum = r + g + b;
            if (sum < _blackForegroundThreshold)
            {
                return Color.White;
            }
            else
            {
                return Color.Black;
            }
        }

        /// <summary>
        /// creates a flow/resdiual network
        /// </summary>
        /// <param name="givenTasks">given tasks from file</param>
        /// <param name="processors">number of processors being used</param>
        /// <param name="superPeriod">superPeriod of schedule</param>
        /// <returns>returns a flow network from tasks</returns>
        private static DirectedGraph<object, int> CreateFlowNetwork(List<Task> givenTasks, int processors, int superPeriod)
        {
            DirectedGraph<object, int> flow = new();
            //Section 4.1!!!

            //source node
            string source = "source";
            //sink node
            string sink = "sink";
            //adding these nodes
            //  flow.AddNode(source);
            //  flow.AddNode(sink);



            //adding nodes for each task instance
            foreach (Task task in givenTasks)
            {
                foreach (TaskInstance instance in task.Instances)
                {
                    flow.AddEdge(source, instance, task.ExecutionTime);
                    flow.AddEdge(instance, source, 0);
                    //adding edges for each time unit in interval(arriavl to end of period)
                    for (int i = instance.Available; i < instance.Deadline; i++)
                    {
                        flow.AddEdge(instance, i, 1);
                        //opposite direction for max flow
                        flow.AddEdge(i, instance, 0);

                    }
                }
            }

            //node for each time unit in superPeriod & edges
            for (int i = 0; i < superPeriod; i++)
            {
                flow.AddEdge(i, sink, processors);
                flow.AddEdge(sink, i, 0);

            }

            //flow network graph hopefully
            return flow;

        }

        /// <summary>
        /// finds max flow for FlowNetwork
        /// </summary>
        /// <param name="flow">the flow network</param>
        /// <param name="source">source node from flow</param>
        /// <param name="sink">sink node from flow</param>
        private static void MaxFlow(DirectedGraph<object, int> flow, object source, object sink)
        {
            Dictionary<object, object> paths;
            while ((paths = FindAugmentingPath(flow, source, sink)) != null)
            {
                AugmentingPath(flow, paths, sink);
            }

        }

        /// <summary>
        /// used to find all the augmenting paths to help find max flow
        /// </summary>
        /// <param name="flow">flow network graph being used</param>
        /// <param name="source">source node</param>
        /// <param name="sink">sink node</param>
        /// <returns>list of the augmenting paths</returns>
        private static Dictionary<object, object> FindAugmentingPath(DirectedGraph<object, int> flow, object source, object sink)
        {
            /*Using depth-first search, find an augmenting path from the source to the sink, 
             * ignoring all edges with 0 capacity.*/
            Dictionary<object, object> path = new();
            path.Add(source, source);
            Stack<Edge<object, int>> stack = new();
            foreach (Edge<object, int> e in flow.OutgoingEdges(source))
            {
                if (e.Data > 0)
                {
                    stack.Push(e);
                }
            }

            while (stack.Count > 0)
            {
                Edge<object, int> e = stack.Pop();

                //if soruce path leads to sink
                if (!path.ContainsKey(e.Destination))
                {
                    path.Add(e.Destination, e.Source);
                    if (e.Destination == sink)
                    {
                        return path;
                    }
                    foreach (Edge<object, int> outgoing in flow.OutgoingEdges(e.Destination))
                    {
                        if (outgoing.Data > 0)
                        {
                            stack.Push(outgoing);
                        }
                    }
                }

            }

            //if no paths were found
            return null!;
        }

        /// <summary>
        /// updates augmenting path
        /// </summary>
        /// <param name="flow">flow network</param>
        /// <param name="paths">augmenting paths found</param>
        /// <param name="sink">sink node from flow network</param>
        private static void AugmentingPath(DirectedGraph<object, int> flow, Dictionary<object, object> paths, object sink)
        {
            /*For each edge along the augmenting path:
             * Decrease the capacity of this edge by 1.
             * Increase by 1 the capacity of the edge going the opposite direction.*/
            object pred = paths[sink];
            while (pred != sink)
            {
                flow[pred, sink]--;
                flow[sink, pred]++;
                sink = pred;
                pred = paths[sink];
            }
        }



        /// <summary>
        /// from the flow network we create a raw schedule
        /// </summary>
        /// <param name="residual">resiidual network we are using</param>
        /// <param name="superPeriod">super Period of schedule</param>
        /// /// <param name="sink">the sink of the residual network</param>
        /// <returns>the raw schedule from flow network</returns>
        private static HashSet<SchedulingDecision>[] RawSchedule(DirectedGraph<object, int> residual, int superPeriod, object sink)
        {
            /*For each edge with nonzero capacity from a time to a task instance, assign that task instance to that time. 
             * (Note that the outgoing edges from times all go to either the sink or a task instance.)
             * 
             * 
             For a raw schedule, you won't need to assign SchedulingDecisions to processors. 
            It therefore makes sense to use, for each time unit in the super-period, a data structure for which efficient lookups are supported. 
            An appropriate data structure for this purpose is a HashSet<SchedulingDecision>. 
            This data type represents a set of SchedulingDecisions, and is implemented using a hash table (one way of thinking of it is as a dictionary with no values - just keys). 
            Elements can be added using its Add method, and lookups can be done using its Contains method. Elements can be removed using its Remove method. 
            A foreach can be used to iterate through its elements. 
            The entire raw schedule should then be represented with a HashSet<SchedulingDecision>[ ] whose length is the super-period.*/

            HashSet<SchedulingDecision>[] rawSchedule = new HashSet<SchedulingDecision>[superPeriod];

            //initialize 
            for (int i = 0; i < superPeriod; i++)
            {
                rawSchedule[i] = new HashSet<SchedulingDecision>();
                foreach (var outgoing in residual.OutgoingEdges(i))
                {
                    if ((outgoing.Data != 0) && (outgoing.Destination != sink))
                    {
                        TaskInstance instance = (TaskInstance)outgoing.Destination;
                        SchedulingDecision d = new SchedulingDecision(instance, i, instance.SchedulingDecisions.Count);
                        rawSchedule[i].Add(d);
                        instance.SchedulingDecisions.Add(d);

                    }

                }
            }

            return rawSchedule;

        }

        /// <summary>
        /// rearranges idles
        /// </summary>
        /// <param name="rawSchedule">raw schedule we are using</param>
        /// <param name="processors">num of processors being used</param>
        /// <param name="arrivals">arriving tasks</param>
        private static void RearrangeIdles(HashSet<SchedulingDecision>[] rawSchedule, int processors, List<TaskInstance>[] arrivals)
        {
            /** Add the first scheduling decision for each task arriving at this time unit to the next decisions.
             * Remove all scheduling decisions scheduled at this time unit from the next decisions.
             * Get up to k next decisions, where k is the number of processors minus the number of tasks scheduled at this time unit. 
             * If there are fewer than k next decisions, get all of them. For each of these decisions:
             * Reschedule this decision so that it is scheduled at the current time unit.
             * Remove the decision from the next decisions.
             * For each scheduling decision at the current time unit:
             * Add the next scheduling decision (if there is one) for the task instance to the next decisions.
             */
            HashSet<SchedulingDecision> nextDecisions = new();
            for (int i = 0; i < rawSchedule.Length; i++)
            {
                AddArrivals(arrivals[i], nextDecisions);
                RemoveCurrentDecisions(rawSchedule[i], nextDecisions);
                List<SchedulingDecision> toMove = GetDecisionsToMove(nextDecisions, processors - rawSchedule[i].Count);
                MoveInstances(toMove, rawSchedule, nextDecisions, i);
                AddNextDecisions(rawSchedule[i], nextDecisions);
            }

        }

        /// <summary>
        /// Adds arriving tasks to nextDecisions
        /// </summary>
        /// <param name="arrivals">list of arriving tasks</param>
        /// <param name="nextDecisions">list of decisions</param>
        private static void AddArrivals(List<TaskInstance> arrivals, HashSet<SchedulingDecision> nextDecisions)
        {
            foreach (TaskInstance t in arrivals)
            {
                nextDecisions.Add(t.SchedulingDecisions[0]);
            }
        }
        /// <summary>
        /// removes current decisions
        /// </summary>
        /// <param name="schedule">schedule we are creating</param>
        /// <param name="nextDecisions">list of decisions</param>
        private static void RemoveCurrentDecisions(HashSet<SchedulingDecision> schedule, HashSet<SchedulingDecision> nextDecisions)
        {
            foreach (SchedulingDecision decision in schedule)
            {
                nextDecisions.Remove(decision);
            }
        }
        /// <summary>
        /// gets the decision that needs to be moved
        /// </summary>
        /// <param name="nextDecision">next decision</param>
        /// <param name="k">k</param>
        /// <returns>list of the decisions that need to move</returns>
        private static List<SchedulingDecision> GetDecisionsToMove(HashSet<SchedulingDecision> nextDecision, int k)
        {
            List<SchedulingDecision> toMove = new();
            foreach (SchedulingDecision decision in nextDecision)
            {
                if (toMove.Count == k)
                {
                    break;

                }
                toMove.Add(decision);
            }
            return toMove;
        }

        /// <summary>
        /// moving the decisions 
        /// </summary>
        /// <param name="toMove">list of decisions needing to be moved</param>
        /// <param name="rawSchedule">the raw schedule created</param>
        /// <param name="nextDecision">next decision in list</param>
        /// <param name="currentTime">current time</param>
        private static void MoveInstances(List<SchedulingDecision> toMove, HashSet<SchedulingDecision>[] rawSchedule, HashSet<SchedulingDecision> nextDecision, int currentTime)
        {
            foreach (var decision in toMove)
            {
                rawSchedule[decision.Time].Remove(decision);
                decision.Time = currentTime;
                rawSchedule[currentTime].Add(decision);
                nextDecision.Remove(decision);
            }

        }

        /// <summary>
        /// adding next decisions
        /// </summary>
        /// <param name="schedule">schedule</param>
        /// <param name="nextDecision">next decision</param>
        private static void AddNextDecisions(HashSet<SchedulingDecision> schedule, HashSet<SchedulingDecision> nextDecision)
        {
            foreach (var decision in schedule)
            {
                if (decision.NextDecision != null)
                {
                    nextDecision.Add(decision.NextDecision);
                }
            }
        }

        /// <summary>
        /// gets a list of the arriving tasks
        /// </summary>
        /// <param name="task">task list</param>
        /// <param name="len">super period</param>
        /// <returns>list of arriaing task intances</returns>
        private static List<TaskInstance>[] GetArrivals(List<Task> task, int len)
        {
            List<TaskInstance>[] arrivals = new List<TaskInstance>[len];

            for (int i = 0; i < len; i++)
            {
                arrivals[i] = new();
            }
            foreach (Task t in task)
            {
                foreach (TaskInstance instance in t.Instances)
                {
                    arrivals[instance.Available].Add(instance);
                }
            }

            return arrivals;
        }


        /// <summary>
        /// creates the final schedule
        /// </summary>
        /// <param name="rawSchedule">raw schedule</param>
        /// <param name="numProcessors"># of processors being used</param>
        /// <param name="superPeriod">super period of schedule</param>
        /// <returns>returns the final schedule</returns>
        private static SchedulingDecision?[,] FinalSchedule(HashSet<SchedulingDecision>[] rawSchedule, int numProcessors, int superPeriod)
        {
            SchedulingDecision?[,] finalSchedule = new SchedulingDecision?[rawSchedule.Length, numProcessors];
            int j = 0;
            foreach (SchedulingDecision d in rawSchedule[0])
            {
                finalSchedule[0, j] = d;
                j++;
            }
            for (int i = 1; i < rawSchedule.Length; i++)
            {
                ScheduleContinuingTasks(rawSchedule[i], finalSchedule!, i, numProcessors);
                SchedulingNewTasks(rawSchedule[i], finalSchedule!, i);
            }
            return finalSchedule;
        }

        /// <summary>
        /// schedules a task if a task continues
        /// </summary>
        /// <param name="toSched">decisions</param>
        /// <param name="schedule">schedule being used</param>
        /// <param name="time">time</param>
        /// <param name="processors">number of processors</param>
        private static void ScheduleContinuingTasks(HashSet<SchedulingDecision> toSched, SchedulingDecision[,] schedule, int time, int processors)
        {
            for (int i = 0; i < processors; i++)
            {
                SchedulingDecision decisions = schedule[time - 1, i];
                if (decisions != null)
                {
                    SchedulingDecision next = decisions.NextDecision!;
                    if (next != null && toSched.Contains(next))
                    {
                        schedule[time, i] = next;
                        toSched.Remove(next);
                    }
                }
            }
        }

        /// <summary>
        /// scheduling the arrving task
        /// </summary>
        /// <param name="toSchedule">scheduling decisions arriving</param>
        /// <param name="schedule">schedule beikng used</param>
        /// <param name="time">time</param>
        private static void SchedulingNewTasks(HashSet<SchedulingDecision> toSchedule, SchedulingDecision[,] schedule, int time)
        {
            int i = 0;
            foreach (SchedulingDecision d in toSchedule)
            {
                while (schedule[time, i] != null)
                {
                    i++;
                }
                schedule[time, i] = d;
            }

        }
    }
}
