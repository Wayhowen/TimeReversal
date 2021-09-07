using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// mechanic influenced by https://gamedevelopment.tutsplus.com/tutorials/how-to-build-a-prince-of-persia-style-time-rewind-system-part-1--cms-26090?_ga=2.250224454.1959373298.1605910750-1776752928.1605207685

public class TimeController : MonoBehaviour
{

    public bool isReversing = false;
    public int framesPerSecond = 5; // says after how many frames do we take a reading of the game object state
    public int rewindTimeSeconds = 20; // time of possible rewind in seconds
    public float timeAvailable; // rewind time available to the player
    public GameObject rewindMarker; // the marker that indicates time rewinding

    private Dictionary<int, List<Keyframe>> keyframes = new Dictionary<int, List<Keyframe>>(); // used to be public, responsible for holding each object position, rotation and anim
    private List<GameObjectProperty> gameObjectsProperties; // used to store each object id, transform and animator

    private readonly float storedFramesPerSecond = 16.0f; // the amount of frames we want to store = fixed update frames/frames we skip
    private int frameCounter = 1; // stores the frame out framesPerSecond in which we are
    private int reverseCounter = 0; // the same as frame counter but for reversing

    private Dictionary<int, Vector3> currentPosition = new Dictionary<int, Vector3>(); // variable used for interpolation when translating character
    private Dictionary<int, Vector3> previousPosition = new Dictionary<int, Vector3>(); // variable used for interpolation when translating character
    private Dictionary<int, Vector3> currentRotation = new Dictionary<int, Vector3>(); // variable used for rotating when rewinding

    private bool firstReverseRun = true; // stores the information whether we are before the first reverse in a cycle of frames
    private bool objectDrivenAnimationOn = true; // whether this script should take control of animating objects

