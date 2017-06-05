using UnityEngine;
using System.Collections;

public class SelectWallSide_UseToAllSideState : SelectWallSide_State {
    private OptionsPage optionsPage { get { return OptionsPage.Instance; } }

    private OriginalInputData data
    {
        get {
            return OriginalInputData.Instance;
        }
    }
    public override void Ready(GameObject skin)
    {
        base.Ready(skin);   
    }
    public override void enter()
    {
        base.enter();
        RoomData room = data.GetRoom(target);
        if (room == null) return;
        Save();
        for (int i = 0; i < room.sideList.Count; i++)
        {
            WallSideData side = room.sideList[i];
            side.tBaseboard.hide = target.tBaseboard.hide;
            side.tBaseboard.type = target.tBaseboard.type;
            side.tBaseboard.width = target.tBaseboard.width;
            side.tBaseboard.height = target.tBaseboard.height;
            side.tBaseboard.disRoot = target.tBaseboard.disRoot;
            MaterialData tMaterialData = side.tBaseboard.materialData;
            if (target.tBaseboard.materialData != null)
            {
                if(tMaterialData == null) tMaterialData = new MaterialData();
                tMaterialData.textureURI = target.tBaseboard.materialData.textureURI;
                tMaterialData.id = target.tBaseboard.materialData.id;
                tMaterialData.guid = target.tBaseboard.materialData.guid;
                tMaterialData.seekId = target.tBaseboard.materialData.seekId;
                tMaterialData.color = target.tBaseboard.materialData.color;
                tMaterialData.tileSize_x = target.tBaseboard.materialData.tileSize_x;
                tMaterialData.tileSize_y = target.tBaseboard.materialData.tileSize_y;
                tMaterialData.rotation = target.tBaseboard.materialData.rotation;
                tMaterialData.offsetX = target.tBaseboard.materialData.offsetX;
                tMaterialData.offsetY = target.tBaseboard.materialData.offsetY;
                
            }
            else
            {
                tMaterialData = null;
            }
            side.tBaseboard.materialData = tMaterialData;

            side.tBaseboard.index = target.tBaseboard.index;

            //top
            side.topBaseboard.hide = target.topBaseboard.hide;
            side.topBaseboard.type = target.topBaseboard.type;
            side.topBaseboard.width = target.topBaseboard.width;
            side.topBaseboard.height = target.topBaseboard.height;
            side.topBaseboard.disRoot = target.topBaseboard.disRoot;
            MaterialData topMaterialData = side.topBaseboard.materialData;
            if (target.topBaseboard.materialData != null)
            {
                if (topMaterialData == null) topMaterialData = new MaterialData();
                topMaterialData.textureURI = target.topBaseboard.materialData.textureURI;
                topMaterialData.id = target.topBaseboard.materialData.id;
                topMaterialData.guid = target.topBaseboard.materialData.guid;
                topMaterialData.seekId = target.topBaseboard.materialData.seekId;
                topMaterialData.color = target.topBaseboard.materialData.color;
                topMaterialData.tileSize_x = target.topBaseboard.materialData.tileSize_x;
                topMaterialData.tileSize_y = target.topBaseboard.materialData.tileSize_y;
                topMaterialData.rotation = target.topBaseboard.materialData.rotation;
                topMaterialData.offsetX = target.topBaseboard.materialData.offsetX;
                topMaterialData.offsetY = target.topBaseboard.materialData.offsetY;

            }
            else
            {
                topMaterialData = null;
            }
            side.topBaseboard.materialData = topMaterialData;

            side.topBaseboard.index = target.topBaseboard.index;
        }
        RefreshView();
    }
    public override void exit()
    {
        base.exit();
    }
}
