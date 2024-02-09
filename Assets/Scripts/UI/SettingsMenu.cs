using Paperial.Services;
using Paperial.Sound;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Paperial.UI
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] private Button invertXOn;
        [SerializeField] private Button invertXOff;
        [SerializeField] private Button invertYOn;
        [SerializeField] private Button invertYOff;
        [SerializeField] private Slider bgmVolume;
        [SerializeField] private Slider sfxVolume;
        [SerializeField] private Slider playerColor;
        [SerializeField] private Slider playerColor2;
        [SerializeField] private Image colorPreview;
        [SerializeField] private Image colorPreview2;
        [SerializeField] private Button closeButton;

        [SerializeField] private GameObject xOn;
        [SerializeField] private GameObject xOff;
        [SerializeField] private GameObject yOn;
        [SerializeField] private GameObject yOff;

        [SerializeField] private bool isPauseVerstion;


        private void Awake()
        {
            invertXOn.onClick.AddListener(() =>
            {
                Game.invertX = true;
                NewMethod();
            });
            invertXOff.onClick.AddListener(() =>
            {
                Game.invertX = false;
                NewMethod();
            });
            invertYOn.onClick.AddListener(() =>
            {
                Game.invertY = true;
                NewMethod();
            });
            invertYOff.onClick.AddListener(() =>
            {
                Game.invertY = false;
                NewMethod();
            });
            bgmVolume.onValueChanged.AddListener((x) =>
            {
                Audio.bgmVol = x;
                Audio.AdjustBGM(x);
            });
            sfxVolume.onValueChanged.AddListener((x) =>
            {
                Audio.sfxVol = x;
                Audio.AdjustSFX(x);
            });
            playerColor.onValueChanged.AddListener((x) => Game.currentColor1 = x);
            playerColor.onValueChanged.AddListener(SetPlayerColor);
            playerColor2.onValueChanged.AddListener((x) => Game.currentColor2 = x);
            playerColor2.onValueChanged.AddListener(SetPlayerColor2);
            if (isPauseVerstion)
                closeButton.onClick.AddListener(World.UnpauseGame);
            else
                closeButton.onClick.AddListener(Game.CloseSettings);

            //-- Set from prefab, so don't worry about it.
            //playerColor.gameObject.SetActive(!isPauseVerstion);
            //playerColor2.gameObject.SetActive(!isPauseVerstion);
        }

        private void Start()
        {
            NewMethod();
            bgmVolume.SetValueWithoutNotify(Audio.bgmVol);
            sfxVolume.SetValueWithoutNotify(Audio.sfxVol);
            playerColor.SetValueWithoutNotify(Game.gameData.color);
            playerColor2.SetValueWithoutNotify(Game.gameData.color2);

            SetPlayerColor(Game.gameData.color);
            SetPlayerColor2(Game.gameData.color2);
        }

        private void NewMethod()
        {
            xOn.SetActive(Game.invertX);
            xOff.SetActive(!Game.invertX);
            yOn.SetActive(Game.invertY);
            yOff.SetActive(!Game.invertY);
        }


        private void SetPlayerColor(float arg0) => colorPreview.color = Game.PlayerColors1;
        private void SetPlayerColor2(float arg0) => colorPreview2.color = Game.PlayerColors2;


        //-- Called from pause jank
        public void ToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
            Time.timeScale = 1;
        }
    }
}