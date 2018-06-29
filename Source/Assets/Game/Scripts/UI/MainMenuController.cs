using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Game
{
    public class MainMenuController : MonoBehaviour
    {
        public Button BtnPLAY;
        public Slider SliderColums;
        public Slider SliderRows;
        public InputField InputColums;
        public InputField InputRows;

        void Start()
        {
            GlobalGameSettings.Instance.Score = 0;
            BtnPLAY.onClick.AddListener(OnClickButton_Play);

            SliderColums.onValueChanged.AddListener(OnColumsValueChanged);
            SliderRows.onValueChanged.AddListener(OnRowsValueChanged);

            SliderColums.value = GlobalGameSettings.Instance.Col;
            SliderRows.value = GlobalGameSettings.Instance.Row;
            InputColums.text = "";
            InputRows.text = "";
            InputRows.placeholder.GetComponent<Text>().text = GlobalGameSettings.Instance.Row.ToString();
            InputColums.placeholder.GetComponent<Text>().text = GlobalGameSettings.Instance.Col.ToString();
        }

        public void OnClickButton_3x3()
        {
            GlobalGameSettings.Instance.SetGameGridSize(3, 3);
            SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
        public void OnClickButton_15x15()
        {
            GlobalGameSettings.Instance.SetGameGridSize(15, 15);
            SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
        public void OnClickButton_Play()
        {
            GlobalGameSettings.Instance.SetGameGridSize((int)SliderColums.value, (int)SliderRows.value);
            SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }

        public void OnColumsValueChanged(float value)
        {
            InputColums.text = value.ToString();
        }
        public void OnRowsValueChanged(float value)
        {
            InputRows.text = value.ToString();
        }

        public void ColumValueUp()
        {
            SliderColums.value++;
        }
        public void ColumValueDown()
        {
            SliderColums.value--;
        }
        public void RowValueUp()
        {
            SliderRows.value++;
        }
        public void RowValueDown()
        {
            SliderRows.value--;
        }
    }
}