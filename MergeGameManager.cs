using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using DarkTonic.MasterAudio;

public class MergeGameManager : MonoBehaviour
{
    public static MergeGameManager instance;

    [SerializeField] private List<MergeItem> items;
    [SerializeField] private List<MergeItem> spawners;
    [SerializeField] private GameObject select;
    public Transform itemField;
    [SerializeField] private Item spawnItem;
    [SerializeField] private GameObject storage;
    [SerializeField] private List<Sprite> rewardIcons;
    [SerializeField] private MergeStoreOpenPop storeOpenPop;
    [SerializeField] private Transform warningArea;
    [SerializeField] private Warning warning;
    [SerializeField] private ParticleSystem mergeParticle;
    public ParticleSystem makingParticle;
    [SerializeField] private ParticleSystem mergeEndParticle;
    [SerializeField] private Image returnImage;
    [SerializeField] private Guide guide;

    [Header("MergeType")]
    [SerializeField] private MergeGameType normal;
    [SerializeField] private MergeGameType storeOpen;

    private MergeGameType type;
    private List<ItemLine> lines;
    private List<MergeItem> readyItems;

    public bool uiOpen;
    public int hintTime;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        if (MergeDataManager.instance.storeOpen)
            type = storeOpen;
        else
            type = normal;

        SetTilesIndex(type);

