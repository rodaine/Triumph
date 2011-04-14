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

namespace TileEngine
{
    public class GameConsole
    {
        public static GameConsole singleton;
        string[] sm = new string[15];
        Color[] colors = new Color[15];
        int linesToPrint;

        Texture2D black;
        Texture2D scroll;
        Texture2D clock;

        SpriteFont arial9pt;
        SpriteFont consolas;

        bool changed = false;

        private double totalTimeInSeconds = 0;
        int hr = 0, min = 0, sec = 0;        

        /// <summary>
        /// returns a singleton of GameConsole
        /// </summary>
        /// <returns></returns>
        public static GameConsole getInstanceOf()
        {
            if(singleton == null)
                singleton = new GameConsole();

            return singleton;
        }

        public void LoadContent(ContentManager Content)
        {
            black = Content.Load<Texture2D>("Console/black");
            scroll = Content.Load<Texture2D>("Console/consoleScroll");
            clock = Content.Load<Texture2D>("Console/timer_white");

            arial9pt = Content.Load<SpriteFont>("Console/arial9pt");
            consolas = Content.Load<SpriteFont>("Console/consolas");
        }

        /// <summary>
        /// Splits a message into lines of at most 24 characters without splitting words
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public List<String> splitByWord(String message)
        {
            List<String> ret = new List<string>();
            String[] words = message.Split(' ');
            String curLine = words[0];

            for (int i = 1; i < words.Length; i++)
            {
                if (curLine.Length + 1 + words[i].Length > 24)//change this value if you going to use this method for different sized boxes
                {
                    ret.Add(curLine);
                    curLine = " " + words[i];
                }
                else
                {
                    curLine = curLine + " " + words[i];
                }
            }

            if (curLine != "")
                ret.Add(curLine);
            return ret;
        }

        public void Update(string newMessage, Color col)
        {
            string[] tempMess = new string[15];
            Color[] tempCol = new Color[15];
            for (int i = 0; i < 15; i++)
            {
                tempMess[i] = sm[i];
                tempCol[i] = colors[i];
            }

            /*int lines = 0;
            int len = newMessage.Length;

            while (len > 0)
            {
                if (len >= 24)
                {
                    lines++;
                    len -= 24;
                }
                else
                {
                    lines++;
                    len = 0;
                }
            }*/

            List<String> lines = splitByWord(newMessage);

            for (int j = 0; j < lines.Count; j++)
            {
                sm[j] = lines[j];
                colors[j] = col;
                System.Console.WriteLine(lines[j]);
            }


            for (int k = lines.Count; k < 15; k++)
            {
                sm[k] = tempMess[k - lines.Count];
                colors[k] = tempCol[k - lines.Count];
            }

            linesToPrint = 0;
            for (int z = 0; z < 15; z++)
            {
                if(!String.IsNullOrEmpty(sm[z]))
                {
                    linesToPrint++;
                }
            }
        }

        public void UpdateClockTime(double timeToAdd)
        {
            totalTimeInSeconds += timeToAdd;
            int temp = (int)totalTimeInSeconds;
            hr = (temp / 3600);
            min = (temp % 3600) / 60;
            sec = (temp % 3600) % 60;
        }

        public void Draw(SpriteBatch spriteBatch, int winWidth, int winHeight, bool left, bool bottom, bool hide)
        {            
            drawClock(spriteBatch, winHeight, winWidth, bottom);

            if (!hide)
            {
                spriteBatch.Begin();

                int x, y, wid, hei;

                wid = winWidth / 3;
                hei = winHeight - winHeight / 3;

                if (!left)
                {
                    x = winWidth - wid - Engine.TILE_WIDTH / 2;
                }
                else
                {
                    x = Engine.TILE_WIDTH / 2;
                }

                if (!bottom)
                {
                    y = Engine.TILE_HEIGHT / 2;
                }
                else
                {
                    y = winHeight - hei - Engine.TILE_HEIGHT / 2;
                }

                int yOffset = 15;
                spriteBatch.Draw(scroll, new Rectangle(x, y, wid, hei), Color.White);

                for (int i = 0; i < linesToPrint; i++)
                {
                    spriteBatch.DrawString(consolas, sm[i], new Vector2(x + 35, y + 50 + i * yOffset), colors[i]);
                }

                spriteBatch.End();                

                changed = false;
            }
        }

        private void drawClock(SpriteBatch spriteBatch, int winHeight, int winWidth, bool bottom)
        {
            spriteBatch.Begin();

            int c1 = winWidth / 2 - Engine.TILE_WIDTH;
            int clockX = winWidth / 2 - Engine.TILE_WIDTH / 3;
            int c2, adj;

            if (bottom)
            {
                c2 = winHeight - Engine.TILE_HEIGHT / 2 - Engine.TILE_HEIGHT / 8;
                adj = -1;
            }
            else
            {
                c2 = Engine.TILE_HEIGHT / 8;
                adj = 1;
            }
            spriteBatch.Draw(black, new Rectangle(c1, c2, Engine.TILE_WIDTH * 2 + Engine.TILE_WIDTH / 4, Engine.TILE_HEIGHT / 2), new Color(1f, 1f, 1f, .4f));
            spriteBatch.Draw(clock, new Rectangle(c1, c2 + adj, Engine.TILE_HEIGHT / 2, Engine.TILE_HEIGHT / 2), Color.White);

            string h = "", m = "", s = "";
            if (hr < 10)
            {
                h = "0";
            }
            h += hr.ToString();
            if (min < 10)
            {
                m = "0";
            }
            m += min.ToString();
            if (sec < 10)
            {
                s = "0";
            }
            s += sec.ToString();

            spriteBatch.DrawString(arial9pt, h + ":" + m + ":" + s, new Vector2(clockX, c2), Color.White);

            spriteBatch.End();
        }
        
        public void reset()
        {
            for (int i = 0; i < 10; i++)
            {
                sm[i] = "";
                colors[i] = Color.Black;
            }

            totalTimeInSeconds = hr = min = sec = 0;
        }
    }
}
