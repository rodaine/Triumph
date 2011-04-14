using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TileEngine
{
	/// <summary>
	/// Describes a layer of tile textures for rendering
	/// </summary>
	public class TileLayer
	{
		#region Private Properties

		private List<Texture2D> tileTextures = new List<Texture2D>();
        private int[,] layout;
		private float _alpha = 1f;

		#endregion

		#region Public Properties
	
			/// <summary>
			/// Returns the width in tiles of the Tile Layer
			/// </summary>
			public int widthInTiles
			{
				get { return layout.GetLength(1); }
			}
		
			/// <summary>
			/// Returns the height in tiles of the Tile Layer
			/// </summary>
			public int heightInTiles
			{
				get { return layout.GetLength(0); }
			}
	
			/// <summary>
			/// Returns the width in pixels of the Tile Layer
			/// </summary>
			/// <remarks>Relies on Engine.TILE_WIDTH constant</remarks>
			public int widthInPixels
			{
				get { return Engine.TILE_WIDTH * widthInTiles; }
			}
		
			/// <summary>
			/// Returns the height in pixels of the Tile Layer
			/// </summary>
			/// <remarks>Relies on Engine.TILE_HEIGHT constant</remarks>
			public int heightInPixels
			{
				get { return Engine.TILE_HEIGHT * heightInTiles; }
			}

			/// <summary>
			/// Gets and sets the alpha (opacity) of the layer
			/// </summary>
			public float alpha
			{
				get { return _alpha; }
				set { _alpha = MathHelper.Clamp(value, 0f, 1f); }
			}
		
		#endregion

		#region Initializers & IO

			/// <summary>
			/// Creates an empty Tile Layer with specified dimensions
			/// </summary>
			/// <param name="width">Integer width (x-direction) of layer</param>
			/// <param name="height">Integer height (y-direction) of layer</param>
			public TileLayer(int width, int height)
			{
				layout = new int[height, width];
				for (int y = 0; y < height; ++y)
				{
					for (int x = 0; x < width; ++x)
					{
						layout[y, x] = -1;
					}
				}
			}

			/// <summary>
			/// Creates a Tile Layer from file using the XNA Content Pipeline
			/// </summary>
			/// <param name="content">Content manager reference</param>
			/// <param name="filename">Path (relative or absolute) to the .layer file</param>
			/// <returns>New Tile Layer</returns>
			public static TileLayer fromFile(ContentManager content, string filename)
			{
				TileLayer layer;

				List<string> textureNames = new List<string>();

				layer = processLayerFile(filename, textureNames);

				layer.loadTileTextures(content, textureNames.ToArray());

				return layer;
			}

			/// <summary>
			/// Creates a Tile Layer from file without using the XNA Content Pipeline [used by Tile Editor]
			/// </summary>
			/// <param name="filename">Path (relative or absolute) to the .layer file</param>
			/// <param name="textureNameArray">Output of string texture names referenced by the layer</param>
			/// <returns>New Tile Layer</returns>
			/// <remarks>Textures must be added after initialization to the layer before calling draw()</remarks>
			public static TileLayer fromFile(string filename, out string[] textureNameArray)
			{
				TileLayer layer;
				List<string> textureNames = new List<string>();

				layer = processLayerFile(filename, textureNames);

				textureNameArray = textureNames.ToArray();

				return layer;
			}

			/// <summary>
			/// Performs the IO on a .layer file to obtain a saved Tile Layer used by both fromFile() methods
			/// </summary>
			/// <param name="filename">Path (relative or absolute) to the .layer file</param>
			/// <param name="textureNames">List of string texture names referenced by the layer</param>
			/// <returns>New Tile Layer</returns>
			private static TileLayer processLayerFile(string filename, List<string> textureNames)
			{
				TileLayer layer;
				List<List<int>> tempLayout = new List<List<int>>();
				Dictionary<string, string> propertyDict = new Dictionary<string, string>();
				using (StreamReader reader = new StreamReader(filename))
				{
					bool readingTextures = false;
					bool readingProperties = false;
					bool readingLayout = false;
					while (!reader.EndOfStream)
					{
						string line = reader.ReadLine().Trim();

						if (string.IsNullOrEmpty(line))
							continue;

						if (line.Contains("[Textures]"))
						{
							readingTextures = true;
							readingProperties = false;
							readingLayout = false;
						}
						else if (line.Contains("[Properties]"))
						{
							readingTextures = false;
							readingProperties = true;
							readingLayout = false;
						}
						else if (line.Contains("[Layout]"))
						{
							readingTextures = false;
							readingProperties = false;
							readingLayout = true;
						}
						else if (readingTextures)
						{
							textureNames.Add(line);
						}
						else if (readingProperties)
						{
							string[] pair = line.Split('=');
							propertyDict.Add(pair[0].Trim(), pair[1].Trim());
						}
						else if (readingLayout)
						{
							List<int> row = new List<int>();
							string[] cells = line.Split(' ');
							foreach (string c in cells)
							{
								if (!string.IsNullOrEmpty(c))
									row.Add(int.Parse(c));
							}
							tempLayout.Add(row);
						}
					}
				}

				int width = tempLayout[0].Count;
				int height = tempLayout.Count;
				layer = new TileLayer(width, height);

				foreach (KeyValuePair<string, string> property in propertyDict)
				{
					switch (property.Key)
					{
						case "Alpha":
							layer.alpha = float.Parse(property.Value);
							break;
					}
				}

				for (int y = 0; y < height; ++y)
					for (int x = 0; x < width; ++x)
						layer.setTileTexureIndex(x, y, tempLayout[y][x]);

				return layer;
			}

			/// <summary>
			/// Saves a Tile Layer to file [used by Tile Editor]
			/// </summary>
			/// <param name="filename">Path (relative or absolute) where the .layer file should be saved</param>
			/// <param name="textureNames">String array of texture names (relative to a content root) used by the layer</param>
			public void save(string filename, string[] textureNames)
			{
				using (StreamWriter writer = new StreamWriter(filename))
				{
					writer.WriteLine("[Textures]");
					foreach (string str in textureNames)
						writer.WriteLine(str);
					writer.WriteLine();

					writer.WriteLine("[Properties]");
					writer.WriteLine("Alpha = " + _alpha.ToString());
					writer.WriteLine();

					writer.WriteLine("[Layout]");
					for (int y = 0; y < heightInTiles; ++y)
					{
						string line = string.Empty;

						for (int x = 0; x < widthInTiles; ++x)
						{
							line += layout[y, x].ToString() + " ";
						}

						writer.WriteLine(line);
					}

					writer.WriteLine();
				}
			}

		#endregion

		#region Texture Methods

			/// <summary>
			/// Loads tile textures from file using the XNA Content Pipeline
			/// </summary>
			/// <param name="content">Content manager reference</param>
			/// <param name="textureNames">String array of relative paths to texture asset names</param>
			public void loadTileTextures(ContentManager content, params string[] textureNames)
			{
				Texture2D texture;

				foreach (string textureName in textureNames)
				{
					texture = content.Load<Texture2D>(textureName);
					tileTextures.Add(texture);
				}

			}

			/// <summary>
			/// Add a tile texture to the layer
			/// </summary>
			/// <param name="texture">Texture2D of tile</param>
			public void addTexture(Texture2D texture)
			{
				tileTextures.Add(texture);
			}

			/// <summary>
			/// Remove texture from layer [used by Tile Editor]
			/// </summary>
			/// <param name="texture">Texture2D of tile</param>
			public void removeTexture(Texture2D texture)
			{
				removeTextureIndex(tileTextures.IndexOf(texture));
				tileTextures.Remove(texture);
			}

			/// <summary>
			/// Get the index of a texture in tileTextures
			/// </summary>
			/// <param name="texture">Texture2D of tile</param>
			/// <returns>0-Based index of texture if it exists in the layer; else -1.</returns>
			public int textureIndex(Texture2D texture)
			{
				if (tileTextures.Contains(texture))
					return tileTextures.IndexOf(texture);

				return -1;
			}

		#endregion

		#region Map Methods

			/// <summary>
			/// Set a particular tile's texture index
			/// </summary>
			/// <param name="x">0-Based x-direction tile location</param>
			/// <param name="y">0-Based y-direction tile location</param>
			/// <param name="tileIndex">0-Based texture index; else, -1 for an empty tile</param>
			public void setTileTexureIndex(int x, int y, int tileIndex)
			{
				layout[y, x] = tileIndex;
			}

			/// <summary>
			/// Get the index of a texture at a particular tile
			/// </summary>
			/// <param name="x">0-Based x-direction tile location</param>
			/// <param name="y">0-Based y-direction tile location</param>
			/// <returns>0-Based texture index; else, -1 for an empty tile or -2 if x/y out of range</returns>
			public int getTileTextureIndex(int x, int y)
			{
				if (x < 0 || x > widthInTiles - 1 || y < 0 || y > heightInTiles - 1)
					return -2;
				
				return layout[y, x];
			}

			/// <summary>
			/// Remove all reference to an index and shift all higher indexes down 1
			/// </summary>
			/// <param name="existingIndex">0-Based texture index</param>
			/// <remarks>Should only be used by removeTexture()</remarks>
			private void removeTextureIndex(int existingIndex)
			{
				if (existingIndex == -1) return;
				for (int x = 0; x < widthInTiles; ++x)
				{
					for (int y = 0; y < heightInTiles; ++y)
					{
						if (layout[y, x] == existingIndex)
						{
							layout[y, x] = -1;
						}
						else if (layout[y,x] > existingIndex)
						{
							--layout[y, x];
						}
					}
				}
			}

		#endregion

		#region Draw Methods

			/// <summary>
			/// Draws the Tile Layer
			/// </summary>
			/// <param name="batch">SpriteBatch used to render the layer</param>
			/// <param name="camera">Camera used to view the layer</param>
			public void draw(SpriteBatch batch, Camera camera)
			{

				batch.Begin(SpriteSortMode.Texture, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, camera.transformationMatrix);

				int mapWidth = layout.GetLength(1);
				int mapHeight = layout.GetLength(0);

				for (int x = 0; x < mapWidth; ++x)
				{
					for (int y = 0; y < mapHeight; ++y)
					{
						int textureIndex = layout[y, x];

						if (textureIndex == -1)
							continue;

						Texture2D texture = tileTextures[textureIndex];

						batch.Draw(
							texture,
							new Rectangle(
								x * Engine.TILE_WIDTH,
								y * Engine.TILE_HEIGHT,
								Engine.TILE_WIDTH,
								Engine.TILE_HEIGHT),
							new Color(1f, 1f, 1f, _alpha));
					
					}
				}

				batch.End();
			}

			/// <summary>
		/// Draws the Tile Layer
		/// </summary>
		/// <param name="batch">SpriteBatch used to render the layer</param>
		/// <param name="camera">Camera used to view the layer</param>
		/// <param name="min">Minimum position to draw</param>
		/// <param name="max">Maximum position to draw</param>
			public void draw(SpriteBatch batch, Camera camera, Point min, Point max)
			{

				batch.Begin(SpriteSortMode.Texture, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, camera.transformationMatrix);

				int mapWidth = layout.GetLength(1);
				int mapHeight = layout.GetLength(0);

				min.X = (int)Math.Max(min.X, 0);
				min.Y = (int)Math.Max(min.Y, 0);
				max.X = (int)Math.Min(max.X, widthInTiles);
				max.Y = (int)Math.Min(max.Y, heightInTiles);

				for (int x = min.X; x < max.X; ++x)
				{
					for (int y = min.Y; y < max.Y; ++y)
					{
						int textureIndex = layout[y, x];

						if (textureIndex == -1)
							continue;

						Texture2D texture = tileTextures[textureIndex];

						batch.Draw(
							texture,
							new Rectangle(
								x * Engine.TILE_WIDTH,
								y * Engine.TILE_HEIGHT,
								Engine.TILE_WIDTH,
								Engine.TILE_HEIGHT),
							new Color(1f, 1f, 1f, _alpha));

					}
				}

				batch.End();
			}
		
		#endregion

    }
}
