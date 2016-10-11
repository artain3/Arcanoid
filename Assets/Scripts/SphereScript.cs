using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class SphereScript : NetworkBehaviour
{
	public event Action<SphereScript> OnPlatformCollisin, OnOutOfScreen;

	public float speed = 2f;
	public float life_time = 3;
	public float max_rotaion;
	public float bonus_speed = 0;

	Vector3 rotation;
	Rigidbody rigid;
	bool destoyed_falg = false;

	void Start ()
	{
		rigid = GetComponent<Rigidbody> ();
		rigid.AddForce (new Vector3 (Random.Range (-10f, 10f), -bonus_speed, 0),	ForceMode.Impulse);
		rotation = new Vector3 (Random.Range (-max_rotaion, max_rotaion), Random.Range (-max_rotaion, max_rotaion), 0);
	}

	void Update ()
	{
		if (destoyed_falg)
			return;
		
		transform.Rotate (rotation * Time.deltaTime);
	}

	/// <summary>
	/// Отключить Rigidbody для избежания коллизий
	/// </summary>
	/// <param name="value">If set to <c>true</c> value.</param>
	public void PhysicsLocked (bool value)
	{
		rigid.useGravity = !value;
		rigid.isKinematic = value;
	}

	void OnCollisionEnter (Collision col)
	{
		if (col.collider.CompareTag ("Player") && OnPlatformCollisin != null)
			OnPlatformCollisin (this);
		
		if (col.collider.CompareTag ("Death") && OnOutOfScreen != null)
			OnOutOfScreen (this);
		
	}

	/// <summary>
	/// Красивое самоуничтожение
	/// </summary>
	public void DestroySphere ()
	{
		destoyed_falg = true;
		StartCoroutine (crt_Destruction ());
	}

	IEnumerator crt_Destruction ()
	{
		float time_to_destroy = .6f;
		float timer = time_to_destroy;
		Vector3 rotation_vector = new Vector3 (max_rotaion, max_rotaion, 0);
		Vector3 vect_one = Vector3.one;
		while (timer > 0) {
			timer -= Time.deltaTime;
			transform.Rotate (rotation_vector * Time.deltaTime);
			transform.localScale = vect_one * (timer / time_to_destroy);
			yield return null;
		}
		Destroy (gameObject);
	}
}
