using System.Collections;
using Scrappie;
using UnityEngine;
using UnityEngine.Pool;

namespace Paperial
{
    internal class EnemySpawner : MonoBehaviour
    {
        public Enemy debrisPrototype;
        public Enemy bogeyPrototype;
        public Enemy pillarProtorype;

        public ObjectPool<Enemy> debrisPool;
        public ObjectPool<Enemy> bogeyPool;
        public ObjectPool<Enemy> pillarPool;


        [SerializeField] private Enemy[] debrisModels;


        private void Awake()
        {
            debrisPool = new ObjectPool<Enemy>(CreateDebris, Enemy_OnGet, Enemy_OnRelease, null, true, 128, 128);
            bogeyPool = new ObjectPool<Enemy>(CreateBogey, Enemy_OnGet, Enemy_OnRelease, null, true, 32, 64);
        }

        private void Start()
        {
            StartCoroutine(SpawnDebris());
            BeginBogeySpawning();
        }

        private IEnumerator SpawnDebris()
        {
            yield return null;
            while (true)
            {
                for (int i = 0; i < 12; i++)
                {
                    debrisPool.Get();
                    for (int X = 0; X < 10; X++)
                        yield return null;
                }
                float time = UnityEngine.Random.Range(3.0f, 7.0f);
                yield return new WaitForSeconds(time);
            }
        }

        public void BeginBogeySpawning()
        {
            StartCoroutine(SpawnBogeys());
        }

        private IEnumerator SpawnBogeys()
        {
            while (true)
            {
                bogeyPool.Get();
                var wait = new WaitForSeconds(45);
                yield return wait;
            }
        }

        private Enemy CreateDebris()
        {
            Enemy enemy = Instantiate(debrisModels.Random<Enemy>());

            return enemy;
        }

        private Enemy CreateBogey()
        {
            return Instantiate(bogeyPrototype);
        }

        private Enemy CreatePillar()
        {
            return Instantiate(debrisPrototype);
        }

        private void Enemy_OnRelease(Enemy enemy)
        {
            enemy.Return();
        }

        private void Enemy_OnGet(Enemy enemy)
        {
            enemy.Take();
        }
    }
}