using System;
using CoreAnimation;
using CoreGraphics;
using UIKit;

namespace FuControls
{
	public class FuRangeSliderTrackLayer : CALayer
	{
		public FuRangeSlider Slider {
			get;
			set;
		}

		public FuRangeSliderTrackLayer ()
		{
		}

		public override void DrawInContext (CGContext ctx)
		{
			base.DrawInContext (ctx);

			// clip
			var cornerRadius = Bounds.Height * Slider.Curvaceousness / 2.0f;
			UIBezierPath switchOutline =  UIBezierPath.FromRoundedRect( (CGRect)Bounds, (nfloat)cornerRadius);
			ctx.AddPath (switchOutline.CGPath);
			ctx.Clip ();

			// 1) fill the track
			ctx.SetFillColor (Slider.TrackColor.CGColor);
			ctx.AddPath(switchOutline.CGPath);
			ctx.FillPath ();

			// 2) fill the highlighed range
			ctx.SetFillColor(Slider.TrackHighlightColor.CGColor);
			var lower = Slider.positionForValue (Slider.LowValue);
			var higher = Slider.positionForValue(Slider.HighValue);
			ctx.FillRect((CGRect)new CGRect(lower, 0, higher - lower, Bounds.Height));

			// 3) add a highlight over the track
			CGRect highlight = new CGRect(cornerRadius/2, Bounds.Height/2,
				Bounds.Width - cornerRadius, Bounds.Height/2);
			UIBezierPath highlightPath = UIBezierPath.FromRoundedRect ((CGRect)highlight, (nfloat)highlight.Height * Slider.Curvaceousness / 2.0f);
			ctx.AddPath(highlightPath.CGPath);
			ctx.SetFillColor( UIColor.FromWhiteAlpha((nfloat)1.0f, (nfloat)0.4f).CGColor);
			ctx.FillPath ();

			// 4) inner shadow
			ctx.SetShadow( new CGSize(0f, 2.0f), 3.0f, UIColor.Gray.CGColor);
			ctx.AddPath (switchOutline.CGPath);
			ctx.SetStrokeColor(UIColor.Gray.CGColor);
			ctx.StrokePath ();

			// 5) outline the track
			ctx.AddPath( switchOutline.CGPath);
			ctx.SetStrokeColor(UIColor.Black.CGColor);
			ctx.SetLineWidth ((nfloat)0.5f);
			ctx.StrokePath ();
		}
	}
}

