using System;

namespace POBICOS
{
	/// <summary>
	/// Main program class
	/// </summary>
	/// <remarks>Responsible for starting whole program and 3D application</remarks>
	static class Program
	{
		/// <summary>
		/// The main entry point for application
		/// </summary>
		/// <param name="args">command line arguments</param>
		static void Main(string[] args)
		{
			using (POBICOS game = new POBICOS())
			{
				game.Run();
			}
		}
	}
}

