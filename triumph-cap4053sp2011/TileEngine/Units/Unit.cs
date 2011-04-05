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

/*  Need to determine how we will handle objects blocking target    *
 *  and how we would like to determine distances                    *
 *  distanceTo(target) & isBlocked(target)                          */


namespace TileEngine
{
	public class BaseUnit
    {
        #region baseUnit fields

		//Identification
		public static int index_counter;
        int index;
        string _name;
        Faction _faction;

		//Attributes
		private int _maxHP, _maxAP, _maxMP, _HP, _AP, _MP, _SPD, _delay;
        private bool _isDead, _isStunned, _isDone;
        private List<Ability> moves;
        private List<Buff> itemsAndBuffs;
        private int unitAffinity;
        private static string[] affinities = { "Water", "Fire", "Earth", "Stone", "Wind", "Ice" };

		//Sprite and Movement
		private AnimatedSprite _unitSprite;
		private Point _position;
		private bool isWalking = false;
		private Stack<Point> path;
		
        #endregion

        #region attribute properties

		/// <summary>
		/// Gets the maxHP of the unit.
		/// </summary>
		public int maxHP
		{
			get { return _maxHP; }
		}

		/// <summary>
		/// Gets the maxAP of the unit.
		/// </summary>
		public int maxAP
		{
			get { return _maxAP; }
		}

		/// <summary>
		/// Gets the maxMP of the unit.
		/// </summary>
		public int maxMP
		{
			get { return _maxMP; }
		}
		
		/// <summary>
		/// Get or set the HP of the unit inclusively between 0 and maxHP. If HP reaches zero, unit isDead = true; otherwise, unit isDead = false.
		/// </summary>
		public int HP
		{
			get { return _HP; }
			set
			{
				_HP = (int)MathHelper.Clamp(value, 0, _maxHP);
				_isDead = (_HP == 0) ? true : false;
			}
		}

		/// <summary>
		/// Get or set the AP of the unit inclusively between 0 and maxAP.
		/// </summary>
		public int AP
		{
			get { return _AP; }
			set { _AP = (int)MathHelper.Clamp(value, 0, _maxAP); }
		}

		/// <summary>
		/// Get or set the MP of the unit inclusively between 0 and maxMP.
		/// </summary>
		public int MP
		{
			get { return _MP; }
			set { _MP = (int)MathHelper.Clamp(value, 0, _maxMP); }
		}

		/// <summary>
		/// Get the base speed of the unit.
		/// </summary>
		public int SPD
		{
			get { return _SPD; }
		}
		
        /// <summary>
        /// Get the current delay of the unit
        /// </summary>
        public int delay
        {
            get { return _delay; }
            set { _delay = value; }
        }

		/// <summary>
		/// Get whether or not the unit isDead
		/// </summary>
		public bool isDead
		{
			get { return _isDead; }
		}

		/// <summary>
		/// Get or set whether or not the unit isStunned
		/// </summary>
		public bool isStunned
		{
			get { return _isStunned; }
			set { _isStunned = value; }
		}

        ///<summary>
        /// Get or set whether a unit is done with its turn
        /// </summary>
        public bool isDone
        {
            get { return _isDone; }
            set { _isDone = value; }
        }

		/// <summary>
		/// Get the index of the unit's elemental affinity
		/// </summary>
		public int affinityIndex
		{
			get { return unitAffinity; }
			set
			{
				unitAffinity = (int)MathHelper.Clamp(value, -1, affinities.Length - 1);
			}
		}

		/// <summary>
		/// Get the name of the unit's elemental affinity
		/// </summary>
		public string affinityName
		{
			get { return affinities[unitAffinity]; }
		}

		/// <summary>
		/// Get the position (by tiles) of the unit
		/// </summary>
		public Point position
		{
			get { return _position; }
		}

		/// <summary>
		/// Get or set the Animated Sprite representing the unit
		/// </summary>
		public AnimatedSprite unitSprite
		{
			get { return _unitSprite; }
			set { _unitSprite = value; }
		}

		public int unitIndex
		{
			get { return index; }
			set { index = value; }
		}

		public string name
		{
			get { return _name; }
			set { _name = value; }
		}

		public Faction faction
		{
			get { return _faction; }
			set { _faction = value; }
		}

		#endregion

        #region constructors

