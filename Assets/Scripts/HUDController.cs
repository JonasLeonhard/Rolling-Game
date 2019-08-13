using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HUDController : MonoBehaviour
{

    public TextMeshProUGUI CoinsText;
    public TextMeshProUGUI TimerText;
    public TextMeshProUGUI LifePointsText;
    public TextMeshProUGUI HighscoreText;

    public GameObject levelEndMenu;


    public static bool paused = false;
    public GameObject PauseMenu;

    bool inothermenu = false;

    public GameObject showDeathTriggerMenu;

    public GameObject playerHUD;

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && !inothermenu)
        {
            Debug.Log("ESCAPE PAUSE");
            paused = !paused;

            if (paused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    public void SetCoinsText(int counter)
    {
        if (CoinsText != null)
        {
            CoinsText.text = "Coins: " + counter.ToString() +"/4";
        }
        else
        {
            Debug.Log("ERROR: CoinText not Set in HUDController");
        }

    }

    public void SetTimerText(float timeelapsed)
    {
        if (TimerText != null)
        {
            string min = ((int)timeelapsed / 60).ToString();
            string sec = Math.Round(timeelapsed % 60, 1).ToString();

            //string time = ((int)timeelapsed / 60).ToString() +":" + (timeelapsed % 60).ToString() ;

            TimerText.text = min + ":" + sec;

        }
        else
        {
            Debug.Log("ERROR: TimerText not Set in HUDController");
        }

    }

    public void SetHighscore(String name, float score)
    {
        if (HighscoreText != null)
        {
            HighscoreText.text = name + ":" + score.ToString();
        }
        else
        {
            Debug.Log("ERROR: HighscoreText not Set in HUDController");
        }
    }

    public void SetLifePointsText(float lifepoints)
    {
        Debug.Log("in HUD: Set lifepoints " + lifepoints.ToString());
        if (LifePointsText != null)
        {
            LifePointsText.text = "Life: " + lifepoints.ToString();
        }
        else
        {
            Debug.Log("ERROR: LifePointsText not Set in HUDController");
        }
    }

    public void ShowLevelEndMenu()
    {
        inothermenu = true;
        levelEndMenu.SetActive(true);
    }


    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }


    public void PauseGame()
    {
        Time.timeScale = 0f;
        PauseMenu.SetActive(true);
        hidePlayerHUD();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        PauseMenu.SetActive(false);
        showPlayerHUD();
    }

    public void NormalTime()
    {
        Time.timeScale = 1f;
    }

    public void ShowDeathTriggerMenu()
    {
        inothermenu = true;
        showDeathTriggerMenu.SetActive(true);
    }

    public void showPlayerHUD()
    {
        if(playerHUD!=null)
        playerHUD.SetActive(true);
    }

    public void hidePlayerHUD()
    {
        if(playerHUD!=null)
        playerHUD.SetActive(false);
    }
}


