using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                code += alphaNumeric[r.Next(0, alphaNumeric.Length-1)];
            }

            return code;
        }
    }
}
