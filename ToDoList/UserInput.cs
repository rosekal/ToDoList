using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Configuration;
using System.IO;

namespace ToDoList
{
    public partial class UserInput : Form{
        private List<string> toDoList = new List<string>();
        private FileManager fm;

        private string AppName = ConfigurationManager.AppSettings["AppName"];

        public UserInput(){
            InitializeComponent();
            this.Text = AppName;
            fm = new FileManager();

            //Restore backup file so it writes to the txt box on opening form
            txtbxList.Text = fm.RestoreBackup();

            //Backup to the file every 5 minutes
            var t = new System.Threading.Timer(o => fm.BackUpFile(txtbxList.Text), null, 10000, 10000);
        }

        private void btnGo_Click(object sender, EventArgs e){
            fm.BackUpFile(txtbxList.Text);

            //Clear anything that may be in the list
            toDoList.Clear();

            //Add every item to the list
            var lines = txtbxList.Text.Split('\n');
            foreach (var line in lines){
                toDoList.Add(line);
            }

            //Show the form where the checkboxes are
            var newForm = new ToDoList(toDoList);
            newForm.Show(this);
        }

        private void UserInput_FormClosing(object sender, FormClosingEventArgs e) {
            fm.BackUpFile(txtbxList.Text);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveFileDialog saveFile = new SaveFileDialog {
                Filter = "ToDoList file (*.tdl)|*.tdl",
                DefaultExt = "tdl",
                AddExtension = true
            };

            if (saveFile.ShowDialog() == DialogResult.OK) {
                this.Text = $"{Path.GetFileName(saveFile.FileName)} - {AppName}";
                fm.WriteToFile(saveFile.FileName, txtbxList.Text);
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
                txtbxList.Text = fm.ReadFromFile(openFile.FileName);
            }
        }
    }
}
