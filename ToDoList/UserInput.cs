using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Drawing;

namespace ToDoList
{
    public partial class UserInput : Form{
        private List<Task> toDoList = new List<Task>();
        private FileManager fm;

        private TextBox txbx;

        private int x = 10, y = 20;

        private string AppName = ConfigurationManager.AppSettings["AppName"];

        public UserInput(){
            InitializeComponent();
            this.Text = AppName;

            fm = new FileManager();

            toDoList = fm.RestoreBackup();

            PopulateToDoList();

            //Create dull checkbox
            CheckBox chkbx = new CheckBox() {
                Checked = true,
                Location = new Point(x, y),
                AutoSize = true
            };

            gbxList.Controls.Add(chkbx);

            //Create textbox for user input
            txbx = new TextBox() {
                Location = new Point(x + 20, y - chkbx.Height / 4),
                Width = 200
            };

            txbx.TextChanged += new EventHandler(txbx_TextChanged);
            gbxList.Controls.Add(txbx);

            //Create button to add the user's task
            Button button = new Button() {
                Text = "Create",
                Location = new Point(x + 20, y + 20)
            };

            button.Click += new EventHandler(btnCreate_Click);

            gbxList.Controls.Add(button);

            //Backup to the file every 5 minutes
            var t = new System.Threading.Timer(o => fm.BackUpFile(toDoList), null, 10000, 10000);
        }

        private void txbx_TextChanged(object sender, EventArgs e) {
            Size size = TextRenderer.MeasureText(txbx.Text, txbx.Font);
            if (size.Width > txbx.Width && (size.Width + 40) < gbxList.Width ) {
                txbx.Width = size.Width;
            }
        }

        private void UpdateList(string text) {
            throw new NotImplementedException();
        }

        private void btnCreate_Click(object sender, EventArgs e){
            //Add new item to the list
            toDoList.Add(new Task(Task.CreateId(), txbx.Text, false));

            fm.BackUpFile(toDoList);
        }

        private void UserInput_FormClosing(object sender, FormClosingEventArgs e) {
            fm.BackUpFile(toDoList);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveFileDialog saveFile = new SaveFileDialog {
                Filter = "ToDoList file (*.tdl)|*.tdl",
                DefaultExt = "tdl",
                AddExtension = true
            };

            if (saveFile.ShowDialog() == DialogResult.OK) {
                this.Text = $"{Path.GetFileName(saveFile.FileName)} - {AppName}";
                fm.WriteToFile(saveFile.FileName, toDoList);
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog openFile = new OpenFileDialog {
                Filter = "ToDoList file (*.tdl)|*.tdl",
                DefaultExt = "tdl",
                AddExtension = true
            };

            if (openFile.ShowDialog() == DialogResult.OK) {
                this.Text = $"{Path.GetFileName(openFile.FileName)} - {AppName}";
                PopulateToDoList();
            }
        }

        private void PopulateToDoList() {
            foreach (Task task in toDoList) {
                CheckBox check = new CheckBox {
                    Text = task.Name,
                    Checked = task.Completed,
                    Location = new Point(x, y),
                    AutoSize = true
                };

                if (gbxList.Height < check.Location.Y + 20) {
                    gbxList.Height += 30;
                }

                if (gbxList.Height + 10 > this.Height) {
                    this.Height += 50;
                }

                gbxList.Controls.Add(check);
                this.Height = 500;

                y += 30;
            }
        }
    }
}
