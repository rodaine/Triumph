using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileEngine
{
    public class Ability
    {
        #region ability fields

        int _apCost;
        int _attackRange;
        int _multiplier;
        EffectTypes _abilityType;
        int _abilityAmount;
        String _name;
        String _description;

        #endregion

        #region constructors

        public Ability(String name, EffectTypes abilityType, int abilityAmount, int apCost, int attackRange, String description)
        {
            this._name = name;
            this._apCost = apCost;
            this._attackRange = attackRange;
            this._description = description;

            this._abilityType = abilityType;
            this._abilityAmount = abilityAmount;

        }

        public Ability(String name, EffectTypes abilityType, int abilityAmount, int apCost, int attackRange)
        {
            this._name = name;
            this._apCost = apCost;
            this._attackRange = attackRange;
            this._description = "No description provided.";

            this._abilityType = abilityType;
            this._abilityAmount = abilityAmount;
        }

        public Ability()
        {
            _name = "";
            _apCost = 0;
            _attackRange = 0;
            _description = "";

            _abilityType = EffectTypes.nothing;
            _abilityAmount = 0;
        }

        #endregion

        #region get methods

        /// <summary>
        /// gets the name of the ability
        /// </summary>
        public String name
        {
            get { return _name; }
        }

        /// <summary>
        /// gets the description of the ability
        /// </summary>
        public String description
        {
            get { return _description; }
        }

        /// <summary>
        /// gets the apcost of the ability
        /// </summary>
        public int APCost 
        { 
            get { return _apCost; }
        }

        /// <summary>
        /// gets the attackRange of the ability
        /// </summary>
        public int attackRange
        {
            get { return _attackRange; }
        }

        /// <summary>
        /// gets the abiltiy type of the ability
        /// </summary>
        public EffectTypes abilityType
        {
            get { return _abilityType; }
        }

        /// <summary>
        /// gets the abilityamount of the ability
        /// It is the flat amount for ap/mp modifiers and heals
        /// it is a percentage for damage abilities
        /// </summary>
        public int abilityAmount
        {
            get { return _abilityAmount; }
        }

        #endregion
    }
}
