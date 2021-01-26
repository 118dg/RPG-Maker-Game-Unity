using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{

    public static MovingObject instance;

    public float speed;
    public int walkCount;
    private int currentWalkCount;

    private Vector3 vector; //x,y,z좌표

    private BoxCollider2D boxCollider;
    public LayerMask layerMask; //어떤 layer와 충돌했는지 판단
    private Animator animator;

    public float runSpeed;
    private float applyRunSpeed;
    private bool applyRunFlag = false;
    private bool canMove = true;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    /*
    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }*/

    IEnumerator MoveCoroutine() //한 번에 여러 입력X, 텀을 주기 위해서. 다중처리함수처럼 작동.
    {
        //걷고 coroutine으로 끊고를 반복하다보니 한 발로 걷는 것처럼 보이는 문제를 해결하기 위해 while문 추가.
        //걷고 coroutine으로 끊고 반복이 아니고, coroutine은 update에서 한 번만 실행되고 나머지 입력은 coroutine 내부의 while문에서 처리됨.
        while (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0) 
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                applyRunSpeed = runSpeed; //shift 누르면 더 빠르게 이동
                applyRunFlag = true;
            }
            else
            {
                applyRunSpeed = 0;
                applyRunFlag = false;
            }

            vector.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), transform.position.z);
            //vector.x = Input.GetAxisRaw("Horizontal") 값.
            //Input.GetAxisRaw("Horizontal") : 우 방향키가 눌리면 1 리턴, 좌 방향키가 눌리면 -1 리턴
            //상하도 위와 동일.

            if(vector.x != 0) //방향키를 상하+좌우 동시에 누른 경우(대각선), 좌우로 이동하는 경우에는
                vector.y = 0; //무조건 좌우 방향만 보도록. 몸이 상하로 틀어지지 않게.
            

            animator.SetFloat("DirX", vector.x);
            animator.SetFloat("DirY", vector.y);

            RaycastHit2D hit;
            //A지점에서 B지점으로 레이저를 쐈을 때,
            //레이저가 B지점까지 도달하면 hit = Null;
            //레이저가 B지점까지 도달하지 못하고 방해받으면 hit = 방해불;

            Vector2 start = transform.position; //A지점, 캐릭터의 현재 위치 값.
            Vector2 end = start + new Vector2(vector.x * speed * walkCount, vector.y * speed * walkCount); //B지점, 캐릭터가 이동하고자 하는 위치 값.
            //vector.x * speed * walkCount = 1 * 2.4 * 20 = 48 pixel

            boxCollider.enabled = false; //레이저가 캐릭터 자기 자신의 boxCollider에 충돌되는 경우는 제외해야하므로 잠깐 끄기
            hit = Physics2D.Linecast(start, end, layerMask);
            boxCollider.enabled = true;

            if (hit.transform != null) //벽이 있다면
                break; //아래 내용 실행X.

            animator.SetBool("Walking", true); //걷는 모션으로 바꾸기

            //실제 이동이 이루어지는 부분
            while (currentWalkCount < walkCount)
            {
                if (vector.x != 0)
                {
                    transform.Translate(vector.x * (speed + applyRunSpeed), 0, 0);
                }
                else if (vector.y != 0)
                {
                    transform.Translate(0, vector.y * (speed + applyRunSpeed), 0);
                }
                if (applyRunFlag) //shift 눌렀을 땐 두 칸씩 이동하도록 하기 위해 ++ 두 번 되게!
                {
                    currentWalkCount++;
                }
                currentWalkCount++;

                yield return new WaitForSeconds(0.01f);
            }
            currentWalkCount = 0;
        }
        //while문 밖
        animator.SetBool("Walking", false); //서 있는 모션으로 바꾸기
        canMove = true;
    }
    

    // Update is called once per frame
    void Update()
    {
        if (canMove) //MoveCoroutine 함수가 여러 개 동시에 실행되는 것 방지
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                canMove = false;
                StartCoroutine(MoveCoroutine());
            }
        }
    }
}