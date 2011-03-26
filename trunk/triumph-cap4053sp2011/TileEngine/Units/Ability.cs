using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileEngine
{
    class Ability
    {
        #region ability fields

        int apCost;
        int attackRange;
        int abilityType;
        int abilityAmount;
        String name;
        String description;

        #endregion

        #region constructors

        public Ability(String name, int abilityType, int abilityAmount, int apCost, int attackRange, String description)
        {
            this.name = name;
            this.apCost = apCost;
            this.attackRange = attackRange;
            this.description = description;

            this.abilityType = abilityType;
            this.abilityAmount = abilityAmount;

        }

        public Ability(String name, int abilityType, int abilityAmount, int apCost, int attackRange)
        {
            this.name = name;
            this.apCost = apCost;
            this.attackRange = attackRange;
            this.description = "No description provided.";

            this.abilityType = abilityType;
            this.abilityAmount = abilityAmount;
        }

        public Ability()
        {
            name = "";
            apCost = 0;
            attackRange = 0;
            description = "";

            abilityType = -1;
            abilityAmount = 0;
        }

        #endregion

        #region get methods

        public String getName() { return name; }
        public String getDescription() { return description; }

        public int getAPCost() { return apCost; }
        public int getAttackRange() { return attackRange; }
        public int getAbilityType() { return abilityType; }
        public int getAbilityAmount() { return abilityAmount; }

        #endregion
    }
}
