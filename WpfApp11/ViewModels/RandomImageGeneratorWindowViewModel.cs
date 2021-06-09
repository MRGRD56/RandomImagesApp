using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using WpfApp11.Extensions;
using WpfApp11.Models;

namespace WpfApp11.ViewModels
{
    public class RandomImageGeneratorWindowViewModel : BaseViewModel
    {
        private ImageSource _generatedImageSource;
        private string _generatingTime;
        private string _imageSize;
        private string _convertingToBytesTime;
        private string _convertingToImageSourceTime;
        private int _generatingTotalProgress;
        private int _generatingProgress;

        public int ImageWidth { get; set; } = 240;

        public int SquaresCount { get; set; } = 5;

        public ImageSource GeneratedImageSource
        {
            get => _generatedImageSource;
            set
            {
                if (Equals(value, _generatedImageSource)) return;
                _generatedImageSource = value;
                OnPropertyChanged();
            }
        }

        public Bitmap GeneratedBitmap { get; set; }

        public string ImageSize
        {
            get => _imageSize;
            set
            {
                if (value == _imageSize) return;
                _imageSize = value;
                OnPropertyChanged();
            }
        }

        public string GeneratingTime
        {
            get => _generatingTime;
            set
            {
                if (value.Equals(_generatingTime)) return;
                _generatingTime = value;
                OnPropertyChanged();
            }
        }

        public string ConvertingToBytesTime
        {
            get => _convertingToBytesTime;
            set
            {
                if (value == _convertingToBytesTime) return;
                _convertingToBytesTime = value;
                OnPropertyChanged();
            }
        }

        public string ConvertingToImageSourceTime
        {
            get => _convertingToImageSourceTime;
            set
            {
                if (value == _convertingToImageSourceTime) return;
                _convertingToImageSourceTime = value;
                OnPropertyChanged();
            }
        }

        public ICommand GenerateCommand => new Command(async property =>
        {
            var generatingStopwatch = new Stopwatch();
            generatingStopwatch.Start();
            await Task.Run(() =>
            {
                GeneratedBitmap = BitmapExtensions.GetRandomBitmap(ImageWidth, SquaresCount, (progress, total) =>
                {
                    GeneratingProgress = progress;
                    GeneratingTotalProgress = total;
                });
            });
            generatingStopwatch.Stop();
            var convertingToBytesStopwatch = new Stopwatch();
            convertingToBytesStopwatch.Start();
            var bitmapBytes = GeneratedBitmap.GetBytes();
            convertingToBytesStopwatch.Stop();
            var convertingToImageSourceStopwatch = new Stopwatch();
            convertingToImageSourceStopwatch.Start();
            GeneratedImageSource = BitmapExtensions.GetImageSource(bitmapBytes);
            convertingToImageSourceStopwatch.Stop();
            GeneratingTime = MillisecondsToSecondsString(generatingStopwatch.ElapsedMilliseconds);
            ConvertingToBytesTime = MillisecondsToSecondsString(convertingToBytesStopwatch.ElapsedMilliseconds);
            ConvertingToImageSourceTime = MillisecondsToSecondsString(convertingToImageSourceStopwatch.ElapsedMilliseconds);
            ImageSize = NumbersExtensions.GetSizeSuffix(bitmapBytes.LongLength);
        });

        public int GeneratingProgress
        {
            get => _generatingProgress;
            set
            {
                if (value == _generatingProgress) return;
                _generatingProgress = value;
                OnPropertyChanged();
            }
        }

        public int GeneratingTotalProgress
        {
            get => _generatingTotalProgress;
            set
            {
                if (value == _generatingTotalProgress) return;
                _generatingTotalProgress = value;
                OnPropertyChanged();
            }
        }

        private static string MillisecondsToSecondsString(long ms) => 
            (ms / 1000D).ToString("N3") + " c";
    }
}
