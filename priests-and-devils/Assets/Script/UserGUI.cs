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
    public static SituationType situation = SituationType.Continue;
    public GUISkin MY_GUI;
    // Start is called before the first frame update
    void Start()
    {
        action = SSDirector.getInstance().currentSceneController as IUserAction;
    }

    void OnGUI()
    {
        GUI.skin = MY_GUI;
        if (situation == SituationType.Loss)
            GUI.Label(new Rect(Screen.width * 7 / 16, Screen.height * (7 / 16),
                Screen.width / 8, Screen.height / 8), "You lose!");
        else if(situation == SituationType.Win)
            GUI.Label(new Rect(Screen.width * 7 / 16, Screen.height * (7 / 16),
                Screen.width / 8, Screen.height / 8), "You win!");
        if (GUI.Button(new Rect(Screen.width * 7 / 16, Screen.height / 8,
                Screen.width / 8, Screen.height / 8), "Restart"))
        {
            situation = SituationType.Continue;
            Debug.Log("Game restart");
            action.restart();
        }
    }
}
