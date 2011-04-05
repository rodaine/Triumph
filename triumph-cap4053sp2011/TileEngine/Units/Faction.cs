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
        String name;
        Player owner;
        BaseUnit[] units;
        #endregion

        #region constructors
        public Faction(String name, Player owner, params BaseUnit[] units)
        {
            this.name = name;
            this.owner = owner;
            this.units = units;
        }

        public Faction(String name, Player owner)
        {
            this.name = name;
            this.owner = owner;
        }

        public Faction()
        {
            this.name = "";
            this.owner = null;
        }
        #endregion

        #region accessor
        public string getName() { return name; }
        public Player getOwner() { return owner; }
        public BaseUnit getUnit(int index) { return units[index]; }
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
