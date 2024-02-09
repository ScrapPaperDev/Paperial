using UnityEngine;

namespace Paperial.Services
{
    public class MicroManager : MonoBehaviour
    {

        private void Awake()
        {
            Game.AddService(this);
        }

        private void OnDestroy()
        {
            Game.RemoveService(this);
        }
    }
}
