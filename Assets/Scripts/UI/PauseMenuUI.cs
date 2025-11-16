using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        #region Panels
        [Header("Panels")]
        public GameObject pausePanel;
        public GameObject optionsPanel;
        #endregion

        #region UI
        [Header("UI")]
        public GameObject pauseButton;
        public string mainMenuSceneName = "MainMenu";
        #endregion

        private bool _paused;

        public void TogglePause()
        {
            if (_paused) Resume();
            else Pause();
        }

        private void Pause()
        {
            if (_paused) return;
            _paused = true;
            Time.timeScale = 0f;
            if (pausePanel) pausePanel.SetActive(true);
            if (pauseButton) pauseButton.SetActive(false);
        }

        private void Resume()
        {
            if (!_paused) return;
            _paused = false;
            Time.timeScale = 1f;
            if (pausePanel) pausePanel.SetActive(false);
            if (optionsPanel) optionsPanel.SetActive(false);
            if (pauseButton) pauseButton.SetActive(true);
        }

        public void OpenOptions()
        {
            if (pausePanel) pausePanel.SetActive(false);
            if (optionsPanel) optionsPanel.SetActive(true);
        }

        public void CloseOptions()
        {
            if (optionsPanel) optionsPanel.SetActive(false);
            if (pausePanel) pausePanel.SetActive(true);
        }

        public void ExitToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuSceneName);
        }

        private void OnDisable()
        {
            if (!_paused) return;
            _paused = false;
            Time.timeScale = 1f;
        }
    }
}