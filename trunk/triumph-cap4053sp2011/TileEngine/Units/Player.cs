using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileEngine
{
    public class Player
    {
        #region Player fields
        private Faction _faction;
        private String _name;
        #endregion

        #region constructors
        public Player(String name, Faction faction)
        {
            _name = name;
            _faction = faction;
        }

        public Player(String name)
        {
            _name = name;
            _faction = null;
        }

        public Player()
        {
            _name = "";
            _faction = null;
        }
        #endregion

        #region accessors
        public String name
        { 
            get { return _name; } 
        }
        public Faction faction 
        {
            get { return _faction; }
        }
        #endregion
    }
}
