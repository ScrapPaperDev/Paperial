using System;
using UnityEngine;

namespace Paperial
{
    public partial class CameraController : MonoBehaviour
    {
        [SerializeField] private float followSpeed;
        [SerializeField] private float slerpSpeed;
        [SerializeField, Hidden] private Transform t;
        [SerializeField, Hidden] private Transform player;
        [Hidden] public bool debug;
        [SerializeField, Hidden] private float debug_camControlSpeed;

        private float x;
        private float y;

        private bool pauseRot;

        private void OnEnable() => BasicStarFox64LikeControls.OnLoop += PauseCam_OnSommersault;
        private void OnDisable() => BasicStarFox64LikeControls.OnLoop -= PauseCam_OnSommersault;

        private void PauseCam_OnSommersault(bool b)
        {
            pauseRot = b;
        }

        private void LateUpdate()
        {
            Vector3 position = Vector3.Lerp(t.position, player.position, Time.deltaTime * followSpeed);
            Quaternion rotation = t.rotation;

            if (!pauseRot)
                rotation = Quaternion.Slerp(t.rotation, player.rotation, Time.deltaTime * slerpSpeed);

            t.SetPositionAndRotation(position, rotation);

            if (debug)
            {
                float left = Input.GetAxis(PaperialInput.axis_RHori);
                float forward = Input.GetAxis(PaperialInput.axis_RVerti);

                y += left;
                x += forward;

                t.rotation = Quaternion.Euler(new Vector3(x, -y, 0));
            }
#if !UNITY_EDITOR
            WegiCamAdditions();
#endif
        }
    }


}