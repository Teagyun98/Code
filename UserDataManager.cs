using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using System;
using System.IO;
using Unity.VisualScripting;
using System.Collections;

public class UserDataManager : MonoBehaviour
{
    public static UserDataManager instance;

    //유저 데이터
    private UserData userData;

    // 데이터 고유값
    private string gameDataRowInDate = string.Empty;

    string gameDataFileName = "JsonData.json";

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
            Destroy(this.gameObject);
    }

    // 유저 데이터
    public UserData GetUserData()
    {
        return userData;
    }

    //계정 생성시 유저 데이터 삽입
    public void UserDataInsert(bool reset = false)
    {
        userData = new UserData();

        // 데이터 초기화
        userData.ResetUserData();

        userData.mergeCoin = 60;
        userData.gem = 300;
        userData.pinkGem = 0;

        userData.animalBought.Add("토마토");

        userData.operationDay = 0;
        userData.totalMoneyAmount = 0;
        userData.mergeCount = 0;
        userData.wantCount = 0;

        //  - - - - Get Item
        userData.getMergeItem.Add(10201);

        //  - - - - line 1 - O
        userData.fieldItem.Add(0, -70108);
        userData.fieldItem.Add(1, -20001);
        userData.fieldItem.Add(2, -70103);
        userData.fieldItem.Add(3, -70006);
        userData.fieldItem.Add(4, -110102);
        userData.fieldItem.Add(5, -110104);
        userData.fieldItem.Add(6, -50001);
        //  - - - - line 2 - O
        userData.fieldItem.Add(7, -70005);
        userData.fieldItem.Add(8, -70106);
        userData.fieldItem.Add(9, -70001);
        userData.fieldItem.Add(10, -70105);
        userData.fieldItem.Add(11, -110001);
        userData.fieldItem.Add(12, -110005);
        userData.fieldItem.Add(13, -110003);


        //  - - - - line 3 - O
        userData.fieldItem.Add(14, -70101);
        userData.fieldItem.Add(15, -00001);
        userData.fieldItem.Add(16, -00004);
        userData.fieldItem.Add(17, -00001);
        userData.fieldItem.Add(18, -10101);
        userData.fieldItem.Add(19, -00101);
        userData.fieldItem.Add(20, -40003);


        //  - - - - line 4 - O
        userData.fieldItem.Add(21, -00101);
        userData.fieldItem.Add(22, -10102);
        userData.fieldItem.Add(23, -00003);
        userData.fieldItem.Add(24, -10001);
        userData.fieldItem.Add(25, -10002);
        userData.fieldItem.Add(26, -00105);
        userData.fieldItem.Add(27, -50003);
        //  - - - - line 5 - O
        userData.fieldItem.Add(28, -00001);
        userData.fieldItem.Add(29, -10101);
        //(30, None);
        //(31, None);
        userData.fieldItem.Add(32, -10001);
        userData.fieldItem.Add(33, -10102);
        userData.fieldItem.Add(34, -00102);
        //  - - - - line 6 - O
        userData.fieldItem.Add(35, -00101);
        userData.fieldItem.Add(36, -10001);
        userData.fieldItem.Add(37, -10002);
        userData.fieldItem.Add(38, -10102);
        userData.fieldItem.Add(39, -10101);
        userData.fieldItem.Add(40, -10101);
        userData.fieldItem.Add(41, -60005);


        //  - - - - line 7 - O
        userData.fieldItem.Add(42, -40101);
        userData.fieldItem.Add(43, -00103);
        userData.fieldItem.Add(44, -120101);
        userData.fieldItem.Add(45, -00002);
        userData.fieldItem.Add(46, -10001);
        userData.fieldItem.Add(47, -50106);
        userData.fieldItem.Add(48, -60007);

        //  - - - - line 8 - O
        userData.fieldItem.Add(49, -120001);
        userData.fieldItem.Add(50, -120004);
        userData.fieldItem.Add(51, -120103);
        userData.fieldItem.Add(52, -60002);
        userData.fieldItem.Add(53, -60001);
        userData.fieldItem.Add(54, -60105);
        userData.fieldItem.Add(55, -90003);
        //  - - - - line 9
        userData.fieldItem.Add(56, -120101);
        userData.fieldItem.Add(57, -120002);
        userData.fieldItem.Add(58, -80004);
        userData.fieldItem.Add(59, -60101);
        userData.fieldItem.Add(60, -60103);
        userData.fieldItem.Add(61, -90103);
        userData.fieldItem.Add(62, -20101);

        userData.leftOverAd.Add(10);
        userData.leftOverAd.Add(10);
        userData.leftOverAd.Add(10);
        userData.leftOverAd.Add(1);
        userData.leftOverAd.Add(1);
        userData.leftOverAd.Add(1);
        userData.leftOverAd.Add(1);
        userData.leftOverAd.Add(1);
        userData.leftOverAd.Add(1);
        userData.leftOverAd.Add(1);
        userData.leftOverAd.Add(1);
        userData.leftOverAd.Add(1);
        userData.leftOverAd.Add(10);

        userData.leftOverFree.Add(1);
        userData.leftOverFree.Add(1);

        userData.boxingTiles.Add(0);
        userData.boxingTiles.Add(1);
        userData.boxingTiles.Add(2);
        userData.boxingTiles.Add(3);
        userData.boxingTiles.Add(4);
        userData.boxingTiles.Add(5);
        userData.boxingTiles.Add(6);
        userData.boxingTiles.Add(7);
        userData.boxingTiles.Add(8);
        userData.boxingTiles.Add(9);
        userData.boxingTiles.Add(10);
        userData.boxingTiles.Add(11);
        userData.boxingTiles.Add(12);
        userData.boxingTiles.Add(13);

        userData.boxingTiles.Add(14);
        userData.boxingTiles.Add(15);
        userData.boxingTiles.Add(16);
        userData.boxingTiles.Add(17);
        userData.boxingTiles.Add(18);
        userData.boxingTiles.Add(19);
        userData.boxingTiles.Add(20);
        userData.boxingTiles.Add(42);
        userData.boxingTiles.Add(43);
        userData.boxingTiles.Add(44);
        userData.boxingTiles.Add(45);
        userData.boxingTiles.Add(46);
        userData.boxingTiles.Add(47);
        userData.boxingTiles.Add(48);
        userData.boxingTiles.Add(22);
        userData.boxingTiles.Add(28);
        userData.boxingTiles.Add(36);
        userData.boxingTiles.Add(21);
        userData.boxingTiles.Add(35);
        userData.boxingTiles.Add(25);
        userData.boxingTiles.Add(26);
        userData.boxingTiles.Add(27);
        userData.boxingTiles.Add(33);
        userData.boxingTiles.Add(34);
        userData.boxingTiles.Add(39);
        userData.boxingTiles.Add(40);
        userData.boxingTiles.Add(41);

        userData.boxingTiles.Add(49);
        userData.boxingTiles.Add(50);
        userData.boxingTiles.Add(51);
        userData.boxingTiles.Add(52);
        userData.boxingTiles.Add(53);
        userData.boxingTiles.Add(54);
        userData.boxingTiles.Add(55);
        userData.boxingTiles.Add(56);
        userData.boxingTiles.Add(57);
        userData.boxingTiles.Add(58);
        userData.boxingTiles.Add(59);
        userData.boxingTiles.Add(60);
        userData.boxingTiles.Add(61);
        userData.boxingTiles.Add(62);

        userData.connectimRewardIndex.Add(0);
        userData.connectimRewardIndex.Add(0);
        userData.connectimRewardIndex.Add(0);
        userData.connectimRewardIndex.Add(0);
        userData.connectimRewardIndex.Add(0);
        userData.connectimRewardIndex.Add(0);

        userData.Day7RewardIndex.Add(0);
        userData.Day7RewardIndex.Add(0);
        userData.Day7RewardIndex.Add(0);
        userData.Day7RewardIndex.Add(0);
        userData.Day7RewardIndex.Add(0);
        userData.Day7RewardIndex.Add(0);
        userData.Day7RewardIndex.Add(0);

        userData.firstDay = NowDay();

        userData.setting.Add(false);
        userData.setting.Add(false);
        userData.setting.Add(false);
        userData.setting.Add(false);
        userData.setting.Add(false);

        userData.costumeBuy.Add(false);
        userData.costumeBuy.Add(false);
        userData.costumeBuy.Add(false);
        userData.costumeBuy.Add(false);

        userData.incresePrice.Add(0);
        userData.incresePrice.Add(0);
        userData.incresePrice.Add(0);
        userData.incresePrice.Add(0);

        userData.mergeMission = new List<double> { 10103, -1, 10102, 10002, -1, 10003, -1 };

        if (reset)
            return;

        Param param = userData.ToParam();

        var bro = Backend.GameData.Insert("UserData", param);

        if (bro.IsSuccess())
        {
            string userDataIndate = bro.GetInDate();
            Debug.Log("UserData Insert : " + userDataIndate);
            UserDataLoad();
        }
        else
            Debug.LogError("게임 정보 삽입 실패 : " + bro.GetStatusCode());
    }

    // 유저 데이터 불러오기
    public void UserDataLoad()
    {
        // 서버에서 불러온 데이터를 저장할 새로운 클래스 생성
        userData = new UserData();
        // 서버에서 불러온 데이터를 저장하기 위해 초기화 시켜준다
        userData.ResetUserData();

        var bro = Backend.GameData.GetMyData("UserData", new Where());

        if (bro.IsSuccess())
        {
            Debug.Log($"게임 정보 데이터 불러오기에 성공했습니다. : {bro}");
        }

        LitJson.JsonData gameDataJason = bro.FlattenRows();

        if (gameDataJason.Count <= 0)
        {
            Debug.LogWarning("데이터가 존재하지 않습니다.");
            BackendLogin.instance.terms.SetActive(true);
            return;
        }
        else
        {
            if (gameDataJason[0].ContainsKey("inDate"))
                gameDataRowInDate = gameDataJason[0]["inDate"].ToString();

            // 튜토리얼 대화 진행도
            if (gameDataJason[0].ContainsKey("tutoPro"))
                userData.tutoPro = int.Parse(gameDataJason[0]["tutoPro"].ToString());

            // 튜토리얼 도중에 나간 경우 다시 데이터 초기화
            if (userData.tutoPro != 0 && userData.tutoPro < 29)
            {
                UserDataInsert(true);
                Debug.Log("완료");
                BackendLogin.instance.loading.SetActive(true);
                SceneController.instance.GameStart();
                return;
            }

            // 골드
            if (gameDataJason[0].ContainsKey("earning"))
                userData.earning = double.Parse(gameDataJason[0]["earning"].ToString());
            // 머지 아이템 생산에 필요한 에너지
            if (gameDataJason[0].ContainsKey("energy"))
                userData.energy = int.Parse(gameDataJason[0]["energy"].ToString());
            // 에너지 회복 남은 시간
            if (gameDataJason[0].ContainsKey("energyChargeTime"))
                userData.energyChargeTime = int.Parse(gameDataJason[0]["energyChargeTime"].ToString());
            // 가지고 있는 젬 개수
            if (gameDataJason[0].ContainsKey("gem"))
                userData.gem = int.Parse(gameDataJason[0]["gem"].ToString());
            // 가지고 있는 핑크젬 개수
            if (gameDataJason[0].ContainsKey("pinkGem"))
                userData.pinkGem = int.Parse(gameDataJason[0]["pinkGem"].ToString());
            // 가지고 있는 물고기 개수
            if (gameDataJason[0].ContainsKey("medal"))
                userData.medal = double.Parse(gameDataJason[0]["medal"].ToString());
            // 가지고 있는 열쇠 개수
            if (gameDataJason[0].ContainsKey("ticket"))
                userData.ticket = int.Parse(gameDataJason[0]["ticket"].ToString());
            // 머지 코인 개수
            if (gameDataJason[0].ContainsKey("mergeCoin"))
                userData.mergeCoin = int.Parse(gameDataJason[0]["mergeCoin"].ToString());

            // 접속시간
            if (gameDataJason[0].ContainsKey("connectionTime"))
                userData.connectionTime = int.Parse(gameDataJason[0]["connectionTime"].ToString());

            // ConnectimReward Index
            if (gameDataJason[0].ContainsKey("connectimRewardIndex"))
                foreach (var colum in gameDataJason[0]["connectimRewardIndex"])
                    userData.connectimRewardIndex.Add(int.Parse(colum.ToString()));

            // Day7Reward Index
            if (gameDataJason[0].ContainsKey("Day7RewardIndex"))
                foreach (var colum in gameDataJason[0]["Day7RewardIndex"])
                    userData.Day7RewardIndex.Add(int.Parse(colum.ToString()));

            // 캐릭터 보유 현황
            if (gameDataJason[0].ContainsKey("animalBought"))
                foreach (var colum in gameDataJason[0]["animalBought"])
                    userData.animalBought.Add(colum.ToString());

            // 캐릭터 레벨
            if (gameDataJason[0].ContainsKey("animalLevel"))
                foreach (var colum in gameDataJason[0]["animalLevel"].Keys)
                    userData.animalLevel.Add(colum, int.Parse(gameDataJason[0]["animalLevel"][colum].ToString()));

            // 캐릭터 별개수 저장
            if (gameDataJason[0].ContainsKey("animalStarNum"))
                foreach (var colum in gameDataJason[0]["animalStarNum"].Keys)
                    userData.animalStarNum.Add(colum, int.Parse(gameDataJason[0]["animalStarNum"][colum].ToString()));

            // 캐릭터 조각
            if (gameDataJason[0].ContainsKey("animalPiece"))
                foreach (var colum in gameDataJason[0]["animalPiece"].Keys)
                    userData.animalPiece.Add(colum, int.Parse(gameDataJason[0]["animalPiece"][colum].ToString()));

            // 보유하고 있는 상점
            if (gameDataJason[0].ContainsKey("storeBought"))
                foreach (var colum in gameDataJason[0]["storeBought"])
                    userData.storeBought.Add(int.Parse(colum.ToString()));

            // 상점 레벨
            if (gameDataJason[0].ContainsKey("storeLevel"))
                foreach (var colum in gameDataJason[0]["storeLevel"].Keys)
                    userData.storeLevel.Add(int.Parse(colum), int.Parse(gameDataJason[0]["storeLevel"][colum].ToString()));

            // 상점에 배치된 캐릭터
            if (gameDataJason[0].ContainsKey("storeSelectAnimal"))
                foreach (var colum in gameDataJason[0]["storeSelectAnimal"].Keys)
                    userData.storeSelectAnimal.Add(int.Parse(colum), gameDataJason[0]["storeSelectAnimal"][colum].ToString());

            // 출석 체크
            if (gameDataJason[0].ContainsKey("daily"))
                foreach (var colum in gameDataJason[0]["daily"])
                    userData.daily.Add(bool.Parse(colum.ToString()));

            // 출석체크
            if (gameDataJason[0].ContainsKey("dailyCheck"))
                userData.dailyCheck = bool.Parse(gameDataJason[0]["dailyCheck"].ToString());

            // 마지막 서버 저장 시간
            if (gameDataJason[0].ContainsKey("lastTime"))
                userData.lastTime = DateTime.Parse(gameDataJason[0]["lastTime"].ToString());

            // 최초 로그인 날짜
            if (gameDataJason[0].ContainsKey("firstDay"))
                userData.firstDay = DateTime.Parse(gameDataJason[0]["firstDay"].ToString());

            // 미션 진행도
            if (gameDataJason[0].ContainsKey("missionPro"))
                foreach (var colum in gameDataJason[0]["missionPro"].Keys)
                    userData.missionPro.Add(int.Parse(colum), double.Parse(gameDataJason[0]["missionPro"][colum].ToString()));

            // 미션에 할당된 상점
            if (gameDataJason[0].ContainsKey("storeId"))
                foreach (var colum in gameDataJason[0]["storeId"])
                    userData.storeId.Add(int.Parse(colum.ToString()));

            // 튜토리얼 진행도
            if (gameDataJason[0].ContainsKey("guideMissionPro"))
                foreach (var colum in gameDataJason[0]["guideMissionPro"].Keys)
                    userData.guideMissionPro.Add(int.Parse(colum), int.Parse(gameDataJason[0]["guideMissionPro"][colum].ToString()));

            // 머지 필드 아이템
            if (gameDataJason[0].ContainsKey("fieldItem"))
                foreach (var colum in gameDataJason[0]["fieldItem"].Keys)
                    userData.fieldItem.Add(int.Parse(colum), double.Parse(gameDataJason[0]["fieldItem"][colum].ToString()));

            // 머지 창고 아이템
            if (gameDataJason[0].ContainsKey("storageItem"))
                foreach (var colum in gameDataJason[0]["storageItem"])
                    userData.storageItem.Add(double.Parse(colum.ToString()));

            // 머지 미션
            if (gameDataJason[0].ContainsKey("mergeMission"))
                foreach (var colum in gameDataJason[0]["mergeMission"])
                    userData.mergeMission.Add(double.Parse(colum.ToString()));

            // 상점 단계
            if (gameDataJason[0].ContainsKey("storeStep"))
                foreach (var colum in gameDataJason[0]["storeStep"].Keys)
                    userData.storeStep.Add(int.Parse(colum), int.Parse(gameDataJason[0]["storeStep"][colum].ToString()));

            // village에서 보상으로 받은 아이템
            if (gameDataJason[0].ContainsKey("getMergeItem"))
                foreach (var colum in gameDataJason[0]["getMergeItem"])
                    userData.getMergeItem.Add(double.Parse(colum.ToString()));

            // npc 미션 아이템
            if (gameDataJason[0].ContainsKey("npcMission"))
                foreach (var colum in gameDataJason[0]["npcMission"])
                    userData.npcMission.Add(double.Parse(colum.ToString()));

            // npc 미션 받은 시간
            if (gameDataJason[0].ContainsKey("npcMissionLimit"))
                userData.npcMissionLimit = DateTime.Parse(gameDataJason[0]["npcMissionLimit"].ToString());

            // npc 미션 스킨
            if (gameDataJason[0].ContainsKey("npcSkin"))
                userData.npcSkin = double.Parse(gameDataJason[0]["npcSkin"].ToString());

            // 일일 광고 횟수
            if (gameDataJason[0].ContainsKey("leftOverAd"))
                foreach (var colum in gameDataJason[0]["leftOverAd"])
                    userData.leftOverAd.Add(int.Parse(colum.ToString()));

            // 일일 무료 횟수
            if (gameDataJason[0].ContainsKey("leftOverFree"))
                foreach (var colum in gameDataJason[0]["leftOverFree"])
                    userData.leftOverFree.Add(int.Parse(colum.ToString()));

            // 생성기 에너지
            if (gameDataJason[0].ContainsKey("spawnerEnergy"))
                foreach (var colum in gameDataJason[0]["spawnerEnergy"])
                    userData.spawnerEnergy.Add(int.Parse(colum.ToString()));

            // 박싱된 타일
            if (gameDataJason[0].ContainsKey("boxingTiles"))
                foreach (var colum in gameDataJason[0]["boxingTiles"])
                    userData.boxingTiles.Add(int.Parse(colum.ToString()));

            // 생성기 회복 시간
            if (gameDataJason[0].ContainsKey("productTime"))
                foreach (var colum in gameDataJason[0]["productTime"])
                    userData.productTime.Add(int.Parse(colum.ToString()));

            // 창고 개수
            if (gameDataJason[0].ContainsKey("storageCount"))
                userData.storageCount = int.Parse(gameDataJason[0]["storageCount"].ToString());

            // Setting
            if (gameDataJason[0].ContainsKey("setting"))
                foreach (var colum in gameDataJason[0]["setting"])
                    userData.setting.Add(bool.Parse(colum.ToString()));

            // costumeBuy
            if (gameDataJason[0].ContainsKey("costumeBuy"))
                foreach (var colum in gameDataJason[0]["costumeBuy"])
                    userData.costumeBuy.Add(bool.Parse(colum.ToString()));

            // 오늘 젬으로 에너지를 충전한 횟수
            if (gameDataJason[0].ContainsKey("energyCharge"))
                userData.energyCharge = int.Parse(gameDataJason[0]["energyCharge"].ToString());

            // 꾸미기 몸
            if (gameDataJason[0].ContainsKey("skinBody"))
                userData.skinBody = int.Parse(gameDataJason[0]["skinBody"].ToString());

            // 꾸미기 옷
            if (gameDataJason[0].ContainsKey("skinCostume"))
                userData.skinCostume = int.Parse(gameDataJason[0]["skinCostume"].ToString());

            // 챕터
            if (gameDataJason[0].ContainsKey("chapterNum"))
                userData.chapterNum = int.Parse(gameDataJason[0]["chapterNum"].ToString());

            // 머지 아이템 도감
            if (gameDataJason[0].ContainsKey("dicItems"))
                foreach (var colum in gameDataJason[0]["dicItems"])
                    userData.dicItems.Add(double.Parse(colum.ToString()));

            if (gameDataJason[0].ContainsKey("incresePrice"))
                foreach (var colum in gameDataJason[0]["incresePrice"])
                    userData.incresePrice.Add(int.Parse(colum.ToString()));

            // 머지 버블 데이터
            if (gameDataJason[0].ContainsKey("bubbles"))
                foreach (var colum in gameDataJason[0]["bubbles"].Keys)
                    userData.bubbles.Add(int.Parse(colum.ToString()), int.Parse(gameDataJason[0]["bubbles"][colum].ToString()));

            // 운영 일짜
            if (gameDataJason[0].ContainsKey("operationDay"))
                userData.operationDay = int.Parse(gameDataJason[0]["operationDay"].ToString());

            // 운영 일짜
            if (gameDataJason[0].ContainsKey("mergeCount"))
                userData.mergeCount = int.Parse(gameDataJason[0]["mergeCount"].ToString());
            // 운영 일짜

            if (gameDataJason[0].ContainsKey("wantCount"))
                userData.wantCount = int.Parse(gameDataJason[0]["wantCount"].ToString());

            // 미션 골드 목표
            if (gameDataJason[0].ContainsKey("totalMoneyAmount"))
                userData.totalMoneyAmount = double.Parse(gameDataJason[0]["totalMoneyAmount"].ToString());

            // 당일 첫 로그인 체크
            if (gameDataJason[0].ContainsKey("dayLogin"))
                userData.dayLogin = bool.Parse(gameDataJason[0]["dayLogin"].ToString());

            // 튜토리얼 클리어
            if (gameDataJason[0].ContainsKey("tutoClear"))
                userData.tutoClear = bool.Parse(gameDataJason[0]["tutoClear"].ToString());

            if (gameDataJason[0].ContainsKey("todayItems"))
                foreach (var colum in gameDataJason[0]["todayItems"].Keys)
                    userData.todayItems.Add(double.Parse(colum.ToString()), bool.Parse(gameDataJason[0]["todayItems"][colum].ToString()));


            Debug.Log("완료");
            BackendLogin.instance.loading.SetActive(true);
            SceneController.instance.GameStart();
        }
    }

    private List<int> StringToList(string sList)
    {
        if (sList.StartsWith("[") && sList.EndsWith("]"))
            sList = sList.Substring(1, sList.Length - 2);

        string[] sArr = sList.Split(',');

        List<int> result = new List<int>();

        foreach (string sStr in sArr)
            result.Add(int.Parse(sStr));

        return result;
    }

    // 유저 데이터 업데이트
    // 수정 사항 : 재화 획득같은 호출이 잦은 데이터 주기 저장으로 변경 및 강제종료 시 저장으로 변경
    // Backend.GameData.TransactionWriteV2() 사용 추천

    public void UpdateUserData_VillageData(bool sceneMove = false, bool escape = false)
    {
        userData.energyChargeTime = EarningManager.Instance.time;
        userData.lastTime = NowDay(true);

        Param param = new Param
        {
            { "earning", userData.earning },
            { "tutoPro", userData.tutoPro },
            { "energy", userData.energy },
            { "energyChargeTime", userData.energyChargeTime },
            { "connectimRewardIndex", userData.connectimRewardIndex },
            { "Day7RewardIndex", userData.Day7RewardIndex },
            { "firstDay", userData.firstDay },
            { "dailyCheck", userData.dailyCheck },
            { "gem", userData.gem },
            { "pinkGem", userData.pinkGem },
            { "medal", userData.medal },
            { "ticket", userData.ticket },
            { "mergeCoin", userData.mergeCoin },
            { "connectionTime", userData.connectionTime },
            { "fieldItem", userData.fieldItem },
            { "storeLevel", userData.storeLevel },
            { "storeSelectAnimal", userData.storeSelectAnimal },
            { "missionPro", userData.missionPro },
            { "storeId", userData.storeId },
            { "npcMission", userData.npcMission },
            { "npcMissionLimit", userData.npcMissionLimit },
            { "npcSkin", userData.npcSkin },
            { "lastTime", userData.lastTime },
            { "leftOverAd", userData.leftOverAd },
            { "leftOverFree", userData.leftOverFree },
            { "spawnerEnergy", userData.spawnerEnergy },
            { "setting", userData.setting },
            { "incresePrice", userData.incresePrice },
            { "operationDay", userData.operationDay },
            { "totalMoneyAmount", userData.totalMoneyAmount },
            { "tutoClear", userData.tutoClear },
            { "todayItems", userData.todayItems },
            { "energyCharge", userData.energyCharge },
            { "skinBody", userData.skinBody },
            { "skinCostume", userData.skinCostume },
            { "chapterNum", userData.chapterNum },
            { "wantCount", userData.wantCount },
            { "costumeBuy", userData.costumeBuy },

        };

        // 가이드 미션이 남아 있으면 서버에 데이터 저장
        if (MissionManager.instance.GetGuideMissions().Count != 0)
            param.Add("guideMissionPro", userData.guideMissionPro);

        param.Add("getMergeItem", userData.getMergeItem);
        param.Add("storeStep", userData.storeStep);

        UpdateUserData(param, sceneMove == true ? "MergeScene" : null, escape);
    }

    public void UpdateUserData_MergeData(bool sceneMove = false, bool escape = false)
    {
        userData.energyChargeTime = EarningManager.Instance.time;
        userData.lastTime = NowDay(true);

        Param param = new Param
        {
            { "tutoPro", userData.tutoPro },
            { "earning", userData.earning },
            { "energy", userData.energy },
            { "energyChargeTime", userData.energyChargeTime },
            { "gem", userData.gem },
            { "pinkGem", userData.pinkGem },
            { "medal", userData.medal },
            { "ticket", userData.ticket },
            { "mergeCoin", userData.mergeCoin },
            { "connectionTime", userData.connectionTime },
            { "missionPro", userData.missionPro },
            { "fieldItem", userData.fieldItem },
            { "storageItem", userData.storageItem },
            { "mergeMission", userData.mergeMission },
            { "getMergeItem", userData.getMergeItem },
            { "storeStep", userData.storeStep },
            { "npcMission", userData.npcMission },
            { "npcMissionLimit", userData.npcMissionLimit },
            { "npcSkin", userData.npcSkin },
            { "lastTime", userData.lastTime },
            { "leftOverAd", userData.leftOverAd },
            { "spawnerEnergy", userData.spawnerEnergy },
            { "boxingTiles", userData.boxingTiles },
            { "productTime", userData.productTime },
            { "storageCount", userData.storageCount },
            { "setting", userData.setting },
            { "energyCharge", userData.energyCharge },
            { "skinBody", userData.skinBody },
            { "skinCostume", userData.skinCostume },
            { "dicItems", userData.dicItems },
            { "bubbles", userData.bubbles },
            { "todayItems", userData.todayItems },
            { "incresePrice", userData.incresePrice },
            { "mergeCount", userData.mergeCount },
            { "costumeBuy", userData.costumeBuy },

        };

        UpdateUserData(param, sceneMove == true ? "VillageScene" : null, escape);
    }

    public void UpdateUserData_tutoPro()
    {
        Param param = new Param();

        param.Add("tutoPro", userData.tutoPro);

        UpdateUserData(param);
    }

    public void UpdateUserData_gem()
    {
        Param param = new Param();

        param.Add("gem", userData.gem);

        UpdateUserData(param);
    }

    public void UpdateUserData_Pinkgem()
    {
        Param param = new Param();

        param.Add("pinkGem", userData.pinkGem);

        UpdateUserData(param);
    }

    public void UpdateUserData_ticket()
    {
        Param param = new Param();

        param.Add("ticket", userData.ticket);

        UpdateUserData(param);
    }

    public void UpdateUserData_animalBought()
    {
        Param param = new Param();

        param.Add("animalBought", userData.animalBought);

        UpdateUserData(param);
    }

    public void UpdateUserData_animalLevel()
    {
        Param param = new Param();

        param.Add("animalLevel", userData.animalLevel);

        UpdateUserData(param);
    }
    public void UpdateUserData_animalStarNum()
    {
        Param param = new Param();

        param.Add("animalStarNum", userData.animalStarNum);

        UpdateUserData(param);
    }

    public void UpdateUserData_animalPiece()
    {
        Param param = new Param();

        param.Add("animalPiece", userData.animalPiece);

        UpdateUserData(param);
    }

    public void UpdateUserData_storeBought()
    {
        Param param = new Param();

        param.Add("storeBought", userData.storeBought);

        UpdateUserData(param);
    }

    public void UpdateUserData_storeLevel()
    {
        Param param = new Param();

        param.Add("storeLevel", userData.storeLevel);

        UpdateUserData(param);
    }


    public void UpdateUserData_daily()
    {
        Param param = new Param();

        param.Add("daily", userData.daily);

        UpdateUserData(param);
    }

    public void UpdateUserData_dailyCheck()
    {
        Param param = new Param();

        param.Add("dailyCheck", userData.dailyCheck);

        UpdateUserData(param);
    }

    public void UpdateUserData_lastTime()
    {
        Param param = new Param();

        param.Add("lastLogin", userData.lastTime);

        UpdateUserData(param);
    }

    public void UpdateUserData_missionPro()
    {
        Param param = new Param();

        param.Add("missionPro", userData.missionPro);

        UpdateUserData(param);
    }


    public void UpdateUserData_guideMissionPro()
    {
        Param param = new Param();

        param.Add("guideMissionPro", userData.guideMissionPro);

        UpdateUserData(param);
    }

    public void UpdateUserData_storeStep()
    {
        Param param = new Param();

        param.Add("storeStep", userData.storeStep);

        UpdateUserData(param);
    }

    public void UpdateUserData_storeId()
    {
        Param param = new Param();

        param.Add("storeId", userData.storeId);

        UpdateUserData(param);
    }

    public void UpdateUserData_totalMoneyAmount()
    {
        Param param = new Param();

        param.Add("totalMoneyAmount", userData.totalMoneyAmount);

        UpdateUserData(param);
    }

    public void UpdateUserData_WantCount()
    {
        Param param = new Param();

        param.Add("wantCount", userData.wantCount);

        UpdateUserData(param);
    }

    private void UpdateUserData(Param param, String sceneName = null, bool excape = false)
    {
        if (!BackEndManager.Instance.google && !BackEndManager.Instance.guest)
        {
            if (sceneName != null)
                StartCoroutine(SceneController.instance.SceneLoad(sceneName));

            if (excape)
                Application.Quit();

            return;
        }

        if (userData == null)
        {
            Debug.LogError(" 서버에서 다운받거나 새로 삽입할 데이터가 존재하지 않습니다.");
            return;
        }

        // 게임 정보의 고유값이 없으면 에러 메시지 출력
        if (string.IsNullOrEmpty(gameDataRowInDate))
        {
            Debug.LogError("유저의 inDate 정보가 없어 게임 정보 데이터 수정에 실패했습니다.");
            Debug.Log("IsNullOfEmpty");
        }
        else
        {
            Debug.Log($"{gameDataRowInDate} 의 게임 정보 데이터 수정을 요청합니다.");

            Backend.GameData.UpdateV2("UserData", gameDataRowInDate, Backend.UserInDate, param, callback =>
            {
                if (callback.IsSuccess())
                {
                    Debug.Log($"게임 정보 데이터 수정에 성공했습니다. : {callback}");

                    if (sceneName != null)
                        StartCoroutine(SceneController.instance.SceneLoad(sceneName));

                    if (excape)
                    {
                        Application.Quit();
                    }

                }
                else
                {
                    Debug.LogError($"게임 정보 데이터 수정에 실패했습니다. : {callback}");

                    if (BackendLogin.instance)
                        BackendLogin.instance.connectPop.gameObject.SetActive(true);
                    if (GameManager.instance)
                    {
                        GameManager.instance.moveScene = false;
                        GameManager.instance.connectPop.SetActive(true);
                        CameraMovements.instance.UIOpen = true;
                    }
                    if (MergeGameManager.instance)
                    {
                        MergeGameManager.instance.moveScene = false;
                        MergeGameManager.instance.connectPop.gameObject.SetActive(true);
                        MergeGameManager.instance.UIOpen();
                    }

                    SceneController.instance.moveScene = false;
                }
            });
        }
    }

    public void UpdateUserData_Setting()
    {
        Param param = new Param();

        param.Add("setting", userData.setting);

        UpdateUserData(param);
    }

    public void UpdateUserData_Day7RewardIndex()
    {
        Param param = new Param();

        param.Add("Day7RewardIndex", userData.Day7RewardIndex);

        UpdateUserData(param);
    }

    public void UpdateUserData_SkinBody()
    {
        Param param = new Param();

        param.Add("skinBody", userData.skinBody);

        UpdateUserData(param);
    }

    public void UpdateUserData_SkinCostume()
    {
        Param param = new Param();

        param.Add("skinCostume", userData.skinCostume);

        UpdateUserData(param);
    }

    public void UpdateUserData_ChapterNum()
    {
        Param param = new Param();

        param.Add("chapterNum", userData.chapterNum);

        UpdateUserData(param);
    }

    private bool getTime = false;
    private DateTime nowTime;

    // 서버 시간
    public DateTime NowDay(bool refresh = false)
    {
        BackendReturnObject servertime = Backend.Utils.GetServerTime();

        if (!servertime.IsSuccess() || (getTime && !refresh))
            return nowTime;

        string time = servertime.GetReturnValuetoJSON()["utcTime"].ToString();
        nowTime = DateTime.Parse(time);

        if (!getTime)
            StartCoroutine(TimeStart());

        return nowTime;
    }

    private IEnumerator TimeStart()
    {
        getTime = true;

        while (true)
        {
            yield return new WaitForSeconds(1);
            nowTime.AddSeconds(1);
        }
    }

    public void SetTestData()
    {
        userData = new UserData();

        userData.ResetUserData();

        userData.mergeCoin = 60;
        userData.gem = 300;
        userData.pinkGem = 0;

        userData.animalBought.Add("토마토");

        userData.operationDay = 0;
        userData.totalMoneyAmount = 0;
        userData.mergeCount = 0;
        userData.wantCount = 0;

        //  - - - - Get Item
        userData.getMergeItem.Add(10201);

        //  - - - - line 1 - O
        userData.fieldItem.Add(0, -70108);
        userData.fieldItem.Add(1, -20001);
        userData.fieldItem.Add(2, -70103);
        userData.fieldItem.Add(3, -70006);
        userData.fieldItem.Add(4, -110102);
        userData.fieldItem.Add(5, -110104);
        userData.fieldItem.Add(6, -50001);
        //  - - - - line 2 - O
        userData.fieldItem.Add(7, -70005);
        userData.fieldItem.Add(8, -70106);
        userData.fieldItem.Add(9, -70001);
        userData.fieldItem.Add(10, -70105);
        userData.fieldItem.Add(11, -110001);
        userData.fieldItem.Add(12, -110005);
        userData.fieldItem.Add(13, -110003);


        //  - - - - line 3 - O
        userData.fieldItem.Add(14, -70101);
        userData.fieldItem.Add(15, -00001);
        userData.fieldItem.Add(16, -00004);
        userData.fieldItem.Add(17, -00001);
        userData.fieldItem.Add(18, -10101);
        userData.fieldItem.Add(19, -00101);
        userData.fieldItem.Add(20, -40003);


        //  - - - - line 4 - O
        userData.fieldItem.Add(21, -00101);
        userData.fieldItem.Add(22, -10102);
        userData.fieldItem.Add(23, -00003);
        userData.fieldItem.Add(24, -10001);
        userData.fieldItem.Add(25, -10002);
        userData.fieldItem.Add(26, -00105);
        userData.fieldItem.Add(27, -50003);
        //  - - - - line 5 - O
        userData.fieldItem.Add(28, -00001);
        userData.fieldItem.Add(29, -10101);
        //(30, None);
        //(31, None);
        userData.fieldItem.Add(32, -10001);
        userData.fieldItem.Add(33, -10102);
        userData.fieldItem.Add(34, -00102);
        //  - - - - line 6 - O
        userData.fieldItem.Add(35, -00101);
        userData.fieldItem.Add(36, -10001);
        userData.fieldItem.Add(37, -10002);
        userData.fieldItem.Add(38, -10102);
        userData.fieldItem.Add(39, -10101);
        userData.fieldItem.Add(40, -10101);
        userData.fieldItem.Add(41, -60005);


        //  - - - - line 7 - O
        userData.fieldItem.Add(42, -40101);
        userData.fieldItem.Add(43, -00103);
        userData.fieldItem.Add(44, -120101);
        userData.fieldItem.Add(45, -00002);
        userData.fieldItem.Add(46, -10001);
        userData.fieldItem.Add(47, -50106);
        userData.fieldItem.Add(48, -60007);

        //  - - - - line 8 - O
        userData.fieldItem.Add(49, -120001);
        userData.fieldItem.Add(50, -120004);
        userData.fieldItem.Add(51, -120103);
        userData.fieldItem.Add(52, -60002);
        userData.fieldItem.Add(53, -60001);
        userData.fieldItem.Add(54, -60105);
        userData.fieldItem.Add(55, -90003);
        //  - - - - line 9
        userData.fieldItem.Add(56, -120101);
        userData.fieldItem.Add(57, -120002);
        userData.fieldItem.Add(58, -80004);
        userData.fieldItem.Add(59, -60101);
        userData.fieldItem.Add(60, -60103);
        userData.fieldItem.Add(61, -90103);
        userData.fieldItem.Add(62, -20101);

        userData.leftOverAd.Add(10);
        userData.leftOverAd.Add(10);
        userData.leftOverAd.Add(10);
        userData.leftOverAd.Add(1);
        userData.leftOverAd.Add(1);
        userData.leftOverAd.Add(1);
        userData.leftOverAd.Add(1);
        userData.leftOverAd.Add(1);
        userData.leftOverAd.Add(1);
        userData.leftOverAd.Add(1);
        userData.leftOverAd.Add(1);
        userData.leftOverAd.Add(1);
        userData.leftOverAd.Add(10);

        userData.leftOverFree.Add(1);
        userData.leftOverFree.Add(1);

        userData.boxingTiles.Add(0);
        userData.boxingTiles.Add(1);
        userData.boxingTiles.Add(2);
        userData.boxingTiles.Add(3);
        userData.boxingTiles.Add(4);
        userData.boxingTiles.Add(5);
        userData.boxingTiles.Add(6);
        userData.boxingTiles.Add(7);
        userData.boxingTiles.Add(8);
        userData.boxingTiles.Add(9);
        userData.boxingTiles.Add(10);
        userData.boxingTiles.Add(11);
        userData.boxingTiles.Add(12);
        userData.boxingTiles.Add(13);

        userData.boxingTiles.Add(14);
        userData.boxingTiles.Add(15);
        userData.boxingTiles.Add(16);
        userData.boxingTiles.Add(17);
        userData.boxingTiles.Add(18);
        userData.boxingTiles.Add(19);
        userData.boxingTiles.Add(20);
        userData.boxingTiles.Add(42);
        userData.boxingTiles.Add(43);
        userData.boxingTiles.Add(44);
        userData.boxingTiles.Add(45);
        userData.boxingTiles.Add(46);
        userData.boxingTiles.Add(47);
        userData.boxingTiles.Add(48);
        userData.boxingTiles.Add(22);
        userData.boxingTiles.Add(28);
        userData.boxingTiles.Add(36);
        userData.boxingTiles.Add(21);
        userData.boxingTiles.Add(35);
        userData.boxingTiles.Add(25);
        userData.boxingTiles.Add(26);
        userData.boxingTiles.Add(27);
        userData.boxingTiles.Add(33);
        userData.boxingTiles.Add(34);
        userData.boxingTiles.Add(39);
        userData.boxingTiles.Add(40);
        userData.boxingTiles.Add(41);

        userData.boxingTiles.Add(49);
        userData.boxingTiles.Add(50);
        userData.boxingTiles.Add(51);
        userData.boxingTiles.Add(52);
        userData.boxingTiles.Add(53);
        userData.boxingTiles.Add(54);
        userData.boxingTiles.Add(55);
        userData.boxingTiles.Add(56);
        userData.boxingTiles.Add(57);
        userData.boxingTiles.Add(58);
        userData.boxingTiles.Add(59);
        userData.boxingTiles.Add(60);
        userData.boxingTiles.Add(61);
        userData.boxingTiles.Add(62);

        userData.connectimRewardIndex.Add(0);
        userData.connectimRewardIndex.Add(0);
        userData.connectimRewardIndex.Add(0);
        userData.connectimRewardIndex.Add(0);
        userData.connectimRewardIndex.Add(0);
        userData.connectimRewardIndex.Add(0);

        userData.Day7RewardIndex.Add(0);
        userData.Day7RewardIndex.Add(0);
        userData.Day7RewardIndex.Add(0);
        userData.Day7RewardIndex.Add(0);
        userData.Day7RewardIndex.Add(0);
        userData.Day7RewardIndex.Add(0);
        userData.Day7RewardIndex.Add(0);

        userData.firstDay = NowDay();

        userData.setting.Add(false);
        userData.setting.Add(false);
        userData.setting.Add(false);
        userData.setting.Add(false);
        userData.setting.Add(false);

        userData.costumeBuy.Add(false);
        userData.costumeBuy.Add(false);
        userData.costumeBuy.Add(false);
        userData.costumeBuy.Add(false);

        userData.incresePrice.Add(0);
        userData.incresePrice.Add(0);
        userData.incresePrice.Add(0);
        userData.incresePrice.Add(0);

        userData.mergeMission = new List<double> { 10103, -1, 10102, 10002, -1, 10003, -1 };

        SceneController.instance.GameStart();
    }

    public void Destroy()
    {
        Destroy(this);
    }

    public void SendLog(string message)
    {
        if (!BackEndManager.Instance.google && !BackEndManager.Instance.guest)
            return;

        // 뒤끝 로그 테스트 함수
        Backend.GameLog.InsertLog("GameLog", new Param { { message, NowDay() } });
    }

    public void LoadGameData()
    {
        string filePath = Application.persistentDataPath + "/" + gameDataFileName;

        if (File.Exists(filePath))
        {
            string FromJsonData = File.ReadAllText(filePath);
            userData = JsonUtility.FromJson<UserData>(FromJsonData);
            Debug.Log("Json Data 불러오기 완료");
        }
    }

    public void SaveGameData()
    {
        string toJsonData = JsonUtility.ToJson(userData, true);
        string filePath = Application.persistentDataPath + "/" + gameDataFileName;

        File.WriteAllText(filePath, toJsonData);

        Debug.Log("Json Data 저장 완료");

    }
}