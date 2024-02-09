using System;
using UnityEngine;

namespace Paperial
{
    [Serializable]
    public class WaveData
    {
        public int debrisCount;
        public int pillarCount;
        public int meteorCount;
        public int bogeyCount;

        [Range(0, 1)]
        public float radiusOfArea;
        public int pointsToExpansion;
        public int newBalloonsCount;
        public bool active;
    }
}