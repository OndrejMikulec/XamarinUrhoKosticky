
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

