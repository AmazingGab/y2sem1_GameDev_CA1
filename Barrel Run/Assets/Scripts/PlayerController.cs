using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpHeight;
    private Animator _animator;
    private Rigidbody2D _rigidBody;
    private int _direction = 1;
    private int _playerHealth = 4;
    private int _playerHammerAbility = 0;
    private bool _isJumping = false;
    private bool _playerConcussed = false;
    private int _concussCount = 0;
    public static event Action<bool> OnLvlUp;

    private AudioSource _source;
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
        healthText.text = _playerHealth.ToString();
        hammerText.text = (_playerHammerAbility+"X");
        _source = GetComponent<AudioSource>();
        //gets animator and sets default values at first
        _animator = GetComponent<Animator>();
        _animator.SetFloat("MoveX", 0);
        _animator.SetFloat("MoveY", 0.5f);
        //gets rigidbody
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    void Update() //constant updating method
    {
        if (_playerConcussed) //if player is concussed, they cant move
            return;

        //check whether space or W is pressed and isn't jumping then jumps
        if((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && !_isJumping) {
            _isJumping = true;
            _source.PlayOneShot(jumpSound);
            _rigidBody.linearVelocity = Vector3.zero;
            _rigidBody.AddForce(new Vector2(0, Mathf.Sqrt( -1 * Physics2D.gravity.y * jumpHeight)), ForceMode2D.Impulse);
        }

        //movement in horizontal direction is detected and then moved by the player
        float moveBy = Input.GetAxis("Horizontal");
        Vector2 pos = transform.position;
        pos.x += speed * moveBy * Time.deltaTime;
        transform.position = pos;

        //gets the direction the player is facing at 1 = right, -1 = left
        if (moveBy != 0) {
            _direction = moveBy < 0 ? -1: 1;
        }

        //checking if player didn't fall into the void
        if (pos.y < -5) {
            _source.PlayOneShot(hurtSound);
            transform.position = new Vector2(-2, 0);
            if (_playerHealth != 1) {
                _playerHealth--;
                UpdateHeart();
            } else {
                _playerHealth--;
                UpdateHeart();
                PlayerDeath();
                return;
            }
        }
        
        //checks if player reached the end
        if (pos.x > 237) {
            _source.PlayOneShot(winSound);
            levelPanel.SetActive(true);
            _playerConcussed = true;
            _animator.SetFloat("MoveX", 0.5f);
            _animator.SetFloat("MoveY", -0.5f);
            OnLvlUp?.Invoke(false);
            Invoke("NewLevel", 4f);
            return;
        }

        //play animation based off movement
        if (_isJumping) {
            if (_direction == 1) {
                _animator.SetFloat("MoveX", 0);
                _animator.SetFloat("MoveY", 1f);
            }
            else {
                _animator.SetFloat("MoveX", 0);
                _animator.SetFloat("MoveY", -1f);
            }
        }
        else {
            PlayAnimation(moveBy);
        } 
    }

    // Triggered by collision and col is the object that it was collided with
    void OnCollisionEnter2D() {
        //checks if the player is jumping then make it false to allow to jump again
        if (_isJumping)
            _isJumping = false;
    }

    void UpdateHeart()
    {
        healthText.text = _playerHealth.ToString();
        heartImage.sprite = _playerHealth < 2 ? redHeart : greenHeart;
    }

    //when barrel calls collision this is called, stunning a player for a couple of seconds
    public void ConcussPlayer() {
        _source.PlayOneShot(breakSound);
        //checks if player has power up
        if (_playerHammerAbility > 0) {
            HammerAbility();
            return; //cancels concuss
        }

        _source.PlayOneShot(hurtSound);
        _playerConcussed = true;
        if (_playerHealth != 1) {
            _playerHealth--;
            UpdateHeart();
        } else {
            _playerHealth--;
            UpdateHeart();
            PlayerDeath();
            return;
        } 

        //stops movement
        _direction = 1;
        _animator.SetFloat("MoveX", 0);
        _animator.SetFloat("MoveY", 0);

        //hits the player back
        _rigidBody.linearVelocity = Vector3.zero;
        _rigidBody.AddForce(new Vector2(-_direction*1.5f, Mathf.Sqrt( -1 * Physics2D.gravity.y * 2)), ForceMode2D.Impulse);
        //tracks on what count it is on
        _concussCount++;
        //calls the method 2s later (sourced from documentation)
        Invoke("RemoveConcussion", 2f);
        
    }

    //allows the player to move again and reanimates them
    void RemoveConcussion() {
        //removes 1 count
        _concussCount--;
        //if its 0 return movement
        if(_concussCount == 0) {
            PlayAnimation(0);
            _playerConcussed = false;
        }
    }

    //uses the power up and plays animation
    void HammerAbility() {
        _playerHammerAbility--;
        hammerText.text = (_playerHammerAbility+"X");
        _animator.SetTrigger("Strike");
    }

    //players death
    void PlayerDeath() {
        _playerConcussed = true;
        _source.PlayOneShot(failSound);
        deathPanel.SetActive(true);
        _animator.SetTrigger("Death");
    }

    //freezing game after death animation finishes
    void onDeathFinish(){
        Time.timeScale = 0;
    }

    //called by hp to give more health
    public void AddHealth() {
        _source.PlayOneShot(pickupSound);
        _playerHealth++;
        UpdateHeart();
    }

    //called by hammer to give more power ups
    public void AddPowerUp() {
        _source.PlayOneShot(pickupSound);
        _playerHammerAbility++;
        hammerText.text = (_playerHammerAbility+"X");
    }

    //plays animation based on current movement, either 0,1,-1
    private void PlayAnimation(float moveBy)
    {
        switch (moveBy)
        {
            case 0 when _direction == 1: //checks what direction to idle in
                _animator.SetFloat("MoveX", 0.5f);
                _animator.SetFloat("MoveY", 0.5f);
                break;
            case 0:
                _animator.SetFloat("MoveX", -0.5f);
                _animator.SetFloat("MoveY", -0.5f);
                break;
            case < 0: //left walk
                _animator.SetFloat("MoveX", -1);
                _animator.SetFloat("MoveY", 0);
                break;
            default: //right walk
                _animator.SetFloat("MoveX", 1);
                _animator.SetFloat("MoveY", 0);
                break;
        }
    }

    //this sets up the new level by reset player location and re-enabling movement
    void NewLevel() {
        levelPanel.SetActive(false);
        _playerConcussed = false;
        transform.position = new Vector2(-2,0);
        PlayAnimation(0);
        OnLvlUp?.Invoke(true);
    }
}
