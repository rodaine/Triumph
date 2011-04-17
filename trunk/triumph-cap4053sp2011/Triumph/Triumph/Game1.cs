using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TileEngine;

namespace Triumph
{
    public class Game1 : Microsoft.Xna.Framework.Game
	{
		#region Game Objects
		GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
		Camera camera = new Camera();
		Range range;
		TileMap map = new TileMap();
        UI ui = new UI();
        AI ai = new AI();
        BaseUnit[] testUnits;
        BaseUnit currentUnit;
        BaseUnit targetUnit;
        BaseUnit[] faction1Units;
        BaseUnit[] faction2Units;
        Faction faction1;
        Faction faction2;
		SoundEffect soundMusic;
		SoundEffectInstance soundMusicInstance;
		Cursor cursor;
        TurnManager turnManager;
		Dictionary<string, BaseUnit> unitList;
		Dictionary<string, Ability> abilityList;
		Dictionary<string, Faction> factionList;
        int counter = 10;
        bool inGame = false; //TODO I don't like this, should only be true after UI.screen goes to Main
        bool unitBeingAttacked = false;
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        //Set initial values for everything
        protected override void Initialize()
        {
            base.Initialize();

			faction1 = factionList["Dankelites"];
			faction1.color = Color.Blue;

			faction2 = factionList["Dougonians"];
			faction2.color = Color.Red;

			testUnits = new BaseUnit[faction1.units.Length + faction2.units.Length];

			for (int i = 0; i < faction1.units.Length; ++i)
			{
				testUnits[i] = faction1.units[i];
				testUnits[i + faction1.units.Length] = faction2.units[i];
			}

			foreach (BaseUnit unit in testUnits)
			{
				if (unit.faction == faction1)
					unit.randomPosition(new Point(20, 0), new Point(29, 9), map);
				else
					unit.randomPosition(new Point(8, 20), new Point(17, 29), map);

			}

            turnManager = new TurnManager(testUnits);
            currentUnit = turnManager.getNext();
            cursor.location = currentUnit.position;

			soundMusicInstance.Volume = 0.25f;
			soundMusicInstance.IsLooped = true;
			soundMusicInstance.Play();

			cursor.animations.Add("Normal", new FrameAnimation(1, 32, 32, 0, 0));
			cursor.animations.Add("Collision", new FrameAnimation(1, 32, 32, 32, 0));
			cursor.animations.Add("Select", new FrameAnimation(1, 32, 32, 64, 0));
			cursor.animations.Add("Hidden", new FrameAnimation(1, 32, 32, 96, 0));
			cursor.currentAnimationName = "Normal";
			cursor.capturingKeyboard = true;
			camera.setFocus(cursor);

			range.rangeTypes.Add("Normal", new FrameAnimation(1, 32, 32, 0, 0));
			range.rangeTypes.Add("Collision", new FrameAnimation(1, 32, 32, 32, 0));
			range.rangeTypes.Add("Select", new FrameAnimation(1, 32, 32, 64, 0));
			range.rangeTypes.Add("Hidden", new FrameAnimation(1, 32, 32, 96, 0));
			range.currentRangeTypeName = "Select";

        }

        //load all images and outside files
        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);
			BaseUnit.index_counter = 0;

			map.layers.Add(TileLayer.fromFile(Content, "Content/Layers/outdoor_base2.layer"));
			map.layers.Add(TileLayer.fromFile(Content, "Content/Layers/outdoor_road2.layer"));
			map.layers.Add(TileLayer.fromFile(Content, "Content/Layers/outdoor_trans2.layer"));
			map.layers.Add(TileLayer.fromFile(Content, "Content/Layers/outdoor_objs2.layer"));
			map.collisionLayer = CollisionLayer.fromFile("Content/Layers/Collision.layer");
			map.unitLayer = new UnitLayer(map.getWidthInTiles(), map.getHeightInTiles());
			
			abilityList = Ability.fromFile("Content/Units/abilities.txt");		
			unitList = BaseUnit.fromFile(Content, "Content/Units/units.txt", abilityList);
			factionList = Faction.fromFile(Content, "Content/Units/factions.txt", unitList);