        SetMergeData();
    }

    private void Start()
    {
        int num;

        hintTime = 0;
        StartCoroutine(HintCo());

        StartCoroutine(ComeMerge());
        Discription(null);

        if (type == storeOpen)
        {
            normal.gameObject.SetActive(false);
            storeOpen.gameObject.SetActive(true);
            SetStoreMission();
            num = UserDataManager.instance.GetUserData().storeOpenMerge;
        }
        else
        {
            normal.gameObject.SetActive(true);
            storeOpen.gameObject.SetActive(false);
            SetMission();
            num = UserDataManager.instance.GetUserData().normalMerge;
        }

        for(int i = 0; i<num; i++)
        {
            FillSlider();
        }
    }

    // 미션 생성
    private void SetMission()
    {
        List<MergeItem> items = new List<MergeItem>();
        int missionNum = 0;
        
        foreach(Slider slider in type.sliders)
        {
            slider.value = 0;
        }

        type.slidersText.text = "0/10";

        if (UserDataManager.instance.GetUserData().mergeMission.Count != 0)
        {
            foreach (int value in UserDataManager.instance.GetUserData().mergeMission)
            {
                if (value == -1)
                {
                    if (items.Count != 0)
                    {
                        normal.needInfoes[missionNum++].SetIcon(items, false);
                        items = new List<MergeItem>();
                    }
                }
                else
                {
                    int itemId = value;

                    ItemLine line = (ItemLine)(itemId / 100);
                    itemId %= 100;
                    ItemType type = (ItemType)(itemId / 10);
                    itemId %= 10;
                    int level = itemId;

                    items.Add(SearchMergeItem(line, type, level));
                }
            }
        }
        else
        {
            foreach (MergeNeedInfo info in normal.needInfoes)
            {
                info.SetIcon(GetRandItems(), false);
            }

            MissionSave();
        }

        NeedCheck();
    }

    // store 미션 할당
    private void SetStoreMission()
    {
        type.spine.skeletonDataAsset = MergeDataManager.instance.store.spineAsset;
        type.spine.Initialize(true);
        type.spine.AnimationState.SetAnimation(0, $"Idle{MergeDataManager.instance.store.step + 1}", false);

        storeOpenPop.SetPopup(MergeDataManager.instance.store);

        List<MergeItem> items = new List<MergeItem>();
        int missionNum = 0;

        foreach (Slider slider in type.sliders)
        {
            slider.value = 0;
        }

        type.slidersText.text = "0/10";

        if (UserDataManager.instance.GetUserData().storeMergeMission.Count != 0)
        {
            foreach (int value in UserDataManager.instance.GetUserData().storeMergeMission)
            {
                if (value == -1)
                {
                    if (items.Count != 0)
                    {
                        type.needInfoes[missionNum++].SetIcon(items, false);
                        items = new List<MergeItem>();
                    }
                }
                else
                {
                    int itemId = value;

                    ItemLine line = (ItemLine)(itemId / 100);
                    itemId %= 100;
                    ItemType type = (ItemType)(itemId / 10);
                    itemId %= 10;
                    int level = itemId;

                    items.Add(SearchMergeItem(line, type, level));
                }
            }
        }
        else
        {
            foreach (MergeNeedInfo info in type.needInfoes)
            {
                info.SetIcon(GetRandItems(), false);
            }

            StoreMissionSave();
        }

        NeedCheck();
    }

    // 서버 데이터 불러오기
    private void SetMergeData()
    {
        lines = new List<ItemLine>();

        readyItems = new List<MergeItem>();

        if (UserDataManager.instance.GetUserData().getMergeItem != null)
        {
            foreach(int itemId in UserDataManager.instance.GetUserData().getMergeItem)
            {
                int num = itemId;
                ItemLine line = (ItemLine)(num / 100);
                num %= 100;
                ItemType type = (ItemType)(num / 10);
                num %= 10;
                int level = num;

                MergeItem mergeItem = SearchMergeItem(line, type, level);

                if (mergeItem.itemType == ItemType.Spawner)
                    lines.Add(mergeItem.itemLine);

                readyItems.Add(mergeItem);
            }
        }

        StartCoroutine(SetReadyItem());

        if (UserDataManager.instance.GetUserData().fieldItem != null)
        {
            foreach (int key in UserDataManager.instance.GetUserData().fieldItem.Keys)
            {
                Tile tile = SearchTileInIndex(key);

                int num = UserDataManager.instance.GetUserData().fieldItem[key];
                ItemLine line = (ItemLine)(num / 100);
                num %= 100;
                ItemType type = (ItemType)(num / 10);
                num %= 10;
                int level = num;

                MergeItem mergeItem = SearchMergeItem(line, type, level);

                if (mergeItem.itemType == ItemType.Spawner)
                    lines.Add(mergeItem.itemLine);

                Item item = Instantiate(spawnItem, itemField);

                item.GetComponent<RectTransform>().anchoredPosition = tile.GetComponent<RectTransform>().anchoredPosition;
                item.StartSetItem(mergeItem, tile, (int)type == 2 ? true : false);
            }
        }

        if (UserDataManager.instance.GetUserData().storageItem != null)
        {
            foreach (int value in UserDataManager.instance.GetUserData().storageItem)
            {
                int num = value;
                ItemLine line = (ItemLine)(num / 100);
                num %= 100;
                ItemType type = (ItemType)(num / 10);
                num %= 10;
                int level = num;

                MergeItem mergeItem = SearchMergeItem(line, type, level);

                if (mergeItem.itemType == ItemType.Spawner)
                    lines.Add(mergeItem.itemLine);

                Item item = Instantiate(spawnItem, itemField);
                item.StartSetItem(mergeItem, null);
                item.inStorage = true;

                foreach (Transform trans in this.type.storage.storageTrans)
                {
                    if (trans.transform.childCount == 0)
                    {
                        item.transform.SetParent(trans);
                        item.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                        break;
                    }
                }
            }
        }
    }

    public IEnumerator SetReadyItem()
    {
        if (readyItems.Count == 0)
        {
            type.readyItemField.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f);
            yield return new WaitForSeconds(0.5f);
            type.readyItemField.gameObject.SetActive(false);
        }
        else
        {
            Item item = Instantiate(spawnItem, type.readyItemTrans);
            item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            item.StartSetItem(readyItems[0], null);
            readyItems.RemoveAt(0);
            item.isReady = true;
            item.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            item.GetComponent<BoxCollider2D>().enabled = false;

            Vector2 size;

            if (item.item.itemType == ItemType.Item)
                size = new Vector2(140, 140);
            else
                size = new Vector2(160, 160);

            item.GetComponent<RectTransform>().DOSizeDelta(size, 0.5f);
            yield return new WaitForSeconds(0.5f);
            item.GetComponent<BoxCollider2D>().enabled = true;
        }

    }

    public void FillSlider()
    {
        int num = 0;

        foreach(Slider slider in type.sliders)
        {
            if (slider.value == 0)
            {
                slider.value = 1;
                num++;
                break;
            }
            else
                num++;
        }

        type.slidersText.text = $"{num}/10";

        if (num < 10)
            return;

        if (type == normal)
        {
            EarningManager.Instance.AddGem(30);
            EarningManager.Instance.area.SetEarningParticle(type.sliders[4].transform, 2);


            ////////////////////////////// 임시 초기화
            foreach (Slider slider in type.sliders)
            {
                slider.value = 0;
            }

            type.slidersText.text = "0/10";

            UserDataManager.instance.GetUserData().normalMerge = 0;
        }
        else
        {
            storeOpenPop.gameObject.SetActive(true);

            UIOpen();

            UserDataManager.instance.GetUserData().storeOpenMerge = -1;
        }
    }

    // 필요 아이템 검색
    private MergeItem SearchMergeItem(ItemLine line, ItemType type, int level)
    {
        MergeItem item = null;

        if (type == ItemType.Spawner)
        {
            foreach(MergeItem mergeItem in spawners)
            { 
                if(mergeItem.itemLine == line &&
                    mergeItem.Level == level)
                {
                    item = mergeItem;
                    return item;
                }
            }
        }

        foreach(MergeItem mergeItem in items)
        {
            if(mergeItem.itemLine == line &&
               mergeItem.Level == level)
            {
                item = mergeItem;
                return item;
            }
        }

        return item;
    }

    // 타일 번호 할당
    private void SetTilesIndex(MergeGameType type)
    {
        for (int i = 0; i < type.tiles.Count; i++)
        {
            type.tiles[i].index = i;
        }
    }

    private Tile SearchTileInIndex(int num)
    {

        for (int i = 0; i < type.tiles.Count; i++)
        {
            if (type.tiles[i].index == num)
            {
                return type.tiles[i];
            }
        }

        return null;
    }

    // 가장 가까이에 있는 타일 반환
    public Tile GetNearTile(Vector2 pos)
    {
        Tile result = null;

        foreach (Tile tile in type.tiles)
        {
            if (!tile.seal)
            {
                if (result == null)
                    result = tile;
                else if (Vector2.Distance(pos, tile.GetComponent<RectTransform>().anchoredPosition) < Vector2.Distance(pos, result.GetComponent<RectTransform>().anchoredPosition))
                    result = tile;

            }
        }

        return result;
    }

    // 가장 가까이있는 비어있는 타일 반환
    public Tile GetNearAndEmptyTile(Vector2 pos)
    {
        Tile result = null;

        foreach (Tile tile in type.tiles)
        {
            if (tile.item == null && !tile.seal)
            {
                if (result == null)
                    result = tile;
                else if (Vector2.Distance(pos, tile.GetComponent<RectTransform>().anchoredPosition) < Vector2.Distance(pos, result.GetComponent<RectTransform>().anchoredPosition))
                    result = tile;
            }
        }

        return result;
    }

    // 아이템 선택
    public void SetSelect(Item item)
    {
        Discription(item);

        select.transform.localScale = new Vector3(1, 1, 1);

        if (item == null)
        {
            select.gameObject.SetActive(false);
        }
        else
        {
            select.transform.SetParent(item.nowTile.transform);
            select.gameObject.SetActive(true);
        }

        select.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    // 스폰 아이템
    public void SpawnItem(Vector2 pos, ItemLine itemLine)
    {
        Tile tile = GetNearAndEmptyTile(pos);

        if (tile == null)
        {
            Warning();
            return;
        }

        MergeItem mergeItem = GetRandomStartItem(itemLine);

        for (int i = 0; i < itemField.childCount; i++)
        {
            if (!itemField.GetChild(i).gameObject.activeSelf &&
                itemField.GetChild(i).CompareTag("MergeItem"))
            {
                itemField.GetChild(i).GetComponent<RectTransform>().anchoredPosition = pos;
                itemField.GetChild(i).GetComponent<Item>().SetItem(mergeItem, tile);

                // 버그 주의
                if (UserDataManager.instance.GetUserData().fieldItem.ContainsKey(tile.index))
                    UserDataManager.instance.GetUserData().fieldItem[tile.index] = ChangeItemDate(mergeItem);
                else
                    UserDataManager.instance.GetUserData().fieldItem.Add(tile.index, ChangeItemDate(mergeItem));
                //UserDataManager.instance.UpdateUserData_fieldItem();

                return;
            }
        }

        Item item = Instantiate(spawnItem, itemField);
        item.GetComponent<RectTransform>().anchoredPosition = pos;
        item.SetItem(mergeItem, tile);

        // 버그 주의
        if (UserDataManager.instance.GetUserData().fieldItem.ContainsKey(tile.index))
            UserDataManager.instance.GetUserData().fieldItem[tile.index] = ChangeItemDate(mergeItem);
        else
            UserDataManager.instance.GetUserData().fieldItem.Add(tile.index, ChangeItemDate(mergeItem));
        //UserDataManager.instance.UpdateUserData_fieldItem();
    }

    public int ChangeItemDate(MergeItem item, bool inActive = false)
    {
        int num = 0;

        num += (int)item.itemLine * 100;
        num += (int)item.itemType * 10;
        num += item.Level;

        // 비활성화 아이템 구분
        if (inActive)
            num += 20;

        return num;
    }

    // 1레벨 아이템 랜덤 반환
    private MergeItem GetRandomStartItem(ItemLine line)
    {
        foreach(MergeItem item in items)
        {
            if(item.itemLine == line && item.Level == 1)
            {
                return item;
            }
        }

        return null;
    }

    // 아이템 업그레이드
    public MergeItem UpgradeItem(MergeItem mergeItem)
    {
        MergeItem result = null;

        if (mergeItem.itemType == ItemType.Spawner)
        {
            foreach (MergeItem item in spawners)
            {
                if (item.itemLine == mergeItem.itemLine &&
                   item.itemType == mergeItem.itemType &&
                   item.Level == mergeItem.Level + 1)
                {
                    result = item;
                    break;
                }
            }
        }
        else
        {
            foreach (MergeItem item in items)
            {
                if (item.itemLine == mergeItem.itemLine &&
                   item.itemType == mergeItem.itemType &&
                   item.Level == mergeItem.Level + 1)
                {
                    result = item;
                    break;
                }
            }
        }

        return result;
    }

    // 미션 설정에 들어갈 랜덤 아이템 리스트 반환
    public List<MergeItem> GetRandItems()
    {
        List<MergeItem> list = new List<MergeItem>();

        //
        int num = Random.Range(1, 3);

        while (true)
        {
            if (list.Count == num)
                break;


            MergeItem item = RandomItem(3);

            bool pass = true;

            // 중복 검사
            foreach (MergeItem mergeItem in list)
            {
                if (mergeItem == item)
                    pass = false;
            }

            if (pass)
                list.Add(item);
        }

        return list;
    }

    private MergeItem RandomItem(int maxLevel)
    {
        MergeItem item = null;

        while (true)
        {
            item = items[Random.Range(0, items.Count)];

            bool pass = false;

            foreach(ItemLine line in lines)
            {
                if (line == item.itemLine)
                {
                    pass = true;
                    break;
                }
            }

            if (item.Level > maxLevel || !pass)
                item = null;
            else
                return item;
        }
    }

    // 미션 클리어에 필요한 물품 확인
    public void NeedCheck(bool check = false)
    {
        // 가이드 말풍선 비활성화
        guide.gameObject.SetActive(false);

        foreach (MergeNeedInfo info in type.needInfoes)
        {
            for (int i = 0; i < info.needItems.Count; i++)
            {
                if (SearchFieldItem(info.needItems[i]) != null)
                {
                    if (!info.needItemIcons[i].need)
                    {
                        if(!info.clear)
                            info.needItemIcons[i].NeedOn();

                        info.needCount++;

                        if (info.needCount == info.needItems.Count)
                        {
                            info.rewardBtn.gameObject.SetActive(true);
                        }
                    }
                }
                else
                {
                    if (info.needItemIcons[i].need)
                    {
                        if(!info.clear)
                            info.needItemIcons[i].NeedOff();

                        info.needCount--;

                        if (info.needCount != info.needItems.Count)
                        {
                            info.rewardBtn.gameObject.SetActive(false);
                        }
                    }
                }

                info.needItemIcons[i].SetNeedCount(SearchFieldItemCount(info.needItems[i]));
            }
        }

        if(!check)
            SortInfo();

        hintTime = 0;
    }

    // 정렬
    private void SortInfo()
    {

        for(int i = 0; i<type.needInfoes.Count; i++)
        {
            if (type.needInfoes[i].needItems.Count == type.needInfoes[i].needCount && !type.needInfoes[i].clear)
            {
                for (int j = 0; j < i; j++)
                {
                    if (type.needInfoes[j].needItems.Count != type.needInfoes[j].needCount && !type.needInfoes[j].clear)
                    {
                        type.needInfoes.Insert(j, type.needInfoes[i]);
                        type.needInfoes.RemoveAt(i+1);

                        break;
                    }
                }
            }
        }

        float posX = 0;
        Vector2 nowPos;

        foreach(MergeNeedInfo needInfo in type.needInfoes)
        {
            if (needInfo.gameObject.activeSelf)
            {
                posX += needInfo.GetComponent<RectTransform>().sizeDelta.x / 2;
                nowPos = needInfo.GetComponent<RectTransform>().anchoredPosition;
                needInfo.GetComponent<RectTransform>().anchoredPosition = nowPos;
                needInfo.GetComponent<RectTransform>().DOAnchorPosX(posX, 0.5f);
                posX += needInfo.GetComponent<RectTransform>().sizeDelta.x / 2;
                posX += 30;
            }
        }

        // reward Box 크기 만큼 추가
        if (type.readyItemField.gameObject.activeSelf)
            posX += 545;
        else
            posX += 335;

        type.scrollBack.GetComponent<RectTransform>().sizeDelta = new Vector2(posX, type.scrollBack.GetComponent<RectTransform>().sizeDelta.y);
    }

    // 필드에 있는 아이템 검색
    private MergeItem SearchFieldItem(MergeItem item)
    {
        for (int i = 0; i < itemField.childCount; i++)
        {
            if (itemField.GetChild(i).gameObject.activeSelf
                && itemField.GetChild(i).GetComponent<Item>().item == item
                && !itemField.GetChild(i).GetComponent<Item>().inActive
                && itemField.GetChild(i).GetComponent<Item>().nowTile != null)
            {
                return itemField.GetChild(i).GetComponent<Item>().item;
            }
        }

        return null;
    }

    // autoMerge에서 사용됨
    public Item SearchFieldItem(Item item)
    {
        for (int i = 0; i < itemField.childCount; i++)
        {
            if (itemField.GetChild(i).gameObject.activeSelf
                && itemField.GetChild(i).GetComponent<Item>().item == item.item
                && itemField.GetChild(i).GetComponent<Item>().nowTile != item.nowTile
                && !itemField.GetChild(i).GetComponent<Item>().inActive)
            {
                return itemField.GetChild(i).GetComponent<Item>();
            }
        }

        return null;
    }

    public int SearchFieldItemCount(MergeItem item)
    {
        int num = 0;

        for (int i = 0; i < itemField.childCount; i++)
        {
            if (itemField.GetChild(i).gameObject.activeSelf
                && itemField.GetChild(i).GetComponent<Item>().item == item
                && !itemField.GetChild(i).GetComponent<Item>().inActive
                && itemField.GetChild(i).GetComponent<Item>().nowTile != null)
            {
                num++;
            }
        }

        return num;
    }

    // 상점에서 아이템 꺼내기
    public void TakeOutStorage(Item item)
    {
        item.transform.SetParent(itemField);
        item.inStorage = false;
        UserDataManager.instance.GetUserData().storageItem.Remove(ChangeItemDate(item.item));
        //UserDataManager.instance.UpdateUserData_storageItem();
        item.transform.SetAsLastSibling();
        storage.SetActive(false);
        UIClose();
        NeedCheck();
    }

    public void TakeInStorage(Item item)
    {
        // 상자에서 꺼내서 다시 상자에 넣는 경우를 위한 if 문
        if (item.nowTile != null)
        {
            item.nowTile.item = null;

            UserDataManager.instance.GetUserData().fieldItem.Remove(item.nowTile.index);
            //UserDataManager.instance.UpdateUserData_fieldItem();

            item.nowTile = null;
        }

        UserDataManager.instance.GetUserData().storageItem.Add(ChangeItemDate(item.item));
        //UserDataManager.instance.UpdateUserData_storageItem();

        foreach (Transform trans in type.storage.storageTrans)
        {
            if (trans.transform.childCount == 0)
            {
                item.transform.SetParent(trans);
                item.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                break;
            }
        }

        type.storage.StorageClose();
        NeedCheck();
    }

    // 미션 보상 받기
    public void GetMissionReward(MergeNeedInfo info)
    {
        MasterAudio.PlaySound("머지");
        info.rewardBtn.gameObject.SetActive(false);
        info.clear = true;

        float time = 0;

        foreach (MergeItem item in info.needItems)
        {
            for (int i = 0; i < itemField.childCount; i++)
            {
                if (itemField.GetChild(i).gameObject.activeSelf)
                {
                    Item targetItem = itemField.GetChild(i).GetComponent<Item>();

                    if (targetItem.item == item && !targetItem.inActive)
                    {
                        Vector2 movePos = Camera.main.WorldToScreenPoint(info.needPanel.transform.position);

                        StartCoroutine(targetItem.CollectItem(movePos));

                        if (targetItem.MoveTime(movePos) > time)
                            time = targetItem.MoveTime(targetItem.GetScreenPointToRectangle(movePos));
                        
                        MissionManager.instance.UpdateMissionProgress(MissionType.MergeClear, 1);
                        break;
                    }
                }
            }
        }

        if (MergeDataManager.instance.storeOpen)
        {
            StartCoroutine(info.ClearMission(time));

            UserDataManager.instance.GetUserData().storeOpenMerge++;
        }
        else
        {
            switch (info.rewardType)
            {
                case RewardType.Ticket:
                    // 미구현
                    break;
                case RewardType.Medal:
                    EarningManager.Instance.AddMedal(info.reward);
                    EarningManager.Instance.area.SetEarningParticle(info.transform, 1);
                    break;
                case RewardType.Gem:
                    EarningManager.Instance.AddGem((int)info.reward);
                    EarningManager.Instance.area.SetEarningParticle(info.transform, 2);
                    break;
            }

            StartCoroutine(info.ClearMission(time));

            UserDataManager.instance.GetUserData().normalMerge++;
        }

        FillSlider();
    }

    // 아이템 설명 출력
    private void Discription(Item item)
    {
        if (item == null)
        {
            type.itemDiscriptionText.text = string.Empty;
            type.itemSellBtn.gameObject.SetActive(false);
        }
        else
        {
            type.itemDiscriptionText.text = item.item.discription;

            type.itemSellIcon.sprite = rewardIcons[1];
            switch (item.item.Level)
            {
                case 0:
                    type.itemSellBtn.gameObject.SetActive(false);
                    return;
                case 1:
                    type.itemSellText.text = "20";
                    break;
                case 2:
                    type.itemSellText.text = "50";
                    break;
                case 3:
                    type.itemSellText.text = "90";
                    break;
                case 4:
                    type.itemSellText.text = "140";
                    break;
                case 5:
                    type.itemSellText.text = "200";
                    break;
            }

            if(item.inActive || item.item.itemType == ItemType.Spawner)
                type.itemSellBtn.gameObject.SetActive(false);
            else
                type.itemSellBtn.gameObject.SetActive(true);
        }
    }

    // 아이템 판매
    public void SellItem()
    {
        Item item = select.transform.parent.GetComponent<Tile>().item;

        switch (item.item.Level)
        {
            case 1:
                EarningManager.Instance.AddMedal(20);
                break;
            case 2:
                EarningManager.Instance.AddMedal(50);
                break;
            case 3:
                EarningManager.Instance.AddMedal(90);
                break;
            case 4:
                EarningManager.Instance.AddMedal(140);
                break;
            case 5:
                EarningManager.Instance.AddMedal(200);
                break;
        }

        EarningManager.Instance.area.SetEarningParticle(item.transform, 1);

        item.Sell();

        SetSelect(null);
        NeedCheck();
    }

    // 마을로 돌아가면서 storeOpen 초기화
    public void ReturnVillage()
    {
        StartCoroutine(ReturnVil());
    }

    private IEnumerator ReturnVil()
    {
        returnImage.transform.parent.gameObject.SetActive(true);
        returnImage.transform.GetComponent<RectTransform>().DOScale(new Vector3(50, 50, 50), 1f);
        yield return new WaitForSeconds(1f);
        MergeDataManager.instance.storeOpen = false;
        SceneController.Instance.MoveStageScene();
    }
    private IEnumerator ComeMerge()
    {
        returnImage.transform.parent.gameObject.SetActive(true);
        returnImage.transform.GetComponent<RectTransform>().localScale = new Vector3(50, 50, 50);
        returnImage.transform.GetComponent<RectTransform>().DOScale(new Vector3(0, 0, 0), 1f);
        yield return new WaitForSeconds(0.5f);
        NeedCheck();
        yield return new WaitForSeconds(0.5f);
        returnImage.transform.parent.gameObject.SetActive(false);
    }

    // 같은 아이템끼리 만나면 파티클 재생
    public void SetMergeParticle(Item item)
    {
        mergeParticle.transform.localScale = new Vector3(80, 80, 80);

        if (item == null)
        {
            mergeParticle.gameObject.SetActive(false);

            if (!mergeParticle.isPaused)
                mergeParticle.Pause();
        }
        else
        {
            if (UpgradeItem(item.item) == null)
                return;

            item.transform.SetSiblingIndex(itemField.childCount - 2);
            mergeParticle.transform.SetParent(item.transform);
            mergeParticle.gameObject.SetActive(true);

            if (!mergeParticle.isPlaying)
                mergeParticle.Play();
        }

        mergeParticle.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    // Merge 되었을 때 파티클 재생
    public void PlayMergeEndParticle(Item item)
    {
        mergeEndParticle.transform.localScale = new Vector3(50, 50, 50);
        mergeEndParticle.gameObject.SetActive(false);

        if (item == null)
            return;

        mergeEndParticle.transform.SetParent(item.transform, false);
        mergeEndParticle.gameObject.SetActive(true);
        mergeEndParticle.Play();
    }

    // UI 창을 열 때
    public void UIOpen()
    {
        uiOpen = true;

        PlayMergeEndParticle(null);
        
        for(int i = 0; i<itemField.childCount; i++)
        {
            if(itemField.GetChild(i).GetComponent<BoxCollider2D>())
            {
                itemField.GetChild(i).GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }

    // UI 창을 닫을 때 
    public void UIClose()
    {
        uiOpen = false;

        for (int i = 0; i < itemField.childCount; i++)
        {
            if (itemField.GetChild(i).GetComponent<BoxCollider2D>())
            {
                itemField.GetChild(i).GetComponent<BoxCollider2D>().enabled = true;
            }
        }
    }

    // 창고에서 아이템을 뺄 때 필드가 가득 차 있으면 경고 메세지 띄우기
    public void Warning(bool spawn = false)
    {
        for(int i = 0; i<warningArea.childCount; i++)
        {
            if (!warningArea.GetChild(i).gameObject.activeSelf)
            {
                warningArea.GetChild(i).gameObject.SetActive(true);

                if (spawn)
                    warningArea.GetChild(i).GetComponent<TextMeshProUGUI>().text = "에너지가 부족합니다.";
                else
                    warningArea.GetChild(i).GetComponent<TextMeshProUGUI>().text = "필드에 자리가 부족합니다.";

                return;
            }
        }

        Warning warn = Instantiate(warning, warningArea);

        if (spawn)
            warn.GetComponent<TextMeshProUGUI>().text = "에너지가 부족합니다.";
        else
            warn.GetComponent<TextMeshProUGUI>().text = "필드에 자리가 부족합니다.";
    }

    // 필드에 아이템을 놓을 수 있는지 확인
    public bool SpawnAbleCheck()
    {
        int num = 0;

        for(int i = 0; i<itemField.childCount; i++)
        {
            if (itemField.GetChild(i).GetComponent<Item>() && itemField.GetChild(i).gameObject.activeSelf && itemField.GetChild(i).GetComponent<Item>().nowTile != null)
                num++;
        }

        return num < 63;
    }

    // 미션창의 아이템을 눌렀을 때 그 아이템을 생산할 수 있는 스포너 아이콘 띄우기
    public void SetGuide(MergeItem item)
    {
        // itemInfo
        int i = 1;
        MergeItem spawnerItem = null;

        foreach(Image img in guide.itemInfoes)
        {
            MergeItem mergeItem = SearchMergeItem(item.itemLine, item.itemType, i);

            if (mergeItem)
                img.sprite = mergeItem.icon;

            img.gameObject.SetActive(true);

            mergeItem = SearchFieldItem(SearchMergeItem(item.itemLine, ItemType.Spawner, i++));

            if (mergeItem != null)
                spawnerItem = mergeItem;
        }

        if (spawnerItem == null)
            guide.spawnerImage.sprite = SearchMergeItem(item.itemLine, ItemType.Spawner, 1).icon;
        else
            guide.spawnerImage.sprite = spawnerItem.icon;

        guide.spawnerImage.gameObject.SetActive(true);

        // spawnerInfo

        i = 1;

        foreach(Image image in guide.spawnerInfoes)
        {
            MergeItem mergeItem = SearchMergeItem(item.itemLine, ItemType.Spawner, i++);

            if (mergeItem)
                image.sprite = mergeItem.icon;

            image.gameObject.SetActive(true);
        }

        foreach(GameObject obj in guide.productItems)
        {
            obj.SetActive(false);
        }

        i = 1;

        foreach (GameObject obj in guide.productItems)
        {
            MergeItem mergeItem = SearchMergeItem(item.itemLine, item.itemType, i++);

            if (mergeItem)
            {
                obj.transform.GetChild(0).GetComponent<Image>().sprite = mergeItem.icon;
                obj.SetActive(true);
            }
            else
                break;
        }

        guide.gameObject.SetActive(true);
        UIOpen();
    }

    public void MissionSave()
    {
        UserDataManager.instance.GetUserData().mergeMission.Clear();

        foreach (MergeNeedInfo info in type.needInfoes)
        {
            foreach(MergeItem item in info.needItems)
            {
                UserDataManager.instance.GetUserData().mergeMission.Add(ChangeItemDate(item));
            }

            UserDataManager.instance.GetUserData().mergeMission.Add(-1);
        }

        //UserDataManager.instance.UpdateUserData_mergeMission();
    }

    public void StoreMissionSave()
    {
        UserDataManager.instance.GetUserData().storeMergeMission.Clear();

        foreach (MergeNeedInfo info in type.needInfoes)
        {
            foreach(MergeItem item in info.needItems)
            {
                UserDataManager.instance.GetUserData().storeMergeMission.Add(ChangeItemDate(item));
            }

            UserDataManager.instance.GetUserData().storeMergeMission.Add(-1);
        }

        //UserDataManager.instance.UpdateUserData_mergeMission();
    }

    public IEnumerator HintCo()
    {
        while(true)
        {
            if(hintTime > 2)
            {
                Hint();
                hintTime = 0;
            }
            yield return new WaitForSeconds(1);
            hintTime++;
        }
    }

    private void Hint()
    {
        MergeItem target = null;
        MergeItem mergeItem = null;

        foreach (MergeNeedInfo info in type.needInfoes)
        {
            for(int i = 0; i<info.needItems.Count; i++)
            {
                if(!info.needItemIcons[i].need)
                {
                    target = info.needItems[i];
                    break;
                }
            }

            if (target != null)
                break;
        }

        if (target == null)
            return;

        for(int i = target.Level - 1; i>0; i--)
        {
            mergeItem = SearchMergeItem(target.itemLine, target.itemType, i);
            List<Item> items = new List<Item>();

            for(int j = 0; j<itemField.childCount; j++)
            {
                if(itemField.GetChild(j).gameObject.activeSelf &&
               itemField.GetChild(j).GetComponent<Item>().item == mergeItem)
                {
                    items.Add(itemField.GetChild(j).GetComponent<Item>());
                }

                if (items.Count == 2)
                    break;
            }

            if(items.Count == 2)
            {
                StartCoroutine(items[0].HintMove(items[1].rect.anchoredPosition));
                StartCoroutine(items[1].HintMove(items[0].rect.anchoredPosition));

                return;
            }
        }

        for(int i = 0; i<itemField.childCount; i++)
        {
            if (itemField.GetChild(i).gameObject.activeSelf &&
               itemField.GetChild(i).GetComponent<Item>().item.itemLine == target.itemLine &&
               itemField.GetChild(i).GetComponent<Item>().item.itemType == ItemType.Spawner)
            {
                StartCoroutine(itemField.GetChild(i).GetComponent<Item>().HintMove());
                break;
            }
        }
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            //UserDataManager.instance.UpdateUserData_MergeData();
        }
    }
}