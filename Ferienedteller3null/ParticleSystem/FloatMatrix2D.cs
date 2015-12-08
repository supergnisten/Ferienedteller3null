using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferienedteller3null.ParticleSystem
{
    class FloatMatrix2D
    {
        public uint Width { get; private set; }
        public uint Height { get; private set; }
        public uint Length { get; private set; }
        public float[] Values { get; private set; }
        public float this[uint i, uint j]
        {
            get { return Values[j * Width + i]; }
            set
            {
                if (float.IsNaN(value))
                {
                }
                Values[j * Width + i] = value;
            }
        }

        public FloatMatrix2D(uint i, uint j)
        {
            Width = i;
            Height = j;
            Length = i * j;
            Values = new float[Length];
        }

        public FloatMatrix2D(FloatMatrix2D other) 
            : this(other.Width, other.Height)
        {
            for (int i = 0; i < Length; i++)
                Values[i] = other.Values[i];
        }

        public void Fill(float fillValue)
        {
            for (int i = 0; i < Length; i++)
                Values[i] = fillValue;
        }

        public void Add(float addValue)
        {
            for (int i = 0; i < Length; i++)
                Values[i] += addValue;
        }

        public void Add(FloatMatrix2D other)
        {
            for (int i = 0; i < Length; i++)
                Values[i] += other.Values[i];
        }

        public void MultiplyBy(float multValue)
        {
            for (int i = 0; i < Length; i++)
                Values[i] *= multValue;
        }

        public void AddAndMultiplyBy(FloatMatrix2D other, float multValue)
        {
            for (int i = 0; i < Length; i++)
                Values[i] += other.Values[i] * multValue;
        }
    }
}
