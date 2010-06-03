using PobicosLibrary;
using Microsoft.Xna.Framework;
using POBICOS.SimLogic.Scenarios;
using System.Collections.Generic;
using System;

namespace POBICOS.SimLogic.PobicosObjects
{
	class DawnDetector :SimObject, IPobicosView, IPobicosObjects
	{
		private IModel pobicosModel;
		private int eventID = 0;
		private float brightness;
		private float minBrightness = 100;
		private float maxBrightness = 400;

		public int EventID
		{
			get
			{
				return eventID++;
			}
		}

		public float Brightness
		{
			get
			{
				return brightness;
			}
			set
			{
				brightness = value;
			}
		}

		public DawnDetector(Game game, string modelFile, Room room, string configFile)
			: base(game, modelFile, room)
		{
			List<IPobicosModel> models = PobicosLibrary.AdminTools.readConfiguration(configFile);

			foreach (PobicosLibrary.Model model in models)
			{
                SimScenario.Client.RegisterModel(model);
				model.AddObserver(this);
				this.Model = model;
			}
		}

		#region IPobicosView Members

		public void EventReturn(string callID, string returnValue)
		{
		}

		public void Instruction(string instruction, string callID, string param)
		{
			InstructionsList instr = (InstructionsList)Enum.Parse(typeof(InstructionsList), instruction);

			if (instr.Equals(InstructionsList.GetBrightness))
				SimScenario.Client.InstructionReturn((IPobicosModel)this.Model, callID, ((int)Brightness).ToString());
		}

		#endregion

		#region IView Members

		public void Update(IModel model)
		{
		}

		public IModel Model
		{
			get
			{
				return pobicosModel;
			}
			set
			{
				pobicosModel = value;
			}
		}

		#endregion

		#region IPobicosObjects Members

		public void Interact()
		{
			return;
		}

		public Vector3 Position()
		{
			return model.Transformation.Translate;
		}

		public void SwitchLight(float difference, Room room)
		{
			if (model.room.Equals(room) || room.Equals(Room.All))
			{
				model.basicEffectManager.Light0Direction *= new Vector3(difference);
				model.basicEffectManager.Light1Direction *= new Vector3(difference);
				model.basicEffectManager.Light2Direction *= new Vector3(difference);
			}

			Brightness = CalculateBrightness(model.basicEffectManager.Light0Direction.Length());
		}

		private float CalculateBrightness(float Light0Length)
		{
			float realBrightness;

			realBrightness = (Light0Length - SimScenario.minLight0Length) / (SimScenario.maxLight0Length - SimScenario.minLight0Length);
			realBrightness *= maxBrightness - minBrightness;
			realBrightness += minBrightness;

			return realBrightness;
		}

		public object GetByName(string name, Room room)
		{
			if (this.name.Contains(name) && this.model.room.Equals(room))
				return (Object)this;
			else
				return null;
		}

		#endregion
	}
}
