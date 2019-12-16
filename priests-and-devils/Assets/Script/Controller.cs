using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSDirector : System.Object
{
    private static SSDirector _instance;
    public ISceneController currentSceneController { get; set; }

    public static SSDirector getInstance()
    {
        if (_instance == null)
        {
            _instance = new SSDirector();
        }
        return _instance;
    }
}

public interface ISceneController
{
    void LoadResources();
}

public class Controller : MonoBehaviour, ISceneController, IUserAction
{
    private GameObject left_land, right_land, river;
    private CharacterModel[] MCharacter;  
    private BoatModel MBoat;
    private bool[,,] visited;
    public CCActionManager actionManager;
    public CCJudgement judgement;
    Stack<KeyValuePair<int, int>> path;
    bool auto_mode;
    float spendTime;
    // Start is called before the first frame update
    void Awake()
    {
        SSDirector director = SSDirector.getInstance();
        director.currentSceneController = this;
        MCharacter = new CharacterModel[6];
        director.currentSceneController.LoadResources();
        actionManager = gameObject.AddComponent<CCActionManager>() as CCActionManager;
        visited = new bool[4,4,2];
        auto_mode = false;
    }

    public void LoadResources()
    {
        Vector3 left_land_pos = new Vector3(-8F, 0, 0);
        Vector3 right_land_pos = new Vector3(8F, 0, 0);
        Vector3 river_pos = new Vector3(0, -0.5F, 0);
        Vector3 boat_pos = new Vector3(4F, 0.25F, 0);
        Vector3[] charactor_pos = { new Vector3(5.25F, 1.25F, 0), new Vector3(6.25F, 1.25F, 0), new Vector3(7.25F, 1.25F, 0),
                                    new Vector3(8.25F, 1.25F, 0), new Vector3(9.25F, 1.25F, 0), new Vector3(10.25F, 1.25F, 0) };
        left_land = Object.Instantiate(Resources.Load("Land", typeof(GameObject)), left_land_pos, Quaternion.identity, null) as GameObject;
        left_land.name = "left_land";
        right_land = Object.Instantiate(Resources.Load("Land", typeof(GameObject)), right_land_pos, Quaternion.identity, null) as GameObject;
        right_land.name = "right_land";
        river = Object.Instantiate(Resources.Load("River", typeof(GameObject)), river_pos, Quaternion.identity, null) as GameObject;
        river.name = "river";

        MBoat = new BoatModel(boat_pos);

        for (int i = 0; i < 6; i++)
        {
            if (i < 3)
                MCharacter[i] = new CharacterModel(0, charactor_pos[i], i);
            else
                MCharacter[i] = new CharacterModel(1, charactor_pos[i], i);
        }
    }

    public int get_character_side(int num)
    {
        return MCharacter[num].get_side();
    }

    public void change_game_situation()
    {
        UserGUI.situation = judgement.GetSituation();
        if (UserGUI.situation != 0) stop_all();
    }

    public void stop_all()
    {
        for (int i = 0; i < 6; i++)
            MCharacter[i].stop_character();
        MBoat.stop_boat();
    }
    public void enable_all()
    {
        for (int i = 0; i < 6; i++)
            MCharacter[i].enable_character();
        MBoat.enable_boat();
    }

    bool is_valid(int ld, int lp)
    { //判断一个状态是否违反规则（某一边恶魔大于牧师） 
        if ((lp != 0 && ld > lp) || ((3 - lp != 0) && (3 - ld) > (3 - lp))) return false;
        return true;
    }


    Stack<KeyValuePair<int, int> > search(int ld, int lp, int side)
    { //输入一个状态，进行寻路 
        visited[ld,lp,side] = true;
        Stack<KeyValuePair<int, int> > path = new Stack<KeyValuePair<int, int>>();
        if (ld == 3 && lp == 3)
        {  //结束状态，用全0标记 
            path.Push(new KeyValuePair<int, int>(0, 0));
            return path;
        }
        int D = (side == 0) ? ld : (3 - ld);
        int P = (side == 0) ? lp : (3 - lp);
        int next_side = (side == 0) ? 1 : 0;
        for (int d = 0; d <= D; d++)
        {
            for (int p = 0; p <= P; p++)
            {
                if (d + p > 2 || d + p == 0) continue;
                int temp_ld = (side == 0) ? ld - d : ld + d;
                int temp_lp = (side == 0) ? lp - p : lp + p;
                if (!is_valid(temp_ld, temp_lp)) continue; //检查变换之后是否违反规则 
                if (visited[temp_ld,temp_lp,next_side]) continue; //检测变换之后的状态是否已经展开过 
                //Debug.Log("(%d, %d, %d)->(%d, %d, %d)\n", ld, lp, side, temp_ld, temp_lp, next_side);
                path = search(temp_ld, temp_lp, next_side); //递归查找一条路径 
                if (path.Count != 0)
                { //如果返回结果为空，说明是死路 
                    path.Push(new KeyValuePair<int, int>(d, p));
                    return path;
                }
            }
        }
        return path;
    }

