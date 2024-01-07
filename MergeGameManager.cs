using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using DarkTonic.MasterAudio;
using Spine.Unity;
using System;
using Spine;

public class MergeGameManager : MonoBehaviour
{
    public static MergeGameManager instance;

    public Tutorial tutorial;
    [SerializeField] private GameObject select;
    [SerializeField] private List<Sprite> selectImgs;
    public Transform itemField;
    [SerializeField] private Item spawnItem;
    [SerializeField] private GameObject storage;
    [SerializeField] private List<Sprite> rewardIcons;
    [SerializeField] private Transform warningArea;
    [SerializeField] private Warning warning;
    [SerializeField] private ParticleSystem mergeParticle;
    public ParticleSystem makingParticle;
    [SerializeField] private SkeletonGraphic energySpine;
    [SerializeField] private SkeletonGraphic hintHand;
    [SerializeField] private ParticleSystem mergeEndParticle;
    [SerializeField] private Guide guide;
    [SerializeField] private GameObject generaterInfo;
    [SerializeField] private List<GameObject> covers;

    [Header("MergeType")]
    public MergeGameType normal;

    public List<Animalspecialization> lines;
    public List<MergeItem> readyItems;

    [Header("ItemDiscription")]
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDiscriptionText;
    [SerializeField] private Button itemSellBtn;
    [SerializeField] private Button adChargeBtn;
    [SerializeField] private Button gemChargeBtn;
    [SerializeField] private TextMeshProUGUI adTimer;
    [SerializeField] private TextMeshProUGUI gemTimer;
    [SerializeField] private TextMeshProUGUI gemChargeCost;
    [SerializeField] private Image itemSellIcon;
    [SerializeField] private TextMeshProUGUI itemSellText;
    [SerializeField] private TextMeshProUGUI itemSellModeText;
    [SerializeField] private TextMeshProUGUI itemLevelText;
    [SerializeField] private TextMeshProUGUI spawnerStamina;
    [SerializeField] private Button discriptInfoBtn;
    [SerializeField] private Image discriptInfoImg;
    [SerializeField] private ContentSizeFitter textFitter;
    public Item discriptItem;

    [Header("Storage")]
    public Storage storageBtn;
    public GameObject storageMsg;

    [SerializeField] private OldCat oldCat;

    public bool uiOpen;
    private int hintTime;
    private int hinthint;
    private MergeItem guideItem;
    private Store store;

    public GameObject loading;

    [Header("VillageMission")]
    [SerializeField] private MissionCard missionCard;
    private List<Mission> clearMissions;
    private List<Mission> readyMissions;

    [Header("NPCMission")]
    [SerializeField] private MergeNeedInfo npcMissionInfo;
    [SerializeField] private WantMission wantMissionPanel;

    [Header("RefillPop")]
    [SerializeField] private GameObject refillPop;
    [SerializeField] private TextMeshProUGUI adGemText;
    [SerializeField] private TextMeshProUGUI adEnergyText;
    [SerializeField] private TextMeshProUGUI chargeCost;
    [SerializeField] private Button daliyFreeGemBtn;
    [SerializeField] private Button adGemBtn;

    [Header("CraftList")]
    [SerializeField] private CraftList craftListPanel;

    [Header("BoxingImgs")]
    public List<Sprite> boxingImgs;

    [Header("NewSpawner")]
    [SerializeField] private SpawnerUpPop spawnerUpPop;

    [Header("Effect")]
    public SpecialEffect specialEffect;
    public BoxCrash boxCrash;
    public ChargeEffect chargeEffect;

    [Header("MergeShop")]
    [SerializeField] private MergeShop mergeShop;

    [Header("SystemPop")]
    public GameObject connectPop;
    public GameObject exitPop;

