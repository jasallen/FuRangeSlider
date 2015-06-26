using System;
using UIKit;
using CoreGraphics;
using CoreAnimation;

namespace FuControls
{
    public class FuRangeSlider : UIControl
    {
        public event EventHandler LowerSliderChanged;
        public event EventHandler UpperSliderChanged;

        public event EventHandler SlidersChanged;

        CGPoint _previousTouchPoint;

        FuRangeSliderTrackLayer _trackLayer;
        FuRangeSliderKnobLayer _upperKnobLayer;
        FuRangeSliderKnobLayer _lowerKnobLayer;

        nfloat RawHighValue;

        nfloat RawLowValue;

        nfloat _knobWidth;
        nfloat _useableTrackLength;

        float minValue;
        public float MinValue
        {
            get
            {
                return minValue;
            }
            set
            {
                minValue = value;
                setLayerFrames();
            }
        }

        float maxValue;
        public float MaxValue
        {
            get
            {
                return maxValue;
            }
            set
            {
                maxValue = value;
                setLayerFrames();
            }
        }

        nfloat lowValue;
        public nfloat LowValue
        {
            get
            {
                return lowValue;
            }
            set
            {
                lowValue = value;
                setLayerFrames();
                if (LowerSliderChanged != null) LowerSliderChanged(this, null);
            }
        }

        nfloat highValue;
        public nfloat HighValue
        {
            get
            {
                return highValue;
            }
            set
            {
                highValue = value;
                setLayerFrames();
                if (UpperSliderChanged != null) UpperSliderChanged(this, null);
            }
        }

        int increment;
        public int Increment
        {
            get
            {
                return increment;
            }
            set
            {
                increment = value;
            }
        }

        UIColor trackColor;
        public UIColor TrackColor
        {
            get
            {
                return trackColor;
            }
            set
            {
                trackColor = value;
                redrawLayers();
            }
        }

        UIColor trackHighlightColor;
        public UIColor TrackHighlightColor
        {
            get
            {
                return trackHighlightColor;
            }
            set
            {
                trackHighlightColor = value;
                redrawLayers();
            }
        }

        UIColor knobColor;
        public UIColor KnobColor
        {
            get
            {
                return knobColor;
            }
            set
            {
                knobColor = value;
                redrawLayers();
            }
        }

        float curvaceousness;
        public float Curvaceousness
        {
            get
            {
                return curvaceousness;
            }
            set
            {
                curvaceousness = value;
                redrawLayers();
            }
        }

        public FuRangeSlider()
            : base()
        {
            init();
        }

        public FuRangeSlider(CGRect frame)
            : base(frame)
        {
            init();
        }

        private void init()
        {

            _trackLayer = new FuRangeSliderTrackLayer();
            _trackLayer.Slider = this;
            Layer.AddSublayer(_trackLayer);

            _upperKnobLayer = new FuRangeSliderKnobLayer();
            _upperKnobLayer.Slider = this;
            Layer.AddSublayer(_upperKnobLayer);

            _lowerKnobLayer = new FuRangeSliderKnobLayer();
            _lowerKnobLayer.Slider = this;
            Layer.AddSublayer(_lowerKnobLayer);

            minValue = 0;
            maxValue = 10;
            lowValue = 2;
            highValue = 8;
            increment = 1;

            TrackHighlightColor = UIColor.FromRGB(0, 0.45f, 0.94f);
            TrackColor = UIColor.FromWhiteAlpha((nfloat)0.9f, (nfloat)1);
            KnobColor = UIColor.White;
            Curvaceousness = 1.0f;

            setLayerFrames();

            UpperSliderChanged += (sender, e) => { if (SlidersChanged != null) SlidersChanged(sender, e); };
            LowerSliderChanged += (sender, e) => { if (SlidersChanged != null) SlidersChanged(sender, e); };
        }

