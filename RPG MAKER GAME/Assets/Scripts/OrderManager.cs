using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//npc에게 내릴 명령 지정
public class OrderManager : MonoBehaviour
{
    private PlayerManager thePlayer; //이벤트 도중에 키입력 처리 방지
    private List<MovingObject> characters;
    //NPC A 마을 10마리
    //NPC B 마을 20마리
    // Add(), Remove(), Clear()

    // Start is called before the first frame update
    void Start()
    {
        thePlayer = FindObjectOfType<PlayerManager>();
    }

    public void PreLoadCharacter() //리스트 채우는 함수
    {
        characters = ToList();
    }

    public List<MovingObject> ToList()
    {
        List<MovingObject> tempList = new List<MovingObject>();
        MovingObject[] temp = FindObjectsOfType<MovingObject>(); //MovingObejct가 달린 모든 오브젝트 찾아서 배열에 넣어줌

        for (int i=0; i<temp.Length; i++)
        {
            tempList.Add(temp[i]);
        }
        return tempList;
    }

    public void NotMove()
    {
        thePlayer.notMove = true;
    }

    public void Move()
    {
        thePlayer.notMove = false;
    }

    //npc 지정 경로로 움직이도록
    public void Move(string _name, string _dir)
    {
        for (int i = 0; i < characters.Count; i++)
        {
            if (_name == characters[i].characterName)
            {
                characters[i].Move(_dir);
            }
        }
    }

    //npc 보는 방향
    public void Turn(string _name, string _dir)
    {
        for (int i = 0; i < characters.Count; i++)
        {
            if (_name == characters[i].characterName)
            {
                characters[i].animator.SetFloat("DirX", 0);
                characters[i].animator.SetFloat("DirY", 0);
                switch (_dir)
                { 
                    case "UP":
                        characters[i].animator.SetFloat("DirY", 1f);
                        break;
                    case "DOWN":
                        characters[i].animator.SetFloat("DirY", -1f);
                        break;
                    case "LEFT":
                        characters[i].animator.SetFloat("DirX", -1f);
                        break;
                    case "RIGHT":
                        characters[i].animator.SetFloat("DirX", 1f);
                        break;

                }
            }
        }
    }

    //npc 투명하게(사라지게)
    public void SetTransparent(string _name)
    {
        for (int i = 0; i < characters.Count; i++)
        {
            if (_name == characters[i].characterName)
            {
                characters[i].gameObject.SetActive(false);
            }
        }
    }

    //npc 나타나게
    public void SetUnTransparent(string _name)
    {
        for (int i = 0; i < characters.Count; i++)
        {
            if (_name == characters[i].characterName)
            {
                characters[i].gameObject.SetActive(true);
            }
        }
    }

    //npc 통과 가능하게
    public void SetThrough(string _name)
    {
        for (int i = 0; i < characters.Count; i++)
        {
            if (_name == characters[i].characterName)
            {
                characters[i].boxCollider.enabled = false;
            }
        }
    }

    //npc 통과 불가능하게
    public void SetUnThrough(string _name)
    {
        for (int i = 0; i < characters.Count; i++)
        {
            if (_name == characters[i].characterName)
            {
                characters[i].boxCollider.enabled = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