    public void auto_move()
    {
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
                for (int k = 0; k < 2; k++)
                    visited[i, j, k] = false;
        int left_d = 0, left_p = 0, right_d = 0, right_p = 0;
        for (int i = 0; i < 6; i++)
        {
            if (i < 3)
            {
                if (get_character_side(i) == -1)
                    right_d += 1;
                else
                    left_d += 1;
            }
            else
            {
                if (get_character_side(i) == -1)
                    right_p += 1;
                else
                    left_p += 1;
            }
        }
        int side = (MBoat.get_side()==-1)?1:0;
        path = search(left_d, left_p, side);
        auto_mode = true;
        this.stop_all();
    } 

    void next_step()
    {
        if (path.Count == 0) return;
        int side = MBoat.get_side();
        KeyValuePair<int, int> move = path.Pop();
        if (move.Key == 0 && move.Value == 0)
        {
            Debug.Log("success");
            return;
        }
        string output = "move " + (move.Key).ToString() + " Devils and " + (move.Value).ToString() + " Priests ";
        string move_str = (side == 0) ? "from right to left" : "from left to right";
        output += move_str;
        Debug.Log(output);
        for(int i = 0; i < 6; i++)
        {
            if (MCharacter[i].get_whether_on_boat())
            {
                to_land(i);
            }
        }
        int curr_d = 0, curr_p = 0;
        for(int i = 0; i < 6; i++)
        {
            if(MCharacter[i].get_side() == side)
            {
                if(i < 3 && curr_d < move.Key)
                {
                    take_boat(i);
                    curr_d+=1;
                }
                if(i >= 3 && curr_p < move.Value)
                {
                    take_boat(i);
                    curr_p += 1;
                }
            }
        }
        move_boat();
    }

    public void move_boat()
    {
        Debug.Log("Move boat");
        if (MBoat.is_empty())
            return;
        MBoat.turn_side();
        int[] custom_num = MBoat.get_customs();
        if (custom_num[0] != -1)
        {
            MCharacter[custom_num[0]].turn_side();
            actionManager.Move(MCharacter[custom_num[0]].character, MCharacter[custom_num[0]].get_dst(), 20);
        }
        if (custom_num[1] != -1)
        {
            MCharacter[custom_num[1]].turn_side();
            actionManager.Move(MCharacter[custom_num[1]].character, MCharacter[custom_num[1]].get_dst(), 20);
        }
        actionManager.Move(MBoat.boat, MBoat.get_dst(), 20);
        this.stop_all();
        Debug.Log(UserGUI.situation);
    }

    public void click_character(int character_num)
    {
        if (MCharacter[character_num].get_whether_on_boat())
            to_land(character_num);
        else
            take_boat(character_num);
    }

    public void take_boat(int character_num)
    {
        if (!MBoat.has_empty() || MCharacter[character_num].get_side() != MBoat.get_side()||
             MCharacter[character_num].get_whether_on_boat())
            return;
        Vector3 boat_seat = MBoat.get_seat(character_num);
        MCharacter[character_num].take_boat(boat_seat);
    }

    public void to_land(int character_num)
    {
        if (!MCharacter[character_num].get_whether_on_boat())
            return;
        MBoat.clear_seat(character_num);
        MCharacter[character_num].to_land();
    }

    // Update is called once per frame
    void Update()
    {
        if (!auto_mode) return;
        spendTime += Time.deltaTime;
        if(spendTime > 1.5)
        {
            spendTime = 0;
            next_step();
        }
    }

    public void restart()
    {
        MBoat.restart();
        auto_mode = false;
        for (int i = 0; i < 6; i++)
            MCharacter[i].restart();
        enable_all();
    }
}

