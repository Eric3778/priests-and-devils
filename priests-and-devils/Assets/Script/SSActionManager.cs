using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SSActionEventType : int { Started, Competeted}

public interface ISSActionCallback
{
    void SSActionEvent(SSAction source,
        SSActionEventType events = SSActionEventType.Competeted, 
        int intParam = 0,
        string strParam = null,
        GameObject objectParam = null);
}

public class SSAction : ScriptableObject
{
    public bool enable = true;
    public bool destory = false;

    public GameObject gameobject { get; set; }
    public Transform transform { get; set; }
    public ISSActionCallback callback { get; set; }

    protected SSAction() { }
    // Start is called before the first frame update
    public virtual void Start()
    {
        throw new System.NotImplementedException();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        throw new System.NotImplementedException();
    }
}


public class CCMoveToAction : SSAction
{
    public Vector3 target;
    public float speed;

    public static CCMoveToAction GetSSAction(Vector3 target, float speed)
    {
        CCMoveToAction action = ScriptableObject.CreateInstance<CCMoveToAction>();
        action.target = target;
        action.speed = speed;
        return action;
    }

    public override void Update()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, target, speed * Time.deltaTime);
        if(this.transform.position == target)
        {
            this.destory = true;
            this.callback.SSActionEvent(this);
        }
    }
    public override void Start()
    {
    }
}

public class SSActionManager : MonoBehaviour
{

    private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();
    private List<SSAction> waitingAdd = new List<SSAction>();
    private List<int> waitingDelete = new List<int>();
    protected void Update()
    {
        foreach (SSAction ac in waitingAdd)
        {
            actions[ac.GetInstanceID()] = ac;
        }
        waitingAdd.Clear();
        foreach (KeyValuePair<int, SSAction> kv in actions)
        {
            SSAction ac = kv.Value;
            if (ac.destory)
            {
                waitingDelete.Add(ac.GetInstanceID());
            }
            else if(ac.enable) {
                ac.Update();
            }
        }
        foreach (int key in waitingDelete)
        {
            SSAction ac = actions[key];
            actions.Remove(key);
            DestroyObject(ac);
        }
        waitingDelete.Clear();
    }

    public void RunAction(GameObject gameobject, SSAction action, ISSActionCallback manager)
    {
        action.gameobject = gameobject;
        action.transform = gameobject.transform;
        action.callback = manager;
        waitingAdd.Add(action);
        action.Start();
    }

    void Start() {
    }
}

public class CCActionManager: SSActionManager, ISSActionCallback
{
    public Controller sceneController;
    public CCMoveToAction move_action;

    protected new void Start()
    {
        Debug.Log("action start");
        sceneController = SSDirector.getInstance().currentSceneController as Controller;
        sceneController.actionManager = this;
    }


    public void Move(GameObject thing, Vector3 target, float speed)
    {
        move_action = CCMoveToAction.GetSSAction(target, speed);
        this.RunAction(thing, move_action, this);
    }

    public void SSActionEvent(SSAction source,
        SSActionEventType events = SSActionEventType.Competeted,
        int intParam = 0,
        string strParam = null,
        GameObject objectParam = null)
    {
        sceneController.enable_all();
        sceneController.change_game_situation();
        Debug.Log("move finish");
    }
}