using UnityEngine;
using System;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace Paperial.Sound
{
    public class Audio : MonoBehaviour
    {

        private static new Audio audio;

        [SerializeField] private AudioSource screenSource;
        [SerializeField] private AudioSource worldSource;
        [SerializeField] private AudioSource bgmSource;

        [SerializeField] private AudioClip[] sfx;
        [SerializeField] private AudioClip[] bgm;

        public static float sfxVol = .5f;
        public static float bgmVol = .5f;

        public static event Action<float> OnBGMChanged = delegate { };
        public static event Action<float> OnSFXChanged = delegate { };

        public static void AdjustBGM(float b)
        {
            audio.bgmSource.volume = b;
        }

        public static void AdjustSFX(float b)
        {
            audio.screenSource.volume = b;
            OnSFXChanged(b);
        }


        private void Awake()
        {
            audio = this;


        }

        private void Start()
        {
            audio.bgmSource.volume = bgmVol;
            audio.screenSource.volume = sfxVol;
        }

        public static void SetVolumeSettins(float b, float s)
        {
            sfxVol = s;
            bgmVol = b;
        }


        public static void PlaySFX(TrackSettings track)
        {
            audio.screenSource.volume = track.mixLevel * sfxVol;
            audio.screenSource.PlayOneShot(audio.sfx[track.id]);
        }

        public static void PlayBGM(TrackSettings track)
        {
            audio.bgmSource.clip = audio.sfx[track.id];
            audio.bgmSource.volume = track.mixLevel * bgmVol;
            audio.bgmSource.Play();
        }

        public static void PlaySFX(int track)
        {
            if (audio.screenSource.isPlaying)
                return;
            audio.screenSource.PlayOneShot(audio.sfx[track]);
        }

        public static void PlayBGM(int track)
        {
            audio.bgmSource.clip = audio.bgm[track];
            audio.bgmSource.Play();
        }

#if UNITY_EDITOR
        [ContextMenu("Test RegenGlossary")]
        private void TestRegenGlossary() => RegenGlossary();



        public UnityEditor.MonoScript gloss;
        private void RegenGlossary()
        {
            var s = gloss.GetFullPathTo();

            string script = @"namespace Paperial.Sound
{
    public static class AudioGlossary
    {";

            for (int i = 0; i < sfx.Length; i++)
            {
                AudioClip item = sfx[i];
                script += $"\t\tpublic const int sfx_{i}_{item.name} = {i};{Environment.NewLine}";
            }

            for (int i = 0; i < bgm.Length; i++)
            {
                AudioClip item = bgm[i];
                script += $"\t\tpublic const int bgm_{i}_{item.name} = {i};{Environment.NewLine}";
            }

            script += @"
    }
}";

            script.TrimEnd(Environment.NewLine.ToCharArray());
            File.WriteAllText(s, script);
            AssetDatabase.Refresh();

        }
#endif

    }

    public struct TrackSettings
    {
        public int id;
        public float mixLevel;
        public AudioSpace space;

        public TrackSettings(int i)
        {
            id = i;
            mixLevel = 1;
            space = AudioSpace.Screen;
        }

        public TrackSettings(int i, float l)
        {
            id = i;
            mixLevel = l;
            space = AudioSpace.Screen;
        }
    }

    public enum AudioSpace
    {
        Screen,
        World
    }

    public class SFXPickerAttribute : PropertyAttribute
    {

    }
}