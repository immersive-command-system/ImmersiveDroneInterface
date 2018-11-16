using PointCloud;
using UnityEngine;

public class PointCloudVisualizer : MonoBehaviour {

    private ParticleSystem ps;
    private PointCloud<PointXYZIntensity> cloud = null;
    private bool updated = false;
    ParticleSystem.Particle[] currParticles;

    // Use this for initialization
    void Start () {
        ps = GetComponent<ParticleSystem>();
        currParticles = new ParticleSystem.Particle[ps.particleCount];
        ps.GetParticles(currParticles);
    }
	
	// Update is called once per frame
	void Update () {
        if (updated && cloud != null)
        {
            updated = false;
            if (currParticles.Length < cloud.Size)
            {
                int oldSize = currParticles.Length;
                currParticles = new ParticleSystem.Particle[cloud.Size];
                ps.GetParticles(currParticles);
                for (int i = oldSize; i < currParticles.Length; i++)
                {
                    currParticles[i] = new ParticleSystem.Particle();
                }
            }
            int ind = 0;
            foreach (PointXYZIntensity point in cloud.Points)
            {
                currParticles[ind].position = new Vector3(point.X, -point.Z, point.Y);
                currParticles[ind].startSize = 0.1f;
                ind += 1;
            }
            ps.SetParticles(currParticles, currParticles.Length);
        }
	}

    public void SetPointCloud(PointCloud<PointXYZIntensity> newCloud)
    {
        this.cloud = newCloud;
        updated = true;
    }
    
}
