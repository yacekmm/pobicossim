﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using POBICOS.SimLogic;

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
	}


	class ModelEffect
	{
		Effect effect;

		#region Properties
		public Effect Effect
		{
			get
			{
				return effect;
			}
			set
			{
				effect = value;
			}
		}

		public Vector3 DiffuseColor
		{
			get
			{
				return effect.Parameters["diffuseColor"].GetValueVector3();
			}
		}
		public Vector3 SpecularColor
		{
			get
			{
				return effect.Parameters["specularColor"].GetValueVector3();
			}
			set
			{
				effect.Parameters["specularColor"].SetValue(value);
			}
		}
		public float SpecularPower
		{
			get
			{
				return effect.Parameters["specularPower"].GetValueSingle();
			}
			set
			{
				effect.Parameters["specularPower"].SetValue(value);
			}
		}

		public Vector3 AmbientColor
		{
			get
			{
				return effect.Parameters["ambientLightColor"].GetValueVector3();
			}
			set
			{
				effect.Parameters["ambientLightColor"].SetValue(value);
			}
		}
		public float AmbientIntensity
		{
			get
			{
				return effect.Parameters["ambientLightInt"].GetValueSingle();
			}
			set
			{
				effect.Parameters["ambientLightInt"].SetValue(value);
			}
		}

		public Vector3 Light1Position
		{
			get
			{
				return effect.Parameters["light1Position"].GetValueVector3();
			}
			set
			{
				effect.Parameters["light1Position"].SetValue(value);
			}
		}
		public Vector3 Light1Color
		{
			get
			{
				return effect.Parameters["light1Color"].GetValueVector3();
			}
			set
			{
				effect.Parameters["light1Color"].SetValue(value);
			}
		}
		public Vector3 Light1Direction
		{
			get
			{
				return effect.Parameters["light1Dir"].GetValueVector3();
			}
			set
			{
				effect.Parameters["light1Dir"].SetValue(value);
			}
		}
		public Vector3 Light2Position
		{
			get
			{
				return effect.Parameters["light2Position"].GetValueVector3();
			}
			set
			{
				effect.Parameters["light2Position"].SetValue(value);
			}
		}
		public Vector3 Light2Color
		{
			get
			{
				return effect.Parameters["light2Color"].GetValueVector3();
			}
			set
			{
				effect.Parameters["light2Color"].SetValue(value);
			}
		}

		public Vector3 EyePosition
		{
			get
			{
				return effect.Parameters["eyePosition"].GetValueVector3();
			}
			set
			{
				effect.Parameters["eyePosition"].SetValue(value);
			}
		}
		#endregion

		public ModelEffect(Effect effect)
		{
			this.effect = effect;
			setDefaultValues();
		}

		private void setDefaultValues()
		{
			SpecularColor = new Vector3(0.5f);
			SpecularPower = 32.0f;

			AmbientColor = new Vector3(1.0f);
			AmbientIntensity = 0.1f;

			Light1Color = new Vector3(0.7f);
			Light1Position = new Vector3(7, 121, -5);
			Light1Direction = Vector3.Zero;
			
			Light2Color = new Vector3(0.8f);
			Light2Position = new Vector3(1.0f, 220.1f, -1.0f);

			//EyePosition = game.Services.GetService(typeof(CameraManager)) as CameraManager;
		}














		//// List all the techniques present in this effect
		//public enum Techniques
		//{
		//    AnimatedModel = 0
		//};

		//// Effect file name
		//public static string EFFECT_FILE_NAME = SimAssetsPath.EFFECTS_PATH + "Model";

		//Effect effect;

		//// Store this matrices to help calculating auxiliar matrices (like World * View * Projection)
		//Matrix worldMatrix;
		//Matrix viewMatrix;
		//Matrix projectionMatrix;

		//// Effect parameters - Matrices
		//EffectParameter worldParam;
		//EffectParameter viewParam;
		//EffectParameter viewInverseParam;
		//EffectParameter worldViewParam;
		//EffectParameter worldViewProjectionParam;

		////Material Params
		//EffectParameter diffuseColorParam;
		//EffectParameter specularColorParam;
		//EffectParameter specularPowerParam;

		////Lights
		//EffectParameter ambientLightColorParam;
		//EffectParameter light1PositionParam;
		//EffectParameter light1ColorParam;
		//EffectParameter light2PositionParam;
		//EffectParameter light2ColorParam;
		
		//#region Properties
		//public ModelEffect.Techniques CurrentTechnique
		//{
		//    set
		//    {
		//        effect.CurrentTechnique = effect.Techniques[(int)value];
		//    }
		//}

		//public EffectPassCollection CurrentTechniquePasses
		//{
		//    get
		//    {
		//        return effect.CurrentTechnique.Passes;
		//    }
		//}

		//public Matrix World
		//{
		//    get { return worldMatrix; }
		//    set
		//    {
		//        worldMatrix = value;
		//        worldParam.SetValue(worldMatrix);
		//        worldViewParam.SetValue(worldMatrix * viewMatrix);
		//        worldViewProjectionParam.SetValue(worldMatrix * viewMatrix * projectionMatrix);
		//    }
		//}

		//public Matrix View
		//{
		//    get { return viewMatrix; }
		//    set
		//    {
		//        viewMatrix = value;
		//        Matrix viewInverseMatrix = Matrix.Invert(viewMatrix);
		//        viewInverseParam.SetValue(viewInverseMatrix);

		//        viewParam.SetValue(viewMatrix);
		//        worldViewParam.SetValue(worldMatrix * viewMatrix);
		//        worldViewProjectionParam.SetValue(worldMatrix * viewMatrix * projectionMatrix);
		//    }
		//}

		//public Matrix Projection
		//{
		//    get { return projectionMatrix; }
		//    set
		//    {
		//        projectionMatrix = value;
		//        worldViewProjectionParam.SetValue(worldMatrix * viewMatrix * projectionMatrix);
		//    }
		//}

		//public Vector3 DiffuseColor
		//{
		//    get { return diffuseColorParam.GetValueVector3(); }
		//    set { diffuseColorParam.SetValue(value); }
		//}

		//public Vector3 SpecularColor
		//{
		//    get { return specularColorParam.GetValueVector3(); }
		//    set { specularColorParam.SetValue(value); }
		//}

		//public float SpecularPower
		//{
		//    get { return specularPowerParam.GetValueSingle(); }
		//    set { specularPowerParam.SetValue(value); }
		//}

		//public Vector3 AmbientLightColor
		//{
		//    get { return ambientLightColorParam.GetValueVector3(); }
		//    set { ambientLightColorParam.SetValue(value); }
		//}

		//public Vector3 Light1Position
		//{
		//    get { return light1PositionParam.GetValueVector3(); }
		//    set { light1PositionParam.SetValue(value); }
		//}

		//public Vector3 Light1Color
		//{
		//    get { return light1ColorParam.GetValueVector3(); }
		//    set { light1ColorParam.SetValue(value); }
		//}

		//public Vector3 Light2Position
		//{
		//    get { return light2PositionParam.GetValueVector3(); }
		//    set { light2PositionParam.SetValue(value); }
		//}

		//public Vector3 Light2Color
		//{
		//    get { return light2ColorParam.GetValueVector3(); }
		//    set { light2ColorParam.SetValue(value); }
		//}

		//public ModelEffect(Effect effect)
		//{
		//    this.effect = effect;
		//    GetEffectParameters();
		//} 
		//#endregion

		///// <summary>
		///// Get all effects parameters by name
		///// </summary>
		//private void GetEffectParameters()
		//{
		//    // Matrices
		//    worldParam = effect.Parameters["matW"];
		//    viewParam = effect.Parameters["matV"];
		//    viewInverseParam = effect.Parameters["matVI"];
		//    worldViewParam = effect.Parameters["matWV"];
		//    worldViewProjectionParam = effect.Parameters["matWVP"];

		//    // Material
		//    diffuseColorParam = effect.Parameters["diffuseColor"];
		//    specularColorParam = effect.Parameters["specularColor"];
		//    specularPowerParam = effect.Parameters["specularPower"];

		//    // Lights
		//    ambientLightColorParam = effect.Parameters["ambientLightColor"];
		//    light1PositionParam = effect.Parameters["light1Position"];
		//    light1ColorParam = effect.Parameters["light1Color"];
		//    light2PositionParam = effect.Parameters["light2Position"];
		//    light2ColorParam = effect.Parameters["light2Color"];
		//}

		///// <summary>
		///// Begin effect
		///// </summary>
		//public void Begin()
		//{
		//    effect.Begin();
		//}

		///// <summary>
		///// End effect
		///// </summary>
		//public void End()
		//{
		//    effect.End();
		//}
	}
}