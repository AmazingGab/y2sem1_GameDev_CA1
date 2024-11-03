using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpHeight;
    private Animator _animator;
    private Rigidbody2D rigidBody;
    private bool isJumping = false;
    private int direction = 1;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.SetFloat("MoveX", 0);
        _animator.SetFloat("MoveY", 0.5f);
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && !isJumping) {
            isJumping = true;
            rigidBody.velocity = Vector3.zero;
            rigidBody.AddForce(new Vector2(0, Mathf.Sqrt( -1 * Physics2D.gravity.y * jumpHeight)), ForceMode2D.Impulse);
        }


        float moveBy = Input.GetAxis("Horizontal");
        Vector2 pos = transform.position;
        pos.x += speed * moveBy * Time.deltaTime;
        transform.position = pos;

        if (moveBy != 0) {
            direction = moveBy < 0 ? -1: 1;
        }

        playAnimation(moveBy);


        if (pos.y < -10) {
            transform.position = new Vector2(0,0);
        }
    }


    void OnCollisionEnter2D(Collision2D col) {
        Vector2 point = col.contacts[0].point;
        Vector2 position = transform.position;
        float height = col.collider.bounds.size.y;

        if (isJumping) //&& point.y <position.y-0.4
            isJumping = false;

        if (col.gameObject.tag == "Barrel") {
            Destroy(col.gameObject);
            //remove health
            //animation of concussed
            //stop player movement
        }
    }

    private void playAnimation(float moveBy) {
        if (moveBy == 0) {
            if (direction == 1) {
            _animator.SetFloat("MoveX", 0.5f);
            _animator.SetFloat("MoveY", 0.5f);
            }
           else{
            _animator.SetFloat("MoveX", -0.5f);
            _animator.SetFloat("MoveY", -0.5f);
           }
        }
        else if (moveBy < 0) {
            _animator.SetFloat("MoveX", -1);
            _animator.SetFloat("MoveY", 0);
        }
        else {
            _animator.SetFloat("MoveX", 1);
            _animator.SetFloat("MoveY", 0);
        }
    }

}
