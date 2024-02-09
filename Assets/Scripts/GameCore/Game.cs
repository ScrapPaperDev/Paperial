using System;
using System.Collections.Generic;
using System.Linq;
using Paperial.Sound;
using Paperial.UI;
using UnityEngine;

namespace Paperial.Services
{
    // Ensure this is the first user defined object to be awoken.
    [DefaultExecutionOrder(-2)]
    public class Game : MonoBehaviour, IServiceProvider
    {
        private static Game game;

        public static event Action OnSceneLoaded;
        public static event Action OnGameOver;

        public static bool invertX;
        public static bool invertY;
        public static float currentColor1;
        public static float currentColor2;

        public static Color PlayerColors1 => game.grad.Evaluate(currentColor1);
        public static Color PlayerColors2 => game.grad.Evaluate(currentColor2);

        public static SaveData gameData => game.saveData;

        [SerializeField] private Gradient grad;
        [SerializeField] private SaveData saveData;

        [SerializeField, Hidden] private GameOver gameOverScreen;
        [SerializeField, Hidden] private GameObject settingsScreen;
        [SerializeField, Hidden] private GameDataModel dataModel;

        private GameObject settings;
        private HashSet<object> services;


        private void Awake()
        {
            if (game == null)
                game = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            services = new HashSet<object>();

            if (PlayerPrefs.HasKey("config"))
            {
                string s = PlayerPrefs.GetString("config");
                saveData = JsonUtility.FromJson<SaveData>(s);
            }

            OnSceneLoaded?.Invoke();
            dataModel.trackables.Clear();
        }

        private void OnDestroy()
        {
            game = null;
            OnGameOver = null;
        }

        private void OnApplicationQuit() => Save();

        public static void ShowSettings() => game.settings = Instantiate(game.settingsScreen);

        public static void CloseSettings() => Destroy(game.settings);

        public static T GetService<T>() => (T)game.GetService(typeof(T));

        public static void AddService(object o) => game.services.Add(o);

        public static void RemoveService(object o)
        {
            if (game != null)
                game.services.Remove(o);
        }


        public object GetService(Type serviceType)
        {
            object obj = services.Where(x => x.GetType() == serviceType).FirstOrDefault();
            if (obj != null)
                return obj;
            else
                throw new ArgumentException("The requested type was not present in the service locator: " + serviceType.Name);
        }

        [ContextMenu("Save")]
        private void Save()
        {
            saveData.sfxVol = Audio.sfxVol;
            saveData.bgmVol = Audio.bgmVol;
            string dat = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString("config", dat);
        }


        public static void GameOver()
        {
            Audio.PlaySFX(AudioGlossary.sfx_5_Crash);

            OnGameOver?.Invoke();

            List<int> topScores = new List<int>(game.saveData.topScores);

            topScores.Add(game.dataModel.points);

            topScores.Sort((x, y) => y - x);

            int m = Mathf.Min(5, topScores.Count);

            game.saveData.topScores = new int[m];

            for (int i = 0; i < m; i++)
                game.saveData.topScores[i] = topScores[i];

            game.gameOverScreen.ShowGameOverScreen(game.dataModel.points, game.saveData.topScores);
        }
    }


    [Serializable]
    public class SaveData
    {
        public bool invertX;
        public bool invertY;
        public float sfxVol;
        public float bgmVol;
        public float color;
        public float color2;
        public int[] topScores;
    }
}
