public class DissolveObjectWithParent : DissolveObject {
    protected override void CleanUp()
    {
        base.CleanUp();
        Destroy(transform.parent.gameObject);
    }
}
