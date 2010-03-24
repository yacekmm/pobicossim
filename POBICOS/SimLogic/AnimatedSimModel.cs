using Microsoft.Xna.Framework;
namespace POBICOS.SimLogic
{
	class AnimatedSimModel : SimModel
	{
		/// <summary>
		/// true: key animation, false - bone animation
		/// </summary>
		bool isKeyAnimated;

		public AnimatedSimModel(Game game, string modelPath, bool _isKeyAnimated)
			: base(game, modelPath)
		{
			this.isKeyAnimated = _isKeyAnimated;
		}
	}
}
