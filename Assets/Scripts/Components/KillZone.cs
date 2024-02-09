using Paperial.Services;
using UnityEngine;

namespace Paperial
{
    public class KillZone : MonoBehaviour, IInteractable
    {
        public void Interact(IInteractor interactor)
        {
            interactor.transform.gameObject.SetActive(false);
            Game.GameOver();
        }
    }
}