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
    private LayerMask layerMask;
    private Animator animator;

    public float runSpeed;
    private float applyRunSpeed;
    private bool applyRunFlag = false;
    private bool canMove = true;

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

    IEnumerator MoveCoroutine()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            applyRunSpeed = runSpeed;
            applyRunFlag = true;
        }
        else
        {
            applyRunSpeed = 0;
            applyRunFlag = false;
        }

        vector.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), transform.position.z);

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
        canMove = true;
    }
    

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                canMove = false;
                StartCoroutine(MoveCoroutine());
            }
        }
    }
}