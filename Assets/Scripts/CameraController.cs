using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    public Transform camTrans;
    private void Awake()
    {
        if (instance==null)
        {
            instance = this;
        }
    }
    public float speed;
    public Transform target;


    private void Update()
    {
        if (target!=null)
        {
            //从一个坐标移动到另一个坐标，和移动时间[记得要时间修正]
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.position.x, target.position.y, transform.position.z), speed * Time.deltaTime);
        }
        
    }
        

    
    public void ChangeTarget(Transform newTarget)
    {
        target = newTarget;
        
    }
}
