using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfApp11.Extensions.Models;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace WpfApp11.Extensions
{
    public static class BitmapExtensions
    {
        public delegate void ProgressAction(int progress, int totalCount);

        /// <summary>
        /// Creates a random image with randomly colored squares.
        /// </summary>
        /// <param name="size">Width and height of the image.</param>
        /// <param name="inlineSquaresCount">Squares per line.</param>
        /// <param name="progressAction">The delegate that is called when writing one pixel of the image.</param>
        /// <returns></returns>
        public static Bitmap GetRandomBitmap(int size = 250, int inlineSquaresCount = 5, ProgressAction progressAction = null)
        {
            var width = size;
            var height = size;

            var bitmap = new Bitmap(width, height);
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            var imageLength = bitmapData.Stride * bitmapData.Height;
            var imageBytes = new byte[imageLength];
            var oneSquareSize = (double)width / inlineSquaresCount;

            width = Convert.ToInt32(Math.Floor(oneSquareSize * inlineSquaresCount));
            height = width;

            var progress = 0;
            var pixelsCount = width * height;

            for (var y = 0D; y < height; y += oneSquareSize)
            {
                for (var x = 0D; x < width; x += oneSquareSize)
                {
                    var color = RgbColor.GetRandomColor();
                    for (var squareY = y; squareY < y + oneSquareSize && squareY < height; squareY++)
                    {
                        for (var squareX = x; squareX < x + oneSquareSize && squareX < width; squareX++)
                        {
                            var byteIndex = Convert.ToInt32(Math.Floor(squareY)) * bitmapData.Stride +
                                            Convert.ToInt32(Math.Floor(squareX)) * 3;
                            color.SetBgrBytes(ref imageBytes, byteIndex);

                            progress++;
                            //Обновляем отображаемый прогресс каждые 250 обновлённых пикселей и в самом конце.
                            if (progress % 250 == 0 || progress >= pixelsCount)
                            {
                                progressAction?.Invoke(progress, pixelsCount);
                            }
                        }
                    }
                }
            }

            Marshal.Copy(imageBytes, 0, bitmapData.Scan0, imageLength);
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }

        public static byte[] GetBytes(this Bitmap bitmap)
        {
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Png);
                bytes = memoryStream.ToArray();
            }

            return bytes;
        }

        public static ImageSource GetImageSource(byte[] imageBytes)
        {
            var image = new BitmapImage();
            using (var imageStream = new MemoryStream(imageBytes))
            {
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = imageStream;
                image.EndInit();
            }

            return image;
        }
    }
}
