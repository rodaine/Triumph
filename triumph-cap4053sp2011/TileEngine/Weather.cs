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
using TileEngine;

namespace TileEngine
{
    public enum WeatherTypes { rainy, sunny, snowy, cloudy, windy, dark };
	public class Weather
	{
        private int _duration;
        private WeatherTypes _currentWeather;
        public static Weather singleton;
        private WeatherTypes[] _types = { WeatherTypes.rainy, WeatherTypes.sunny, WeatherTypes.snowy, WeatherTypes.cloudy, WeatherTypes.windy, WeatherTypes.dark };

        /// <summary>
        /// creates a new weather class
        /// </summary>
        public Weather() 
        {
            _duration = RandomNumber.getInstance().getNext(3, 5);
            _currentWeather = _types[RandomNumber.getInstance().getNext(0,5)];
            
        }

        /// <summary>
        /// gets the time until weather changes
        /// </summary>
        public int duration
        {
            get{ return _duration;}
        }

        /// <summary>
        /// gets the current weather
        /// </summary>
        public WeatherTypes currentWeather
        {
            get{ return _currentWeather;}
        }

        /// <summary>
        /// ticks weather
        /// </summary>
        public void tick()
        {
            _duration--;
            if (_duration == 0)
            {
                _duration = RandomNumber.getInstance().getNext(8, 12);
                _currentWeather = _types[RandomNumber.getInstance().getNext(0,5)];
            }
        }


        /// <summary>
        /// gets singleton of weather
        /// </summary>
        /// <returns></returns>
        public static Weather getInstance()
        {
            if (singleton == null)
            {
                singleton = new Weather();
            }
            return singleton;
        }

	}
}
