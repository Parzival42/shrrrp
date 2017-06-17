using UnityEngine;

public class MapRandomizer : MonoBehaviour
{
	[SerializeField]
	private string[] randomizedMaps;
	
	public string GetRandomMap ()
	{
		return randomizedMaps[Random.Range(0, randomizedMaps.Length)];
	}
}
