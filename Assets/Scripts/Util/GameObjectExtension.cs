using UnityEngine;

public static class GameObjectExtension
{
    /// <summary>
    /// Returns the component with the given tag. Returns null if nothing was found.
    /// </summary>
    /// <param name="parent">Parent gameobject in the hierarchy.</param>
    /// <param name="tag">Tag</param>
    /// <returns>First componentn found with the specified tag.</returns>
    public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag) where T : Component
    {
        Transform t = parent.transform;
        foreach (Transform tr in t)
            if (tr.tag == tag)
                return tr.GetComponent<T>();

        return null;
    }
}
