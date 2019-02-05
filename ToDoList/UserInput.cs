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

        private readonly string DIRECTORY = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\KalenGitHub\ToDoList\";
        private readonly string FILE = "backup.txt";

        List<string> toDoList = new List<string>();

        public UserInput(){
            InitializeComponent();

            ReadFromFile();
            var t = new System.Threading.Timer(o => BackUpFile(), null, 0, 300000);
        }

        private void btnGo_Click(object sender, EventArgs e){
            toDoList.Clear();

            var lines = txtbxList.Text.Split('\n');
            foreach (var line in lines){
                toDoList.Add(line);
            }

            var newForm = new ToDoList(toDoList);
            newForm.Show(this);
        }

        private void ReadFromFile() {
            ValidateBackupFile();

            txtbxList.Text = File.ReadAllText(DIRECTORY + FILE).Trim();
        }

        private void ValidateBackupFile() {
            if (!Directory.Exists(DIRECTORY)) {
                Directory.CreateDirectory(DIRECTORY);
            }

            if (!File.Exists(DIRECTORY + FILE)) {
                File.Create(DIRECTORY + FILE);
            }
        }

        private void BackUpFile() {
            ValidateBackupFile();

            using (StreamWriter sw = new StreamWriter(DIRECTORY + FILE)) {
                sw.WriteLine(txtbxList.Text);
            }
        }

        private void UserInput_FormClosing(object sender, FormClosingEventArgs e) {
            BackUpFile();
        }
    }
}
