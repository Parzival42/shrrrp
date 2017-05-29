using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveObjectWithParent : DissolveObject {

		
    protected override void CleanUp()
    {
        base.CleanUp();
        Destroy(transform.parent.gameObject);
    }
}
