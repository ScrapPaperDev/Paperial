using Paperial.Services;
using UnityEngine;

namespace Paperial
{
    public class Fella : MonoBehaviour
    {
        private int apt_release;
        private int apt_gameOver;
        private int apb_inDanger;

        private PlayArea playArea;

        [SerializeField] private Transform head;
        [SerializeField] private float distToStare;
        [SerializeField] private Animator anim;

        private void Start()
        {
            playArea = Game.GetService<PlayArea>();
            Game.OnGameOver += FellaGameOver;

            apt_release = Animator.StringToHash(nameof(apt_release));
            apt_gameOver = Animator.StringToHash(nameof(apt_gameOver));
            apb_inDanger = Animator.StringToHash(nameof(apb_inDanger));
        }

        private void FellaGameOver()
        {
            anim.SetTrigger(apt_release);
            anim.SetTrigger(apt_gameOver);
        }

        private void Update()
        {
            if (playArea.closestDistance < distToStare)
            {
                anim.SetBool(apb_inDanger, true);
                head.LookAt(playArea.closestItem.transform);
            }
            else
            {
                anim.SetBool(apb_inDanger, false);
                head.LookAt(Vector3.zero);
            }
        }
    }
}