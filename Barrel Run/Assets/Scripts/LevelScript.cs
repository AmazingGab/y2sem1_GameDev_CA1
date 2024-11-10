using System;
using System.Collections;
using UnityEngine;

public class LevelScript : MonoBehaviour
{
    [SerializeField] private GameObject smallBarrel;
    [SerializeField] private GameObject bigBarrel;
    [SerializeField] private GameObject healthPack;
    [SerializeField] private GameObject hammer;
    private Vector3 spawnPosition = new Vector3(250,25,0);
    private bool gameRunning = true;
    private int currentLevel = 1;

    // detecting event when player reaches the next level
    private void OnEnable() {
        PlayerController.onLvlUp += setGameState;
    }

    private void OnDisable() {
        PlayerController.onLvlUp -= setGameState;
    }

    //sets up the next level
    void setGameState(bool state) {
        gameRunning = state;
        if (state) {
            currentLevel++;
            spawnHealthPack();
            spawnHammer();
            StartCoroutine(spawner());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //spawns health pack and hammer
        spawnHealthPack();
        spawnHammer();
        //starts a spawner method to allow delays (sourced from documentation)
        StartCoroutine(spawner());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //creates method and spawns a barrerl while the game in running  
    IEnumerator spawner() {
        while (gameRunning) {
            spawnBarrel();
            yield return new WaitForSeconds((float) 4f -(currentLevel/4));
        }    
    }

    //spawns a barrel depending on level 
    void spawnBarrel() {
        var barrel = UnityEngine.Random.Range(1,50) <= (45-currentLevel) ? smallBarrel : bigBarrel;
        Instantiate(barrel, spawnPosition, Quaternion.identity);
    }

    //spawns healthPack at random location
    void spawnHealthPack() {
        int amount = UnityEngine.Random.Range(0,1+currentLevel);

        if (amount == 0)
            return;

        int yPosition = UnityEngine.Random.Range(1, 15)-1;
        int xPosition = 12 + 16*Math.Abs(yPosition);
        Instantiate(healthPack, new Vector2(xPosition,yPosition-1), Quaternion.identity);
    }

    //spawns hammer at random location
    void spawnHammer() {
        int amount = UnityEngine.Random.Range(0,0+currentLevel);

        if (amount == 0)
            return;

        int yPosition = UnityEngine.Random.Range(1, 15)-1;
        int xPosition = 12 + 16*Math.Abs(yPosition);
        Instantiate(hammer, new Vector2(xPosition,yPosition-1), Quaternion.identity);
    }
}
