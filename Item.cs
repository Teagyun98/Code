using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DarkTonic.MasterAudio;

public enum ItemLine
{
    Bread, //
    Thread, //
    Fish,
    Book, //
    Coffe, //
    Saw, //
    Potion, //
    Vegetable, //
    Milk,
    Hospital,
    Smith, //
    Jewel, //
    Gallery
}

public enum ItemType
{
    Item,
    Spawner
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

    private void Awake()
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        clickCount = 0;
    }

    // 마우스로 눌렀을 때 실행되는 함수
    private void OnMouseDown()
    {
        if (inActive)
            return;

        if (MergeGameManager.instance.uiOpen && nowTile != null)
            return;

        if (isReady)
        {
            if (!MergeGameManager.instance.SpawnAbleCheck())
            {
                MergeGameManager.instance.Warning();
                return;
            }

            transform.SetParent(MergeGameManager.instance.itemField);
            StartCoroutine(MergeGameManager.instance.SetReadyItem());
        }

        if (inStorage)
        {
            if (!MergeGameManager.instance.SpawnAbleCheck())
            {
                MergeGameManager.instance.Warning();
                return;
            }

            MergeGameManager.instance.TakeOutStorage(this);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            MergeGameManager.instance.NeedCheck();
        }

        gameObject.transform.SetAsLastSibling();
        
    }

    // 드래그 함수
    private void OnMouseDrag()
    {
        if (inActive)
            return;

        if (isReady && !MergeGameManager.instance.SpawnAbleCheck())
            return;
       
        if (inStorage && transform.parent.transform != MergeGameManager.instance.itemField)
            return;

        if (MergeGameManager.instance.uiOpen && nowTile != null)
            return;

        Vector2 movePos = GetScreenPointToRectangle(Input.mousePosition);

        // 현재 위치한 타일에서 위치를 조금 이동해야 따라가도록
        if (!isDrag)
        {
            if (Vector2.Distance(movePos, rect.anchoredPosition) < 50)
                return;
            else
                isDrag = true;
        }

        // drag dotween 사용하면 버그남
        rect.anchoredPosition = movePos;
        MergeGameManager.instance.SetSelect(null);
    }

    // 마우스에서 손을 땔 때 실행되는 함수
    private void OnMouseUp()
    {
        if (inActive)
        {
            MergeGameManager.instance.SetSelect(this);
            return;
        }

        // storage col에 닿아있으면 return
        if (inStorage)
        {
            if(transform.parent.transform == MergeGameManager.instance.itemField)
                MergeGameManager.instance.TakeInStorage(this);

            return;
        }

        if (MergeGameManager.instance.uiOpen && nowTile != null)
            return;

        // 선택되어 오브젝트가 자식으로 존재하고, 스폰 타입이고, 드래그하지 않아았을 때
        if (!isDrag && item.itemType == ItemType.Spawner && !isReady)
        {
            if (EarningManager.Instance.energy > 0)
                SpawnItem();    // 아이템 스폰
            else
                MergeGameManager.instance.Warning(true);
        }

        if (isReady && MergeGameManager.instance.SpawnAbleCheck())
        {
            isReady = false;
            UserDataManager.instance.GetUserData().getMergeItem.Remove(MergeGameManager.instance.ChangeItemDate(item));
        }

        if (isReady)
            return;

        if (!isDrag && item.itemType != ItemType.Spawner)
        {
            if(clickCount >= 0)
            {
                clickCount++;
                StartCoroutine(ResetClickCount());
            }

            if(clickCount == 2 && item.Level < 5)
            {
                AutoMerge();
                clickCount = 0;
            }
        }

        isDrag = false;

        // 기존에 있던 타일의 item을 빼고
        if (nowTile != null)
        {
            nowTile.item = null;

            UserDataManager.instance.GetUserData().fieldItem.Remove(nowTile.index);
            //UserDataManager.instance.UpdateUserData_fieldItem();
        }

        Tile nearTile = MergeGameManager.instance.GetNearTile(GetScreenPointToRectangle(Input.mousePosition));

        // 이동한 타일에 아이템이 없는 경우
        if (nearTile.item == null)
        {
            // 새로 이동한 타일에 item 저장
            nowTile = nearTile;
            nowTile.item = this;

            if (UserDataManager.instance.GetUserData().fieldItem.ContainsKey(nowTile.index))
                UserDataManager.instance.GetUserData().fieldItem[nowTile.index] = MergeGameManager.instance.ChangeItemDate(item, inActive);
            else
                UserDataManager.instance.GetUserData().fieldItem.Add(nowTile.index, MergeGameManager.instance.ChangeItemDate(item, inActive));
            //UserDataManager.instance.UpdateUserData_fieldItem();
        }
        // 이동한 타일에 아이템이 있는 경우
        else if (nearTile.item != null && nearTile != nowTile)
        {
            if (item == nearTile.item.item && MergeGameManager.instance.UpgradeItem(item) != null)
            {
                if (nearTile.item.transform.childCount != 0)
                    MergeGameManager.instance.SetMergeParticle(null);

                if(nearTile.item.inActive)
                {
                    nearTile.item.inActive = false;
                    nearTile.item.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                }

                nearTile.item.gameObject.SetActive(false);
                nearTile.item.nowTile = null;
                UserDataManager.instance.GetUserData().fieldItem.Remove(nearTile.index);

                // 스포너 마지 후 다음에 생산되는 아이템이 일반 아이템이면 스포너 파티클을 종료해야함
                for (int i = 0; i < nearTile.item.transform.childCount; i++)
                {
                    nearTile.item.transform.GetChild(i).gameObject.SetActive(false);
                }

                nearTile.item = null;

                Merge();

                nowTile = nearTile;
                nowTile.item = this;

                // merge된 아이템으로 저장됨
                UserDataManager.instance.GetUserData().fieldItem[nowTile.index] = MergeGameManager.instance.ChangeItemDate(item, inActive);
                //UserDataManager.instance.UpdateUserData_fieldItem();
            }
            else
            {
                // 이동한 타일의 아이템이 비활성화 상태이고 다른 아이템이면 밀려남
                if(nearTile.item.inActive)
                {
                    PushItem();
                    return;
                }

                nearTile.item.PushItem();

                nowTile = nearTile;
                nowTile.item = this;

                UserDataManager.instance.GetUserData().fieldItem[nowTile.index] = MergeGameManager.instance.ChangeItemDate(item, inActive);
                //UserDataManager.instance.UpdateUserData_fieldItem();
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

        if (num > 0.7f) num = 0.7f;

        return num;
    }

    // 아이템 이동 함수
    private IEnumerator MoveItem(Vector2 movePos)
    {
        rect.DOAnchorPos(movePos, MoveTime(movePos));
        yield return new WaitForSeconds(MoveTime(movePos));
        MergeGameManager.instance.SetSelect(this);
    }

    // 아이템이 밀어내질 때 함수
    public void PushItem()
    {
        Tile nearTile = MergeGameManager.instance.GetNearAndEmptyTile(nowTile.GetComponent<RectTransform>().anchoredPosition);

        if (nearTile == null)
            return;

        nowTile = nearTile;
        nowTile.item = this;

        if(UserDataManager.instance.GetUserData().fieldItem.ContainsKey(nowTile.index))
            UserDataManager.instance.GetUserData().fieldItem[nowTile.index] = MergeGameManager.instance.ChangeItemDate(item, inActive);
        else
            UserDataManager.instance.GetUserData().fieldItem.Add(nowTile.index, MergeGameManager.instance.ChangeItemDate(item, inActive));
        //UserDataManager.instance.UpdateUserData_fieldItem();

        Vector2 movePos = nowTile.GetComponent<RectTransform>().anchoredPosition;

        StartCoroutine(PushingItem(movePos));
    }

    // 아이템이 밀어내질 때 이동코루틴
    private IEnumerator PushingItem(Vector2 movePos)
    {
        BoxCollider2D col = transform.GetComponent<BoxCollider2D>();

        gameObject.transform.SetAsLastSibling();
        col.enabled = false;
        rect.DOAnchorPos(movePos, MoveTime(movePos));
        yield return new WaitForSeconds(MoveTime(movePos));
        col.enabled = true;
    }

    // 새로운 아이템 스폰 함수
    private void SpawnItem()
    {
        MergeGameManager.instance.SpawnItem(rect.anchoredPosition, item.itemLine);
        EarningManager.Instance.RemoveEnergy(item.Level);
        MergeGameManager.instance.NeedCheck();
        MissionManager.instance.UpdateMissionProgress(MissionType.MergeSpawn, 1);
    }

    public void StartSetItem(MergeItem mergeItem, Tile tile, bool inActive = false)
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();

        if (mergeItem.itemType == ItemType.Spawner)
        {
            rect.sizeDelta = new Vector2(160, 160);
            ParticleSystem particle = Instantiate(MergeGameManager.instance.makingParticle, this.transform);
            particle.Play();
        }
        else if(inActive)
        {
            this.inActive = true;
            GetComponent<Image>().color = new Color32(77, 77, 77, 255);
        }

        item = mergeItem;
        image.sprite = item.icon;
        //image.SetNativeSize();
        if (tile != null)
        {
            nowTile = tile;
            tile.item = this;
        }
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
            StartCoroutine(SpawnItem(tile.GetComponent<RectTransform>().anchoredPosition));
        }
    }

    // 아이템이 생성되었을 때 이동함코루틴
    private IEnumerator SpawnItem(Vector2 movePos)
    {
        BoxCollider2D col = transform.GetComponent<BoxCollider2D>();

        Vector2 UpPos = new Vector2(rect.anchoredPosition.x + movePos.x / 3, rect.anchoredPosition.y + Random.Range(150,300));

        gameObject.transform.SetAsLastSibling();
        col.enabled = false;

        float jumpPower = Vector2.Distance(rect.anchoredPosition, movePos);

        if (jumpPower > 200f)
            jumpPower = 200;

        float moveTime = MoveTime(movePos);

        if (moveTime < 0.3f)
            moveTime = 0.3f;

        rect.DOJumpAnchorPos(movePos, jumpPower, 1, moveTime);
        yield return new WaitForSeconds(moveTime);
        col.enabled = true;
    }

    // 이동한 Tile에 같은 아이템이 존재할 경우 합친다.
    public void Merge()
    {
        item = MergeGameManager.instance.UpgradeItem(item);

        if (item == null)
            return;

        MergeGameManager.instance.PlayMergeEndParticle(this);
        image.sprite = item.icon;

        if (item.itemType == ItemType.Spawner)
            rect.sizeDelta = new Vector2(160, 160);

        MergeGameManager.instance.NeedCheck();
        MasterAudio.PlaySound("머지");

        Vector2 size;
        size = rect.sizeDelta;
        rect.sizeDelta = new Vector2(30, 30);

        DOTween.Sequence().Append(rect.DOSizeDelta(new Vector2(200, 200), 0.2f)).Append(rect.DOSizeDelta(size, 0.1f));
         
        MissionManager.instance.UpdateMissionProgress(MissionType.MergeCombine, 1);
    }

    // 보상 획득시 아이템 회수이동
    public IEnumerator CollectItem(Vector2 targetPos)
    {
        MergeGameManager.instance.PlayMergeEndParticle(null);

        nowTile.item = null;

        UserDataManager.instance.GetUserData().fieldItem.Remove(nowTile.index);
        //UserDataManager.instance.UpdateUserData_fieldItem();

        nowTile = null;

        MergeGameManager.instance.NeedCheck(true);

        Vector2 movePos = GetScreenPointToRectangle(targetPos);

        if (transform.childCount != 0)
            MergeGameManager.instance.SetSelect(null);

        rect.DOSizeDelta(new Vector2(200, 200), 0.1f);
        yield return new WaitForSeconds(0.1f);
        rect.DOSizeDelta(new Vector2(80, 80), MoveTime(movePos));
        if(gameObject.activeInHierarchy)
        {
            StartCoroutine(PushingItem(movePos));
            yield return new WaitForSeconds(MoveTime(movePos));
        }
        rect.sizeDelta = new Vector2(160, 160);
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

        UserDataManager.instance.GetUserData().fieldItem.Remove(targetItem.nowTile.index);
        //UserDataManager.instance.UpdateUserData_fieldItem();

        targetItem.nowTile.item = null;
        targetItem.nowTile = null;

        StartCoroutine(targetItem.PushingItem(rect.anchoredPosition));
        StartCoroutine(targetItem.AutoMove(this));
    }

    public IEnumerator AutoMove(Item item)
    {
        item.GetComponent<BoxCollider2D>().enabled = false;
        rect.DOSizeDelta(new Vector2(80, 80), MoveTime(item.rect.anchoredPosition));
        yield return new WaitForSeconds(MoveTime(item.rect.anchoredPosition));
        rect.sizeDelta = new Vector2(160, 160);
        gameObject.SetActive(false);
        item.Merge();

        UserDataManager.instance.GetUserData().fieldItem[item.nowTile.index]++;
        //UserDataManager.instance.UpdateUserData_fieldItem();

        item.GetComponent<BoxCollider2D>().enabled = true;
        MasterAudio.PlaySound("머지");
    }

    public void Sell()
    {
        nowTile.item = null;

        UserDataManager.instance.GetUserData().fieldItem.Remove(nowTile.index);
        //UserDataManager.instance.UpdateUserData_fieldItem();

        nowTile = null;
        StartCoroutine(SellMove());
    }

    private IEnumerator SellMove()
    {
        BoxCollider2D col = transform.GetComponent<BoxCollider2D>();

        col.enabled = false;
        rect.DOSizeDelta(new Vector2(0, 0), 0.2f);
        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);
        rect.sizeDelta = new Vector2(160, 160);
        col.enabled = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("MergeItem") && !isDrag && !MergeGameManager.instance.uiOpen)
        {
            if (collision.transform.GetComponent<Item>().item == item && nowTile != null)
                MergeGameManager.instance.SetMergeParticle(this);
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
        BoxCollider2D col = GetComponent<BoxCollider2D>();

        if(item.itemType == ItemType.Item)
        {
            col.enabled = false;
            Vector2 pos = rect.anchoredPosition;

            if(!isDrag)
                DOTween.Sequence().Append(rect.DOAnchorPos(HintPos(movePos), 0.25f)).Append(rect.DOAnchorPos(pos, 0.25f));
        }
        else
            rect.DOShakeScale(1, 0.5f);

        yield return new WaitForSeconds(0.5f);
        col.enabled = true;
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
}
