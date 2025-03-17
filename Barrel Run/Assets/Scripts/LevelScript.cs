using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelScript : MonoBehaviour
{
    [SerializeField] private GameObject smallBarrel;
    [SerializeField] private GameObject bigBarrel;
    [SerializeField] private GameObject healthPack;
    [SerializeField] private GameObject hammer;
    [SerializeField] private TMP_Text levelText;
    private Vector3 spawnPosition = new Vector3(250,25,0);
    private bool gameRunning = true;
    private int currentLevel = 1;

    // detecting event when player reaches the next level
    private void OnEnable() {
        PlayerController.OnLvlUp += setGameState;
    }

    private void OnDisable() {
        PlayerController.OnLvlUp -= setGameState;
    }

    //sets up the next level
    void setGameState(bool state) {
        gameRunning = state;
        if (state) {
            currentLevel++;
            levelText.text = ("LEVEL: "+currentLevel).ToString();
            spawnHealthPack();
            spawnHammer();
            StartCoroutine(spawner());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        levelText.text = ("LEVEL: "+currentLevel).ToString();
        //spawns health pack and hammer
        spawnHealthPack();
        spawnHammer();
        //starts a spawner method to allow delays (sourced from documentation)
        StartCoroutine(spawner());
    }

    //creates method and spawns a barrerl while the game in running  
    IEnumerator spawner() {
        while (gameRunning) {
            spawnBarrel();
            yield return new WaitForSeconds((float) 4f -(currentLevel/5));
        }    
    }

    //spawns a barrel depending on level 
    void spawnBarrel() {
        var barrel = UnityEngine.Random.Range(1,50) <= (45-currentLevel) ? smallBarrel : bigBarrel;
        Instantiate(barrel, spawnPosition, Quaternion.identity);
    }

    //spawns healthPack at random location
    void spawnHealthPack() {
        int amount = UnityEngine.Random.Range(1,1+currentLevel);

        if (amount == 0)
            return;

        for (int i = 1;i <= amount; i++) {
            int yPosition = UnityEngine.Random.Range(1, 15)-1;
            int xPosition = 12 + 16*Math.Abs(yPosition);
            Instantiate(healthPack, new Vector2(xPosition,yPosition-1), Quaternion.identity);
        }
    }

    //spawns hammer at random location
    void spawnHammer() {
        int amount = UnityEngine.Random.Range(0,1+currentLevel);

        if (amount == 0)
            return;

        for (int i = 1;i <= amount; i++) {
            int yPosition = UnityEngine.Random.Range(1, 15)-1;
            int xPosition = 12 + 16*Math.Abs(yPosition);
            Instantiate(hammer, new Vector2(xPosition,yPosition-1), Quaternion.identity);
        }
    }

    //reloads the scene by using button click from canvas
    public void onClickTryAgain() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    //changes the scene by using button click from canvas
    public void changeScene(String name) {
        SceneManager.LoadScene(name);
        Time.timeScale = 1;
    }
}
