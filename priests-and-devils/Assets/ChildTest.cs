using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildTest : MonoBehaviour
{
    private Transform m_Transform;
    private GameObject[] devils;
    private GameObject[] priests;
    private GameObject left_land, right_land, river,boat;
    // Start is called before the first frame update
    void Start()
    {
        devils = new GameObject[3];
        priests = new GameObject[3];
        m_Transform = gameObject.GetComponent<Transform>();
        devils[0] = GameObject.Find("devil0");
        devils[1] = GameObject.Find("devil1");
        devils[2] = GameObject.Find("devil2");
        priests[0] = GameObject.Find("priest0");
        priests[1] = GameObject.Find("priest1");
        priests[2] = GameObject.Find("priest2");
        left_land = GameObject.Find("left_land");
        right_land = GameObject.Find("right_land");
        river = GameObject.Find("river");
        boat = GameObject.Find("boat");
        for (int i = 0; i < 3; i++)
        {
            devils[i].transform.parent = this.transform;
            priests[i].transform.parent = this.transform;
        }
        left_land.transform.parent = this.transform;
        right_land.transform.parent = this.transform;
        river.transform.parent = this.transform;
        boat.transform.parent = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 3; i++)
        {
            devils[i].transform.parent = this.transform;
            priests[i].transform.parent = this.transform;
        }
        left_land.transform.parent = this.transform;
        right_land.transform.parent = this.transform;
        river.transform.parent = this.transform;
        boat.transform.parent = this.transform;
    }
}
