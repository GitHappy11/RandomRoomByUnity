using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    Vector2 movement;

    public float speed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        //切换方向
        if (movement.x!=0)
        {
            transform.localScale = new Vector3(movement.x, 1, 1);
        }
        SwitchAnim();

    }

    //固定帧运行，防止因为电脑机能导致每秒Update的更新次数不稳定
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position+movement*speed*Time.fixedDeltaTime);
    }

    private void SwitchAnim()
    {
        anim.SetFloat("speed", movement.magnitude);
    }

}
