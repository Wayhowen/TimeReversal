using UnityEngine;

// source https://learn.unity.com/tutorial/movement-basics#5c7f8528edbc2a002053b711

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public GameObject player;        //Public variable to store a reference to the player game object
    private Vector3 offset;            //Private variable to store the offset distance between the player and camera
    private float cameraSize = 10f;

    private GameObject leftBorder;
    private GameObject rightBorder;
    private GameObject bottomBorder;

    [SerializeField] private float MIN_X;
    [SerializeField] private float MAX_X;
    [SerializeField] private float MIN_Y;
    [SerializeField] private float MAX_Y;


    // Use this for initialization
    void Start()
    {
        // add vector so that camera is not covering player and enviroment
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        offset = transform.position - player.transform.position;

        // get goal and spawn so that we could restrict camera position later on
        GetWorldObjects();
        CalculateMinMaxCameraPositions();
    }

    private void GetWorldObjects()
    {
        leftBorder = GameObject.Find("LeftWorldBorder");
        rightBorder = GameObject.Find("RightWorldBorder");
        bottomBorder = GameObject.Find("BottomWorldBorder");
    }
    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
        transform.position = player.transform.position + offset;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, MIN_X, MAX_X),
                                         Mathf.Clamp(transform.position.y, MIN_Y, MAX_Y),
                                         transform.position.z);
    }

    private void SetupCamera()
    {
        transform.position = player.transform.position - new Vector3(0.0f, 0.0f, 10f);
        Camera.main.orthographicSize = cameraSize;
    }

    private void CalculateMinMaxCameraPositions()
    {
        float horzExtent = Camera.main.orthographicSize * Screen.width / Screen.height;
        float vertExtent = Camera.main.orthographicSize;

        BoxCollider2D leftBorderCollider = leftBorder.GetComponent<BoxCollider2D>();
        BoxCollider2D rightBorderCollider = rightBorder.GetComponent<BoxCollider2D>();
        MIN_X = leftBorder.transform.position.x + leftBorderCollider.offset.x + horzExtent + (leftBorderCollider.size.x/2);
        MIN_Y = -1 + vertExtent;
        MAX_X = rightBorder.transform.position.x + rightBorderCollider.offset.x - horzExtent - (leftBorderCollider.size.x/2);
        MAX_Y = rightBorder.transform.position.y + leftBorder.GetComponent<BoxCollider2D>().offset.y;
    }
}