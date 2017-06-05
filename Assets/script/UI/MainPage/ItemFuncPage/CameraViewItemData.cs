using UnityEngine;


public class CameraViewItemData : ItemData {

    public Vector3 position;
    public Vector3 rotate;
    public string imagemeta;
    public bool special;
    public CameraViewItemData(Vector3 position,Vector3 rotate,string imagemeta,bool special = false )
    {
        this.position = position;
        this.rotate = rotate;
        this.imagemeta = imagemeta;
        this.special = special;
    }
    public CameraViewItemData()
    {

    }

}
