using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Paperial.UI
{
    public class GameOver : MonoBehaviour
    {

        public TMP_Text topScores;

        [SerializeField] private Button b1;
        [SerializeField] private Button b2;

        private void Awake()
        {
            gameObject.SetActive(false);
            b1.onClick.AddListener(Retry);
            b2.onClick.AddListener(ToMainMenu);
        }

        private void DisplayTopScores(IEnumerable<int> scores)
        {
            string s = $"TOP SCORES: {Environment.NewLine}";
            int i = 0;
            foreach (var item in scores)
            {
                s += $"{i + 1}: {item.ToString()}{Environment.NewLine}";
                i++;
            }

            topScores.SetText(s);
        }

        internal void ShowGameOverScreen(int score, IEnumerable<int> prevScores)
        {
            gameObject.SetActive(true);
            DisplayTopScores(prevScores);
        }

        private void Retry()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void ToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}