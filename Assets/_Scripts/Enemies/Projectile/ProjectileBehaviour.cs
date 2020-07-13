using Player;
using UnityEngine;

/// <summary>
/// TODO refactor to project code culture.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ProjectileBehaviour : MonoBehaviour {

	[Header("Movement")]
	public float speed;
	private Rigidbody projectileRigidbody;

	[Header("Life Settings")]
	public float lifeTime;

	[Header("Damage")]
	public int damageToEnemy;

	[Header("Hit Object AVFX")]
	public GameObject hitEnemyParticles;
	public GameObject hitWallParticles;


	void Start()
	{
		projectileRigidbody = GetComponent<Rigidbody>();
		Invoke("RemoveProjectile", lifeTime);
	}
	
	void OnTriggerEnter(Collider theCollider)
	{	
 
		if(theCollider.CompareTag("Player"))
		{
			if(damageToEnemy > 0)
			{
				theCollider.GetComponent<PlayerController>().TakeDamage(1);	
			}

			Instantiate(hitEnemyParticles, transform.position, transform.rotation);
			RemoveProjectile();

		} else if (theCollider.CompareTag("Environment"))
		{
			Instantiate(hitWallParticles, transform.position, transform.rotation);
			RemoveProjectile();
		}

	}
	
	void Update()
	{
			Vector3 movement = transform.forward * speed * Time.deltaTime;
			projectileRigidbody.MovePosition(transform.position + movement);
	}

	void RemoveProjectile()
	{
		Destroy(gameObject);
	}

}
