using UnityEngine;

namespace Paperial
{
    public class PillarNode : MonoBehaviour
    {
        [SerializeField] private Pillar pill;
        public int id { get; private set; }

        private void Awake() => id = GetComponent<Collider>().GetInstanceID();

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetInstanceID() == PlayerController.instanceID)
                pill.Mark(id);
        }
    }
}