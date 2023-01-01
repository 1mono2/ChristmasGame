using UnityEngine;

namespace MoNo.Christmas
{
	public class GenerateConfetti: MonoBehaviour, IObstacle

	{

		[SerializeField] private ParticleSystem _confettiPrefab;


		public void OnEnterEvent(SnowBallBehavior snowball)
		{
			var confetti = Instantiate(_confettiPrefab, transform.position, _confettiPrefab.transform.rotation);
		}

		public void OnStayEvent(SnowBallBehavior snowball)
		{
		}

		public void OnExitEvent(SnowBallBehavior snowball)
		{
		}
	}
}