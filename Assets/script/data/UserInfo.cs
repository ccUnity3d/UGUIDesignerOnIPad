using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserInfo {
    public string company;
    public string createdTime;
    public string email;
    public string isCreatedBy;
    public string lastTime;
    public string name;
    public string newpassword;
    public string phone;
    public string platform;
    public string profile;
    public string status;
    public string updatedTime;
    public string userType;
    public string uuid = "youke001";

    public void DeSerialize(Dictionary<string, object> dic)
    {
        foreach (string key in dic.Keys)
        {
            switch (key)
            {
                case "name":
                    name = MyJsonTool.getStringValue(dic, key);
                    break;
                case "company":
                    company = MyJsonTool.getStringValue(dic, key);
                    break;
                case "createdTime":
                    createdTime = MyJsonTool.getStringValue(dic, key);
                    break;
                case "email":
                    email = MyJsonTool.getStringValue(dic, key);
                    break;
                case "isCreatedBy":
                    isCreatedBy = MyJsonTool.getStringValue(dic, key);
                    break;
                case "lastTime":
                    lastTime = MyJsonTool.getStringValue(dic, key);
                    break;
                case "newpassword":
                    newpassword = MyJsonTool.getStringValue(dic, key);
                    break;
                case "phone":
                    phone = MyJsonTool.getStringValue(dic, key);
                    break;
                case "platform":
                    platform = MyJsonTool.getStringValue(dic, key);
                    break;
                case "profile":
                    profile = MyJsonTool.getStringValue(dic, key);
                    break;
                case "status":
                    status = MyJsonTool.getStringValue(dic, key);
                    break;
                case "updatedTime":
                    updatedTime = MyJsonTool.getStringValue(dic, key);
                    break;
                case "userType":
                    userType = MyJsonTool.getStringValue(dic, key);
                    break;
                case "uuid":
                    uuid = MyJsonTool.getStringValue(dic, key);
                    break;
                default:
                    break;
            }
        }
    }
}
