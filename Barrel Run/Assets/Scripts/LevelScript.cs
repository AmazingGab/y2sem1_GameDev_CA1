using System.Collections;
using UnityEngine;

public class LevelScript : MonoBehaviour
{
    [SerializeField] private GameObject smallBarrel;
    [SerializeField] private GameObject bigBarrel;
    Vector3 spawnPosition = new Vector3(250,25,0);
    private bool gameRunning = true;

    private int currentLevel = 1;

    // detecting event when player reaches the next level
    private void OnEnable() {
        PlayerController.onLvlUp += setGameState;
    }

    private void OnDisable() {
        PlayerController.onLvlUp -= setGameState;
    }

    void setGameState(bool state) {
        gameRunning = state;
        if (state) {
            currentLevel++;
            StartCoroutine(spawner());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
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
            yield return new WaitForSeconds(5-(currentLevel/5));
        }    
    }

    //spawns a barrel depending on level 
    void spawnBarrel() {
        var barrel = Random.Range(1,50) <= (45-currentLevel) ? smallBarrel : bigBarrel;
        Instantiate(barrel, spawnPosition, Quaternion.identity);
    }

}
