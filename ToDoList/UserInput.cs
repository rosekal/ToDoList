using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace ToDoList
{
    public partial class UserInput : Form{
        List<string> toDoList = new List<string>();
        FileManager fm;

        public UserInput(){
            InitializeComponent();

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
                txtbxList.Text = fm.ReadFromFile(openFile.FileName);
            }
        }
    }
}