		/// <summary>
		/// Create baseUnit object with abilities
		/// </summary>
		/// <param name="name">Name of unit</param>
		/// <param name="maxHP">Maximum Hit Points of unit</param>
		/// <param name="maxAP">Maximum Action Points of unit</param>
		/// <param name="maxMP">Maximum Movement Points of unit</param>
		/// <param name="SPD">Speed of unit</param>
		/// <param name="unitAffinity">Index of elemental affinity of unit</param>
		/// <param name="abilities">Abilities of unit</param>
        public BaseUnit(String name, int maxHP, int maxAP, int maxMP, int SPD, int unitAffinity, params Ability[] abilities)
        {
			_name = name;
			_HP = _maxHP = maxHP;
			_AP = _maxAP = maxAP;
			_MP = _maxMP = maxMP;
			_SPD = _delay = SPD;
            _isDone = false;

			if (_HP > 0)
				_isDead = false;
			else
				_isDead = true;

			this.unitAffinity = unitAffinity;

			moves = new List<Ability>();
            for (int i = 0; i < abilities.Length; ++i)
                moves.Add(abilities[i]);

			index = ++index_counter;

        }

		/// <summary>
		/// Create baseUnit object without abilities
		/// </summary>
		/// <param name="name">Name of unit</param>
		/// <param name="maxHP">Maximum Hit Points of unit</param>
		/// <param name="maxAP">Maximum Action Points of unit</param>
		/// <param name="maxMP">Maximum Movement Points of unit</param>
		/// <param name="SPD">Speed of unit</param>
		/// <param name="unitAffinity">Index of elemental affinity of unit</param>
        public BaseUnit(String name, int maxHP, int maxAP, int maxMP, int SPD, int unitAffinity)
        {
			this._name = name;
			_HP = _maxHP = maxHP;
			_AP = _maxAP = maxAP;
			_MP = _maxMP = maxMP;
            _SPD = _delay = SPD;
            _isDone = false;
			if (_HP > 0)
				_isDead = false;
			else
				_isDead = true;

			this.unitAffinity = unitAffinity;

			moves = new List<Ability>();

			index = ++index_counter;
        }

		/// <summary>
		/// Create an empty baseUnit
		/// </summary>
        public BaseUnit()
        {
			this._name = "";
			_HP = _maxHP = 1;
			_AP = _maxAP = 1;
			_MP = _maxMP = 1;
			_SPD = _delay = 1;
			_isDead = false;
            _isDone = false;

			this.unitAffinity = -1;
		
			moves = new List<Ability>();

			index = ++index_counter;
        }

		/// <summary>
		/// Add one or more abilities to the unit
		/// </summary>
		/// <param name="abilities"></param>
		public void addAbility(params Ability[] abilities)
		{
			for (int i = 0; i < abilities.Length; ++i)
				moves.Add(abilities[i]);
		}

		public static Dictionary<string, BaseUnit> fromFile(ContentManager content, string filename)
		{
			Dictionary<string, BaseUnit> unitList = new Dictionary<string, BaseUnit>();

			FrameAnimation up = new FrameAnimation(2, 32, 32, 0, 0);
			up.framesPerSecond = 5;

			FrameAnimation down = new FrameAnimation(2, 32, 32, 64, 0);
			down.framesPerSecond = 5;

			FrameAnimation left = new FrameAnimation(2, 32, 32, 128, 0);
			left.framesPerSecond = 5;

			FrameAnimation right = new FrameAnimation(2, 32, 32, 192, 0);
			right.framesPerSecond = 5;

			using (StreamReader reader = new StreamReader(filename))
			{
				while (!reader.EndOfStream)
				{
					string line = reader.ReadLine().Trim();
					if (string.IsNullOrEmpty(line)) continue;
					if (line.Contains("///")) continue;
					string[] unitParams = line.Split(',');
					BaseUnit unit = new BaseUnit(unitParams[0].Trim(), int.Parse(unitParams[1].Trim()), int.Parse(unitParams[2].Trim()), int.Parse(unitParams[3].Trim()), int.Parse(unitParams[4].Trim()), int.Parse(unitParams[5].Trim()));

					unit.unitSprite = new AnimatedSprite(content.Load<Texture2D>(unitParams[6].Trim()));
					unit.unitSprite.animations.Add("Up", up);
					unit.unitSprite.animations.Add("Down", down);
					unit.unitSprite.animations.Add("Left", left);
					unit.unitSprite.animations.Add("Right", right);
					unit.unitSprite.speed = 2.5f;
					unit.unitSprite.originOffset = new Vector2(16f, 32f);
					unitList.Add(unit._name, unit);
				}
			}

			return unitList;
		}

		#endregion

        #region attack
        /// <summary>
        /// tells the unit to attack another unit
        /// </summary>
        /// <param name="target"></param>
        public void attack(BaseUnit target, int rand)
        {
            target.HP -= rand; //filler at the moment for an attack formula
            _delay += _SPD;
            _isDone = true;
        }

        #endregion

        #region comparers

