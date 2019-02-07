using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            return ReadFromFile(BACKUP_DIRECTORY + BACKUP_FILE);
        }

        internal void ValidateBackupFile() {
            //If the backup directory doesn't exist, create one.
            if (!Directory.Exists(BACKUP_DIRECTORY)) {
                Directory.CreateDirectory(BACKUP_DIRECTORY);
            }

            //If the backup file doesn't exist, create one.
            if (!File.Exists(BACKUP_DIRECTORY + BACKUP_FILE)) {
                File.Create(BACKUP_DIRECTORY + BACKUP_FILE);
            }
        }

        internal void BackUpFile(List<Task> data) {
            ValidateBackupFile();

            //Writing to the backup file.
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

        internal List<Task> ReadFromFile(string path) {
            XmlDocument doc = new XmlDocument();

            try {
                doc.Load(path);
            } catch (XmlException) {
                MessageBox.Show("There is something wrong with the TDL file.  Please try another one.", "Open TDL Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return null;
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
    }
}
