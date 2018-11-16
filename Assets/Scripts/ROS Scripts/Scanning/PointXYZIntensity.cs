
namespace PointCloud
{

    public class PointXYZIntensity : PointXYZ
    {
        public float intensity { get; set; }

        public PointXYZIntensity() : base()
        {
            this.intensity = 0;
        }

        public PointXYZIntensity(float x, float y, float z, float intensity) : base(x, y, z)
        {
            this.intensity = intensity;
        }
    }
}
