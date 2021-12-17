using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace AT3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected ProtectedFileObject fileObject;
        protected Queue queue;

        protected DataTable taskTable;
        protected int test = 0;
        protected List<TimeSpan> cbxTime_List;

        protected SaveFileDialog saveFD;
        protected OpenFileDialog openFD;
        protected BinReadWrite.BinReadWrite binRW;

        protected int queueIndex;
        protected bool canEdit;

        protected const string DATETIME_FORMAT = "dd/MM/yyyy HH:mm";

        public MainWindow()
        {
            InitializeComponent();
            initialise();
        }
        private void initialise()
        {
            // initalise objects to null
            queue = new Queue();
            taskTable = new DataTable();

            // PLACEHOLDER TEST VALUES
            queue = new Queue(
                new List<QueueNode>() {
                    new QueueNode(new Task(
                        1,
                        DateTime.Now,
                        "Test",
                        "Lorem Ipsum etc.",
                        TaskPriority.Low)),
                    new QueueNode(new Task(
                        2,
                        DateTime.Now,
                        "Test2",
                        "Lorem Ipsum etc.",
                        TaskPriority.Minor)),
                    new QueueNode(new Task(
                        3,
                        DateTime.Now,
                        "Test3",
                        "Lorem Ipsum etc.",
                        TaskPriority.Moderate)),
                    new QueueNode(new Task(
                        4,
                        DateTime.Now,
                        "Test4",
                        "Lorem Ipsum etc.",
                        TaskPriority.High)),
                    new QueueNode(new Task(
                        5,
                        DateTime.Now,
                        "Test5",
                        "Lorem Ipsum etc.",
                        TaskPriority.Major))
                }
            );
            
            binRW = new BinReadWrite.BinReadWrite();

            saveFD = new SaveFileDialog();
            saveFD.Filter = "dat Files|*.dat";
            openFD = new OpenFileDialog();
            openFD.Filter = "dat Files|*.dat";

            taskTable = initialiseDataTable();

            populateFields();

            initialise_DG_DoubleClick();
            initialise_cbxTime();
        }
        /*****************************************************
         * Initialises data table and data grid for initial 
         * form load
         *****************************************************/
        private DataTable initialiseDataTable()
        {
            // create table
            DataTable dataTable = new DataTable("Task");

            // initalise table columns
            DataColumn colId = new DataColumn("ID", typeof(int));
            dataTable.Columns.Add(colId);
            DataColumn colTitle = new DataColumn("Title", typeof(String));
            dataTable.Columns.Add(colTitle);
            DataColumn colDue = new DataColumn("Date Due", typeof(String));
            dataTable.Columns.Add(colDue);

            dataTable.PrimaryKey = new DataColumn[] { colId };

            // populate table
            InsertTasks(dataTable, queue);
            // add table as DataGrid data source
            dgTasks.DataContext = dataTable;

            return dataTable;
        }
        /****************************************************************
         * Sets taskTable to contain ID, Title, DueDateTime of all 
         * tasks in taskQueue
         ****************************************************************/
        private void InsertTasks(DataTable taskTable, Queue taskQueue)
        {
            taskTable.Clear();
            Object[] row;

            QueueNode temp = taskQueue.Head;
            while (temp != null)
            {
                row = new Object[] { temp.Data.ID, temp.Data.Title,
                                     temp.Data.DueDateTimeString() };
                taskTable.Rows.Add(row);
                temp = temp.NextNode;
            }
        }
        private void populateFields()
        {
            populateFields(0);
        }
        private void populateFields(int i)
        {
            if (queue != null && queue.NodeList.Count > i)
            {
                clearFields();

                // get task at top of queue
                Task task = queue.NodeList[i].Data;

                // populate fields
                txtID.Text = task.ID.ToString();
                datePicker.SelectedDate = null;
                cbxTime.SelectedItem = null;
                txtDueDateTime.Text = task.DueDateTime.ToString(DATETIME_FORMAT);
                txtTitle.Text = task.Title;
                txtDetails.Text = task.Details;

                queueIndex = i;
            }
        }
        private void clearFields()
        {
            txtID.Clear();
            txtDueDateTime.Clear();
            txtTitle.Clear();
            txtDetails.Clear();
        }
        /****************************************************************
         * Defines a DataGridRow style that gives it the specified
         * MouseDoubleClickEvent.
         ****************************************************************/
        private void initialise_DG_DoubleClick()
        {
            //define style
            Style rowStyle = new Style(typeof(DataGridRow));
            rowStyle.Setters.Add(
                new EventSetter(
                    DataGridRow.MouseDoubleClickEvent,
                    new MouseButtonEventHandler(dgTasks_row_PreviewMouseDoubleClick)
                ));
            //apply style to the datagrid rows
            dgTasks.RowStyle = rowStyle;
        }
        /****************************************************************
         * Generates all times to be added to combo box
         ****************************************************************/
        private void initialise_cbxTime()
        {
            cbxTime_List = new List<TimeSpan>();
            // add each time from 12 AM onwards in 15 minute increments
            for (int i = 0; i < 24; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    // hour, minute, second
                    cbxTime_List.Add(new TimeSpan(i, j * 15, 0));
                }
            }

            // add each time from list to combo box
            foreach (TimeSpan time in cbxTime_List)
            {
                cbxTime.Items.Add(time.ToString());
            }
        }
        private void btnLoadFile_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)openFD.ShowDialog())
            {
                binRW.Filepath = openFD.FileName;

                if (binRW.readFile())
                {
                    fileObject = (ProtectedFileObject)binRW.FileData;
                    queue = (Queue)fileObject.FileData;

                    InsertTasks(taskTable, queue);

                    populateFields();
                    employeeMode();
                }
            }
        }

        private void btnSaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password.Length > 0)
            {
                createFileObject();
                saveFD.FileName = "untitled";

                // NOTE: ShowDialog() returns a nullable bool, however, according to MSDN it only returns true or false.
                if ((bool)saveFD.ShowDialog())
                {
                    binRW.Filepath = saveFD.FileName;
                    binRW.FileData = fileObject;

                    if (binRW.writeFile())
                    {
                        MessageBox.Show("File saved successfully.", "Save as", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please enter a file password in the password field.", "File Requires Password", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private void createFileObject()
        {
            byte[] hash = createHash(txtPassword.Password);
            fileObject = new ProtectedFileObject(queue, hash);
        }

        private void txtID_LostFocus(object sender, RoutedEventArgs e)
        {
            updateQueue_ID();
        }

        private bool updateQueue_ID()
        {
            bool isSuccess = false;
            if (queueIndex > -1
                && queueIndex < taskTable.Rows.Count)
            {
                //get node+task of current selected row
                QueueNode node = queue.NodeList[queueIndex];
                Task task = node.Data;

                //try to update task
                int temp = 0;
                if (int.TryParse(txtID.Text, out temp))
                {
                    //update value in queue
                    task.ID = temp;
                    //update value in datatable
                    taskTable.Rows[queueIndex].SetField(0, temp);
                    isSuccess = true;
                }
            }
            return isSuccess;
        }
        private void datePicker_LostFocus(object sender, RoutedEventArgs e)
        {
            updateQueue_DueDate();
        }
        private bool updateQueue_DueDate()
        {
            bool isSuccess = false;
            if (queueIndex > -1
                && queueIndex < taskTable.Rows.Count)
            {
                //try to update task
                if (datePicker.SelectedDate != null)
                {
                    //get node+task of current selected row
                    QueueNode node = queue.NodeList[queueIndex];
                    Task task = node.Data;

                    // set only due time to combo box time
                    task.DueDateTime = (DateTime)datePicker.SelectedDate + task.DueDateTime.TimeOfDay;
                    //Update readOnly field
                    txtDueDateTime.Text = task.DueDateTime.ToString(DATETIME_FORMAT);
                    //update datagrid
                    taskTable.Rows[queueIndex].SetField(2, txtDueDateTime.Text);

                    isSuccess = true;
                }
            }
            return isSuccess;
        }
        private void cbxTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateQueue_DueTime();
        }
        private bool updateQueue_DueTime()
        {
            bool isSuccess = false;
            if (queueIndex > -1
                && queueIndex < taskTable.Rows.Count)
            {
                //try to update task
                if (cbxTime.SelectedIndex > -1)
                {
                    //get node+task of current selected row
                    QueueNode node = queue.NodeList[queueIndex];
                    Task task = node.Data;

                    //Update only due date to datePicker date
                    task.DueDateTime = task.DueDateTime.Date + cbxTime_List[cbxTime.SelectedIndex];
                    //Update readOnly field
                    txtDueDateTime.Text = task.DueDateTime.ToString(DATETIME_FORMAT);
                    //update datagrid
                    taskTable.Rows[queueIndex].SetField(2, txtDueDateTime.Text);

                    isSuccess = true;
                }
            }
            return isSuccess;
        }
        private void txtTitle_LostFocus(object sender, RoutedEventArgs e)
        {
            updateQueue_Title();
        }

        private bool updateQueue_Title()
        {
            bool isSuccess = false;
            if (queueIndex > -1
                && queueIndex < taskTable.Rows.Count)
            {
                //try to update task
                if (txtTitle.Text.Length > 0)
                {
                    //get node+task of current selected row
                    QueueNode node = queue.NodeList[queueIndex];
                    Task task = node.Data;

                    //update value in queue
                    task.Title = txtTitle.Text;
                    //update value in datatable
                    taskTable.Rows[queueIndex].SetField(1, txtTitle.Text);
                    isSuccess = true;
                }
            }
            return isSuccess;
        }
        private void txtDetails_LostFocus(object sender, RoutedEventArgs e)
        {
            updateQueue_Details();
        }
        private bool updateQueue_Details()
        {
            bool isSuccess = false;
            if (queueIndex > -1
                && queueIndex < taskTable.Rows.Count)
            {
                //try to update task
                if (txtDetails.Text.Length > 0)
                {
                    //get node+task of current selected row
                    QueueNode node = queue.NodeList[queueIndex];
                    Task task = node.Data;

                    //update value in queue
                    task.Details = txtDetails.Text;

                    isSuccess = true;
                }
            }
            return isSuccess;
        }

        // update QueueNode with current form data
        private void updateQueue()
        {
            updateQueue_ID();
            updateQueue_Title();
            updateQueue_Details();
        }

        // facilitates regex usage
        private void txtID_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void dgTasks_row_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            queueIndex = dgTasks.SelectedIndex;
            if (queueIndex > -1
                && queueIndex < taskTable.Rows.Count)
            {
                populateFields(dgTasks.SelectedIndex);
            }
        }
        private byte[] createHash(String value)
        {
            byte[] valueBytes = KeyDerivation.Pbkdf2(
                value, saltDump(), KeyDerivationPrf.HMACSHA512,
                10000, 32
            );

            return valueBytes;
        }
        public static byte[] saltDump()
        {
            byte[] salt = new byte[32];

            for (int i = 0; i < salt.Length; i++)
            {
                salt[i] = 0;
            }

            return salt;
        }

        private void btnPasswordSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password.Length > 0 && fileObject.Hash != null)
            {
                byte[] userHash = createHash(txtPassword.Password);

                if (compareHash(userHash, fileObject.Hash))
                {
                    managerMode();
                    MessageBox.Show("Correct password entered, file edit enabled.", "Password Accepted", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Entered password is incorrect.", "Password Declined", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                // Clear password field
                txtPassword.Password = "";
            }
        }
        private bool compareHash(byte[] hash1, byte[] hash2)
        {
            bool isEqual = true;
            if (hash1.Length == hash2.Length)
            {
                for (int i = 0; i < hash1.Length; i++)
                {
                    if (hash1[i] != hash2[i])
                    {
                        isEqual = false;
                        i = hash1.Length;
                    }
                }
            }
            else
            {
                isEqual = false;
            }
            return isEqual;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            updateQueue_Add();
        }
        private bool updateQueue_Add()
        {
            bool isSuccess = false;
            QueueNode node;
            QueueNode lastNode = null;
            int tempID = 0;
                
            if (queue.NodeList.Count > 0)
            {
                lastNode = queue.NodeList[queue.NodeList.Count - 1];
                // set ID of new task to the ID of the last task, plus 1
                tempID = lastNode.Data.ID + 1;
            }

            // initialise node/task
            node = new QueueNode(new Task(tempID,
                DateTime.Now, null, null, TaskPriority.Low));
            // add to queue
            if (lastNode != null)
            {
                lastNode.NextNode = node;
            }
            queue.NodeList.Add(node);
            // add to dataTable
            Object[] row = new Object[] { node.Data.ID, node.Data.Title,
                                     node.Data.DueDateTimeString() };
            taskTable.Rows.Add(row);

            return isSuccess;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            updateQueue_Delete();
        }
        private bool updateQueue_Delete()
        {
            bool isSuccess = false;
            if (queueIndex > -1
                && queueIndex < taskTable.Rows.Count)
            {
                //get node+task of current selected row
                QueueNode node = queue.NodeList[queueIndex];

                //remove .NextNode reference
                if (queueIndex > 0)
                {
                    if (node.NextNode != null)
                    {
                        queue.NodeList[queueIndex - 1].NextNode = node.NextNode;
                    }
                }
                else if (queue.NodeList.Count > 1)
                {
                    queue.Head = queue.NodeList[1];
                }
                queue.NodeList.RemoveAt(queueIndex);
                node = null;

                // And again in dataTable
                taskTable.Rows[queueIndex].Delete();

                //dgTasks.
                clearFields();
                queueIndex = -1;

                isSuccess = true;
            }
            return isSuccess;
        }
        private void employeeMode()
        {
            txtID.IsReadOnly = true;

            datePicker.IsEnabled = false;
            cbxTime.IsEnabled = false;
            txtDueDateTime.IsReadOnly = true;

            txtTitle.IsReadOnly = true;
            txtDetails.IsReadOnly = true;

            btnAdd.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnPasswordSubmit.IsEnabled = true;
            btnSaveFile.IsEnabled = false;
        }
        private void managerMode()
        {
            txtID.IsReadOnly = false;

            datePicker.IsEnabled = true;
            cbxTime.IsEnabled = true;
            txtDueDateTime.IsReadOnly = false;

            txtTitle.IsReadOnly = false;
            txtDetails.IsReadOnly = false;

            btnAdd.IsEnabled = true;
            btnDelete.IsEnabled = true;
            btnPasswordSubmit.IsEnabled = false;
            btnSaveFile.IsEnabled = true;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            int temp = -1;
            temp = binarySearch(queue.NodeList, 9);
            if (temp < 0)
            {
                MessageBox.Show("Value is either not present or queue is not sorted.", "Value Not Found", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            dgTasks.SelectedIndex = temp;
            queueIndex = temp;
        }
        private int binarySearch(List<QueueNode> queueList, int value)
        {
            int max = queueList.Count - 1;
            int min = 0;
            int index = -1;
            bool isFound = false;

            while (min <= max)
            {
                index = (max + min) / 2;

                if (queueList[index].Data.ID == value)
                {
                    min = max + 1;
                    isFound = true;
                }
                else if (queueList[index].Data.ID < value)
                {
                    min = index + 1;
                }
                else
                {
                    max = index - 1;
                }
            }

            return isFound ? index : -1;
        }

        private void btnSort_Click(object sender, RoutedEventArgs e)
        {
            queue = new Queue(
                QuickSort.quickSort(
                    queue.NodeList.ToArray()));

            InsertTasks(taskTable, queue);
        }
    }
}
