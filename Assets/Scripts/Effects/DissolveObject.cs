using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveObject : MonoBehaviour {

	public float dissolveTime = 1.0f;
	public LeanTweenType dissolveTweenType = LeanTweenType.easeOutSine;
	Material dissolveMaterial;
	Renderer rend;

	void Start () {
		PrepareDissolve(dissolveTime);
	}

	protected void PrepareDissolve(float dissolveTime){
		 dissolveMaterial = new Material(Resources.Load("DissolveMaterial", typeof(Material)) as Material);

		 rend = GetComponent<Renderer>();
		 rend.material.EnableKeyword("_ALPHATEST_ON");
		 rend.material.EnableKeyword("_EMISSION");
		 dissolveMaterial.color = rend.material.color;
		 dissolveMaterial.SetTexture("_MainTex", rend.material.GetTexture("_MainTex"));
		 dissolveMaterial.SetTexture("_BumpMap", rend.material.GetTexture("_BumpMap"));
		 dissolveMaterial.SetTexture("_MetallicGlossMap", rend.material.GetTexture("_MetallicGlossMap"));
		 //dissolveMaterial.SetFloat("_BumpScale", rend.material.GetFloat("_BumpScale"));
		 dissolveMaterial.SetFloat("_Metallic", rend.material.GetFloat("_Metallic"));
		 dissolveMaterial.SetFloat("_Glossiness", rend.material.GetFloat("_Glossiness"));
		 rend.material = dissolveMaterial;

		 LeanTween.value(gameObject, Dissolve, 0f, 1f, dissolveTime).setEase(dissolveTweenType).setOnComplete(() => {
			CleanUp();
		 });
	}

    protected virtual void CleanUp()
    {
        Destroy(gameObject);
    }

	private void Dissolve( float val ){
		rend.material.SetFloat("_DissolvePercentage", val);
	}
}
