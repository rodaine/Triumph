using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework;

namespace TileEngine
{
    public class TurnManager
    {
        #region TurnManager fields
        private BaseUnit[] units;
        #endregion

        #region constructors 
        /// <summary>
        /// Constructor to create a turnmanager to manage a set of units
        /// </summary>
        /// <param name="units"></param>
        public TurnManager(BaseUnit[] units)
        {
            this.units = units;
        }

        /// <summary>
        /// Empty constructor for turnmanager
        /// </summary>
        public TurnManager()
        {

        }

        #endregion


        #region Methods

        private int getMax()
        {
            int index = -1;
            int max = 999;

            for (int i = 0; i < units.Length; i++)
            {
                if (!units[i].isDead && units[i].delay > max)
                {
                    max = units[i].delay;
                    index = i;
                }
            }

            return index;
        }



        /// <summary>
        /// Gets the next unit in line
        /// </summary>
        /// <returns></returns>
        public BaseUnit getNext()
        {
            for (int i = 0; i < units.Length; i++)
            {
                units[i].tick();
            }
            while (getMax() == -1)
            {
                for (int i = 0; i < units.Length; i++)
                {
                    units[i].tick();
                }
            }
            Weather.getInstance().tick();
			GameConsole.getInstanceOf().Update("It's " + units[getMax()].name + "'s turn!", units[getMax()].faction.color);
            return units[getMax()];
        }


        #endregion

    }
}
