using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileEngine
{

     public enum EffectTypes { heal, damage, stun, incMP, decMP, incAP, decAP, nothing };

     public class Buff
    {
        #region triggerobject fields

        String name;
        String description;
        int turnDuration;
        int objectAmount;
        EffectTypes objectType;         
        /*
         * Enums basically do this for you... using ints just leads to hard to read code
         */
        Boolean objectFlip;

        #endregion

        #region constructors

        public Buff(String name, EffectTypes objectType, int objectAmount, int turnDuration, Boolean objectFlip, String description)
        {
            this.name = name;
            this.objectType = objectType;
            this.objectAmount = objectAmount;
            this.turnDuration = turnDuration;
            this.objectFlip = objectFlip;
            this.description = description;
        }

        public Buff(String name, EffectTypes objectType, int objectAmount, int turnDuration, Boolean objectFlip)
        {
            this.name = name;
            this.objectType = objectType;
            this.objectAmount = objectAmount;
            this.turnDuration = turnDuration;
            this.objectFlip = objectFlip;
            description = "No description provided.";
        }

        public Buff()
        {
            name = "";
            objectType = EffectTypes.nothing;
            objectAmount = 0;
            turnDuration = 0;
            objectFlip = false;
            description = "";
        }

        #endregion

        #region get methods
        
        public EffectTypes getObjectType() { return objectType; }
        public int getObjectAmount() { return objectAmount; }
        public int getTurnDuration() { return turnDuration; }
        public String getName() { return name; }
        public String getDescription() { return description; }
        public Boolean getObjectFlip() { return objectFlip; }

        #endregion

        #region methods

        public void decTurn()
        {
            turnDuration--;
        }

        #endregion

    }
}

