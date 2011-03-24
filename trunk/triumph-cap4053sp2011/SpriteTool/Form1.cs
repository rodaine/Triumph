using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpriteTool
{
	public partial class Form1 : Form
	{

		string searchString = "*_bk1.gif";
		string[] endings = new string[] { "_bk1.gif", "_bk2.gif", "_fr1.gif", "_fr2.gif", "_lf1.gif", "_lf2.gif", "_rt1.gif", "_rt2.gif" };

		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				textBox1.Text = folderBrowserDialog1.SelectedPath;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(textBox1.Text))
			{
				MessageBox.Show("Please choose a file path first!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			string[] files = Directory.GetFiles(textBox1.Text, searchString);

			foreach (string file in files)
			{
				List<Image> frames = new List<Image>();
				string rootName = file.Remove(file.Length - "_bk1.gif".Length);
				
				foreach(string ending in endings)
				{
					FileStream stream = new FileStream(rootName + ending, FileMode.Open);
					frames.Add(Image.FromStream(stream));
					stream.Dispose();
				}
				
				Bitmap outputFile = new Bitmap(32 * 8, 32);
				foreach(Bitmap b in frames)
				{
					for (int x = 0; x < b.Width; ++x)
					{
						for (int y = 0; y < b.Height; ++y)
						{
							Color color = b.GetPixel(x, y);
							outputFile.SetPixel(x + (32 * frames.IndexOf(b)), y, color);
						}
					}
				}

				outputFile.Save(rootName + ".png", ImageFormat.Png);

			}

			MessageBox.Show("Done!");
			
		}


	}
}
