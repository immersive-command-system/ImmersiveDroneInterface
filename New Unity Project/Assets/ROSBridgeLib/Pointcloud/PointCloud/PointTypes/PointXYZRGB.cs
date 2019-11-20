using System.ComponentModel;
using System;
using PointCloud.PointTypes.Converters;

namespace PointCloud
{
	[TypeConverter(typeof(PointXYZRGBConverter))]
	public class PointXYZRGB : PointXYZ
	{
		public PointXYZRGB() : base()
		{
			R = 0;
			G = 0;
			B = 0;
		}

		public PointXYZRGB(float x, float y, float z, byte r, byte g, byte b) : base(x, y, z)
		{
			R = r;
			G = g;
			B = b;
		}

		public PointXYZRGB(float x, float y, float z, float rgb) : base(x, y, z)
		{
			Byte[] bytes = BitConverter.GetBytes (rgb);
			R = bytes[2];
			G = bytes[1];
			B = bytes[0];
		}

		public byte R { get; set; }
		public byte G { get; set; }
		public byte B { get; set; }
	}
}
