using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpHeight;
    private Animator _animator;
    private Rigidbody2D rigidBody;
    private int direction = 1;
    private int playerHealth = 4;
    private int playerHammerAbility = 0;
    private bool isJumping = false;
    private bool playerConcussed = false;
    private int concusCount = 0;
    public static event Action<bool> onLvlUp;

    private AudioSource source;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip failSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip breakSound;

    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text hammerText;
    [SerializeField] private Image heartImage;
    [SerializeField] private Sprite redHeart;
    [SerializeField] private Sprite greenHeart;

    [SerializeField] private GameObject deathPanel;
    [SerializeField] private GameObject levelPanel;

    void Start() //start off method
    {
        healthText.text = playerHealth.ToString();
        hammerText.text = (playerHammerAbility+"X").ToString();
        source = GetComponent<AudioSource>();
        //gets animator and sets default values at first
        _animator = GetComponent<Animator>();
        _animator.SetFloat("MoveX", 0);
        _animator.SetFloat("MoveY", 0.5f);
        //gets rigidbody
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void Update() //constant updating method
    {
        if (playerConcussed) //if player is concussed, they cant move
            return;

        //check whether space or W is pressed and isnt jumping then jumps
        if((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && !isJumping) {
            isJumping = true;
            source.PlayOneShot(jumpSound);
            rigidBody.velocity = Vector3.zero;
            rigidBody.AddForce(new Vector2(0, Mathf.Sqrt( -1 * Physics2D.gravity.y * jumpHeight)), ForceMode2D.Impulse);
        }

        //movement in horizontal direction is detected and then moved by the player
        float moveBy = Input.GetAxis("Horizontal");
        Vector2 pos = transform.position;
        pos.x += speed * moveBy * Time.deltaTime;
        transform.position = pos;

        //gets the direction the player is facing at 1 = right, -1 = left
        if (moveBy != 0) {
            direction = moveBy < 0 ? -1: 1;
        }

        //checking if player didnt fall into the void
        if (pos.y < -5) {
            source.PlayOneShot(hurtSound);
            transform.position = new Vector2(-2, 0);
            if (playerHealth != 1) {
                playerHealth--;
                updateHeart();
            } else {
                playerHealth--;
                updateHeart();
                playerDeath();
                return;
            }
        }
        
        //checks if player reached the end
        if (pos.x > 237) {
            source.PlayOneShot(winSound);
            levelPanel.SetActive(true);
            playerConcussed = true;
            _animator.SetFloat("MoveX", 0.5f);
            _animator.SetFloat("MoveY", -0.5f);
            onLvlUp?.Invoke(false);
            Invoke("newLevel", 4f);
            return;
        }

        //play animation based off movement
        if (isJumping) {
            if (direction == 1) {
                _animator.SetFloat("MoveX", 0);
                _animator.SetFloat("MoveY", 1f);
            }
            else {
                _animator.SetFloat("MoveX", 0);
                _animator.SetFloat("MoveY", -1f);
            }
        }
        else {
            playAnimation(moveBy);
        } 
    }

    // Triggered by collision and col is the object that it was collided with
    void OnCollisionEnter2D(Collision2D col) {
        //checks if the player is jumping then make it false to allow to jump again
        if (isJumping)
            isJumping = false;
    }

    void updateHeart() {
        healthText.text = playerHealth.ToString();
        if (playerHealth < 2)
            heartImage.sprite = redHeart;
        else
            heartImage.sprite = greenHeart;
    }

    //when barrel calls collision this is called, stunning a player for a couple of seconds
    public void concussPlayer() {
        source.PlayOneShot(breakSound);
        //checks if player has powerup
        if (playerHammerAbility > 0)
        {
            hammerAbility();
            return; //cancels concuss
        }

        source.PlayOneShot(hurtSound);
        playerConcussed = true;
        if (playerHealth != 1) {
            playerHealth--;
            updateHeart();
        } else {
            playerHealth--;
            updateHeart();
            playerDeath();
            return;
        } 

        //stops movement
        direction = 1;
        _animator.SetFloat("MoveX", 0);
        _animator.SetFloat("MoveY", 0);

        //hits the player back
        rigidBody.velocity = Vector3.zero;
        rigidBody.AddForce(new Vector2(-direction*1.5f, Mathf.Sqrt( -1 * Physics2D.gravity.y * 2)), ForceMode2D.Impulse);
        //tracks on what count it is on
        concusCount++;
        //calls the method 2s later (sourced from documentation)
        Invoke("uncuncussPlayer", 2f);
        
    }

    //allows the player to move again and reanimates them
    void uncuncussPlayer() {
        //removes 1 count
        concusCount--;
        //if its 0 return movement
        if(concusCount == 0)
        {
            playAnimation(0);
            playerConcussed = false;
        }
    }

    //uses the powerup and plays animation
    void hammerAbility() {
        playerHammerAbility--;
        hammerText.text = (playerHammerAbility+"X").ToString();
        _animator.SetTrigger("Strike");
    }

    //players death
    void playerDeath() {
        playerConcussed = true;
        source.PlayOneShot(failSound);
        deathPanel.SetActive(true);
        _animator.SetTrigger("Death");
    }

    //freezing game after death animation finishes
    void onDeathFinish(){
        Time.timeScale = 0;
    }

    //called by hp to give more health
    public void addHealth() {
        source.PlayOneShot(pickupSound);
        playerHealth++;
        updateHeart();
    }

    //called by hammer to give more powerups
    public void addPowerUp() {
        source.PlayOneShot(pickupSound);
        playerHammerAbility++;
        hammerText.text = (playerHammerAbility+"X").ToString();
    }

    //plays animation based on current movement, either 0,1,-1
    private void playAnimation(float moveBy) {
        if (moveBy == 0) {
            if (direction == 1) { //checks what direction to idle in
            _animator.SetFloat("MoveX", 0.5f);
            _animator.SetFloat("MoveY", 0.5f);
            }
            else{
            _animator.SetFloat("MoveX", -0.5f);
            _animator.SetFloat("MoveY", -0.5f);
           }
        }
        else if (moveBy < 0) { //left walk
            _animator.SetFloat("MoveX", -1);
            _animator.SetFloat("MoveY", 0);
        }
        else { //right walk
            _animator.SetFloat("MoveX", 1);
            _animator.SetFloat("MoveY", 0);
        }
    }

    //this sets up the new level by reset player location and reenabling movement
    void newLevel() {
        levelPanel.SetActive(false);
        playerConcussed = false;
        transform.position = new Vector2(-2,0);
        playAnimation(0);
        onLvlUp?.Invoke(true);
    }
}
