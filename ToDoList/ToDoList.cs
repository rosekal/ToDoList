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
    public partial class ToDoList : Form
    {
        public List<string> toDoList = new List<string>();

        public ToDoList()
        {
            InitializeComponent();
        }

        public ToDoList(List<string> list)
        {
            this.toDoList.Clear();
            this.toDoList = list;
            InitializeComponent();


            int y = 20;
            int x = 10;
            foreach (var item in toDoList)
            {
                Debug.WriteLine("test");
                Debug.WriteLine(item);
                CheckBox check = new CheckBox
                {
                    Text = item,
                    Location = new Point(x, y),
                    AutoSize = true
                };

                if(gbxList.Height < check.Location.Y + 20){
                    gbxList.Height += 30;
                }

                if(gbxList.Height + 10 > this.Height){
                    this.Height += 50;
                }

                gbxList.Controls.Add(check);
                this.Height = 500;
                y += 20;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }
    }
}
