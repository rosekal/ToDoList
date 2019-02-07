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

            CheckBox chkbx = new CheckBox() {
                Location = new Point(x, y)
            };

            txbx = new TextBox() {
                Location = new Point(x + 20, y),
                
            };

            gbxList.Controls.Add(chkbx);
            gbxList.Controls.Add(txbx);

            //Backup to the file every 5 minutes
            var t = new System.Threading.Timer(o => fm.BackUpFile(toDoList), null, 10000, 10000);
        }

        private void UpdateList(string text) {
            throw new NotImplementedException();
        }

        private void btnCreate_Click(object sender, EventArgs e){
            fm.BackUpFile(toDoList);

            //Add new item to the list
            toDoList.Add(new Task(Task.CreateId(), txbx.Text, false));
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
                y += 20;
            }
        }
    }
}
