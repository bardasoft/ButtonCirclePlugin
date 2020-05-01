using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using ButtonCircle.FormsPlugin.Abstractions;
using ButtonCircle.FormsPlugin.Droid;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CircleButton), typeof(ButtonCircleRenderer))]

namespace ButtonCircle.FormsPlugin.Droid
{
    /// <summary>
    /// ButtonCircle Renderer
    /// </summary>
    [Preserve(AllMembers = true)]
    public class ButtonCircleRenderer : ButtonRenderer
    {

        public ButtonCircleRenderer(Context context) : base(context)
        {
        }
        /// <summary>
        /// Used for registration with dependency service
        /// </summary>
        public static void Init()
        {
            var temp = DateTime.Now;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="child"></param>
        /// <param name="drawingTime"></param>
        /// <returns></returns>
        protected override bool DrawChild(Canvas canvas, Android.Views.View child, long drawingTime)
        {
            try
            {
                var radius = Math.Min(Width, Height) / 2;

                var borderThickness = (float)((CircleButton)Element).BorderThickness;

                int strokeWidth = 0;

                if (borderThickness > 0)
                {
                    var logicalDensity = Context.Resources.DisplayMetrics.Density;
                    strokeWidth = (int)Math.Ceiling(borderThickness * logicalDensity + .5f);
                }

                radius -= strokeWidth / 2;

                var path = new Path();
                path.AddCircle(Width / 2.0f, Height / 2.0f, radius, Path.Direction.Ccw);

                canvas.Save();
                canvas.ClipPath(path);

                var paint = new Paint();
                paint.AntiAlias = true;
                paint.SetStyle(Paint.Style.Fill);
                canvas.DrawPath(path, paint);
                paint.Dispose();

                var result = base.DrawChild(canvas, child, drawingTime);

                path.Dispose();
                canvas.Restore();

                path = new Path();
                path.AddCircle(Width / 2, Height / 2, radius, Path.Direction.Ccw);

                if (strokeWidth > 0.0f)
                {
                    paint = new Paint();
                    paint.AntiAlias = true;
                    paint.StrokeWidth = strokeWidth;
                    paint.SetStyle(Paint.Style.Stroke);
                    paint.Color = ((CircleButton)Element).BorderColor.ToAndroid();
                    canvas.DrawPath(path, paint);
                    paint.Dispose();
                }

                path.Dispose();
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Unable to create circle button: " + ex);
            }

            return base.DrawChild(canvas, child, drawingTime);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                //Only enable hardware accelleration on lollipop
                if ((int)Android.OS.Build.VERSION.SdkInt < 21)
                {
                    SetLayerType(LayerType.Software, null);
                }
            }

            if (!String.IsNullOrEmpty(((CircleButton)Element).Icon))
            {
                Control.Typeface = Typeface.CreateFromAsset(Context.Assets,
                    Abstractions.Helpers.Extensions.FindNameFileForFont(((CircleButton)Element).FontIcon));

                IIcon icon = Abstractions.Helpers.Extensions.FindIconForKey(((CircleButton)Element).Icon,
                    ((CircleButton)Element).FontIcon);

                Element.Text = $"{icon.Character}";
            }
            else
            {
                Control.Typeface = Typeface.Create(Element.FontFamily, TypefaceStyle.Normal);
                Element.Text = ((CircleButton)Element).Text;
            }

            if (((CircleButton)Element).ImageSource != null)
            {
                Android.Widget.Button thisButton = Control as Android.Widget.Button;
                thisButton.Touch += (object sender, Android.Views.View.TouchEventArgs e2) =>
                {
                    if (e2.Event.Action == MotionEventActions.Down)
                    {
                        Control.SetBackgroundColor(Element.BackgroundColor.ToAndroid());
                    }
                    else if (e2.Event.Action == MotionEventActions.Up)
                    {
                        Control.SetBackgroundColor(Element.BackgroundColor.ToAndroid());
                        Control.SetShadowLayer(0, 0, 0, Android.Graphics.Color.Transparent);
                        Control.CallOnClick();
                    }
                };
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == CircleButton.BorderColorProperty.PropertyName ||
              e.PropertyName == CircleButton.BorderThicknessProperty.PropertyName ||
              e.PropertyName == CircleButton.IconProperty.PropertyName ||
              e.PropertyName == CircleButton.TextProperty.PropertyName ||
              e.PropertyName == CircleButton.FontIconProperty.PropertyName)
            {
                if (!String.IsNullOrEmpty(((CircleButton)Element).Icon))
                {
                    Control.Typeface = Typeface.CreateFromAsset(Context.Assets,
                    Abstractions.Helpers.Extensions.FindNameFileForFont(((CircleButton)Element).FontIcon));

                    IIcon icon = Abstractions.Helpers.Extensions.FindIconForKey(((CircleButton)Element).Icon,
                        ((CircleButton)Element).FontIcon);

                    Element.Text = $"{icon.Character}";
                }
                else
                {
                    Control.Typeface = Typeface.Create(Element.FontFamily, TypefaceStyle.Normal);
                    Element.Text = ((CircleButton)Element).Text;
                }

                this.Invalidate();
            }
        }
    }
}
