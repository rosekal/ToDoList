using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Diagnostics;
using ToDoList.JSON;

namespace ToDoList {
    public partial class UserInput : Form {
        private List<Task> toDoList = new List<Task>();
        private FileManager fm;

        private TextBox txbx;
        private CheckBox chkbx;
        private Button createBtn;
        private Button clearBtn;

        public static string currFile = "";

        private int x = 0, y = 0;

        private static string AppName = ConfigurationManager.AppSettings["AppName"];

        public UserInput() {
            InitializeComponent();
            SetTitle(null);

            string s = JSONHelper.GetMovies();

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
            SaveNewFile();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenNewFile();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveCurrentFile();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e) {
            ResetForm();

            //Setting current file to nothing
            currFile = "";
            SetTitle(null);
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e) {
            PrintList();
        }

        private void chkbx_CheckStateChanged(object sender, EventArgs e) {
            CheckBox chkbx = (CheckBox)sender;

            for(int i = 0; i < toDoList.Count; i++) {
                Task task = toDoList[i];

                if (task.Id == chkbx.Tag.ToString()) {
                    task.Completed = chkbx.Checked;
                }
            }

            PopulateToDoList();
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
        CheckBox hovered;

        private void chkbxPnl_Hover(object sender, EventArgs e) {
            if (!entered) {
                hovered = (CheckBox) sender;
                Bitmap editBMP = new Bitmap(@"C:\Users\Kalen\source\repos\ToDoList\ToDoList\Icons\edit.png");
                Bitmap deleteBMP = new Bitmap(@"C:\Users\Kalen\source\repos\ToDoList\ToDoList\Icons\delete.png");

                editBMP.MakeTransparent();

                PictureBox editPB = new PictureBox {
                    Image = editBMP,
                    Height = 20,
                    Width = 20,
                    Location = new Point(hovered.Location.X + 60, hovered.Location.Y)
                };

                editPB.Click += new EventHandler(EditTask);

                editPB.SizeMode = PictureBoxSizeMode.StretchImage;

                PictureBox deletePB = new PictureBox {
                    Image = deleteBMP,
                    Height = 20,
                    Width = 20,
                    Location = new Point(hovered.Location.X + 90, hovered.Location.Y)
                };

                deletePB.SizeMode = PictureBoxSizeMode.StretchImage;

                mainPanel.Controls.Add(editPB);
                mainPanel.Controls.Add(deletePB);
                entered = true;
            }
        }

        private void chkbxPnl_Leave(object sender, EventArgs e) {
            entered = false;
        }

        private void EditTask(object sender, EventArgs e) {
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e) {
            foreach(Task task in toDoList) {
                task.Completed = chkAll.Checked;
            }

            PopulateToDoList();
        }

        private void txbx_KeyPressed(object sender, EventArgs e) {
            KeyEventArgs key = (KeyEventArgs)e;

            if (key.KeyCode == Keys.Enter) {
                CreateNewTask();
            }
        }
        
        private void CreateNewTask() {
            toDoList.Add(Task.CreateNewTask(txbx.Text));

            PopulateToDoList();
            fm.BackUpFile(toDoList);
            AutoFocusTextBox();
        }

        private void ResetForm() {
            //Resetting x and y variables
            ResetXAndY();

            //Clearing the list, and recreating the input widgets
            toDoList.Clear();
            mainPanel.Controls.Clear();
            CreateInputWidgets();
            AutoFocusTextBox();

            //Setting current file to nothing
            currFile = "";
        }

        private void SetTitle(string title) {
            Text = (title == null) ? AppName : $"{title} - {AppName}";
        }

        private void CreateInputWidgets() {
            //Create dull checkbox
            chkbx = new CheckBox() {
                Checked = false,
                Enabled = false,
                AutoSize = true
            };            

            //Create textbox for user input
            txbx = new TextBox() {
                Width = 200
            };

            txbx.TextChanged += new EventHandler(txbx_TextChanged);
            txbx.KeyDown += new KeyEventHandler(txbx_KeyPressed);   

            //Create button to add the user's task
            createBtn = new Button() {
                Text = "Create",
            };

            createBtn.Click += new EventHandler(btnCreate_Click);

            //Clear button to clear the current list
            clearBtn = new Button() {
                Text = "Clear"
            };

            clearBtn.Click += new EventHandler(btnClear_Click);


            //Add all controls to the main panel
            mainPanel.Controls.Add(chkbx);
            mainPanel.Controls.Add(txbx);
            mainPanel.Controls.Add(createBtn);
            mainPanel.Controls.Add(clearBtn);

            //Position them after creating
            PositionInputWidgets();
        }

        private void PopulateToDoList() {
            ResetXAndY();

            mainPanel.Controls.Clear();
            CreateInputWidgets();

            //Have to sort it by not completed first, then completed
            SortToDoList();

            foreach (Task task in toDoList) {

                Panel checkPanel = new Panel {
                    Location = new Point(x, y),
                    Height = 20,
                    Width = 100,
                    AutoSize = true,
                };

                //checkPanel.Enter += new EventHandler(chkbxPnl_Hover);
                //checkPanel.Leave += new EventHandler(chkbxPnl_Leave);

                CheckBox check = new CheckBox {
                    Text = task.Name,
                    Checked = task.Completed,
                    AutoSize = true,
                    Tag = task.Id
                };

                Font f = new Font(chkbx.Font, (check.Checked ? FontStyle.Strikeout : FontStyle.Regular));
                check.Font = f;

                check.CheckStateChanged += new EventHandler(chkbx_CheckStateChanged);
                check.MouseDown += new MouseEventHandler(chkbx_Clicked);

                if (mainPanel.Height < 500 && mainPanel.Height < checkPanel.Location.Y + 70) {
                    mainPanel.Height += 30;
                    gbxList.Height += 30;
                }

                checkPanel.Controls.Add(check);
                mainPanel.Controls.Add(checkPanel);

                y += 30;
            }

            PositionInputWidgets();
        }

        private void ResetXAndY() {
            x = 10;
            y = 20;
        }

        private void SortToDoList() {
            List<Task> temp = new List<Task>();

            //Loop #1: Add only not completed to list
            foreach (Task t in toDoList) {
                if (!t.Completed) {
                    temp.Add(t);
                }
            }

            //Loop #2: Append the completed tasks to end of list
            foreach (Task t in toDoList) {
                if (t.Completed) {
                    temp.Add(t);
                }
            }

            toDoList = temp;
        }

        private void PositionInputWidgets() {
            chkbx.Location = new Point(x, y);
            txbx.Location = new Point(x + 20, y - chkbx.Height / 4);
            createBtn.Location = new Point(x + 20, y + 20);
            clearBtn.Location = new Point(x + 125, y + 20);

            mainPanel.AutoScrollPosition = new Point(0, mainPanel.VerticalScroll.Maximum);
        }

        private void UserInput_KeyDown(object sender, KeyEventArgs e) {
            //If control was held down
            if (e.Control) {
                switch (e.KeyCode) {
                    //New to do list
                    case Keys.N:
                        ResetForm();
                        break;
                    
                    //Open to do list
                    case Keys.O:
                        OpenNewFile();
                        break;

                    //Save to do lists
                    case Keys.S:
                        SaveCurrentFile();
                        break;

                    case Keys.P:
                        PrintList();
                        break;
                }
            }

            if ((Keys.S | Keys.Alt | Keys.Control) == e.KeyData) {
                SaveNewFile();
            }
        }

        private void SaveNewFile() {
            fm.SaveNewFile(toDoList);
            SetTitle(currFile);
        }

        private void SaveCurrentFile() {
            if (currFile.Equals("")) {
                SaveNewFile();
            } else {
                fm.WriteToFile(currFile, toDoList);
            }
        }

        private void OpenNewFile() {
            var results = fm.OpenNewFile();

            if (results != null) {
                toDoList = results;
                SetTitle(currFile);
                PopulateToDoList();
            }
        }

        private void AutoFocusTextBox() {
            txbx.Focus();
        }

        Bitmap bmp;

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e) {
            e.Graphics.DrawImage(bmp, 0, 0);
        }

        private void PrintList() {
            Graphics g = this.CreateGraphics();
            bmp = new Bitmap(gbxList.Size.Width - 15, gbxList.Size.Height -100, g);

            Graphics mg = Graphics.FromImage(bmp);
            mg.CopyFromScreen(this.Location.X, this.Location.Y, 50, 50, this.Size);
            mg.CopyFromScreen(this.Location.X + gbxList.Location.X, 
                this.Location.Y + gbxList.Location.Y + menuStrip1.Height + 40, 0, 0, this.Size);
            printPreviewDialog1.ShowDialog();
        }
    }
}
