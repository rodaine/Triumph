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
	class Weather
	{
        private int _duration;
        public enum WeatherTypes {rainy, sunny, snowy, cloudy, windy, dark};
        private WeatherTypes _currentWeather;
        public static Weather singleton;
        private WeatherTypes[] _types = { WeatherTypes.rainy, WeatherTypes.sunny, WeatherTypes.snowy, WeatherTypes.cloudy, WeatherTypes.windy, WeatherTypes.dark };

        public Weather() 
        {
            _duration = RandomNumber.getInstance().getNext(3, 5);
            _currentWeather = _types[RandomNumber.getInstance().getNext(0,5)];
            
        }

        public int duration
        {
            get{ return _duration;}
        }

        public WeatherTypes currentWeather
        {
            get{ return _currentWeather;}
        }


        public void tick()
        {
            _duration--;
            if (_duration == 0)
            {
                _duration = RandomNumber.getInstance().getNext(3, 5);
                _currentWeather = _types[RandomNumber.getInstance().getNext(0,5)];
            }
        }

        public Weather getInstance()
        {
            if (singleton == null)
            {
                singleton = new Weather();
            }
            return singleton;
        }

	}
}
