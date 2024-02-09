using System.Collections;
using Paperial.Services;
using UnityEngine;

namespace Paperial
{
    public class ZoneIndicator : MonoBehaviour
    {

        [SerializeField, Hidden] private Camera cam;
        [SerializeField, Hidden] private ParticleSystem ps;
        [SerializeField] private int emitAmount;

        private PlayArea area;

        private void Start()
        {
            area = Game.GetService<PlayArea>();
            StartCoroutine(ShowFilledZones());
        }

        private IEnumerator ShowFilledZones()
        {
            float nearClipPlane = 8.0f;
            float farClipPlane = 64.0f;

            Vector3[] frustrum = new Vector3[4];

            const float nodeSize = 8.0f;

            while (true)
            {
                cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), farClipPlane, cam.stereoActiveEye, frustrum);
                for (float z = nearClipPlane; z < farClipPlane; z += nodeSize)
                {
                    for (float x = frustrum[0].x; x < frustrum[2].x; x += nodeSize)
                    {
                        for (float y = frustrum[0].y; y < frustrum[1].y; y += nodeSize)
                        {
                            Vector3 spawnPosition = cam.transform.TransformPoint(new Vector3(x, y, z));
                            spawnPosition = area.SnapToNearestCoord(spawnPosition);
                            if (!area.CheckZoneFlagged(spawnPosition))
                                continue;

                            ps.transform.position = spawnPosition;
                            ps.Emit(emitAmount);
                        }
                    }
                    yield return null;
                }
                yield return null;
            }
        }
    }
}