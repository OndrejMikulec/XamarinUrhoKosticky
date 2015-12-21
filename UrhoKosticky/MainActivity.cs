using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;

using Urho;
using Urho.Droid;

namespace UrhoKosticky
{
	[Activity (Label = "UrhoKosticky", MainLauncher = true, Icon = "@mipmap/icon",
		ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation,
		ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : Activity
	{

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);

			button.Click += delegate {

				StartActivity(typeof(KostickyActivity));

			};
		}

		//TODO: 			
		/*Button quit = FindViewById<Button> (Resource.Id.buttonQuit);
		quit.Click += delegate {
			Finish();
		};*/
	}
}


