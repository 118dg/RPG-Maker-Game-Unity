using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public float speed;
    public int walkCount;
    protected int currentWalkCount;

    protected bool npcCanMove = true;

    protected Vector3 vector; //x,y,z좌표


    public BoxCollider2D boxCollider;
    public LayerMask layerMask; //어떤 layer와 충돌했는지 판단
    public Animator animator;

    protected void Move(string _dir, int _frequency)
    {
        StartCoroutine(MoveCoroutine(_dir, _frequency));
    }

    IEnumerator MoveCoroutine(string _dir, int _frequency)
    {
        npcCanMove = false;
        vector.Set(0, 0, vector.z);

        switch (_dir)
        {
            case "UP":
                vector.y = 1f;
                break;
            case "DOWN":
                vector.y = -1f;
                break;
            case "RIGHT":
                vector.x = 1f;
                break;
            case "LEFT":
                vector.x = -1f;
                break;
        }

        //animator 생략 (part10 20:00 - NPC)
        animator.SetFloat("DirX", vector.x);
        animator.SetFloat("DirY", vector.y);
        animator.SetBool("Walking", true);

        while (currentWalkCount < walkCount)
        {
            transform.Translate(vector.x * speed, vector.y * speed, 0);
            currentWalkCount++;

            yield return new WaitForSeconds(0.01f);
        }
        currentWalkCount = 0;

        if(_frequency != 5) //애니메이션이 이상하게 작동하는 - 한 발로만 움직이는 것처럼 보이는 - 문제 해결
        {
            animator.SetBool("Walking", false);
        }

        npcCanMove = true;
    }

    protected bool CheckCollision()
    {
        RaycastHit2D hit;
        //A지점에서 B지점으로 레이저를 쐈을 때,
        //레이저가 B지점까지 도달하면 hit = Null;
        //레이저가 B지점까지 도달하지 못하고 방해받으면 hit = 방해물;

        Vector2 start = transform.position; //A지점, 캐릭터의 현재 위치 값.
        Vector2 end = start + new Vector2(vector.x * speed * walkCount, vector.y * speed * walkCount); //B지점, 캐릭터가 이동하고자 하는 위치 값.
                                                                                                       //vector.x * speed * walkCount = 1 * 2.4 * 20 = 48 pixel

        boxCollider.enabled = false; //레이저가 캐릭터 자기 자신의 boxCollider에 충돌되는 경우는 제외해야하므로 잠깐 끄기
        hit = Physics2D.Linecast(start, end, layerMask);
        boxCollider.enabled = true;

        if (hit.transform != null) //벽이 있다면 (앞에 뭐가 있다면)
            return true; //true 반환
        return false; //앞에 뭐가 없으면 false 반환
    }
}