        /// <summary>
        /// Allows units to be sorted by delay
        /// </summary>
        public class sortByDelay:IComparer
        {
            int IComparer.Compare(Object x, Object y)
            {
                BaseUnit a = (BaseUnit)x;
                BaseUnit b = (BaseUnit)y;
                if (a.delay < b.delay)
                {
                    return 1;
                }
                else if (a.delay == b.delay)
                {
                    return 0;
                }
                return -1;
            }
        }

        #endregion

        #region use_ability

        //public Boolean use_ability(Ability action, BaseUnit target);

		/*public Boolean use_ability(Ability action, BasePlayer target)
		{
			//check that ability may be performed (enough ap and target within range is all for now)
			if (action.getAPCost() > AP) { return false; }

			if (action.getAttackRange() < distanceTo(target)) { return false; }

			if (isBlocked(target)) { return false; }

			//if all checks pass then carry out action
			//include buffs/debuffs/affinities here as well
			if (action.getAbilityType() == 0)                   //attack
			{
				target.damage(action.getAbilityAmount());
			}
			else if (action.getAbilityType() == 1)              //heal
			{
				target.heal(action.getAbilityAmount());
			}                                                   //more to come (buffs/debuffs)
			else
			{

			}

			return true;
		}*/

		#endregion

		#region Turn Checks

		public void run()
        {
            //adjust unit stats
            foreach (Buff item in itemsAndBuffs)
            {
                switch (item.getObjectType())
                {
                    case 1:
                        //heal
						HP += item.getObjectAmount();
                        break;
                    case 2:
                        //damage
						HP -= item.getObjectAmount();
                        break;
                    case 3:
                        //stun
						isStunned = item.getObjectFlip();
                        break;
                    case 4:
                        //increase MP
						MP += item.getObjectAmount();
                        break;
                    case 5:
                        //decrease MP
						MP -= item.getObjectAmount();
                        break;
                    case 6:
                        //increase AP
						AP += item.getObjectAmount();
                        break;
                    case 7:
                        //decrease AP
						AP -= item.getObjectAmount();
                        break;
                    default:
                        //not a valid choice
                        break;
                }

				item.decTurn();
				if (item.getTurnDuration() == 0)
					itemsAndBuffs.Remove(item);
            }

        }

        #endregion

		#region Sprite Updates

		/// <summary>
		/// Updates the units sprite representation based on flags.
		/// </summary>
		/// <param name="gameTime">GameTime object passed from Game class</param>
		/// <param name="screenWidth">Viewport screen width in pixels</param>
		/// <param name="screenHeight">Viewport screen height in pixels</param>
		/// <param name="map">Tile Map of play area</param>
		public void update(GameTime gameTime, int screenWidth, int screenHeight, TileMap map)
		{
			//check flags
			if (_unitSprite.isMoving)
				isWalking = true;
			else
				isWalking = false;
			
			//check if dead...check if stunned...etc.
			//update sprite 
			map.unitLayer.moveUnit(index, position);
			_unitSprite.update(gameTime, screenWidth, screenHeight, map);
		}

		/// <summary>
		/// Move the unit to a specified tile. This function does not check for the MP cost of the move!
		/// </summary>
		/// <param name="goal">Goal location of the </param>
		/// <param name="map">Tile Map of play area</param>
		public void goToTile(Point goal, TileMap map)
		{
			if (isWalking) return;
            if (map.unitLayer.getTileUnitIndex(goal) != 0) return;
			if (map.collisionLayer.getTileCollisionIndex(goal) != 0) return;

			unitSprite.goToTile(goal, map);
			_position = goal;
			isWalking = true;
			map.unitLayer.moveUnit(index, goal);
		}

		/// <summary>
		/// Teleports the unit to any non-collision point on the map
		/// </summary>
		/// <param name="goal">Goal tile location to teleport to.</param>
		/// <param name="map">Tile Map of play area</param>
		public void teleportToTile(Point goal, TileMap map)
		{
			if (isWalking) return;
			if (map.unitLayer.getTileUnitIndex(goal) != 0) return;
			if (map.collisionLayer.getTileCollisionIndex(goal) != 0) return;

			unitSprite.position = new Vector2((float)goal.X * Engine.TILE_WIDTH, (float)goal.Y * Engine.TILE_HEIGHT);
			_position = goal;
			map.unitLayer.moveUnit(index, goal);
		}

		#endregion

		#region drawing player

		/// <summary>
		/// Draws the unit on the map
		/// </summary>
		/// <param name="spriteBatch">SpriteBatch passed from Game class</param>
		/// <param name="camera">Camera object passed from Game class</param>
		public void draw(SpriteBatch spriteBatch, Camera camera)
		{
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, camera.transformationMatrix);
			unitSprite.Draw(spriteBatch);
			spriteBatch.End();
		}

        #endregion

	}

}
