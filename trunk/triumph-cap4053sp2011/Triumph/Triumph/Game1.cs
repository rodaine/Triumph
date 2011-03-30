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
		AnimatedSprite sprite;
		BaseUnit testUnit;
        BaseUnit currentUnit;
		SoundEffect soundMusic;
		SoundEffectInstance soundMusicInstance;
		Cursor cursor;
        SpriteFont font, font2;
        TurnManager turnManager = new TurnManager();
		

        private enum Screen
        {
            Title,
            Main,
            Menu
        }
        Screen mCurrentScreen = Screen.Title;

        private enum MenuOptions
        {
            Resume,
            ExitGame
        }
        MenuOptions mCurrentMenuOption = MenuOptions.Resume;

        Texture2D mTitleScreen;
        Texture2D mMainScreen;
        Texture2D mInventoryScreen;
        Texture2D mMenu;
        Texture2D mMenuOptions;

        KeyboardState mPreviousKeyboardState;
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

			FrameAnimation down = new FrameAnimation(2, 32, 32, 64, 0);
			down.framesPerSecond = 5;
			sprite.animations.Add("Down", down);
		
			FrameAnimation left = new FrameAnimation(2, 32, 32, 128, 0);
			left.framesPerSecond = 5;
			sprite.animations.Add("Left", left);

			FrameAnimation right = new FrameAnimation(2, 32, 32, 192, 0);
			right.framesPerSecond = 5;
			sprite.animations.Add("Right", right);

			sprite.currentAnimationName = "Down";
			sprite.speed = 2.5f;
			sprite.originOffset = new Vector2(16, 32);

			testUnit = new BaseUnit("Test Unit", 999, 999, 999, 990, -1);
			testUnit.unitSprite = sprite;
            turnManager.add(testUnit);


            currentUnit = turnManager.getNext();

			soundMusicInstance.Volume = 0.75f;
			soundMusicInstance.IsLooped = true;
			soundMusicInstance.Play();

			cursor.animations.Add("Normal", new FrameAnimation(1, 32, 32, 0, 0));
			cursor.animations.Add("Collision", new FrameAnimation(1, 32, 32, 32, 0));
			cursor.animations.Add("SubSelect", new FrameAnimation(1, 32, 32, 64, 0));
			cursor.animations.Add("Hidden", new FrameAnimation(1, 32, 32, 96, 0));
			cursor.currentAnimationName = "Normal";
			cursor.position = new Vector2(32f, 32f);
			cursor.capturingKeyboard = true;
			camera.setFocus(cursor);

        }

        //load all images and outside files
        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);

			map.layers.Add(TileLayer.fromFile(Content, "Content/Layers/baseTiles.layer"));
			map.layers.Add(TileLayer.fromFile(Content, "Content/Layers/g2m1.layer"));
			map.layers.Add(TileLayer.fromFile(Content, "Content/Layers/g2m2.layer"));
			map.layers.Add(TileLayer.fromFile(Content, "Content/Layers/g2m3.layer"));
			map.layers.Add(TileLayer.fromFile(Content, "Content/Layers/fieldObjects.layer"));
			map.collisionLayer = CollisionLayer.fromFile("Content/Layers/Collision.layer");
			map.unitLayer = new UnitLayer(map.getWidthInTiles(), map.getHeightInTiles());
			sprite = new AnimatedSprite(Content.Load<Texture2D>("Sprites/mnt1"));
            

			soundMusic = Content.Load<SoundEffect>("Music/POL-battle-march-long");
			soundMusicInstance = soundMusic.CreateInstance();

			cursor = new Cursor(Content.Load<Texture2D>("UI/cursor"));

            font = Content.Load<SpriteFont>("UI/SpriteFont1");
            font2 = Content.Load<SpriteFont>("UI/SpriteFont2");

            mTitleScreen = Content.Load<Texture2D>("UI/Title");
            mMainScreen = Content.Load<Texture2D>("UI/MainScreen");
            mMenu = Content.Load<Texture2D>("UI/Menu");
            mMenuOptions = Content.Load<Texture2D>("UI/MenuOptions");

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

            switch (mCurrentScreen)
            {
                case Screen.Title:
                    {
                        //If the user presses the "X" key while on the Title screen, start the game
                        //by switching the current state to the Main Screen
                        if (aKeyboardState.IsKeyDown(Keys.X) == true)
                        {
                            mCurrentScreen = Screen.Main;
                        }
                        break;
                    }
                case Screen.Main:
                    {
                        //If the user presses the "Q" key while in the main game screen, bring
                        //up the Menu options by switching the current state to Menu
                        if (aKeyboardState.IsKeyDown(Keys.Q) == true)
                        {
                            mCurrentScreen = Screen.Menu;
                        }

                        //checks if a unit has finsihed its turn, if it has then make the next unit the active unit
                        if (currentUnit.isDone)
                        {
                            currentUnit.delay += currentUnit.SPD;
                            turnManager.add(currentUnit);
                            currentUnit = turnManager.getNext();
                        }

                        if (soundMusicInstance.State == SoundState.Paused || soundMusicInstance.State == SoundState.Paused)
                        {
                            soundMusicInstance.Volume = 0.75f;
                            soundMusicInstance.IsLooped = true;
                            soundMusicInstance.Play();
                        }

                        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                        {
                            currentUnit.goToTile(Engine.convertPositionToTile(cursor.position), map);
                        }

                        if (Keyboard.GetState().IsKeyDown(Keys.E))
                        {
                            currentUnit.isDone = true;
                        }

                        // TODO: Add your update logic here 
                        int screenWidth = GraphicsDevice.Viewport.Width;
                        int screenHeight = GraphicsDevice.Viewport.Height;

                        testUnit.update(gameTime, screenWidth, screenHeight, map);
                        cursor.update(gameTime, screenWidth, screenHeight, map);

                        camera.update(screenWidth, screenHeight, map);
                        break;
                    }
                case Screen.Menu:
                    {
                        //Move the currently highlighted menu option 
                        //up and down depending on what key the user has pressed
                        if (aKeyboardState.IsKeyDown(Keys.Down) == true && mPreviousKeyboardState.IsKeyDown(Keys.Down) == false)
                        {
                            //Move selection down
                            if (mCurrentMenuOption == MenuOptions.Resume)
                            {
                                mCurrentMenuOption = MenuOptions.ExitGame;

                            }     
                        }

                        if (aKeyboardState.IsKeyDown(Keys.Up) == true && mPreviousKeyboardState.IsKeyDown(Keys.Up) == false)
                        {
                            if (mCurrentMenuOption == MenuOptions.ExitGame)
                            {
                                mCurrentMenuOption = MenuOptions.Resume;
                            }
                        }

                        //If the user presses the "X" key, move the state to the 
                        //appropriate game state based on the current selection
                        if (aKeyboardState.IsKeyDown(Keys.X) == true)
                        {
                            switch (mCurrentMenuOption)
                            {
                                //Return to the Main game screen and close the menu
                                case MenuOptions.Resume:
                                    {
                                        mCurrentScreen = Screen.Main;
                                        break;
                                    }
                                //Exit the game
                                case MenuOptions.ExitGame:
                                    {
                                        this.Exit();
                                        break;
                                    }
                            }

                            //Reset the selected menu option to Resume
                            mCurrentMenuOption = MenuOptions.Resume;
                        }
                        break;
                    }
           }

            //Store the Keyboard state
            mPreviousKeyboardState = aKeyboardState;
           
            base.Update(gameTime);
        }
            

        //called from update, draws screen
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //spriteBatch.Begin();

            switch (mCurrentScreen)
            {
                case Screen.Title:
                    {
                        spriteBatch.Begin();
                        spriteBatch.Draw(mTitleScreen, new Rectangle(0, 0, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height), Color.White);
                        spriteBatch.End();
                        break;
                    }

                case Screen.Main:
                    {
                        map.draw(spriteBatch, camera);
                        cursor.Draw(spriteBatch, camera);
                        testUnit.draw(spriteBatch, camera);
                        break;
                    }

                case Screen.Menu:
                    {
                        spriteBatch.Begin();
                        //spriteBatch.Draw(mMainScreen, new Rectangle(0, 0, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height), Color.Gray);
                        spriteBatch.Draw(mMenu, new Rectangle(this.Window.ClientBounds.Width / 2 - mMenu.Width / 2, this.Window.ClientBounds.Height / 2 - mMenu.Height / 2, mMenu.Width, mMenu.Height), Color.White);

                        switch (mCurrentMenuOption)
                        {
                            case MenuOptions.Resume:
                                {
                                    spriteBatch.DrawString(font2, "Main Menu", new Vector2(this.Window.ClientBounds.Width / 2 - 50, this.Window.ClientBounds.Height / 2 - mMenu.Height / 2 + 75), Color.Red);
                                    spriteBatch.DrawString(font, "Resume", new Vector2(this.Window.ClientBounds.Width / 2 - 50, this.Window.ClientBounds.Height / 2 - mMenu.Height / 2 + 150), Color.Gold);
                                    spriteBatch.DrawString(font, "Exit", new Vector2(this.Window.ClientBounds.Width / 2 - 50, this.Window.ClientBounds.Height / 2 - mMenu.Height / 2 + 250), Color.White);
                                    break;
                                }

                            case MenuOptions.ExitGame:
                                {
                                    spriteBatch.DrawString(font2, "Main Menu", new Vector2(this.Window.ClientBounds.Width / 2 - 50, this.Window.ClientBounds.Height / 2 - mMenu.Height / 2 + 75), Color.Red);
                                    spriteBatch.DrawString(font, "Resume", new Vector2(this.Window.ClientBounds.Width / 2 - 50, this.Window.ClientBounds.Height / 2 - mMenu.Height / 2 + 150), Color.White);
                                    spriteBatch.DrawString(font, "Exit", new Vector2(this.Window.ClientBounds.Width / 2 - 50, this.Window.ClientBounds.Height / 2 - mMenu.Height / 2 + 250), Color.Gold);
                                    break;
                                }
                        }
                        spriteBatch.End();
                        break;
                    }
            }

           // spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
