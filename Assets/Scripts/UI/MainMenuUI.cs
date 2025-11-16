using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        #region Scenes
        [Header("Scenes")]
        public string gameSceneName = "Main";
        #endregion

        #region Panels
        [Header("Panels")]
        public GameObject mainPanel;
        public GameObject optionsPanel;
        #endregion

        public void Play()
        {
            SceneManager.LoadScene(gameSceneName);
        }

        public void OpenOptions()
        {
            if (mainPanel) mainPanel.SetActive(false);
            if (optionsPanel) optionsPanel.SetActive(true);
        }

        public void CloseOptions()
        {
            if (optionsPanel) optionsPanel.SetActive(false);
            if (mainPanel) mainPanel.SetActive(true);
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}