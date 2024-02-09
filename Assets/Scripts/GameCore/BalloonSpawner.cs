using System;
using System.Collections;
using UnityEngine;

namespace Paperial
{
    public class BalloonSpawner : MonoBehaviour
    {
        [SerializeField, Range(0, .5f)] private float bleedFromEdge;
        [SerializeField] private BalloonSpawnInfo initialSpawn;

        private IEnumerator Start()
        {
            //-- Need to wait a frame... for reasons.
            yield return null;
            CreateInitialBalloonSpread();
        }

        [ContextMenu("Test Spawn")]
        private void Test()
        {
            if (!Application.isPlaying)
                return;
            CreateInitialBalloonSpread();
        }

        private void CreateInitialBalloonSpread()
        {
            SpawnNewBalloons(initialSpawn);
        }

        internal void SpawnNewBalloons(BalloonSpawnInfo info)
        {
            var newInfo = new BalloonSpawnInfo(info, bleedFromEdge);

            float count = newInfo.count;
            for (int i = 0; i < count; i++)
            {
                Balloon balloon = Balloon.GetNewParentBalloon();
                BalloonSpread(balloon.transform, i, newInfo.minDistance, newInfo.maxDistance);
            }
        }

        private void BalloonSpread(Transform t, int i, float minDist, float maxDist)
        {
            t.transform.position = JankyScrappyFallback(t, i, minDist, maxDist);
        }

        private Vector3 JankyScrappyFallback(Transform t, int iteration, float minDist, float maxDist)
        {

            Vector3 finalPos = Vector3.zero;

            int imdub = 0;

            while (true)
            {
                Vector3 a = UnityEngine.Random.insideUnitSphere * minDist;
                Vector3 b = UnityEngine.Random.insideUnitSphere * maxDist;

                Vector3 c = (a + b) / 2f;

                if (Vector3.Distance(c, Vector3.zero) > minDist && Vector3.Distance(c, Vector3.zero) < maxDist)
                {
                    finalPos = c;
                    break;
                }

                imdub++;

                if (imdub > 1024)
                    throw new System.Exception("im dub");

            }

            return finalPos;
        }


    }

    [Serializable]
    internal struct BalloonSpawnInfo
    {

        public float count;
        public float minDistance;
        public float maxDistance;

        public BalloonSpawnInfo(float count, float minRadius, float maxRadius, float bleed = .25f)
        {
            this.count = count;
            this.minDistance = minRadius;
            this.maxDistance = maxRadius - (maxRadius * bleed);
        }

        public BalloonSpawnInfo(BalloonSpawnInfo other, float bleed = .25f)
        {
            this.count = other.count;
            this.minDistance = other.minDistance;
            this.maxDistance = other.maxDistance - (other.maxDistance * bleed);
        }
    }
}