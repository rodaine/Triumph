using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileEngine;

namespace TileEditor
{

	using Image = System.Drawing.Image;
	
	public partial class Form1 : Form
	{
		
		#region Properties
		
			Dictionary<string, TileLayer> tileLayerDict = new Dictionary<string, TileLayer>();
			Dictionary<string, Texture2D> textureDict = new Dictionary<string, Texture2D>();
			Dictionary<string, Image> previewDict = new Dictionary<string, Image>();
			string[] imageExts = new string[]
				{
					".jpg",
					".png",
					".bmp",
					".tga"
				};
			int maxWidth = 0, maxHeight = 0;
		
		#endregion

		public Form1()
		{
			InitializeComponent();
			openFileDialog1.Filter = "Layer File|*.layer";
			saveFileDialog1.Filter = "Layer File|*.layer";
			Mouse.WindowHandle = tileDisplayPane1.Handle;

			while (string.IsNullOrEmpty(tbxContentPath.Text))
			{
				if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
				{
					tbxContentPath.Text = folderBrowserDialog1.SelectedPath;
					this.Focus();
				}
				else
				{
					MessageBox.Show("Please choose a content directory");
				}
			}
		}

		#region File Menu Events
	
			private void openToolStripMenuItem_Click(object sender, EventArgs e)
			{
				openFileDialog2.InitialDirectory = tbxContentPath.Text;
				if (openFileDialog1.ShowDialog() == DialogResult.OK)
				{
					string filename = openFileDialog1.FileName;
					string[] textureNames;
					TileLayer layer = TileLayer.fromFile(filename, out textureNames);

					tileLayerDict.Add(Path.GetFileName(filename), layer);
					tileDisplayPane1.map.layers.Add(layer);
					lbxLayers.Items.Add(Path.GetFileName(filename));

					foreach (string textureName in textureNames)
					{

						string fullPath = tbxContentPath.Text + "/" + textureName;

						foreach (string ext in imageExts)
						{
							if (File.Exists(fullPath + ext))
							{
								fullPath += ext;
								break;
							}
						}

						FileStream stream = new FileStream(fullPath, FileMode.Open);

						Texture2D texture = Texture2D.FromStream(tileDisplayPane1.GraphicsDevice, stream);
						layer.addTexture(texture);

						if (!textureDict.ContainsKey(textureName))
							textureDict.Add(textureName, texture);
				
						Image image = Image.FromStream(stream);
						if (!previewDict.ContainsKey(textureName))
							previewDict.Add(textureName, image);

						stream.Dispose();

						if (!lbxTextures.Items.Contains(textureName))
							lbxTextures.Items.Add(textureName);
					}
					resetScrollbars();
				}
			}

			private void saveToolStripMenuItem_Click(object sender, EventArgs e)
			{
				if (lbxLayers.SelectedItem != null)
				{
					string filename = lbxLayers.SelectedItem as string;
					TileLayer layer = tileLayerDict[filename];
					Dictionary<int, string> textures = new Dictionary<int, string>();
				
					foreach (string textureName in lbxTextures.Items)
					{
						int index = layer.textureIndex(textureDict[textureName]);
						if (index != -1)
						{
							textures.Add(index, textureName);
						}
					}

					List<string> textureList = new List<string>();
					for (int i = 0; i < textures.Count; ++i)
					{
						//try { textureList.Add(textures[i]); }
						//catch (Exception ex) { }
						textureList.Add(textures[i]);
					}

					if (saveFileDialog1.ShowDialog() == DialogResult.OK)
					{
						layer.save(saveFileDialog1.FileName, textureList.ToArray());
					}

				}
			}

			private void exitToolStripMenuItem_Click(object sender, EventArgs e)
			{
				Close();
			}

			private void newTileMapToolStripMenuItem_Click(object sender, EventArgs e)
			{

			}

		#endregion

		#region Layer Related Events

			private void lbxLayers_SelectedIndexChanged(object sender, EventArgs e)
			{
				if (lbxLayers.SelectedItem != null)
				{
					tileDisplayPane1.currentLayer = tileLayerDict[lbxLayers.SelectedItem as string];
					trkAlpha.Value = (int)(tileDisplayPane1.currentLayer.alpha * 100);
				}
				
			}

			private void btnAddLayer_Click(object sender, EventArgs e)
			{
				Form2 frm2 = new Form2();
				if (frm2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					tileLayerDict.Add(frm2.filename, frm2.layer);
					tileDisplayPane1.map.layers.Add(frm2.layer);
					lbxLayers.Items.Add(frm2.filename);
					resetScrollbars();
				}
				frm2.Dispose();
			}

			private void btnRemoveLayer_Click(object sender, EventArgs e)
			{
				if (lbxLayers.SelectedItem != null)
				{
					tileDisplayPane1.map.layers.Remove(tileLayerDict[lbxLayers.SelectedItem as string]);
					tileLayerDict.Remove(lbxLayers.SelectedItem as string);
					lbxLayers.Items.RemoveAt(lbxLayers.SelectedIndex);
					tileDisplayPane1.currentLayer = null;
					resetScrollbars();
				}
			}

