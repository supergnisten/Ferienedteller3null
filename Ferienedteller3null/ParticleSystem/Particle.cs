using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Ferienedteller3null.ParticleSystem
{
    class Particle
    {
        public Point3D Position { get; set; }

        Vector3D _velocity;
        public Vector3D Velocity
        {
            get { return _velocity; }
            set
            {
                _velocity = value;
                if (Math.Abs(_velocity.X) > MaxVelocity.X)
                    _velocity.X = Math.Sign(_velocity.X) * MaxVelocity.X;
                if (Math.Abs(_velocity.Y) > MaxVelocity.Y)
                    _velocity.Y = Math.Sign(_velocity.Y) *  MaxVelocity.Y;
                // Handle Z if needed
            }

        }
        public Vector3D MaxVelocity { get; set; }
        public Vector3D Acceleration { get; set; }
        public double Size { get; set; }
        public double Life { get; set; }
    }
}
