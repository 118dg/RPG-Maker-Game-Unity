using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public string characterName;

    public float speed;
    public int walkCount;
    protected int currentWalkCount;

    private bool notCoroutine = false;

    protected Vector3 vector; //x,y,z좌표

    public Queue<string> queue;

    public BoxCollider2D boxCollider;
    public LayerMask layerMask; //어떤 layer와 충돌했는지 판단
    public Animator animator;

    public void Move(string _dir, int _frequency = 5) //frequency 파라미터를 생략하면 값이 자동으로 5가 된다는 뜻
    {
        queue.Enqueue(_dir);
        if (!notCoroutine)
        {
            notCoroutine = true;
            StartCoroutine(MoveCoroutine(_dir, _frequency));
        }
    }

    IEnumerator MoveCoroutine(string _dir, int _frequency)
    {
        while(queue.Count != 0)
        {
            switch (_frequency)
            {
                case 1:
                    yield return new WaitForSeconds(4f);
                    break;
                case 2:
                    yield return new WaitForSeconds(3f);
                    break;
                case 3:
                    yield return new WaitForSeconds(2f);
                    break;
                case 4:
                    yield return new WaitForSeconds(1f);
                    break;
                case 5:
                    break;
            }

            string direction = queue.Dequeue();

            vector.Set(0, 0, vector.z);

            switch (direction)
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

            animator.SetFloat("DirX", vector.x);
            animator.SetFloat("DirY", vector.y);

            //part12 npc 충돌방지
            while (true)
            {
                bool checkCollisionFlag = CheckCollision();

                if (checkCollisionFlag) //앞에 뭐가 있으면(플레이어랑 부딪히면)
                {
                    animator.SetBool("Walking", false);
                    yield return new WaitForSeconds(1f); //대기
                }
                else //앞에 뭐가 사라지면
                    break; //진행
            }

            animator.SetBool("Walking", true);

            boxCollider.offset = new Vector2(vector.x * 0.7f * speed * walkCount, vector.y * 0.7f * speed * walkCount);
            //speed * walkCount = 48 (pixel) 이고
            //움직이기 전에 움직이려는 방향으로 boxCollider를 48픽셀의 70%만큼 먼저 움직이는 것!
            //player랑 npc가 동시에 같은 방향으로 움직여서 충돌하지 않도록 - part12

            while (currentWalkCount < walkCount)
            {
                transform.Translate(vector.x * speed, vector.y * speed, 0);
                currentWalkCount++;

                if(currentWalkCount == 12)
                {
                    boxCollider.offset = Vector2.zero;
                    //boxCollider 원위치
                }

                yield return new WaitForSeconds(0.01f);
            }
            currentWalkCount = 0;

            if (_frequency != 5) //애니메이션이 이상하게 작동하는 - 한 발로만 움직이는 것처럼 보이는 - 문제 해결
            {
                animator.SetBool("Walking", false);
            }
        }
        animator.SetBool("Walking", false);
        notCoroutine = false;
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