using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace Ferienedteller3null.ParticleSystem
{
    class ParticleSystem
    {
        const double _size = 32;
        readonly Random _random;
        readonly List<Particle> _particleList;

        public GeometryModel3D ParticleModel { get; private set; }
        public int MaxCount { get; set; }
        public int Count => _particleList.Count;
        public List<Force> Forces { get; private set; }
        public Rect3D Bounds { get; set; }
        public ParticleSystem(int maxCount, Color color)
        {
            _random = new Random(GetHashCode());
            _particleList = new List<Particle>();

            Forces = new List<Force>();

            MaxCount = maxCount;
            CreateParticleModel(color);
        }

        public void Update(double elapsed)
        {
            for (int i = _particleList.Count - 1; i >= 0; i--)
            {
                var particle = _particleList[i];
                if (!Bounds.Contains(particle.Position))
                {
                    _particleList.RemoveAt(i);
                    continue;
                }

                foreach (var force in Forces)
                    force.AppendForce(particle, elapsed);

                particle.Position += particle.Velocity * elapsed;
            }

            UpdateGeometry();
        }

        void UpdateGeometry()
        {
            var position = new Point3DCollection();
            var indices = new Int32Collection();
            var texcoords = new PointCollection();

            for (int i = 0; i < _particleList.Count; i++)
            {
                var positionIndex = i * 4;
                var indexIndex = i * 6;
                var particle = _particleList[i];

                var p1 = new Point3D(particle.Position.X, particle.Position.Y, particle.Position.Z);
                var p2 = new Point3D(particle.Position.X, particle.Position.Y + particle.Size, particle.Position.Z);
                var p3 = new Point3D(particle.Position.X + particle.Size, particle.Position.Y + particle.Size, particle.Position.Z);
                var p4 = new Point3D(particle.Position.X + particle.Size, particle.Position.Y, particle.Position.Z);

                position.Add(p1);
                position.Add(p2);
                position.Add(p3);
                position.Add(p4);

                var t1 = new Point(0.0, 0.0);
                var t2 = new Point(0.0, 1.0);
                var t3 = new Point(1.0, 1.0);
                var t4 = new Point(1.0, 0.0);

                texcoords.Add(t1);
                texcoords.Add(t2);
                texcoords.Add(t3);
                texcoords.Add(t4);

                indices.Add(positionIndex);
                indices.Add(positionIndex + 2);
                indices.Add(positionIndex + 1);
                indices.Add(positionIndex);
                indices.Add(positionIndex + 3);
                indices.Add(positionIndex + 2);
            }

            var meshGeo3d = ParticleModel.Geometry as MeshGeometry3D;
            meshGeo3d.Positions = position;
            meshGeo3d.TriangleIndices = indices;
            meshGeo3d.TextureCoordinates = texcoords;
        }

        public void SpawnParticle(Point3D position, Vector3D velocity, Vector3D maxVelocity, double size, double life)
        {
            if (_particleList.Count > MaxCount)
                return;

            var particle = new Particle()
            {
                Position = position,
                Life = life,
                Size = size,
                Velocity = velocity,
                MaxVelocity = maxVelocity
            };

            _particleList.Add(particle);
        }

        void CreateParticleModel(Color color)
        {
            var material = CreateMaterial(color);
            ParticleModel = new GeometryModel3D(new MeshGeometry3D(), material);
        }

        Material CreateMaterial(Color color)
        {
            var ellipse = CreateEllipse(color);

            var renderTarget = new RenderTargetBitmap((int)_size, (int)_size, 96, 96, PixelFormats.Pbgra32);
            renderTarget.Render(ellipse);
            renderTarget.Freeze();
            var brush = new ImageBrush(renderTarget);
            var material = new DiffuseMaterial(brush);

            return material;
        }

        Ellipse CreateEllipse(Color color)
        {
            var ellipse = new Ellipse()
            {
                Width = _size,
                Height = _size
            };
            var fill = CreateRadialGradientBrush(color);
            ellipse.Fill = fill;
            ellipse.Measure(new Size(_size, _size));
            ellipse.Arrange(new Rect(0, 0, _size, _size));

            return ellipse;
        }

        Brush CreateRadialGradientBrush(Color color)
        {
            var radialGradientBrush = new RadialGradientBrush();

            radialGradientBrush.GradientStops.Add(
                new GradientStop(color, 0.25));

            color.A = 0;
            radialGradientBrush.GradientStops.Add(
                new GradientStop(color, 1.0));

            return radialGradientBrush;
        }
    }
}
