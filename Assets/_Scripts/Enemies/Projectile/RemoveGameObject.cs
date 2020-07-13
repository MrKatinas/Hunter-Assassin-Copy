using UnityEngine;

/// <summary>
/// TODO refactor to project code culture.
/// </summary>
public class RemoveGameObject : MonoBehaviour {

	public float lifeTime;

	void Start()
	{
		Destroy(gameObject, lifeTime);
	}
}
