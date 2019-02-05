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

namespace ToDoList
{
    public partial class UserInput : Form{
        //Readonly strings for where to save backup information
        private readonly string DIRECTORY = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\KalenGitHub\ToDoList\";
        private readonly string FILE = "backup.txt";

        List<string> toDoList = new List<string>();

        public UserInput(){
            InitializeComponent();

            //Read from file so it writes to the txt box on opening form
            ReadFromFile();

            //Backup to the file every 5 minutes
            var t = new System.Threading.Timer(o => BackUpFile(), null, 0, 300000);
        }

        private void btnGo_Click(object sender, EventArgs e){
            BackUpFile();

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

        private void ReadFromFile() {
            ValidateBackupFile();

            //Simply reading all lines from the backup file.
            txtbxList.Text = File.ReadAllText(DIRECTORY + FILE).Trim();
        }

        private void ValidateBackupFile() {
            //If the backup directory doesn't exist, create one.
            if (!Directory.Exists(DIRECTORY)) {
                Directory.CreateDirectory(DIRECTORY);
            }

            //If the backup file doesn't exist, create one.
            if (!File.Exists(DIRECTORY + FILE)) {
                File.Create(DIRECTORY + FILE);
            }
        }

        private void BackUpFile() {
            ValidateBackupFile();

            //Writing to the backup file.
            using (StreamWriter sw = new StreamWriter(DIRECTORY + FILE)) {
                sw.WriteLine(txtbxList.Text);
            }
        }

        private void UserInput_FormClosing(object sender, FormClosingEventArgs e) {
            BackUpFile();
        }
    }
}
