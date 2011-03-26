using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileEngine
{
    class Faction
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
    }
}
