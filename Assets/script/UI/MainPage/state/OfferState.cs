
public class OfferState : MainPageState {

    public const string Name = "OfferState";
    private SchemePageController schemeController
    {
        get
        {
            return SchemePageController.Instance;
        }
    }

    public override void enter()
    {
        base.enter();
        if (inputMachine.currentInputIs2D)
        {
            inputMachine.setState(FreeState2D.NAME);
        }
        else
        {
            inputMachine.setState(FreeState3D.NAME);
        }
        schemeController.OnOpenOffer(true);
    }

    public override void Ready()
    {
        base.Ready();
    }

    public override void exit()
    {
        base.exit();
    }
}
