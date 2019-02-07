using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Drawing;

namespace ToDoList {
    public partial class UserInput : Form {
        private List<Task> toDoList = new List<Task>();
        private FileManager fm;

        private TextBox txbx;
        private CheckBox chkbx;
        private Button btn;

        private string currFile = "";

        private int x = 0, y = 0;

        private string AppName = ConfigurationManager.AppSettings["AppName"];

        public UserInput() {
            InitializeComponent();
            this.Text = AppName;

            fm = new FileManager();

            toDoList = fm.RestoreBackup();


            AddInputWidgets();

            PopulateToDoList();


            //Backup to the file every 5 minutes
            var t = new System.Threading.Timer(o => fm.BackUpFile(toDoList), null, 10000, 10000);
        }

        private void AddInputWidgets() {
            //Create dull checkbox
            chkbx = new CheckBox() {
                Checked = false,
                Enabled = false,
                AutoSize = true
            };

            gbxList.Controls.Add(chkbx);

            //Create textbox for user input
            txbx = new TextBox() {
                Width = 200
            };

            txbx.TextChanged += new EventHandler(txbx_TextChanged);
            gbxList.Controls.Add(txbx);

            //Create button to add the user's task
            btn = new Button() {
                Text = "Create",
            };

            btn.Click += new EventHandler(btnCreate_Click);

            gbxList.Controls.Add(btn);
        }

        private void txbx_TextChanged(object sender, EventArgs e) {
            Size size = TextRenderer.MeasureText(txbx.Text, txbx.Font);
            if (size.Width > txbx.Width && (size.Width + 40) < gbxList.Width) {
                txbx.Width = size.Width;
            }
        }

        private void btnCreate_Click(object sender, EventArgs e) {
            //Add new item to the list
            toDoList.Add(new Task(Task.CreateId(), txbx.Text, false));

            PopulateToDoList();

            fm.BackUpFile(toDoList);
        }

        private void UserInput_FormClosing(object sender, FormClosingEventArgs e) {
            fm.BackUpFile(toDoList);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            PromptSaveLocation();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog openFile = new OpenFileDialog {
                Filter = "ToDoList file (*.tdl)|*.tdl",
                DefaultExt = "tdl",
                AddExtension = true
            };

            if (openFile.ShowDialog() == DialogResult.OK) {
                var results = fm.ReadFromFile(openFile.FileName);

                if (results != null) {
                    toDoList = results;
                    this.Text = $"{Path.GetFileName(openFile.FileName)} - {AppName}";
                    currFile = openFile.FileName;
                    PopulateToDoList();
                }
            }
        }

        private void PopulateToDoList() {
            x = 10;
            y = 20;

            gbxList.Controls.Clear();
            AddInputWidgets();
            foreach (Task task in toDoList) {
                CheckBox check = new CheckBox {
                    Text = task.Name,
                    Checked = task.Completed,
                    Location = new Point(x, y),
                    AutoSize = true,
                    Tag = task.Id
                };

                check.CheckStateChanged += new EventHandler(chkbx_CheckStateChanged);

                if (gbxList.Height < check.Location.Y + 70) {
                    gbxList.Height += 30;
                }

                gbxList.Controls.Add(check);
                this.Height = 500;

                y += 30;
            }

            PositionInputWidgets();
        }

        private void chkbx_CheckStateChanged(object sender, EventArgs e) {
            CheckBox chkbx = (CheckBox)sender;

            foreach (Task task in toDoList) {
                if (task.Id == chkbx.Tag.ToString()) {
                    task.Completed = chkbx.Checked;
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            if (currFile.Equals("")) {
                PromptSaveLocation();
            } else {
                fm.WriteToFile(currFile, toDoList);
            }
        }

        private void PromptSaveLocation() {
            SaveFileDialog saveFile = new SaveFileDialog {
                Filter = "ToDoList file (*.tdl)|*.tdl",
                DefaultExt = "tdl",
                AddExtension = true
            };

            if (saveFile.ShowDialog() == DialogResult.OK) {
                this.Text = $"{Path.GetFileName(saveFile.FileName)} - {AppName}";
                fm.WriteToFile(saveFile.FileName, toDoList);

                currFile = saveFile.FileName;
            }
        }

        private void PositionInputWidgets() {
            chkbx.Location = new Point(x, y);
            txbx.Location = new Point(x + 20, y - chkbx.Height / 4);
            btn.Location = new Point(x + 20, y + 20);
        }
    }
}
