using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// Основной класс
/// Содержит ссылки на необходимые сущности
/// Управляет платформой
/// Отслеживает изменение очков игрока
/// Отслеживает поражение
/// </summary>
public class MainController : NetworkBehaviour
{
	public static MainController Instance;

	public float size_x_offset = 3f;
	public float platform_speed = 3f;

	public SphereSpawner spawner;
	public Camera main_camera;
	public Text count_label;
	public Transform platform;
	public GameObject buttons;
	public GameObject collision_particle;
	public GameObject shadow;
	public GameObject stop_client_button;

	Vector3 tmp_vect3;
	int direction = 0;
	float size_x = 10f;
	int points_count = 0;
	bool paused = false;


	void Awake ()
	{
		Instance = this;

		var frustumHeight = platform.localPosition.z * Mathf.Tan (main_camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
		size_x = frustumHeight * main_camera.aspect - size_x_offset;
		spawner.transform.localPosition = new Vector3 (0, frustumHeight, platform.localPosition.z);
		spawner.Init (new Vector2 (size_x, frustumHeight));
		spawner.OnSphereOutOfScreen += SphereOutOfScreenAction;
		spawner.OnSpherePlatformCollisin += SpereOnPlatformAction;
	}

	/// <summary>
	/// Установить направление движения платформы
	/// </summary>
	/// <param name="direction">Direction.</param>
	public void SetMoving (int direction)
	{
		this.direction = direction;
	}

	/// <summary>
	/// Движение платформы в соответствии с зажатой кнопкой
	/// </summary>
	void Update ()
	{
		if (direction == 0)
			return;

		tmp_vect3 = platform.position;
		tmp_vect3.x = Mathf.Clamp (tmp_vect3.x + platform_speed * Time.deltaTime * direction, -size_x, size_x);
		platform.position = tmp_vect3;
	}

	/// <summary>
	/// Включить состояние клиента
	/// </summary>
	public void HideServer ()
	{
		buttons.SetActive (false);
		stop_client_button.SetActive (true);
	}

	/// <summary>
	/// Событие касание платформы сферой 
	/// </summary>
	/// <param name="sender">Sender.</param>
	void SpereOnPlatformAction (SphereScript sender)
	{
		PointsCount++;
		sender.PhysicsLocked (true);
		var particle = (GameObject)Instantiate (collision_particle, sender.transform.position, Quaternion.identity);
		NetworkServer.Spawn (particle);
		Destroy (particle, 1f);
		sender.DestroySphere ();
	}

	/// <summary>
	/// Событие ухода сферы за пределы экрана
	/// </summary>
	/// <param name="sender">Sender.</param>
	void SphereOutOfScreenAction (SphereScript sender)
	{
		Destroy (sender.gameObject);
		NetworkManager.singleton.StopHost ();
		NetworkServer.DisconnectAll ();
	}

	/// <summary>
	/// Счетчик очков
	/// </summary>
	/// <value>The points count.</value>
	public int PointsCount {
		get{ return points_count; }
		set {
			if (!isServer)
				return;
			points_count = value;
			RpcOnPointsChange (points_count);
		}
	}

	/// <summary>
	/// Изменение текста очков для всех клиентов
	/// </summary>
	/// <param name="value">Value.</param>
	[ClientRpc]
	void RpcOnPointsChange (int value)
	{
		count_label.text = string.Format ("Points: {0}", value.ToString ());
	}

	/// <summary>
	/// Вызов паузы на сервере
	/// </summary>
	public void Pause ()
	{
		if (!isServer)
			return;
		paused = !paused;
		RpcPause (paused);
	}

	/// <summary>
	/// Вызов паузы для всех зрителей
	/// </summary>
	/// <param name="value">If set to <c>true</c> value.</param>
	[ClientRpc]
	void RpcPause (bool value)
	{
		Time.timeScale = value ? 0 : 1;
		shadow.SetActive (paused);
	}

	/// <summary>
	/// Отключится от просмотра
	/// </summary>
	public void StopClient ()
	{
		NetworkManager.singleton.StopHost ();
		
	}
}