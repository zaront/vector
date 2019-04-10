using Anki.Vector.ExternalInterface;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Vector
{
	public class RobotScreen : RobotModule
	{
		public const int ScreenWidth = 184;
		public const int ScreenHeight = 96;

		internal RobotScreen(RobotConnection connection) : base(connection)
		{
		}

		public async Task SetEyeColor(float hue, float saturation, CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await Client.SetEyeColorAsync(new SetEyeColorRequest() { Hue = hue, Saturation = saturation }, cancellationToken: cancellationToken);
		}

		public async Task SetScreenColor(Color color, TimeSpan? duration = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			//convert color to rgb565 image
			var imageData = new byte[ScreenWidth * ScreenHeight * 2];
			var pixel = ConvertToRGB565(color.R, color.G, color.B);
			for (int i = 0; i < imageData.Length; i = i + 2)
			{
				imageData[i] = pixel.Item1;
				imageData[i + 1] = pixel.Item2;
			}
			var data = ByteString.CopyFrom(imageData);

			//send image
			await SetScreenImage(data, duration, cancellationToken);
		}

		public async Task SetScreenImage(string imagePath, TimeSpan? duration = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			//convert image
			ByteString data = null;
			using (var image = Image.FromFile(imagePath))
				data = ConvertToRGB565(image);

			//send image
			await SetScreenImage(data, duration, cancellationToken);
		}

		public async Task SetScreenImage(Stream imageStream, TimeSpan? duration = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			//convert image
			ByteString data = null;
			using (var image = Image.FromStream(imageStream))
				data = ConvertToRGB565(image);

			//send image
			await SetScreenImage(data, duration, cancellationToken);
		}

		ByteString ConvertToRGB565(Image image)
		{
			//resize
			if (image.Width != ScreenWidth || image.Height != ScreenHeight)
			{
				var resized = new Bitmap(ScreenWidth, ScreenHeight);
				using (var graphics = Graphics.FromImage(resized))
				{
					graphics.CompositingQuality = CompositingQuality.HighSpeed;
					graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
					graphics.CompositingMode = CompositingMode.SourceCopy;
					graphics.DrawImage(image, 0, 0, ScreenWidth, ScreenHeight);
				}
				image = resized;
			}

			//convert to rgb565 image
			var bitmap = image as Bitmap;
			if (bitmap == null)
				bitmap = new Bitmap(image);
			var imageData = new byte[ScreenWidth * ScreenHeight * 2];
			var index = 0;
			for (int y = 0; y < ScreenHeight; y++)
			{
				for (int x = 0; x < ScreenWidth; x++)
				{
					var pixel = bitmap.GetPixel(x, y);
					var newPixel = ConvertToRGB565(pixel.R, pixel.G, pixel.B);
					imageData[index] = newPixel.Item1;
					imageData[index + 1] = newPixel.Item2;
					index = index + 2;
				}
			}

			return ByteString.CopyFrom(imageData);
		}

		(byte,byte) ConvertToRGB565(byte red, byte green, byte blue)
		{
			var color = (red << 16) + (green << 8) + blue; //shift to 32 bit int
			color = ((color & 0xf80000) >> 8) + ((color & 0xfc00) >> 5) + ((color & 0xf8) >> 3); //mask and shift to rgb565
			return ((byte)((color & 0xff00) >> 8), (byte)(color & 0xff)); //return first and second byte
		}

		async Task SetScreenImage(ByteString imageData, TimeSpan? duration = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var durationMS = duration == null ? 0 : duration.Value.TotalMilliseconds;
			var result = await Client.DisplayFaceImageRGBAsync(new DisplayFaceImageRGBRequest() { DurationMs = (uint)durationMS, FaceData = imageData, InterruptRunning = true }, cancellationToken: cancellationToken);
		}

	}
}
