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

    // ���콺�� ������ �� ����Ǵ� �Լ�
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

    // �巡�� �Լ�
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

        // ���� ��ġ�� Ÿ�Ͽ��� ��ġ�� ���� �̵��ؾ� ���󰡵���
        if (!isDrag)
        {
            if (Vector2.Distance(movePos, rect.anchoredPosition) < 50)
                return;
            else
                isDrag = true;
        }

        // drag dotween ����ϸ� ���׳�
        rect.anchoredPosition = movePos;
        MergeGameManager.instance.SetSelect(null);
    }

    // ���콺���� ���� �� �� ����Ǵ� �Լ�
    private void OnMouseUp()
    {
        if (inActive)
        {
            MergeGameManager.instance.SetSelect(this);
            return;
        }

        // storage col�� ��������� return
        if (inStorage)
        {
            if(transform.parent.transform == MergeGameManager.instance.itemField)
                MergeGameManager.instance.TakeInStorage(this);

            return;
        }

        if (MergeGameManager.instance.uiOpen && nowTile != null)
            return;

        // ���õǾ� ������Ʈ�� �ڽ����� �����ϰ�, ���� Ÿ���̰�, �巡������ �ʾƾ��� ��
        if (!isDrag && item.itemType == ItemType.Spawner && !isReady)
        {
            if (EarningManager.Instance.energy > 0)
                SpawnItem();    // ������ ����
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

        // ������ �ִ� Ÿ���� item�� ����
        if (nowTile != null)
        {
            nowTile.item = null;

            UserDataManager.instance.GetUserData().fieldItem.Remove(nowTile.index);
            //UserDataManager.instance.UpdateUserData_fieldItem();
        }

        Tile nearTile = MergeGameManager.instance.GetNearTile(GetScreenPointToRectangle(Input.mousePosition));

        // �̵��� Ÿ�Ͽ� �������� ���� ���
        if (nearTile.item == null)
        {
            // ���� �̵��� Ÿ�Ͽ� item ����
            nowTile = nearTile;
            nowTile.item = this;

            if (UserDataManager.instance.GetUserData().fieldItem.ContainsKey(nowTile.index))
                UserDataManager.instance.GetUserData().fieldItem[nowTile.index] = MergeGameManager.instance.ChangeItemDate(item, inActive);
            else
                UserDataManager.instance.GetUserData().fieldItem.Add(nowTile.index, MergeGameManager.instance.ChangeItemDate(item, inActive));
            //UserDataManager.instance.UpdateUserData_fieldItem();
        }
        // �̵��� Ÿ�Ͽ� �������� �ִ� ���
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

                // ������ ���� �� ������ ����Ǵ� �������� �Ϲ� �������̸� ������ ��ƼŬ�� �����ؾ���
                for (int i = 0; i < nearTile.item.transform.childCount; i++)
                {
                    nearTile.item.transform.GetChild(i).gameObject.SetActive(false);
                }

                nearTile.item = null;

                Merge();

                nowTile = nearTile;
                nowTile.item = this;

                // merge�� ���������� �����
                UserDataManager.instance.GetUserData().fieldItem[nowTile.index] = MergeGameManager.instance.ChangeItemDate(item, inActive);
                //UserDataManager.instance.UpdateUserData_fieldItem();
            }
            else
            {
                // �̵��� Ÿ���� �������� ��Ȱ��ȭ �����̰� �ٸ� �������̸� �з���
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

    // Ÿ����ǥ�� �������� �θ� RectTransform�� ��ǥ �������� �������ִ� �Լ� 
    public Vector2 GetScreenPointToRectangle(Vector2 targetPos)
    {
        Vector2 pos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), targetPos, Camera.main, out pos);

        return pos;
    }

    // �� Ʈ�� �̵� �ð� ���ϴ� �Լ�
    public float MoveTime(Vector2 vec)
    {
        float num = Vector2.Distance(rect.anchoredPosition, vec);
        num /= 5f;
        num *= Time.deltaTime;

        if (num > 0.7f) num = 0.7f;

        return num;
    }

    // ������ �̵� �Լ�
    private IEnumerator MoveItem(Vector2 movePos)
    {
        rect.DOAnchorPos(movePos, MoveTime(movePos));
        yield return new WaitForSeconds(MoveTime(movePos));
        MergeGameManager.instance.SetSelect(this);
    }

    // �������� �о�� �� �Լ�
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

    // �������� �о�� �� �̵��ڷ�ƾ
    private IEnumerator PushingItem(Vector2 movePos)
    {
        BoxCollider2D col = transform.GetComponent<BoxCollider2D>();

        gameObject.transform.SetAsLastSibling();
        col.enabled = false;
        rect.DOAnchorPos(movePos, MoveTime(movePos));
        yield return new WaitForSeconds(MoveTime(movePos));
        col.enabled = true;
    }

    // ���ο� ������ ���� �Լ�
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

    // Spawn�� ������ �ʱ�ȭ 
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

    // �������� �����Ǿ��� �� �̵����ڷ�ƾ
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

    // �̵��� Tile�� ���� �������� ������ ��� ��ģ��.
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
        MasterAudio.PlaySound("����");

        Vector2 size;
        size = rect.sizeDelta;
        rect.sizeDelta = new Vector2(30, 30);

        DOTween.Sequence().Append(rect.DOSizeDelta(new Vector2(200, 200), 0.2f)).Append(rect.DOSizeDelta(size, 0.1f));
         
        MissionManager.instance.UpdateMissionProgress(MissionType.MergeCombine, 1);
    }

    // ���� ȹ��� ������ ȸ���̵�
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
        MasterAudio.PlaySound("����");
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

    // ��Ʈ ����� ������ �����̴� �Լ�
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

    // �� ������ ������ �Ÿ��� �־ ���� �Ÿ��� �̵��ϵ��� ��ǥ ������ִ� �Լ�
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
