using Microsoft.Xna.Framework;
namespace POBICOS.SimLogic
{
	class AnimatedSimModel : SimModel
	{
		/// <summary>
		/// true: key animation, false - bone animation
		/// </summary>
		bool isKeyAnimated;

		public AnimatedSimModel(Game game, string modelPath, EffectList effectToUse, bool _isKeyAnimated)
			: base(game, modelPath, effectToUse)
		{
			this.isKeyAnimated = _isKeyAnimated;
		}
	}
}
