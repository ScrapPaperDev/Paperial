using System.Collections;
using Scrappie;
using UnityEngine;

namespace Paperial
{
    public class Debris : Enemy, IManaged
    {
        private float speed;
        private Vector3 direction;
        private Vector3 rot;

        protected override void Init()
        {
            base.Init();
            area.debris.Add(this);
            Vector3 startingDir = UnityEngine.Random.onUnitSphere * area.Radius;
            Vector3 endingDir = UnityEngine.Random.insideUnitSphere;
            rot = UnityEngine.Random.insideUnitCircle;
            transform.position = Vector3.zero + startingDir;
            direction = startingDir.DirectionTo(endingDir);
            speed = UnityEngine.Random.Range(8, 16);

        }

        public override void ManageUpdate() => transform.SetPositionAndRotation(transform.position + (direction * (speed * Time.deltaTime)), transform.rotation * Quaternion.Euler(rot * (speed * Time.deltaTime)));

        public override void Return()
        {
            base.Return();
            area.debris.Remove(this);
        }
    }
}
