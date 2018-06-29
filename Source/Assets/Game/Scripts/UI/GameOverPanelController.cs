using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public class GameOverPanelController : MonoBehaviour
    {
        public Button BtnRestart;
        public Button BtnExit;
        public Text Score;

        private void Start()
        {
            BtnExit.onClick.AddListener(OnBtnExitClick);
            BtnRestart.onClick.AddListener(OnBtnRestartClick);
        }

        private void OnEnable()
        {
            Score.text = GlobalGameSettings.Instance.Score.ToString();
        }

        void OnBtnExitClick()
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
        void OnBtnRestartClick()
        {
            Match3SceneManager.Instance.OnClickBtnRestart();
            gameObject.SetActive(false);
        }
    }
}