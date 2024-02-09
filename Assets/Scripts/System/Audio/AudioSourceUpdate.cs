using UnityEngine;

namespace Paperial.Sound
{
    public class AudioSourceUpdate : MonoBehaviour
    {

        private AudioSource source;

        private void Start()
        {
            source = GetComponent<AudioSource>();
            source.volume = Audio.sfxVol;
            Audio.OnSFXChanged += ChangeVol;
        }

        private void OnDestroy() => Audio.OnSFXChanged -= ChangeVol;

        private void ChangeVol(float obj) => source.volume = obj;
    }


}