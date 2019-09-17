using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cure1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Rigidbody>().velocity = new Vector3(10, 10, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
