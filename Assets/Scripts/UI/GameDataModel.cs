using System.Collections.Generic;
using UnityEngine;

namespace Paperial
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Game Data Model")]
    public class GameDataModel : ScriptableObject
    {
        [Hidden] public float playerCurrentSpeed;
        [Hidden] public float currentAngulareVelo;
        [Hidden] public float interp;
        [Hidden] public int balloons;
        [Hidden] public int pointsToNext;
        [Hidden] public float percentExplored;
        [Hidden] public int points;
        [Hidden] public float exitTime;
        public List<ITrackable> trackables = new();
    }

}
