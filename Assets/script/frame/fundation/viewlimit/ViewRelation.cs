using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class ViewRelation
{
    /// <summary>
    /// mediator 之间互斥关系列表
    /// </summary>
    private static List<List<string>> mutualView = new List<List<string>>();

    /// <summary>
    /// 阻止该view打开的view列表，value列表中系统若有打开，则禁止key中ui打开
    /// </summary>
    private static Dictionary<string,List<string>> preventView = new Dictionary<string, List<string>>();

    /// <summary>
    /// 添加互斥关系列表
    /// </summary>
    private static void AddMutualView(List<string> temp1)
    {
        mutualView.Add(temp1);
    }

    /// <summary>
    /// 添加阻止关系
    /// </summary>
    /// <param name="str"></param>
    /// <param name="temp"></param>
    private static void AddpreventView(string str, List<string> temp)
    {
        if(!preventView.ContainsKey(str))
        preventView.Add(str,temp);
    }

    /// <summary>
    /// 注册view关系列表
    /// </summary>
    public static void Inject()
    {
        AddMutualView(new List<string>() { "DesignPanel", "DesignHeroPanel", "WorldMapPanel", "HonoHallPanel" });
        AddMutualView(new List<string>() { "PkzhuExitPanel", "Pkzhu_LostPanel", "PkZhu_revivePanel", "PkZhu_winPanel" });

        AddMutualView(new List<string>() { "MultipleDesignPanel", "DesignPanel", "DesignHeroPanel", "MistyforestPanel" });
        AddMutualView(new List<string>() { "MultipleDesignPanel", "TriedDesignPanel", "DesignHeroPanel", "FriendInvitedPanel" });
        AddMutualView(new List<string>() { "BagPanel", "CKPanel", "EmailPanel", "HonoHallPanel", "MenuPanel", "PetPanel",
            "PVPPanel", "QianDaoPanel", "RankPanel", "JuesePanel", "ShopPanel", "SmithyPanel", "SmShopPanel","ZhuJieMianPanel",
            "TriedDesignPanel" ,"TreasurePanel", "DesignHeroPanel","design_ActivityPanel","MistyforestPanel"});

        AddMutualView(new List<string>() { "BagPanel", "CKPanel", "EmailPanel", "HonoHallPanel", "MenuPanel",
            "PVPPanel", "QianDaoPanel", "RankPanel", "JuesePanel", "ShopPanel", "SmithyPanel", "SmShopPanel","ZhuJieMianPanel",
            "PetCallPanel","PetStarUpPanel","PetAdvancePanel","TriedDesignPanel","TreasurePanel", "DesignHeroPanel",
            "design_ActivityPanel","MistyforestPanel"});

        AddpreventView("ZhuJieMianPanel", new List<string>() {"BagPanel", "CKPanel", "EmailPanel", "HonoHallPanel", "MenuPanel", "PetPanel", 
            "PVPPanel", "QianDaoPanel", "RankPanel", "JuesePanel", "ShopPanel", "SmithyPanel", "SmShopPanel" , "PetCallPanel","PetStarUpPanel",
            "PetAdvancePanel","TriedDesignPanel","MultipleDesignPanel", "DesignPanel","TreasurePanel", "DesignHeroPanel","design_ActivityPanel",
            "MistyforestPanel" });

        AddpreventView("WorldMapPanel", new List<string>() { "DesignPanel", "DesignHeroPanel", "HonoHallPanel"});

    }

    /// <summary>
    /// 获取所有与之互斥的mediator
    /// </summary>
    /// <returns></returns>
    public static List<string> GetMutualPanel(string name)
    {
        List<string> mutuals = new List<string>();
        for (int i = 0; i < mutualView.Count; i++)
        {
            if (mutualView[i].Contains(name))
            {
                List<string> temp = mutualView[i];
                for (int j = 0; j < temp.Count; j++)
                {
                    if (temp[j] != name && !mutuals.Contains(temp[j]))
                    {
                        mutuals.Add(temp[j]);
                    }
                }
            }
        }
        return mutuals;
    }


    /// <summary>
    /// 获取阻止其打开的mediator列表
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static List<string> GetPreventPanel(string name)
    {
        if (preventView.ContainsKey(name))
            return preventView[name];
        return null;
    }

}

