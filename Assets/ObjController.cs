using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjController : MonoBehaviour
{ 
        public GameObject player;        //Public variable to store a reference to the player game object


        private Vector3 offset;            //Private variable to store the offset distance between the player and camera

    // Start is called before the first frame update
    void Start()
    {
        transform.position = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
