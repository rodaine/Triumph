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
		TileMap map = new TileMap();
        UI ui = new UI();
		AnimatedSprite sprite;
        AnimatedSprite sprite2;
        BaseUnit[] testUnits;
		BaseUnit testUnit;
        BaseUnit testUnit2;
        BaseUnit currentUnit;
        BaseUnit targetUnit;
		SoundEffect soundMusic;
		SoundEffectInstance soundMusicInstance;
		Cursor cursor;
        SpriteFont font, font2;
        TurnManager turnManager = new TurnManager();
        RandomNumber random = new RandomNumber();
		Dictionary<string, BaseUnit> unitList;
        int counter = 100;		
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

			FrameAnimation up = new FrameAnimation(2, 32, 32, 0, 0);
			up.framesPerSecond = 5;
			sprite.animations.Add("Up", up);
            sprite2.animations.Add("Up", up);

			FrameAnimation down = new FrameAnimation(2, 32, 32, 64, 0);
			down.framesPerSecond = 5;
			sprite.animations.Add("Down", down);
            sprite2.animations.Add("Down", down);
		
			FrameAnimation left = new FrameAnimation(2, 32, 32, 128, 0);
			left.framesPerSecond = 5;
			sprite.animations.Add("Left", left);
            sprite2.animations.Add("Left", left);

			FrameAnimation right = new FrameAnimation(2, 32, 32, 192, 0);
			right.framesPerSecond = 5;
			sprite.animations.Add("Right", right);
            sprite2.animations.Add("Right", right);

			sprite.currentAnimationName = "Down";
			sprite.speed = 2.5f;
			sprite.originOffset = new Vector2(16, 32);

            sprite2.currentAnimationName = "Left";
            sprite2.speed = 2.5f;
            sprite2.originOffset = new Vector2(16, 32);

			testUnit = new BaseUnit("Test Unit", 999, 999, 999, 8, -1);
			testUnit.unitSprite = sprite;

            testUnit2 = new BaseUnit("Test Unit 2", 999, 999, 999, 9, -1);
            testUnit2.unitSprite = sprite2;

			//testUnit.unitIndex = 1;
			//testUnit2.unitIndex = 2;

			testUnit2.teleportToTile(new Point(2, 2), map);

			turnManager.add(testUnit2);
            turnManager.add(testUnit);

            testUnits = new BaseUnit[2];
            testUnits[0] = testUnit;
            testUnits[1] = testUnit2;

            currentUnit = turnManager.getNext();

			soundMusicInstance.Volume = 0.75f;
			soundMusicInstance.IsLooped = true;
			//soundMusicInstance.Play();

			cursor.animations.Add("Normal", new FrameAnimation(1, 32, 32, 0, 0));
			cursor.animations.Add("Collision", new FrameAnimation(1, 32, 32, 32, 0));
			cursor.animations.Add("SubSelect", new FrameAnimation(1, 32, 32, 64, 0));
			cursor.animations.Add("Hidden", new FrameAnimation(1, 32, 32, 96, 0));
			cursor.currentAnimationName = "Normal";
			cursor.capturingKeyboard = true;
			camera.setFocus(cursor);

        }

        //load all images and outside files
        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);
			BaseUnit.index_counter = 0;

			map.layers.Add(TileLayer.fromFile(Content, "Content/Layers/baseTiles.layer"));
			map.layers.Add(TileLayer.fromFile(Content, "Content/Layers/g2m1.layer"));
			map.layers.Add(TileLayer.fromFile(Content, "Content/Layers/g2m2.layer"));
			map.layers.Add(TileLayer.fromFile(Content, "Content/Layers/g2m3.layer"));
			map.layers.Add(TileLayer.fromFile(Content, "Content/Layers/fieldObjects.layer"));
			map.collisionLayer = CollisionLayer.fromFile("Content/Layers/Collision.layer");
			map.unitLayer = new UnitLayer(map.getWidthInTiles(), map.getHeightInTiles());
			sprite = new AnimatedSprite(Content.Load<Texture2D>("Sprites/mnt1"));
            sprite2 = new AnimatedSprite(Content.Load<Texture2D>("Sprites/mnv1"));
			unitList = BaseUnit.fromFile(Content, "Content/Units/units.txt");

			soundMusic = Content.Load<SoundEffect>("Music/POL-battle-march-long");
			soundMusicInstance = soundMusic.CreateInstance();

			cursor = new Cursor(Content.Load<Texture2D>("UI/cursor"));

            font = Content.Load<SpriteFont>("UI/SpriteFont1");
            font2 = Content.Load<SpriteFont>("UI/SpriteFont2");

            ui.LoadContent(Content);

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

            //If user hits the Escape key exit the game
            if (aKeyboardState.IsKeyDown(Keys.Escape) == true)
            {
                this.Exit();
            }

            if (soundMusicInstance.State == SoundState.Paused || soundMusicInstance.State == SoundState.Paused)
            {
                soundMusicInstance.Volume = 0.75f;
                soundMusicInstance.IsLooped = true;
                //soundMusicInstance.Play();
            }

            //check current target unit
            Boolean found = false;
            for (int i = 0; i < testUnits.Length; i++)
            {
                if (testUnits[i].position.X==cursor.location.X && testUnits[i].position.Y == cursor.location.Y)
                {
                    targetUnit = testUnits[i];
                    found = true;
                }
            }
            if (!found)
            {
                targetUnit = null;
            }

            ui.Update(gameTime, aKeyboardState, currentUnit,targetUnit, cursor, map, counter, turnManager, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, testUnits, camera, random);
            counter--;
            //checks if a unit has finsihed its turn, if it has then make the next unit the active unit
            if (currentUnit.isDone)
            {
                counter = 100;
                currentUnit.delay += currentUnit.SPD;
                currentUnit.isDone = false;
                turnManager.add(currentUnit);
                currentUnit = turnManager.getNext();
                cursor.location = currentUnit.position;
            }

            base.Update(gameTime);
        }
            

        //called from update, draws screen
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            ui.Draw(gameTime, spriteBatch, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height, map, camera, cursor, testUnits, currentUnit, targetUnit, font, font2);

            base.Draw(gameTime);
        }
    }
}
