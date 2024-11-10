using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    // Third-Party Code: https://www.youtube.com/watch?v=AoRBZh6HvIk
    // Description: Enables the background to scroll using the camera and creates parallax background scrolling
    // edited to support y movement.

    private float startPosX, length;
    public GameObject cam;
    public float parallaxEffect;

    // Start is called before the first frame update
    void Start()
    {
        //getting starting place and size of the sprite
        startPosX = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //calculating the distance the background has to move.
        float distanceX = cam.transform.position.x * parallaxEffect;
        float distanceY = cam.transform.position.y;
        float movement = cam.transform.position.x * (1 - parallaxEffect);

        transform.position = new Vector3(startPosX + distanceX, distanceY, transform.position.z);

        //move background when player has reached nearly the end.
        if (movement > startPosX + length-6)
            startPosX += length;
        else if (movement < startPosX - length+6)
            startPosX -= length;
    }
}
