using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using System;

public class UserDataManager : MonoBehaviour
{
    public static UserDataManager instance;

    //���� ������
    private UserData userData;

    // ������ ������
    private string gameDataRowInDate = string.Empty;

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

    // ���� ������
    public UserData GetUserData()
    {
        return userData;
    }

    //���� ������ ���� ������ ����
    public void UserDataInsert()
    {
        userData = new UserData();

        // ������ �ʱ�ȭ
        userData.ResetUserData();
        // ó�� ������ �� store1 ������ ����
        userData.storeBought.Add(1);
        userData.fieldItem.Add(4, 29);
        userData.fieldItem.Add(14, 32);
        userData.fieldItem.Add(24, 35);

        Param param = userData.ToParam();

        var bro = Backend.GameData.Insert("UserData", param);

        if (bro.IsSuccess())
        {
            string userDataIndate = bro.GetInDate();
            Debug.Log("UserData Insert : " + userDataIndate);
        }
        else
        {
            Debug.LogError("���� ���� ���� ���� : " + bro.GetStatusCode());
        }
    }

    // ���� ������ �ҷ�����
    public void UserDataLoad()
    {
        // �������� �ҷ��� �����͸� ������ ���ο� Ŭ���� ����
        userData = new UserData();
        // �������� �ҷ��� �����͸� �����ϱ� ���� �ʱ�ȭ �����ش�
        userData.ResetUserData();

        var bro = Backend.GameData.GetMyData("UserData", new Where());

        if (bro.IsSuccess())
        {
            Debug.Log($"���� ���� ������ �ҷ����⿡ �����߽��ϴ�. : {bro}");
        }

        LitJson.JsonData gameDataJason = bro.FlattenRows();

        foreach (var key in gameDataJason[0])
        {
            Debug.Log(key.ToString());
        }

        if (gameDataJason.Count <= 0)
        {
            Debug.LogWarning("�����Ͱ� �������� �ʽ��ϴ�.");
        }
        else
        {
            gameDataRowInDate = gameDataJason[0]["inDate"].ToString();

            userData.earning = double.Parse(gameDataJason[0]["earning"].ToString());
            userData.energy = int.Parse(gameDataJason[0]["energy"].ToString());
            userData.gem = int.Parse(gameDataJason[0]["gem"].ToString());
            userData.medal = double.Parse(gameDataJason[0]["medal"].ToString());
            userData.ticket = int.Parse(gameDataJason[0]["ticket"].ToString());
            userData.stage = int.Parse(gameDataJason[0]["stage"].ToString());

            foreach (var colum in gameDataJason[0]["animalBought"])
            {
                userData.animalBought.Add(colum.ToString());
            }

            foreach (var colum in gameDataJason[0]["animalLevel"].Keys)
            {
                userData.animalLevel.Add(colum, int.Parse(gameDataJason[0]["animalLevel"][colum].ToString()));
            }

            foreach (var colum in gameDataJason[0]["animalStarNum"].Keys)
            {
                userData.animalStarNum.Add(colum, int.Parse(gameDataJason[0]["animalStarNum"][colum].ToString()));
            }

            foreach (var colum in gameDataJason[0]["animalPiece"].Keys)
            {
                userData.animalPiece.Add(colum, int.Parse(gameDataJason[0]["animalPiece"][colum].ToString()));
            }

            foreach (var colum in gameDataJason[0]["storeBought"])
            {
                userData.storeBought.Add(int.Parse(colum.ToString()));
            }

            foreach (var colum in gameDataJason[0]["storeLevel"].Keys)
            {
                userData.storeLevel.Add(int.Parse(colum), int.Parse(gameDataJason[0]["storeLevel"][colum].ToString()));
            }


            foreach (var colum in gameDataJason[0]["storeSelectAnimal"].Keys)
            {
                userData.storeSelectAnimal.Add(int.Parse(colum), gameDataJason[0]["storeSelectAnimal"][colum].ToString());
            }

            foreach(var colum in gameDataJason[0]["daily"])
            {
                userData.daily.Add(bool.Parse(colum.ToString()));
            }

            userData.dailyCheck = bool.Parse(gameDataJason[0]["dailyCheck"].ToString());

            //lastLogin
            //missionpro

            foreach (var colum in gameDataJason[0]["fieldItem"].Keys)
            {
                userData.fieldItem.Add(int.Parse(colum), int.Parse(gameDataJason[0]["fieldItem"][colum].ToString()));
            }

            foreach (var colum in gameDataJason[0]["storageItem"])
            {
                userData.storageItem.Add(int.Parse(colum.ToString()));
            }

            foreach (var colum in gameDataJason[0]["mergeMission"])
            {
                userData.mergeMission.Add(int.Parse(colum.ToString()));
            }

            userData.normalMerge = int.Parse(gameDataJason[0]["normalMerge"].ToString());
            userData.storeOpenMerge = int.Parse(gameDataJason[0]["storeOpenMerge"].ToString());

            userData.storeStep = int.Parse(gameDataJason[0]["storeStep"].ToString());

            userData.guideMission = int.Parse(gameDataJason[0]["guideMission"].ToString());

            foreach(var colum in gameDataJason[0]["getMergeItem"])
            {
                userData.getMergeItem.Add(int.Parse(colum.ToString()));
            }

            Debug.Log("�Ϸ�");
        }
    }

    // ���� ������ ������Ʈ
    // ���� ���� : ��ȭ ȹ�氰�� ȣ���� ���� ������ �ֱ� �������� ���� �� �������� �� �������� ����
    // Backend.GameData.TransactionWriteV2() ��� ��õ

