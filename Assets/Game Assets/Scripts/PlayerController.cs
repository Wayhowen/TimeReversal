using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{

    [Range(0, .5f)] public float MovementSmoothing = 0.5f;
    public float JumpForce = 650f;
    public float MoveForce = 8;
    public float MaxSpeed = 15;
    public float timeToMaxSpeed = 6f;
    public float MaxFallingSpeed = 10f;

    public enum Orientation // enum for values of input corresponding to where character should be facing
    {
        Left = -1,
        Right = 1,
    };
    public Orientation orientation = Orientation.Right;

    private Vector3 vec_force = Vector3.zero; // vector that is used as a reference in a smooth damp function
    private bool isJumping = false;
    private bool allowedToClimb = false;
    private bool isDead = false;

    private Rigidbody2D rb2d; //Store a reference to the Rigidbody2D component required to use 2D Physics.
    private Animator anim; // Reference to the animator of character
    private TimeController timeControllerScript; // reference to the script that is controlling the game time
    private GameObject gameController; // reference to the object controlling the game

    // Use this for initialization
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        gameController = GameObject.Find("GameController");
        timeControllerScript = gameController.GetComponent<TimeController>();

        SetupPlayerProperties(rb2d);
    }

    // function that sets all of the player properties when the character is initialized 
    private void SetupPlayerProperties(Rigidbody2D rb2d)
    {
        rb2d.drag = 1f;
        rb2d.mass = 80f;
        rb2d.transform.localScale = new Vector3(0.5f, 1f, 1f);
    }

    private void Update()
    {
        isDead = false; // make sure to set player to alive when the game is on
        UpdatePlayerAnimations();

        if (CanTakeInput(timeControllerScript))
        {
            JumpAction();
        }
    }

    private void FixedUpdate()
    {
        if (CanTakeInput(timeControllerScript))
        {
            MoveAction();
        }
        if (IsFalling() && !allowedToClimb)
        {
            ChangeGravity(rb2d, 2.5f);
            ClampFallingSpeed(rb2d);
        }
        else if (!allowedToClimb)
        {
            ChangeGravity(rb2d, 1f);
        }
    }

    private bool CanTakeInput(TimeController script)
    {
        return (!script.isReversing);
    }

    private void MoveAction()
    {
        //Store the current horizontal input in the float moveHorizontal.
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        SetDirection(moveHorizontal);
        anim.SetFloat("HorizontalSpeed", moveHorizontal);
        float moveVertical = GetLadderClimbValue(allowedToClimb);

        //Use the two store floats to create a new Vector2 variable movement.
        Vector3 targetVelocity = CalculateMoveSpeed(moveHorizontal, moveVertical);
        rb2d.velocity = Vector3.SmoothDamp(rb2d.velocity, targetVelocity, ref vec_force, MovementSmoothing, MaxSpeed, timeToMaxSpeed);
    }

    private void SetDirection(float input)
    {
        if ((input == 1f))
        {
            orientation = Orientation.Right;
            rb2d.transform.localScale = new Vector3((float)orientation/2, 1f, 1f);
        }
        else if (input == -1f)
        {
            orientation = Orientation.Left;
            rb2d.transform.localScale = new Vector3((float)orientation/2, 1f, 1f);
        }
    }

    private float GetLadderClimbValue(bool climbing)
    {
        if (climbing)
        {
            float moveVertical = Input.GetAxisRaw("Vertical");
            return moveVertical;
        }
        return rb2d.velocity.y;
    }

    private Vector2 CalculateMoveSpeed(float moveHorizontal, float moveVertical)
    {
        float horizontalForce = moveHorizontal * MoveForce;
        float verticalForce = rb2d.velocity.y;
        if (!isJumping && allowedToClimb)
        {
           verticalForce = moveVertical * MoveForce;
        }
        return new Vector2(horizontalForce, verticalForce);
    }

    private bool IsFalling()
    {
        if (rb2d.velocity.y < -0.1f)
        {
            return true;
        }
        return false;
    }

    private void ChangeGravity(Rigidbody2D rgbd, float amount)
    {
        if (rb2d.gravityScale != amount)
        {
            rb2d.gravityScale = amount;
        }
    }

    private void ClampFallingSpeed(Rigidbody2D rgbd)
    {
        if (rgbd.velocity.y < -MaxFallingSpeed)
        {
            rgbd.velocity = new Vector2(rgbd.velocity.x, -MaxFallingSpeed);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            isJumping = false;
        }
        else if (other.CompareTag("Ladder"))
        {
            isJumping = false;
            allowedToClimb = true;
            anim.SetBool("allowedToClimb", allowedToClimb);
            rb2d.gravityScale = 0;
        }
        else if (other.CompareTag("Spikes") && !isDead)
        {
            isDead = true;
            gameController.SendMessage("LoadMenu", "death_menu");
        }
        else if (other.CompareTag("Stone") && other.GetType() == typeof(CircleCollider2D) && !isDead)
        {
            isDead = true;
            gameController.SendMessage("LoadMenu", "death_menu");
        }
        else if (other.CompareTag("Finish"))
        {
            gameController.SendMessage("LoadMenu", "win_menu");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            allowedToClimb = false;
            anim.SetBool("allowedToClimb", allowedToClimb);
            rb2d.gravityScale = 1;
            rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
        }
    }

    private void JumpAction()
    {
        if (Input.GetButtonDown("Jump") && !allowedToClimb)
        {
            if (isJumping == false)
            {
                anim.SetTrigger("Jump");
                rb2d.AddForce(rb2d.transform.up * JumpForce, ForceMode2D.Impulse);
                isJumping = true;
            }
        }
    }

    private void UpdatePlayerAnimations()
    {
        Vector2 velocity = rb2d.velocity;
        anim.SetFloat("VerticalSpeed", velocity.y);
        anim.SetBool("Grounded", !(isJumping));
    }
}
