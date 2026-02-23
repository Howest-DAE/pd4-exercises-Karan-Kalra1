using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerMessageBubble : NetworkBehaviour
{
	[SerializeField]
	private GameObject _rootGameObject;

	[SerializeField]
	private TMP_Text _messageText;

	private void Awake()
	{
		_rootGameObject.SetActive(false);
	}

	public void ShowMessage(string message)
	{
		_messageText.text = message;
		_rootGameObject.SetActive(true);
		StopAllCoroutines();
		StartCoroutine(ShowAnimation());
	}


	//Movement executed on the server
	[Rpc(SendTo.Owner)]
    public void ShowMessageRpc(string message)
	{
		ShowMessage(message);
	}

        IEnumerator ShowAnimation()
	{
		_rootGameObject.transform.localScale = Vector3.zero;

		//Show animation
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime;
			float tSmoothed = Easing.OutElastic(t);

			_rootGameObject.transform.localScale = Vector3.one * tSmoothed;
			yield return null;
		}

		_rootGameObject.transform.localScale = Vector3.one;

		yield return new WaitForSeconds(2f); // show for 2 seconds

		//Hide animation
		t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime;
			float tSmoothed = 1f - Easing.OutCubic(t);

			_rootGameObject.transform.localScale = Vector3.one * tSmoothed;

			yield return null;
		}
		_rootGameObject.transform.localScale = Vector3.zero;

		_rootGameObject.SetActive(false);
	}


	//Easing functions micro class, TIP: For full production, use full easing library instead
	//Source: https://gist.github.com/Kryzarel/bba64622057f21a1d6d44879f9cd7bd4
	private class Easing {
		public static float InElastic(float t) => 1 - OutElastic(1 - t);
		public static float OutElastic(float t)
		{
			float p = 0.3f;
			return (float)Mathf.Pow(2, -10 * t) * (float)Mathf.Sin((t - p / 4) * (2 * Mathf.PI) / p) + 1;
		}
		public static float InCubic(float t) => t * t * t;
		public static float OutCubic(float t) => 1 - InCubic(1 - t);
	}


}
