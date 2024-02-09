using System;
using System.Collections;
using System.Collections.Generic;
using Paperial.Services;
using Paperial.Sound;
using Scrappie;
using UnityEngine;
using UnityEngine.Pool;

namespace Paperial
{
    public class Balloon : MonoBehaviour, IFiller, IInteractable, IPresenter<HUDPresenter>
    {
        private static GameDataModel gameData;
        private static HUDPresenter presenter;
        private static PlayArea playArea;
        private static Balloon prototype;
        private static ObjectPool<Balloon> balloons;

        [Header("--DESIGNER--")]
        [SerializeField, SerializeIf(nameof(Serif_ShowRadiusField))] private float fillRadius;
        [SerializeField] private bool useVisualAsRadius;
        [SerializeField] int childrenToSpawn;
        [SerializeField] private bool childCanAlsoSpawnChildren;
        [SerializeField, Hidden] private float creationCooldown;

        private bool burst;
        private bool isChild;
        private float origScale;
        private Vector3 enterPos;
        private Vector3 otherFacing;
        private Vector3 directionToSendChildren;
        private Collider col;
        private Renderer rend;

        private float FillArea => useVisualAsRadius ? transform.localScale.y : fillRadius;

        [field: SerializeField, Required, Hidden] public HUDPresenter Presenter { get; set; }
        [field: SerializeField, Required, Hidden] public GameDataModel GameData { get; set; }


        private void Awake()
        {
            col = GetComponent<Collider>();
            rend = GetComponentInChildren<Renderer>();
            origScale = transform.localScale.x;
        }

        private void Start()
        {
            if (playArea == null)
                playArea = Game.GetService<PlayArea>();

            InitPrototypeBalloon();

            if (balloons == null)
            {
                prototype = Instantiate(this);
                prototype.name = "<Balloon>";
                balloons = new ObjectPool<Balloon>(InstantiateBallon_OnPoolCreated, EnableBalloon_OnPoolGet, HideBalloon_OnPoolRelease, x => Destroy(x.gameObject), true, 64, 8096);
                Prewarm();
            }

            if (useVisualAsRadius)
                fillRadius = FillArea;

            if (isChild)
                return;

            transform.position = SnapToNearestCoord(transform.position);

        }

        public void OnDestroy() => balloons = null;

        private Vector3 SnapToNearestCoord(Vector3 snap)
        {
            //-- Balloons need to be aligned with the play area grid.
            Vector3 snapPos = playArea.SnapToNearestCoord(snap);
            //-- To account for size of nodes to snap to play grid.
            snapPos += (playArea.nodeSize / 2f).ToVector3();

            return snapPos;
        }

        private void Prewarm()
        {
            for (int i = 0; i < 64; i++)
                balloons.Release(balloons.Get());
        }

        private void SetBalloonReady()
        {
            burst = false;
            col.enabled = true;
        }

        private Balloon CreateChildBalloon()
        {
            var bal = balloons.Get();
            bal.isChild = true;
            rend.enabled = true;
            GameData.balloons++;
            Present();
            return bal;
        }

        public static Balloon GetNewParentBalloon()
        {
            Balloon balloon = prototype.CreateChildBalloon();
            balloon.isChild = false;
            balloon.burst = false;
            balloon.col.enabled = true;
            return balloon;
        }

        private void DestroyBalloon()
        {
            rend.enabled = false;
            GameData.balloons--;
            balloons.Release(this);
            Present();
        }

        [ContextMenu("Test Fill")]
        public void Fill() => playArea.FillArea(this);

        public void Interact(IInteractor interactor)
        {
            if (burst)
                return;

            enterPos = interactor.transform.position;
            otherFacing = interactor.transform.forward;
            Fill();
            Burst();
        }

