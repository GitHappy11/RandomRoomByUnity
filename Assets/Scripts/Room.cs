using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{

    public List<Door> doorLst = new List<Door>();
    public Dictionary<Direction, bool> isActiveByDoorDict = new Dictionary<Direction, bool>();

    public int stepToStart;
    public Text txtStepToStart;
    public int doorNum=0;

    private void Awake()
    {
        UpdateRoom();
    }

    private void Start()
    {
        ShowDoor();
    }

    private void ShowDoor()
    {
        for (int i = 0; i < doorLst.Count; i++)
        {
            foreach (var dict in isActiveByDoorDict)
            {
                if (doorLst[i].direction==dict.Key)
                {
                    doorLst[i].gameObject.SetActive(dict.Value);
                    if (dict.Value==true)
                    {
                        doorNum += 1;
                    }
                }
            }
        }
        
    }

    public void UpdateRoom()
    {
        stepToStart = (int)(Mathf.Abs(transform.position.x /RoomGenerator.xOffset) + Mathf.Abs(transform.position.y / RoomGenerator.yOffset));
        txtStepToStart.text = stepToStart.ToString()+"--"+doorNum.ToString();
       
    }

    //代码量过多，暂时不做 详情请看相关视频P4
    public void CreatWall()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CameraController.instance.ChangeTarget(transform);
        }
    }



}
