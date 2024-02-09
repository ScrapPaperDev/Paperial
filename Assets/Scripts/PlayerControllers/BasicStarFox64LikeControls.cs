using Paperial.Sound;
using Scrappie;
using System;
using System.Collections;
using UnityEngine;
using static Paperial.PaperialInput;

namespace Paperial
{
    [Serializable]
    public class BasicStarFox64LikeControls : PlayerMovement, IConfettiAffectable, IPresenter<HUDPresenter>
    {
        public static event Action<bool> OnLoop = delegate { };
        public static event Action OnMaxSpeed = delegate { };

        [SerializeField, Range(4.0f, 90.0f)] private float maxVertiLookAngle;

        [SerializeField, Min(1), Tooltip("The max amount the base velocities can be multiplied by.")]
        private float multiplierCap;

        [SerializeField, Range(0f, 1f), Tooltip("The amount the multiplier effects speed.")]
        private float speedInfluence;

        [SerializeField, Range(0f, 1f), Tooltip("The amount the multiplier effects angular velocity.")]
        private float angularInfluence;

        [SerializeField, Min(1), Tooltip("The time it takes to reach the maximum speed cap.")]
        private float timeToSpeedCap;

        [SerializeField, Min(1), Tooltip("The time it takes to begin losing speed after exiting confetti.")]
        private float exitSustainTime;

        [SerializeField, Tooltip("The time it takes to be in confetti to begin accounting for the time in it.")]
        private float enterThresh;

        [SerializeField, Range(1, 12), Tooltip("How fast the sommersault will complete")]
        private float sommersaultSpeed;

        [SerializeField, Range(1, 12), Tooltip("How far the sommersault will extend outwards. Higher = shorter orbit")]
        private float sommersaultOrbit;

        [SerializeField, Tooltip("Allow speed to be affected by confetti while sommersaulting")]
        private bool accumulateWhileSommersaulting;

        [Header("READ ONLY------------------------")]
        [SerializeField, Range(0, 1), Tooltip("Where the evaluator is between no confetti influence and max confetti influence.")]
        private float current;
        [SerializeField] private float speedModifier;
        [SerializeField] private float angleModifier;
        [SerializeField, Tooltip("In units per second")] private float actualSpeed;
        [SerializeField] private float actualAngularVelocity;

        protected float rotY;
        protected float rotX;

        private bool sommersaulting;
        private bool inConfetti;
        private bool maxSpeedReached;
        private float timeInConfetti;
        private float decayTimer;
        private float enterTimer;
        private Vector2 sommersaultDirection;
        private MonoBehaviour co;
        private Transform planeModel;

        [SerializeField] private AnimationCurve jitter;

        [field: SerializeField, Required, Hidden] public GameDataModel GameData { get; set; }
        [field: SerializeField, Required, Hidden] public HUDPresenter Presenter { get; set; }

        public override void Bind(Transform t, params object[] dependencies)
        {
            base.Bind(t, dependencies);
            co = t.GetComponent<MonoBehaviour>();
            var a = (Action)dependencies[0];
            a += delegate { Debug.Log("OK!"); };
            planeModel = (Transform)dependencies[1];
        }

        private void IncreaseSpeed()
        {
            if (sommersaulting && !accumulateWhileSommersaulting)
                return;

            enterTimer += Time.deltaTime;

            if (enterTimer < enterThresh)
                return;

            timeInConfetti += Time.deltaTime;

            timeInConfetti = Mathf.Clamp(timeInConfetti, 0, timeToSpeedCap);

            decayTimer = 0f;

            current = timeInConfetti.Normalize(timeToSpeedCap);

            if (current >= 1.0f && !maxSpeedReached)
            {
                Audio.PlaySFX(AudioGlossary.sfx_13_TopSpeed);
                OnMaxSpeed();
                maxSpeedReached = true;
            }

            InterpolateModifiers();

            if (speedModifier < .1f)
                speedModifier = .1f;
            if (angleModifier < .1f)
                angleModifier = .1f;
        }

