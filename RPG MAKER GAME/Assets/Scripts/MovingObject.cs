using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public float speed;
    public int walkCount;
    protected int currentWalkCount;

    protected Vector3 vector; //x,y,z좌표


    public BoxCollider2D boxCollider;
    public LayerMask layerMask; //어떤 layer와 충돌했는지 판단
    protected Animator animator;
}