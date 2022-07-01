using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{


    [SerializeField]
    private GameObject hazardPrefab;

    [SerializeField]
    private int maxHazardsToSpawn = 3;

    [SerializeField]
    private TMPro.TextMeshProUGUI scoreText;

    [SerializeField]
    private Image backgroundMenu;

    [SerializeField]
    private GameObject mainVCam;

    [SerializeField]
    private GameObject zoomVCam;

    [SerializeField]
    private GameObject gameOverMenu;

    [SerializeField]
    private GameObject player;


    private int highScore;
    private int score;
    private float timer;
    private bool gameOver;
    private Coroutine hazardCoroutine;

    private static GameManager instance;
    public int HighScore => highScore;

    private const string HighScorePreferenceKey = "HighScore";

    public static GameManager Instance => instance;
    void Start()
    {
        instance = this;

        highScore = PlayerPrefs.GetInt(HighScorePreferenceKey);
        
    }

    private void OnEnable()
    {
        player.SetActive(true);

        zoomVCam.SetActive(false);
        mainVCam.SetActive(true);

        gameOver = false;
        score = 0;
        timer = 0;
        scoreText.text = "0";


    hazardCoroutine = StartCoroutine(SpawnHazard());
}



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 0)
            {
                UnPause();
               
            }

            if (Time.timeScale == 1)
            {
                Pause();
                
            }
        }


        if (gameOver)
            return;
        timer += Time.deltaTime;

        if(timer >= 1f)
        {
            score++;
            scoreText.text = score.ToString();

            timer = 0;
        }
    }

    private void Pause()
    {
        LeanTween.value(1, 0, 0.75f)
           .setOnUpdate(SetTimeScale)
           .setIgnoreTimeScale(true);
        backgroundMenu.gameObject.SetActive(true);
    }

    private void UnPause()
    {
        LeanTween.value(0, 1, 0.75f)
            .setOnUpdate(SetTimeScale)
            .setIgnoreTimeScale(true);
        backgroundMenu.gameObject.SetActive(false);
    }

    private void SetTimeScale(float value)
    {
        Time.timeScale = value;
        Time.fixedDeltaTime = 0.02f * value;
    }

    private IEnumerator SpawnHazard()
    {
        var hazardToSpawn = Random.Range(1, maxHazardsToSpawn);

        for (int i = 0; i < hazardToSpawn; i++)
        {
        var x = Random.Range(-8, 8);
        var drag = Random.Range(0.5f, 2f);

        var hazard = Instantiate(hazardPrefab, new Vector3(x, 11, 0), Quaternion.identity);

        hazard.GetComponent<Rigidbody>().drag = drag;

        }

        var timeToWait = Random.Range(0.5f, 1.5f);
        yield return new WaitForSeconds(timeToWait);

        yield return SpawnHazard();
    }

    public void GameOver()
    {
        StopCoroutine(hazardCoroutine);   
        gameOver = true;

        if(Time.timeScale < 1)
        {
            UnPause();
        }
        if(score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(HighScorePreferenceKey, highScore);


        }

        mainVCam.SetActive(false);
        zoomVCam.SetActive(true);

        gameObject.SetActive(false);
        gameOverMenu.SetActive(true);
    }

    public void Enable()
    {
        gameObject.SetActive(true);

        zoomVCam.SetActive(false);
        mainVCam.SetActive(true);
    }
}
