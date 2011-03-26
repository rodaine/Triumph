using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileEngine
{
    public class Player
    {
        #region Player fields
        Faction faction;
        String name;
        #endregion

        #region constructors
        public Player(String name, Faction faction)
        {
            this.name = name;
            this.faction = faction;
        }

        public Player(String name)
        {
            this.name = name;
            this.faction = null;
        }

        public Player()
        {
            this.name = "";
            this.faction = null;
        }
        #endregion

        #region accessors
        public String getName() { return this.name; }
        public Faction getFaction() { return this.faction; }
        #endregion
    }
}
