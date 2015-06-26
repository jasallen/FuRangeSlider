using System;
using CoreAnimation;
using CoreGraphics;
using UIKit;

namespace FuControls
{
	public class FuRangeSliderKnobLayer : CALayer
	{
		public FuRangeSlider Slider {
			get;
			set;
		}

		public bool Highlighted {
			get;
			set;
		}

		public CGRect PaintFrame {
			get;
			set;
		}

		public FuRangeSliderKnobLayer ()
		{
		}

		public override void DrawInContext (CoreGraphics.CGContext ctx)
		{
			base.DrawInContext (ctx);

			var knobFrame = CGRect.Inflate(PaintFrame, -2.0f, -2.0f);

			UIBezierPath knobPath = UIBezierPath.FromRoundedRect((CGRect)knobFrame, (nfloat)knobFrame.Height * Slider.Curvaceousness / 2.0f);

			// 1) fill - with a subtle shadow
			ctx.SetShadow(new CGSize(0, 1), 1.0f, UIColor.Gray.CGColor);
			ctx.SetFillColor( Slider.KnobColor.CGColor);
			ctx.AddPath( knobPath.CGPath);
			ctx.FillPath ();

			// 2) outline
			ctx.SetStrokeColor(UIColor.Gray.CGColor);
			ctx.SetLineWidth((nfloat)0.5f);
			ctx.AddPath(knobPath.CGPath);
			ctx.StrokePath ();


			// 3) inner gradient
			var rect = CGRect.Inflate(knobFrame, -2.0f, -2.0f);
			var clipPath = UIBezierPath.FromRoundedRect ((CGRect)rect, (nfloat)rect.Height * Slider.Curvaceousness / 2.0f);

			CGGradient myGradient;
			CGColorSpace myColorspace;

			nfloat[] locations = { 0.0f, 1.0f };
			nfloat[] components = { 0.0f, 0.0f, 0.0f , 0.15f,  // Start color
				0.0f, 0.0f, 0.0f, 0.05f }; // End color

			myColorspace = CGColorSpace.CreateDeviceRGB (); // CGColorSpaceCreateDeviceRGB();
			myGradient = new CGGradient( myColorspace, components, locations);

			CGPoint startPoint = new CGPoint((float)rect.GetMidX(), (float)rect.GetMinY());
			CGPoint endPoint = new CGPoint((float)rect.GetMidX(), (float)rect.GetMaxY());

			ctx.SaveState ();
			ctx.AddPath( clipPath.CGPath);
			ctx.Clip ();
			ctx.DrawLinearGradient( (CGGradient)myGradient, (CGPoint)startPoint, (CGPoint)endPoint, (CGGradientDrawingOptions)0);

			myGradient.Dispose ();
			myColorspace.Dispose();
			ctx.RestoreState();

			// 4) highlight
			if (Highlighted)
			{
				// fill
				ctx.SetFillColor(UIColor.FromWhiteAlpha((nfloat)0.0f, (nfloat)0.1f).CGColor);
				ctx.AddPath( knobPath.CGPath);
				ctx.FillPath();
			}
		}
	}
}

