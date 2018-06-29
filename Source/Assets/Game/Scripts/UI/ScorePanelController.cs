using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScorePanelController : MonoBehaviour
{
    public delegate void ButtonClick();
    public event ButtonClick ClickBtnRestart;

    public Button BtnBack;
    public Button BtnRestart;

    public Text Score;

    void Start ()
    {
        BtnBack.onClick.AddListener(OnClickBtnBack);
        BtnRestart.onClick.AddListener(OnClickBtnRestart);
    }
    void OnClickBtnBack()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
    void OnClickBtnRestart()
    {
        if (ClickBtnRestart != null)
            ClickBtnRestart();
    }
}