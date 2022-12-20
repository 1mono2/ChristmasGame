
namespace MoNo.Christmas
{
	public interface IObstacle
	{
		//  interface's valunable enfore coder to write accessor.
		public void OnEnterEvent(SnowBallBehavior snowball);
		public void OnStayEvent(SnowBallBehavior snowball);
		public void OnExitEvent(SnowBallBehavior snowball);
	}
}