using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace POBICOS.Helpers
{
	class InputHelper
	{
		KeyboardState keyboardState, lastKeyboardState;
		PlayerIndex playerIndex;

		public InputHelper(PlayerIndex playerIndex)
		{
			this.playerIndex = playerIndex;
		}

		public void Update()
		{
			lastKeyboardState = keyboardState;
			keyboardState = Keyboard.GetState(playerIndex);
		}

		public bool IsKeyPressed(Keys key)
		{
			return keyboardState.IsKeyDown(key);
		}

		public bool IsKeyJustPressed(Keys key)
		{
			return (keyboardState.IsKeyDown(key) && lastKeyboardState.IsKeyUp(key));
		}
	}
}
