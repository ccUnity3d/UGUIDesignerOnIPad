
public class PointView : ObjView {
    public Point data;

    public override void SetData(ObjData data)
    {
        base.SetData(data);
        this.data = data as Point;
    }

    public override string GetState()
    {
        return SelectPointState.NAME;
    }
}
