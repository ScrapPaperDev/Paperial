using System.Collections.Generic;
using Paperial.Services;
using Paperial.Sound;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Paperial.PaperialInput;

namespace Paperial
{
    public class World : MonoBehaviour
    {
        private static bool paused;
        public static HashSet<ITrackable> mapEntities;

        private void Awake()
        {
            mapEntities = new();
        }

        private void Start()
        {
            Game.AddService(this);
            Audio.PlayBGM(AudioGlossary.bgm_0_BGM_MainTheme);
        }

        private void OnDestroy()
        {
            Game.RemoveService(this);
            paused = false;
            Time.timeScale = 1;
            mapEntities = null;
        }

        private void Update()
        {
            if (Input.GetButtonDown(button_Pause))
            {
                paused = !paused;

                if (paused)
                {
                    //-- Cringe, but it will work until it doesnt.
                    Time.timeScale = 0;
                    SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
                }
                else
                {
                    UnpauseGame();
                }
            }
        }

        public static void UnpauseGame()
        {
            Time.timeScale = 1;
            SceneManager.UnloadSceneAsync("PauseMenu");
            paused = false;
        }
    }
}