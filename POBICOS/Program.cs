using System;

namespace POBICOS
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			using (POBICOS game = new POBICOS())
			{
				game.Run();
			}
		}
	}
}

