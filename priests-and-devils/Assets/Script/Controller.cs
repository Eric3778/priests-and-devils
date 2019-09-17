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
    private bool boat_moving;
    // Start is called before the first frame update
    void Awake()
    {
        SSDirector director = SSDirector.getInstance();
        director.currentSceneController = this;
        MCharacter = new CharacterModel[6];
        director.currentSceneController.LoadResources();
        boat_moving = false;
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

    public int get_game_situation()   // -1:fail 0:not_end 1:success
    {
        int left_d = 0, left_p = 0, right_d = 0, right_p = 0;
        for (int i = 0; i < 6; i++)
        {
            if(i < 3)
            {
                if (MCharacter[i].get_side() == -1)
                    right_d += 1;
                else
                    left_d += 1;
            }
            else
            {
                if (MCharacter[i].get_side() == -1)
                    right_p += 1;
                else
                    left_p += 1;
            }
            
        }
        if (left_d == 3 && left_p == 3) return 1;
        if (((left_d > left_p)&&(left_p!=0))|| ((right_d > right_p)&&(right_p!=0))) return -1;
        return 0;
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

    public void move_boat()
    {
        Debug.Log("Move boat");
        if (MBoat.is_empty())
            return;
        MBoat.turn_side();
        int[] custom_num = MBoat.get_customs();
        if (custom_num[0] != -1)
            MCharacter[custom_num[0]].turn_side();
        if (custom_num[1] != -1)
            MCharacter[custom_num[1]].turn_side();
        boat_moving = true;
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
        if (!boat_moving) return;
        int[] custom_num = MBoat.get_customs();
        if (custom_num[0] != -1)
            MCharacter[custom_num[0]].move_with_boat();
        if (custom_num[1] != -1)
            MCharacter[custom_num[1]].move_with_boat();
        if (MBoat.move_boat())
        {
            UserGUI.situation = get_game_situation();
            if (UserGUI.situation != 0) stop_all();
            boat_moving = false;
        }
    }

    public void restart()
    {
        MBoat.restart();
        for (int i = 0; i < 6; i++)
            MCharacter[i].restart();
        enable_all();
    }
}

public class CharacterModel
{
    readonly GameObject character;
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
    public void move_with_boat()
    {
        if (!on_boat) return;
        Vector3 move = new Vector3((float)which_side * -20F, 0, 0);
        //dst_pos
        character.transform.position += move* Time.deltaTime;
        //which_side *= -1;
    }

    public void restart()
    {
        on_boat = false;
        which_side = -1;
        character.transform.position = ori_pos;
    }
}

public class BoatModel
{
    readonly GameObject boat;
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

    public bool move_boat()
    {
        //float step = 10 * Time.deltaTime;
        //boat.transform.localPosition = 
        //    Vector3.MoveTowards(boat.transform.localPosition, new Vector3(-4F, 0.25F, 0), step);
        if ((seat[0] == -1) && (seat[1] == -1))
            return false;
        dst_pos = new Vector3(-4F * (float)which_side, 0.25F, 0);
        boat.transform.position = Vector3.MoveTowards(boat.transform.position, dst_pos, 20 * Time.deltaTime);
        if (boat.transform.position.x == dst_pos.x) return true;
        //boat.transform.position = new Vector3(4F * (float)which_side, 0.25F, 0);
        //which_side *= -1;
        return false;
    }
}
