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
        
        #endregion

        #region constructors
        public Faction(String name, Player owner, params BaseUnit[] units)
        {
            this._name = name;
            this._owner = owner;
            this._units = units;
        }

        public Faction(String name, Player owner)
        {
            this._name = name;
            this._owner = owner;
        }

        public Faction()
        {
            this._name = "";
            this._owner = null;
        }
        #endregion

        #region accessor
        public string name
        {
            get { return _name; }
        }
        public Player owner
        {
            get { return _owner; }
        }
        public BaseUnit[] units
        {
            get { return _units; }
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
