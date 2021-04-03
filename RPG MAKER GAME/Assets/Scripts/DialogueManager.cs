using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    #region Singleton
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
    }
    #endregion Singleton

    public Text text;
    public SpriteRenderer rendererSprite;
    public SpriteRenderer rendererDialogueWindow;

    public List<string> listSentences;
    public List<Sprite> listSprites;
    public List<Sprite> listDialogueWindows;

    private int count;

    public Animator animSprite;
    public Animator animDialogueWindow;

    public string typeSound;
    public string enterSound;

    private AudioManager theAudio;
    private OrderManager theOrder;

    public bool talking = false;
    private bool keyActivated = false;

    //public bool talking;

    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        text.text = "";
        listSentences = new List<string>();
        listSprites = new List<Sprite>();
        listDialogueWindows = new List<Sprite>();
        theAudio = FindObjectOfType<AudioManager>();
        theOrder = FindObjectOfType<OrderManager>();
    }

    public void showDialogue(Dialogue dialogue)
    {
        talking = true;

        theOrder.NotMove();

        for (int i = 0; i < dialogue.sentences.Length; i++)
        {
            listSentences.Add(dialogue.sentences[i]);
            listSprites.Add(dialogue.sprites[i]);
            listDialogueWindows.Add(dialogue.dialogueWindows[i]);
        }

        animSprite.SetBool("Appear", true);
        animDialogueWindow.SetBool("Appear", true);
        StartCoroutine(StartDialogueCoroutine());
    }

    public void ExitDialogue()
    {
        text.text = "";
        count = 0;
        listSentences.Clear();
        listSprites.Clear();
        listDialogueWindows.Clear();
        animSprite.SetBool("Appear", false);
        animDialogueWindow.SetBool("Appear", false);

        talking = false;
        theOrder.Move();
    }

    IEnumerator StartDialogueCoroutine()
    {
        if (count > 0)
        {
            //대사 바가 다를 경우 (캐릭터 이미지도 다름)
            if (listDialogueWindows[count] != listDialogueWindows[count - 1]) //dialogueWindow 이미지가 다르다면 -> 교체
            {
                animDialogueWindow.SetBool("Appear", false);
                animSprite.SetBool("Change", true);
                yield return new WaitForSeconds(0.2f);
                rendererDialogueWindow.GetComponent<SpriteRenderer>().sprite = listDialogueWindows[count];
                rendererSprite.GetComponent<SpriteRenderer>().sprite = listSprites[count];
                animDialogueWindow.SetBool("Appear", true);
                animSprite.SetBool("Change", false);
            }
            else
            {
                //대사 바는 같은데 캐릭터 이미지가 다를 경우
                if (listSprites[count] != listSprites[count - 1]) //sprite 이미지가 다르다면 -> 교체
                {
                    animSprite.SetBool("Change", true);
                    yield return new WaitForSeconds(0.1f);
                    rendererSprite.GetComponent<SpriteRenderer>().sprite = listSprites[count];
                    animSprite.SetBool("Change", false);
                }
                //대사 바랑 캐릭터 이미지가 모두 같튼 경우
                else
                {
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }
        else //count <= 0일 경우
        {
            rendererDialogueWindow.GetComponent<SpriteRenderer>().sprite = listDialogueWindows[count];
            rendererSprite.GetComponent<SpriteRenderer>().sprite = listSprites[count];
        }

        keyActivated = true;
        for (int i = 0; i < listSentences[count].Length; i++)
        {
            text.text += listSentences[count][i]; //listSentences[count] 문장의 한 글자씩 출력
            if(i % 7 == 1)
            {
                theAudio.Play(typeSound);
            }
            yield return new WaitForSeconds(0.01f); //한 글자씩 순서대로 출력하기 위해 약간의 딜레이
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (talking & keyActivated)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                keyActivated = false;
                count++;
                Debug.Log("count: " + count);
                Debug.Log("listSentences.Count: " + listSentences.Count);
                text.text = "";
                theAudio.Play(enterSound);

                if (count == listSentences.Count) //문장 끝까지 다 봤을 경우
                {
                    StopAllCoroutines();
                    ExitDialogue();
                }
                else
                {
                    StopAllCoroutines();
                    StartCoroutine(StartDialogueCoroutine());
                }
            }
        }
    }
}