    void Start()
    {
        List<GameObject> gameObjects = GetAllGameObjects();
        gameObjectsProperties = GetAllGameObjectsProperties(gameObjects);
        List<int> gameObjectsKeys = new List<int>();
        foreach (GameObjectProperty go in gameObjectsProperties)
        {
            gameObjectsKeys.Add(go.id);
        }
        keyframes = InitializeKeyframes(ref keyframes, new List<int>(gameObjectsKeys));
        rewindMarker = GameObject.Find("RewindCanvas");
        rewindMarker.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.T) && HasEnoughKeyframes(keyframes) && !IsInAnyMenu())
        {
            // time scale make sure the game starts again on "t" presss
            Time.timeScale = 1.0f;
            isReversing = true;
        }
        else
        {
            isReversing = false;
            firstReverseRun = true;
        }
    }

    void FixedUpdate()
    {
        if (!isReversing)
        {
            rewindMarker.SetActive(false);
            if (frameCounter != framesPerSecond)
            {
                frameCounter += 1;
            }
            else
            {
                frameCounter = 1;
                keyframes = GetAllKeyframes(gameObjectsProperties, ref keyframes);
            }
            TurnObjectDrivenAnimationOn(gameObjectsProperties);
        }
        else if (isReversing)
        {
            rewindMarker.SetActive(true);
            if (reverseCounter > 0)
            {
                reverseCounter -= 1;
            }
            else
            {
                reverseCounter = framesPerSecond;
                RestorePositions(ref keyframes);
            }

            if (firstReverseRun)
            {
                firstReverseRun = false;
                RestorePositions(ref keyframes);
            }

            float interpolation = (float)reverseCounter / (float)framesPerSecond;
            MoveAllObjects(gameObjectsProperties, interpolation);
            AnimateAllObjectsBackwards(gameObjectsProperties, keyframes);
        }

        foreach (List<Keyframe> keyframesList in keyframes.Values)
        {
            if (keyframesList.Count > (storedFramesPerSecond * rewindTimeSeconds))
            {
                RemoveOldestKeyframes(keyframesList);
            }
        }
        timeAvailable = CalculateAvailableTime(keyframes);
    }

    // function that finds all of the game objects that can be moved in a scene
    private List<GameObject> GetAllGameObjects()
    {
        Object[] objectsInScene = Resources.FindObjectsOfTypeAll(typeof(GameObject));
        List<GameObject> gameObjects = new List<GameObject>();

        foreach (GameObject go in objectsInScene as GameObject[])
        {
            if (go is GameObject && go.hideFlags == HideFlags.None)
            {
                HashSet<string> staticObjects = new HashSet<string> { "Finish", "Spikes", "Ladder", "Ground" };
                if (go.GetComponent<Rigidbody2D>() != null && !staticObjects.Contains(go.tag))
                {
                    gameObjects.Add(go);
                }
            }
        }
        return gameObjects;
    }

    // function that gets all the important properties of an object
    private List<GameObjectProperty> GetAllGameObjectsProperties(List<GameObject> gameObjects)
    {
        List<GameObjectProperty> gameObjectsProperties = new List<GameObjectProperty>();

        foreach (GameObject gameObject in gameObjects)
        {
            GameObjectProperty go = new GameObjectProperty(gameObject.GetInstanceID(),
                                                           gameObject.GetComponent<Transform>(),
                                                           gameObject.GetComponent<Animator>());
            gameObjectsProperties.Add(go);
        }

        return gameObjectsProperties;
    }

    // function that creates lists of keyframes for each id in the dictionary
    private Dictionary<int, List<Keyframe>> InitializeKeyframes(ref Dictionary<int, List<Keyframe>> keyframes, List<int> keys)
    {
        foreach (int key in keys)
        {
            keyframes.Add(key, new List<Keyframe>());
        }
        return keyframes;
    }

    // function that returns whether keyframes is long enough to rewind time
    private bool HasEnoughKeyframes(Dictionary<int, List<Keyframe>> keyframes)
    {
        foreach (List<Keyframe> list in keyframes.Values)
        {
            if (list.Count < 2)
            {
                return false;
            }
        }
        return true;
    }

    // function that returns whether there is any other menu open
    private bool IsInAnyMenu()
    {
        if (SceneManager.sceneCount > 1)
        {
            return true;
        }
        return false;
    }

    // function that gets each of the objects position, rotation, scale and animation state
    private Dictionary<int, List<Keyframe>> GetAllKeyframes(List<GameObjectProperty> gameObjectsProperties, ref Dictionary<int, List<Keyframe>> keyframes)
    {
        foreach (GameObjectProperty properties in gameObjectsProperties)
        {
            int state;
            if (properties.animator != null && properties.animator.gameObject.activeInHierarchy)
            {
                state = properties.animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
            }
            else
            {
                state = 0;
            }
            keyframes[properties.id].Add(new Keyframe(properties.transform.position, properties.transform.localScale, state));
        }
        return keyframes;
    }

    // function that stops animator from playing animations
    private void TurnObjectDrivenAnimationOn(List<GameObjectProperty> gameObjectsProperties)
    {
        foreach (GameObjectProperty properties in gameObjectsProperties)
        {
            if (properties.animator != null && !objectDrivenAnimationOn && properties.animator.gameObject.activeInHierarchy)
            {
                // used because the only object with animator is player atm
                properties.animator.SetBool("isRewinding", isReversing);
                objectDrivenAnimationOn = true;
            }
        }
    }

    // function that sets the appriopriate positions for each object to be used in the next rewinding step
    void RestorePositions(ref Dictionary<int, List<Keyframe>> keyframes)
    {
        foreach (KeyValuePair<int, List<Keyframe>> referenceKeyframe in keyframes)
        {
            int lastIndex = referenceKeyframe.Value.Count - 1;
            int secondToLastIndex = referenceKeyframe.Value.Count - 2;

            if (secondToLastIndex >= 0)
            {
                currentPosition[referenceKeyframe.Key] = (referenceKeyframe.Value[lastIndex] as Keyframe).position;
                previousPosition[referenceKeyframe.Key] = (referenceKeyframe.Value[secondToLastIndex] as Keyframe).position;

                currentRotation[referenceKeyframe.Key] = referenceKeyframe.Value[lastIndex].rotation;

                keyframes[referenceKeyframe.Key].RemoveAt(lastIndex);
            }
        }
    }

    // function that moves and rotates all of the objects in rewinding step
    private void MoveAllObjects(List<GameObjectProperty> gameObjectsProperties, float interpolation)
    {
        foreach (GameObjectProperty properties in gameObjectsProperties)
        {
            properties.transform.position = Vector3.Lerp(previousPosition[properties.id], currentPosition[properties.id], interpolation);
            properties.transform.localScale = currentRotation[properties.id];
        }
    }

    // function that animates all of the objects in the rewinding step
    private void AnimateAllObjectsBackwards(List<GameObjectProperty> gameObjectsProperties, Dictionary<int, List<Keyframe>> keyframes)
    {
        foreach (GameObjectProperty properties in gameObjectsProperties)
        {
            if (properties.animator != null)
            {
                // used because the only object with animator is player atm
                if (objectDrivenAnimationOn && properties.animator.gameObject.activeInHierarchy)
                {
                    properties.animator.SetBool("isRewinding", isReversing);
                    objectDrivenAnimationOn = false;
                }
                int lastIndex = keyframes[properties.id].Count - 1;
                int currentAnimHash = keyframes[properties.id][lastIndex].animStateHash;
                if (!AnimatorIsPlaying(properties.animator, currentAnimHash) && properties.animator.gameObject.activeInHierarchy)
                {
                    properties.animator.Play(currentAnimHash);
                }
            }

        }
    }

    // function used to check whether the object animator is playing the animation that hash of is saved in the memory
    bool AnimatorIsPlaying(Animator animator, int animHash)
    {
        if (animator.gameObject.activeInHierarchy)
        {
            return animator.GetCurrentAnimatorStateInfo(0).fullPathHash == animHash;
        }
        return false;
    }

    // function used to remove the oldest key frame from the list
    void RemoveOldestKeyframes(List<Keyframe> keyframes)
    {
        keyframes.RemoveAt(0);
    }

    // function used to calculate the available rewind time for player
    private float CalculateAvailableTime(Dictionary<int, List<Keyframe>> keyframes)
    {
        float keyframesTotal = 0;
        float objects = 0;
        foreach (List<Keyframe> keyframe in keyframes.Values)
        {
            objects += 1;
            keyframesTotal += keyframe.Count;
        }
        return ((keyframesTotal / objects) / storedFramesPerSecond);
    }
}

// class that stores important information for each keyframe
public class Keyframe
{
    public Vector3 position;
    public Vector3 rotation;
    public int animStateHash;

    public Keyframe(Vector3 position, Vector3 rotation, int animStateHash = 0)
    {
        this.position = position;
        this.rotation = rotation;
        this.animStateHash = animStateHash;
    }
}

// class that stores important information for each game object
public class GameObjectProperty
{
    public int id;
    public Transform transform;
    public Animator animator;


    public GameObjectProperty(int id, Transform transform, Animator animator = null)
    {
        this.id = id;
        this.transform = transform;
        this.animator = animator;
    }
}