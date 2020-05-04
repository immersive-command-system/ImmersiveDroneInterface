using System.IO;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// A wrapper class around Unity's CPU-based particle system gameobject component.
/// It configures the particle system to be useful for displaying a general static point cloud.
/// </summary>
public class PointCloudVisualizer2 : MonoBehaviour
{
    /// <value> The initial capacity of this point cloud visualizer. Set this value to roughly how many points are expected.</value>
    public int initialParticleCount = 20000;

    /// <value> The actual Unity CPU-based particle system object that will take care of rendering the particles.</value>
    protected ParticleSystem cloud;
    /// <value> The renderer component of the particle system.</value>
    protected ParticleSystemRenderer cloud_renderer;
    /// <value> The array of particles that will be pushed to the particle system for rendering.</value>
    /// <remarks> Note: the size of the array may be greater than particle_count, but particle_count ultimately determines the number of particles shown.</remarks>
    protected ParticleSystem.Particle[] particles;
    /// <value> Number of particles in the particle system.</value>
    protected int particle_count = 0;
    
    /// <summary>
    /// Different update modes for the particle system.
    /// <list type="bullet">
    /// <item> TIMED: updates on a timer.</item>
    /// <item> ON_REFRESH: refrehses on demand (next render cycle after the point cloud is updated)</item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// Sometimes, ON_REFRESH is buggy on the first few cycles.
    /// The timed one will refresh even when the point cloud hasn't been updated.
    /// </remarks>
    public enum UpdateMode
    {
        TIMED,
        ON_REFRESH
    };
    private UpdateMode update_mode = UpdateMode.ON_REFRESH;
    /// <value>
    /// The timer for TIMED mode.
    /// Is reset to 0 every time the timer expires.
    /// </value>
    private float updateTimer = 0;
    /// <value> The interval between updates in seconds in TIMED mode.</value>
    public float updateInterval = 2;
    /// <value> Whether the underlying points have changed since the last particle system update.</value>
    protected bool hasChanged = false;

    /// <value>
    /// Whether the particle system is initialized and ready to render the points.
    /// While this field is false, the points will not be displayed yet.
    /// </value>
    private bool initialized = false;

    /// <value> The name of the shader to use for each particle.</value>
    //private string shader = "Standard";
    private string shader = "Particles/Standard Unlit";
    /// <summary>
    /// Do initialization work on the particle system. Configure it for displaying static point cloud data.
    /// This involves turning off a lot of its animation componenents and setting the materials/shaders.
    /// After this function is finished, the initialized flag is set to true.
    /// </summary>
    protected void Initialize()
    {
        // Ensure that there is a particle system component attached to this gameobject.
        cloud = GetComponent<ParticleSystem>();
        if (cloud == null)
        {
            cloud = gameObject.AddComponent<ParticleSystem>();
        }
        cloud_renderer = GetComponent<ParticleSystemRenderer>();

        InitializeParticleSystem(cloud, cloud_renderer, initialParticleCount);

        particles = new ParticleSystem.Particle[initialParticleCount];

        initialized = true;
    }

    void Update()
    {
        bool should_update = false;
        if (update_mode == UpdateMode.TIMED)
        {
            if (updateTimer >= updateInterval)
            {
                should_update = true;
                updateTimer = 0;
            }
            updateTimer += Time.deltaTime;
        }
        else if (update_mode == UpdateMode.ON_REFRESH)
        {
            should_update = hasChanged;
            hasChanged = false;
        }

        if (should_update)
        {
            cloud.SetParticles(particles, particle_count, 0);
        }

        // Debug purposes
        if (Input.GetKeyDown("space"))
        {
            ExportPointCloud();
        }
    }

    /// <summary>
    /// Change when the particle system should update.
    /// </summary>
    /// <param name="mode">The new update mode to change to.</param>
    public void SetUpdateMode(UpdateMode mode)
    {
        if (mode == UpdateMode.TIMED)
        {
            updateTimer = updateInterval;
        }
        update_mode = mode;
    }

