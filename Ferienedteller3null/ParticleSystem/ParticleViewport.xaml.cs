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
        readonly ParticleSystemManager _particleSystemManager;
        readonly Random _random;
        int _currentTick;
        double _elapsed;
        int _frameCount;
        double _frameCountTime;
        int _frameRate;
        int _lastTick;
        double _totalElapsed;
        Rect3D _bounds;
        public ParticleViewport()
        {
            InitializeComponent();

            var frameTimer = new DispatcherTimer();
            frameTimer.Tick += OnFrame;
            frameTimer.Interval = TimeSpan.FromSeconds(1.0 / 60.0);
            frameTimer.Start();

            _lastTick = Environment.TickCount;

            _particleSystemManager = new ParticleSystemManager();
            _random = new Random(GetHashCode());

            var forces = _particleSystemManager.Forces;
            _bounds = new Rect3D(-64, -36, 0, 128, 74, 0);

            WorldModels.Children.Add(
                _particleSystemManager.CreateParticleSystem(1000, Colors.White, "Snø", _bounds, forces["Gravity"]));
        }

        private void OnFrame(object sender, EventArgs e)
        {
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

            _particleSystemManager.Update(_elapsed);
            AddSnowParticle();
        }

        private void AddSnowParticle()
        {
            var startPos = _bounds.X + _random.NextDouble() * _bounds.SizeX;
            var size = _random.NextDouble();
            var maxSpeed = size * 5;
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
