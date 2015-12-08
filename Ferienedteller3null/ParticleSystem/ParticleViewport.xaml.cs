using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Ferienedteller3null.ParticleSystem
{
    /// <summary>
    /// Interaction logic for ParticleViewport.xaml
    /// </summary>
    public partial class ParticleViewport : UserControl
    {
        ParticleSystemManager _particleSystemManager;
        readonly Random _random;

        int _currentTick;
        double _elapsed;
        int _frameCount;
        double _frameCountTime;
        int _frameRate;
        int _lastTick;
        double _totalElapsed;
        Rect3D _bounds;

        double _dt = 1.0 / 60.0;

        readonly List<Line> _velocityLines;
        FluidForce _fluidForce;

        public bool DrawVelocity { get; set; } = true;

        public ParticleViewport()
        {
            InitializeComponent();

            _random = new Random(GetHashCode());
            _velocityLines = new List<Line>();

            _lastTick = Environment.TickCount;

            Loaded += (s, e) =>
            {
                _bounds = new Rect3D(-64, -36, 0, 128, 74, 0);
                _particleSystemManager = new ParticleSystemManager(_bounds);
                var forces = _particleSystemManager.Forces;
                _fluidForce = forces["Fluid"] as FluidForce;

                var frameTimer = new DispatcherTimer();
                frameTimer.Tick += OnFrame;
                frameTimer.Interval = TimeSpan.FromSeconds(_dt);
                frameTimer.Start();




                WorldModels.Children.Add(
                    _particleSystemManager.CreateParticleSystem(10000, Colors.White, "Snø", _bounds, forces["Gravity"], forces["Fluid"]));

                var xStep = ParticleContainer.ActualWidth / (_fluidForce.Nx - 2);
                var yStep = ParticleContainer.ActualHeight / (_fluidForce.Ny - 2);

                var velocityBrush = new SolidColorBrush(Colors.Red);
                var lines = new List<Line>();
                for (uint i = 1; i <= _fluidForce.Nx; i++)
                    for (uint j = 1; j <= _fluidForce.Ny; j++)
                    {
                        var x = _bounds.X + i * xStep;
                        var y = _bounds.Y + j * yStep;

                        var line = new Line()
                        {
                            X1 = x,
                            X2 = x,// + _random.NextDouble() * 5,
                            Y1 = y,
                            Y2 = y,// + _random.NextDouble() * 5,
                            Stroke = velocityBrush,
                            StrokeThickness = 1
                        };
                        VelocityCanvas.Children.Add(line);
                        lines.Add(line);
                    }

                _velocityLines.AddRange(lines);
            };
        }

        private void OnFrame(object sender, EventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.V))
            {
                DrawVelocity = !DrawVelocity;
                VelocityCanvas.Visibility = DrawVelocity ? 
                    Visibility.Visible : Visibility.Hidden;
            }

            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                var pos = Mouse.GetPosition(this);
                var i = (uint)((pos.X / ActualWidth) * (_fluidForce.Nx - 2));
                var j = (uint)((pos.Y / ActualHeight) * (_fluidForce.Ny - 2));
                var force = 10000000;
                var dirX = -1.0 + _random.NextDouble() * 2;
                var dirY = -1.0 + _random.NextDouble() * 2;
                var vel = new Vector(dirX, dirY) * force;
                _fluidForce.AddVelocity(i, j, vel);
                Console.WriteLine(pos + ": " + vel);
            }

            _currentTick = Environment.TickCount;
            _elapsed = (_currentTick - _lastTick) / 1000.0;
            _totalElapsed += _elapsed;
            _lastTick = _currentTick;

            _frameCount++;
            _frameCountTime += _elapsed;
            if (_frameCountTime >= 1.0)
            {
                _frameCountTime -= 1.0;
                _frameRate = _frameCount;
                _frameCount = 0;
                FrameRateLabel.Content = "FPS: " + _frameRate + "  Particles: " + _particleSystemManager.ActiveParticleCount;
            }

            _particleSystemManager.Update(_dt);

            if (DrawVelocity)
                for (uint i = 0; i < _fluidForce.Nx; i++)
                    for (uint j = 0; j < _fluidForce.Ny; j++)
                    {
                        uint index = j * _fluidForce.Nx + i;
                        var line = _velocityLines.ElementAt((int)index);
                        var velocity = _fluidForce.Velocity(i, j);
                        //if (velocity.X > 0.01)
                        line.X2 = line.X1 + velocity.X;
                        //if (velocity.Y > 0.01)
                        line.Y2 = line.Y1 + velocity.Y;
                    }

            for (int i = 0; i < 10; i++)
                AddSnowParticle();
        }

        private void AddSnowParticle()
        {
            var startPos = _bounds.X + _random.NextDouble() * _bounds.SizeX;
            var size = _random.NextDouble();
            var maxSpeed = size * 100;
            _particleSystemManager.SpawnParticle(
                "Snø", 
                new Point3D(startPos, 36, 0), 
                new Vector3D(), 
                new Vector3D(maxSpeed, maxSpeed, maxSpeed), 
                _random.NextDouble(), 
                _random.NextDouble());
        }
    }
}
