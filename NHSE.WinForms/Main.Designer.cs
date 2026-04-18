namespace NHSE.WinForms
{
    public partial class Main
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
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            B_Open = new System.Windows.Forms.Button();
            B_OpenRecent = new System.Windows.Forms.Button();
            toolTipRecent = new System.Windows.Forms.ToolTip(components);
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            //
            // tableLayoutPanel1
            //
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(B_Open, 0, 0);
            tableLayoutPanel1.Controls.Add(B_OpenRecent, 0, 1);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            tableLayoutPanel1.Size = new System.Drawing.Size(306, 150);
            tableLayoutPanel1.TabIndex = 1;
            //
            // B_Open
            //
            B_Open.Dock = System.Windows.Forms.DockStyle.Fill;
            B_Open.Location = new System.Drawing.Point(3, 3);
            B_Open.Name = "B_Open";
            B_Open.Size = new System.Drawing.Size(300, 99);
            B_Open.TabIndex = 0;
            B_Open.Text = "Open main.dat\r\n\r\nOr...\r\n\r\nDrag && Drop folder here!";
            B_Open.UseVisualStyleBackColor = true;
            B_Open.Click += Menu_Open;
            //
            // B_OpenRecent
            //
            B_OpenRecent.Dock = System.Windows.Forms.DockStyle.Fill;
            B_OpenRecent.Location = new System.Drawing.Point(3, 108);
            B_OpenRecent.Name = "B_OpenRecent";
            B_OpenRecent.Size = new System.Drawing.Size(300, 39);
            B_OpenRecent.TabIndex = 1;
            B_OpenRecent.Text = "Open most recent folder";
            B_OpenRecent.UseVisualStyleBackColor = true;
            B_OpenRecent.Visible = false;
            B_OpenRecent.Click += Menu_OpenRecent;
            //
            // Main
            //
            AllowDrop = true;
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(306, 150);
            Controls.Add(tableLayoutPanel1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.icon;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Main";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "NHSE";
            DragDrop += Main_DragDrop;
            DragEnter += Main_DragEnter;
            KeyDown += Main_KeyDown;
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button B_Open;
        private System.Windows.Forms.Button B_OpenRecent;
        private System.Windows.Forms.ToolTip toolTipRecent;
    }
}