        private void setLayerFrames()
        {
            _trackLayer.Frame = CGRect.Inflate(Bounds, 0f, Bounds.Height / 3.5f * -1f);
            _trackLayer.SetNeedsDisplay();

            _knobWidth = Bounds.Height;
            _useableTrackLength = Bounds.Width - _knobWidth;

            var upperKnobCentre = positionForValue(HighValue);
            _upperKnobLayer.Frame = new CGRect(0, 0, Bounds.Width, _knobWidth);
            _upperKnobLayer.PaintFrame = new CGRect(upperKnobCentre - _knobWidth / 2, 0, _knobWidth, _knobWidth);

            var lowerKnobCentre = positionForValue(LowValue);
            _lowerKnobLayer.Frame = new CGRect(0, 0, Bounds.Width, _knobWidth);
            _lowerKnobLayer.PaintFrame = new CGRect(lowerKnobCentre - _knobWidth / 2, 0, _knobWidth, _knobWidth);

            _upperKnobLayer.SetNeedsDisplay();
            _lowerKnobLayer.SetNeedsDisplay();

        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();


            UpperSliderChanged(this, new EventArgs());
            LowerSliderChanged(this, new EventArgs());
        }

        internal nfloat positionForValue(nfloat value)
        {
            return _useableTrackLength * (value - MinValue) /
                (MaxValue - MinValue) + (_knobWidth / 2);
        }

        public override bool BeginTracking(UITouch touch, UIEvent uievent)
        {

            _previousTouchPoint = (CGPoint)touch.LocationInView((UIView)this);

            // hit test the knob layers
            if (_lowerKnobLayer.PaintFrame.Contains(_previousTouchPoint))
            {
                _lowerKnobLayer.Highlighted = true;
                _lowerKnobLayer.SetNeedsDisplay();
            }
            else if (_upperKnobLayer.PaintFrame.Contains(_previousTouchPoint))
            {
                _upperKnobLayer.Highlighted = true;
                _upperKnobLayer.SetNeedsDisplay();
            }

            RawHighValue = HighValue;
            RawLowValue = LowValue;

            return _upperKnobLayer.Highlighted || _lowerKnobLayer.Highlighted;
        }

        public override bool ContinueTracking(UITouch touch, UIEvent uievent)
        {
            CGPoint touchPoint = (CGPoint)touch.LocationInView((UIView)this);

            // 1. determine by how much the user has dragged
            var delta = touchPoint.X - _previousTouchPoint.X;
            var valueDelta = (MaxValue - MinValue) * delta / _useableTrackLength;

            _previousTouchPoint = touchPoint;

            // 2. update the values
            if (_lowerKnobLayer.Highlighted)
            {
                RawLowValue += valueDelta;
                RawLowValue = (nfloat)Math.Min(Math.Max(RawLowValue, MinValue), HighValue);  //(_lowerValue, _upperValue, _minimumValue);

                if (increment > 0)
                {
                    LowValue = (int)(RawLowValue / increment) * increment;
                }
                else
                {
                    LowValue = RawLowValue;
                }
            }
            if (_upperKnobLayer.Highlighted)
            {
                RawHighValue += valueDelta;
                RawHighValue = (nfloat)Math.Max(Math.Min(RawHighValue, MaxValue), LowValue);

                if (increment > 0)
                {
                    HighValue = (int)(RawHighValue / increment) * increment;
                }
                else
                {
                    HighValue = RawHighValue;
                }
            }

            // 3. Update the UI state
            CATransaction.Begin();
            CATransaction.DisableActions = true;

            setLayerFrames();

            CATransaction.Commit();

            if (_lowerKnobLayer.Highlighted)
            {
                LowerSliderChanged(this, new EventArgs());
            }
            else
            {
                UpperSliderChanged(this, new EventArgs());
            }

            if (_lowerKnobLayer.Highlighted || _upperKnobLayer.Highlighted)
                return true;

            return false;
            //return true;
        }


        public override void EndTracking(UITouch touch, UIEvent uievent)
        {
            RawLowValue = LowValue;
            RawHighValue = HighValue;

            _lowerKnobLayer.Highlighted = _upperKnobLayer.Highlighted = false;
            _lowerKnobLayer.SetNeedsDisplay();
            _upperKnobLayer.SetNeedsDisplay();
        }

        private void redrawLayers()
        {
            if (_upperKnobLayer != null) _upperKnobLayer.SetNeedsDisplay();
            if (_lowerKnobLayer != null) _lowerKnobLayer.SetNeedsDisplay();
            if (_trackLayer != null) _trackLayer.SetNeedsDisplay();
        }
    }
}

