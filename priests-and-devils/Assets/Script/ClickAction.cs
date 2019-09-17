using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAction : MonoBehaviour
{
    private IUserAction action;
    private int character_num;
    private bool moveable;

    // Start is called before the first frame update
    void Start()
    {
        action = SSDirector.getInstance().currentSceneController as IUserAction;
        moveable = true;
    }

    public void set_character_num(int num)
    {
        character_num = num;
    }
    public void set_moveable(bool move)
    {
        moveable = move;
    }

    void OnMouseDown()
    {
        if (!moveable) return;
        if (gameObject.name == "boat")
        {
            action.move_boat();
        }
        else
        {
            action.click_character(character_num);
        }
    }
}
