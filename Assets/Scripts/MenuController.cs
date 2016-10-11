using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;

public class MenuController : MonoBehaviour
{

	[SerializeField] GameObject input_field_parent;
	[SerializeField] InputField input_field;
	[SerializeField] Text err_label;

	void Start ()
	{
		var se = new InputField.SubmitEvent ();
		se.AddListener (TryConnect);
		input_field.onEndEdit = se;
	}

	public void StartServer ()
	{
		NetworkManager.singleton.StartHost ();
	}

	public void Exit ()
	{
		Application.Quit ();
	}

	public void WatchClick ()
	{
		if (input_field_parent.activeSelf) {
			TryConnect (input_field.text);
			return;
		}
		
		err_label.gameObject.SetActive (false);	
		input_field_parent.SetActive (true);
	}

	public void TryConnect (string ip_string)
	{
		err_label.gameObject.SetActive (true);
		if (!IpIsValid (ip_string)) {
			err_label.text = "IP isn't valid";
			return;
		}


		err_label.text = "Connecting...";
		NetworkManager.singleton.networkAddress = ip_string;
		NetworkManager.singleton.StartClient ();
	}

	public bool IpIsValid (string ip_string)
	{
		if (string.IsNullOrEmpty (ip_string))
			return false;
		
		string[] splitValues = ip_string.Split ('.');
		if (splitValues.Length != 4)
			return false;

		byte tempForParsing;

		return splitValues.All (r => byte.TryParse (r, out tempForParsing));
	}
}