			soundMusic = Content.Load<SoundEffect>("Music/POL-battle-march-long");
			soundMusicInstance = soundMusic.CreateInstance();

			SoundEffect attack = Content.Load<SoundEffect>("Music/attack");
			SoundEffect hit = Content.Load<SoundEffect>("Music/hit");
			SoundEffect crit = Content.Load<SoundEffect>("Music/crit");
			SoundEffect dodge = Content.Load<SoundEffect>("Music/dodge");
			SoundEffect dead = Content.Load<SoundEffect>("Music/dead");
			SoundEffect walking = Content.Load<SoundEffect>("Music/walking");

			foreach (KeyValuePair<string, BaseUnit> unit in unitList)
			{
				unit.Value.unitSprite.loadSFX(attack, hit, crit, dodge, dead, walking);
			}

			cursor = new Cursor(Content.Load<Texture2D>("UI/cursor"));
			range = new Range(Content.Load <Texture2D>("UI/cursor"));
            

            ui.LoadContent(Content);
            GameConsole.getInstanceOf().LoadContent(Content);

        }
        
        //we do not currently use this!!
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        //every game tick prompts the following actions
        protected override void Update(GameTime gameTime)
        {
            KeyboardState aKeyboardState = Keyboard.GetState();


            if (soundMusicInstance.State == SoundState.Paused || soundMusicInstance.State == SoundState.Paused)
            {
                soundMusicInstance.Volume = 0.75f;
                soundMusicInstance.IsLooped = true;
              //  soundMusicInstance.Play();
            }

            //check current target unit
            Boolean found = false;
            unitBeingAttacked = false;
            for (int i = 0; i < testUnits.Length; i++)
            {
                if (testUnits[i].position.X==cursor.location.X && testUnits[i].position.Y == cursor.location.Y)
                {
                    targetUnit = testUnits[i];
                    found = true;
                }

                if (testUnits[i].isBeingHit) unitBeingAttacked = true;
            }
            if (!found)
            {
                targetUnit = null;
            }


            /* Sorta hacky but it works for the time being, replace condition with some parameter inside Faction or Player that 
             * tells whether or not that player/faction is AI controlled or human controlled. */
            if (inGame && currentUnit.faction == faction2)
            {
                ai.update(gameTime, currentUnit, cursor, map, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, testUnits, camera, unitBeingAttacked);
            }
            else
            {
                bool end = false;
                if (faction1.isDefeated || faction2.isDefeated)
                    end = true;
                ui.Update(gameTime, aKeyboardState, currentUnit, targetUnit, cursor, map, counter, turnManager, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, testUnits, camera, range, end, ref inGame, unitBeingAttacked, faction1);
            }
            
            if (ui.readyToExit())
            {
                this.Exit();
            }

            counter--;
            //checks if a unit has finsihed its turn, if it has then make the next unit the active unit
            if (currentUnit.isDone&& !currentUnit.isWalking && !currentUnit.isAttacking&&!unitBeingAttacked)
            {
                counter = 10;
                currentUnit.endTurn();
                currentUnit = turnManager.getNext();
                if (currentUnit.isStunned)
                {
                    currentUnit.MP = 0;
                    currentUnit.AP = 0;
                    currentUnit.stunLength-=1;
                    currentUnit.beStunned();
                    GameConsole.getInstanceOf().Update(currentUnit.name + " is stunned!", currentUnit.faction.color);
                }
                cursor.location = currentUnit.position;
            }


            //checks if all living units are from the same faction
            
            if (faction1.isDefeated)
            {
                inGame = false;
                //TODO You lost
            }
            else if (faction2.isDefeated)
            {
                inGame = false;
                //TODO You won
            }


            base.Update(gameTime);
        }
            

        //called from update, draws screen
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            int winner = 0;
            if (faction2.isDefeated)
                winner = 1;
            else if (faction1.isDefeated)
                winner = 2;
                  

            ui.Draw(gameTime, spriteBatch, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height, map, camera, cursor, testUnits, currentUnit, targetUnit, range, winner, faction1);

            base.Draw(gameTime);
        }

    }
}
