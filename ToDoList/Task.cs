using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ToDoList {
    public class Task {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Completed { get; set; }

        public Task() {

        }

        public Task(string Id, string Name, bool Completed) {
            this.Id = Id;
            this.Name = Name;
            this.Completed = Completed;
        }

        public static string CreateId() {
            char[] alphaNumeric = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890".ToCharArray();

            string code = "";
            Random r = new Random();

            for (int i = 0; i < 10; i++) {
                code += alphaNumeric[r.Next(0, alphaNumeric.Length - 1)];
            }

            return code;
        }

        public static Task CreateNewTask(string info) {
            //Validate input
            if (info == "") {
                MessageBox.Show("Task cannot be empty.", "Task Creation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return null;
            } else if (info.Length > 250) {
                MessageBox.Show("Task cannot be more than 250 characters.  Please shorten the task.", "Task Creation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return null;
            }

            //Add new item to the list
            return new Task(Task.CreateId(), info, false);
        }
    }
}
