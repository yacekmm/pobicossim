using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Diagnostics;

namespace POBICOS.SimBase.Effects
{
	/// <summary>
	/// Manages effects for 3D model it is associated with
	/// </summary>
	/// <remarks>All <o>BasicEffect</o> parameters can be managed using this class. Moreover it introduces extended functionality 
	/// such as custom mesh texturing, or centralized lights management</remarks>
	public class BasicEffectManager
	{
		/// <summary>indicates if Light0 is turned on or off</summary>
		public bool Light0Enabled = true;
		/// <summary>Light0 Diffuse Color</summary>
		public Vector3 Light0DiffuseColor;
		/// <summary>Light0 Direction</summary>
		public Vector3 Light0Direction = new Vector3(0, 0, -0.75f);
		/// <summary>Light0 Specular Color</summary>
		public Vector3 Light0SpecularColor = Color.White.ToVector3();

		/// <summary>indicates if Light1 is turned on or off</summary>
		public bool Light1Enabled = true;
		/// <summary>Light1 Diffuse Color</summary>
		public Vector3 Light1DiffuseColor;
		/// <summary>Light1 Direction</summary>
		public Vector3 Light1Direction = new Vector3(0.5f, -0.5f, 0);
		/// <summary>Light1 Specular Color</summary>
		public Vector3 Light1SpecularColor = Color.Red.ToVector3();

		/// <summary>indicates if Light2 is turned on or off</summary>
		public bool Light2Enabled = true;
		/// <summary>Light2 Diffuse Color</summary>
		public Vector3 Light2DiffuseColor;
		/// <summary>Light2 Direction</summary>
		public Vector3 Light2Direction = new Vector3(-0.5f, -0.5f, 0);
		/// <summary>Light2 Specular Color</summary>
		public Vector3 Light2SpecularColor;

		/// <summary>Gets or sets a value indicating that per-pixel lighting should be used if it is available
		/// for the current adapter. Per-pixel lighting is available if a graphics adapter supports Pixel Shader Model 2.0.</summary>
		public bool preferPerPixelLighting = false;

		/// <summary>Gets or sets the ambient light color of this effect.</summary>
		public Vector3 AmbientColor = new Vector3(0.2f);

		/// <summary>Gets or sets the specular color of this effect material.</summary>
		public Vector3 specularColor = new Vector3(0.3f);
		/// <summary>Gets or sets the specular power of this effect material.</summary>
		public float SpecularPower = 16.0f;

		/// <summary>Enables textures for this effect.</summary>
		public bool texturesEnabled = false;

		/// <summary>Textures collection.</summary>
		public Texture2D[] textures;
		/// <summary>Texture from <c>textures</c> to be applied on mesh</summary>
		public int currentTexture = 0, currentTexture2 = 0;
		/// <summary>mesh name where texture will be applied</summary>
		public string texturedMeshName, texturedMeshName2;

		/// <summary>enables witing on object</summary>
		public bool writeOnObject = false;
		/// <summary>text to be written on object</summary>
		public char[] textToWrite;
		
		/// <summary>letters used to write text on objects</summary>
		public Texture2D[] letters;
		/// <summary>letters that has to be written on objectusing textures from <c>letters</c></summary>
		public Hashtable letter;
		/// <summary>a collection of meshes that will be textured with letters</summary>
		public string[] objectsTextured;

		/// <summary>Sets text to write on object and turns on possibility to write on it</summary>
		public string TextToWrite
		{
			set
			{
				textToWrite = value.ToLower().ToCharArray();
				writeOnObject = true;
				if(POBICOS.enablePerformanceLog)
					Trace.TraceInformation("Performance;" + (DateTime.Now - POBICOS.timeStarted) + ";Message displayed on screen;" + value);
			}
		}

		/// <summary>
		/// <o>BasicEffectManager</o> constructor
		/// </summary>
		/// <remarks>loads all letter and number containing textures and stores textured meshes names</remarks>
		/// <param name="game"><o>Game</o> parameter where actions will be performed</param>
		public BasicEffectManager(Game game)
		{
			letter = new Hashtable();
			letter.Add("0", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-0"));
			letter.Add("a", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-a"));
			letter.Add("b", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-b"));
			letter.Add("c", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-c"));
			letter.Add("d", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-d"));
			letter.Add("e", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-e"));
			letter.Add("f", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-f"));
			letter.Add("g", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-g"));
			letter.Add("h", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-h"));
			letter.Add("i", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-i"));
			letter.Add("j", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-j"));
			letter.Add("k", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-k"));
			letter.Add("l", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-l"));
			letter.Add("m", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-m"));
			letter.Add("n", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-n"));
			letter.Add("o", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-o"));
			letter.Add("p", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-p"));
			letter.Add("q", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-q"));
			letter.Add("r", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-r"));
			letter.Add("s", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-s"));
			letter.Add("t", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-t"));
			letter.Add("u", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-u"));
			letter.Add("v", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-v"));
			letter.Add("w", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-w"));
			letter.Add("x", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-x"));
			letter.Add("y", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-y"));
			letter.Add("z", game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-z"));

			letters = new Texture2D[27];
			letters[0] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-0");
			letters[1] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-a");
			letters[2] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-b");
			letters[3] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-c");
			letters[4] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-d");
			letters[5] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-e");
			letters[6] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-f");
			letters[7] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-g");
			letters[8] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-h");
			letters[9] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-i");
			letters[10] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-j");
			letters[11] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-k");
			letters[12] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-l");
			letters[13] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-m");
			letters[14] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-n");
			letters[15] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-o");
			letters[16] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-p");
			letters[17] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-q");
			letters[18] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-r");
			letters[19] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-s");
			letters[20] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-t");
			letters[21] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-u");
			letters[22] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-v");
			letters[23] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-w");
			letters[24] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-x");
			letters[25] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-y");
			letters[26] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-z");

			objectsTextured = new String[16];
			objectsTextured[0] = "Letter00";
			objectsTextured[1] = "Letter01";
			objectsTextured[2] = "Letter02";
			objectsTextured[3] = "Letter03";
			objectsTextured[4] = "Letter04";
			objectsTextured[5] = "Letter05";
			objectsTextured[6] = "Letter06";
			objectsTextured[7] = "Letter07";
			objectsTextured[8] = "Letter08";
			objectsTextured[9] = "Letter09";
			objectsTextured[10] = "Letter10";
			objectsTextured[11] = "Letter11";
			objectsTextured[12] = "Letter12";
			objectsTextured[13] = "Letter13";
			objectsTextured[14] = "Letter14";
			objectsTextured[15] = "Letter15";
		}
	}
}