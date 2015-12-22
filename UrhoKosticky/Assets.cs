/*
The MIT License (MIT)

Copyright (c) 2015 Ondrej Mikulec
o.mikulec@seznam.cz
Vsetin, Czech Republic

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
	SOFTWARE.
*/

using System;

namespace UrhoKosticky
{
	public static  class Assets
	{
		public static class Materials
		{
			public const string gray = "Materials/M_0132_LightGray.xml";
			public const string yellow = "Materials/M_0056_Yellow.xml";
			public const string red = "Materials/M_0020_Red.xml";
			public const string green = "Materials/M_0060_GrassGreen.xml";
			public const string blue = "Materials/M_0103_Blue.xml";
			public const string orange = "Materials/M_0039_DarkOrange.xml";

			public const string stone = "Materials/StoneEnvMapSmall.xml";
			public const string terrain = "Materials/Terrain.xml";

			public const string skyBoxSunSet = "Materials/SkyboxSunSet.xml";
			public const string skyBoxFullMoon = "Materials/SkyboxFullMoon.xml";
		}

		public static class Models
		{
			public const string box = "Models/Box.mdl";
			public const string sphere = "Models/Sphere.mdl";
			public const string Player = "Models/Player.mdl";
			public const string Enemy1 = "Models/Enemy1.mdl";
			public const string Enemy2 = "Models/Enemy2.mdl";
			public const string Enemy3 = "Models/Enemy3.mdl";
			public const string Enemy3weapon = "Models/Enemy3weapon.mdl";
			public const string Coin = "Models/Coin.mdl";
			public const string SMWeapon = "Models/SMWeapon.mdl";
		}

		public static class UI
		{
			public const string myJoystick = "UI/ScreenJoystickMy.xml";
			public const string defaultStyle = "UI/DefaultStyle.xml";
			public const string myJoystickNoMega = "UI/ScreenJoystickMyNoMega.xml";
		}


		public static class Fonts
		{
			public const string Font = "Fonts/Anonymous Pro.ttf";
		}
	}
}