			private void trkAlpha_Scroll(object sender, EventArgs e)
			{
				if (tileDisplayPane1.currentLayer != null)
				{
					tileDisplayPane1.currentLayer.alpha = (float)trkAlpha.Value / 100f;
				}
			}

		#endregion

		#region Texture Related Events
				
			private void lbxTextures_SelectedIndexChanged(object sender, EventArgs e)
			{
				if (lbxTextures.SelectedItem != null)
				{
					pbxTexture.Image = previewDict[lbxTextures.SelectedItem as string];
					tileDisplayPane1.currentTexture = textureDict[lbxTextures.SelectedItem as string];
				}
			}

			private void btnAddTexture_Click(object sender, EventArgs e)
				{
					openFileDialog2.Filter = "JPG Image|*.jpg|PNG Image|*.png|BMP Image|*.bmp|TGA Image|*.tga";
					openFileDialog2.InitialDirectory = tbxContentPath.Text;

					if (openFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						string filename = openFileDialog2.FileName;

						FileStream stream = new FileStream(filename, FileMode.Open);
						Texture2D texture = Texture2D.FromStream(tileDisplayPane1.GraphicsDevice, stream);
						Image image = Image.FromStream(stream);
						stream.Dispose();

						filename = filename.Replace(tbxContentPath.Text + "\\", "");
						filename = filename.Replace("\\", "/");
						filename = filename.Remove(filename.LastIndexOf('.'));

						if (!lbxTextures.Items.Contains(filename))
						{
							lbxTextures.Items.Add(filename);
							textureDict.Add(filename, texture);
							previewDict.Add(filename, image);
						}

					}
				}

			private void btnRemoveTexture_Click(object sender, EventArgs e)
			{
				if (lbxTextures.SelectedItem != null)
				{
					string textureName = lbxTextures.SelectedItem as string;
					foreach (TileLayer layer in tileDisplayPane1.map.layers)
						if (layer.textureIndex(textureDict[textureName]) != -1)
							layer.removeTexture(textureDict[textureName]);
					textureDict.Remove(textureName);
					previewDict.Remove(textureName);
					lbxTextures.Items.Remove(lbxTextures.SelectedItem);
					pbxTexture.Image = null;
				}
			}

		#endregion

		#region Scrollbar Events

			private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
			{
				tileDisplayPane1.camera.position.Y = vScrollBar1.Value * Engine.TILE_HEIGHT;
			}

			private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
			{
				tileDisplayPane1.camera.position.X = hScrollBar1.Value * Engine.TILE_HEIGHT;
			}

			private void resetScrollbars()
			{
				if (tileDisplayPane1.map.getWidthInPixels() > tileDisplayPane1.Width)
				{
					maxWidth = (int)Math.Max(tileDisplayPane1.map.getWidthInTiles(), maxWidth);
					hScrollBar1.Visible = true;
					hScrollBar1.Minimum = 0;
					hScrollBar1.Maximum = maxWidth - 1;
				}
				else
				{
					maxWidth = 0;
					hScrollBar1.Value = 0;
					hScrollBar1_Scroll(null, new ScrollEventArgs(ScrollEventType.SmallDecrement,0));
					hScrollBar1.Visible = false;
				}
				if (tileDisplayPane1.map.getHeightInPixels() > tileDisplayPane1.Height)
				{
					maxHeight = (int)Math.Max(tileDisplayPane1.map.getHeightInTiles(), maxHeight);
					vScrollBar1.Visible = true;
					vScrollBar1.Minimum = 0;
					vScrollBar1.Maximum = maxHeight - 1;
				}
				else
				{
					maxHeight = 0;
					vScrollBar1.Value = 0;
					vScrollBar1_Scroll(null, new ScrollEventArgs(ScrollEventType.SmallDecrement, 0));
					vScrollBar1.Visible = false;
				}
			}

		#endregion

		#region Tile Display Events
			
			private void tileDisplayPane1_MouseEnter(object sender, EventArgs e)
			{
				System.Windows.Forms.Cursor.Hide();
			}

			private void tileDisplayPane1_MouseLeave(object sender, EventArgs e)
			{
				System.Windows.Forms.Cursor.Show();
			}

			private void tileDisplayPane1_Resize(object sender, EventArgs e)
			{

				resetScrollbars();
			}

		#endregion

		#region Drawing Events

			private void toggleDrawErase(object sender, EventArgs e)
			{
				if (rdbDraw.Checked)
					tileDisplayPane1.isErase = false;
				else
					tileDisplayPane1.isErase = true;
			}

			private void cbxFill_CheckedChanged(object sender, EventArgs e)
			{
				if (cbxFill.Checked)
					tileDisplayPane1.isFill = true;
				else
					tileDisplayPane1.isFill = false;
			}
		
		#endregion

	}
}
