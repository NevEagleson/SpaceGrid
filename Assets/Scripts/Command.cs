using UnityEngine;
using UnityEngine.UI;

public class Command : MonoBehaviour {

	private string mText;

	public string Text
	{
		get { return mText; }
		set
		{
			mText = value;
			Text[] texts = GetComponentsInChildren<Text>();
			foreach (Text t in texts)
				t.text = mText;
		}

	}
}
