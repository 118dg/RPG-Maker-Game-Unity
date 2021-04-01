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
        if(instance == null){
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

    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        text.text = "";
        listSentences = new List<string>();
        listSprites = new List<Sprite>();
        listDialogueWindows = new List<Sprite>();
    }

    public void showDialogue(Dialogue dialogue)
    {
        for(int i = 0; i < dialogue.sentences.Length; i++)
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
    }

    IEnumerator StartDialogueCoroutine()
    {
        if(count > 0)
        {
            if (listDialogueWindows[count] != listDialogueWindows[count - 1]) //dialogueWindow 이미지가 다르다면 -> 교체
            {
                animDialogueWindow.SetBool("Appear", false);
                animSprite.SetBool("Change", true);
                yield return new WaitForSeconds(0.1f);
                rendererDialogueWindow.GetComponent<SpriteRenderer>().sprite = listDialogueWindows[count];
                rendererSprite.GetComponent<SpriteRenderer>().sprite = listSprites[count];
                animDialogueWindow.SetBool("Appear", true);
                animSprite.SetBool("Change", false);
            }
            else
            {
                if (listSprites[count] != listSprites[count - 1]) //sprite 이미지가 다르다면 -> 교체
                {
                    animSprite.SetBool("Change", true);
                    yield return new WaitForSeconds(0.1f);
                    rendererSprite.GetComponent<SpriteRenderer>().sprite = listSprites[count];
                    animSprite.SetBool("Change", false);
                }
            }
        }

        for(int i = 0; i < listSentences[count].Length; i++)
        {
            text.text += listSentences[count][i]; //listSentences[count] 문장의 한 글자씩 출력
            yield return new WaitForSeconds(0.01f); //한 글자씩 순서대로 출력하기 위해 약간의 딜레이
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            count++;
            text.text = "";

            if(count != listSentences.Count - 1) //문장 끝까지 다 봤을 경우
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
