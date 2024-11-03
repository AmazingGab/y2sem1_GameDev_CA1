using UnityEngine;

public class BarrelScript : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D rigidBody;
    [SerializeField] private float speedX;

    void Start()
    {
        //gets animation and plays the barrel animation
        _animator = GetComponent<Animator>();
        _animator.Play("BarrelSpinning");
        
        //gets rigidbody and sets the velocity
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = new Vector2(speedX, rigidBody.velocity.y);
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
}
