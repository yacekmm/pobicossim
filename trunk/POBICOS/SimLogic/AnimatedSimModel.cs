using Microsoft.Xna.Framework;
namespace POBICOS.SimLogic
{
	class AnimatedSimModel : SimModel
	{
		/// <summary>
		/// true: key animation, false - bone animation
		/// </summary>
		bool isKeyAnimated;

		public AnimatedSimModel(Game game, string modelPath, EffectList effectToUse, Room room, bool _isKeyAnimated)
			: base(game, modelPath, effectToUse, room)
		{
			this.isKeyAnimated = _isKeyAnimated;
		}
	}
}
