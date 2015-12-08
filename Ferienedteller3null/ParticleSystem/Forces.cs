﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        readonly Rect3D _worldBounds;
        public float Viscosity
        {
            get { return _solver.Viscosity; }
            set { _solver.Viscosity = value; }
        }

        public float Diffusion
        {
            get { return _solver.Diffusion; }
            set { _solver.Diffusion = value; }
        }

        public uint Nx { get { return _solver.Nx; } }
        public uint Ny { get { return _solver.Ny; } }

        public FluidForce(uint sizeX, uint sizeY, Rect3D worldBounds)
        {
            _solver = new FluidSolver2D(sizeX, sizeY);
            _worldBounds = worldBounds;
        }

        public override void AppendForce(Particle particle, double elapsed)
        {
            throw new NotImplementedException();
        }

        public override void Update(double elapsed)
        {
            _solver.Update((float)elapsed);
        }

        public Vector Velocity(uint i, uint j)
        {
            return _solver.Velocity(i, j);
        }

        public void AddVelocity(uint i, uint j, Vector vel)
        {
            _solver.AddVelocity(i, j, vel);
        }
    }
}
