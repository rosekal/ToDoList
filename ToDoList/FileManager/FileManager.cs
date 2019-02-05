using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList {
    internal class FileManager {
        //Readonly strings for where to save backup information
        private readonly string BACKUP_DIRECTORY = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\KalenGitHub\ToDoList\";
        private readonly string BACKUP_FILE = "backup.tdl";

        internal string RestoreBackup() {
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

        internal void BackUpFile(string data) {
            ValidateBackupFile();

            //Writing to the backup file.
            WriteToFile(BACKUP_DIRECTORY + BACKUP_FILE, data);
        }

        internal void WriteToFile(string path, string data) {
            //Thread.Sleep(500);
            using (StreamWriter sw = new StreamWriter(path)) {
                sw.WriteLine(data);
            }
        }

        internal string ReadFromFile(string path) {
            return File.ReadAllText(path).Trim();
        }
    }
}
