using UnityEngine;

public class DissolveManual : DissolveObject
{
    private float dissolveScale = 1f;

	protected override void Start ()
    {
		// Nothing to do here
	}

    public void DissolveObject(float dissolveTime, float dissolveScale)
    {
        PrepareDissolve(dissolveTime);
    }

    protected override void PrepareDissolve(float dissolveTime, string material)
    {
        base.PrepareDissolve(dissolveTime, material);
        dissolveMaterial.SetFloat("_DissolveScale", dissolveScale);
    }
}
