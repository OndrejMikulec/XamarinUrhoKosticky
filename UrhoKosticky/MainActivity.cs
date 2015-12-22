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

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;

using Urho;
using Urho.Droid;

namespace UrhoKosticky
{
	[Activity (Label = "Happy New Year 2016!", MainLauncher = true, Icon = "@mipmap/icon",
		ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation/*,
		ScreenOrientation = ScreenOrientation.Portrait*/)]
	public class MainActivity : Activity
	{
		public static CheckBox checkVibration;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.Main);

			Button button = FindViewById<Button> (Resource.Id.buttonStart);
			button.Click += delegate {

				StartActivity(typeof(KostickyActivity));

			};

			Button buttonAbout = FindViewById<Button> (Resource.Id.buttonAbout);
			buttonAbout.Click += delegate {

				StartActivity(typeof(About));

			};

			checkVibration = FindViewById<CheckBox> (Resource.Id.checkBox1);

			Button buttonQuit = FindViewById<Button> (Resource.Id.buttonQuit);
			buttonQuit.Click += delegate {

				Finish ();

			};
		}

	}
}


