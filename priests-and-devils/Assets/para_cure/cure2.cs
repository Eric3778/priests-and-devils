using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cure2 : MonoBehaviour
{
    private float x_v;
    private float y_v;
    private float g;
    // Start is called before the first frame update
    void Start()
    {
        x_v = 10f;
        y_v = 10f;
        g = 10f;
}

    // Update is called once per frame
    void Update()
    {
        y_v -= g * Time.deltaTime;
        this.transform.position += new Vector3(Time.deltaTime*x_v, Time.deltaTime*y_v, 0);
    }
}
