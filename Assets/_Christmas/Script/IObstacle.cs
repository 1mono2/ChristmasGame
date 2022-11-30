
namespace MoNo.Christmas
{
	public interface IObstacle
	{
		//  interface's valunable enfore coder to write accessor.
		public void OnEnterEvent(SnowBallBehavior1 snowball);
		public void OnStayEvent(SnowBallBehavior1 snowball);
		public void OnExitEvent(SnowBallBehavior1 snowball);
	}
}