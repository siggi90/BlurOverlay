using System;
using Xamarin.Forms;
using System.Diagnostics;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Plugin.Screenshot;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
//using System.Geometry;
using System.Numerics;
using CubicBezierEasings;

namespace BlurOverlay {
	public class blur {
        AbsoluteLayout body_wrap;
        ContentPage page;

        Easing ease_in_3;
        Easing ease_out_2;

		StackLayout black_overlay = null;

		private uint ease_in_time = 1250;
		private uint ease_out_time = 720;

        public blur(ContentPage page, StackLayout black_overlay = null, Easing ease_in = null, Easing ease_out = null, uint ease_in_time=1250, uint ease_out_time=720, float sigma_blur=15.0f, float sigma_x=15, float sigma_y=15) {
            this.page = page;
            this.body_wrap = this.page.FindByName<AbsoluteLayout>("body_wrap");

			easings easings = new easings();
			
            this.ease_in_3 = easings.bezier(0.02, 1, 0.76, 0.99);
            this.ease_out_2 = easings.bezier(0.64, 0.14, 0.23, 0.73);

			if(ease_in != null) {
				this.ease_in_3 = ease_in;
			}
			if(ease_out != null) {
				this.ease_out_2 = ease_out;
			}
			this.black_overlay = black_overlay;

			this.ease_in_time = ease_in_time;
			this.ease_out_time = ease_out_time;

			this.sigma_blur = sigma_blur;
			this.sigma_x = sigma_x;
			this.sigma_y = sigma_y;
        }



        SKBitmap page_bitmap;
		SKCanvasView blur_overlay;
		public void init_blur() {
			this.body_wrap = this.page.FindByName<AbsoluteLayout>("body_wrap");

			this.blur_overlay = this.page.FindByName<SKCanvasView>("blur_overlay");
			this.blur_overlay.PaintSurface += this.paint_blur;
		}

		public async Task<bool> make_blur() {
			MemoryStream stream = new MemoryStream(await CrossScreenshot.Current.CaptureAsync()); 
			byte[] bytes = stream.ToArray();
			this.page_bitmap = SKBitmap.Decode(stream);
			this.blur_overlay.InvalidateSurface();
			this.blur_overlay.SizeChanged += (s, e) => {
				this.page_bitmap = null;
				this.blur_overlay.InvalidateSurface();
				//this.make_blur();
				this.blur_overlay.BackgroundColor = Color.Black;
			};
			return true;
		}

		public async Task<bool> display_blur() {
			this.blur_overlay.IsVisible = true;
			if(this.black_overlay != null) {
				await this.black_overlay.FadeTo(0, 0);
				this.black_overlay.IsVisible = true;
				this.black_overlay.FadeTo(1, this.ease_in_time, this.ease_in_3);
			}
			await this.blur_overlay.FadeTo(1, this.ease_in_time, this.ease_in_3);
			return true;
		}

		public async Task<bool> hide_blur() {
			if(this.black_overlay != null) {
				this.black_overlay.FadeTo(0, this.ease_out_time, this.ease_out_2);
			}
			await this.blur_overlay.FadeTo(0, this.ease_out_time, this.ease_out_2);
			this.blur_overlay.IsVisible = false;
			this.black_overlay.IsVisible = false;
			return true;
		}

		private float sigma_blur = 25.0f;
		private float sigma_x = 25;
		private float sigma_y = 25;

		SKBitmap last_image = null;

		public void paint_blur(object sender, SKPaintSurfaceEventArgs e) {

			// we get the current surface from the event args
			var surface = e.Surface;
			// then we get the canvas that we can draw on
			var canvas = surface.Canvas;
			// clear the canvas / view
			var circleFill = new SKPaint {
				IsAntialias = true,
				Style = SKPaintStyle.Fill,
				Color = SKColors.Blue
			};
 
			// draw the circle fill
			//canvas.DrawCircle(100, 100, 40, circleFill);

			//SKMaskFilter mask = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 50);
			SKMaskFilter mask = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, this.sigma_blur);
			SKImageFilter image_filter = SKImageFilter.CreateBlur(this.sigma_x, this.sigma_y);

			//var snap = surface.Snapshot();
			SKBitmap snap = this.page_bitmap;
			
			if(snap != null) {
				// set up the blur
				var filter = SKImageFilter.CreateBlur(5, 5);
				var paint = new SKPaint {
					ImageFilter = filter
				};
				
				/*if(snap != this.last_image) {
					//SKSize size = new SKSize(Convert.ToInt32(this.blur_overlay.Width), Convert.ToInt32(this.blur_overlay.Height));
					SKImageInfo size = new SKImageInfo();
					size = size.WithSize(Convert.ToInt32(this.blur_overlay.Width), Convert.ToInt32(this.blur_overlay.Height));
					snap.Resize(size, SKBitmapResizeMethod.Box);
				}*/
				// draw the blurry screen
				//canvas.DrawImage(snap, 0, 0, paint);
				canvas.DrawBitmap(snap, 0, 0, paint);
				/*SKPaint black = new SKPaint() {
					Color = new SKColor(0, 0, 0, 100)
				};
				canvas.DrawPaint(black);*/
				//this.last_image = snap;
			} else {
			}
		}

    }
}
