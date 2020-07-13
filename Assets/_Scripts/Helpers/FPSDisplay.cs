using TMPro;
using UnityEngine;

namespace Helpers
{
	public class FPSDisplay : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _fpsText;

		private float _deltaTime;
	
		private void Update ()
		{
			_deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
			
			DisplayFPS();
		}

		private void DisplayFPS()
		{
			var ms = _deltaTime * 1000.0f;
			var fps = 1.0f / _deltaTime;
			
			_fpsText.text = $"FPS: {fps:00.} ({ms:00.0} ms)";
		}
	}
}
