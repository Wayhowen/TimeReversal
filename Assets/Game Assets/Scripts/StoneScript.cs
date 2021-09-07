using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(Rigidbody2D))]
public class StoneScript : MonoBehaviour
{
    private BoxCollider2D cldr2d;
    private Rigidbody2D rgb2d;
    private Transform trnsfrm;

    void Start()
    {
        getComponents();
        float position_y = trnsfrm.position.y;
        SetupActivationCollider(position_y);
        rgb2d.bodyType = RigidbodyType2D.Static;
    }

    void getComponents()
    {
        cldr2d = GetComponent<BoxCollider2D>();
        rgb2d = GetComponent<Rigidbody2D>();
        trnsfrm = GetComponent<Transform>();
    }

    void SetupActivationCollider(float collider_length)
    {
        cldr2d.offset = new Vector2(0f, -(collider_length / 2));
        cldr2d.size = new Vector2(1f, collider_length);
        cldr2d.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            rgb2d.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}
