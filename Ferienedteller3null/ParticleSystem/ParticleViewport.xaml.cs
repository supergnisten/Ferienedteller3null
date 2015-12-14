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
        readonly List<Ellipse> _ellipses;
        FluidForce _fluidForce;

        readonly SolidColorBrush _velocityBrush = new SolidColorBrush(Colors.Red);
        readonly SolidColorBrush _ellipseBrush = new SolidColorBrush(Colors.Yellow);

        public bool DrawVelocity { get; set; } = true;

        public ParticleViewport()
        {
            InitializeComponent();

            _random = new Random(GetHashCode());
            _velocityLines = new List<Line>();
            _ellipses = new List<Ellipse>();
            _lastTick = Environment.TickCount;

            Loaded += (s, e) =>
            {
                _bounds = new Rect3D(-64, -36, 0, 128, 74, 0);
                _particleSystemManager = new ParticleSystemManager(_bounds);
                var forces = _particleSystemManager.Forces;
                //_fluidForce = forces["Fluid"] as FluidForce;

                var frameTimer = new DispatcherTimer();
                frameTimer.Tick += OnFrame;
                frameTimer.Interval = TimeSpan.FromSeconds(_dt);
                frameTimer.Start();

                WorldModels.Children.Add(
                    _particleSystemManager.CreateParticleSystem(10000, Colors.White, "Snø", _bounds, forces["Gravity"]));//, forces["Fluid"]));

                if (_fluidForce != null)
                {
                    var xStep = ParticleContainer.ActualWidth / _fluidForce.Nx;
                    var yStep = ParticleContainer.ActualHeight / _fluidForce.Ny;

                    int max = (int)(_fluidForce.Nx * _fluidForce.Nx);
                    var lines = new Line[max];
                    var ellipses = new Ellipse[max];
                    for (uint i = 0; i < _fluidForce.Nx; i++)
                        for (uint j = 0; j < _fluidForce.Ny; j++)
                        {
                            var x = i * xStep;
                            var y = j * yStep;

                            var line = new Line()
                            {
                                X1 = x,
                                X2 = x,
                                Y1 = y,
                                Y2 = y,
                                Stroke = _velocityBrush,
                                StrokeThickness = 1
                            };
                            var size = 2;
                            var hSize = size * 0.5;
                            var ellipse = new Ellipse()
                            {
                                Height = size,
                                Width = size,
                                Fill = _ellipseBrush
                            };
                            Canvas.SetLeft(ellipse, x - hSize);
                            Canvas.SetTop(ellipse, y - hSize);

                            VelocityCanvas.Children.Add(line);
                            VelocityCanvas.Children.Add(ellipse);
                            var index = (int)(j * _fluidForce.Nx + i);
                            lines[index] = line;
                            ellipses[index] = ellipse;
                        }

                    _velocityLines.AddRange(lines);
                    _ellipses.AddRange(ellipses);
                }
            };
        }

        Point _lastMousePosition;
        private void OnFrame(object sender, EventArgs e)
        {
            if (_fluidForce != null)
            {
                var mousePosition = Mouse.GetPosition(VelocityCanvas);
                if (mousePosition.X >= 0 && mousePosition.Y >= 0)
                {
                    if (Keyboard.IsKeyDown(Key.V))
                    {
                        DrawVelocity = !DrawVelocity;
                        VelocityCanvas.Visibility = DrawVelocity ?
                            Visibility.Visible : Visibility.Hidden;
                    }

                    var mi = (uint)((mousePosition.X / ActualWidth) * _fluidForce.Nx);
                    var mj = (uint)((mousePosition.Y / ActualHeight) * _fluidForce.Ny);
                    //FrameRateLabel.Content = mi + ", " + mj + "\n" + mousePosition.X + "," + mousePosition.Y;


                    for (uint i = 0; i < _fluidForce.Nx; i++)
                        for (uint j = 0; j < _fluidForce.Ny; j++)
                            _ellipses[(int)(j * _fluidForce.Nx + i)].Fill = _ellipseBrush;

                    _ellipses[(int)(mj * _fluidForce.Nx + mi)].Fill = _velocityBrush;

                    if (Mouse.LeftButton == MouseButtonState.Pressed)
                    {


                        var force = 100000;
                        var dir = mousePosition - _lastMousePosition;
                        if (dir.LengthSquared > 0)
                        {
                            dir.Normalize();
                            var vel = dir * force;
                            _fluidForce.AddVelocity(mi, mj, vel);
                            Console.WriteLine(mousePosition + ": " + vel);
                        }
                    }
                    if (mousePosition != _lastMousePosition)
                    {
                        Console.WriteLine(mousePosition);
                        _lastMousePosition = mousePosition;
                    }
                }
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


            if (DrawVelocity && _fluidForce != null)
                for (uint i = 0; i < _fluidForce.Nx; i++)
                    for (uint j = 0; j < _fluidForce.Ny; j++)
                    {
                        uint index = j * _fluidForce.Nx + i;
                        var line = _velocityLines[(int)index];
                        var velocity = _fluidForce.Velocity(i, j);
                        line.X2 = line.X1 + velocity.X;
                        line.Y2 = line.Y1 + velocity.Y;
                    }

            _particleSystemManager.Update(_dt);
            for (int i = 0; i < 10; i++)
                AddSnowParticle();
        }

        private void AddSnowParticle()
        {
            var startPos = _bounds.X + _random.NextDouble() * _bounds.SizeX;
            var size = _random.NextDouble() + 0.1;
            var maxSpeed = size * 10;
            _particleSystemManager.SpawnParticle(
                "Snø", 
                new Point3D(startPos, 36, 0),
                //new Vector3D(), 
                new Vector3D(0, -maxSpeed, 0),
                new Vector3D(maxSpeed, maxSpeed, maxSpeed), 
                _random.NextDouble(), 
                _random.NextDouble());
        }
    }
}
