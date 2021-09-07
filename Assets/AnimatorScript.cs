using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorScript : MonoBehaviour
{
    //public PlayerScript player = GameObject.Find("Player");
    private Animator anim;
    private Rigidbody2D rb2d;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerAnimations();
    }

    void UpdatePlayerAnimations()
    {
        Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
        float horizontalVelocity = rb2d.velocity.x;
        float verticalVelocity = rb2d.velocity.y;
        anim.SetFloat("HorizontalSpeed", horizontalVelocity);
        anim.SetFloat("VerticalSpeed", verticalVelocity);
    }

    void UpdateJumpAnimation()
    {
        Debug.Log("ha");
       // anim.SetBool("Grounded", player.GetComponent);
    }
}
