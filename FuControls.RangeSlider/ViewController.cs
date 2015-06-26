using System;

using UIKit;

namespace FuControls.RangeSlider
{
	public partial class ViewController : UIViewController
	{
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

			var uilabel = new UILabel (new CoreGraphics.CGRect (10, 10, View.Bounds.Width, 40));

			var fuRangeSlider = new FuRangeSlider (new CoreGraphics.CGRect (0, 50, View.Bounds.Width, 50));
			fuRangeSlider.Increment = 5;
			fuRangeSlider.MaxValue = 100;
			fuRangeSlider.MinValue = 0;
			fuRangeSlider.LowValue = 25;
			fuRangeSlider.HighValue = 75;

			fuRangeSlider.Curvaceousness = 1f;

			fuRangeSlider.KnobColor = UIColor.White;
			fuRangeSlider.TrackColor = UIColor.Gray;
			fuRangeSlider.TrackHighlightColor = UIColor.Green;
			fuRangeSlider.SlidersChanged += (sender, e) => { uilabel.Text = string.Format("{0} - {1}", fuRangeSlider.LowValue, fuRangeSlider.HighValue); };

			View.Add (uilabel);
			View.Add (fuRangeSlider);
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

