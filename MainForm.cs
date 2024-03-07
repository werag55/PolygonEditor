using System.Windows.Forms;

namespace PolygonEditor
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            Manager.colorDialog = this.colorDialog;
            EditMode.contextMenuStrip = this.contextMenuStrip;
            EditMode.horizontalMenuItem = this.horizontalMenuItem;
            EditMode.verticalMenuItem = this.verticalMenuItem;

            Manager.generateStartScene();
            workSpace.Invalidate();
        }

        #region MenuStrip

        #region file

        private void saveMenuItem_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;

            // Displays a SaveFileDialog so the user can save the Image
            // assigned to Button2.
            using (SaveFileDialog saveFileDialog1 = new SaveFileDialog())
            {
                saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
                saveFileDialog1.Title = "Save scene";
                saveFileDialog1.ShowDialog();

                // If the file name is not an empty string open it for saving.
                if (saveFileDialog1.FileName != "")
                {
                    //Get the path of specified file
                    filePath = saveFileDialog1.FileName;
                    Manager.SaveData(filePath);
                }
            }
        }

        private void loadMenuItem_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                    Manager.LoadData(filePath);
                }
            }

            workSpace.Invalidate();
        }

        #endregion

        #region mode
        private void drawingMenuItem_Click(object sender, EventArgs e)
        {
            if (!drawingMenuItem.Checked)
            {
                drawingMenuItem.Checked = true;
                editingMenuItem.Checked = false;

                Manager.mode = Mode.Draw;
            }
        }

        private void editingMenuItem_Click(object sender, EventArgs e)
        {
            if (!editingMenuItem.Checked)
            {
                editingMenuItem.Checked = true;
                drawingMenuItem.Checked = false;

                Manager.mode = Mode.Edit;
            }
        }

        #endregion

        #region settings

        private void libraryAlgorithmMenuItem_Click(object sender, EventArgs e)
        {
            if (!libraryAlgorithmMenuItem.Checked)
            {
                libraryAlgorithmMenuItem.Checked = true;
                bresenhamAlgorithmMenuItem.Checked = false;
                wUAlgorithmMenuItem.Checked = false;

                Manager.drawSet = DrawSet.Lib;
            }

            workSpace.Invalidate();
        }

        private void bresenhamAlgorithmMenuItem_Click(object sender, EventArgs e)
        {
            if (!bresenhamAlgorithmMenuItem.Checked)
            {
                bresenhamAlgorithmMenuItem.Checked = true;
                libraryAlgorithmMenuItem.Checked = false;
                wUAlgorithmMenuItem.Checked = false;

                Manager.drawSet = DrawSet.Bresenham;
            }

            workSpace.Invalidate();
        }

        private void wUAlgorithmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!wUAlgorithmMenuItem.Checked)
            {
                bresenhamAlgorithmMenuItem.Checked = false;
                libraryAlgorithmMenuItem.Checked = false;
                wUAlgorithmMenuItem.Checked = true;

                Manager.drawSet = DrawSet.WU;
            }

            workSpace.Invalidate();
        }

        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                workSpace.BackColor = colorDialog.Color;
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!toolStripMenuItem1.Checked)
            {
                toolStripMenuItem1.Checked = true;
                toolStripMenuItem2.Checked = false;
                toolStripMenuItem3.Checked = false;

                Manager.offset = Offset.I;
            }
            workSpace.Invalidate();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (!toolStripMenuItem2.Checked)
            {
                toolStripMenuItem2.Checked = true;
                toolStripMenuItem1.Checked = false;
                toolStripMenuItem3.Checked = false;

                Manager.offset = Offset.II;
            }
            workSpace.Invalidate();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (!toolStripMenuItem3.Checked)
            {
                toolStripMenuItem3.Checked = true;
                toolStripMenuItem1.Checked = false;
                toolStripMenuItem2.Checked = false;

                Manager.offset = Offset.III;
            }
            workSpace.Invalidate();
        }

        #endregion

        #endregion

        #region panel

        private void colorButton_Click(object sender, EventArgs e)
        {
            Manager.colorButton_Click(sender, e);
        }

        #endregion

        #region workSpace

        private void workSpace_MouseDown(object sender, MouseEventArgs e)
        {
            Manager.workSpace_MouseDown(sender, e);
        }

        private void workSpace_MouseMove(object sender, MouseEventArgs e)
        {
            Manager.workSpace_MouseMove(sender, e);
        }

        private void workSpace_Paint(object sender, PaintEventArgs e)
        {
            Manager.workSpace_Paint(sender, e);

        }

        private void workSpace_MouseUp(object sender, MouseEventArgs e)
        {
            Manager.workSpace_MouseUp(sender, e);
        }

        private void workSpace_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Manager.workSpace_MouseDoubleClick(sender, e);
        }

        #endregion

        #region ContextMenuStrip

        private void horizontalMenuItem_Click(object sender, EventArgs e)
        {
            Manager.horizontalMenuItem_Click(sender, e);
        }

        private void verticalMenuItem_Click(object sender, EventArgs e)
        {
            Manager.verticalMenuItem_Click(sender, e);
        }

        #endregion

        #region offsetTrackBar
        private void offsetTrackBar_Scroll(object sender, EventArgs e)
        {
            Manager.offsetTrackBar_Scroll(sender, e);
            workSpace.Invalidate();
        }
        #endregion

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            Manager.MainForm_KeyDown(sender, e);
            workSpace.Invalidate();
        }
    }
}