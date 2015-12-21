using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;

using Urho;
using Urho.Droid;

namespace UrhoKosticky
{
	[Activity (Label = "Happy New Year 2016", MainLauncher = true, Icon = "@mipmap/icon",
		ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation/*,
		ScreenOrientation = ScreenOrientation.Portrait*/)]
	public class MainActivity : Activity
	{

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.buttonStart);
			button.Click += delegate {

				StartActivity(typeof(KostickyActivity));

			};

			Button buttonAbout = FindViewById<Button> (Resource.Id.buttonAbout);
			buttonAbout.Click += delegate {

				StartActivity(typeof(About));

			};


			Button buttonQuit = FindViewById<Button> (Resource.Id.buttonQuit);
			buttonQuit.Click += delegate {

				Finish ();

			};
		}

	}
}


