public enum CameraStateType
{
    Camera2D,
    Camera3DFree,
    Camera3DFollow,
}

public enum InputWallType
{
    Mid,
    Side
}

public enum GoodsType
{
    //公共部分 选中状态分2D 和 3D
    Default,
    Product,//产品
    Door,//门
    Window,//窗

    //2D部分 选中状态只有2D
    Point,//墙顶点
    Wall,//墙
    Room,//房间  选中房间换贴图时候视图进入3D 并且 地板被换掉

    //3D部分 选中状态只有3D
    WallSide,//墙的一边
    Ceiling,//天花板
    Floor,//地板
}

public enum SceneType
{
    Is2D,
    Is3D
}

/// <summary>
/// 选中物体的编辑
/// </summary>
public class EditTypeOnSelect
{
    public const string Free = "Free";
    public const string Detail = "Detail";
    public const string Copy = "Copy";
    public const string MirroredX = "MirroredX";
    public const string MirroredY = "MirroredY";
    public const string Replace = "Replace";
    public const string DisWall = "Distance";
    public const string DisFloor = "DisFloor";    
    public const string Group = "Group";
    public const string Delet = "Delet";
    public const string Collect = "Collect";
    public const string Hide = "Hide";

    public const string Rotate = "Rotate";
    public const string MoveUp = "MoveUp";

    public const string WallWidth = "WallWidth";
    public const string WallLength = "WallLength";

    public const string SetRoomType = "SetRoomType";
    public const string UseToAllSide = "UseToAllSide";
    public const string SetTLine = "SetTLine";
    public const string SetTopLine = "SetTopLine";
}