    public void UpdateUserData_VillageData()
    {
        Param param = new Param();

        param.Add("earning", userData.earning);
        param.Add("energy", userData.energy);
        param.Add("gem", userData.gem);
        param.Add("medal", userData.medal);
        param.Add("ticket", userData.ticket);
        param.Add("stage", userData.stage);
        param.Add("storeLevel", userData.storeLevel);
        param.Add("storeSelectAnimal", userData.storeSelectAnimal);
        param.Add("missionPro", userData.missionPro);
        param.Add("getMergeItem", userData.getMergeItem);
        param.Add("storeStep", userData.storeStep);

        UpdateUserData(param);
    }

    public void UpdateUserData_MergeData()
    {
        Param param = new Param();

        param.Add("earning", userData.earning);
        param.Add("energy", userData.energy);
        param.Add("gem", userData.gem);
        param.Add("medal", userData.medal);
        param.Add("ticket", userData.ticket);
        param.Add("missionPro", userData.missionPro);
        param.Add("fieldItem", userData.fieldItem);
        param.Add("storageItem", userData.storageItem);
        param.Add("mergeMission", userData.mergeMission);
        param.Add("storeMergeMission", userData.storeMergeMission);
        param.Add("normalMerge", userData.normalMerge);
        param.Add("storeOpenMerge", userData.storeOpenMerge);
        param.Add("getMergeItem", userData.getMergeItem);
        param.Add("storeStep", userData.storeStep);

        UpdateUserData(param);
    }

    public void UpdateUserData_earning()
    {
        Param param = new Param();

        param.Add("earning", userData.earning);

        UpdateUserData(param);
    }

    public void UpdateUserData_gem()
    {
        Param param = new Param();

        param.Add("gem", userData.gem);

        UpdateUserData(param);
    }

    public void UpdateUserData_medal()
    {
        Param param = new Param();

        param.Add("medal", userData.medal);

        UpdateUserData(param);
    }
    
     public void UpdateUserData_ticket()
    {
        Param param = new Param();

        param.Add("ticket", userData.ticket);

        UpdateUserData(param);
    }

    public void UpdateUserData_stage()
    {
        Param param = new Param();

        param.Add("stage", userData.stage);

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

    public void UpdateUserData_storeSelectAnimal()
    {
        Param param = new Param();

        param.Add("storeSelectAnimal", userData.storeSelectAnimal);

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

    public void UpdateUserData_lastLogin()
    {
        Param param = new Param();

        param.Add("lastLogin", userData.lastLogin);

        UpdateUserData(param);
    }

    public void UpdateUserData_missionPro()
    {
        Param param = new Param();

        param.Add("missionPro", userData.missionPro);

        UpdateUserData(param);
    }

    public void UpdateUserData_fieldItem()
    {
        Param param = new Param();

        param.Add("fieldItem", userData.fieldItem);

        UpdateUserData(param);
    }

    public void UpdateUserData_storageItem()
    {
        Param param = new Param();

        param.Add("storageItem", userData.storageItem);

        UpdateUserData(param);
    }

    public void UpdateUserData_mergeMission()
    {
        Param param = new Param();

        param.Add("mergeMission", userData.mergeMission);

        UpdateUserData(param);
    }

    public void UpdateUserData_guideMission()
    {
        Param param = new Param();

        param.Add("guideMission", userData.guideMission);

        UpdateUserData(param);
    }

    public void UpdateUserData_getMergeItem()
    {
        Param param = new Param();

        param.Add("getMergeItem", userData.getMergeItem);

        UpdateUserData(param);
    }

     public void UpdateUserData_storeStep()
    {
        Param param = new Param();

        param.Add("storeStep", userData.storeStep);

        UpdateUserData(param);
    }

    private void UpdateUserData(Param param)
    {
        if (userData == null)
        {
            Debug.LogError(" �������� �ٿ�ްų� ���� ������ �����Ͱ� �������� �ʽ��ϴ�.");
        }

        // ���� ������ �������� ������ ���� �޽��� ���
        if (string.IsNullOrEmpty(gameDataRowInDate))
        {
            Debug.LogError("������ inDate ������ ���� ���� ���� ������ ������ �����߽��ϴ�.");
        }
        else
        {
            Debug.Log($"{gameDataRowInDate} �� ���� ���� ������ ������ ��û�մϴ�.");

            Backend.GameData.UpdateV2("UserData", gameDataRowInDate, Backend.UserInDate, param, callback =>
            {
                if (callback.IsSuccess())
                {
                    Debug.Log($"���� ���� ������ ������ �����߽��ϴ�. : {callback}");
                }
                else
                {
                    Debug.LogError($"���� ���� ������ ������ �����߽��ϴ�. : {callback}");
                }
            });
        }
    }

    // ���� �ð�
    public DateTime NowDay()
    {
        BackendReturnObject servertime = Backend.Utils.GetServerTime();

        string time = servertime.GetReturnValuetoJSON()["utcTime"].ToString();
        DateTime parsedDate = DateTime.Parse(time);

        return parsedDate;
    }

    public void SetTestData()
    {
        userData = new UserData();

        userData.ResetUserData();

        userData.earning = 500;
        userData.energy = 100;
        userData.medal = 500;
        userData.gem = 500;
        userData.stage = 0;
        userData.ticket = 0;
        userData.animalBought.Add("ĳ��Ƽ");

        userData.getMergeItem.Add(711);

        userData.fieldItem.Add(0, 21);
    }

    public void Destroy()
    {
        Destroy(this);
    }

}