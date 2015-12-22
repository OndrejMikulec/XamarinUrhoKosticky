
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
//using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;

using Urho;
using Urho.Droid;

namespace UrhoKosticky
{
	[Activity (Label = "Happy New Year 2016",
		ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation,
		ScreenOrientation = ScreenOrientation.Landscape)]
	public class KostickyActivity : Activity
	{
		public static Vibrator oVibrator;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			var mLayout = new AbsoluteLayout(this);
			UrhoEngine.Init();
			var surface = UrhoSurface.CreateSurface<Kosticky>(this);
			mLayout.AddView(surface);
			SetContentView(mLayout);

			try {
				oVibrator = (Vibrator)this.GetSystemService (Android.Content.Context.VibratorService);
			} catch  {}
		}

		protected override void OnResume()
		{
			UrhoSurface.OnResume();
			base.OnResume();
		}

		protected override void OnPause()
		{
			UrhoSurface.OnPause();
			base.OnPause();
		}

		public override void OnLowMemory()
		{
			UrhoSurface.OnLowMemory();
			base.OnLowMemory();
		}

		protected override void OnDestroy()
		{
			UrhoSurface.OnDestroy();
			base.OnDestroy();
		}

		public override bool DispatchKeyEvent(KeyEvent e)
		{
			if (!UrhoSurface.DispatchKeyEvent(e))
				return false;
			return base.DispatchKeyEvent(e);
		}

		public override void OnWindowFocusChanged(bool hasFocus)
		{
			UrhoSurface.OnWindowFocusChanged(hasFocus);
			base.OnWindowFocusChanged(hasFocus);
		}

		public const int vibrateSmall = 20;
		public const int vibrateBig = 50;
		public static void vibrate(int time)
		{
			if (oVibrator!=null && oVibrator.HasVibrator && MainActivity.checkVibration.Checked) {
				oVibrator.Vibrate (time);
			}
		}
	}
}

