using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
//using POBICOS.SimLogic;

namespace POBICOS.SimBase.Effects
{
	public class BasicEffectManager
	{
		public bool Light0Enabled = true;
		public Vector3 Light0DiffuseColor;
		public Vector3 Light0Direction = new Vector3(0, 0, -0.75f);
		public Vector3 Light0SpecularColor = Color.White.ToVector3();

		public bool Light1Enabled = true;
		public Vector3 Light1DiffuseColor;
		public Vector3 Light1Direction = new Vector3(0.5f, -0.5f, 0);
		public Vector3 Light1SpecularColor = Color.Red.ToVector3();

		public bool Light2Enabled = true;
		public Vector3 Light2DiffuseColor;
		public Vector3 Light2Direction = new Vector3(-0.5f, -0.5f, 0);
		public Vector3 Light2SpecularColor;

		public bool preferPerPixelLighting = false;

		public Vector3 AmbientColor = new Vector3(0.2f);

		public Vector3 specularColor = new Vector3(0.3f);
		public float SpecularPower = 16.0f;

		public bool texturesEnabled = false;
		public Texture2D[] textures;
		public int currentTexture = 0, currentTexture2 = 0;
		public string texturedMeshName, texturedMeshName2;

		public bool writeOnObject = false;
		public char[] textToWrite;
		
		public Texture2D[] letters;
		public Hashtable letter;
		public string[] objectsTextured;

		public string TextToWrite
		{
			set
			{
				textToWrite = value.ToLower().ToCharArray();
				//WriteText();
				writeOnObject = true;
			}
		}

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


//    class ModelEffect
//    {
//        Effect effect;

//        #region Properties
//        public Effect Effect
//        {
//            get
//            {
//                return effect;
//            }
//            set
//            {
//                effect = value;
//            }
//        }

//        public Vector3 DiffuseColor
//        {
//            get
//            {
//                return effect.Parameters["diffuseColor"].GetValueVector3();
//            }
//        }
//        public Vector3 SpecularColor
//        {
//            get
//            {
//                return effect.Parameters["specularColor"].GetValueVector3();
//            }
//            set
//            {
//                effect.Parameters["specularColor"].SetValue(value);
//            }
//        }
//        public float SpecularPower
//        {
//            get
//            {
//                return effect.Parameters["specularPower"].GetValueSingle();
//            }
//            set
//            {
//                effect.Parameters["specularPower"].SetValue(value);
//            }
//        }

//        public Vector3 AmbientColor
//        {
//            get
//            {
//                return effect.Parameters["ambientLightColor"].GetValueVector3();
//            }
//            set
//            {
//                effect.Parameters["ambientLightColor"].SetValue(value);
//            }
//        }
//        public float AmbientIntensity
//        {
//            get
//            {
//                return effect.Parameters["ambientLightInt"].GetValueSingle();
//            }
//            set
//            {
//                effect.Parameters["ambientLightInt"].SetValue(value);
//            }
//        }

//        public Vector3 Light1Position
//        {
//            get
//            {
//                return effect.Parameters["light1Position"].GetValueVector3();
//            }
//            set
//            {
//                effect.Parameters["light1Position"].SetValue(value);
//            }
//        }
//        public Vector3 Light1Color
//        {
//            get
//            {
//                return effect.Parameters["light1Color"].GetValueVector3();
//            }
//            set
//            {
//                effect.Parameters["light1Color"].SetValue(value);
//            }
//        }
//        public Vector3 Light1Direction
//        {
//            get
//            {
//                return effect.Parameters["light1Dir"].GetValueVector3();
//            }
//            set
//            {
//                effect.Parameters["light1Dir"].SetValue(value);
//            }
//        }
//        public Vector3 Light2Position
//        {
//            get
//            {
//                return effect.Parameters["light2Position"].GetValueVector3();
//            }
//            set
//            {
//                effect.Parameters["light2Position"].SetValue(value);
//            }
//        }
//        public Vector3 Light2Color
//        {
//            get
//            {
//                return effect.Parameters["light2Color"].GetValueVector3();
//            }
//            set
//            {
//                effect.Parameters["light2Color"].SetValue(value);
//            }
//        }

//        public Vector3 EyePosition
//        {
//            get
//            {
//                return effect.Parameters["eyePosition"].GetValueVector3();
//            }
//            set
//            {
//                effect.Parameters["eyePosition"].SetValue(value);
//            }
//        }
//        #endregion

//        public ModelEffect(Effect effect)
//        {
//            this.effect = effect;
//            setDefaultValues();
//        }

//        private void setDefaultValues()
//        {
//            SpecularColor = new Vector3(0.5f);
//            SpecularPower = 32.0f;

//            AmbientColor = new Vector3(1.0f);
//            AmbientIntensity = 0.1f;

//            Light1Color = new Vector3(0.7f);
//            Light1Position = new Vector3(7, 121, -5);
//            Light1Direction = Vector3.Zero;
			
//            Light2Color = new Vector3(0.8f);
//            Light2Position = new Vector3(1.0f, 220.1f, -1.0f);
//        }
//    }
//}
