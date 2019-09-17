using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUserAction
{
    void restart();
    void move_boat();
    void click_character(int character_num);
}

public class UserGUI : MonoBehaviour
{
    private IUserAction action;
    public static int situation = 0;
    //private int count = 0;
    public GUISkin MY_GUI;
    // Start is called before the first frame update
    void Start()
    {
        action = SSDirector.getInstance().currentSceneController as IUserAction;
        //action.move_boat();
    }

    void OnGUI()
    {
        GUI.skin = MY_GUI;
        if (situation == -1)
        {
            GUI.Label(new Rect(Screen.width * 7 / 16, Screen.height * (7 / 16),
                Screen.width / 8, Screen.height / 8), "You lose!");
            if (GUI.Button(new Rect(Screen.width * 7 / 16, Screen.height / 8,
                Screen.width / 8, Screen.height / 8), "Restart"))
            {
                situation = 0;
                Debug.Log("Game restart");
                action.restart();
            }
        }
        else if(situation == 1)
        {
            GUI.Label(new Rect(Screen.width * 7 / 16, Screen.height * (7 / 16),
                Screen.width / 8, Screen.height / 8), "You win!");
            if (GUI.Button(new Rect(Screen.width * 7 / 16, Screen.height / 8,
                Screen.width / 8, Screen.height / 8), "Restart"))
            {
                situation = 0;
                Debug.Log("Game restart");
                action.restart();
            }
        }
        else
        {
            if (GUI.Button(new Rect(Screen.width * 7 / 16, Screen.height / 8,
                Screen.width / 8, Screen.height / 8), "Restart"))
            {
                Debug.Log("Game restart");
                //action.move_boat();
                /*if (count == 0)
                {
                    count = 1;
                    action.take_boat(0);
                }
                else
                {
                    count = 0;
                    action.to_land(0);
                }  */ 
                action.restart();
            }
        }
    }
    
        // Update is called once per frame
    void Update()
    {
        
    }
}
