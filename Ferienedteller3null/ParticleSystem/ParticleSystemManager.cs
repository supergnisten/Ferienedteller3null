using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Ferienedteller3null.ParticleSystem
{
    class ParticleSystemManager
    {
        readonly Dictionary<string, ParticleSystem> _particleSystems;
        readonly Dictionary<string, Force> _forces;
        public int ActiveParticleCount => _particleSystems.Values.Sum(i => i.Count);
        public IReadOnlyDictionary<string, Force> Forces => new ReadOnlyDictionary<string, Force>(_forces);
        public Rect3D WorldBounds { get; private set; }
        public ParticleSystemManager(Rect3D worldBounds)
        {
            WorldBounds = worldBounds;
            _particleSystems = new Dictionary<string, ParticleSystem>();
            _forces = new Dictionary<string, Force>();

            _forces.Add("Gravity", new GravityForce() { Gravity = new Vector3D(0, -98.1, 0) } );
            _forces.Add("Fluid", new FluidForce(32, 16, worldBounds));
        }

        public void Update(double elapsed)
        {
            foreach (var force in _forces.Values)
                force.Update(elapsed);

            foreach (var pSystem in _particleSystems.Values)
                pSystem.Update(elapsed);
        }

        public Model3D CreateParticleSystem(int maxCount, Color color, string name, Rect3D bounds, params Force[] forces)
        {
            if (_particleSystems.ContainsKey(name))
                return null;

            var particleSystem = new ParticleSystem(maxCount, color) { Bounds = bounds };
            particleSystem.Forces.AddRange(forces);
            _particleSystems.Add(name, particleSystem);

            return particleSystem.ParticleModel;
        }

        public void SpawnParticle(string name, Point3D position, Vector3D velocity, Vector3D maxVelocity, double size, double life)
        {
            ParticleSystem system;
            if (_particleSystems.TryGetValue(name, out system))
                system.SpawnParticle(position, velocity, maxVelocity, size, life);
        }
    }
}
