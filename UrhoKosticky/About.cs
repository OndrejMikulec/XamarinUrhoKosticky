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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace UrhoKosticky
{
	[Activity (Label = "About")]			
	public class About : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.About);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.buttonBack);
			button.Click += delegate {

				Finish ();

			};

			TextView text = FindViewById<TextView> (Resource.Id.textView1);
			text.Text = @"
This is absolutely free open code application!
You can find the source code at: https://github.com/OndrejMikulec/XamarinUrhoKosticky
Application compiled with parts of code from: https://github.com/xamarin/urho-samples
Sky box textures are from:  https://93i.de/p/free-skybox-texture-set/ created by Heiko Irrgang.
Developed with Xamarin INDIE software. https://xamarin.com/
Developed with Urho3D game engine: http://urho3d.github.io/

Author: Ondrej Mikulec,
		https://github.com/OndrejMikulec/";
		}
	}
}

