using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Vuforia;

public class VirtualButtonEventHandler : MonoBehaviour, IVirtualButtonEventHandler
{
    public VirtualButtonBehaviour[] vbs;
    private IUserAction action;
    public GameObject auto_button;
    public GameObject restart_button;
    // Start is called before the first frame update
    void Start()
    {
        vbs = GetComponentsInChildren<VirtualButtonBehaviour>();
        for (int i = 0; i < vbs.Length; i++)
        {
            vbs[i].RegisterEventHandler(this);
        }
        action = SSDirector.getInstance().currentSceneController as IUserAction;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        switch (vb.VirtualButtonName)
        {
            case "restart":
                Debug.Log("vbutton restart pressed");
                restart_button.GetComponent<MeshRenderer>().material.color = Color.red;
                action.restart();
                break;
            case "auto":
                Debug.Log("vbutton auto pressed");
                auto_button.GetComponent<MeshRenderer>().material.color = Color.red;
                action.auto_move();
                break;
            default:
                break;
        }
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        switch (vb.VirtualButtonName)
        {
            case "restart":
                Debug.Log("vbutton restart released");
                restart_button.GetComponent<MeshRenderer>().material.color = Color.white;
                action.restart();
                break;
            case "auto":
                Debug.Log("vbutton auto released");
                auto_button.GetComponent<MeshRenderer>().material.color = Color.white;
                action.auto_move();
                break;
            default:
                break;
        }
    }
}
