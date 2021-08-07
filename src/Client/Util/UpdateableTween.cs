using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spyglass.src.Client
{
    class UpdateableTween
    {
        private ClientTime startTime;

        private float startValue;
        private float targetValue;
        private readonly float transitionLength;

        public UpdateableTween( float startValue, float transitionLength)
        {
            this.startTime = ClientTime.Eariler();
            this.startValue = startValue;
            this.targetValue = startValue;
            this.transitionLength = transitionLength;
        }

        public float getValue( float target )
        {
            if ( Math.Abs(target- targetValue) < 0.01)
            {
                return CosineSmooth(startValue, targetValue, startTime.ElapsedMilliseconds);
            }
            else
            {
                startValue = CosineSmooth(startValue, targetValue, startTime.ElapsedMilliseconds);
                startTime = ClientTime.StartNew();
                targetValue = target;
                return startValue;
            }
        }

        private float CosineSmooth(float from, float to, float timeSinceStart)
        {
            float transitionPercentage = (float)Math.Pow(Math.Abs(from - to),0.5);

            float mu = Math.Max(0f, Math.Min(1f, startTime.ElapsedMilliseconds / (transitionLength * transitionPercentage)));
            float mu2 = (1f - (float)Math.Cos(mu * Math.PI)) / 2f;
            return (from * (1f - mu2) + to * mu2);
        }
    }
}
