using System.ComponentModel;
using System;
using PointCloud.PointTypes.Converters;

namespace PointCloud
{
//[TypeConverter(typeof(PointXYZRGBAConverter))]
	public class PointXYZRGBA : PointXYZRGB
	{
		public PointXYZRGBA() : base()
		{
            A = 0;
		}

		public PointXYZRGBA(float x, float y, float z, byte r, byte g, byte b, byte a) : base(x, y, z, r, g, b)
		{
            A = a;
		}

		public PointXYZRGBA(float x, float y, float z, uint rgba) : base(x, y, z, rgba)
		{
			Byte[] bytes = BitConverter.GetBytes (rgba);
            A = bytes[3];
		}

        public byte A { get; set; }
	}
}
