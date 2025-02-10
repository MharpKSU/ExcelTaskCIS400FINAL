//Maddie Harp
//UserInterface.cs
//creates user interface/gui and shows schedule
namespace Ksu.Cis300.TaskScheduler
{
    /// <summary>
    /// creates GUI for the user
    /// </summary>
    public partial class UserInterface : Form
    {
        /// <summary>
        /// size of font
        /// </summary>
        private const int _fontSize = 18;
        /// <summary>
        /// font being used
        /// </summary>
        private readonly Font _font = new Font(FontFamily.GenericSansSerif, _fontSize, GraphicsUnit.Pixel);
        /// <summary>
        /// width of the time column for list view
        /// </summary>
        private const int _timeColumnWidth = _fontSize * 4;
        /// <summary>
        /// width of the processor columns for list view
        /// </summary>
        private const int _processorColumnWidth = _fontSize * ScheduleIO.MaxNameLength;
        /// <summary>
        /// the schedule being shown
        /// </summary>
        private SchedulingDecision[,] _schedule = new SchedulingDecision[0, 0];

        public UserInterface()
        {
            InitializeComponent();
            uxListView.Font = _font;
            uxListView.View = View.Details;
            uxListView.GridLines = true;
            Save.Enabled = false;
        }

        /// <summary>
        /// handler of list view
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void UXListView(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// handles open button from file menu
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void UXOpen(object sender, EventArgs e)
        {
            //creating open file dialog to get file
            OpenFileDialog openFile = new OpenFileDialog();
            //sets window filter to show only csv files
            openFile.Filter = "CSV Files|*.csv|All Files|*.*";

            //reading in schedule from file
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    int numProcessors;
                    int superPeriod;
                    List<Task> tasks = ScheduleIO.ReadTasks(openFile.FileName, out numProcessors, out superPeriod);

                    _schedule = ScheduleGenerator.GetSchedule(tasks, numProcessors, superPeriod)!;

                    CreateSchedule(tasks, numProcessors);
                    Save.Enabled = true;
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// creates the list view update, sets up schedule
        /// </summary>
        /// <param name="tasks">list of tasks for shcedule</param>
        /// <param name="numProcessors">how many processors being used in schedule</param>
        private void CreateSchedule(List<Task> tasks, int numProcessors)
        {
            uxListView.BeginUpdate();
            //clearing list view for new update
            uxListView.Columns.Clear();
            uxListView.Items.Clear();

            //adding headerrrrrrs
            uxListView.Columns.Add("Time", _timeColumnWidth);
            for (int i = 0; i < numProcessors; i++)
            {
                uxListView.Columns.Add($"Processor {i + 1}", _processorColumnWidth, HorizontalAlignment.Center);
            }

            //settinmg background colors for the tasks
            Color[] backgroundColors = ScheduleGenerator.GetBackgroundColors(tasks.Count);
            //creating the rows for the list view
            FillListRows(backgroundColors, tasks);

            //updating the list view
            uxListView.EndUpdate();
        }

        /// <summary>
        /// fills the rows of tasks in list view
        /// </summary>
        /// <param name="bgColors">bacjkground colors for the tasks</param>
        /// <param name="tasks">tasks in the schedule</param>
        private void FillListRows(Color[] bgColors, List<Task> tasks)
        {
            for (int t = 0; t < _schedule.GetLength(0); t++)
            {
                //row for list view
                ListViewItem row = new ListViewItem(t.ToString());
                row.UseItemStyleForSubItems = false;

                //sets row with tasks
                for (int i = 0; i < _schedule.GetLength(1); i++)
                {
                    SchedulingDecision decision = _schedule[t, i];
                    //To fill in the elements of the processor columns for this row,
                    //construct a new ListViewItem.ListViewSubItem for each processor. 
                    var subItem = new ListViewItem.ListViewSubItem();


                    if (decision != null)
                    {
                        string taskName = string.Empty;
                        if (t == 0)
                        {
                            taskName = decision.TaskInstance.Task.Name;
                        }
                        else
                        {
                            SchedulingDecision? previousDecision = _schedule[t - 1, i];
                            if (previousDecision == null || previousDecision.TaskInstance != decision.TaskInstance)
                            {
                                taskName = decision.TaskInstance.Task.Name;
                            }
                        }
                        subItem.Text = taskName;
                        //setting color of certain taak
                        subItem.BackColor = bgColors[decision.TaskInstance.Task.Index];
                        subItem.ForeColor = ScheduleGenerator.GetForegroundColor(subItem.BackColor);
                    }
                    row.SubItems.Add(subItem);
                }
                //adds row to list view
                uxListView.Items.Add(row);
            }
        }


        /// <summary>
        /// handles Save, saves the file
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void UXSave(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            //sets filter to only csv files
            saveFile.Filter = "CSV Files|*.csv|All Files|*.*";

            //saves file
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ScheduleIO.WriteSchedule(_schedule, saveFile.FileName);
                    MessageBox.Show("Schedule saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// userinterface form
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void UserInterface_Load(object sender, EventArgs e)
        {

        }
    }
}

