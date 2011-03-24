using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;
using TileEngine;
using Microsoft.Xna.Framework.Input;

namespace TileEditor
{
	public class TileDisplayPane : GraphicsDeviceControl
	{

		public TileMap map = new TileMap();
		public Camera camera = new Camera();
		Texture2D emptyTexture;
		SpriteBatch batch;
		public TileLayer currentLayer;
		public Texture2D currentTexture;
		public bool isErase = false, isFill = false;
		int cellX, cellY;

		protected override void Initialize()
		{
			currentLayer = null;
			batch = new SpriteBatch(GraphicsDevice);
			emptyTexture = Texture2D.FromStream(GraphicsDevice, new FileStream("Content/cursor.png", FileMode.Open));

			Application.Idle += delegate { Invalidate(); };
		}

		protected override void Draw()
		{
			logic();
			render();
		}

		private void logic()
		{
			int mouseX = Mouse.GetState().X;
			int mouseY = Mouse.GetState().Y;
			if (currentLayer != null)
			{
				if (mouseX >= 0
					&& mouseX < Math.Min(Width, currentLayer.widthInPixels - camera.position.X)
					&& mouseY >= 0
					&& mouseY < Math.Min(Height, currentLayer.heightInPixels - camera.position.Y))
				{
					cellX = (int)MathHelper.Clamp((mouseX + camera.position.X) / Engine.TILE_WIDTH, 0, currentLayer.widthInTiles - 1);
					cellY = (int)MathHelper.Clamp((mouseY + camera.position.Y) / Engine.TILE_HEIGHT, 0, currentLayer.heightInTiles - 1);
				}
				else
				{
					cellX = cellY = -1;
				}

				if (Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && cellX != -1 && cellY != -1)
				{
					if (isErase)
					{
						eraseCell();
					}
					else
					{
						drawCell();
					}
				}

			}


		}

		private void render()
		{
			GraphicsDevice.Clear(Color.Black);

			foreach (TileLayer layer in map.layers)
			{
				layer.draw(batch, camera);

				if (layer == currentLayer)
				{
					batch.Begin();
					for (int y = 0; y < layer.heightInTiles; ++y)
					{
						for (int x = 0; x < layer.widthInTiles; ++x)
						{
							if (layer.getTileTextureIndex(x, y) == -1)
							{
								batch.Draw(
									emptyTexture,
									new Rectangle(
										x * Engine.TILE_WIDTH - (int)camera.position.X,
										y * Engine.TILE_HEIGHT - (int)camera.position.Y,
										Engine.TILE_WIDTH,
										Engine.TILE_HEIGHT),
									Color.White);
							}
						}
					}
					batch.End();
				}
			}

			if (currentLayer != null)
			{
				if (cellX != -1 && cellY != -1)
				{

					batch.Begin();
						batch.Draw(
							emptyTexture,
							new Rectangle(
									cellX * Engine.TILE_WIDTH - (int)camera.position.X,
									cellY * Engine.TILE_HEIGHT - (int)camera.position.Y,
									Engine.TILE_WIDTH,
									Engine.TILE_HEIGHT),
							Color.Red);
					batch.End();
				}

			}
			
		}

		private void drawCell()
		{
			if (currentLayer != null && currentTexture != null)
			{
				if (currentLayer.textureIndex(currentTexture) == -1)
					currentLayer.addTexture(currentTexture);

				if (isFill)
				{
					Stack<int[]> openStack = new Stack<int[]>();
					List<int[]> closeList = new List<int[]>();
					openStack.Push(new int[] { cellX, cellY });
					int oldIndex = currentLayer.getTileTextureIndex(cellX, cellY);
					while (openStack.Count > 0)
					{
						bool isDone = false;
						int[] cell = openStack.Pop();
						foreach (int[] c in closeList)
						{
							if (c[0] == cell[0] && c[1] == cell[1])
								isDone = true;
						}

						if (isDone)
							continue;

						currentLayer.setTileTexureIndex(cell[0], cell[1], currentLayer.textureIndex(currentTexture));

						if (currentLayer.getTileTextureIndex(cell[0] + 1, cell[1] + 0) == oldIndex)
							openStack.Push(new int[] { cell[0] + 1, cell[1] + 0 });

						if (currentLayer.getTileTextureIndex(cell[0] - 1, cell[1] + 0) == oldIndex)
							openStack.Push(new int[] { cell[0] - 1, cell[1] + 0 });

						if (currentLayer.getTileTextureIndex(cell[0] + 0, cell[1] + 1) == oldIndex)
							openStack.Push(new int[] { cell[0] + 0, cell[1] + 1 });

						if (currentLayer.getTileTextureIndex(cell[0] + 0, cell[1] - 1) == oldIndex)
							openStack.Push(new int[] { cell[0] + 0, cell[1] - 1 });

						closeList.Add(cell);
					}
				}
				else
				{
					currentLayer.setTileTexureIndex(cellX, cellY, currentLayer.textureIndex(currentTexture));
				}
			}
		}

		private void eraseCell()
		{
			if (currentLayer != null)
			{
				if (isFill)
				{
					Stack<int[]> openStack = new Stack<int[]>();
					List<int[]> closeList = new List<int[]>();
					openStack.Push(new int[] { cellX, cellY });
					int oldIndex = currentLayer.getTileTextureIndex(cellX, cellY);
					while (openStack.Count > 0)
					{
						bool isDone = false;
						int[] cell = openStack.Pop();
						foreach (int[] c in closeList)
						{
							if (c[0] == cell[0] && c[1] == cell[1])
								isDone = true;
						}

						if (isDone)
							continue;

						currentLayer.setTileTexureIndex(cell[0], cell[1], -1);

						if (currentLayer.getTileTextureIndex(cell[0] + 1, cell[1] + 0) == oldIndex)
							openStack.Push(new int[] { cell[0] + 1, cell[1] + 0 });

						if (currentLayer.getTileTextureIndex(cell[0] - 1, cell[1] + 0) == oldIndex)
							openStack.Push(new int[] { cell[0] - 1, cell[1] + 0 });

						if (currentLayer.getTileTextureIndex(cell[0] + 0, cell[1] + 1) == oldIndex)
							openStack.Push(new int[] { cell[0] + 0, cell[1] + 1 });

						if (currentLayer.getTileTextureIndex(cell[0] + 0, cell[1] - 1) == oldIndex)
							openStack.Push(new int[] { cell[0] + 0, cell[1] - 1 });

						closeList.Add(cell);
					}
				}
				else
				{
					currentLayer.setTileTexureIndex(cellX, cellY, -1);
				}
			}
		}
	}
}
