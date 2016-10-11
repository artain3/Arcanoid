using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerScript : NetworkBehaviour
{
	public override void OnStartLocalPlayer ()
	{
		if (isServer) {
			MainController.Instance.spawner.StartSpawn ();
		} else {
			MainController.Instance.HideServer ();
		}
	}


}
