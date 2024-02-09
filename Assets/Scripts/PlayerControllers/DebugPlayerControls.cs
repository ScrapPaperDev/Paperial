using System;
using UnityEngine;
using Transform = UnityEngine.Transform;

namespace Paperial
{
    public abstract class PlayerMovement : IMovementProvider
    {
        public Transform arwing { get; private set; }

        [Header("--DESIGNER--")]
        [SerializeField] protected float baseSpeed;
        [SerializeField] protected float baseAngularVelocity;

        protected float DeltaSpeed => baseSpeed * Time.deltaTime;
        protected float DeltaAngularVelocity => baseAngularVelocity * Time.deltaTime;

        public abstract void Fly();

        public virtual void Bind(Transform t, params object[] dependencies) => arwing = t;

        public virtual void Sommersault(float x, float y) { }

    }

    [Serializable]
    public class DebugPlayerControls : PlayerMovement
    {
        private Transform cam;
        public override void Bind(Transform t, params object[] dependencies)
        {
            base.Bind(t, dependencies);
            cam = Camera.main.transform;
        }

        public override void Fly()
        {
            float left = Input.GetKey("a") ? -DeltaSpeed : 0;
            float right = Input.GetKey("d") ? DeltaSpeed : 0;
            float up = Input.GetKey("q") ? DeltaSpeed : 0; ;
            float down = Input.GetKey("e") ? -DeltaSpeed : 0; ;
            float forward = Input.GetKey("w") ? DeltaSpeed : 0; ;
            float back = Input.GetKey("s") ? -DeltaSpeed : 0; ;

            Vector3 velo = new Vector3(left + right, up + down, forward + back);

            if (Input.GetKey(KeyCode.Space))
                velo = velo * 3;

            arwing.Translate(velo, cam);
        }
    }
}