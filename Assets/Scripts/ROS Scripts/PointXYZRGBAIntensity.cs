using System.ComponentModel;
using System;
using PointCloud.PointTypes.Converters;

namespace PointCloud
{
//[TypeConverter(typeof(PointXYZRGBAIntensityConverter))]
	public class PointXYZRGBAIntensity : PointXYZRGBA
	{
		public PointXYZRGBAIntensity() : base()
		{
            Intensity = 0;
		}

		public PointXYZRGBAIntensity(float x, float y, float z, byte r, byte g, byte b, byte a, float intensity) : base(x, y, z, r, g, b, a)
		{
            Intensity = intensity;
		}

		public PointXYZRGBAIntensity(float x, float y, float z, uint rgba, float intensity) : base(x, y, z, rgba)
		{
            Intensity = intensity;
        }

        public float Intensity { get; set; }
	}
}