    public bool isDrage;
    private UserData data;
    private DateTime startTime;
    public bool moveScene;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        SetTilesIndex();
        storageBtn.SetStorage(UserDataManager.instance.GetUserData().storageCount);
        SetMergeData();
    }

    private void Start()
    {
        moveScene = false;
        Input.multiTouchEnabled = false;

        tutorial.SetTutorial(4);
        SetCover();

        hintTime = 0;
        hinthint = 0;
        hintHand.AnimationState.TimeScale = 0;

        clearMissions = new List<Mission>();
        readyMissions = new List<Mission>();

        // 이미 클리어된 미션 체크
        foreach (Mission mission in MissionManager.instance.GetMissions())
        {
            if (mission.type == MissionType.MergeClear || mission.type == MissionType.MergeSpawn || mission.type == MissionType.MergeCombine)
                if (mission.progress == mission.goal)
                    clearMissions.Add(mission);
        }

        StartCoroutine(HintCo());
        Discription(null, true);

        select.GetComponent<Image>().sprite = selectImgs[1];
        SetMission();
        SetNPCMission();

        StartCoroutine(mergeTimeCheck());
        StartCoroutine(SetMergeShopTime());
        NeedCheck();

        if (MissionManager.instance.GetGuideMissions().Count > 0)
        {
            normal.needInfoes[1].gameObject.SetActive(false);
            normal.needInfoes[2].gameObject.SetActive(false);
        }

        foreach (Store store in MergeDataManager.instance.storeList)
        {
            if (store.BuyAndStepUpCost() <= EarningManager.Instance.mergeCoin)
            {
                oldCat.Pop();
                break;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            exitPop.SetActive(true);
            UIOpen();
        }
    }

    public void EscapeGame()
    {
        UserDataManager.instance.UpdateUserData_MergeData(false, true);
    }

    // 머지 미션 세팅
    private void SetMission()
    {
        List<MergeItem> items = new List<MergeItem>();
        int missionNum = 0;

        if (data.mergeMission.Count != 0)
        {
            foreach (int value in data.mergeMission)
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

                    items.Add(MergeDataManager.instance.DataChangeItem(itemId));
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

    // 서버 데이터 불러오기
    private void SetMergeData()
    {
        data = UserDataManager.instance.GetUserData();

        lines = new List<Animalspecialization>();

        readyItems = new List<MergeItem>();

        foreach (int itemId in data.getMergeItem)
        {
            MergeItem mergeItem = MergeDataManager.instance.DataChangeItem(itemId);

            if (mergeItem != null)
            {
                if (mergeItem.itemType == ItemType.Spawner)
                {
                    if((int)mergeItem.itemLine < 13)
                        lines.Add(mergeItem.itemLine);
                }

                readyItems.Add(mergeItem);
            }
        }

        StartCoroutine(SetReadyItem());

        foreach (int key in data.fieldItem.Keys)
        {
            Tile tile = SearchTileInIndex(key);

            double num = data.fieldItem[key];

            bool active = false;

            if (num < 0)
            {
                active = true;
                num *= -1;
            }

            MergeItem mergeItem = MergeDataManager.instance.DataChangeItem(num);

            if (mergeItem.itemType == ItemType.Spawner)
            {
                if ((int)mergeItem.itemLine < 13)
                    lines.Add(mergeItem.itemLine);
            }

            Item item = Instantiate(spawnItem, itemField);

            // 버블 체크
            if (data.bubbles.ContainsKey(tile.index))
                item.SetBubble(data.bubbles[tile.index]);

            item.GetComponent<RectTransform>().anchoredPosition = tile.GetComponent<RectTransform>().anchoredPosition;
            item.StartSetItem(mergeItem, tile, active);

            // 박싱 대상이면 박싱하고 아니고 도감에 등록 안되어 있는 아이템이면 도감 등록
            if (data.boxingTiles.Contains(tile.index))
                item.Boxing();
            else if(!data.dicItems.Contains(num))
                data.dicItems.Add(num);

            // 에너지 데이터를 가져오기 위해 미리 할당
            item.item = mergeItem;
            tile.item = item;
        }

        if (data.storageItem != null)
        {
            foreach (int value in data.storageItem)
            {
                MergeItem mergeItem = MergeDataManager.instance.DataChangeItem(value);

                if (mergeItem.itemType == ItemType.Spawner)
                {
                    if ((int)mergeItem.itemLine < 13)
                        lines.Add(mergeItem.itemLine);
                }

                Item item = Instantiate(spawnItem, itemField);
                item.StartSetItem(mergeItem, null);
                item.GetComponent<Canvas>().sortingOrder = 6;
                item.inStorage = true;

                foreach (Transform trans in storageBtn.storageTrans)
                {
                    if (trans.transform.childCount == 1)
                    {
                        item.transform.GetComponent<Canvas>().overrideSorting = false;
                        item.transform.SetParent(trans);
                        item.transform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                        item.transform.GetComponent<RectTransform>().localScale = Vector3.one;
                        break;
                    }
                }
            }
        }
        Debug.Log("머지 에너지충전");
        SetEnergy();
    }

    private void SetEnergy()
    {
        int spawnerNum = 0;
        int timeNum = 0;

        for (int i = 0; i < itemField.childCount; i++)
        {
            Item item = null;

            if(itemField.GetChild(i).GetComponent<Item>())
               item =  itemField.GetChild(i).GetComponent<Item>();

            if (item && item.item.itemType == ItemType.Spawner)
            {
                item.productCount = data.spawnerEnergy[spawnerNum++];

                if ((int)item.item.itemLine < 13)
                {
                    item.chargeTime = data.productTime[timeNum++];

                    if (item.productCount < ProductLimit(item))
                    {
                        if (item.productCount + MergeDataManager.instance.chargeEnergy > ProductLimit(item))
                        {
                            item.productCount = ProductLimit(item);
                            item.chargeTime = 120;
                        }
                        else
                        {
                            item.productCount += MergeDataManager.instance.chargeEnergy;
                            item.chargeTime -= MergeDataManager.instance.remainTime;

                            if (item.chargeTime < 0)
                            {
                                Debug.Log(item.chargeTime);
                                item.chargeTime = 120;

                                if (item.productCount + 1 < ProductLimit(item))
                                    item.productCount++;
                            }
                        }
                    }
                    StartCoroutine(item.CountEnergyTime());
                }
            }
            else if(item && item.isBubble)
            {
                item.productCount -= MergeDataManager.instance.chargeEnergy * 60 + MergeDataManager.instance.remainTime;

                if (item.productCount < 0)
                    item.productCount = 0;
            }
        }

        foreach (Transform trans in storageBtn.storageTrans)
        {
            if (trans.childCount > 1 && trans.GetChild(1).GetComponent<Item>())
            {
                Item item = trans.GetChild(1).GetComponent<Item>();

                if (item.item.itemType == ItemType.Spawner)
                {
                    item.productCount = data.spawnerEnergy[spawnerNum++];

                    if ((int)item.item.itemLine < 13)
                    {
                        item.chargeTime = data.productTime[timeNum++];

                        if (item.productCount < ProductLimit(item))
                        {
                            if (item.productCount + MergeDataManager.instance.chargeEnergy > ProductLimit(item))
                            {
                                item.productCount = ProductLimit(item);
                                item.chargeTime = 120;
                            }
                            else
                            {
                                item.productCount += MergeDataManager.instance.chargeEnergy;
                                item.chargeTime -= MergeDataManager.instance.remainTime;

                                if (item.chargeTime < 0)
                                {
                                    Debug.Log(item.chargeTime);
                                    item.chargeTime = 120;

                                    if (item.productCount + 1 < ProductLimit(item))
                                        item.productCount++;
                                }
                            }
                        }
                        StartCoroutine(item.CountEnergyTime());
                    }
                }
            }
        }

        // 기존에 에너지 충전 까지 남은시간 
        int time1 = EarningManager.Instance.time;
        //  remainTime 
        int time2 = MergeDataManager.instance.remainTime;
        // 기존 남은시간 - remainTime
        int time3 = time1 - time2;


        if (EarningManager.Instance.energy < 100 && (EarningManager.Instance.energy + MergeDataManager.instance.chargeEnergy >= 100))
        {
            Debug.Log("에너지충전1");
            EarningManager.Instance.energy = 100;
            EarningManager.Instance.time = 0;
        }
        else if (EarningManager.Instance.energy < 100 && EarningManager.Instance.energy + MergeDataManager.instance.chargeEnergy <= 100)
        {
            EarningManager.Instance.energy += MergeDataManager.instance.chargeEnergy;
            EarningManager.Instance.time -= MergeDataManager.instance.remainTime;

            Debug.Log("에너지충전2");
            if (EarningManager.Instance.time < 0)
            {
                EarningManager.Instance.energy += 1;
                EarningManager.Instance.time = 0;
                EarningManager.Instance.time += time3;
            }
        }
        else
        {
            Debug.Log("에너지충전3");
            EarningManager.Instance.time = 0;
        }



        MergeDataManager.instance.chargeEnergy = 0;
        MergeDataManager.instance.remainTime = 0;
    }

    public void SaveSpeEng()
    {
        data.productTime.Clear();
        data.spawnerEnergy.Clear();
        data.fieldItem.Clear();
        data.storageItem.Clear();
        data.bubbles.Clear();

        foreach (Tile tile in normal.tiles)
        {
            if(tile.item)
            {
                double itemData = MergeDataManager.instance.ItemChangeData(tile.item.item);

                if (tile.item.inActive)
                    itemData *= -1;

                data.fieldItem.Add(tile.index, itemData);

                if(tile.item.item.itemType == ItemType.Spawner)
                {
                    data.spawnerEnergy.Add(tile.item.productCount);

                    if ((int)tile.item.item.itemLine < 13)
                        data.productTime.Add(tile.item.chargeTime);
                }

                // 버블 저장
                if (tile.item.isBubble)
                    data.bubbles.Add(tile.index, tile.item.productCount);
            }
        }

        foreach(Transform trans in storageBtn.storageTrans)
        {
            if (trans.childCount == 2 && trans.GetChild(1).GetComponent<Item>())
            {
                Item storageItem = trans.GetChild(1).transform.GetComponent<Item>();
                data.storageItem.Add(MergeDataManager.instance.ItemChangeData(storageItem.item));

                if (storageItem.item.itemType == ItemType.Spawner)
                {
                    data.spawnerEnergy.Add(storageItem.productCount);

                    if ((int)storageItem.item.itemLine < 13)
                        data.productTime.Add(storageItem.chargeTime);
                }
            }
        }
    }

    public void SetCover()
    {
        foreach(Animalspecialization spe in lines)
        {
            switch(spe)
            {
                case Animalspecialization.Smithy:
                    foreach(Tile tile in normal.smithyTiles)
                    {
                        tile.seal = false;
                    }
                    covers[0].SetActive(false);
                    break;
                case Animalspecialization.Library:
                    foreach (Tile tile in normal.libraryTiles)
                    {
                        tile.seal = false;
                    }
                    covers[1].SetActive(false);
                    break;
                case Animalspecialization.Gallery:
                    foreach (Tile tile in normal.galleryTiles)
                    {
                        tile.seal = false;
                    }
                    covers[2].SetActive(false);
                    break;
                case Animalspecialization.Carpentry:
                    foreach (Tile tile in normal.carpentryTiles)
                    {
                        tile.seal = false;
                    }
                    covers[3].SetActive(false);
                    break;
            }
        }
    }

    public IEnumerator SetReadyItem()
    {
        if (readyItems.Count == 0)
        {
            normal.readyItemField.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f);
            normal.readyItemText.text = string.Empty;
            yield return new WaitForSeconds(0.5f);
            normal.readyItemField.gameObject.SetActive(false);
        }
        else
        {
            if (readyItems.Count == 1)
            {
                normal.leftItem.gameObject.SetActive(false);
            }

            if (normal.readyItemField.transform.localScale != Vector3.one)
                normal.readyItemField.transform.localScale = Vector3.one;

            normal.leftItemText.text = (readyItems.Count).ToString();
            normal.readyItemField.gameObject.SetActive(true);
            Item item = Instantiate(spawnItem, normal.readyItemTrans);
            item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            item.StartSetItem(readyItems[0], null);
            normal.readyItemText.text = readyItems[0].itemName;
            readyItems.RemoveAt(0);
            item.isReady = true;
            item.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            item.GetComponent<CircleCollider2D>().enabled = false;

            if (!data.dicItems.Contains(MergeDataManager.instance.ItemChangeData(item.item)))
                data.dicItems.Add(MergeDataManager.instance.ItemChangeData(item.item));

            Vector2 size;

            if (item.item.itemType != ItemType.Spawner)
                size = new Vector2(140, 140);
            else
                size = new Vector2(160, 160);

            item.GetComponent<RectTransform>().DOSizeDelta(size, 0.5f);
            yield return new WaitForSeconds(0.5f);
            item.GetComponent<CircleCollider2D>().enabled = true;

            if (item.item.itemType == ItemType.Spawner)
            {
                item.productCount = ProductLimit(item);

                if ((int)item.item.itemLine < 13)
                    item.chargeTime = 120;

                if (tutorial.tutoPro < 29)
                    item.productCount = 100;

                if ((int)item.item.itemLine < 13)
                    StartCoroutine(item.CountEnergyTime());
            }
        }
    }

    // 타일 번호 세팅
    private void SetTilesIndex()
    {
        for (int i = 0; i < normal.tiles.Count; i++)
        {
            normal.tiles[i].index = i;
        }
    }

    private Tile SearchTileInIndex(int num)
    {

        for (int i = 0; i < normal.tiles.Count; i++)
        {
            if (normal.tiles[i].index == num)
            {
                return normal.tiles[i];
            }
        }

        return null;
    }

    // 가까운 타일 찾기
    public Tile GetNearTile(Vector2 pos)
    {
        Tile result = null;

        foreach (Tile tile in normal.tiles)
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

    // 가깝고 비어있는 타일 찾기
    public Tile GetNearAndEmptyTile(Vector2 pos)
    {
        Tile result = null;

        foreach (Tile tile in normal.tiles)
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
    public void SetSelect(Item item, bool discriptionTextClear = true)
    {
        if (item &&item.isBoxing)
            return;

        Discription(item, discriptionTextClear);

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

    // 아이템 스폰
    public bool SpawnItem(Vector2 pos, Animalspecialization itemLine, int spawnerLevel, MergeItem selectItem = null)
    {
        Tile tile = GetNearAndEmptyTile(pos);

        if (tile == null)
        {
            Warning("보드가 꽉 찼습니다.");
            return false;
        }
        MasterAudio.PlaySound("아이템생성");

        MergeItem mergeItem = null;

        if (!selectItem)
            mergeItem = MergeDataManager.instance.GetRandomStartItem(itemLine, spawnerLevel);
        else
            mergeItem = MergeDataManager.instance.SearchMergeItem(selectItem.itemLine, selectItem.itemType, selectItem.Level);

        // 튜토 진행 중에는 당근만 생산
        if(tutorial.tutoPro == 5)
            mergeItem = MergeDataManager.instance.SearchMergeItem(Animalspecialization.Vegetable, ItemType.Item_B, 1);

        for (int i = 0; i < itemField.childCount; i++)
        {
            if (!itemField.GetChild(i).gameObject.activeSelf &&
                itemField.GetChild(i).CompareTag("MergeItem"))
            {
                itemField.GetChild(i).GetComponent<RectTransform>().anchoredPosition = pos;
                itemField.GetChild(i).GetComponent<Item>().SetItem(mergeItem, tile);

                return true;
            }
        }

        Item item = Instantiate(spawnItem, itemField);
        item.GetComponent<RectTransform>().anchoredPosition = pos;
        item.SetItem(mergeItem, tile);

        return true;
    }

    public void SpawnBubble(Vector2 pos , MergeItem item)
    {
        Tile tile = GetNearAndEmptyTile(pos);

        if (tile == null)
        {
            Warning("보드가 꽉 찼습니다.");
            return;
        }
        Debug.Log("Bubble");

        MergeItem mergeItem = item;

        for (int i = 0; i < itemField.childCount; i++)
        {
            if (!itemField.GetChild(i).gameObject.activeSelf &&
                itemField.GetChild(i).GetComponent<Item>())
            {
                itemField.GetChild(i).GetComponent<RectTransform>().anchoredPosition = pos;
                itemField.GetChild(i).GetComponent<Item>().SetBubble();
                itemField.GetChild(i).GetComponent<Item>().SetItem(mergeItem, tile);
                return ;
            }
        }

        Item spawnItem = Instantiate(this.spawnItem, itemField);
        spawnItem.GetComponent<RectTransform>().anchoredPosition = pos;
        spawnItem.SetBubble();
        spawnItem.SetItem(mergeItem, tile);
    }

    public bool SpawnSpecial(Vector2 pos, Animalspecialization itemLine, int spawnerLevel)
    {
        Tile tile = GetNearAndEmptyTile(pos);

        if (tile == null)
        {
            Warning("보드가 꽉 찼습니다.");
            return false;
        }

         MasterAudio.PlaySound("아이템생성");

        MergeItem mergeItem = null;

        mergeItem = MergeDataManager.instance.GetRandomSpecialItem(itemLine, spawnerLevel);

        for (int i = 0; i < itemField.childCount; i++)
        {
            if (!itemField.GetChild(i).gameObject.activeSelf &&
                itemField.GetChild(i).CompareTag("MergeItem"))
            {
                itemField.GetChild(i).GetComponent<RectTransform>().anchoredPosition = pos;
                itemField.GetChild(i).GetComponent<Item>().SetItem(mergeItem, tile);

                return true;
            }
        }

        Item item = Instantiate(spawnItem, itemField);
        item.GetComponent<RectTransform>().anchoredPosition = pos;
        item.SetItem(mergeItem, tile);

        return true;
    }

    // 랜던 아이템 리스트
    public List<MergeItem> GetRandItems()
    {
        List<MergeItem> list = new List<MergeItem>();

        int num = UnityEngine.Random.Range(1, 3);

        while (true)
        {
            if (list.Count == num)
                break;

            MergeItem item = MergeDataManager.instance.RandomItem(lines[UnityEngine.Random.Range(0, lines.Count)]);

            bool pass = true;

            //리스트 안에 이미 같은 아이템이 있는지 확인
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

    // 미션에 필요한 필드 아이템 체크
    public void NeedCheck(bool check = false)
    {
        for(int i = 0; i<itemField.childCount; i++)
        {
            if(itemField.GetChild(i).GetComponent<Item>())
                itemField.GetChild(i).GetComponent<Item>().checkImg.gameObject.SetActive(false);
        }

        if (npcMissionInfo.transform.parent.gameObject.activeSelf)
        {
            for (int i = 0; i < npcMissionInfo.needItems.Count; i++)
            {
                if (SearchFieldItem(npcMissionInfo.needItems[i]) != null)
                {
                    if (!npcMissionInfo.needItemIcons[i].need)
                    {
                        if (!npcMissionInfo.clear)
                        {
                            npcMissionInfo.needItemIcons[i].NeedOn();
                        }

                        npcMissionInfo.needCount++;

                        if (npcMissionInfo.needCount == npcMissionInfo.needItems.Count)
                        {
                            npcMissionInfo.ClearMove();
                            MasterAudio.PlaySound("동전수집");
                        }
                    }
                }
                else
                {
                    if (npcMissionInfo.needItemIcons[i].need)
                    {
                        if (!npcMissionInfo.clear)
                            npcMissionInfo.needItemIcons[i].NeedOff();

                        npcMissionInfo.needCount--;

                        if (npcMissionInfo.needCount != npcMissionInfo.needItems.Count)
                        {
                            npcMissionInfo.StopMove();
                        }
                    }
                }

                npcMissionInfo.needItemIcons[i].SetNeedCount(SearchFieldItemCount(npcMissionInfo.needItems[i]));
            }
        }

        foreach (MergeNeedInfo info in normal.needInfoes)
        {
            for (int i = 0; i < info.needItems.Count; i++)
            {
                if (SearchFieldItem(info.needItems[i]) != null)
                {
                    if (!info.needItemIcons[i].need)
                    {
                        if(!info.clear)
                        {
                            info.needItemIcons[i].NeedOn();
                        }

                        info.needCount++;

                        if (info.needCount == info.needItems.Count)
                        {
                            info.ClearMove();
                            MasterAudio.PlaySound("동전수집");

                            tutorial.SetTutorial(8);
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
                            info.StopMove();
                        }
                    }
                }

                info.needItemIcons[i].SetNeedCount(SearchFieldItemCount(info.needItems[i]));
            }
        }

        if(!check)
            SortInfo();

        hintTime = 0;
        hinthint = 0;
        hintHand.gameObject.SetActive(false);

        foreach (Store store in MergeDataManager.instance.storeList)
        {
            if (store.BuyAndStepUpCost() <= EarningManager.Instance.mergeCoin)
            {
                oldCat.Pop();
                break;
            }
        }
    }

    private void SortInfo()
    {

        for(int i = 0; i< normal.needInfoes.Count; i++)
        {
            if (normal.needInfoes[i].needItems.Count == normal.needInfoes[i].needCount && !normal.needInfoes[i].clear)
            {
                for (int j = 0; j < i; j++)
                {
                    if (normal.needInfoes[j].needItems.Count != normal.needInfoes[j].needCount && !normal.needInfoes[j].clear)
                    {
                        normal.needInfoes.Insert(j, normal.needInfoes[i]);
                        normal.needInfoes.RemoveAt(i+1);

                        break;
                    }
                }
            }
        }

        float posX = 0;
        Vector2 nowPos;

        foreach(MergeNeedInfo needInfo in normal.needInfoes)
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

        if(MissionManager.instance.GetGuideMissions().Count>0)
        {
            posX = 0;
            posX += normal.needInfoes[0].GetComponent<RectTransform>().sizeDelta.x / 2;
            nowPos = normal.needInfoes[0].GetComponent<RectTransform>().anchoredPosition;
            normal.needInfoes[0].GetComponent<RectTransform>().anchoredPosition = nowPos;
            normal.needInfoes[0].GetComponent<RectTransform>().DOAnchorPosX(posX, 0.5f);
            posX += normal.needInfoes[0].GetComponent<RectTransform>().sizeDelta.x / 2;
            posX += 30;
        }


        if (normal.readyItemField.gameObject.activeSelf)
            posX += 545;
        else
            posX += 335;

        if(npcMissionInfo.transform.parent.gameObject.activeSelf)
            posX +=320;

        normal.scrollBack.GetComponent<RectTransform>().sizeDelta = new Vector2(posX, normal.scrollBack.GetComponent<RectTransform>().sizeDelta.y);
    }

    public MergeItem SearchFieldItem(MergeItem item)
    {
        for (int i = 0; i < itemField.childCount; i++)
        {
            if (itemField.GetChild(i).gameObject.activeSelf
                && itemField.GetChild(i).GetComponent<Item>().item == item
                && !itemField.GetChild(i).GetComponent<Item>().inActive
                && itemField.GetChild(i).GetComponent<Item>().nowTile != null
                && !itemField.GetChild(i).GetComponent<Item>().isBubble)
            {
                return itemField.GetChild(i).GetComponent<Item>().item;
            }
        }

        return null;
    }

    // autoMerge���� ����
    public Item SearchFieldItem(Item item)
    {
        for (int i = 0; i < itemField.childCount; i++)
        {
            Item fieldItem = itemField.GetChild(i).GetComponent<Item>();

            if (fieldItem &&fieldItem.gameObject.activeSelf && fieldItem.item == item.item && fieldItem.nowTile != item.nowTile && !fieldItem.inActive && !fieldItem.isBoxing && !fieldItem.isBubble)
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
                && itemField.GetChild(i).GetComponent<Item>().nowTile != null
                && !itemField.GetChild(i).GetComponent<Item>().isBubble)
            {
                num++;
                itemField.GetChild(i).GetComponent<Item>().checkImg.gameObject.SetActive(true);
            }
        }

        return num;
    }

    public void TakeOutStorage(Item item)
    {
        item.transform.SetParent(itemField);
        item.inStorage = false;
        item.transform.SetAsLastSibling();
        item.GetComponent<CircleCollider2D>().enabled = false;
        if(item.item.itemType == ItemType.Spawner)
        item.OnMouseUp();
        NeedCheck();
    }

    public void TakeInStorage(Item item)
    {
        isDrage = false;
        item.isDrag = false;

        if (item.nowTile != null)
        {
            item.nowTile.item = null;
            item.nowTile = null;
        }

        foreach (Transform trans in storageBtn.storageTrans)
        {
            if (trans.transform.childCount == 1)
            {
                item.transform.GetComponent<Canvas>().overrideSorting = false;
                item.transform.SetParent(trans);
                item.rect.anchoredPosition = new Vector2(0, 0);

                if (item.item.itemType == ItemType.Spawner)
                    item.rect.sizeDelta = new Vector2(160, 160);
                else
                    item.rect.sizeDelta = new Vector2(140, 140);

                item.rect.localScale = Vector3.one;

                break;
            }
        }

        storageBtn.StorageClose();
        Discription(null, true);
        NeedCheck();
    }

    // 창고 빈공간 확인
    public bool StorageFullCheck()
    {
        bool result = false;

        foreach(Transform trans in storageBtn.storageTrans)
        {
            if (trans.transform.childCount == 1)
                result = true;
        }

        return result;
    }

    // 머지 미션 보상
    public void GetMissionReward(MergeNeedInfo info)
    {
        MasterAudio.PlaySound("동전수집");
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

                    if (targetItem.item == item && !targetItem.inActive && !targetItem.isBubble)
                    {
                        Vector2 movePos = Camera.main.WorldToScreenPoint(info.needPanel.transform.position);

                        StartCoroutine(targetItem.CollectItem(movePos));

                        if (targetItem.MoveTime(movePos) > time)
                            time = targetItem.MoveTime(targetItem.GetScreenPointToRectangle(movePos));

                        //MissionManager.instance.UpdateMissionProgress(MissionType.MergeClear, 1);

                        break;
                    }
                }
            }
        }

        EarningManager.Instance.AddMedal(info.reward);
        EarningManager.Instance.area.SetEarningParticle(info.rewardFishText.transform, 1);
        EarningManager.Instance.AddMergeCoin((int)info.reward / 7 * 5);
        EarningManager.Instance.area.SetEarningParticle(info.rewardMergeCoinText.transform, 3);

        StartCoroutine(info.ClearMission(time));

        if (tutorial.tutoPro == 8)
            tutorial.NextTuto();
    }

    public void Discription(Item item, bool textClear = true)
    {
        if (item == null)
        {
            if (textClear)
            {
                discriptItem = null;
                itemName.text = string.Empty;
                itemDiscriptionText.text = string.Empty;
                itemLevelText.gameObject.SetActive(false);
                spawnerStamina.gameObject.SetActive(false);
                discriptInfoBtn.gameObject.SetActive(false);
                discriptInfoImg.gameObject.SetActive(false);
            }

            itemSellBtn.gameObject.SetActive(false);
            gemChargeBtn.gameObject.SetActive(false);
            adChargeBtn.gameObject.SetActive(false);
        }
        else
        {
            discriptItem = item;
            itemName.text = item.item.itemName;
            itemDiscriptionText.text = item.item.discription;
            guideItem = item.item;

            if (item.item.itemType == ItemType.Spawner)
            {
                if ((int)item.item.itemLine < 13)
                {
                    spawnerStamina.text = $"{item.productCount}/{ProductLimit(item)}";
                }
                else
                {
                    if(item.item.itemLine == Animalspecialization.MergePay)
                        spawnerStamina.text = $"{item.productCount}/{ProductLimit(item)}";
                    else
                        spawnerStamina.text = $"{item.productCount}/{ProductLimit(item)}";
                }
                spawnerStamina.gameObject.SetActive(true);
            }
            else
                spawnerStamina.gameObject.SetActive(false);

            discriptInfoBtn.gameObject.SetActive(true);
            discriptInfoImg.gameObject.SetActive(true);

            itemSellIcon.sprite = rewardIcons[1];

            switch (item.item.Level)
            {
                case 0:
                    itemSellBtn.gameObject.SetActive(false);
                    return;
                case 1:
                    itemSellText.text = "20";
                    break;
                case 2:
                    itemSellText.text = "50";
                    break;
                case 3:
                    itemSellText.text = "90";
                    break;
                case 4:
                    itemSellText.text = "140";
                    break;
                case 5:
                    itemSellText.text = "200";
                    break;
            }

            if (item.item.itemType == ItemType.Spawner)
            {
                if ((int)item.item.itemLine < 13 && item.productCount == 0)
                {
                    itemSellBtn.gameObject.SetActive(false);
                    gemChargeBtn.gameObject.SetActive(true);
                    adChargeBtn.gameObject.SetActive(true);
                    spawnerStamina.gameObject.SetActive(false);

                    if(item.chargeTime/60>0)
                    {
                        adTimer.text = $"{item.chargeTime / 60:0}:{item.chargeTime % 60:00}";
                        gemTimer.text = $"{item.chargeTime / 60:0}:{item.chargeTime % 60:00}";
                    }
                    else
                    {
                        adTimer.text = $"{item.chargeTime % 60:00}";
                        gemTimer.text = $"{item.chargeTime % 60:00}";
                    }

                    gemChargeCost.text = $"{4+item.item.Level}";

                    itemDiscriptionText.text = "광고를 보거나 젬을 사용해서 충전하세요.";
                }
                else
                {
                    itemSellBtn.gameObject.SetActive(false);
                    gemChargeBtn.gameObject.SetActive(false);
                    adChargeBtn.gameObject.SetActive(false);
                }
            }
            else if (item.inActive || item.item.itemType == ItemType.Goods)
            {
                itemSellBtn.gameObject.SetActive(false);
                gemChargeBtn.gameObject.SetActive(false);
                adChargeBtn.gameObject.SetActive(false);
            }
            else
            {
                itemSellText.gameObject.SetActive(true);
                itemSellBtn.gameObject.SetActive(true);
                gemChargeBtn.gameObject.SetActive(false);
                adChargeBtn.gameObject.SetActive(false);
            }

            if(item.isBubble)
            {
                gemTimer.text = $"{item.productCount % 60:00}";
                gemChargeCost.text = $"{discriptItem.item.Level * 5}";
                itemSellBtn.gameObject.SetActive(false);
                gemChargeBtn.gameObject.SetActive(true);
                adChargeBtn.gameObject.SetActive(false);

                itemDiscriptionText.text = "젬을 사용해서 획득하세요.";
            }

            itemLevelText.gameObject.SetActive(true);
            itemLevelText.text = $"Lv.{item.item.Level}";

            if (discriptItem.inActive)
                Warning("잠겨있음");
        }
    }

    public void SellItem()
    {
        Item item = discriptItem;

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
        MasterAudio.PlaySound("클릭");
    }

    public void SellItem(Item item)
    {
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

    public void GemCharge()
    {
        int costGem = discriptItem.item.Level+4;

        if (discriptItem.isBubble)
        {
            if (EarningManager.Instance.gem < discriptItem.item.Level * 5)
            {
                Warning("젬이 부족합니다.");
                OpenMergeShop(true);
                return;
            }

            EarningManager.Instance.RemoveGem(discriptItem.item.Level * 5);
            StartCoroutine(discriptItem.PopBubble());
            Discription(discriptItem);
        }
        else
        {
            if (EarningManager.Instance.gem < costGem)
            {
                Warning("젬이 부족합니다.");
                OpenMergeShop(true);
                return;
            }
            else
            {
                EarningManager.Instance.RemoveGem(costGem);
                ChargeProductCount();
            }
        }
    }

    public void ChargeProductCount()
    {
        discriptItem.productCount = ProductLimit(discriptItem);
        discriptItem.chargeTime = 120;
        SetSelect(discriptItem);
        chargeEffect.SetChargeEffect(discriptItem.transform);
    }

    //  마을 씬으로 돌아가기
    public void ReturnVillage(bool storeCheck = false)
    {
        if (moveScene)
            return;

        moveScene = true;

        if (storeCheck)
            MergeDataManager.instance.store = store;

        SceneController.instance.MoveStageScene();
    }

    // 합성 가능 아이템 위에 있을 때 실행되는 파티클
    public void SetMergeParticle(Item item)
    {
        if (item && item.isBoxing)
            return;

        mergeParticle.transform.localScale = new Vector3(120, 120, 120);

        if (item == null)
        {
            mergeParticle.gameObject.SetActive(false);
        }
        else
        {
            if (MergeDataManager.instance.UpgradeItem(item.item) == null)
                return;

            if (Vector3.Distance(mergeParticle.transform.parent.transform.position,Camera.main.ScreenToWorldPoint(Input.mousePosition)) < Vector3.Distance(item.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)))
                return;

            item.transform.SetSiblingIndex(itemField.childCount - 2);
            mergeParticle.transform.SetParent(item.transform);
            mergeParticle.gameObject.SetActive(true);
        }

        mergeParticle.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

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

    public void UIOpen()
    {
        uiOpen = true;

        hintHand.gameObject.SetActive(false);

        PlayMergeEndParticle(null);
        
        for(int i = 0; i<itemField.childCount; i++)
        {
            if(itemField.GetChild(i).GetComponent<CircleCollider2D>())
            {
                itemField.GetChild(i).GetComponent<CircleCollider2D>().enabled = false;
            }
        }
    }

    public void UIClose()
    {
        uiOpen = false;

        if(guide.gameObject.activeSelf)
            uiOpen = true; 

        for (int i = 0; i < itemField.childCount; i++)
        {
            if (itemField.GetChild(i).GetComponent<CircleCollider2D>())
            {
                itemField.GetChild(i).GetComponent<CircleCollider2D>().enabled = true;
            }
        }
    }

    // 경고 메시지
    public void Warning(string msg)
    {
        for (int i = 0; i < warningArea.childCount; i++)
        {
            if (!warningArea.GetChild(i).gameObject.activeSelf)
            {
                warningArea.GetChild(i).GetComponent<TextMeshProUGUI>().text = msg;
                warningArea.GetChild(i).gameObject.SetActive(true);

                return;
            }
        }

        Warning warn = Instantiate(warning, warningArea);

        warn.GetComponent<TextMeshProUGUI>().text = msg;
    }

    // 아이템을 스폰 가능한 상태인지 확인
    public bool SpawnAbleCheck()
    {
        int num = 0;

        for(int i = 0; i<itemField.childCount; i++)
        {
            Item item = itemField.GetChild(i).GetComponent<Item>();

            if (item && item.gameObject.activeSelf && item.nowTile != null)
                num++;
        }

        return num < 63;
    }

    public void SetGuide(MergeItem item)
    {
        guide.SetGuide(item);
    }

    public void MissionSave()
    {
        data.mergeMission.Clear();

        foreach (MergeNeedInfo info in normal.needInfoes)
        {
            foreach(MergeItem item in info.needItems)
            {
                data.mergeMission.Add(MergeDataManager.instance.ItemChangeData(item));
            }

            data.mergeMission.Add(-1);
        }
    }

    public IEnumerator HintCo()
    {
        while(true)
        {
            if(hintTime > 1)
            {
                Hint();
                hintTime = 0;
            }
            yield return new WaitForSeconds(1);

            if (!isDrage)
                hintTime++;
            else
            {
                hintTime = 0;
                hintHand.gameObject.SetActive(false);
            }
        }
    }

    private void Hint()
    {
        if (isDrage)
        {
            hintHand.gameObject.SetActive(false);
            return;
        }

        Item item1 = null;
        Item item2 = null;

        for(int i = 0; i< itemField.childCount; i++)
        {
            if(itemField.GetChild(i).gameObject.activeSelf && itemField.GetChild(i).GetComponent<Item>() && !itemField.GetChild(i).GetComponent<Item>().inActive && MergeDataManager.instance.UpgradeItem(itemField.GetChild(i).GetComponent<Item>().item) && !itemField.GetChild(i).GetComponent<Item>().isBubble)
                item1 = itemField.GetChild(i).GetComponent<Item>();

            if(item1)
            {
                for(int j = 0; j < itemField.childCount; j++)
                    if(itemField.GetChild(j).gameObject.activeSelf && itemField.GetChild(j).GetComponent<Item>().item == item1.item && itemField.GetChild(j).GetComponent<Item>().nowTile != item1.nowTile && !itemField.GetChild(j).GetComponent<Item>().isBoxing && !itemField.GetChild(j).GetComponent<Item>().isBubble)
                    {
                        item2 = itemField.GetChild(j).GetComponent<Item>();
                        break;
                    }
            }

            if(item2)
            {
                StartCoroutine(item1.HintMove(item2.rect.anchoredPosition));
                StartCoroutine(item2.HintMove(item1.rect.anchoredPosition));

                hinthint++;

                if (hinthint > 1)
                {
                    Vector3 startPos = new Vector3(item1.transform.position.x, item1.transform.position.y - 0.2f, item1.transform.position.z);
                    Vector3 targetPos = new Vector3(item2.transform.position.x, item2.transform.position.y - 0.2f, item2.transform.position.z);

                    if (!uiOpen && tutorial.tutoPro > 12)
                    {
                        hintHand.gameObject.SetActive(true);
                        hintHand.AnimationState.TimeScale = 0;

                        hintHand.transform.position = startPos;
                        hintHand.transform.DOMove(targetPos, 1f);
                    }
                }
                return;
            }
        }

        // 스폰 자리가 없을 시
        if (!SpawnAbleCheck())
        {
            for (int i = normal.tiles.Count - 1; i >= 0; i--)
            {
                if (!uiOpen && tutorial.tutoPro > 12 && normal.tiles[i].item && !normal.tiles[i].item.inActive && normal.tiles[i].item.item.itemType != ItemType.Spawner && !normal.tiles[i].item.isBubble)
                {
                    // storageMsg.SetActive(true);
                    hintHand.transform.position = normal.tiles[i].item.transform.position;
                    hintHand.gameObject.SetActive(true);
                    hintHand.AnimationState.TimeScale = 0;

                    hintHand.transform.DOMove(storageBtn.transform.position, 1f);
                    return;
                }
            }
        }
        else
        {
            for (int i = 0; i < itemField.childCount; i++)
            {
                Item item = null;
                if (itemField.GetChild(i).GetComponent<Item>())
                    item = itemField.GetChild(i).GetComponent<Item>();

                if (itemField.GetChild(i).gameObject.activeSelf &&
                   item.item.itemType == ItemType.Spawner &&
                   !item.inActive && item.productCount > 0)
                {
                    StartCoroutine(itemField.GetChild(i).GetComponent<Item>().HintMove());

                    hinthint++;

                    if (hinthint > 1)
                    {
                        Vector3 targetPos = new Vector3(itemField.GetChild(i).transform.position.x, itemField.GetChild(i).transform.position.y - 0.2f, itemField.GetChild(i).transform.position.z);

                        if (!uiOpen && tutorial.tutoPro > 12)
                        {
                            hintHand.gameObject.SetActive(true);
                            hintHand.transform.position = targetPos;
                            if (hintHand.AnimationState.TimeScale == 0)
                            {
                                hintHand.Initialize(true);
                                hintHand.AnimationState.SetAnimation(1, "Click", true);
                            }
                        }
                    }
                    return;
                }
            }
        }
    }

    public void SetEnergySpine(Item item)
    {
        SkeletonGraphic spine = Instantiate(energySpine, item.transform);
        spine.Initialize(true);
        spine.AnimationState.SetAnimation(1, "animation", true);
        item.energySpine = spine;
    }

    public bool SelectCheck(Item item)
    {
        if (select.transform.parent.GetComponent<Tile>() &&
            select.transform.parent.GetComponent<Tile>().item == item)
            return true;
        else
            return false;
    }

    private void OnApplicationQuit()
    {
        Item item = null;

        // 상자에서 꺼낸 드래그 중인 아이템
        for (int i = 0; i < itemField.childCount; i++)
        {
            if (itemField.GetChild(i).GetComponent<Item>())
            {
                item = itemField.GetChild(i).GetComponent<Item>();

                if (item.gameObject.activeSelf && item.isDrag && !item.nowTile)
                    TakeInStorage(item);
            }
        }

        SaveSpeEng();
        UserDataManager.instance.UpdateUserData_MergeData();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            Item item = null;

            // 상자에서 꺼낸 드래그 중인 아이템
            for (int i = 0; i < itemField.childCount; i++)
            {
                if (itemField.GetChild(i).GetComponent<Item>())
                {
                    item = itemField.GetChild(i).GetComponent<Item>();

                    if (item.gameObject.activeSelf && item.isDrag && !item.nowTile)
                        TakeInStorage(item);
                }
            }
            SaveSpeEng();
            UserDataManager.instance.UpdateUserData_MergeData();
        }
        else
        {
            TimeSpan duration = UserDataManager.instance.NowDay(true) - UserDataManager.instance.GetUserData().lastTime;

            MergeDataManager.instance.chargeEnergy = (int)(duration.TotalSeconds / 120);
            MergeDataManager.instance.remainTime = (int)(duration.TotalSeconds % 120);

            SetEnergy();
        }
    }

    //   private void OnApplicationPause(bool pause)
    // {

    //     if (pause)
    //     {
    //         bPaused = true;
    //         UserDataManager.instance.GetUserData().lastTime = UserDataManager.instance.NowDay();
    //     }
    //     else
    //     {

    //         if (bPaused)
    //         {
    //             bPaused = false;
    //             TimeSpan duration = UserDataManager.instance.NowDay() - UserDataManager.instance.GetUserData().lastTime;

    //             MergeDataManager.instance.chargeEnergy = (int)(duration.TotalSeconds / 120);
    //             MergeDataManager.instance.remainTime = (int)(duration.TotalSeconds % 120);

    //             SetEnergy();

    //         }
    //     }

    // }


    private void OnApplicationPause(bool pasue)
    {
        Debug.Log("퍼즈"+pasue );
    }

    public void OpenRefiilPop()
    {
        if (data.leftOverFree[1] == 0)
        {
            daliyFreeGemBtn.gameObject.SetActive(false);
            adGemBtn.gameObject.SetActive(true);
        }
        else
        {
            daliyFreeGemBtn.gameObject.SetActive(true);
            adGemBtn.gameObject.SetActive(false);
        }

        adGemText.text = $"잔여 {data.leftOverAd[1]}회";
        adEnergyText.text = $"잔여 {data.leftOverAd[2]}회";
        chargeCost.text = $"{data.energyCharge*5+5}";
        refillPop.SetActive(true);
        UserDataManager.instance.SendLog("RifillOpen");
        UIOpen();
    }

    public void RefillBtnClickEvent(int num)
    {
        if(mergeShop.gameObject.activeInHierarchy)
        {
            if (num == 0)
            {
                mergeShop.BuyGem(2);
                return;
            }
            if (num == 1)
            {
                mergeShop.BuyEnerge(2);
                return;
            }
        }

        switch(num)
        {
            case 0:
                EarningManager.Instance.AddGem(9);
                EarningManager.Instance.area.SetEarningParticle(refillPop.transform, 2);
                break;
            case 1:
                EarningManager.Instance.AddEnergy(30);
                EarningManager.Instance.area.SetEarningParticle(refillPop.transform, 4);
                break;
            case 2:
                if (EarningManager.Instance.gem < data.energyCharge * 5 + 5)
                    break;
                EarningManager.Instance.RemoveGem(data.energyCharge * 5 + 5);
                data.energyCharge++;
                EarningManager.Instance.AddEnergy(100);
                EarningManager.Instance.area.SetEarningParticle(refillPop.transform, 4);
                break;
        }

        refillPop.SetActive(false);
        UIClose();
    }

    public void DaliyFreeGemClick()
    {
        if (data.leftOverFree[1] == 0)
            return;
        else
        {
            data.leftOverFree[1]--;
            EarningManager.Instance.AddGem(9);
            EarningManager.Instance.area.SetEarningParticle(refillPop.transform, 4);
            daliyFreeGemBtn.gameObject.SetActive(false);
            adGemBtn.gameObject.SetActive(true);

            refillPop.SetActive(false);
            UIClose();
        }
    }

    public void DiscriptInfo()
    {
        SetGuide(guideItem);
    }

    public void WatchAd(int num)
    {
        if (data.leftOverAd[num] == 0)
        {
            Warning("오늘은 광고를 다 보셨네요.");
            return;
        }

        AdManager.instance.LoadRewardedAd(num);
        loading.SetActive(true);
        UIOpen();
    }

    public void CloseAd()
    {
        loading.SetActive(false);
        UIClose();
    }

    private IEnumerator mergeTimeCheck()
    {
        MergeDataManager.instance.mergeTime = 0;

        while (true)
        {
            MergeDataManager.instance.mergeTime++;
            yield return new WaitForSeconds(1);
        }
    }

    // 미션 클리어 알람
    public void MissionComplete(Mission mission)
    {
        foreach(Mission clearMission in clearMissions)
        {
            if (clearMission.type == mission.type)
                return;
        }

        foreach(Mission readyMission in readyMissions)
        {
            if (readyMission.type == mission.type)
                return;
        }

        if (missionCard.isplay)
        {
            readyMissions.Add(mission);
            return;
        }

        clearMissions.Add(mission);

        switch (mission.type)
        {
            //case MissionType.MergeClear:
              //  missionCard.missionText.text = $"주민 의뢰 해결";
                //break;
            case MissionType.MergeSpawn:
                missionCard.missionText.text = $"아이템 생성";
                break;
            case MissionType.MergeCombine:
                missionCard.missionText.text = $"아이템 합치기";
                break;
        }

        missionCard.sliderText.text = $"{mission.progress}/{mission.goal}";
        StartCoroutine(missionCardMove());
    }

    private IEnumerator missionCardMove()
    {
        RectTransform rect = missionCard.GetComponent<RectTransform>();

        missionCard.isplay = true;
        rect.DOAnchorPosX(50, 0.5f);
        yield return new WaitForSeconds(5);
        rect.DOAnchorPosX(-390, 0.5f);
        yield return new WaitForSeconds(0.5f);
        missionCard.isplay = false;

        if (readyMissions.Count > 0)
        {
            MissionComplete(readyMissions[0]);
            readyMissions.RemoveAt(0);
        }
    }

    private void SetNPCMission()
    {
        if (data.npcMission.Count == 0)
            npcMissionInfo.transform.parent.gameObject.SetActive(false);
        else
        {
            List<MergeItem> items = new List<MergeItem>();

            foreach(int itemData in data.npcMission)
                items.Add(MergeDataManager.instance.DataChangeItem(itemData));

            npcMissionInfo.transform.parent.gameObject.SetActive(true);
            npcMissionInfo.SetNPCMissionInfo(items);
        }
    }

    public void GiveUpNpcMission()
    {
        StartCoroutine(npcMissionInfo.GiveUpNpcMission());
    }

    // npcMission 시간 설정
    public IEnumerator SetNPCMissionTimer()
    {
        DateTime time = data.npcMissionLimit;
        DateTime nowTime = UserDataManager.instance.NowDay();
        startTime = nowTime;

        if (nowTime > time)
        {
            data.npcMissionLimit = new DateTime();
            data.npcMission.Clear();
            SetNPCMission();
            yield return null;
        }
        else
        {
            // 서버 시간을 과도하게 불러오는 것을 방지하기 위한 반복문
            while (true)
            {
                nowTime = nowTime.AddSeconds(1);

                TimeSpan duration = time - nowTime;
                npcMissionInfo.rewardMergeCoinText.text = $"{duration.Hours:00}:{duration.Minutes:00}:{duration.Seconds:00}";

                if (time == nowTime)
                {
                    data.npcMission.Clear();
                    data.npcMissionLimit = new DateTime();
                    npcMissionInfo.transform.parent.gameObject.SetActive(false);
                }

                yield return new WaitForSeconds(1);
            }
        }
    }

    private IEnumerator SetMergeShopTime()
    {
        DateTime time = UserDataManager.instance.NowDay();

        DateTime nextDay = time.AddDays(1);
        nextDay = nextDay.AddHours(-nextDay.Hour);
        nextDay = nextDay.AddMinutes(-nextDay.Minute);
        nextDay = nextDay.AddSeconds(-nextDay.Second);

        while (true)
        {
            TimeSpan timeSpan = nextDay - time;

            mergeShop.SetTime(timeSpan);

            time = time.AddSeconds(1);
            yield return new WaitForSeconds(1);
        }
    }

    public void GetRewardNPCMission(MergeNeedInfo info)
    {
        MasterAudio.PlaySound("클릭");

        MergeDataManager.instance.readyClearMission = true;
        SceneController.instance.MoveStageScene();
    }

    public void OpenWantMissionPanel()
    {
        List<MergeItem> list = new List<MergeItem>();

        foreach(int itemData in data.npcMission)
        {
            list.Add(MergeDataManager.instance.DataChangeItem(itemData));
        }

        wantMissionPanel.SetWantMission(MergeDataManager.instance.npcMissionStore, list, data.npcSkin);
        wantMissionPanel.transform.parent.gameObject.SetActive(true);
    }

    public void OpenCraftPanel()
    {
        craftListPanel.SetCraftListPanel(MergeDataManager.instance.storeList);
        craftListPanel.transform.parent.gameObject.SetActive(true);
        UIOpen();
    }

    public int ProductLimit(Item item)
    {
        int result = 0;

        if ((int)item.item.itemLine > 12)
        {
            result = 5 * item.item.Level + 5;

            if (item.item.itemLine == Animalspecialization.MergePay)
                result += 5;
        }
        else if ((int)item.item.itemLine < 13)
            result = 5 * item.item.Level + 15;

        return result;
    }

    public void OpenNewSpawnerUpPop(MergeItem item)
    {
        spawnerUpPop.SetNewSpawnerPop(item);
    }

    public void BuyMergeShopAd(int num)
    {
        mergeShop.BuyMergeItem(num);
        CloseAd();
    }

    public void OpenMergeShop(bool gem = false)
    {
        if (gem)
            mergeShop.content.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 3602, 0);
        else
            mergeShop.content.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

        mergeShop.transform.parent.gameObject.SetActive(true);
        UIOpen();
    }

    public void CloseStorage()
    {
        storage.gameObject.SetActive(false);
        UIClose();
    }
}