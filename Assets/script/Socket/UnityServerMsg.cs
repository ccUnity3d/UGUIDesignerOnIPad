using System.Collections.Generic;

[System.Serializable]
public class UnityServerMsg{
    /// <summary>
    /// 自定义显式转换
    /// </summary>
    /// <param name="json"></param>
    public static explicit operator UnityServerMsg(Dictionary<string, object> obj)
    {
        UnityServerMsg data = new UnityServerMsg();
        data.Deserialize(obj);
        return data;
    }

    private void Deserialize(Dictionary<string, object> obj)
    {
        
    }
    //自定义隐式转换
    //public static implicit operator MsgFromIOS(UnityEngine.Object v)
    //{
    //    throw new NotImplementedException();
    //}


    public string code;
    public ServerInfo info;

    public class ServerInfo
    {

    }

}
