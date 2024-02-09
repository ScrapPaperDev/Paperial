//#pragma warning disable CS0649
using Scrappie;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Paperial.Services;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Paperial
{
    public class PlayArea : MonoBehaviour, IPresenter<HUDPresenter>
    {
        private static int[] flags;

        [ReadOnlyInPlayMode, SerializeField, Range(24, 8096), Header("--DESIGNER--"), Tooltip("The actual size of the play area as a sphere.")]
        private int areaDiameter;

        [field: SerializeField, ReadOnlyInPlayMode, Range(1.0f, 32f), Tooltip("The actual size of a node. Size is constant regardless of play area size")]
        public float nodeSize { get; private set; }

        [SerializeField] private bool gameOverOnOutOfBounds;

        [SerializeField] private WaveData[] waves;

        public Transform playerTransform => player;
        public float diameter => areaDiameter;
        public float radius { get; private set; }
        public float Radius => areaDiameter / 2f;

        public float closestDistance { get; private set; }
        public Transform closestItem { get; private set; }
        public List<Enemy> debris { get; private set; }

        private int waveIndex;
        private int nodeDensity;
        private float areaDiameterf;

        private PlayerController playerController;
        private WaveData currentWave => waves[waveIndex];

        [field: SerializeField, Required, Hidden] public HUDPresenter Presenter { get; set; }
        [field: SerializeField, Required, Hidden] public GameDataModel GameData { get; set; }

        [SerializeField, Hidden] private Transform player;
        [SerializeField, Hidden] private Transform cursor;
        [SerializeField, Hidden] private Transform nodePrefab;
        [SerializeField, Hidden] private Transform boundary;
        [SerializeField, Hidden] private ParticleSystem areaFilledSys;

        [Header("--DEBUG--")]
        [SerializeField] private bool log;
        [SerializeField, SerializeIf(nameof(log)), BitMask("Game Over, World Coord, Abs Coord, ID, Flagged")] private int logFlags;
        [SerializeField, Hidden] private Vector3 debug_testCoord;
        [SerializeField, Hidden] private Vector3Int debug_testCoordAbs;
        [SerializeField, Hidden] private int debug_testCoordID;
        [SerializeField, ReadOnlyInPlayMode] private bool visualizePlayArea;
        [SerializeField] private bool visFlownArea;
        [SerializeField] private bool dontUpdate;
        [SerializeField, ReadOnlyInInspector] private int nodesFlagged;
        [SerializeField, ReadOnlyInInspector] private int currentMaxFlags;
        [SerializeField, ReadOnlyInInspector] private bool inConfettiThisFrame;
        [SerializeField, ReadOnlyInInspector] private int lastConfettiZoneID;
        [SerializeField, Hidden] private int debug_fillInRate;

        #region MAGIC_METHODS
        private void Awake()
        {
            Game.AddService(this);
            debris = new List<Enemy>();
            playerController = player.GetComponent<PlayerController>();
            areaDiameterf = areaDiameter;
            radius = areaDiameterf / 2.0f;
            UpdateBoundary();
            nodeDensity = Mathf.RoundToInt(areaDiameter / nodeSize);
            GameData.pointsToNext = waves[1].pointsToExpansion;
            closestItem = transform;
            closestDistance = 100;
            try
            {
                if (flags == null)
                    flags = new int[GridUtils.GetGridFlagCount(areaDiameter) / 32];
                else
                {
                    for (int i = 0; i < flags.Length; i++)
                        flags[i] = 0;
                }
            }
            catch (OverflowException)
            {
                Debug.LogError("OVERFLOW!. for now either reduce areaDiameter or increase nodeSize and try again. this can be solved later");
                throw;
            }
            debug_testCoord = new Vector3(-radius, -radius, -radius);
            boundary.gameObject.SetActive(visualizePlayArea);
        }

        private void OnDestroy()
        {
            Game.RemoveService(this);
            flags = null;
        }

        private void Update()
        {
            if (dontUpdate)
                return;

            if (Vector3.Distance(player.position, Vector3.zero) > PlayRadius)
            {
#if UNITY_EDITOR
                Log("OUT OF BOUNDS", 1);
#endif
                if (gameOverOnOutOfBounds)
                {
                    Game.GameOver();
                    player.gameObject.SetActive(false);
                    enabled = false;
                    return;
                }


            }

            int posID = UpdatePlayerRelativePosition();

            EvaluateCurrentZone(posID);

            bool timeToCheck = Time.frameCount % 8 == 0;

            if (timeToCheck)
            {
                for (int i = debris.Count - 1; i >= 0; i--)
                {
                    Enemy m = debris[i];
                    m.ManageUpdate();
                    if (CheckZoneFlagged(m.transform.position))
                        m.Return();
                    else
                        CalcClosestDebris(m.transform);
                }
            }
            else
            {
                for (int i = debris.Count - 1; i >= 0; i--)
                    debris[i].ManageUpdate();
            }
        }
        #endregion
        
        private int GetFlagValue(int x, int y, int z) => GridUtils.CoordinatesToIndex(x, y, z, nodeDensity);

        public int PlayAreaSpaceToCoordinateID(Vector3 v) => GetFlagValue(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));

        private float PlayRadius => waves[waveIndex].radiusOfArea * diameter;

        private float SnapToIncrement(float value, float increment) => Mathf.Round(value / increment) * increment;

        public Vector3 SnapToNearestCoord(Vector3 o) => new Vector3(SnapToIncrement(o.x, nodeSize), SnapToIncrement(o.y, nodeSize), SnapToIncrement(o.z, nodeSize));

        private void UpdateBoundary() => boundary.transform.localScale = ((waves[waveIndex].radiusOfArea * diameter) * 2).ToVector3();

        private void CalcClosestDebris(Transform i)
        {
            float distance = Vector3.Distance(i.position, player.position);
            if (distance < closestDistance)
            {
                closestItem = i;
                closestDistance = distance;
            }
        }


        public Vector3 WorldToPlayAreaSpace(Vector3 obj)
        {
            Vector3 playerWorldCoord = new Vector3(SnapToIncrement(obj.x, nodeSize), SnapToIncrement(obj.y, nodeSize), SnapToIncrement(obj.z, nodeSize));

            float normalizedX = ScrappieUtils.MapValue(playerWorldCoord.x, -radius, radius);
            float normalizedY = ScrappieUtils.MapValue(playerWorldCoord.y, -radius, radius);
            float normalizedZ = ScrappieUtils.MapValue(playerWorldCoord.z, -radius, radius);

            float x = Mathf.Lerp(0, nodeDensity, normalizedX);
            float y = Mathf.Lerp(0, nodeDensity, normalizedY);
            float z = Mathf.Lerp(0, nodeDensity, normalizedZ);

            return new Vector3(x, y, z);
        }



        // TODO: implement interpolation to account for speeds that are too fast to track each coordinate you were in
        // just get the pos from last frame and this frame and then fill in the area of the direction.
        private int UpdatePlayerRelativePosition()
        {
            Vector3 playerWorldCoord = new Vector3(SnapToIncrement(player.position.x, nodeSize), SnapToIncrement(player.position.y, nodeSize), SnapToIncrement(player.position.z, nodeSize));

            float normalizedX = ScrappieUtils.MapValue(playerWorldCoord.x, -radius, radius);
            float normalizedY = ScrappieUtils.MapValue(playerWorldCoord.y, -radius, radius);
            float normalizedZ = ScrappieUtils.MapValue(playerWorldCoord.z, -radius, radius);

            int x = Mathf.RoundToInt(Mathf.Lerp(0, nodeDensity, normalizedX));
            int y = Mathf.RoundToInt(Mathf.Lerp(0, nodeDensity, normalizedY));
            int z = Mathf.RoundToInt(Mathf.Lerp(0, nodeDensity, normalizedZ));


            int flagIDAtCoord = GetFlagValue(x, y, z);
            Vector3Int playerAbsCoord = new Vector3Int(x, y, z);

            cursor.transform.position = playerWorldCoord;
#if UNITY_EDITOR
            Log("WorldCoord: " + playerWorldCoord, 2);
            Log("AbsCoord: " + playerAbsCoord, 4);
            Log("Node ID @ pos: " + flagIDAtCoord, 8);
#endif
            return flagIDAtCoord;
        }

        private void DoAtCoord(Vector3 orig, Action<Vector3> act, bool flagArea)
        {
            Vector3 playPos = WorldToPlayAreaSpace(orig);
            int playCoordID = PlayAreaSpaceToCoordinateID(playPos);
            EvaluateCurrentZone(playCoordID);
            act?.Invoke(orig);
        }


        public void FillArea(IFiller filler)
        {
            Action<Vector3> act = null;

            if (visFlownArea)
                act = delegate (Vector3 x) { cursor.position = x; };


            foreach (var item in filler.GetFillArea(nodeSize))
                DoAtCoord(item, act, true);
        }


        private void EvaluateCurrentZone(int nodeIndex)
        {
            bool changedZones = nodeIndex != lastConfettiZoneID;

            if (!changedZones)
                return;

            bool isNewZone = !CheckFlaggedInts(nodeIndex);

            inConfettiThisFrame = !isNewZone;

            if (isNewZone)
            {
                lastConfettiZoneID = nodeIndex;
                FillCooordinate();
#if UNITY_EDITOR
                Log("FLAGGED: " + nodeIndex, 16);
#endif
            }
            playerController.UpdateConfettiState(inConfettiThisFrame);
        }

        private void FillCooordinate()
        {
            if (visFlownArea)
                InstantiateVisualizerAtCursorPos();

            nodesFlagged++;
            areaFilledSys.transform.position = player.transform.position + player.transform.forward;
            areaFilledSys.Emit(32);
            CheckExpansion();
            Present();
        }

        private void CheckExpansion()
        {
            if (waveIndex >= waves.Length)
                return;

            for (int i = 1; i < waves.Length; i++)
            {
                WaveData wave = waves[i];
                if (!wave.active && nodesFlagged > wave.pointsToExpansion)
                {
                    waveIndex = i;
                    Expand();
                    break;
                }
            }
        }

        private void Expand()
        {
            UpdateBoundary();
            GameData.pointsToNext = waves[waveIndex + 1].pointsToExpansion;
            currentWave.active = true;
            if (waveIndex == waves.Length)
                GameData.pointsToNext = -1;
        }

        private void InstantiateVisualizerAtCursorPos()
        {
            var node = Instantiate(nodePrefab, cursor.transform.position, Quaternion.identity);
            node.transform.localScale = nodeSize.ToVector3();
        }

        public void Present()
        {
            GameData.points = nodesFlagged;
            Presenter.UpdateAreaTraveled();
        }

        public bool CheckZoneFlagged(Vector3 pos)
        {
            Vector3 p = WorldToPlayAreaSpace(pos);
            int flagIDAtCoord = GetFlagValue((int)p.x, (int)p.y, (int)p.z);

            return flags.IsFlagged(flagIDAtCoord);
        }

        private bool CheckFlaggedInts(int nodeIndex)
        {
            if (flags.IsFlagged(nodeIndex))
                return true;

            flags.SetFlag(nodeIndex, true);

            return false;
        }





#if UNITY_EDITOR
        #region  DEBUGGING

        private void OnValidate()
        {
            if (Application.isPlaying)
                debug_CheckCoord();
            else
                UpdateBoundary();
        }

        [ContextMenu("Visualize Grid")]
        private void vis() => StartCoroutine(VisualizeGridByID());


        private void Log(object msg, int i, UnityEngine.Object ori = null)
        {
            if (!log)
                return;
            if ((i & logFlags) != 0)
                Debug.Log(msg, ori);
        }

        private IEnumerator VisualizeGridByID()
        {
            float n2 = nodeSize / 2f;

            List<int> idCheck = new List<int>();
            int stepper = 0;
            int xCoord = 0;
            float x = -radius;
            while (x < radius)
            {
                float y = -radius;
                int yCoord = 0;
                while (y < radius)
                {
                    float z = -radius;
                    int zCoord = 0;
                    while (z < radius)
                    {
                        Vector3 pos = new Vector3(x + n2, y + n2, z + n2);


                        if (Vector3.Distance(Vector3.zero, pos) <= radius)
                        {
                            cursor.position = pos;
                            InstantiateVisualizerAtCursorPos();
                            int flagID = GetFlagValue(xCoord, yCoord, zCoord);
                            idCheck.Add(flagID);
                        }

                        z += nodeSize;

                        if (stepper % debug_fillInRate == 0)
                            yield return null;

                        stepper++;
                        zCoord++;

                    }
                    y += nodeSize;
                    yCoord++;
                }
                x += nodeSize;
                xCoord++;
            }

            idCheck.Sort();
            idCheck.ForEach(x => Debug.Log(x));
        }


        [ContextMenu("test en")]
        private void Test() => ForEachCoord((x, y) =>
        {
            Debug.Log(x);
            Debug.Log(y);
        });

        private void ForEachChordInPlayArea(Action<Vector3> act)
        {
            int nodesPerAxis = areaDiameter;
            float actualRadius = areaDiameter / 2f;

            for (int x = 0; x < nodesPerAxis; x++)
            {
                for (int y = 0; y < nodesPerAxis; y++)
                {
                    for (int z = 0; z < nodesPerAxis; z++)
                    {
                        Vector3 pos = new Vector3((x - (nodesPerAxis / 2)) * nodeSize, (y - (nodesPerAxis / 2)) * nodeSize, (z - (nodesPerAxis / 2)) * nodeSize);

                        if (Vector3.Distance(transform.position, pos) <= actualRadius)
                            act(pos);
                    }
                }
            }
        }

        private void ForEachCoord(Action<Vector3, int> act)
        {
            float n2 = nodeSize / 2f;

            float x = -radius;
            int xCoord = 0;
            while (x < radius)
            {
                float y = -radius;
                int yCoord = 0;
                while (y < radius)
                {
                    float z = -radius;
                    int zCoord = 0;
                    while (z < radius)
                    {
                        Vector3 pos = new Vector3(x + n2, y + n2, z + n2);
                        if (Vector3.Distance(Vector3.zero, pos) <= radius)
                            act(pos, GetFlagValue(xCoord, yCoord, zCoord));

                        z += nodeSize;
                        zCoord++;
                    }
                    y += nodeSize;
                    yCoord++;
                }
                x += nodeSize;
                xCoord++;
            }
        }

        private void debug_CheckCoord()
        {
            float normalizedX = ScrappieUtils.MapValue(debug_testCoord.x, -radius, radius);
            float normalizedY = ScrappieUtils.MapValue(debug_testCoord.y, -radius, radius);
            float normalizedZ = ScrappieUtils.MapValue(debug_testCoord.z, -radius, radius);

            int x = (int)Mathf.Lerp(0, areaDiameter, normalizedX);
            int y = (int)Mathf.Lerp(0, areaDiameter, normalizedY);
            int z = (int)Mathf.Lerp(0, areaDiameter, normalizedZ);

            var pos = new Vector3Int((int)debug_testCoord.x, (int)debug_testCoord.y, (int)debug_testCoord.z);
            debug_testCoordID = GetFlagValue(x, y, z);
            debug_testCoordAbs = new Vector3Int(x, y, z);

            cursor.position = pos;
        }

        [ContextMenu("Check Cursor Pos")]
        private void CheckUnit()
        {
            UpdatePlayerRelativePosition();
        }

        internal void UpdatePlayAreaSize()
        {
            currentMaxFlags = 0;
            float tot = GridUtils.GetGridFlagCount(areaDiameter);
            EditorUtility.DisplayProgressBar("Updating", "Wait a sec...", .5f);

            int nodesPerAxis = Mathf.RoundToInt((float)areaDiameter / (float)nodeSize);
            float actualRadius = areaDiameter / 2f;

            int total = nodesPerAxis * nodesPerAxis * nodesPerAxis;

            for (int x = 0; x < nodesPerAxis; x++)
            {
                for (int y = 0; y < nodesPerAxis; y++)
                {
                    for (int z = 0; z < nodesPerAxis; z++)
                    {
                        bool b = EditorUtility.DisplayCancelableProgressBar("Updating", "Wait a sec...", ((float)currentMaxFlags).Normalize((float)total));
                        if (b)
                        {
                            EditorUtility.ClearProgressBar();
                            return;
                        }
                        Vector3 pos = new Vector3((x - (nodesPerAxis / 2)) * nodeSize, (y - (nodesPerAxis / 2)) * nodeSize, (z - (nodesPerAxis / 2)) * nodeSize);

                        if (Vector3.Distance(transform.position, pos) <= actualRadius)
                            currentMaxFlags++;
                    }
                }
            }
            ScrappieUtils.SetDirty(this);
            EditorUtility.ClearProgressBar();
        }
        #endregion
#endif

    }



#if UNITY_EDITOR
    [CustomEditor(typeof(PlayArea))]
    public class PlayArea_Editor : Editor
    {
        private PlayArea linked;
        private float origSize;
        private float origSize2;


        private void OnEnable()
        {
            linked = (PlayArea)target;
            origSize = linked.nodeSize;
            origSize2 = linked.diameter;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (linked.nodeSize == origSize && linked.diameter == origSize2)
                return;

            var t = linked.diameter / linked.nodeSize;

            int tot = (int)(t * t * t);


            if (tot < 0 || tot >= int.MaxValue)
            {
                EditorGUILayout.HelpBox("The play area is too large to support current flag implementation. Either reduces the area diameter or increase the node size.", MessageType.Error);
                Debug.LogError("The play area is too large to support current flag implementation. Either reduces the area diameter or increase the node size.");
            }



            if (GUILayout.Button(new GUIContent("Recalculate Play Area Size", "If you change node size, click this!")))
            {
                linked.UpdatePlayAreaSize();
                origSize = linked.nodeSize;
                origSize2 = linked.diameter;
            }

        }
    }
#endif
}