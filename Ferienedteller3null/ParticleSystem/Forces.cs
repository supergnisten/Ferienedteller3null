using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Ferienedteller3null.ParticleSystem
{
    abstract class Force
    {
        public abstract void AppendForce(Particle particle, double elapsed);
        public abstract void Update(double elapsed);
    }

    class GravityForce : Force
    {
        public Vector3D Gravity { get; set; }
        public override void AppendForce(Particle particle, double elapsed)
        {
            particle.Acceleration += Gravity * elapsed;
            particle.Velocity += particle.Acceleration * elapsed;
        }

        public override void Update(double elapsed) { }
    }

    class FluidForce : Force
    {
        readonly FluidSolver2D _solver;
        public FluidForce(uint sizeX, uint sizeY)
        {
            _solver = new FluidSolver2D(sizeX, sizeY);
        }

        public override void AppendForce(Particle particle, double elapsed)
        {
            throw new NotImplementedException();
        }

        public override void Update(double elapsed)
        {
            _solver.Update((float)elapsed);
        }
    }
}
