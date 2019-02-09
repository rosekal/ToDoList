﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Drawing;

namespace ToDoList {
    public partial class UserInput : Form {
        private List<Task> toDoList = new List<Task>();
        private FileManager fm;

        private TextBox txbx;
        private CheckBox chkbx;
        private Button createBtn;
        private Button clearBtn;
        //private VScrollBar scroll;

        private string currFile = "";

        private int x = 0, y = 0;

        private string AppName = ConfigurationManager.AppSettings["AppName"];

        public UserInput() {
            InitializeComponent();
            SetTitle(null);

            fm = new FileManager();

            toDoList = fm.RestoreBackup();


            CreateInputWidgets();

            PopulateToDoList();

            AutoFocusTextBox();

            //Backup to the file every 5 minutes
            var t = new System.Threading.Timer(o => fm.BackUpFile(toDoList), null, 10000, 10000);
        }

        private void txbx_TextChanged(object sender, EventArgs e) {
            Size size = TextRenderer.MeasureText(txbx.Text, txbx.Font);
            if (size.Width > txbx.Width && (size.Width + 40) < mainPanel.Width) {
                txbx.Width = size.Width;
            }
        }

        private void btnCreate_Click(object sender, EventArgs e) {
            CreateNewTask();
        }

        private void CreateNewTask() {
            //Validate input
            if (txbx.Text == "") {
                MessageBox.Show("Task cannot be empty.", "Task Creation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            } else if (txbx.Text.Length > 250) {
                MessageBox.Show("Task cannot be more than 250 characters.  Please shorten the task.", "Task Creation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            //Add new item to the list
            toDoList.Add(new Task(Task.CreateId(), txbx.Text, false));

            PopulateToDoList();

            fm.BackUpFile(toDoList);

            AutoFocusTextBox();
        }

        private void btnClear_Click(object sender, EventArgs e) {
            DialogResult result = MessageBox.Show("Are you sure you want to clear this list?", 
                "Clear List Confirmation", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes) {
                ResetForm();
            }
        }

        private void UserInput_FormClosing(object sender, FormClosingEventArgs e) {
            fm.BackUpFile(toDoList);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            PromptSaveLocation();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog openFile = new OpenFileDialog {
                Filter = "ToDoList file (*.tdl)|*.tdl",
                DefaultExt = "tdl",
                AddExtension = true
            };

            if (openFile.ShowDialog() == DialogResult.OK) {
                var results = fm.ReadFromFile(openFile.FileName);

                if (results != null) {
                    toDoList = results;
                    SetTitle(Path.GetFileName(openFile.FileName));
                    currFile = openFile.FileName;
                    PopulateToDoList();
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            if (currFile.Equals("")) {
                PromptSaveLocation();
            } else {
                fm.WriteToFile(currFile, toDoList);
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e) {
            ResetForm();

            //Setting current file to nothing
            currFile = "";
            SetTitle(null);
        }

        private void chkbx_CheckStateChanged(object sender, EventArgs e) {
            CheckBox chkbx = (CheckBox)sender;

            foreach (Task task in toDoList) {
                if (task.Id == chkbx.Tag.ToString()) {
                    task.Completed = chkbx.Checked;
                }
            }
        }

        private void chkbx_Clicked(object sender, EventArgs e) {
            MouseEventArgs me = (MouseEventArgs)e;

            if (me.Button == MouseButtons.Right) {

                CheckBox chkbx = (CheckBox)sender;

                for (int i = 0; i < toDoList.Count; i++) {
                    if (toDoList[i].Id.Equals(chkbx.Tag)) {
                        toDoList.Remove(toDoList[i]);
                    }
                }

                PopulateToDoList();
            }
        }

        bool entered = false;

        private void chkbx_Enter(object sender, EventArgs e) {
            if (!entered) {
                CheckBox hovered = (CheckBox)sender;
                ToolTip tp = new ToolTip();
                tp.SetToolTip(hovered, "Right click to remove");

                entered = true;
            }
        }

        private void chkbx_Leave(object sender, EventArgs e) {
            entered = false;
        }

        private void ResetForm() {
            //Resetting x and y variables
            x = 10;
            y = 20;

            //Clearing the list, and recreating the input widgets
            toDoList.Clear();
            mainPanel.Controls.Clear();
            CreateInputWidgets();
            PositionInputWidgets();
        }

        private void SetTitle(string title) {
            this.Text = (title == null) ? AppName : $"{title} - {AppName}";
        }

        private void CreateInputWidgets() {
            //Create dull checkbox
            chkbx = new CheckBox() {
                Checked = false,
                Enabled = false,
                AutoSize = true
            };

            mainPanel.Controls.Add(chkbx);

            //Create textbox for user input
            txbx = new TextBox() {
                Width = 200
            };

            txbx.TextChanged += new EventHandler(txbx_TextChanged);
            txbx.KeyDown += new KeyEventHandler(txbx_KeyPressed);
            mainPanel.Controls.Add(txbx);

            //Create button to add the user's task
            createBtn = new Button() {
                Text = "Create",
            };

            createBtn.Click += new EventHandler(btnCreate_Click);

            mainPanel.Controls.Add(createBtn);

            //Clear button to clear the current list
            clearBtn = new Button() {
                Text = "Clear"
            };

            clearBtn.Click += new EventHandler(btnClear_Click);

            mainPanel.Controls.Add(clearBtn);

            PositionInputWidgets();
        }

        private void txbx_KeyPressed(object sender, EventArgs e) {
            KeyEventArgs key = (KeyEventArgs) e;

            if(key.KeyCode == Keys.Enter) {
                CreateNewTask();
            }
        }

        private void PopulateToDoList() {
            x = 10;
            y = 20;

            mainPanel.Controls.Clear();
            CreateInputWidgets();
            foreach (Task task in toDoList) {
                CheckBox check = new CheckBox {
                    Text = task.Name,
                    Checked = task.Completed,
                    Location = new Point(x, y),
                    AutoSize = true,
                    Tag = task.Id
                };

                check.CheckStateChanged += new EventHandler(chkbx_CheckStateChanged);
                check.MouseDown += new MouseEventHandler(chkbx_Clicked);
                check.MouseEnter += new EventHandler(chkbx_Enter);
                check.MouseLeave += new EventHandler(chkbx_Leave);

                if (mainPanel.Height > 350) {
                    //scroll.Enabled = true;
                } else if (mainPanel.Height < check.Location.Y + 70) {
                    //scroll.Enabled = false;
                    mainPanel.Height += 30;
                    gbxList.Height += 30;
                }

                mainPanel.Controls.Add(check);

                y += 30;
            }

            PositionInputWidgets();
        }

        private void PromptSaveLocation() {
            SaveFileDialog saveFile = new SaveFileDialog {
                Filter = "ToDoList file (*.tdl)|*.tdl",
                DefaultExt = "tdl",
                AddExtension = true
            };

            if (saveFile.ShowDialog() == DialogResult.OK) {
                SetTitle(Path.GetFileName(saveFile.FileName));
                fm.WriteToFile(saveFile.FileName, toDoList);

                currFile = saveFile.FileName;
            }
        }

        private void PositionInputWidgets() {
            chkbx.Location = new Point(x, y);
            txbx.Location = new Point(x + 20, y - chkbx.Height / 4);
            createBtn.Location = new Point(x + 20, y + 20);
            clearBtn.Location = new Point(x + 125, y + 20);

            mainPanel.AutoScrollPosition = new Point(0, mainPanel.VerticalScroll.Maximum);
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e) {
            foreach(Control con in mainPanel.Controls) {
                if(con is CheckBox && con != chkbx) {
                    ((CheckBox)con).Checked = chkAll.Checked;
                }
            }
        }

        private void AutoFocusTextBox() {
            txbx.Focus();
        }
    }
}
