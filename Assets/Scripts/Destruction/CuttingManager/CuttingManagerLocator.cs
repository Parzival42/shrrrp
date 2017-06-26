
using UnityEngine;

public class CuttingManagerLocator : MonoBehaviour {

	#region variable
	private static CuttingManager cuttingManager;
	#endregion

	#region methods
	public static void Provide(CuttingManager newCuttingManager){
		cuttingManager = newCuttingManager;
	}
	public static CuttingManager GetInstance{
		get{
			if(cuttingManager==null){
				cuttingManager = new NullProvider();
			}
			
			return cuttingManager;
		}
	}

	public void OnDestroy(){
		cuttingManager = null;
	}

	#endregion
}
