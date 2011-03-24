namespace TileEditor
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
			this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newTileMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.tbxContentPath = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.rdbDraw = new System.Windows.Forms.RadioButton();
			this.rdbErase = new System.Windows.Forms.RadioButton();
			this.lbxLayers = new System.Windows.Forms.ListBox();
			this.btnAddLayer = new System.Windows.Forms.Button();
			this.btnRemoveLayer = new System.Windows.Forms.Button();
			this.lbxTextures = new System.Windows.Forms.ListBox();
			this.btnAddTexture = new System.Windows.Forms.Button();
			this.btnRemoveTexture = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.pbxTexture = new System.Windows.Forms.PictureBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
			this.cbxFill = new System.Windows.Forms.CheckBox();
			this.tileDisplayPane1 = new TileEditor.TileDisplayPane();
			this.trkAlpha = new System.Windows.Forms.TrackBar();
			this.label6 = new System.Windows.Forms.Label();
			this.menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pbxTexture)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trkAlpha)).BeginInit();
			this.SuspendLayout();
			// 
			// vScrollBar1
			// 
			this.vScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.vScrollBar1.Location = new System.Drawing.Point(652, 27);
			this.vScrollBar1.Name = "vScrollBar1";
			this.vScrollBar1.Size = new System.Drawing.Size(24, 664);
			this.vScrollBar1.TabIndex = 1;
			this.vScrollBar1.Visible = false;
			this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
			// 
			// hScrollBar1
			// 
			this.hScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.hScrollBar1.Location = new System.Drawing.Point(12, 667);
			this.hScrollBar1.Name = "hScrollBar1";
			this.hScrollBar1.Size = new System.Drawing.Size(640, 24);
			this.hScrollBar1.TabIndex = 2;
			this.hScrollBar1.Visible = false;
			this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar1_Scroll);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(954, 24);
			this.menuStrip1.TabIndex = 3;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newTileMapToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// newTileMapToolStripMenuItem
			// 
			this.newTileMapToolStripMenuItem.Name = "newTileMapToolStripMenuItem";
			this.newTileMapToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.newTileMapToolStripMenuItem.Text = "New Tile Map";
			this.newTileMapToolStripMenuItem.Click += new System.EventHandler(this.newTileMapToolStripMenuItem_Click);
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.openToolStripMenuItem.Text = "Open";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.saveToolStripMenuItem.Text = "Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// tbxContentPath
			// 
			this.tbxContentPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbxContentPath.Location = new System.Drawing.Point(679, 44);
			this.tbxContentPath.Name = "tbxContentPath";
			this.tbxContentPath.ReadOnly = true;
			this.tbxContentPath.Size = new System.Drawing.Size(263, 20);
			this.tbxContentPath.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(679, 28);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(113, 16);
			this.label1.TabIndex = 5;
			this.label1.Text = "Content Source";
			// 
			// rdbDraw
			// 
			this.rdbDraw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.rdbDraw.AutoSize = true;
			this.rdbDraw.Checked = true;
			this.rdbDraw.Location = new System.Drawing.Point(763, 684);
			this.rdbDraw.Name = "rdbDraw";
			this.rdbDraw.Size = new System.Drawing.Size(50, 17);
			this.rdbDraw.TabIndex = 7;
			this.rdbDraw.TabStop = true;
			this.rdbDraw.Text = "Draw";
			this.rdbDraw.UseVisualStyleBackColor = true;
			this.rdbDraw.CheckedChanged += new System.EventHandler(this.toggleDrawErase);
			// 
			// rdbErase
			// 
			this.rdbErase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.rdbErase.AutoSize = true;
			this.rdbErase.Location = new System.Drawing.Point(819, 684);
			this.rdbErase.Name = "rdbErase";
			this.rdbErase.Size = new System.Drawing.Size(52, 17);
			this.rdbErase.TabIndex = 7;
			this.rdbErase.Text = "Erase";
			this.rdbErase.UseVisualStyleBackColor = true;
			this.rdbErase.CheckedChanged += new System.EventHandler(this.toggleDrawErase);
			// 
			// lbxLayers
			// 
			this.lbxLayers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lbxLayers.FormattingEnabled = true;
			this.lbxLayers.Location = new System.Drawing.Point(678, 86);
			this.lbxLayers.Name = "lbxLayers";
			this.lbxLayers.Size = new System.Drawing.Size(263, 82);
			this.lbxLayers.TabIndex = 8;
			this.lbxLayers.SelectedIndexChanged += new System.EventHandler(this.lbxLayers_SelectedIndexChanged);
			// 
			// btnAddLayer
			// 
			this.btnAddLayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAddLayer.Location = new System.Drawing.Point(679, 249);
			this.btnAddLayer.Name = "btnAddLayer";
			this.btnAddLayer.Size = new System.Drawing.Size(135, 23);
			this.btnAddLayer.TabIndex = 9;
			this.btnAddLayer.Text = "Add Layer";
			this.btnAddLayer.UseVisualStyleBackColor = true;
			this.btnAddLayer.Click += new System.EventHandler(this.btnAddLayer_Click);
			// 
			// btnRemoveLayer
			// 
			this.btnRemoveLayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRemoveLayer.Location = new System.Drawing.Point(820, 249);
			this.btnRemoveLayer.Name = "btnRemoveLayer";
			this.btnRemoveLayer.Size = new System.Drawing.Size(122, 23);
			this.btnRemoveLayer.TabIndex = 9;
			this.btnRemoveLayer.Text = "Remove Layer";
			this.btnRemoveLayer.UseVisualStyleBackColor = true;
			this.btnRemoveLayer.Click += new System.EventHandler(this.btnRemoveLayer_Click);
			// 
			// lbxTextures
			// 
			this.lbxTextures.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lbxTextures.FormattingEnabled = true;
			this.lbxTextures.Location = new System.Drawing.Point(679, 293);
			this.lbxTextures.Name = "lbxTextures";
			this.lbxTextures.Size = new System.Drawing.Size(263, 69);
			this.lbxTextures.TabIndex = 8;
			this.lbxTextures.SelectedIndexChanged += new System.EventHandler(this.lbxTextures_SelectedIndexChanged);
			// 
			// btnAddTexture
			// 
			this.btnAddTexture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAddTexture.Location = new System.Drawing.Point(679, 368);
			this.btnAddTexture.Name = "btnAddTexture";
			this.btnAddTexture.Size = new System.Drawing.Size(135, 23);
			this.btnAddTexture.TabIndex = 9;
			this.btnAddTexture.Text = "Add Texture";
			this.btnAddTexture.UseVisualStyleBackColor = true;
			this.btnAddTexture.Click += new System.EventHandler(this.btnAddTexture_Click);
			// 
			// btnRemoveTexture
			// 
			this.btnRemoveTexture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRemoveTexture.Location = new System.Drawing.Point(820, 368);
			this.btnRemoveTexture.Name = "btnRemoveTexture";
			this.btnRemoveTexture.Size = new System.Drawing.Size(122, 23);
			this.btnRemoveTexture.TabIndex = 9;
			this.btnRemoveTexture.Text = "Remove Texture";
			this.btnRemoveTexture.UseVisualStyleBackColor = true;
			this.btnRemoveTexture.Click += new System.EventHandler(this.btnRemoveTexture_Click);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(679, 67);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(55, 16);
			this.label2.TabIndex = 5;
			this.label2.Text = "Layers";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(676, 274);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(68, 16);
			this.label3.TabIndex = 5;
			this.label3.Text = "Textures";
			// 
			// pbxTexture
			// 
			this.pbxTexture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pbxTexture.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pbxTexture.Location = new System.Drawing.Point(682, 417);
			this.pbxTexture.Name = "pbxTexture";
			this.pbxTexture.Size = new System.Drawing.Size(260, 260);
			this.pbxTexture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pbxTexture.TabIndex = 10;
			this.pbxTexture.TabStop = false;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(679, 684);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(78, 16);
			this.label4.TabIndex = 5;
			this.label4.Text = "Edit Mode";
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(679, 398);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(126, 16);
			this.label5.TabIndex = 5;
			this.label5.Text = "Selected Texture";
			// 
			// openFileDialog2
			// 
			this.openFileDialog2.FileName = "openFileDialog2";
			// 
			// cbxFill
			// 
			this.cbxFill.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbxFill.AutoSize = true;
			this.cbxFill.Location = new System.Drawing.Point(877, 685);
			this.cbxFill.Name = "cbxFill";
			this.cbxFill.Size = new System.Drawing.Size(38, 17);
			this.cbxFill.TabIndex = 11;
			this.cbxFill.Text = "Fill";
			this.cbxFill.UseVisualStyleBackColor = true;
			this.cbxFill.CheckedChanged += new System.EventHandler(this.cbxFill_CheckedChanged);
			// 
			// tileDisplayPane1
			// 
			this.tileDisplayPane1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tileDisplayPane1.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.tileDisplayPane1.Location = new System.Drawing.Point(12, 27);
			this.tileDisplayPane1.Name = "tileDisplayPane1";
			this.tileDisplayPane1.Size = new System.Drawing.Size(640, 640);
			this.tileDisplayPane1.TabIndex = 0;
			this.tileDisplayPane1.Text = "tileDisplayPane1";
			this.tileDisplayPane1.MouseEnter += new System.EventHandler(this.tileDisplayPane1_MouseEnter);
			this.tileDisplayPane1.MouseLeave += new System.EventHandler(this.tileDisplayPane1_MouseLeave);
			this.tileDisplayPane1.Resize += new System.EventHandler(this.tileDisplayPane1_Resize);
			// 
			// trkAlpha
			// 
			this.trkAlpha.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.trkAlpha.Location = new System.Drawing.Point(679, 198);
			this.trkAlpha.Maximum = 100;
			this.trkAlpha.Name = "trkAlpha";
			this.trkAlpha.Size = new System.Drawing.Size(263, 45);
			this.trkAlpha.TabIndex = 12;
			this.trkAlpha.TickFrequency = 5;
			this.trkAlpha.Value = 100;
			this.trkAlpha.Scroll += new System.EventHandler(this.trkAlpha_Scroll);
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(679, 179);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(91, 16);
			this.label6.TabIndex = 13;
			this.label6.Text = "Layer Alpha";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(954, 703);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.trkAlpha);
			this.Controls.Add(this.cbxFill);
			this.Controls.Add(this.pbxTexture);
			this.Controls.Add(this.btnRemoveTexture);
			this.Controls.Add(this.btnRemoveLayer);
			this.Controls.Add(this.btnAddTexture);
			this.Controls.Add(this.btnAddLayer);
			this.Controls.Add(this.lbxTextures);
			this.Controls.Add(this.lbxLayers);
			this.Controls.Add(this.rdbErase);
			this.Controls.Add(this.rdbDraw);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.tbxContentPath);
			this.Controls.Add(this.hScrollBar1);
			this.Controls.Add(this.vScrollBar1);
			this.Controls.Add(this.tileDisplayPane1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "Form1";
			this.Text = "Tile Layer Editor";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pbxTexture)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trkAlpha)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private TileDisplayPane tileDisplayPane1;
		private System.Windows.Forms.VScrollBar vScrollBar1;
		private System.Windows.Forms.HScrollBar hScrollBar1;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newTileMapToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.TextBox tbxContentPath;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.RadioButton rdbDraw;
		private System.Windows.Forms.RadioButton rdbErase;
		private System.Windows.Forms.ListBox lbxLayers;
		private System.Windows.Forms.Button btnAddLayer;
		private System.Windows.Forms.Button btnRemoveLayer;
		private System.Windows.Forms.ListBox lbxTextures;
		private System.Windows.Forms.Button btnAddTexture;
		private System.Windows.Forms.Button btnRemoveTexture;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.PictureBox pbxTexture;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.OpenFileDialog openFileDialog2;
		private System.Windows.Forms.CheckBox cbxFill;
		private System.Windows.Forms.TrackBar trkAlpha;
		private System.Windows.Forms.Label label6;
	}
}