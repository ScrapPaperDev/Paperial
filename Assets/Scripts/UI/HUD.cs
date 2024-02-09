using System.Collections.Generic;
using System.Text;
using Scrappie;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Paperial.UI
{
    public class HUD : MonoBehaviour
    {

        private StringBuilder builder;

        [SerializeField] private float k;

        [SerializeField] private Slider speedometer;
        [SerializeField] private Slider decaymeter;
        [SerializeField] private Slider tachometer;
        [SerializeField] private TMP_Text speedText;

        [SerializeField] private TMP_Text percentExplored;
        [SerializeField] private TMP_Text balloonsOnField;

        [SerializeField] private bool debug;
        [SerializeField] private GameObject debugPanel;

        private const string unitsPerHour = "/uph";

        //-- Creating an object pool here makes no sense, cause everything being tracked is already being pooled, so,
        //-- theres never going to be more blips than there are objects being pooled.
        //private ObjectPool<Image> blipPool;

        [SerializeField] private Transform radar;
        [SerializeField] private Image blip;

        private List<RadarBlip> activeRadarBlips;



        private void Awake()
        {
            builder = new();
            activeRadarBlips = new();
            debugPanel.SetActive(debug);
        }

        internal void SetupInitialTrackables(List<ITrackable> t)
        {
            for (int i = 0; i < t.Count; i++)
                AddNewTrackable(t[i]);
        }

        internal void AddNewTrackable(ITrackable trackable)
        {
            //-- See if theres a record for the object first
            int id = trackable.transform.GetInstanceID();
            for (int i = 0; i < activeRadarBlips.Count; i++)
            {
                if (activeRadarBlips[i].id == id)
                {
                    activeRadarBlips[i].Enable();
                    return;
                }
            }

            //-- otherwise add a new one.
            Image inst = Instantiate(blip, radar);
            inst.sprite = trackable.Icon;
            inst.transform.localScale = trackable.IconSize.ToVector3();
            RadarBlip nrb = new(trackable, inst);
            activeRadarBlips.Add(nrb);
        }

        internal void RemoveTrackable(ITrackable enemy)
        {
            int id = enemy.transform.GetInstanceID();
            for (int i = 0; i < activeRadarBlips.Count; i++)
            {
                if (activeRadarBlips[i].id == id)
                {
                    activeRadarBlips[i].Disable();
                    break;
                }
            }
        }

        internal void UpdatePercentTraveled(int p, int balloonsToNext)
        {
            builder.Clear();
            builder.Append(p.ToString());
            builder.Append("/");
            builder.Append(balloonsToNext.ToString());
            percentExplored.SetText(builder.ToString());
        }

        internal void UpdateBalloonCount(int balloons)
        {
            balloonsOnField.SetText(balloons.ToString());
        }

        internal void UpdateDecayTimer(float exitTime)
        {
            decaymeter.SetValueWithoutNotify(1 - exitTime);
        }

        internal void UpdateSpeedometer(float playerCurrentSpeed, float interp)
        {
            speedometer.SetValueWithoutNotify(interp);
            builder.Clear();
            builder.Append(Mathf.RoundToInt(playerCurrentSpeed).ToString());
            builder.Append(unitsPerHour);
            speedText.SetText(builder.ToString());
        }

        internal void UpdateTachometer(float interp)
        {
            tachometer.SetValueWithoutNotify(1 - interp);
        }

        internal void UpdateRadar()
        {
            for (int i = 0; i < activeRadarBlips.Count; i++)
            {
                RadarBlip current = activeRadarBlips[i];
                Vector3 pos = current.trackable.transform.position;
                Vector3 radarPos = new Vector3(pos.x / k, (pos.z / k) + 128f, 0);
                Quaternion radarRot = Quaternion.Euler(new Vector3(0, 0, -current.trackable.transform.localEulerAngles.y));
                current.blip.transform.SetLocalPositionAndRotation(radarPos, radarRot);
            }
        }
    }

    internal class RadarBlip
    {
        internal int id;
        internal ITrackable trackable;
        internal Image blip;

        internal RadarBlip(ITrackable trackable, Image blip)
        {
            this.trackable = trackable;
            this.blip = blip;
            id = trackable.transform.GetInstanceID();
        }

        internal void Enable()
        {
            blip.enabled = true;
        }

        internal void Disable()
        {
            blip.enabled = false;
        }
    }
}