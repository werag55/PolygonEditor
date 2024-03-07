namespace PolygonEditor
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            statusStrip = new StatusStrip();
            menuStrip = new MenuStrip();
            fileMenuItem = new ToolStripMenuItem();
            saveMenuItem = new ToolStripMenuItem();
            loadMenuItem = new ToolStripMenuItem();
            modeMenuItem = new ToolStripMenuItem();
            drawingMenuItem = new ToolStripMenuItem();
            editingMenuItem = new ToolStripMenuItem();
            settingsMenuItem = new ToolStripMenuItem();
            drawingSegmentsMenuItem = new ToolStripMenuItem();
            libraryAlgorithmMenuItem = new ToolStripMenuItem();
            bresenhamAlgorithmMenuItem = new ToolStripMenuItem();
            wUAlgorithmMenuItem = new ToolStripMenuItem();
            backgroundColorToolStripMenuItem = new ToolStripMenuItem();
            offsetToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripMenuItem();
            toolStripMenuItem3 = new ToolStripMenuItem();
            workSpace = new PictureBox();
            contextMenuStrip = new ContextMenuStrip(components);
            horizontalMenuItem = new ToolStripMenuItem();
            verticalMenuItem = new ToolStripMenuItem();
            colorDialog = new ColorDialog();
            panel1 = new Panel();
            colorLabel = new Label();
            offsetLabel = new Label();
            offsetTrackBar = new TrackBar();
            colorButton = new Button();
            menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)workSpace).BeginInit();
            contextMenuStrip.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)offsetTrackBar).BeginInit();
            SuspendLayout();
            // 
            // statusStrip
            // 
            statusStrip.ImageScalingSize = new Size(24, 24);
            statusStrip.Location = new Point(0, 879);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(1504, 22);
            statusStrip.TabIndex = 0;
            statusStrip.Text = "statusStrip1";
            // 
            // menuStrip
            // 
            menuStrip.ImageScalingSize = new Size(24, 24);
            menuStrip.Items.AddRange(new ToolStripItem[] { fileMenuItem, modeMenuItem, settingsMenuItem });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(1504, 33);
            menuStrip.TabIndex = 1;
            menuStrip.Text = "menuStrip1";
            // 
            // fileMenuItem
            // 
            fileMenuItem.DropDownItems.AddRange(new ToolStripItem[] { saveMenuItem, loadMenuItem });
            fileMenuItem.Name = "fileMenuItem";
            fileMenuItem.Size = new Size(54, 29);
            fileMenuItem.Text = "File";
            // 
            // saveMenuItem
            // 
            saveMenuItem.Name = "saveMenuItem";
            saveMenuItem.Size = new Size(270, 34);
            saveMenuItem.Text = "Save";
            saveMenuItem.Click += saveMenuItem_Click;
            // 
            // loadMenuItem
            // 
            loadMenuItem.Name = "loadMenuItem";
            loadMenuItem.Size = new Size(270, 34);
            loadMenuItem.Text = "Load";
            loadMenuItem.Click += loadMenuItem_Click;
            // 
            // modeMenuItem
            // 
            modeMenuItem.DropDownItems.AddRange(new ToolStripItem[] { drawingMenuItem, editingMenuItem });
            modeMenuItem.Name = "modeMenuItem";
            modeMenuItem.Size = new Size(75, 29);
            modeMenuItem.Text = "Mode";
            // 
            // drawingMenuItem
            // 
            drawingMenuItem.Checked = true;
            drawingMenuItem.CheckState = CheckState.Checked;
            drawingMenuItem.Name = "drawingMenuItem";
            drawingMenuItem.Size = new Size(180, 34);
            drawingMenuItem.Text = "Drawing";
            drawingMenuItem.Click += drawingMenuItem_Click;
            // 
            // editingMenuItem
            // 
            editingMenuItem.Name = "editingMenuItem";
            editingMenuItem.Size = new Size(180, 34);
            editingMenuItem.Text = "Editing";
            editingMenuItem.Click += editingMenuItem_Click;
            // 
            // settingsMenuItem
            // 
            settingsMenuItem.DropDownItems.AddRange(new ToolStripItem[] { drawingSegmentsMenuItem, backgroundColorToolStripMenuItem, offsetToolStripMenuItem });
            settingsMenuItem.Name = "settingsMenuItem";
            settingsMenuItem.Size = new Size(92, 29);
            settingsMenuItem.Text = "Settings";
            // 
            // drawingSegmentsMenuItem
            // 
            drawingSegmentsMenuItem.DropDownItems.AddRange(new ToolStripItem[] { libraryAlgorithmMenuItem, bresenhamAlgorithmMenuItem, wUAlgorithmMenuItem });
            drawingSegmentsMenuItem.Name = "drawingSegmentsMenuItem";
            drawingSegmentsMenuItem.Size = new Size(262, 34);
            drawingSegmentsMenuItem.Text = "Drawing segments";
            // 
            // libraryAlgorithmMenuItem
            // 
            libraryAlgorithmMenuItem.Checked = true;
            libraryAlgorithmMenuItem.CheckState = CheckState.Checked;
            libraryAlgorithmMenuItem.Name = "libraryAlgorithmMenuItem";
            libraryAlgorithmMenuItem.Size = new Size(283, 34);
            libraryAlgorithmMenuItem.Text = "Library algorithm";
            libraryAlgorithmMenuItem.Click += libraryAlgorithmMenuItem_Click;
            // 
            // bresenhamAlgorithmMenuItem
            // 
            bresenhamAlgorithmMenuItem.Name = "bresenhamAlgorithmMenuItem";
            bresenhamAlgorithmMenuItem.Size = new Size(283, 34);
            bresenhamAlgorithmMenuItem.Text = "Bresenham algorithm";
            bresenhamAlgorithmMenuItem.Click += bresenhamAlgorithmMenuItem_Click;
            // 
            // wUAlgorithmMenuItem
            // 
            wUAlgorithmMenuItem.Name = "wUAlgorithmMenuItem";
            wUAlgorithmMenuItem.Size = new Size(283, 34);
            wUAlgorithmMenuItem.Text = "WU algorithm";
            wUAlgorithmMenuItem.Click += wUAlgorithmToolStripMenuItem_Click;
            // 
            // backgroundColorToolStripMenuItem
            // 
            backgroundColorToolStripMenuItem.Name = "backgroundColorToolStripMenuItem";
            backgroundColorToolStripMenuItem.Size = new Size(262, 34);
            backgroundColorToolStripMenuItem.Text = "Background color";
            backgroundColorToolStripMenuItem.Click += backgroundColorToolStripMenuItem_Click;
            // 
            // offsetToolStripMenuItem
            // 
            offsetToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem1, toolStripMenuItem2, toolStripMenuItem3 });
            offsetToolStripMenuItem.Name = "offsetToolStripMenuItem";
            offsetToolStripMenuItem.Size = new Size(262, 34);
            offsetToolStripMenuItem.Text = "Offset";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Checked = true;
            toolStripMenuItem1.CheckState = CheckState.Checked;
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(124, 34);
            toolStripMenuItem1.Text = "1";
            toolStripMenuItem1.Click += toolStripMenuItem1_Click;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(124, 34);
            toolStripMenuItem2.Text = "2";
            toolStripMenuItem2.Click += toolStripMenuItem2_Click;
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new Size(124, 34);
            toolStripMenuItem3.Text = "3";
            toolStripMenuItem3.Click += toolStripMenuItem3_Click;
            // 
            // workSpace
            // 
            workSpace.BackColor = Color.White;
            workSpace.Dock = DockStyle.Fill;
            workSpace.Location = new Point(0, 33);
            workSpace.Name = "workSpace";
            workSpace.Size = new Size(1504, 846);
            workSpace.TabIndex = 2;
            workSpace.TabStop = false;
            workSpace.Paint += workSpace_Paint;
            workSpace.MouseDoubleClick += workSpace_MouseDoubleClick;
            workSpace.MouseDown += workSpace_MouseDown;
            workSpace.MouseMove += workSpace_MouseMove;
            workSpace.MouseUp += workSpace_MouseUp;
            // 
            // contextMenuStrip
            // 
            contextMenuStrip.ImageScalingSize = new Size(24, 24);
            contextMenuStrip.Items.AddRange(new ToolStripItem[] { horizontalMenuItem, verticalMenuItem });
            contextMenuStrip.Name = "contextMenuStrip";
            contextMenuStrip.Size = new Size(167, 68);
            // 
            // horizontalMenuItem
            // 
            horizontalMenuItem.Name = "horizontalMenuItem";
            horizontalMenuItem.Size = new Size(166, 32);
            horizontalMenuItem.Text = "Horizontal";
            horizontalMenuItem.Click += horizontalMenuItem_Click;
            // 
            // verticalMenuItem
            // 
            verticalMenuItem.Name = "verticalMenuItem";
            verticalMenuItem.Size = new Size(166, 32);
            verticalMenuItem.Text = "Vertical";
            verticalMenuItem.Click += verticalMenuItem_Click;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            panel1.AutoSize = true;
            panel1.Controls.Add(colorLabel);
            panel1.Controls.Add(offsetLabel);
            panel1.Controls.Add(offsetTrackBar);
            panel1.Controls.Add(colorButton);
            panel1.Location = new Point(1155, 33);
            panel1.Name = "panel1";
            panel1.Size = new Size(349, 129);
            panel1.TabIndex = 3;
            // 
            // colorLabel
            // 
            colorLabel.AutoSize = true;
            colorLabel.Location = new Point(271, 12);
            colorLabel.Name = "colorLabel";
            colorLabel.Size = new Size(55, 25);
            colorLabel.TabIndex = 3;
            colorLabel.Text = "Color";
            // 
            // offsetLabel
            // 
            offsetLabel.AutoSize = true;
            offsetLabel.Location = new Point(106, 12);
            offsetLabel.Name = "offsetLabel";
            offsetLabel.Size = new Size(61, 25);
            offsetLabel.TabIndex = 2;
            offsetLabel.Text = "Offset";
            // 
            // offsetTrackBar
            // 
            offsetTrackBar.Location = new Point(3, 55);
            offsetTrackBar.Maximum = 100;
            offsetTrackBar.Name = "offsetTrackBar";
            offsetTrackBar.Size = new Size(246, 69);
            offsetTrackBar.TabIndex = 1;
            offsetTrackBar.Scroll += offsetTrackBar_Scroll;
            // 
            // colorButton
            // 
            colorButton.BackColor = Color.Black;
            colorButton.Location = new Point(255, 40);
            colorButton.Name = "colorButton";
            colorButton.Size = new Size(86, 86);
            colorButton.TabIndex = 0;
            colorButton.UseVisualStyleBackColor = false;
            colorButton.Click += colorButton_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1504, 901);
            Controls.Add(panel1);
            Controls.Add(workSpace);
            Controls.Add(statusStrip);
            Controls.Add(menuStrip);
            KeyPreview = true;
            MainMenuStrip = menuStrip;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PolygonEditor";
            WindowState = FormWindowState.Maximized;
            KeyDown += MainForm_KeyDown;
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)workSpace).EndInit();
            contextMenuStrip.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)offsetTrackBar).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private StatusStrip statusStrip;
        private MenuStrip menuStrip;
        private ToolStripMenuItem modeMenuItem;
        private ToolStripMenuItem drawingMenuItem;
        private ToolStripMenuItem editingMenuItem;
        private ToolStripMenuItem settingsMenuItem;
        private ToolStripMenuItem drawingSegmentsMenuItem;
        private ToolStripMenuItem libraryAlgorithmMenuItem;
        private ToolStripMenuItem bresenhamAlgorithmMenuItem;
        private PictureBox workSpace;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem horizontalMenuItem;
        private ToolStripMenuItem verticalMenuItem;
        private ColorDialog colorDialog;
        private Panel panel1;
        private Button colorButton;
        private ToolStripMenuItem backgroundColorToolStripMenuItem;
        private TrackBar offsetTrackBar;
        private Label colorLabel;
        private Label offsetLabel;
        private ToolStripMenuItem offsetToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem wUAlgorithmMenuItem;
        private ToolStripMenuItem fileMenuItem;
        private ToolStripMenuItem saveMenuItem;
        private ToolStripMenuItem loadMenuItem;
    }
}