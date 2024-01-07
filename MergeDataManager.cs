using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeDataManager : MonoBehaviour
{
	public static MergeDataManager instance;

    public List<MergeItem> items;
    public List<MergeItem> spawners;

    public int mergeLevel;
	public int mergeTime;

    [Header("MergeNeedData")]
	public Store store;
    public List<Store> storeList;

    [Header("NPCMission")]
    public Store npcMissionStore;
    public bool readyClearMission;

    [Header("LastCameraPos")]
    public Vector3 camPos;
    public float proJecSize;
    public bool camSave;

    [Header("Charge")]
    public int chargeEnergy;
    public int remainTime;

    [Header("Product/Min")]
    public double productAmount;



    private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
			Destroy(gameObject);


        StartCoroutine(ConnectionTime());
	}

    public void SetMergeLevel()
    {
        mergeLevel = 0;

        foreach (StoreController controller in GameManager.instance.GetStores())
        {
            mergeLevel += controller.GetStore().step;
        }
    }

    public double ItemChangeData(MergeItem item, bool inActive = false)
    {
        double num = 0;

        num += (int)item.itemLine * 10000;
        num += (int)item.itemType * 100;
        num += item.Level;

        // ��Ȱ��ȭ ������
        if (inActive)
            num *= -1;

        return num;
    }

    // ��Ȱ��ȭ �������� ���� üũ �ؾ���
    public MergeItem DataChangeItem(double data)
    {
        MergeItem mergeItem = null;

        double num = data;
        Animalspecialization line = (Animalspecialization)(num / 10000);
        num %= 10000;
        ItemType type = (ItemType)(num / 100);
        num %= 100;
        int level = (int)num;

        mergeItem = SearchMergeItem(line, type, level);


        return mergeItem;
    }

    // ������ ã��
    public MergeItem SearchMergeItem(Animalspecialization line, ItemType type, int level)
    {
        MergeItem item = null;

        if (type == ItemType.Spawner)
        {
            foreach (MergeItem mergeItem in spawners)
            {
                if (mergeItem.itemLine == line &&
                    mergeItem.Level == level)
                {
                    item = mergeItem;
                    return item;
                }
            }
        }

        foreach (MergeItem mergeItem in items)
        {
            if (mergeItem.itemLine == line &&
               mergeItem.Level == level
               && mergeItem.itemType == type)
            {
                item = mergeItem;
                return item;
            }
        }

        return item;
    }


    // ���� ������ ����
    public MergeItem GetRandomStartItem(Animalspecialization line, int level)
    {
        ItemType type = (ItemType)Random.Range(0, 2);
        int itemLevel = 1;

        for (int i = 1; i < level; i++)
        {
            if (Random.Range(0, 4) == 0)
                itemLevel++;
        }

        foreach (MergeItem item in items)
        {
            if (item.itemLine == line && item.Level == itemLevel && item.itemType == type)
            {
                return item;
            }
        }

        return null;
    }

    public MergeItem GetRandomSpecialItem(Animalspecialization line, int level)
    {
        List<Animalspecialization> wantLines = new List<Animalspecialization>();
        List<ItemType> wantTypes = new List<ItemType>();
        int wantLevel = 1;

        switch(line)
        {
            case Animalspecialization.Merge:
            case Animalspecialization.MergePay:
                wantLines.Add(Animalspecialization.Merge);
                wantTypes.Add(ItemType.Multiply);
                wantTypes.Add(ItemType.Add);
                wantTypes.Add(ItemType.Scissors);
                break;
            case Animalspecialization.Medal:
            case Animalspecialization.Gem:
            case Animalspecialization.Energy:
                wantLines.Add(line);
                wantTypes.Add(ItemType.Goods);
                break;
        }

        if (level == 1)
            wantLevel = 1;
        if (level == 2)
            wantLevel = Random.Range(1, 3);
        if (level == 3)
            wantLevel = Random.Range(1, 4);

        List<MergeItem> wantItems = new List<MergeItem>();

        foreach(MergeItem item in items)
        {
            if(wantLines.Contains(item.itemLine) && wantTypes.Contains(item.itemType) && wantLevel == item.Level)
                wantItems.Add(item);
        }

        if (wantItems.Count == 0)
            return null;
        else
            return wantItems[Random.Range(0, wantItems.Count)];
    }

    // ������ ���׷��̵�
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

    // ���� ������
    public MergeItem RandomItem(Animalspecialization line)
    {
        List<int> itemLevel = new List<int>();
        ItemType type = Random.Range(0,2) == 0 ? ItemType.Item_A : ItemType.Item_B;

        int lowLevel;
        int heithLevel;

        switch (Mathf.FloorToInt(mergeLevel / 3.64f))
        {
            case 0:
                for (int i = 0; i < 9; i++)
                    itemLevel.Add(2);
                for (int i = 0; i < 1; i++)
                    itemLevel.Add(3);
                break;
            case 1:
                for (int i = 0; i < 8; i++)
                    itemLevel.Add(2);
                for (int i = 0; i < 2; i++)
                    itemLevel.Add(3);
                break;
            case 2:
                for (int i = 0; i < 7; i++)
                    itemLevel.Add(2);
                for (int i = 0; i < 3; i++)
                    itemLevel.Add(3);
                break;
            case 3:
                for (int i = 0; i < 6; i++)
                    itemLevel.Add(2);
                for (int i = 0; i < 3; i++)
                    itemLevel.Add(3);
                for (int i = 0; i < 1; i++)
                    itemLevel.Add(4);
                break;
            case 4:
                for (int i = 0; i < 5; i++)
                    itemLevel.Add(2);
                for (int i = 0; i < 3; i++)
                    itemLevel.Add(3);
                for (int i = 0; i < 2; i++)
                    itemLevel.Add(4);
                break;
            case 5:
                for (int i = 0; i < 4; i++)
                    itemLevel.Add(2);
                for (int i = 0; i < 4; i++)
                    itemLevel.Add(3);
                for (int i = 0; i < 2; i++)
                    itemLevel.Add(4);
                break;
            case 6:
                for (int i = 0; i < 3; i++)
                    itemLevel.Add(2);
                for (int i = 0; i < 5; i++)
                    itemLevel.Add(3);
                for (int i = 0; i < 2; i++)
                    itemLevel.Add(4);
                break;
            case 7:
                for (int i = 0; i < 2; i++)
                    itemLevel.Add(2);
                for (int i = 0; i < 6; i++)
                    itemLevel.Add(3);
                for (int i = 0; i < 2; i++)
                    itemLevel.Add(4);
                break;
            case 8:
                for (int i = 0; i < 1; i++)
                    itemLevel.Add(2);
                for (int i = 0; i < 7; i++)
                    itemLevel.Add(3);
                for (int i = 0; i < 2; i++)
                    itemLevel.Add(4);
                break;
            case 9:
                for (int i = 0; i < 8; i++)
                    itemLevel.Add(3);
                for (int i = 0; i < 2; i++)
                    itemLevel.Add(4);
                break;
            case 10:
                for (int i = 0; i < 7; i++)
                    itemLevel.Add(3);
                for (int i = 0; i < 3; i++)
                    itemLevel.Add(4);
                break;
            case 11:
                for (int i = 0; i < 6; i++)
                    itemLevel.Add(3);
                for (int i = 0; i < 5; i++)
                    itemLevel.Add(4);
                for (int i = 0; i < 1; i++)
                    itemLevel.Add(5);
                break;
            case 12:
                for (int i = 0; i < 5; i++)
                    itemLevel.Add(3);
                for (int i = 0; i < 3; i++)
                    itemLevel.Add(4);
                for (int i = 0; i < 2; i++)
                    itemLevel.Add(5);
                break;
            case 13:
                for (int i = 0; i < 4; i++)
                    itemLevel.Add(3);
                for (int i = 0; i < 4; i++)
                    itemLevel.Add(4);
                for (int i = 0; i < 2; i++)
                    itemLevel.Add(5);
                break;
            case 14:
                for (int i = 0; i < 3; i++)
                    itemLevel.Add(3);
                for (int i = 0; i < 5; i++)
                    itemLevel.Add(4);
                for (int i = 0; i < 2; i++)
                    itemLevel.Add(5);
                break;
            case 15:
                for (int i = 0; i < 2; i++)
                    itemLevel.Add(3);
                for (int i = 0; i < 6; i++)
                    itemLevel.Add(4);
                for (int i = 0; i < 2; i++)
                    itemLevel.Add(5);
                break;
            case 16:
                for (int i = 0; i < 1; i++)
                    itemLevel.Add(3);
                for (int i = 0; i < 7; i++)
                    itemLevel.Add(4);
                for (int i = 0; i < 2; i++)
                    itemLevel.Add(5);
                break;
            case 17:
                for (int i = 0; i < 8; i++)
                    itemLevel.Add(4);
                for (int i = 0; i < 2; i++)
                    itemLevel.Add(5);
                break;
            case 18:
                for (int i = 0; i < 8; i++)
                    itemLevel.Add(4);
                for (int i = 0; i < 2; i++)
                    itemLevel.Add(5);
                break;
            case 19:
                for (int i = 0; i < 7; i++)
                    itemLevel.Add(4);
                for (int i = 0; i < 3; i++)
                    itemLevel.Add(5);
                break;
            case 20:
                for (int i = 0; i < 6; i++)
                    itemLevel.Add(4);
                for (int i = 0; i < 4; i++)
                    itemLevel.Add(5);
                break;
            case 21:
                for (int i = 0; i < 5; i++)
                    itemLevel.Add(4);
                for (int i = 0; i < 5; i++)
                    itemLevel.Add(5);
                break;
            case 22:
                for (int i = 0; i < 4; i++)
                    itemLevel.Add(4);
                for (int i = 0; i < 6; i++)
                    itemLevel.Add(5);
                break;
            case 23:
                for (int i = 0; i < 3; i++)
                    itemLevel.Add(4);
                for (int i = 0; i < 7; i++)
                    itemLevel.Add(5);
                break;
            case 24:
                for (int i = 0; i < 2; i++)
                    itemLevel.Add(4);
                for (int i = 0; i < 8; i++)
                    itemLevel.Add(5);
                break;
        }

        lowLevel = itemLevel[Random.Range(0, itemLevel.Count)];

        switch (Random.Range(0, 10))
        {
            case 0:
                heithLevel = GetItemLastLevel(line, type);
                break;
            case 1:
            case 2:
            case 3:
            case 4:
                heithLevel = GetItemLastLevel(line, type) - 1;
                break;
            default:
                heithLevel = GetItemLastLevel(line, type) - 2;
                break;
        }

        int level;

        int boughtStore = 0;

        foreach (Store store in storeList)
            if (store.bought)
                boughtStore++;

        level = Random.Range(0, 27-boughtStore*2) != 0 ? lowLevel : heithLevel;

        return SearchMergeItem(line, type, level);
    }

    // npcMission ������
    public List<MergeItem> RandomWantItem(Animalspecialization line)
    {
        List<MergeItem> result = new List<MergeItem>();
        List<MergeItem> iitems = new List<MergeItem>();

        foreach (MergeItem item in items)
        {
            if(item.itemLine == line)
                iitems.Add(item);
        }

        MergeItem wantItem;

        while (result.Count < 2)
        {
            wantItem = iitems[Random.Range(0, iitems.Count)];

            foreach(MergeItem want in result)
            {
                if (wantItem == want)
                    wantItem = null;
            }

            if(wantItem && wantItem.Level>2)
                result.Add(wantItem);
        }

        return result;
    }

    // ������ �������ӿ��� �������� ���� �Լ�
    public void SetSkin(Skeleton skel, double num)
    {
        int bodyNum = (int)(num / 1000000);
        num %= 1000000;
        int clothNum = (int)num / 10000;
        num %= 10000;
        int hatNum = (int)(num / 100);
        num %= 100;
        int scarfNum = (int)num;

        SkeletonData skelData = skel.Data;
        Skin mixAndMatchSkin = new Skin("Skin");

        if(skelData.FindSkin($"Body/{bodyNum}") != null)
            mixAndMatchSkin.AddSkin(skelData.FindSkin($"Body/{bodyNum}"));

        if (skelData.FindSkin($"Cloth/{clothNum}") != null)
            mixAndMatchSkin.AddSkin(skelData.FindSkin($"Cloth/{clothNum}"));

        if (skelData.FindSkin($"Hat/{hatNum}") != null)
            mixAndMatchSkin.AddSkin(skelData.FindSkin($"Hat/{hatNum}"));

        if (skelData.FindSkin($"Scarf/{scarfNum}") != null)
            mixAndMatchSkin.AddSkin(skelData.FindSkin($"Scarf/{scarfNum}"));

        skel.SetSkin(mixAndMatchSkin);
        skel.SetSlotsToSetupPose();
    }

    public double RandomSetSkin(Skeleton skel)
    {
        double skinData;

        int bodyNum = Random.Range(1, 10);
        int clothNum = Random.Range(1, 69); // 44
        int hatNum = Random.Range(0, 22);
        int scarfNum = Random.Range(0, 22);

        if (clothNum == 1 || clothNum == 2)
            scarfNum = 0;

        if (Random.Range(0, 2) == 1)
            scarfNum = 0;

        if (Random.Range(0, 2) == 1)
            hatNum = 0;

        skinData = bodyNum * 1000000 + clothNum * 10000 + hatNum * 100 + scarfNum;

        if(UserDataManager.instance.GetUserData().npcSkin == skinData)
        {
            return RandomSetSkin(skel); 
        }

        SetSkin(skel, skinData);

        return skinData;
    }


      private IEnumerator ConnectionTime()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);
           UserDataManager.instance.GetUserData().connectionTime += 1;
        }
    }

    public List<MergeItem> ProductAbleItems(MergeItem spawner)
    {
        List<MergeItem> spawnAbleItems = new List<MergeItem>();

        // �Ϲ� ������
        if((int)spawner.itemLine < 13)
        {
            foreach (MergeItem item in items)
            {
                if (item.itemLine == spawner.itemLine && item.Level <= spawner.Level)
                    spawnAbleItems.Add(item);
            }
        }
        // Ư�� ������
        else
        {
            List<Animalspecialization> wantLines = new List<Animalspecialization>();
            List<ItemType> wantTypes = new List<ItemType>();
            List<int> wantLevels = new List<int>();

            switch (spawner.itemLine)
            {
                case Animalspecialization.Merge:
                case Animalspecialization.MergePay:
                    wantLines.Add(Animalspecialization.Merge);
                    wantTypes.Add(ItemType.Multiply);
                    wantTypes.Add(ItemType.Add);
                    wantTypes.Add(ItemType.Scissors);
                    break;
                case Animalspecialization.Medal:
                case Animalspecialization.Gem:
                case Animalspecialization.Energy:
                    wantLines.Add(spawner.itemLine);
                    wantTypes.Add(ItemType.Goods);
                    break;
            }

            if (spawner.Level == 1)
                    wantLevels.Add(1);
            if (spawner.Level == 2)
                for (int i = 1; i < 3; i++)
                    wantLevels.Add(i);
            if (spawner.Level == 3)
                for (int i = 1; i < 4; i++)
                    wantLevels.Add(i);

            foreach (MergeItem item in items)
            {
                if (wantLines.Contains(item.itemLine) && wantTypes.Contains(item.itemType) && wantLevels.Contains(item.Level))
                    spawnAbleItems.Add(item);
            }
        }

        return spawnAbleItems;
    }

    public int GetItemLastLevel(Animalspecialization line, ItemType type)
    {
        int result = 0;

        foreach(MergeItem item in items)
        {
            if (item.itemLine == line  && item.itemType == type && item.Level > result)
                result = item.Level;
        }

        return result;
    }


}