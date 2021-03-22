using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomGenerator : MonoBehaviour
{
    
    
    [Header("房间生成位置")]
    public Direction direction;

    [Header("房间信息")]
    public Room roomPrefab;
    public int roomNum;
    //使用Color可以在可视化界面中直接调整【注意：默认的Alpha值是0，要记得首先调整，否则看不到】
    public Color startColor, endColor;

    //自定义最后一个房间
    private Room endRoom;

    [Header("位置控制")]
    public Transform generatorPoint;
    //每个房间的生成偏移量
    
    public static float xOffset=18;
    public static float yOffset=9;

    public LayerMask roomLayer;

    public List<Room> roomLst = new List<Room>();

    //最远房间距离
    public int maxStep;

    //最远的房间
    public List<Room> farRoomLst = new List<Room>();
    //最远的房间附近的房间
    public List<Room> lessFarRoomLst = new List<Room>();
    //上面两个列表中，只有单独门的房间
    public List<Room> oneWayRoomLst = new List<Room>();
    //墙壁类型
    public WallType wallType;

    private void Start()
    {
        CreatRoom(roomPrefab, generatorPoint, xOffset, yOffset);
    }
    private void Update()
    {
        //按下任意按键 
        if (Input.anyKeyDown)
        {
            //重新加载场景           //参数：当前场景名
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }


    //生成房间
    private void CreatRoom(Room roomPrefab,Transform point,float xOffset, float yOffset)
    {
        for (int i = 0; i < roomNum; i++)
        {
            //生成一个房间然后添加一个列表，方便管理
            roomLst.Add(Instantiate(roomPrefab, point.position, Quaternion.identity));
            //生成后，生成点随机变化
            ChangePointPos(xOffset, yOffset);
        }

        //生成房间完成，更改房间设置
        //更改初始房间的颜色
        roomLst[0].GetComponent<SpriteRenderer>().color = startColor;

        //循环查找离初始房间最远的房间
        //先将结束房间定义为初始房间的位置，再去查找离这个位置最远的房间的位置，最后这个最远的房间就是结束的房间
        endRoom = roomLst[0];
        //遍历所有房间，开始查找
        foreach (var room in roomLst)
        {
            //使用选择排序，获得离初始房间最远的房间，
            //此方法判断房间的距离【具体原理请查看API手册】如果比原来的远就覆盖，不然就跳过，最终得出的房间就是离初始房间最远的房间
            //if (room.transform.position.sqrMagnitude>endRoom.transform.position.sqrMagnitude)
            //{
            //    endRoom = room;
            //}
            SetupRoom(room, room.transform.position);
        }
        FindEndRoom();
        //遍历完成后，最终覆盖endRoom的Room就是最后得到的最远房间
        endRoom.GetComponent<SpriteRenderer>().color = endColor;
    }
    //随机生成房间位置【上，下，左，右】
    private void ChangePointPos(float xOffset,float yOffset)
    {
        //do while 至少执行一次的循环
        do
        {
            //生成随机位置（0到4不包括4）
            direction = (Direction)Random.Range(0, 4);
            switch (direction)
            {
                case Direction.Up:
                    generatorPoint.position += new Vector3(0, yOffset);
                    break;
                case Direction.Down:
                    generatorPoint.position += new Vector3(0, -yOffset);
                    break;
                case Direction.Left:
                    generatorPoint.position += new Vector3(-xOffset, 0);
                    break;
                case Direction.Right:
                    generatorPoint.position += new Vector3(xOffset, 0);
                    break;
                default:
                    break;
            }
            //检查选择的点是否已经存在房间，如果已经存在，就再选一次，直到选到没有房间的点为止
            //检测的点，检测的范围，检测的图层 如果点在图层里，并处于范围之中，就返回True
        } while (Physics2D.OverlapCircle(generatorPoint.position, 0.2f, roomLayer));

    }

    //检测房间周围是否有房间，有的话就要开一个门
    public void SetupRoom(Room room,Vector3 roomPosition)
    {
        room.isActiveByDoorDict.Add(Direction.Up, Physics2D.OverlapCircle(roomPosition + new Vector3(0, yOffset, 0), 0.2f, roomLayer));
        room.isActiveByDoorDict.Add(Direction.Down, Physics2D.OverlapCircle(roomPosition + new Vector3(0, -yOffset, 0), 0.2f, roomLayer));
        room.isActiveByDoorDict.Add(Direction.Left, Physics2D.OverlapCircle(roomPosition + new Vector3(-xOffset, 0, 0), 0.2f, roomLayer));
        room.isActiveByDoorDict.Add(Direction.Right, Physics2D.OverlapCircle(roomPosition + new Vector3(xOffset, 0, 0), 0.2f, roomLayer));
    }

    //找到最远的房间
    public void FindEndRoom()
    {
        //寻找最远房间的距离
        for (int i = 0; i < roomLst.Count; i++)
        {
         

            if (roomLst[i].stepToStart>maxStep)
            {
                
                maxStep = roomLst[i].stepToStart;
            }
        }

        foreach (var room in roomLst)
        {
            //得到刚刚获得了那个最远房间
            if (room.stepToStart==maxStep)
            {
                //最远房间添加进列表
                farRoomLst.Add(room);
            }
            if (room.stepToStart==maxStep-1)
            {
                //最远房间附近房间添加进列表
                lessFarRoomLst.Add(room);
            }
        }
        //找到最远房间的单向门房间
        for (int i = 0; i < farRoomLst.Count; i++)
        {
            if (farRoomLst[i].doorNum==1)
            {
                oneWayRoomLst.Add(farRoomLst[i]);
            }
        }
        //找到最远房间附近房间的单向门房间
        for (int i = 0; i < lessFarRoomLst.Count; i++)
        {
            if (lessFarRoomLst[i].doorNum == 1)
            {
                oneWayRoomLst.Add(lessFarRoomLst[i]);
            }
        }
        //如果符合条件的房间比较多，就随机一个当最终房间
        if (oneWayRoomLst.Count!=0)
        {
            endRoom = oneWayRoomLst[Random.Range(0, oneWayRoomLst.Count)];
        }
        //如果没有符合条件的房间，就随机一个最远的房间作为最终房间
        else
        {
            endRoom = farRoomLst[Random.Range(0, farRoomLst.Count)];
        }
        
    }

}


