using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ferienedteller3null.ParticleSystem
{
    class FluidSolver2D
    {
        FloatMatrix2D _u, _v, _uPrev, _vPrev, _dens, _densPrev, _densSrc, _uVelSrc, _vVelSrc;

        public float Viscosity { get; set; } = 0.1f;
        public float Diffusion { get; set; } = 0.0001f;

        public uint Nx { get; private set; }
        public uint Ny { get; private set; }

        public FluidSolver2D(uint i, uint j)
        {
            Nx = i;
            Ny = j;
            _u = new FloatMatrix2D(i, j);
            _v = new FloatMatrix2D(i, j);
            _uPrev = new FloatMatrix2D(i, j);
            _vPrev = new FloatMatrix2D(i, j);
            _dens = new FloatMatrix2D(i, j);
            _densPrev = new FloatMatrix2D(i, j);
            _densSrc = new FloatMatrix2D(i, j);
            _uVelSrc = new FloatMatrix2D(i, j);
            _vVelSrc = new FloatMatrix2D(i, j);
        }

        public Vector Velocity(uint i, uint j)
        {
            return new Vector(_u[i, j], _v[i, j]);
        }

        public void AddVelocity(uint i, uint j, Vector vel)
        {
            _u[i, j] += (float)vel.X;
            _v[i, j] += (float)vel.Y;
        }

        public void Update(float dt)
        {
            VelocityStep(_u, _v, _uPrev, _vPrev, Viscosity, dt);
            VelocityStep(_dens, _densPrev, _u, _v, Diffusion, dt);

            _uPrev.Fill(0);
            _vPrev.Fill(0);
            _densPrev.Fill(0);

            _densPrev.Add(_densSrc);
            _uPrev.Add(_uVelSrc);
            _vPrev.Add(_vVelSrc);
        }

        public void DensityStep(FloatMatrix2D x, FloatMatrix2D x0, FloatMatrix2D u, FloatMatrix2D v, float diff, float dt)
        {
            AddSource(x, x0, dt);
            Swap(ref x0, ref x);
            Diffuse(0, x, x0, diff, dt);
            Swap(ref x0, ref x);
            Advect(0, x, x0, u, v, dt);
        }

        public void VelocityStep(FloatMatrix2D u, FloatMatrix2D v, FloatMatrix2D u0, FloatMatrix2D v0, float visc, float dt)
        {
            AddSource(u, u0, dt);
            AddSource(v, v0, dt);
            Swap(ref u0, ref u);
            Diffuse(1, u, u0, visc, dt);
            Swap(ref v0, ref v);
            Diffuse(2, v, v0, visc, dt);

            Project(u, v, u0, v0);
            Swap(ref u0, ref u);
            Swap(ref v0, ref v);
            Advect(1, u, u0, u0, v0, dt);
            Advect(2, v, v0, u0, v0, dt);
            Project(u, v, u0, v0);
        }
        public void ResetFluid()
        {
            _u.Fill(0);
            _v.Fill(0);
            _uPrev.Fill(0);
            _vPrev.Fill(0);
            _dens.Fill(0);
            _densPrev.Fill(0);
        }

        public void ResetSources()
        {
            _densSrc.Fill(0);
            _uVelSrc.Fill(0);
            _vVelSrc.Fill(0);
        }

        public void Reset()
        {
            ResetFluid();
            ResetSources();
        }

        void AddSource(FloatMatrix2D target, FloatMatrix2D source, float dt)
        {
            target.AddAndMultiplyBy(source, dt);
        }

        void Diffuse(int b, FloatMatrix2D x, FloatMatrix2D x0, float diff, float dt)
        {
            float a = dt * diff * (x.Width - 2) * (x.Height - 2);
            for (uint k = 0; k < 10; k++)
                for (uint i = 1; i < x.Width - 2; i++)
                    for (uint j = 1; j < x.Height - 2; j++)
                    {
                        if (a == 0)
                            x[i, j] = x0[i, j];
                        else
                            x[i, j] =
                                (x0[i, j] + a * (x[i - 1, j] + x[i + 1, j] + x[i, j - 1] + x[i, j + 1])) / (1 + 4 * a);
                    }
            SetBound(b, x);
        }

        void Advect(int b, FloatMatrix2D d, FloatMatrix2D d0, FloatMatrix2D u, FloatMatrix2D v, float dt)
        {
            uint ni = d.Width - 2;
            uint nj = d.Height - 2;
            uint dt0x = (uint)(dt * ni);
            uint dt0y = (uint)(dt * nj);

            for (uint i = 1; i <= ni; i++)
                for (uint j = 1; j <= nj; j++)
                {
                    float x = i - dt0x * u[i, j];
                    float y = j - dt0y * v[i, j];

                    if (x < 0.5f) x = 0.5f;
                    if (x > ni + 0.5f) x = ni + 0.5f;

                    uint i0 = (uint)x;
                    uint i1 = i0 + 1;

                    if (y < 0.5f) y = 0.5f;
                    if (y > nj + 0.5f) y = nj + 0.5f;

                    uint j0 = (uint)j;
                    uint j1 = j0 + 1;

                    float s1 = x - i0;
                    float s0 = 1 - s1;
                    float t1 = y - j0;
                    float t0 = 1 - t1;

                    d[i, j] = s0 * (t0 * d0[i0, j0] + t1 * d0[i0, j1]) + 
                              s1 * (t0 * d0[i1, j0] + t1 * d0[i1, j1]);
                }
            SetBound(b, d);
        }


        void Project(FloatMatrix2D u, FloatMatrix2D v, FloatMatrix2D p, FloatMatrix2D div)
        {
            float hu = 1.0f / (u.Width - 2);
            float hv = 1.0f / (v.Height - 2);

            for (uint i = 1; i <= u.Width - 2; i++)
                for (uint j = 1; j <= u.Height - 2; j++)
                {
                    div[i, j] = -0.5f * hu * (u[i + 1, j] - u[i - 1, j]) - 
                                 0.5f * hv * (v[i, j + 1] - v[i, j - 1]);
                    p[i, j] = 0.0f;
                }

            SetBound(0, div);
            SetBound(0, p);

            for (uint k = 0; k < 10; k++)
            {
                for (uint i = 1; i <= u.Width - 2; i++)
                    for (uint j = 1; j <= u.Height - 2; j++)
                        p[i, j] = (div[i, j] + p[i - 1, j] + p[i + 1, j] + p[i, j - 1] + p[i, j + 1]) / 4.0f;

                SetBound(0, p);
            }

            for (uint i = 1; i <= u.Width - 2; i++)
                for (uint j = 1; j <= u.Height - 2; j++)
                {
                    u[i, j] = u[i, j] - 0.5f * (p[i + 1, j] - p[i - 1, j]) / hu;
                    v[i, j] = v[i, j] - 0.5f * (p[i, j + 1] - p[i, j - 1]) / hv;
                }

            SetBound(1, u);
            SetBound(2, v);
        }

        void SetBound(int b, FloatMatrix2D x)
        {
            uint nx = x.Width, ny = x.Height;

            if (b < 3)
            {
                for (uint i = 1; i < nx - 1; i++)
                {
                    x[i, 0] = b == 2 ? -x[i, 1] : x[i, 1];
                    x[i, ny - 1] = b == 2 ? -x[i, ny - 2] : x[i, ny - 2];
                }
                for (uint i = 1; i < ny - 1; i++)
                {
                    x[0, i] = b == 1 ? -x[1, i] : x[1, i];
                    x[nx - 1, i] = b == 1 ? x[nx - 2, i] : x[nx - 2, i];
                }
            }
            else if (b == 3)
            {
                for (uint i = 1; i < nx - 1; i++)
                {
                    x[i, 0] = x[i, ny - 2];
                    x[i, ny - 1] = x[i, 1];
                }
                for (uint i = 1; i < ny - 1; i++)
                {
                    x[0, i] = x[nx - 2, i];
                    x[nx - 1, i] = x[1, i];
                }
                x[0, 0] = 0.5f * (x[1, 0] + x[0, 1]);
                x[0, ny - 1] = 0.5f * (x[1, ny - 1] + x[0, ny - 2]);
                x[nx - 1, 0] = 0.5f * (x[nx - 2, 0] + x[nx - 1, 1]);
                x[nx - 1, ny - 1] = 0.5f * (x[nx - 2, ny - 1] + x[nx - 1, ny - 2]);

                return;
            }
            if (b == 4)
            {
                for (uint i = 1; i < nx; i++)
                {
                    x[i, 0] = 0;
                    x[i, ny - 1] = 0;
                }
                for (uint i = 1; i < ny; i++)
                {
                    x[0, i] = 0;
                    x[nx - 1, i] = 0;
                }
                x[0, 0] = 0;
                x[0, ny - 1] = 0;
                x[nx - 1, 0] = 0;
                x[nx - 1, ny - 1] = 0;

                return;
            }

            x[0, 0] = 0.5f * (x[1, 0] + x[0, 1]);
            x[0, ny - 1] = 0.5f * (x[1, ny - 1] + x[0, ny - 2]);
            x[nx - 1, 0] = 0.5f * (x[nx - 2, 0] + x[nx - 1, 1]);
            x[nx - 1, ny - 1] = 0.5f * (x[nx - 2, ny - 1] + x[nx - 1, ny - 2]);
        }

        

        void Swap(ref FloatMatrix2D x0, ref FloatMatrix2D x)
        {
            var tmp = x0;
            x0 = x;
            x = tmp;
        }
    }
}
