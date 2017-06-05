using UnityEngine;
using System.Collections;

public class SelectGoods3D_CollectState : SelectGoods3D_State
{
    public override void enter()
    {
        base.enter();
        MsgToIOS msg = new MsgToIOS();
        msg.code = "101008";
        MsgToIOS.InfoToIOS info = new MsgToIOS.InfoToIOS();
        info.goodsId = target.seekId;
        info.image = targetProduct.tempTexture;
        info.collectType = 1;/*data.isNew == true ? 1 : 0;*/
        msg.info = info;
        UnityIOSMsg.sendToIOS(msg);
    }

    public override void exit()
    {
        base.exit();
    }
}
