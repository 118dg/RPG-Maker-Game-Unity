using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    //public static Inventory instance;

    //private DatabaseManager theDatabase;
    private OrderManager theOrder;
    private AudioManager theAudio;
    //private OkOrCancel theOOC;

    public string key_sound;
    public string enter_sound;
    public string cancel_sound;
    public string open_sound;
    public string beep_sound;

    private InventorySlot[] slots; // 인벤토리 슬롯들

    private List<Item> inventoryItemList; // 플레이어가 소지한 아이템 리스트.
    private List<Item> inventoryTabList; // 선택한 탭에 따라 다르게 보여질 아이템 리스트.

    public Text Description_Text; // 부연 설명.
    public string[] tabDescription; // 탭 부연 설명.

    public Transform tf; // slot 부모객체.

    public GameObject go; // 인벤토리 활성화 비활성화.
    public GameObject[] selectedTabImages;
    //public GameObject go_OOC; // 선택지 활성화 비활성화.

    private int selectedItem; // 선택된 아이템.
    private int selectedTab; // 선택된 탭.

    private bool activated; // 인벤토리 활성화시 true;
    private bool tabActivated; // 탭 활성화시 true
    private bool itemActivated; // 아이템 활성화시 true.
    private bool stopKeyInput; // 키입력 제한 (소비할 때 질의가 나올 텐데, 그 때 키입력 방지)
    private bool preventExec; // 중복실행 제한.

    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);


    // Use this for initialization
    void Start()
    {
        //instance = this;
        theAudio = FindObjectOfType<AudioManager>();
        theOrder = FindObjectOfType<OrderManager>();
        //theDatabase = FindObjectOfType<DatabaseManager>();
        //theOOC = FindObjectOfType<OkOrCancel>();

        inventoryItemList = new List<Item>();
        inventoryTabList = new List<Item>();
        slots = tf.GetComponentsInChildren<InventorySlot>();

        //아이템 데이터! 하드코딩!
        inventoryItemList.Add(new Item(10001, "빨간 포션", "체력을 50 채워주는 마법의 물약", Item.ItemType.Use));
        inventoryItemList.Add(new Item(10002, "파란 포션", "마나를 15 회복시켜주는 기적의 물약", Item.ItemType.Use));
        inventoryItemList.Add(new Item(10003, "농축 빨간 포션", "체력을 350 회복시켜주는 기적의 농축 물약", Item.ItemType.Use));
        inventoryItemList.Add(new Item(10004, "농축 파란 포션", "마나를 80 회복시켜주는 기적의 농축 물약", Item.ItemType.Use));
        inventoryItemList.Add(new Item(11001, "랜덤 상자", "랜덤으로 포션이 나온다. 낮은 확률로 꽝", Item.ItemType.Use));
        inventoryItemList.Add(new Item(20001, "짧은 검", "기본적인 용사의 검", Item.ItemType.Equip));
        inventoryItemList.Add(new Item(21001, "사파이어 반지", "1분에 마나 1을 회복시켜주는 마법 반지", Item.ItemType.Equip));
        inventoryItemList.Add(new Item(30001, "고대 유물의 조각 1", "반으로 쪼개진 고대 유물의 파편", Item.ItemType.Quest));
        inventoryItemList.Add(new Item(30002, "고대 유물의 조각 2", "반으로 쪼개진 고대 유물의 파편", Item.ItemType.Quest));
        inventoryItemList.Add(new Item(30003, "고대 유물", "고대 유적에 잠들어있던 고대의 유물", Item.ItemType.Quest));
    }

    public void RemoveSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].RemoveItem();
            slots[i].gameObject.SetActive(false);
        }
    } // 인벤토리 슬롯 초기화
    public void ShowTab()
    {
        RemoveSlot();
        SelectedTab(); //선택된 탭 반짝반짝 빛나게.
    } // 탭 활성화
    public void SelectedTab()
    {
        StopAllCoroutines();
        Color color = selectedTabImages[selectedTab].GetComponent<Image>().color;
        color.a = 0f;
        for (int i = 0; i < selectedTabImages.Length; i++)
        {
            selectedTabImages[i].GetComponent<Image>().color = color;
        } //일단 다 투명하게 만든 뒤.

        Description_Text.text = tabDescription[selectedTab]; //설명란에 탭 설명 적기.
        StartCoroutine(SelectedTabEffectCoroutine()); //반짝 거리게 하기
    } // 선택된 탭을 제외하고 다른 모든 탭의 컬러 알파값 0으로 조정.
    IEnumerator SelectedTabEffectCoroutine()
    {
        while (tabActivated)
        {
            Color color = selectedTabImages[0].GetComponent<Image>().color; //첫 번째 탭부터 반짝.
            while (color.a < 0.5f) //0.5f, 즉 반투명이 될 때까지.
            {
                color.a += 0.03f; //점점 색이 짙어짐.
                selectedTabImages[selectedTab].GetComponent<Image>().color = color;
                yield return waitTime;
            }
            while (color.a > 0f) //50% 정도 짙어지고 나면 다시 투명해짐. (반짝반짝).
            {
                color.a -= 0.03f;
                selectedTabImages[selectedTab].GetComponent<Image>().color = color;
                yield return waitTime;
            }

            yield return new WaitForSeconds(0.3f); //너무 빨리 반짝거리지 않게 대기.
        }
    } // 선택된 탭 반짝임 효과

    public void ShowItem()
    {
        inventoryTabList.Clear();
        RemoveSlot();
        selectedItem = 0;

        switch (selectedTab) //고른 탭에 따라서 다른 아이템 보여주기.
        {
            case 0: //소모품 탭
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Use == inventoryItemList[i].itemType)
                        inventoryTabList.Add(inventoryItemList[i]); //소모품인 아이템 다 inventoryTabList에 넣기.
                }
                break;
            case 1:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Equip == inventoryItemList[i].itemType)
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
            case 2:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Quest == inventoryItemList[i].itemType)
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
            case 3:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.ETC == inventoryItemList[i].itemType)
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
        } // 탭에 따른 아이템 분류. 그것을 인벤토리 탭 리스트에 추가

        for (int i = 0; i < inventoryTabList.Count; i++)
        {
            slots[i].gameObject.SetActive(true);
            slots[i].Additem(inventoryTabList[i]);
        } // 인벤토리 탭 리스트의 내용을, 인벤토리 슬롯에 추가

        SelectedItem();
    } // 아이템 활성화 (inventoryTabList에 조건에 맞는 아이템들만 넣어주고, 인벤토리 슬롯에 출력)
    public void SelectedItem()
    {
        StopAllCoroutines();
        if (inventoryTabList.Count > 0) //탭에 해당되는 아이템이 있을 경우.
        {
            Color color = slots[0].selected_Item.GetComponent<Image>().color;
            color.a = 0f;
            for (int i = 0; i < inventoryTabList.Count; i++)
                slots[i].selected_Item.GetComponent<Image>().color = color;
            Description_Text.text = inventoryTabList[selectedItem].itemDescription; //아이템 설명 노출.
            StartCoroutine(SelectedItemEffectCoroutine()); //아이템 슬롯 반짝거리게.
        }
        else
            Description_Text.text = "해당 타입의 아이템을 소유하고 있지 않습니다.";
    } // 선택된 아이템을 제외하고, 다른 모든 탭의 컬러 알파값을 0으로 조정.
    IEnumerator SelectedItemEffectCoroutine()
    {
        while (itemActivated)
        {
            Color color = slots[0].GetComponent<Image>().color;
            while (color.a < 0.5f)
            {
                color.a += 0.03f;
                slots[selectedItem].selected_Item.GetComponent<Image>().color = color; //slots[selectedItem].selected_Item : panel만 반짝거려야 하니까.
                yield return waitTime;
            }
            while (color.a > 0f)
            {
                color.a -= 0.03f;
                slots[selectedItem].selected_Item.GetComponent<Image>().color = color;
                yield return waitTime;
            }

            yield return new WaitForSeconds(0.3f);
        }
    } // 선택된 아이템 반짝임 효과.

    // Update is called once per frame
    void Update()
    {
        if (!stopKeyInput)
        {
            if (Input.GetKeyDown(KeyCode.I)) // I 키 누르면 인벤토리 창 뜸.
            {
                activated = !activated;

                if (activated)
                {
                    theAudio.Play(open_sound);
                    theOrder.NotMove();
                    go.SetActive(true);
                    selectedTab = 0;
                    tabActivated = true;
                    itemActivated = false;
                    ShowTab();
                }
                else
                {
                    theAudio.Play(cancel_sound);
                    StopAllCoroutines();
                    go.SetActive(false);
                    tabActivated = false;
                    itemActivated = false;
                    theOrder.Move();
                }
            } // 인벤토리 열고 닫기

            if (activated)
            {
                if (tabActivated)
                {
                    if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        if (selectedTab < selectedTabImages.Length - 1)
                            selectedTab++;
                        else
                            selectedTab = 0;
                        theAudio.Play(key_sound);
                        SelectedTab();
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        if (selectedTab > 0)
                            selectedTab--;
                        else
                            selectedTab = selectedTabImages.Length - 1;
                        theAudio.Play(key_sound);
                        SelectedTab();
                    }
                    else if (Input.GetKeyDown(KeyCode.Z))
                    {
                        theAudio.Play(enter_sound);
                        Color color = selectedTabImages[selectedTab].GetComponent<Image>().color;
                        color.a = 0.25f;
                        selectedTabImages[selectedTab].GetComponent<Image>().color = color;  //반짝거리지 않고 색 짙은 상태로 고정.
                        itemActivated = true;
                        tabActivated = false;
                        preventExec = true; //preventExec가 true일 때 키 입력이 안되도록(탭과 아이템 창에서 Z키를 둘 다 쓰니까)
                        ShowItem();
                    }//탭 선택 ( ->, <-, 선택 Z )

                } // 탭 활성화시 키입력 처리.

                else if (itemActivated)
                {
                    if (inventoryTabList.Count > 0)
                    {
                        if (Input.GetKeyDown(KeyCode.DownArrow))
                        {
                            if (selectedItem < inventoryTabList.Count - 2)
                                selectedItem += 2;
                            else
                                selectedItem %= 2; //짝수였으면 0으로, 홀수였으면 1로 돌아가게.
                            theAudio.Play(key_sound); //버튼이 눌렸으니까 sound.
                            SelectedItem(); //아이템을 새로 선택했으니까.
                        }
                        else if (Input.GetKeyDown(KeyCode.UpArrow))
                        {
                            if (selectedItem > 1)
                                selectedItem -= 2;
                            else
                                selectedItem = inventoryTabList.Count - 1 - selectedItem; //대충 이해. part 24 - 53:52
                            theAudio.Play(key_sound);
                            SelectedItem();
                        }
                        else if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            if (selectedItem < inventoryTabList.Count - 1)
                                selectedItem++;
                            else
                                selectedItem = 0;
                            theAudio.Play(key_sound);
                            SelectedItem();
                        }
                        else if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            if (selectedItem > 0)
                                selectedItem--;
                            else
                                selectedItem = inventoryTabList.Count - 1;
                            theAudio.Play(key_sound);
                            SelectedItem();
                        }
                        else if (Input.GetKeyDown(KeyCode.Z) && !preventExec) //Z키 입력 && Z키 중복입력 방지.
                        {
                            if (selectedTab == 0) // 소모품
                            {
                                theAudio.Play(enter_sound);
                                stopKeyInput = true;
                                //StartCoroutine(OOCCoroutine("사용", "취소"));
                            }
                            else if (selectedTab == 1)
                            {
                                //StartCoroutine(OOCCoroutine("장착", "취소"));
                            }
                            else // 비프음 출력
                            {
                                theAudio.Play(beep_sound);
                            }

                        }
                    }

                    if (Input.GetKeyDown(KeyCode.X))
                    {
                        theAudio.Play(cancel_sound);
                        StopAllCoroutines();
                        itemActivated = false;
                        tabActivated = true;
                        ShowTab();
                    }
                } // 아이템 활성화시 키입력 처리.

                if (Input.GetKeyUp(KeyCode.Z)) // 중복 실행 방지.
                    preventExec = false; //Z키 눌렀던 거 때면 다시 false해서 탭에서 쓸 수 있게.
            } // 인벤토리가 열리면 키입력처리 활성화.
        }
    }

    /*
    IEnumerator OOCCoroutine(string _up, string _down)
    {
        theAudio.Play(enter_sound);
        stopKeyInput = true;

        go_OOC.SetActive(true);
        theOOC.ShowTwoChoice(_up, _down);

        yield return new WaitUntil(() => !theOOC.activated);
        if (theOOC.GetResult())
        {
            for (int i = 0; i < inventoryItemList.Count; i++)
            {
                if (inventoryItemList[i].itemID == inventoryTabList[selectedItem].itemID)
                {
                    if (selectedTab == 0)
                    {
                        theDatabase.UseItem(inventoryItemList[i].itemID);

                        if (inventoryItemList[i].itemCount > 1)
                            inventoryItemList[i].itemCount--;
                        else
                            inventoryItemList.RemoveAt(i);

                        // theAudio.Play() // 아이템 먹는 소리 출력.

                        ShowItem();
                        break;
                    }
                    else if (selectedTab == 1)
                    {
                        theEquip.EquipItem(inventoryItemList[i]);
                        inventoryItemList.RemoveAt(i);
                        ShowItem();
                        break;
                    }
                }
            }
        }
        stopKeyInput = false;
        go_OOC.SetActive(false);
    }*/

}
