using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TileEngine;

namespace TileEditor
{
	public partial class Form2 : Form
	{

		public TileLayer layer;
		public string filename;

		public Form2()
		{
			InitializeComponent();
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			layer = new TileLayer((int) nudWidth.Value, (int)nudHeight.Value);
			filename = tbxLayerName.Text.Trim() + ".layer";
			DialogResult = System.Windows.Forms.DialogResult.OK;
			this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Close();
		}


	}
}