    /// <summary>
    /// Add a single particle to the particle system.
    /// </summary>
    /// <param name="p">The new particle.</param>
    protected void AddParticle(ParticleSystem.Particle p)
    {
        if (!initialized)
        {
            Initialize();
        }

        // Ensure that the particle array has enough capacity for the new particle.
        ParticleSystem.Particle[] new_particles = particles;
        while (particle_count >= new_particles.Length)
        {
            new_particles = new ParticleSystem.Particle[new_particles.Length * 2];
        }
        // If a resize was needed, copy all particles over from old array to new array.
        if (new_particles != particles)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                new_particles[i] = particles[i];
            }
            particles = new_particles;
        }


        particles[particle_count] = p;
        particle_count++;

        hasChanged = true;
    }

    /// <summary>
    /// Call this method after manually changing the underlying array of particles.
    /// This will notify the update cycle that the particle system needs to be updated.
    /// </summary>
    protected void OnParticlesUpdated()
    {
        hasChanged = true;
    }

    /// <value> A holder for the results of the octree search.</value>
    private List<Color> results = new List<Color>();
    /// <summary>
    /// Apply a spatial color mapping to the points in the point cloud.
    /// </summary>
    /// <param name="colorMap">The bounding-box based octree defining the colors distribution of the space.</param>
    public void ApplyColorSpace(BoundsOctree<Color> colorMap)
    {
        // Search the octree for each particle in the point cloud.
        for (int i = 0; i < particle_count; i++)
        {
            // Search the octree using the current point.
            results.Clear();
            colorMap.GetColliding(results, new Bounds(particles[i].position, particles[i].startSize * Vector3.one));

            // Average the results.
            if (results.Count > 0)
            {
                Color c = results[0];
                for (int j = 1; j < results.Count; j++)
                {
                    c += results[j];
                }
                particles[i].startColor = c / results.Count;
            }
        }

        // Trigger particle system refresh.
        OnParticlesUpdated();
    }

    /// <summary>
    /// Apply a spatial color mapping to the points in the point cloud.
    /// </summary>
    /// <param name="colorMap">The dense, voxel-based defining the colors distribution of the space.</param>
    public void ApplyColorSpace(Color[, ,] colorMap, Vector3 origin, Vector3 voxelDimensions)
    {
        for (int i = 0; i < particle_count; i++)
        {
            Vector3 particlePos = particles[i].position;

            // Check that the point is within the voxel space's bounds.

            if (particlePos.x < origin.x ||
                particlePos.y < origin.y || 
                particlePos.z < origin.z)
            {
                continue;
            }
            // Find the indices of the voxel that the point belongs in.
            int ind_x = Mathf.RoundToInt((particlePos.x - origin.x) / voxelDimensions.x);
            int ind_y = Mathf.RoundToInt((particlePos.y - origin.y) / voxelDimensions.y);
            int ind_z = Mathf.RoundToInt((particlePos.z - origin.z) / voxelDimensions.z);
            if (ind_x >= colorMap.GetLength(0) || 
                ind_y >= colorMap.GetLength(1) || 
                ind_z >= colorMap.GetLength(2))
            {
                continue;
            }

            particles[i].startColor = colorMap[ind_x, ind_y, ind_z];
        }

        // Trigger a particle system update.
        OnParticlesUpdated();
    }


    /// <summary>
    /// Set the default emission color of particles.
    /// </summary>
    /// <param name="c">Default emission color.</param>
    /// <remarks> This may have different/no effects depending on the shader.</remarks>
    protected void SetEmissionColor(Color c)
    {
        Material particleMaterial = cloud_renderer.material;
        particleMaterial.SetColor("_EmissionColor", c);
    }

    /// <summary>
    /// Set the default albedo color of particles.
    /// </summary>
    /// <param name="c">Default albedo color.</param>
    /// <remarks> This may have different/no effects depending on the shader.</remarks>
    protected void SetColor(Color c)
    {
        Material particleMaterial = cloud_renderer.material;
        particleMaterial.SetColor("_Color", c);
    }

    /// <summary>
    /// Set whether the particles always face in the direction of the camera.
    /// </summary>
    /// <param name="follow">Whether particles should face the camera always.</param>
    protected void SetFollowCamera(bool follow)
    {
        cloud_renderer.alignment = (follow) ? ParticleSystemRenderSpace.Facing : ParticleSystemRenderSpace.World;
    }

    /// <summary>
    /// Change the rendering method of the particles.
    /// </summary>
    /// <param name="mode">The new rendering mode.</param>
    /// <param name="mesh">The mesh the particles should take on, if necessary (for ParticleSystemRenderMode.Mesh mode).</param>
    protected void SetRenderMethod(ParticleSystemRenderMode mode, Mesh mesh = null)
    {
        cloud_renderer.renderMode = mode;
        if (mode == ParticleSystemRenderMode.Mesh)
        {
            SetFollowCamera(false);
            cloud_renderer.mesh = mesh;
            cloud_renderer.enableGPUInstancing = true;
        } else
        {
            SetFollowCamera(true);
        }
    }

    /// <summary>
    /// Set the shader for the particles.
    /// </summary>
    /// <param name="shader_name">The string name of the shader.</param>
    protected void SetShader(string shader_name)
    {
        shader = shader_name;
        Material particleMaterial = new Material(Shader.Find(shader));
        cloud_renderer.material = particleMaterial;

        PrepareMaterial(particleMaterial);
    }

    protected string debug_file = "C:\\Users\\ISAACS\\Documents\\ISAACSRadViz\\ColoredPointCloud.off";
    /// <summary>
    /// A debug method to write out the point clouds in xyz format into a file.
    /// </summary>
    protected void ExportPointCloud()
    {
        StreamWriter sw = new StreamWriter(debug_file, false);
        sw.WriteLine("COFF");
        sw.WriteLine(particle_count + " " + 0 + " " + 0);

        for(int i = 0; i < particle_count; i++) {
            ParticleSystem.Particle particle = particles[i];
            Vector3 pos = particle.position;
            Color32 col = particle.startColor;

            sw.WriteLine(pos.x + " " + pos.y + " " + pos.z + " " + col.r + " " + col.g + " " + col.b + " " + col.a);
        }

        sw.Close();

    }

    /// <summary>
    /// Set a material's properties to make it more effective for being used on many particles in a point cloud.
    /// </summary>
    /// <param name="particleMaterial">The material to configure.</param>
    protected static void PrepareMaterial(Material particleMaterial)
    {
        // Make it transparent
        particleMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        particleMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.DstAlpha);
        particleMaterial.SetInt("_ZWrite", 0);
        particleMaterial.DisableKeyword("_ALPHATEST_ON");
        particleMaterial.EnableKeyword("_ALPHABLEND_ON");
        particleMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        particleMaterial.renderQueue = 3000;
    }

    /// <summary>
    /// Prepare a particle system and its renderer for effectively showing point cloud data.
    /// </summary>
    /// <param name="ps">The particle system to configure.</param>
    /// <param name="renderer">The particle system's renderer component.</param>
    /// <param name="max_particles">The maximum number of particles the particle system is to handle.</param>
    protected void InitializeParticleSystem(ParticleSystem ps, ParticleSystemRenderer renderer, int max_particles)
    {
        ParticleSystem.MainModule main = ps.main;

        // The point cloud is unanimated, so we don't need looping or play-on-awake.
        main.loop = false;
        main.playOnAwake = false;

        main.maxParticles = max_particles;
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(1.0f, 1.0f, 1.0f, 0.0f));

        renderer.sortMode = ParticleSystemSortMode.Distance;

        // Performance optimizing options.
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
        renderer.enableGPUInstancing = true;
        
        Material particleMaterial = new Material(Shader.Find(shader));
        renderer.material = particleMaterial;
        PrepareMaterial(particleMaterial);
        particleMaterial.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, 1.0f));
        particleMaterial.SetColor("_EmissionColor", new Color(0.8f, 0.8f, 0.8f, 1.0f));

        SetRenderMethod(ParticleSystemRenderMode.Billboard);

        // Disable all unneeded components of the particle system.

        ParticleSystem.EmissionModule em = ps.emission;
        em.enabled = false;
        ParticleSystem.ShapeModule sh = ps.shape;
        sh.enabled = false;
        ParticleSystem.VelocityOverLifetimeModule vol = ps.velocityOverLifetime;
        vol.enabled = false;
        ParticleSystem.LimitVelocityOverLifetimeModule lvol = ps.limitVelocityOverLifetime;
        lvol.enabled = false;
        ParticleSystem.InheritVelocityModule ivm = ps.inheritVelocity;
        ivm.enabled = false;
        ParticleSystem.ForceOverLifetimeModule fol = ps.forceOverLifetime;
        fol.enabled = false;
        ParticleSystem.ColorOverLifetimeModule col = ps.colorOverLifetime;
        col.enabled = false;
        ParticleSystem.ColorBySpeedModule cbs = ps.colorBySpeed;
        cbs.enabled = false;
        ParticleSystem.SizeOverLifetimeModule sol = ps.sizeOverLifetime;
        sol.enabled = false;
        ParticleSystem.SizeBySpeedModule sbs = ps.sizeBySpeed;
        sbs.enabled = false;
        ParticleSystem.RotationOverLifetimeModule rol = ps.rotationOverLifetime;
        rol.enabled = false;
        ParticleSystem.RotationBySpeedModule rbs = ps.rotationBySpeed;
        rbs.enabled = false;
        ParticleSystem.ExternalForcesModule extf = ps.externalForces;
        extf.enabled = false;
        ParticleSystem.NoiseModule noise = ps.noise;
        noise.enabled = false;
        ParticleSystem.CollisionModule collision = ps.collision;
        collision.enabled = false;
        ParticleSystem.TriggerModule triggers = ps.trigger;
        triggers.enabled = false;
        ParticleSystem.SubEmittersModule subem = ps.subEmitters;
        subem.enabled = false;
        ParticleSystem.TextureSheetAnimationModule tsa = ps.textureSheetAnimation;
        tsa.enabled = false;
        ParticleSystem.LightsModule lights = ps.lights;
        lights.enabled = false;
        ParticleSystem.CustomDataModule cd = ps.customData;
        cd.enabled = false;
    }


}
