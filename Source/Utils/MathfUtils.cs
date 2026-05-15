using System;
using UnityEngine;

namespace ThickerTrajectoryLines
{
    public class MathfUtils
    {
        public static float RoundStep(float value, float step)
        {
            if (step == Single.Epsilon)
            {
                return value;
            }
            else
            {
                return Mathf.Round(value / step) * step;
            }
        }
    }
}