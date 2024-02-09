using Paperial.Services;
using Scrappie;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Paperial.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button exitButton;

        [SerializeField] private Button openCharlesWebsite;
        [SerializeField] private Button openBuberWebsite;
        [SerializeField] private Button openScrappieWebsite;
        [SerializeField] private Button openWeigWebsite;

        [SerializeField, HideInInspector] private string gameLevelName;

        [SerializeField] private GameObject menuPanel;
        [SerializeField] private GameObject creditsPanel;
        [SerializeField] private TMP_Text creditsText;
        [SerializeField] private Button creditsExitButton;

        [SerializeField] private Button invertXOn;
        [SerializeField] private Button invertXOff;
        [SerializeField] private Button invertYOn;
        [SerializeField] private Button invertYOff;
        [SerializeField] private Slider bgmVolume;
        [SerializeField] private Slider sfxVolume;
        [SerializeField] private Slider playerColor;
        [SerializeField] private Gradient playrColorGrad;

        [SerializeField] private string[] links;

#if UNITY_EDITOR
        [SerializeField] private UnityEditor.SceneAsset gameLevel;
        private void OnValidate() => gameLevelName = gameLevel.name;
#endif

        [ContextMenu("Test GetLinks")]
        private void TestGetLinks() => GetLinks();

        private void GetLinks()
        {
            TextAsset t = Resources.Load<TextAsset>("credits");
            string text = t.text;

            links = text.Split(',');

            for (int i = 0; i < links.Length; i++)
                links[i] = ScrappieUtils.GetEnclosedValue(links[i]);
        }


        private void Start()
        {
            playButton.onClick.AddListener(StartGame);
            settingsButton.onClick.AddListener(OpenSettings);
            creditsButton.onClick.AddListener(OpenCredits);
            exitButton.onClick.AddListener(Application.Quit);

            openCharlesWebsite.onClick.AddListener(() => OpenSite(links[1]));
            openBuberWebsite.onClick.AddListener(() => OpenSite(links[0]));
            openScrappieWebsite.onClick.AddListener(() => OpenSite(links[2]));
            openWeigWebsite.onClick.AddListener(() => OpenSite(links[3]));

            creditsExitButton.onClick.AddListener(() =>
            {
                menuPanel.SetActive(true);
                creditsPanel.SetActive(false);
            });

            creditsPanel.SetActive(false);
        }

        private void OpenSite(string link) => Application.OpenURL(link);

        private void StartGame() => UnityEngine.SceneManagement.SceneManager.LoadScene(gameLevelName);

        private void OpenSettings() => Game.ShowSettings();

        private void OpenCredits()
        {
            creditsPanel.SetActive(true);
            menuPanel.SetActive(false);
        }
    }
}