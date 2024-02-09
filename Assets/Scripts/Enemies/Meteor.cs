using System.Collections;
using UnityEngine;

namespace Paperial
{
    public class Meteor : Enemy
    {
        [SerializeField] private Transform player;
        [SerializeField] private float speed;

        protected override void Init()
        {
            base.Init();



        }


        [ContextMenu("test")]
        private void test() => CrashCourse();

        [ContextMenu("Test CreachCourse")]
        private void TestCreachCourse() => StartCoroutine(CrashCourse());

        private IEnumerator CrashCourse()
        {
            float wait = UnityEngine.Random.Range(10, 60);

            while (true)
            {
                yield return new WaitForSeconds(wait);
                transform.position = player.transform.forward * 12;
                transform.position += new Vector3(0, area.radius, 0);
                while (transform.position.y > -area.radius)
                {

                    transform.Translate(Vector3.down * (speed * Time.deltaTime));

                    yield return null;
                }
            }
        }
    }
}
