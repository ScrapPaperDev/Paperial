using Scrappie;
using UnityEngine;

namespace Paperial
{
    public class Bogey : Enemy, IManaged
    {
        [SerializeField] private Transform targ;
        [SerializeField] private float startingSpeed;
        [SerializeField] private float speedOverTime;

        protected override void Init()
        {
            base.Init();
            targ = area.playerTransform;
            area.debris.Add(this);
            Vector3 startingDir = UnityEngine.Random.onUnitSphere * area.Radius;
            transform.position = Vector3.zero + startingDir;
            startingSpeed = UnityEngine.Random.Range(.5f, 1.2f);
        }


        public override void ManageUpdate()
        {
            transform.LookAt(targ);

            Vector3 dir = transform.position.DirectionTo(targ.position);

            transform.position += dir * (startingSpeed * Time.deltaTime);

            startingSpeed += speedOverTime * Time.deltaTime;
        }
    }
}