public class CharacterModel
{
    public GameObject character;
    readonly int type;        // devil:0, priest:1
    private int which_side;  // right:-1, left:1
    private bool on_boat;
    readonly int character_num;
    readonly Vector3 ori_pos;
    private Vector3 dst_pos;
    private ClickAction click_action;

    public CharacterModel(int Type, Vector3 Ori_pos, int i)
    {
        ori_pos = Ori_pos;
        on_boat = false;
        which_side = -1;
        type = Type;
        character_num = i;
        if (Type == 0)
        {

            character = Object.Instantiate(Resources.Load("Devil", typeof(GameObject)), ori_pos, Quaternion.identity, null) as GameObject;
            character.name = "devil" + i.ToString();
        }
        else
        {
            character = Object.Instantiate(Resources.Load("Priest", typeof(GameObject)), ori_pos, Quaternion.identity, null) as GameObject;
            character.name = "priest" + (i-3).ToString();
        }
        click_action = character.AddComponent(typeof(ClickAction)) as ClickAction;
        click_action.set_character_num(character_num);
    }
    public void stop_character()
    {
        click_action.set_moveable(false);
    }
    public void enable_character()
    {
        click_action.set_moveable(true);
    }
    public void turn_side()
    {
        which_side *= -1;
    }
    public int get_side()
    {
        return which_side;
    } 
    public void set_whether_on_boat(bool turn)
    {
        on_boat = turn;
    }
    public bool get_whether_on_boat()
    {
        return on_boat;
    }
    public void take_boat(Vector3 seat)
    {
        on_boat = true;
        character.transform.position = seat;
    }
    public void to_land()
    {
        on_boat = false;
        Vector3 curr_pos = ori_pos;
        curr_pos.x = curr_pos.x * (float)which_side * -1;
        character.transform.position = curr_pos;
    }
    public Vector3 get_dst()
    {
        return character.transform.position + new Vector3((float)which_side * -8F, 0, 0);
    }

    public void restart()
    {
        on_boat = false;
        which_side = -1;
        character.transform.position = character.transform.parent.position+ori_pos;
    }
}

public class BoatModel
{
    public GameObject boat;
    private int which_side;  // right:-1, left:1
    readonly Vector3 ori_pos;
    private Vector3 dst_pos;
    readonly Vector3[] relative_pos; 
    private int[] seat;
    readonly ClickAction click_action;

    public BoatModel(Vector3 boat_pos)
    {
        which_side = -1;
        seat = new int[2];
        seat[0] = -1;
        seat[1] = -1;
        relative_pos = new Vector3[2];
        relative_pos[0] = new Vector3(-0.5F, 0.5F, 0);
        relative_pos[1] = new Vector3(0.5F, 0.5F, 0);
        ori_pos = boat_pos;
        boat = Object.Instantiate(Resources.Load("Boat", typeof(GameObject)), boat_pos, Quaternion.identity, null) as GameObject;
        boat.name = "boat";
        click_action = boat.AddComponent(typeof(ClickAction)) as ClickAction;
    }
    public void stop_boat()
    {
        click_action.set_moveable(false);
    }
    public void enable_boat()
    {
        click_action.set_moveable(true);
    }
    public int get_side()
    {
        return which_side;
    }
    public void turn_side()
    {
        which_side*=-1;
    }
    public bool has_empty()
    {
        return ((seat[0] == -1) || (seat[1] == -1));
    }
    public bool is_empty()
    {
        return ((seat[0] == -1) && (seat[1] == -1));
    }
    public int[] get_customs()
    {
        return seat;
    }

    public Vector3 get_seat(int custom_num)
    {
        if (seat[0] == -1)
        {
            seat[0] = custom_num;
            return boat.transform.position + relative_pos[0];
        }
        seat[1] = custom_num;
        return boat.transform.position + relative_pos[1];
    }

    public void clear_seat(int custom_num)
    {
        if (seat[0] == custom_num)
            seat[0] = -1;
        else if (seat[1] == custom_num)
            seat[1] = -1;
    }

    public void restart()
    {
        which_side = -1;
        seat = new int[2];
        seat[0] = -1;
        seat[1] = -1;
        boat.transform.position = ori_pos;
    }
    public Vector3 get_dst()
    {
        return new Vector3(-4F * (float)which_side, 0.25F, 0);
    }
}
