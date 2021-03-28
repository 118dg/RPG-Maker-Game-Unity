using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCMove
{
    [Tooltip("NPCMove를 체크하면 NPC가 움직임")]
    public bool NPCmove;
    public string[] direction;
    [Range(1, 5)] [Tooltip("1: 천천히, 2: 조금 천천히, 3: 보통, 4: 빠르게, 5:연속적으로")]
    public int frequency;
}

public class NPCManager : MovingObject
{
    [SerializeField]
    public NPCMove npc;

    // Start is called before the first frame update
    void Start()
    {
        queue = new Queue<string>();
        SetMove();
    }

    public void SetMove()
    {
        StartCoroutine(MoveCoroutine());
    }
    public void SetNotMove()
    {
        StopAllCoroutines();
    }

    IEnumerator MoveCoroutine()
    {
        if(npc.direction.Length != 0)
        {
            for(int i = 0; i < npc.direction.Length; i++)
            { 
                //실질적인 이동 구간
                yield return new WaitUntil(() => queue.Count < 2); //화살표 옆의 조건이 true가 될 때까지 기다렸다가 true가 되는 순간(MovingObject의 Coroutine이 끝나는 순간 실행
                base.Move(npc.direction[i], npc.frequency);

                if (i == npc.direction.Length - 1)
                    i = -1; //무한 반복!
            }
        }
    }
}
