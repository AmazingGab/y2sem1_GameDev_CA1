using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    [SerializeField] private GameObject smallBarrel;
    [SerializeField] private GameObject bigBarrel;
    private Vector3 spawnPosition = new Vector3(32,9,0);
    private bool gameRunning = true;

    void Start()
    {
        StartCoroutine(spawner());
    }

    //creates method and spawns a barrerl while the game in running  
    IEnumerator spawner() {
        while (gameRunning) {
            spawnBarrel();
            yield return new WaitForSeconds(UnityEngine.Random.Range(1,5));
        }    
    }

    //spawns a barrel depending on level 
    void spawnBarrel() {
        var barrel = UnityEngine.Random.Range(1,50) <= 25 ? smallBarrel : bigBarrel;
        Instantiate(barrel, spawnPosition, Quaternion.identity);
    }

    

    public void changeScene(String name) {
        gameRunning = false;
        SceneManager.LoadScene(name);
    }
}
