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

namespace ToDoList
{
    public partial class UserInput : Form
    {

        List<string> toDoList = new List<string>();

        public UserInput()
        {
            InitializeComponent();
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
    }
}
