namespace POBICOS
{
	/// <summary>
	/// Relative assets paths.
	/// </summary>
	/// <remarks>
	/// Contains a set of <o>string</o> constants pointing on an asset source file location (3D models, textures, XML files).
	/// </remarks>
	public static class SimAssetsPath
	{
		/// <summary>
		/// Relative path to directory containing general 3D models.
		/// </summary>
		public static string MODELS_PATH = @"Models/";

		/// <summary>
		/// Relative path to directory containing 3D models connected with building creation (i.e. walls, doors).
		/// </summary>
		public static string MODELS_BUILDING_PATH = @"Building/";

		/// <summary>
		/// Relative path to directory containing furniture 3D models.
		/// </summary>
		public static string MODELS_FURNITURE_PATH = @"Furniture/";

		/// <summary>
		/// Relative path to directory containing 3D models presenting enviroment around the house (i.e. sky, ground)
		/// </summary>
        public static string MODELS_ENVIRONMENT_PATH = @"Building/Environment/";

		/// <summary>
		/// Relative path to directory containing textures.
		/// </summary>
		public static string TEXTURES_PATH = @"Textures/";

		/// <summary>
		/// Relative path to directory containing font textures.
		/// </summary>
		public static string TEXTURES_FONT_PATH = @"Textures/font/";

		/// <summary>
		/// Relative path to directory containing POBICOS objects definitions in XML files.
		/// </summary>
		public static string POBICOS_OBJECTS_PATH = @"./Resources/PobicosObjects/";
	}
}
