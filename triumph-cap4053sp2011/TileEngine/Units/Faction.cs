using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections;
using System.IO;

namespace TileEngine
{
    public class Faction
    {
        #region Faction fields
        private String _name;
        private Player _owner;
        private BaseUnit[] _units;
        private bool _isDefeated = false;
        private int _numDead = 0;
        private Color _col;
        private Point _rallyPoint;
        
        #endregion

        #region constructors
        public Faction(String name, Player owner, params BaseUnit[] units)
        {
            this._name = name;
            this._owner = owner;
            this._units = units;

			foreach (BaseUnit unit in _units)
			{
				unit.faction = this;
			}

        }

        public Faction(String name, Player owner)
        {
            this._name = name;
            this._owner = owner;
        }

		public Faction(string name)
		{
			this._name = name;
			this._owner = null;
		}

        public Faction()
        {
            this._name = "";
            this._owner = null;
        }

		public static Dictionary<string, Faction> fromFile(ContentManager content, string filename, Dictionary<string, BaseUnit> unitList)
		{
			Dictionary<string, Faction> output = new Dictionary<string, Faction>();

			using (StreamReader reader = new StreamReader(filename))
			{
				bool startedFaction = false;
				Faction faction = new Faction();
				while (!reader.EndOfStream)
				{
					string line = reader.ReadLine().Trim();

					if (string.IsNullOrEmpty(line))
						continue;

					if (line[0] == '[' && line[line.Length - 1] == ']')
					{
						if (startedFaction)
						{
							output.Add(faction.name, faction);
						}

						startedFaction = true;
						faction = new Faction(line.Substring(1, line.Length - 2));
					}
					else
					{
						faction.addUnit(unitList[line]);
					}



				}
				if (startedFaction)
				{
					output.Add(faction.name, faction);
				}
			}


			return output;
		}

        #endregion

        #region accessor
        /// <summary>
        /// gets the name of the faction
        /// </summary>
        public string name
        {
            get { return _name; }
			set { _name = value; }
        }
        /// <summary>
        /// gets the name of the owner
        /// </summary>
        public Player owner
        {
            get { return _owner; }
        }
        /// <summary>
        /// gets array of units for the faction
        /// </summary>
        public BaseUnit[] units
        {
            get { return _units; }
        }
        /// <summary>
        /// number of dead soldiers for this faction
        /// </summary>
        public int numDead
        {
            get{ return _numDead;}
            set
            { 
                _numDead = value;
                if (_numDead == _units.Length)
                {
                    _isDefeated = true;
                }
                if (_numDead != units.Length)
                {
                    _isDefeated = false;
                }
            }
        }

        public Color color
        {
            get { return _col; }
            set
            {
                _col = value;
            }
        }

		public void addUnit(BaseUnit unit)
		{
			BaseUnit[] units;

			if (_units == null)
				units = new BaseUnit[1];
			else
			{
				units = new BaseUnit[_units.Length + 1];
				for (int i = 0; i < _units.Length; ++i)
					units[i] = _units[i];
			}
			unit.faction = this;
			units[units.Length - 1] = unit;
			_units = units;
		}

        /// <summary>
        /// Are all units of this faction defeated?
        /// </summary>
        public bool isDefeated
        {
            get { return _isDefeated; }
        }

        /// <summary>
        /// A potentially safe point to go back towards...
        /// </summary>
        public Point rallyPoint
        {
            get { return _rallyPoint; }
            set { _rallyPoint = value; }
        }

        #endregion

        #region draw
        /// <summary>
        /// draws every unit in the faction
        /// </summary>
        /// <param name="spritebatch"></param>
        /// <param name="camera"></param>
        public void draw(SpriteBatch spriteBatch, Camera camera)
        {
            for (int i = 0; i < units.Length; i++)
            {
                units[i].draw(spriteBatch, camera);
            }
        }

        #endregion
    }
}
