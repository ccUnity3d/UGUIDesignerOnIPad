using UnityEngine;
using System.Collections;

public class SetController : UIController<SetController> {

    private SettingPage set;
    public SetController()
    {
        panel = set = SettingPage.Instance;
    }

    public override void ready()
    {
       

    }
   
}
