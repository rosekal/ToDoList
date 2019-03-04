using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace ToDoList {
    internal class FileManager {
        //Readonly strings for where to save backup information
        private readonly string BACKUP_DIRECTORY = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\KalenGitHub\ToDoList\";
        private readonly string BACKUP_FILE = "backup.tdl";

        internal List<Task> RestoreBackup() {
            ValidateBackupFile();

            //Simply reading all lines from the backup file.
            return ReadFromXMLFile(BACKUP_DIRECTORY + BACKUP_FILE);
        }

        internal void ValidateBackupFile() {
            //If the backup directory doesn't exist, create one.
            if (!Directory.Exists(BACKUP_DIRECTORY)) {
                Directory.CreateDirectory(BACKUP_DIRECTORY);
            }

            //If the backup file doesn't exist, create one.
            if (!File.Exists(BACKUP_DIRECTORY + BACKUP_FILE)) {
                File.Create(BACKUP_DIRECTORY + BACKUP_FILE).Close();
            }

            //If backup file is corrupted/new, re-write it
            XmlDocument doc = new XmlDocument();

            try {
                doc.Load(BACKUP_DIRECTORY + BACKUP_FILE);
            } catch (XmlException) {
                using (StreamWriter sw = new StreamWriter(BACKUP_DIRECTORY + BACKUP_FILE)) {
                    sw.WriteLine("<Tasks>");
                    sw.WriteLine("</Tasks>");
                }
            }
        }

        internal void BackUpFile(List<Task> data) {
            ValidateBackupFile();

            //Writing to the backup file
            WriteToFile(BACKUP_DIRECTORY + BACKUP_FILE, data);
        }

        internal void WriteToFile(string path, List<Task> data) {
            using (StreamWriter sw = new StreamWriter(path)) {
                sw.WriteLine("<Tasks>");

                foreach (Task task in data) {
                    sw.WriteLine($"\t<Task id=\"{task.Id}\">");

                    sw.WriteLine($"\t\t<Name>{task.Name}</Name>");
                    sw.WriteLine($"\t\t<Completed>{task.Completed}</Completed>");

                    sw.WriteLine("\t</Task>");
                }

                sw.WriteLine("</Tasks>");
            }
        }

        internal List<Task> ReadFromXMLFile(string path) {
            XmlDocument doc = new XmlDocument();

            try {
                doc.Load(path);
            } catch (XmlException) {
                MessageBox.Show("There is something wrong with the TDL file.  Please try another one.", "Open TDL Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return null;
            } catch (IOException) {
                return new List<Task> { };
            }

            List<Task> tasks = new List<Task>();

            foreach (XmlNode node in doc.DocumentElement) {
                string id = node.Attributes[0].InnerText;

                string name = null;
                bool completed = false;
                foreach (XmlNode subnode in node.ChildNodes) {
                    if (subnode.Name == "Name") {
                        name = subnode.InnerText;
                    } else if (subnode.Name == "Completed") {
                        completed = Boolean.Parse(subnode.InnerText);
                    }
                }

                tasks.Add(new Task(id, name, completed));
            }

            return tasks;
        }

        internal string SaveNewFile(List<Task> toDoList) {
            SaveFileDialog saveFile = new SaveFileDialog {
                Filter = "ToDoList file (*.tdl)|*.tdl",
                DefaultExt = "tdl",
                AddExtension = true
            };

            if (saveFile.ShowDialog() == DialogResult.OK) {
                WriteToFile(saveFile.FileName, toDoList);
                UserInput.currFile = Path.GetFileName(saveFile.FileName);

                return saveFile.FileName;
            }

            return "";
        }

        internal List<Task> OpenNewFile() {
            OpenFileDialog openFile = new OpenFileDialog {
                Filter = "ToDoList file (*.tdl)|*.tdl",
                DefaultExt = "tdl",
                AddExtension = true
            };

            if (openFile.ShowDialog() == DialogResult.OK) {
                UserInput.currFile = Path.GetFileName(openFile.FileName);
                return ReadFromXMLFile(openFile.FileName);
            }

            return null;
        }
    }
}