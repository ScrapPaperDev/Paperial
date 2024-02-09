using Paperial.Services;
using Scrappie;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Paperial.PaperialInput;

namespace Paperial
{
    public class PlayerController : MonoBehaviour, ITrackable, IInteractor
    {
        [SerializeField] private GameObject model;
        [SerializeField] private Sprite radarIcon;
        [SerializeField] private KeyCode debug_controlSwap;
        [SerializeField] private DebugPlayerControls debugControls;
        [SerializeField] private BasicStarFox64LikeControls sf64Controls;
        [SerializeField] private GameDataModel gameData;
        [SerializeField] private Transform audioLoop;

        private int controllerIndex;
        private IMovementProvider playerMovement;
        private List<IMovementProvider> controllers;
        private IConfettiAffectable confettiAffectables;
        public Sprite Icon => radarIcon;
        public float IconSize => 1.85f;
        public string Identifier => "Player";
        public int ID => GetHashCode();
        public override int GetHashCode() => Identifier.GetHashCode();

        public static int instanceID;

        private void Awake()
        {
            instanceID = GetComponent<Collider>().GetInstanceID();
            controllers = new List<IMovementProvider>();
            GameObject md = Instantiate(model, transform);
            UpdateModelColor(md);
            playerMovement = debugControls;
            debugControls.Bind(transform);
            sf64Controls.Bind(transform, (Action)Start, md.transform);
            controllers.Add(debugControls);
            controllers.Add(sf64Controls);
            confettiAffectables = sf64Controls;

#if !UNITY_EDITOR
            playerMovement = sf64Controls;
#endif
        }

        private void Start() { }

        private void OnDestroy() => gameData.trackables.Remove(this);

        public void Update()
        {
            playerMovement.Fly();


            audioLoop.transform.position = transform.position;

            if (Input.GetButton(button_Loop))
                playerMovement.Sommersault(GetHori, GetVerti);

#if UNITY_EDITOR

            if (Input.GetKeyDown(debug_controlSwap))
            {
                controllerIndex = controllerIndex.GetNextIndex(controllers.Count);
                playerMovement = controllers[controllerIndex];
            }

            DebugStuff();

#endif
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IInteractable comp))
                comp.Interact(this);
        }

        internal void UpdateConfettiState(bool inConfettiThisFrame)
        {
            if (inConfettiThisFrame)
                confettiAffectables.EnterConfetti();
            else
                confettiAffectables.ExitConfetti();
        }

        private void UpdateModelColor(GameObject md)
        {
            var rend = md.GetComponentInChildren<Renderer>();
            var prop = new MaterialPropertyBlock();
            rend.GetPropertyBlock(prop);

            prop.SetColor("_BaseColor", Game.PlayerColors1);

            rend.SetPropertyBlock(prop);

            ParticleSystem p = GameObject.Find("ConfettiBurst").GetComponent<ParticleSystem>();
            var m = p.main;
            m.startColor = new ParticleSystem.MinMaxGradient(Game.PlayerColors2, Game.PlayerColors2);

            ParticleSystem p2 = GameObject.Find("SpaceConfetti").GetComponent<ParticleSystem>();
            var m2 = p.main;

            float newCola = Game.PlayerColors2.r + .2f;
            float newColb = Game.PlayerColors2.g + .2f;
            float newColc = Game.PlayerColors2.b + .2f;

            Color c = new Color(newCola.Normalize(1), newColb.Normalize(1), newColc.Normalize(1));

            m2.startColor = new ParticleSystem.MinMaxGradient(c, c);

            gameData.trackables.Add(this);

        }

        private void DebugStuff()
        {
            if (Input.GetKey(KeyCode.Z))
            {
                var c = FindObjectOfType<CameraController>();
                c.debug = true;
            }

            if (Input.GetKey(KeyCode.X))
            {
                var c = FindObjectOfType<CameraController>();
                c.debug = false;
            }
        }

        private void OnDrawGizmos()
        {
            const float thickness = .15f;
            Gizmos.color = Color.red;
            Gizmos.DrawCube(transform.position + transform.right, new Vector3(1.5f, thickness, thickness));
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.position + transform.up, new Vector3(thickness, 1.5f, thickness));
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(transform.position + transform.forward, new Vector3(thickness, thickness, 1.5f));
        }
    }
}