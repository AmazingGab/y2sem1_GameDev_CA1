using UnityEngine;

public class BarrelScript : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    [SerializeField] PlayerController player;
    [SerializeField] private float speedX;

    void Start()
    {
        //gets rigidbody and sets the velocity
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.linearVelocity = new Vector2(speedX, rigidBody.linearVelocity.y);

        //gets player
        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }
    
    void Update()
    {
        //makes it still go the same velocity
        rigidBody.linearVelocity = new Vector2(speedX, rigidBody.linearVelocity.y);

        //checks the current position and if its less than 0 it gets deleted
        Vector2 pos = transform.position;
        if (pos.x < 0 || pos.y < -6) {
            Destroy(this.gameObject);
        }
    }

    //tries to concuss player and destroys after being touched by player
    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Player") {
            Destroy(this.gameObject);
            player.concussPlayer();
        }
    }
}
