using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
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
