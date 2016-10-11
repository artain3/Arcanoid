using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class SphereSpawner : NetworkBehaviour
{
	public event Action<SphereScript> OnSpherePlatformCollisin, OnSphereOutOfScreen;

	public GameObject shphere_prefab;
	public float spawn_delay = 2f;
	public float time_step = 30f;
	public float bonus_speed = 2f;

	bool spawn = false;
	int lemon_total = 0;
	Vector2 screen_size;
	float time_left = 0;

	public void Init (Vector2 screen_size)
	{
		this.screen_size = screen_size;
	}

	public void StartSpawn ()
	{
		spawn = true;
		StartCoroutine (crt_Spawn ());
	}

	public void StopSpawn ()
	{
		spawn = false;
	}

	IEnumerator crt_Spawn ()
	{
		while (spawn) {
			yield return new WaitForSeconds (spawn_delay);
			time_left += spawn_delay;
			var sphere = (GameObject)Instantiate (shphere_prefab, transform.position + RandomXOffset, Quaternion.identity);
			sphere.transform.SetParent (transform);
			sphere.name = "Shere " + lemon_total.ToString ();

			var sphere_script = sphere.GetComponent<SphereScript> ();
			sphere_script.OnOutOfScreen += (sender) => {
				if (OnSphereOutOfScreen != null)
					OnSphereOutOfScreen (sender);
			};
			sphere_script.OnPlatformCollisin += (sender) => {
				if (OnSpherePlatformCollisin != null)
					OnSpherePlatformCollisin (sender);
			};
			float speed_mul = Mathf.Floor (time_left / time_step);
			sphere_script.bonus_speed = speed_mul * bonus_speed;

			NetworkServer.Spawn (sphere);
		}
	}

	/// <summary>
	/// Случайное смещение по ширине экрана
	/// </summary>
	/// <value>The random X offset.</value>
	Vector3 RandomXOffset {
		get{ return new Vector3 (Random.Range (-screen_size.x, screen_size.x), 0, 0); }
	}
}
