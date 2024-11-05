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
        rigidBody.velocity = new Vector2(speedX, rigidBody.velocity.y);

        //gets player
        player = GameObject.Find("Player").GetComponent<PlayerController>();

        //sound
    }

    
    void Update()
    {
        //makes it still go the same velocity
        rigidBody.velocity = new Vector2(speedX, rigidBody.velocity.y);

        //checks the current position and if its less than 0 it gets deleted
        Vector2 pos = transform.position;
        if (pos.x < 0) {
            Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Player") {
            Destroy(this.gameObject);
            player.concussPlayer();
            //sound
        }
    }
}
