using UnityEngine;

public class BarrelScript : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D rigidBody;
    [SerializeField] private float speedX;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.Play("BarrelSpinning");
        
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = new Vector2(speedX, rigidBody.velocity.y);
    }

    
    void Update()
    {
        rigidBody.velocity = new Vector2(speedX, rigidBody.velocity.y);
    }
}