        private void DecreaseSpeed()
        {
            if (sommersaulting && !accumulateWhileSommersaulting)
                return;

            decayTimer += Time.deltaTime;
            enterTimer = 0;
            maxSpeedReached = false;

            if (decayTimer > exitSustainTime)
            {
                timeInConfetti -= Time.deltaTime;
                current = timeInConfetti.Normalize(timeToSpeedCap);
            }

            InterpolateModifiers();

            if (timeInConfetti < 0)
                timeInConfetti = 0;
        }

        private void InterpolateModifiers()
        {
            speedModifier = Mathf.Lerp(1, multiplierCap * speedInfluence, current);
            angleModifier = Mathf.Lerp(1, multiplierCap * angularInfluence, current);

            Vector2 rand = UnityEngine.Random.insideUnitCircle * jitter.Evaluate(current);
            if (sommersaulting)
                rand /= 8;

            planeModel.transform.localPosition = rand;
        }

        public override void Fly()
        {
            if (inConfetti)
                IncreaseSpeed();
            else
                DecreaseSpeed();

            Calc();
        }

        private void Calc()
        {
            if (!sommersaulting)
            {
                float horizontalInput = GetHori;
                float verticalInput = GetVerti;

                actualSpeed = DeltaSpeed * speedModifier;
                actualAngularVelocity = DeltaAngularVelocity / angleModifier;

                rotY += horizontalInput * actualAngularVelocity;
                rotX += verticalInput * actualAngularVelocity;
                rotY = rotY.WrapAngle();
                rotX = Mathf.Clamp(rotX, -maxVertiLookAngle, maxVertiLookAngle);
            }

            Vector3 rotation = new Vector3(rotX, rotY, 0f);
            Vector3 movement = arwing.transform.position + (arwing.forward * actualSpeed);

            arwing.SetPositionAndRotation(movement, Quaternion.Euler(rotation));

            Present();
        }

        private IEnumerator FlashKick()
        {
            sommersaulting = true;
            float mrRollingCirlce = 360;
            float brainRevolutionGirl = 0;
            float startingRotX = rotX;
            float startingRotY = rotY;
            if (sommersaultDirection.x != 0)
                rotX = 0;
            while (brainRevolutionGirl < mrRollingCirlce)
            {
                actualAngularVelocity = DeltaAngularVelocity / angleModifier * sommersaultSpeed;
                actualSpeed = DeltaAngularVelocity / angleModifier / sommersaultOrbit;

                if (sommersaultDirection.x > 0)
                    rotY += actualAngularVelocity;
                else if (sommersaultDirection.x < 0)
                    rotY += -actualAngularVelocity;
                else if (sommersaultDirection.y < 0)
                    rotX += -actualAngularVelocity;
                else if (sommersaultDirection.y > 0)
                    rotX += actualAngularVelocity;

                brainRevolutionGirl += actualAngularVelocity;
                yield return null;
            }
            rotX = startingRotX;
            rotY = startingRotY;
            OnLoop(false);
            sommersaulting = false;
        }

        public override void Sommersault(float x, float y)
        {
            if (sommersaulting)
                return;

            sommersaultDirection = new Vector2(x, y);

            if (sommersaultDirection == Vector2.zero)
                return;

            co.StartCoroutine(FlashKick());

            Audio.PlaySFX(AudioGlossary.sfx_12_Loop);

            OnLoop(true);
        }

        public void EnterConfetti() => inConfetti = true;
        public void ExitConfetti() => inConfetti = false;

        public void Present()
        {
            GameData.playerCurrentSpeed = baseSpeed * speedModifier;
            GameData.currentAngulareVelo = actualAngularVelocity;
            GameData.interp = current;
            GameData.exitTime = decayTimer.Normalize(exitSustainTime);
            Presenter.UpdateFlightInfo();
        }
    }

    public class BogeyArwing : BasicStarFox64LikeControls
    {
        public override void Bind(Transform t, params object[] dependencies)
        {
            base.Bind(t, dependencies);
        }

        public override void Fly()
        {
            base.Fly();
        }

        public override void Sommersault(float x, float y)
        {

        }
    }
}
