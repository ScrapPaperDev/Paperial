using UnityEngine;

namespace Paperial
{
    public class Pillar : Enemy
    {
        private int total;
        private int currentTotal;

        private void Awake()
        {
            var nodes = transform.parent.GetComponentsInChildren<PillarNode>();
            foreach (var item in nodes)
                total += item.id;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetInstanceID() == PlayerController.instanceID)
                currentTotal = 0;
        }

        internal void Mark(int iD)
        {
            currentTotal += iD;

            if (currentTotal == total)
                Destroy(gameObject);
        }
    }
}