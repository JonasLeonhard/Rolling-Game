using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameController : MonoBehaviour
{
    public HUDController hudController;

    //last scene played:
    public static int lastScene = 1;

    //coins in this scene:
    int coinsCounter = 0;
    int orbsForGate = 4;

    //orbs for gate
    int orbsCounter = 0;
    public GameObject gate;
    public CameraController cam;

    //timer
    private float startTime;
    private float timeelapsed;

    //player invincible:
    public float invincibleTimer = 1f;
    private bool invincible = false;

    //livepoints
    public static float lifepoints = 3;
    public float maxLife = 10;

    private void Start()
    {
        StartLevelTimer();
        hudController.SetLifePointsText(lifepoints);

        NormalTime();
        if (SceneManager.GetActiveScene().name == "EndScene")
        {
            Debug.Log("SETSCORE");
            hudController.SetHighscore("playernameXYZ", coinsCounter);
        }
    }

    private void Update()
    {
        hudController.SetTimerText(GetLevelTime());
    }


    public void LoadnextScene()
    {
        lastScene = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadLastScene()
    {
        Debug.Log("load last scene: " + lastScene);
        Debug.Log(SceneManager.GetSceneByName("EndScene").buildIndex);
        if (lastScene == SceneManager.GetSceneByName("EndScene").buildIndex)
        {
            lastScene = 1;
        }
        else
        {
            SceneManager.LoadScene(lastScene);
        }
    }

    public void SetLastScene()
    {

        lastScene = SceneManager.GetActiveScene().buildIndex;
    }

    public void QuitGame()
    {
        Debug.Log("QuitGame();");
        Application.Quit();
    }

    public void LevelEndPointTrigger()
    {
        hudController.ShowLevelEndMenu();
        PauseGame();
    }

    public void CoinTrigger()
    {
        coinsCounter++;
        hudController.SetCoinsText(coinsCounter);
        SubLifePoint(1);
    }

    public void HearthTrigger()
    {
        addLifePoint(1);
    }

    public void StartLevelTimer()
    {
        startTime = Time.time;
    }

    public float GetLevelTime()
    {
        return Time.time - startTime;
    }

    public void addLifePoint(float points)
    {
        if (lifepoints + points <= maxLife)
        {
            lifepoints += points;
            FindObjectOfType<AudioController>().Play("AddLife");
            hudController.SetLifePointsText(lifepoints);
            Debug.Log("SET LIFE POITNS TEXT : " + lifepoints);
        }
        else
        {
            Debug.Log("Cant go beyond max LifeP.");
        }


    }

    public void SubLifePoint(float points)
    {
        if (!invincible)
        {
            StartCoroutine(Invincible(invincibleTimer));
            lifepoints -= points;
            FindObjectOfType<AudioController>().Play("PlayerDamaged");
            hudController.SetLifePointsText(lifepoints);
        }
        else
        {
            Debug.Log("Second Hit while invincibleTimer, " + gameObject.name);
        }

        if (lifepoints <= 0)
        {
            lifepoints = 0;
            PlayerDied();
        }

    }

    public void setLifePoints(float points)
    {
        lifepoints = points;
        hudController.SetLifePointsText(lifepoints);
    }

    public float GetLifePoints()
    {
        return lifepoints;
    }

    private IEnumerator Invincible(float time)
    {
        invincible = true;
        //Debug.Log("Inv Start" + invincible);
        yield return new WaitForSeconds(time);
        invincible = false;
        //Debug.Log("Inv Stop" + invincible);
    }

    public bool PlayerInvincible()
    {
        return invincible;
    }
    public void PlayerDied()
    {
        Debug.Log("PLAYER DIED, lifepoints = 0" + gameObject.name);
        setLifePoints(3); //start with hp
        LoadLastScene(); //###WIP

        //PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        //player.ResetToStartOrLastCheckpoint();

    }
    public void PauseGame()
    {
        Time.timeScale = 0f;
        hudController.hidePlayerHUD();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        hudController.showPlayerHUD();
    }

    public void NormalTime()
    {
        Time.timeScale = 1f;
        hudController.showPlayerHUD();
    }

    public void instantDeathZoneTrigger()
    {
        FindObjectOfType<AudioController>().Play("GameOver");
        hudController.ShowDeathTriggerMenu();
        PauseGame();
    }

    public void OrbTrigger()
    {
        FindObjectOfType<AudioController>().Play("Orb");
        orbsCounter++;
        hudController.SetCoinsText(orbsCounter);
        Debug.Log("OrbTrigger: " + orbsCounter);

        if (orbsCounter >= orbsForGate)
        {
            OpenGate();
        }

    }

    private void OpenGate()
    {
        Debug.Log("OPEN THE GATES!");
        if (gate == null)
        {
            Debug.Log("OpenGate Error: gate is not set in " + gameObject.name);
        }
        else
        {
            cam.CameraTargetMode(gate.transform.position, 3);
            gate.SetActive(false);

        }
    }
}
