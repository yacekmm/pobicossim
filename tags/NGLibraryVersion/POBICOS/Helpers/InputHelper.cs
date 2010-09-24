using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace POBICOS.Helpers
{
	/// <summary>
	/// Handles keyboard input
	/// </summary>
	class InputHelper
	{
		/// <summary>Logs Keyboard state</summary>
		KeyboardState keyboardState, lastKeyboardState;

		/// <summary>index of player for which input is checked</summary>
		PlayerIndex playerIndex;

		/// <summary>
		/// <o>InputHelper</o> constructor
		/// </summary>
		/// <param name="playerIndex">index of player for which input is checked</param>
		public InputHelper(PlayerIndex playerIndex)
		{
			this.playerIndex = playerIndex;
		}

		/// <summary>
		/// Update keyboard state
		/// </summary>
		public void Update()
		{
			lastKeyboardState = keyboardState;
			keyboardState = Keyboard.GetState(playerIndex);
		}

		/// <summary>
		/// Checks if key is pressed
		/// </summary>
		/// <param name="key">checked key</param>
		/// <returns><c>true</c> if key is pressed, else returns <c>false</c></returns>
		public bool IsKeyPressed(Keys key)
		{
			return keyboardState.IsKeyDown(key);
		}

		/// <summary>
		/// Checks if key was pressed (pressed and released)
		/// </summary>
		/// <param name="key">checked key</param>
		/// <returns><c>true</c> if key was pressed, else returns <c>false</c></returns>
		public bool IsKeyJustPressed(Keys key)
		{
			return (keyboardState.IsKeyDown(key) && lastKeyboardState.IsKeyUp(key));
		}
	}
}
