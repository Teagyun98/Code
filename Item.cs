using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DarkTonic.MasterAudio;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Spine;
using Spine.Unity;

public enum ItemType
{
    Item_A,
    Item_B,
    Spawner, 
    Add, 
    Multiply,
    Scissors,
    Goods
}

public class Item : MonoBehaviour
{
    private Image image;
    public RectTransform rect;
    private int clickCount;

    public MergeItem item;
    public Tile nowTile;
    public bool isDrag;
    public bool inStorage;
    public bool inActive;
    public bool isReady;
    private bool isHint;
    public bool isRefusal;
    public Image checkImg;
    public Image web;
    public int productCount;
    public int chargeTime;
    public bool isBoxing;
    public Slider timer;
    public SkeletonGraphic energySpine;
    public bool isBubble;
    public SkeletonGraphic bubbleSpine;
    public GameObject mergeSkillGuide;
    public SkeletonGraphic mergeSkillSpine;

    private Vector3 mousePos;

    public bool outOfStorageRange;

    private DG.Tweening.Sequence sequence;
    private IEnumerator co;

    private float mergeSkillGuideTime;

    private void Awake()
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        productCount = 0;
        chargeTime = 0;
        bubbleSpine.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        for (int i = 0; i < transform.childCount; i++)
            if(!inStorage)
                transform.GetChild(i).gameObject.SetActive(false);
    }

    private void Start()
    {
        clickCount = 0;
        mergeSkillGuideTime = 0;
        co = HintMove();
        isRefusal = false;
        timer.transform.parent.gameObject.SetActive(false);
    }

    // 마우스로 눌렀을 때 실행되는 함수
    private void OnMouseDown()
    {
        MergeGameManager.instance.storageMsg.SetActive(false);

        if (inActive || outOfStorageRange)
            return;

        if (MergeGameManager.instance.uiOpen && nowTile != null)
            return;

        if (isReady)
        {
            if (!MergeGameManager.instance.SpawnAbleCheck())
            {
                MergeGameManager.instance.Warning("보드가 꽉 찼습니다.");
                return;
            }

            transform.SetParent(MergeGameManager.instance.itemField);
            StartCoroutine(MergeGameManager.instance.SetReadyItem());

            if(MergeGameManager.instance.tutorial.tutoPro == 4)
            {
                MergeGameManager.instance.tutorial.StopTuto();
                MergeGameManager.instance.tutorial.SetTutorial(5);
            }
        }

        if (inStorage)
        {
            mousePos = Input.mousePosition;
            return;
        }


        if (isBubble)
            StartCoroutine(BubbleTouch());

        if(MergeGameManager.instance.discriptItem != this)
            MasterAudio.PlaySound("아이템탭");

        gameObject.transform.SetAsLastSibling();

        transform.GetComponent<Canvas>().sortingOrder = 3;
    }

    // 드래그 함수
    private void OnMouseDrag()
    {
        if (inActive || outOfStorageRange)
            return;

        if (isReady && !MergeGameManager.instance.SpawnAbleCheck())
            return;

        if (MergeGameManager.instance.uiOpen && nowTile != null)
            return;

        mergeSkillGuideTime += Time.deltaTime;

        if(mergeSkillGuideTime>0.2f)
        {
            if (!isDrag)
            {
                if (inStorage)
                    return;

                checkImg.gameObject.SetActive(false);
                isDrag = true;
                MergeGameManager.instance.isDrage = isDrag;
                MergeGameManager.instance.normal.scrollRect.enabled = false;

                if (sequence != null)
                {
                    StopCoroutine(co);
                    sequence.Kill();
                    sequence = null;
                }
            }

            switch (item.itemType)
            {
                case ItemType.Add:
                    for (int i = 0; i < MergeGameManager.instance.itemField.childCount; i++)
                    {
                        if (MergeGameManager.instance.itemField.GetChild(i).GetComponent<Item>())
                        {
                            Item target = MergeGameManager.instance.itemField.GetChild(i).GetComponent<Item>();

                            if (!target.isBoxing && target != this)
                            {
                                if ((int)target.item.itemLine < 13 && !target.isBubble && !target.inActive && target.item.itemType == ItemType.Spawner)
                                {
                                    if (!target.mergeSkillGuide.gameObject.activeSelf)
                                    {
                                        target.mergeSkillSpine.AnimationState.SetAnimation(0, "+", true);
                                        target.mergeSkillGuide.gameObject.SetActive(true);
                                    }
                                }
                                else
                                {
                                    if (target.inActive && target.item != item)
                                        target.image.color = new Color32(130, 130, 130, 123);
                                    else if (target.item == item)
                                        target.image.color = new Color32(255, 255, 255, 255);
                                    else
                                        target.image.color = new Color32(255, 255, 255, 123);
                                }

                                if (target.item.itemType == ItemType.Spawner)
                                {
                                    target.energySpine.gameObject.SetActive(false);
                                    target.timer.transform.parent.gameObject.SetActive(false);
                                }

                                target.checkImg.gameObject.SetActive(false);
                            }
                        }
                    }
                    break;
                case ItemType.Multiply:
                    for (int i = 0; i < MergeGameManager.instance.itemField.childCount; i++)
                    {
                        if (MergeGameManager.instance.itemField.GetChild(i).GetComponent<Item>())
                        {
                            Item target = MergeGameManager.instance.itemField.GetChild(i).GetComponent<Item>();

                            if (!target.isBoxing && target != this)
                            {
                                if ((int)target.item.itemLine < 13 && !target.isBubble && !target.inActive && target.item.itemType != ItemType.Spawner 
                                    && ((item.Level == 1 && target.item.Level < 5) || (item.Level == 2 && target.item.Level < 8) || item.Level == 3))
                                {
                                    if (!target.mergeSkillGuide.gameObject.activeSelf)
                                    {
                                        target.mergeSkillSpine.AnimationState.SetAnimation(0, "x", true);
                                        target.mergeSkillGuide.gameObject.SetActive(true);
                                    }
                                }
                                else
                                {
                                    if (target.inActive && target.item != item)
                                        target.image.color = new Color32(130, 130, 130, 123);
                                    else if (target.item == item)
                                        target.image.color = new Color32(255, 255, 255, 255);
                                    else
                                        target.image.color = new Color32(255, 255, 255, 123);
                                }

                                if (target.item.itemType == ItemType.Spawner)
                                {
                                    target.energySpine.gameObject.SetActive(false);
                                    target.timer.transform.parent.gameObject.SetActive(false);
                                }

                                target.checkImg.gameObject.SetActive(false);
                            }
                        }
                    }
                    break;
                case ItemType.Scissors:
                    for (int i = 0; i < MergeGameManager.instance.itemField.childCount; i++)
                    {
                        if (MergeGameManager.instance.itemField.GetChild(i).GetComponent<Item>())
                        {
                            Item target = MergeGameManager.instance.itemField.GetChild(i).GetComponent<Item>();

                            if (!target.isBoxing && target != this)
                            {
                                if ((int)target.item.itemLine < 13 && !target.isBubble && !target.inActive && target.item.itemType != ItemType.Spawner
                                    && ((item.Level == 1 && target.item.Level < 6) || (item.Level == 2 && target.item.Level < 9) || item.Level == 3))
                                {
                                    if(!target.mergeSkillGuide.gameObject.activeSelf)
                                    {
                                        target.mergeSkillSpine.AnimationState.SetAnimation(0, "-", true);
                                        target.mergeSkillGuide.gameObject.SetActive(true);
                                    }
                                }
                                else
                                {
                                    if (target.inActive)
                                        target.image.color = new Color32(130, 130, 130, 123);
                                    else if(target.item == item)
                                        target.image.color = new Color32(255, 255, 255, 255);
                                    else
                                        target.image.color = new Color32(255, 255, 255, 123);
                                }

                                if (target.item.itemType == ItemType.Spawner)
                                {
                                    target.energySpine.gameObject.SetActive(false);
                                    target.timer.transform.parent.gameObject.SetActive(false);
                                }

                                target.checkImg.gameObject.SetActive(false);
                            }
                        }
                    }
                    break;
            }
        }

        Vector2 movePos = GetScreenPointToRectangle(Input.mousePosition);

        // 현재 위치한 타일에서 위치를 조금 이동해야 따라가도록
        if (!isDrag)
        {
            if (Vector2.Distance(movePos, rect.anchoredPosition) < 100)
                return;
            else
            {
                if (inStorage)
                    return;

                checkImg.gameObject.SetActive(false);
                isDrag = true;
                MergeGameManager.instance.isDrage = isDrag;
                MergeGameManager.instance.normal.scrollRect.enabled = false;

                if (sequence != null)
                {
                    StopCoroutine(co);
                    sequence.Kill();
                    sequence = null;
                }
            }
        }

        // drag dotween 사용하면 버그남
        rect.anchoredPosition = movePos;
        transform.GetComponent<Canvas>().sortingOrder = 4;
        MergeGameManager.instance.SetSelect(null, false);
    }

    // 마우스에서 손을 땔 때 실행되는 함수
    public void OnMouseUp()
    {
        if (isBoxing || outOfStorageRange)
            return;

        if (inActive)
        {
            MergeGameManager.instance.SetSelect(this);
            return;
        }

        mergeSkillGuideTime = 0;

        for (int i = 0; i < MergeGameManager.instance.itemField.childCount; i++)
        {
            if (MergeGameManager.instance.itemField.GetChild(i).GetComponent<Item>())
            {
                Item target = MergeGameManager.instance.itemField.GetChild(i).GetComponent<Item>();

                if (!target.isBoxing)
                {
                    target.mergeSkillGuide.gameObject.SetActive(false);
                    if (!target.inActive)
                        target.image.color = new Color32(255, 255, 255, 255);
                    else
                        target.image.color = new Color32(130, 130, 130, 255);

                    if (target.item.itemType == ItemType.Spawner)
                    {
                        if (target.productCount > 0)
                            target.energySpine.gameObject.SetActive(true);
                        else
                            target.timer.transform.parent.gameObject.SetActive(true);
                    }
                }
            }
        }

        // storage col에 닿아있으면 return
        if (inStorage)
        {
            if (transform.parent.transform == MergeGameManager.instance.itemField)
            {
                MergeGameManager.instance.TakeInStorage(this);
                MergeGameManager.instance.isDrage = isDrag;
                MergeGameManager.instance.normal.scrollRect.enabled = true;
                transform.GetComponent<Canvas>().sortingOrder = 6;
                return;
            }
            else
            {
                if (!MergeGameManager.instance.SpawnAbleCheck())
                {
                    MergeGameManager.instance.Warning("보드가 꽉 찼습니다.");
                    return;
                }

                if (Vector3.Distance(mousePos, Input.mousePosition) > 50)
                    return;

                MergeGameManager.instance.TakeOutStorage(this);
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);

                if (item.itemType == ItemType.Spawner)
                    rect.sizeDelta = new Vector2(160, 160);
                else
                    rect.sizeDelta = new Vector2(140, 140);

                rect.localScale = Vector3.one;

                GetComponent<Canvas>().overrideSorting = true;
                transform.GetComponent<Canvas>().sortingOrder = 4;

                MergeGameManager.instance.NeedCheck();
            }
        }

        if (MergeGameManager.instance.uiOpen && nowTile != null)
            return;

        Tile nearTile = MergeGameManager.instance.GetNearTile(GetScreenPointToRectangle(Input.mousePosition));

        bool refusal = false;

        if (!nowTile && !isDrag)
            nearTile = MergeGameManager.instance.GetNearAndEmptyTile(GetScreenPointToRectangle(Input.mousePosition));
        else if (isRefusal)
        {
            // 창고가 가득차 있으면 원래 위치로 이동
            nearTile = nowTile;
            clickCount = 0;
            isRefusal = false;
            refusal = true;
        }
        else if (!inStorage && Vector2.Distance(nearTile.GetComponent<RectTransform>().anchoredPosition, GetScreenPointToRectangle(Input.mousePosition)) > 500)
        {
            nearTile = nowTile;
            refusal = true;
        }

        if (item.itemType == ItemType.Spawner && !isReady && MergeGameManager.instance.SelectCheck(this))
        {
            if (!isDrag || (nearTile == nowTile && !refusal))
            {
                if (EarningManager.Instance.energy >= 1 && productCount > 0)
                {
                    SpawnItem();    // 아이템 스폰

                    if (MergeGameManager.instance.tutorial.tutoPro == 5)
                    {
                        MergeGameManager.instance.tutorial.StopTuto();
                        MergeGameManager.instance.tutorial.SetTutorial(6);
                    }
                }
                else
                {
                    if (productCount <= 0)
                        MergeGameManager.instance.Warning("생성기 에너지가 부족합니다.");
                    else
                    {
                        MergeGameManager.instance.Warning("에너지가 부족합니다.");
                        MergeGameManager.instance.OpenRefiilPop();
                    }
                }   
            }
        }

        if (isReady && MergeGameManager.instance.SpawnAbleCheck())
        {
            isReady = false;

            if(!isDrag)
                nearTile = MergeGameManager.instance.GetNearAndEmptyTile(GetScreenPointToRectangle(Input.mousePosition));

            UserDataManager.instance.GetUserData().getMergeItem.Remove(MergeDataManager.instance.ItemChangeData(item));
        }

        if (MergeGameManager.instance.tutorial.tutoPro < 6 && item.itemType == ItemType.Spawner)
            foreach (Tile tile in MergeGameManager.instance.normal.tiles)
                if (tile.index == 31)
                    nearTile = tile;

        if (isReady)
            return;

        if (nowTile == nearTile && item.itemType != ItemType.Spawner)
        {
            if(clickCount >= 0)
            {
                clickCount++;
                StartCoroutine(ResetClickCount());
            }

            if(clickCount == 2 && item.itemType == ItemType.Goods)
            {
                GetGoods();
                clickCount = 0;

                isDrag = false;
                MergeGameManager.instance.isDrage = isDrag;
                MergeGameManager.instance.normal.scrollRect.enabled = true;

                return;
            }
            else if(clickCount == 2 && MergeDataManager.instance.UpgradeItem(item))
            {
                if(!isBubble)
                    AutoMerge();
                clickCount = 0;
            }
        }

        isDrag = false;
        MergeGameManager.instance.isDrage = isDrag;
        MergeGameManager.instance.normal.scrollRect.enabled = true;

        if (!gameObject.activeSelf)
            return;

        // 기존에 있던 타일의 item을 빼고
        if (nowTile != null)
        {
            nowTile.item = null;
        }

        // 이동한 타일에 아이템이 없는 경우
        if (nearTile.item == null)
        {
            // 새로 이동한 타일에 item 저장
            nowTile = nearTile;
            nowTile.item = this;
        }
        // 이동한 타일에 아이템이 있는 경우
        else if (nearTile.item != null && nearTile != nowTile)
        {
            if (item == nearTile.item.item && MergeDataManager.instance.UpgradeItem(item) != null && !nearTile.item.isBoxing && !isBubble && !nearTile.item.isBubble)
            {
                MergeGameManager.instance.SetMergeParticle(null);

                if(nearTile.item.inActive)
                {
                    nearTile.item.inActive = false;
                    nearTile.item.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                    nearTile.item.web.gameObject.SetActive(false);
                }

                // 스포너 마지 후 다음에 생산되는 아이템이 일반 아이템이면 스포너 파티클을 종료해야함
                for(int i = 0; i< nearTile.item.transform.childCount; i++)
                    nearTile.item.transform.GetChild(i).gameObject.SetActive(false);

                nearTile.item.gameObject.SetActive(false);
                nearTile.item.nowTile = null;
                nearTile.item = null;

                Merge(nearTile);

                nowTile = nearTile;
                nowTile.item = this;

                // 3레벨 이상의 아이템을 합했을 때 10%확률로 특별 아이템 생성
                if (item.Level > 3 && (int)item.itemLine < 13 && MergeGameManager.instance.SpawnAbleCheck() && item.itemType != ItemType.Spawner)
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        Animalspecialization line = (Animalspecialization)Random.Range(13, 18);
                        int level = 0;

                        switch (Random.Range(0, 6))
                        {
                            case 0:
                                level = 2;
                                break;
                            default:
                                level = 1;
                                break;
                        }

                        MergeGameManager.instance.SpawnSpecial(rect.anchoredPosition, line, level);
                    }
                    else
                        MergeGameManager.instance.SpawnBubble(rect.anchoredPosition, item);
                }
            }
            else
            {
                if(item.itemLine == Animalspecialization.Merge && item.itemType != ItemType.Spawner && nearTile.item && !nearTile.item.inActive && !nearTile.item.isBubble && !isBubble)
                {
                    if((int)nearTile.item.item.itemLine < 13)
                    {
                        if ( nearTile.item.item.itemType == ItemType.Spawner && item.itemType == ItemType.Add)
                        {
                            if (nearTile.item.AddMove(this))
                                return;
                        }
                        else if (nearTile.item.item.itemType != ItemType.Spawner && item.itemType == ItemType.Multiply)
                        {
                            if (nearTile.item.MultiplyMove(this))
                                return;
                        }
                        else if (nearTile.item.item.itemType != ItemType.Spawner && item.itemType == ItemType.Scissors)
                        {
                            if (nearTile.item.ScissorsMove(this))
                                return;
                        }
                    }
                }

                if (item == nearTile.item.item && MergeDataManager.instance.UpgradeItem(item) == null && !nearTile.item.isBoxing && !isBubble && !nearTile.item.isBubble)
                {
                    MergeGameManager.instance.Warning("최고레벨");
                }

                // 이동한 타일의 아이템이 비활성화 상태이고 다른 아이템이면 밀려남
                if (nearTile.item.inActive)
                {
                    PushItem();
                    MergeGameManager.instance.NeedCheck();
                    return;
                }

                nearTile.item.PushItem();

                nowTile = nearTile;
                nowTile.item = this;
            }
        }

        Vector2 movePos = nowTile.GetComponent<RectTransform>().anchoredPosition;

        if(rect.anchoredPosition == movePos)
            MergeGameManager.instance.SetSelect(this);
        else
            StartCoroutine(MoveItem(movePos));

        MergeGameManager.instance.NeedCheck();
    }

    // 타겟좌표를 아이템의 부모 RectTransform의 좌표 기준으로 변경해주는 함수 
    public Vector2 GetScreenPointToRectangle(Vector2 targetPos)
    {
        Vector2 pos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), targetPos, Camera.main, out pos);

        return pos;
    }

    // 닷 트윈 이동 시간 구하는 함수
    public float MoveTime(Vector2 vec)
    {
        float num = Vector2.Distance(rect.anchoredPosition, vec);
        num /= 5f;
        num *= Time.deltaTime;

        if (num > 0.3f) num = 0.3f;

        return num;
    }

    // 아이템 이동 함수
    private IEnumerator MoveItem(Vector2 movePos)
    {
        rect.DOAnchorPos(movePos, MoveTime(movePos));
        yield return new WaitForSeconds(MoveTime(movePos));
        MergeGameManager.instance.SetSelect(this);
        transform.GetComponent<Canvas>().sortingOrder = 3;
    }

    // 아이템이 밀어내질 때 함수
    public void PushItem()
    {
        Tile nearTile = MergeGameManager.instance.GetNearAndEmptyTile(rect.anchoredPosition);

        if (nearTile == null)
            return;

        nowTile = nearTile;
        nowTile.item = this;

        Vector2 movePos = nowTile.GetComponent<RectTransform>().anchoredPosition;

        StartCoroutine(PushingItem(movePos));
    }

    // 아이템이 밀어내질 때 이동코루틴
    private IEnumerator PushingItem(Vector2 movePos)
    {
        CircleCollider2D col = transform.GetComponent<CircleCollider2D>();
        GetComponent<Canvas>().sortingOrder = 4;
        gameObject.transform.SetAsLastSibling();
        col.enabled = false;
        rect.DOAnchorPos(movePos, MoveTime(movePos));
        yield return new WaitForSeconds(MoveTime(movePos));
        GetComponent<Canvas>().sortingOrder = 3;
        col.enabled = true;
    }

    // 새로운 아이템 스폰 함수
    private void SpawnItem()
    {
        if ((int)item.itemLine < 13)
        {
            if (MergeGameManager.instance.SpawnItem(rect.anchoredPosition, item.itemLine, item.Level))
            {
                EarningManager.Instance.RemoveEnergy(1);
                productCount--;
               MissionManager.instance.UpdateMissionProgress(MissionType.MergeSpawn, 1);
            }
        }
        else
        {
            if (MergeGameManager.instance.SpawnSpecial(rect.anchoredPosition, item.itemLine, item.Level))
            {
                EarningManager.Instance.RemoveEnergy(1);
               MissionManager.instance.UpdateMissionProgress(MissionType.MergeSpawn, 1);
                productCount--;

                if (productCount == 0)
                    Sell();
            }
        }
    }

    public void StartSetItem(MergeItem mergeItem, Tile tile, bool inActive = false)
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();

        if (mergeItem.itemType == ItemType.Spawner)
        {
            rect.sizeDelta = new Vector2(160, 160);

            if (inActive)
            {
                this.inActive = true;
                GetComponent<Image>().color = new Color32(130, 130, 130, 255);
                transform.GetComponent<Canvas>().sortingOrder = 1;
                web.gameObject.SetActive(true);
            }
            else
            {
                web.gameObject.SetActive(false);
                transform.GetComponent<Canvas>().sortingOrder = 3;
                GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                this.inActive = false;
                MergeGameManager.instance.SetEnergySpine(this);
                ParticleSystem particle = Instantiate(MergeGameManager.instance.makingParticle, this.transform);
                particle.Play();
            }
        }
        else if (inActive)
        {
            this.inActive = true;
            GetComponent<Image>().color = new Color32(130, 130, 130, 255);
            transform.GetComponent<Canvas>().sortingOrder = 1;
            web.gameObject.SetActive(true);
        }
        else if(!isBubble)
        {
            rect.sizeDelta = new Vector2(140, 140);
            web.gameObject.SetActive(false);
            transform.GetComponent<Canvas>().sortingOrder = 3;
            GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            this.inActive = false;
        }


        item = mergeItem;
        image.sprite = item.icon;
        //image.SetNativeSize();
        if (tile != null)
        {
            nowTile = tile;
            tile.item = this;
        }

        if (isBubble)
            StartCoroutine(BubbleLifeTime());
    }

    // Spawn된 아이템 초기화 
    public void SetItem(MergeItem mergeItem, Tile tile)
    {
        transform.gameObject.SetActive(true);
        item = mergeItem;
        image.sprite = item.icon;
        //image.SetNativeSize();
        if (tile != null)
        {
            nowTile = tile;
            tile.item = this;

            if (!UserDataManager.instance.GetUserData().dicItems.Contains(MergeDataManager.instance.ItemChangeData(item)))
                UserDataManager.instance.GetUserData().dicItems.Add(MergeDataManager.instance.ItemChangeData(item));

            StartCoroutine(SpawnItem(tile.GetComponent<RectTransform>().anchoredPosition));
        }

        if(isBubble)
            StartCoroutine(BubbleLifeTime());

        if(inActive)
        {
            GetComponent<Image>().color = new Color32(130, 130, 130, 255);
            transform.GetComponent<Canvas>().sortingOrder = 1;
            web.gameObject.SetActive(true);
        }
        else
        {
            web.gameObject.SetActive(false);
            transform.GetComponent<Canvas>().sortingOrder = 3;
            GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }

    }

    // 아이템이 생성되었을 때 이동함코루틴
    private IEnumerator SpawnItem(Vector2 movePos)
    {
        CircleCollider2D col = transform.GetComponent<CircleCollider2D>();

        Vector2 UpPos = new Vector2(rect.anchoredPosition.x + movePos.x / 3, rect.anchoredPosition.y + Random.Range(150,300));

        if(!isBubble)
            rect.sizeDelta = new Vector2(140, 140);

        transform.GetComponent<Canvas>().sortingOrder = 8;
        gameObject.transform.SetAsLastSibling();
        col.enabled = false;

        float jumpPower = Vector2.Distance(rect.anchoredPosition, movePos);

        if (jumpPower > 200f)
            jumpPower = 200;

        float moveTime = MoveTime(movePos);

        if (moveTime < 0.5f)
            moveTime = 0.5f;

        rect.DOJumpAnchorPos(movePos, jumpPower, 1, moveTime);
        yield return new WaitForSeconds(moveTime);
        col.enabled = true;

        transform.GetComponent<Canvas>().sortingOrder = 3;
        MergeGameManager.instance.NeedCheck();

        if((item.itemType == ItemType.Item_A || item.itemType == ItemType.Item_B ) && item.Level>1 && !isBubble)
            MergeGameManager.instance.specialEffect.SetNice(nowTile.transform.position);
    }

    // 이동한 Tile에 같은 아이템이 존재할 경우 합친다.
    public void Merge(Tile tile = null)
    {
        item = MergeDataManager.instance.UpgradeItem(item);

        // 도감 등록
        if (!UserDataManager.instance.GetUserData().dicItems.Contains(MergeDataManager.instance.ItemChangeData(item)))
        {
            if(item.itemType == ItemType.Spawner)
            {
                MergeGameManager.instance.OpenNewSpawnerUpPop(item);
            }

            UserDataManager.instance.GetUserData().dicItems.Add(MergeDataManager.instance.ItemChangeData(item));
        }

        if (item == null)
            return;

        MergeGameManager.instance.PlayMergeEndParticle(this);
        image.sprite = item.icon;

        if (item.itemType == ItemType.Spawner)
        {
            MasterAudio.PlaySound("생성기");

            rect.sizeDelta = new Vector2(160, 160);

            if ((int)item.itemLine < 13)
            {
                productCount = 15 + item.Level * 5;
            }
            else if ((int)item.itemLine > 12)
            {
                productCount = item.Level * 5 + 5;

                if (item.itemLine == Animalspecialization.MergePay)
                    productCount += 5;
            }
        }
        else
            rect.sizeDelta = new Vector2(140, 140);


        MergeGameManager.instance.NeedCheck();

        // 연속해서 머지했을때 음 올라가게 하기 
        // MergeSoundManager.instance.MergeSound();
        //MasterAudio.PlaySound("머지");

        if (item.itemType != ItemType.Spawner)
        {
            // // 레벨별로 음 다르게 하기
            // switch (item.Level)
            // {
            //     case 2:
            //         MasterAudio.PlaySound("tone1");
            //         break;
            //     case 3:
            //         MasterAudio.PlaySound("tone2");
            //         break;
            //     case 4:
            //         MasterAudio.PlaySound("tone3");
            //         break;
            //     case 5:
            //         MasterAudio.PlaySound("tone4");
            //         break;
            //     case 6:
            //         MasterAudio.PlaySound("tone5");
            //         break;
            //     case 7:
            //         MasterAudio.PlaySound("tone6");
            //         break;
            //     case 8:
            //         MasterAudio.PlaySound("tone7");
            //         break;
            //     case 9:
            //         MasterAudio.PlaySound("tone8");
            //         break;
            // }
             MasterAudio.PlaySound("아이템생성");
        }

        UserDataManager.instance.GetUserData().mergeCount++;

        Vector2 size;
        size = rect.sizeDelta;
        rect.sizeDelta = new Vector2(30, 30);

        DOTween.Sequence().Append(rect.DOSizeDelta(new Vector2(250, 250), 0.2f)).Append(rect.DOSizeDelta(size, 0.1f));

        if(tile)
            UnBoxingOrder(tile);

        MissionManager.instance.UpdateMissionProgress(MissionType.MergeCombine, 1);

        if (MergeGameManager.instance.tutorial.tutoPro == 6)
        {
            MergeGameManager.instance.tutorial.StopTuto();
            MergeGameManager.instance.tutorial.SetTutorial(7);
        }
        else if (MergeGameManager.instance.tutorial.tutoPro == 7)
        {
            MergeGameManager.instance.tutorial.StopTuto();
            MergeGameManager.instance.tutorial.SetTutorial(8);
        }
    }

    private void UnBoxingOrder(Tile gettile)
    {
        int up, down, left, right;

        // getMerge에서 바로 꺼내서 합쳤을 때 버그 생김
        int nowTileIndex = gettile.index;

        up = nowTileIndex - 7;
        down = nowTileIndex + 7;
        left = nowTileIndex - 1;
        right = nowTileIndex + 1;

        if (nowTileIndex % 7 == 0)
            left = -1;
        
        if(right%7 == 0)
            right = -1;

        List<int> list = new List<int>
        {
            up,
            down,
            left,
            right
        };

        foreach (int i in list)
        {
            if(i > -1)
                foreach (Tile tile in MergeGameManager.instance.normal.tiles)
                {
                    if (!tile.seal && tile.index == i && tile.item && tile.item.isBoxing)
                        tile.item.UnBoxing();
                }
        }
    }

    // 보상 획득시 아이템 회수이동
    public IEnumerator CollectItem(Vector2 targetPos)
    {
        MergeGameManager.instance.PlayMergeEndParticle(null);

        nowTile.item = null;
        nowTile = null;
        transform.GetComponent<Canvas>().sortingOrder = 4;

        MergeGameManager.instance.NeedCheck(true);

        Vector2 movePos = GetScreenPointToRectangle(targetPos);

        if (transform.childCount != 0)
            MergeGameManager.instance.SetSelect(null);

        rect.DOSizeDelta(new Vector2(250, 250), 0.1f);
        yield return new WaitForSeconds(0.1f);
        rect.DOSizeDelta(new Vector2(160, 160), MoveTime(movePos));
        if(gameObject.activeInHierarchy)
        {
            StartCoroutine(PushingItem(movePos));
            yield return new WaitForSeconds(MoveTime(movePos));
        }
        rect.sizeDelta = new Vector2(160, 160);
        transform.GetComponent<Canvas>().sortingOrder = 3;
        gameObject.SetActive(false);
    }

    private IEnumerator ResetClickCount()
    {
        yield return new WaitForSeconds(0.5f);
        clickCount = 0;
    }

    private void AutoMerge()
    {
        Item targetItem = MergeGameManager.instance.SearchFieldItem(this);

        if (targetItem == null)
            return;

        targetItem.nowTile.item = null;
        targetItem.nowTile = null;

        StartCoroutine(targetItem.PushingItem(rect.anchoredPosition));
        StartCoroutine(targetItem.AutoMove(this));
    }

    public IEnumerator AutoMove(Item item)
    {
        item.GetComponent<CircleCollider2D>().enabled = false;
        yield return new WaitForSeconds(MoveTime(item.rect.anchoredPosition));
        rect.sizeDelta = new Vector2(160, 160);
        gameObject.SetActive(false);
        item.Merge();

        item.GetComponent<CircleCollider2D>().enabled = true;
       
    }

    public void Sell()
    {
        StartCoroutine(SellMove());
    }

    private IEnumerator SellMove()
    {
        CircleCollider2D col = transform.GetComponent<CircleCollider2D>();

        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);

        col.enabled = false;
        rect.DOSizeDelta(new Vector2(0, 0), 0.2f);

        yield return new WaitForSeconds(0.2f);

        nowTile.item = null;
        nowTile = null;
        gameObject.SetActive(false);
        rect.sizeDelta = new Vector2(160, 160);

        MergeGameManager.instance.SetSelect(null);
        col.enabled = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("MergeItem") && !isDrag && !MergeGameManager.instance.uiOpen && !isHint && !isBubble)
        {
            if (collision.transform.GetComponent<Item>().item == item && nowTile != null && !collision.transform.GetComponent<Item>().isHint && !collision.transform.GetComponent<Item>().isBubble)
            {
                MergeGameManager.instance.SetMergeParticle(this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("MergeItem") && !isDrag && !MergeGameManager.instance.uiOpen)
        {
            if (collision.transform.GetComponent<Item>().item == item && nowTile != null)
                MergeGameManager.instance.SetMergeParticle(null);
        }
    }

    // 힌트 실행시 아이템 움직이는 함수
    public IEnumerator HintMove(Vector2 movePos = new Vector2())
    {
       // CircleCollider2D col = GetComponent<CircleCollider2D>();

        if(item.itemType != ItemType.Spawner)
        {
            isHint = true;
            Vector2 pos = rect.anchoredPosition;

            if(!isDrag)
                sequence = DOTween.Sequence().Append(rect.DOAnchorPos(HintPos(movePos), 0.25f)).Append(rect.DOAnchorPos(pos, 0.25f));
        }
        else
        {
            isHint = true;
             sequence = DOTween.Sequence().Append(rect.DOScaleY(0.7f, 0.2f)).Append(rect.DOScaleY(1.3f, 0.1f)).Append(rect.DOScaleY(0.8f, 0.1f)).Append(rect.DOScaleY(1, 0.1f));
        }

        yield return new WaitForSeconds(0.5f);

        sequence = null;
        if (nowTile && rect.anchoredPosition != nowTile.GetComponent<RectTransform>().anchoredPosition && !isDrag)
            rect.DOAnchorPos(nowTile.GetComponent<RectTransform>().anchoredPosition, 0.1f);

        isHint = false;
    }

    // 두 아이템 사이의 거리가 멀어도 일정 거리만 이동하도록 좌표 계산해주는 함수
    private Vector2 HintPos(Vector2 movePos)
    {
        Vector2 pos = movePos;

        while(true)
        {
            if (Vector2.Distance(rect.anchoredPosition, pos) < 40)
                return pos;

            pos = (rect.anchoredPosition + pos) / 2;
        }
    }

    public bool AddMove(Item item)
    {
        bool result = false;

        int addCount = 0;

        switch(item.item.Level)
        {
            case 1:
                addCount += 10;
                break;
            case 2:
                addCount += 20;
                break;
            case 3:
                addCount += 40;
                break;
        }

        productCount += addCount;

        if(MergeGameManager.instance.discriptItem == this)
            MergeGameManager.instance.Discription(this);

        // 실행되고 있는 파티클 종료
        for (int i = 0; i < item.transform.childCount; i++)
        {
            item.transform.GetChild(i).gameObject.SetActive(false);
        }

        item.gameObject.SetActive(false);

        MergeGameManager.instance.chargeEffect.SetChargeEffect(transform);
        result = true;

        return result;
    }

    public bool MultiplyMove(Item item)
    {
        bool result = false;
        int limitLevel = 99;

        switch (item.item.Level)
        {
            case 1:
                limitLevel = 4;
                break;
            case 2:
                limitLevel = 7;
                break;
        }

        if(this.item.Level<=limitLevel)
        {
            if (MergeGameManager.instance.SpawnItem(rect.anchoredPosition, this.item.itemLine, this.item.Level, this.item))
            {
                for (int i = 0; i < item.transform.childCount; i++)
                {
                    item.transform.GetChild(i).gameObject.SetActive(false);
                }
                item.gameObject.SetActive(false);
                result = true;
            }
        }
        return result;
    }

    public bool ScissorsMove(Item item)
    {
        bool result = false;

        int limitLevel = 99;

        switch (item.item.Level)
        {
            case 1:
                limitLevel = 5;
                break;
            case 2:
                limitLevel = 8;
                break;
        }

        if(this.item.Level <= limitLevel && this.item.Level > 1)
        {
            MergeItem scissorsItem = MergeDataManager.instance.SearchMergeItem(this.item.itemLine, this.item.itemType, this.item.Level - 1);

            if (scissorsItem && MergeGameManager.instance.SpawnItem(rect.anchoredPosition, scissorsItem.itemLine, scissorsItem.Level, scissorsItem))
            {
                nowTile.item = null;
                nowTile = null;

                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
                gameObject.SetActive(false);

                MergeGameManager.instance.SpawnItem(rect.anchoredPosition, scissorsItem.itemLine, scissorsItem.Level, scissorsItem);

                for (int i = 0; i < item.transform.childCount; i++)
                {
                    item.transform.GetChild(i).gameObject.SetActive(false);
                }
                item.gameObject.SetActive(false);
                result = true;
            }
        }

        return result;
    }

    public void Boxing()
    {
        transform.GetComponent<Image>().sprite = MergeGameManager.instance.boxingImgs[Random.Range(0, 3)];
        transform.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        isBoxing = true;
        web.gameObject.SetActive(false);
    }

    public void UnBoxing()
    {
        UserDataManager.instance.GetUserData().boxingTiles.Remove(nowTile.index);
        StartCoroutine(UnBoxingMove());
    }

    private IEnumerator UnBoxingMove()
    {
        MergeGameManager.instance.boxCrash.SetBoxCrash(transform);
        yield return new WaitForSeconds(0.1f);
        rect.DOScale(0.8f, 0.3f);
        yield return new WaitForSeconds(0.3f);
        rect.localScale = Vector3.one;
        transform.GetComponent<Image>().sprite = item.icon;
        transform.GetComponent<Image>().color = new Color32(130, 130, 130, 255);
        isBoxing = false;
        web.gameObject.SetActive(true);

        if (!UserDataManager.instance.GetUserData().dicItems.Contains(MergeDataManager.instance.ItemChangeData(item)))
            UserDataManager.instance.GetUserData().dicItems.Add(MergeDataManager.instance.ItemChangeData(item));
    }

    private void GetGoods()
    {
        switch (item.itemLine)
        {
            case Animalspecialization.Medal:
                switch (item.Level)
                {
                    case 1:
                        EarningManager.Instance.AddMedal(20);
                        break;
                    case 2:
                        EarningManager.Instance.AddMedal(40);
                        break;
                    case 3:
                        EarningManager.Instance.AddMedal(90);
                        break;
                    case 4:
                        EarningManager.Instance.AddMedal(200);
                        break;
                    case 5:
                        EarningManager.Instance.AddMedal(450);
                        break;
                    case 6:
                        EarningManager.Instance.AddMedal(1000);
                        break;
                }
                EarningManager.Instance.area.SetEarningParticle(transform, 1);
                break;
            case Animalspecialization.Gem:
                switch (item.Level)
                {
                    case 1:
                        EarningManager.Instance.AddGem(1);
                        break;
                    case 2:
                        EarningManager.Instance.AddGem(2);
                        break;
                    case 3:
                        EarningManager.Instance.AddGem(4);
                        break;
                    case 4:
                        EarningManager.Instance.AddGem(9);
                        break;
                    case 5:
                        EarningManager.Instance.AddGem(20);
                        break;
                    case 6:
                        EarningManager.Instance.AddGem(50);
                        break;
                }
                EarningManager.Instance.area.SetEarningParticle(transform, 2);
                break;
            case Animalspecialization.Energy:
                switch (item.Level)
                {
                    case 1:
                        EarningManager.Instance.AddEnergy(1);
                        break;
                    case 2:
                        EarningManager.Instance.AddEnergy(2);
                        break;
                    case 3:
                        EarningManager.Instance.AddEnergy(4);
                        break;
                    case 4:
                        EarningManager.Instance.AddEnergy(9);
                        break;
                    case 5:
                        EarningManager.Instance.AddEnergy(20);
                        break;
                    case 6:
                        EarningManager.Instance.AddEnergy(50);
                        break;
                }
                EarningManager.Instance.area.SetEarningParticle(transform, 4);
                break;
        }

        Sell();
    }

    public IEnumerator CountEnergyTime()
    {
        while (true)
        {
            if (item.itemType != ItemType.Spawner || !gameObject.activeSelf)
                break;

            if (chargeTime > 0 && MergeGameManager.instance.ProductLimit(this) > productCount)
            {
                chargeTime--;

                if (MergeGameManager.instance.discriptItem == this)
                    MergeGameManager.instance.Discription(this);
            }

            // 생산 횟소 모두 소모시 다음 회복 까지 남은 시간 표시
            if (productCount == 0 && (!mergeSkillGuide.gameObject.activeSelf && image.color.a == 255))
            {
                timer.transform.parent.gameObject.SetActive(true);
                energySpine.gameObject.SetActive(false);
                timer.value = (float)chargeTime / 120;
            }
            else if(!mergeSkillGuide.gameObject.activeSelf && image.color.a == 255)
            {
                timer.transform.parent.gameObject.SetActive(false);
                energySpine.gameObject.SetActive(true);
            }

            if (chargeTime == 0)
            {
                ChargeSpawnerEnergy();
            }

            yield return new WaitForSeconds(1);
        }
    }

    private void ChargeSpawnerEnergy()
    {
        if (productCount < MergeGameManager.instance.ProductLimit(this))
        {
            productCount++;
            chargeTime = 120;

            if (MergeGameManager.instance.discriptItem == this)
                MergeGameManager.instance.Discription(this);
        }
    }

    public void SetBubble(int time = -1)
    {
        isBubble = true;
        bubbleSpine.gameObject.SetActive(true);

        if (time != -1)
            productCount = time;
        else
            productCount = 60;

        rect.sizeDelta = new Vector2(120, 120);
        bubbleSpine.AnimationState.SetAnimation(0, "Idle", true);
    }

    public IEnumerator BubbleTouch()
    {
        bubbleSpine.AnimationState.SetAnimation(0, "Touch", false);

        yield return new WaitForSeconds(2.6f);

        if(isBubble)
            bubbleSpine.AnimationState.SetAnimation(0, "Idle", true);
    }

    public IEnumerator PopBubble()
    {
        if (isBubble)
        {
            GetComponent<CircleCollider2D>().enabled = false;

            if (isDrag)
                OnMouseUp();

            bubbleSpine.AnimationState.SetAnimation(0, "pop!", false);
            rect.DOSizeDelta(new Vector2(140, 140), 1.8f);
            yield return new WaitForSeconds(1.3f);

            bubbleSpine.gameObject.SetActive(false);
            isBubble = false;
            GetComponent<CircleCollider2D>().enabled = true;

            if (MergeGameManager.instance.discriptItem == this)
                MergeGameManager.instance.Discription(this);

            MergeGameManager.instance.NeedCheck();
        }
    }

    private IEnumerator BubbleLifeTime()
    {
        while (true)
        {
            if (!isBubble)
                break;

            productCount--;

            if (productCount < 0)
            {
                GetComponent<CircleCollider2D>().enabled = false;

                if (isDrag)
                    OnMouseUp();

                bubbleSpine.AnimationState.SetAnimation(0, "pop!", false);

                yield return new WaitForSeconds(1f);

                rect.sizeDelta = new Vector2(140, 140);
                isBubble = false;
                GetComponent<CircleCollider2D>().enabled = true;
                bubbleSpine.gameObject.SetActive(false);

                SetItem(MergeDataManager.instance.SearchMergeItem(Animalspecialization.Medal, ItemType.Goods, 3), nowTile);
                break;
            }

            if (MergeGameManager.instance.discriptItem == this)
                MergeGameManager.instance.Discription(this);

            yield return new WaitForSeconds(1);
        }

        if (MergeGameManager.instance.discriptItem == this)
            MergeGameManager.instance.Discription(this);
    }
}
