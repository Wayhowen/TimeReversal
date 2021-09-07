using UnityEngine;

public class GameController : MonoBehaviour
{
    public float offsetFromSpawnToBorder = -18.5f;
    public float offsetFromGoalToBorder = 5.5f;
    public float bordersLength = 1000f;
    public float bottomBorderOffset = -10f;

    private GameObject spawn;
    private GameObject goal;

    private GameObject leftBorder;
    private GameObject rightBorder;
    private GameObject bottomBorder;

    // Start is called before the first frame update
    void Awake()
    {
        FindGameObjects();

        CreateWorldBorders(spawn, goal);
    }

    // get references to all objects necessary 
    private void FindGameObjects()
    {
        spawn = GameObject.Find("Spawn");
        goal = GameObject.Find("Goal");

        leftBorder = GameObject.Find("LeftWorldBorder");
        rightBorder = GameObject.Find("RightWorldBorder");
        bottomBorder = GameObject.Find("BottomWorldBorder");
    }
    
    // create the borders that will limit the player and the camera
    private void CreateWorldBorders(GameObject spawn, GameObject goal)
    {
        float mapCenter = (leftBorder.transform.position.x + rightBorder.transform.position.y) / 2;
        MoveWorldBorders(spawn.transform.position, goal.transform.position, new Vector3(mapCenter, bottomBorderOffset, 0f));

        Vector2 sideWallHeigth = new Vector2(1f, bordersLength);
        Vector2 bottomWallLength = new Vector2(bordersLength, 1f);
        Vector2 leftWallOffset = new Vector2(offsetFromSpawnToBorder, 0f);
        Vector2 rightWallOffset = new Vector2(offsetFromGoalToBorder, 0f);

        AdjustWorldBorderSizes(sideWallHeigth, bottomWallLength, leftWallOffset, rightWallOffset);
    }

    // move the borders to appriopriate places
    private void MoveWorldBorders(Vector3 leftBorderPositionm, Vector3 rightBorderPosition, Vector3 bottomBorderPosition)
    {
        leftBorder.transform.position = leftBorderPositionm;
        rightBorder.transform.position = rightBorderPosition;
        bottomBorder.transform.position = bottomBorderPosition;
    }

    // adjust the size and the offset of the borders
    private void AdjustWorldBorderSizes(Vector2 sideWallHeigth, Vector2 bottomWallLength, Vector2 leftWallOffset, Vector2 rightWallOffset)
    {
        BoxCollider2D leftBorderCollider = leftBorder.GetComponent<BoxCollider2D>();
        BoxCollider2D rightBorderCollider = rightBorder.GetComponent<BoxCollider2D>();
        BoxCollider2D bottomBorderCollider = bottomBorder.GetComponent<BoxCollider2D>();

        leftBorderCollider.size = sideWallHeigth;
        leftBorderCollider.offset = leftWallOffset;
        rightBorderCollider.size = sideWallHeigth;
        rightBorderCollider.offset = rightWallOffset;
        bottomBorderCollider.size = bottomWallLength;
    }
}