        private void Burst()
        {
            DestroyBalloon();

            Audio.PlaySFX(UnityEngine.Random.Range(AudioGlossary.sfx_0_BalloonBurst, AudioGlossary.sfx_4_BalloonBurst5 + 1));

            if (isChild && !childCanAlsoSpawnChildren)
                return;

            directionToSendChildren = enterPos.DirectionTo(transform.position);
            // TODO: we want to combine the entered pos with the direction of the player facing for a more smooth burst.
            //directionToSendChildren = (directionToSendChildren + otherFacing) / 2f ?;

            for (int i = 0; i < childrenToSpawn; i++)
            {
                Balloon child = CreateChildBalloon();
                Vector3 v = JankyScrappieChildSpawn(i);
                child.StartCoroutine(AnimateToPos(child, v));
            }
        }

        [ContextMenu("Test Burst")]
        private void TestBurst()
        {
            if (!Application.isPlaying)
                throw new NotSupportedException("PLAY MODE ONLY");

            enterPos = -transform.forward;
            Burst();
        }

        private IEnumerator AnimateToPos(Balloon child, Vector3 endPos)
        {
            float t = 0;
            child.transform.position = transform.position;
            endPos = child.SnapToNearestCoord(endPos);
            Vector3 b = origScale.ToVector3();
            Vector3 a = 0.5f.ToVector3();
            while (t < 1)
            {
                child.transform.localScale = Vector3.Lerp(a, b, t);
                child.transform.position = Vector3.Lerp(child.transform.position, endPos, t);
                t += Time.deltaTime * 2;
                yield return null;
            }
            child.transform.position = endPos;
            child.transform.localScale = b;
            child.SetBalloonReady();
        }

        private Vector3 JankyScrappieChildSpawn(int i)
        {
            Vector3 pos = transform.position;
            pos += UnityEngine.Random.insideUnitSphere * transform.localScale.x + (directionToSendChildren * (i + 1 * transform.localScale.x * 4));
            return pos;
        }

        public IEnumerable<Vector3> GetFillArea(float nodeSize)
        {
            if (fillRadius < nodeSize * 2)
            {
                yield return transform.position;
                yield break;
            }

            int nodesPerAxis = (int)FillArea;
            float actualRadius = fillRadius / 2f;

            for (int x = 0; x < nodesPerAxis; x++)
            {
                for (int y = 0; y < nodesPerAxis; y++)
                {
                    for (int z = 0; z < nodesPerAxis; z++)
                    {
                        Vector3 pos = new Vector3((x - (nodesPerAxis / 2)) * nodeSize, (y - (nodesPerAxis / 2)) * nodeSize, (z - (nodesPerAxis / 2)) * nodeSize);
                        pos += transform.position + (nodeSize / 2f).ToVector3();

                        if (Vector3.Distance(transform.position, pos) <= actualRadius)
                            yield return pos;
                    }
                }
            }
        }

        private void InitPrototypeBalloon()
        {
            if (prototype != null)
                return;
            GameData.balloons = 0;
            presenter = Presenter;
            gameData = GameData;
            presenter = null;
            gameData = null;
            prototype = Instantiate(this);
            HideBalloon_OnPoolRelease(prototype);

        }

        private static Balloon InstantiateBallon_OnPoolCreated()
        {
            var proto = Instantiate(prototype);
            proto.gameObject.SetActive(false);
            return proto;
        }

        private static void EnableBalloon_OnPoolGet(Balloon b)
        {
            b.gameObject.SetActive(true);
            b.burst = true;
            b.col.enabled = false;
        }

        private static void HideBalloon_OnPoolRelease(Balloon b)
        {
            b.col.enabled = false;
            b.gameObject.SetActive(false);
            b.burst = true;
        }

        public void Present()
        {
            Presenter.UpdateBalloonCount();
        }


        #region EDITOR
        private bool Serif_ShowRadiusField() => !useVisualAsRadius;

#if UNITY_EDITOR
        Transform player;


        private void Update()
        {
            if (player == null)
                player = GameObject.Find("Player").transform;

            Debug.DrawRay(transform.position, transform.position.DirectionTo(player.position), Color.red);
            Debug.DrawRay(transform.position, transform.position.DirectionTo(enterPos), Color.green);

        }


#endif
        #endregion
    }
}
