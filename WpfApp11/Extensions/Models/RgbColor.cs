using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp11.Extensions.Models
{
    public struct RgbColor
    {
        public byte R;

        public byte G;

        public byte B;

        public RgbColor(byte r, byte g, byte b) => (R, G, B) = (r, g, b);

        public RgbColor(byte[] rgb)
        {
            if (rgb.Length != 3)
            {
                throw new ArgumentOutOfRangeException(nameof(rgb), "RgbColor(byte[] rgb): rgb.Length must be 3");
            }

            (R, G, B) = (rgb[0], rgb[1], rgb[2]);
        }

        public byte[] GetRgbBytes() => new[] { R, G, B };

        public byte[] GetBgrBytes() => new[] { B, G, R };

        public void SetRgbBytes(ref byte[] bytes, int index) => SetBytes(GetRgbBytes(), ref bytes, index);
        public void SetBgrBytes(ref byte[] bytes, int index) => SetBytes(GetBgrBytes(), ref bytes, index);

        private void SetBytes(byte[] fromBytes, ref byte[] toBytes, int toBytesIndex)
        {
            //if (toBytes.Length <= toBytesIndex + 2) return;
            toBytes[toBytesIndex + 0] = fromBytes[0];
            toBytes[toBytesIndex + 1] = fromBytes[1];
            toBytes[toBytesIndex + 2] = fromBytes[2];
        }

        private static readonly Lazy<Random> Random = new Lazy<Random>();

        public static RgbColor GetRandomColor()
        {
            var bytes = new byte[3];
            Random.Value.NextBytes(bytes);
            return new RgbColor(bytes);
        }
    }
}
