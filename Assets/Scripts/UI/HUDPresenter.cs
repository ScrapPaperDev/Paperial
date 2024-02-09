using Paperial.Services;
using Paperial.UI;
using UnityEngine;

namespace Paperial
{
    public class HUDPresenter : MonoBehaviour
    {
        [SerializeField, Hidden] private GameDataModel data;
        [SerializeField, Hidden] private HUD hud;

        private void Awake()
        {
            Game.AddService(this);
        }

        private void Start()
        {

            hud = FindObjectOfType<HUD>();
            hud.SetupInitialTrackables(data.trackables);
        }

        private void OnDestroy() => Game.RemoveService(this);

        private void Update() => hud.UpdateRadar();

        internal void UpdateAreaTraveled() => hud.UpdatePercentTraveled(data.points, data.pointsToNext);

        internal void UpdateBalloonCount() => hud.UpdateBalloonCount(data.balloons);

        internal void AddNewTrackable(ITrackable trackable)
        {
            data.trackables.Add(trackable);
            hud.AddNewTrackable(trackable);
        }

        internal void UpdateFlightInfo()
        {
            hud.UpdateSpeedometer(data.playerCurrentSpeed, data.interp);
            hud.UpdateTachometer(data.interp);
            hud.UpdateDecayTimer(data.exitTime);
        }

        internal void RemoveTrackable(ITrackable enemy)
        {
            data.trackables.Remove(enemy);
            hud.RemoveTrackable(enemy);
        }
    }
}