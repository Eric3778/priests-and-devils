using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SituationType : int { Continue, Win, Loss }

public interface IGetSituation
{
    SituationType GetSituation();
}


public class CCJudgement : MonoBehaviour, IGetSituation
{
    public Controller sceneController;
    // Start is called before the first frame update
    void Start()
    {
        sceneController = SSDirector.getInstance().currentSceneController as Controller;
        sceneController.judgement = this;
    }

    public SituationType GetSituation()
    {
        int left_d = 0, left_p = 0, right_d = 0, right_p = 0;
        for (int i = 0; i < 6; i++)
        {
            if (i < 3)
            {
                if (sceneController.get_character_side(i) == -1)
                    right_d += 1;
                else
                    left_d += 1;
            }
            else
            {
                if (sceneController.get_character_side(i) == -1)
                    right_p += 1;
                else
                    left_p += 1;
            }

        }
        if (left_d == 3 && left_p == 3) return SituationType.Win;
        if (((left_d > left_p) && (left_p != 0)) || ((right_d > right_p) && (right_p != 0))) return SituationType.Loss;
        return SituationType.Continue;
    }
}


