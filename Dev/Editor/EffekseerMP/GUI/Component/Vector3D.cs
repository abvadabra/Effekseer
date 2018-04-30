﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Effekseer.GUI.Component
{
	class Vector3D : Control, IParameterControl
	{
		string id = "";

		public string Label { get; set; } = string.Empty;

		public string Description { get; set; } = string.Empty;

		bool isActive = false;

		Data.Value.Vector3D binding = null;

		float[] internalValue = new float[] { 0.0f, 0.0f, 0.0f };

		public bool EnableUndo { get; set; } = true;

		public Data.Value.Vector3D Binding
		{
			get
			{
				return binding;
			}
			set
			{
				if (binding == value) return;

				binding = value;

				if (binding != null)
				{
					internalValue[0] = binding.X.Value;
					internalValue[1] = binding.Y.Value;
					internalValue[2] = binding.Z.Value;
				}
			}
		}

		public Vector3D(string label = null)
		{
			if (label != null)
			{
				Label = label;
			}

			var rand = new Random();
			id = "###" + Manager.GetUniqueID().ToString();
		}

		public void SetBinding(object o)
		{
			var o_ = o as Data.Value.Vector3D;
			Binding = o_;
		}

		public void FixValue()
		{
			FixValueInternal(false);
		}

		void FixValueInternal(bool combined)
		{
			if (binding == null) return;

			if (EnableUndo)
			{
				binding.X.SetValue(internalValue[0], combined);
				binding.Y.SetValue(internalValue[1], combined);
				binding.Z.SetValue(internalValue[2], combined);
			}
			else
			{
				binding.X.SetValueDirectly(internalValue[0]);
				binding.Y.SetValueDirectly(internalValue[1]);
				binding.Z.SetValueDirectly(internalValue[2]);
			}
		}

		public override void OnDisposed()
		{
			FixValueInternal(false);
		}

		public override void Update()
		{
			if (binding != null)
			{
				internalValue[0] = binding.X.Value;
				internalValue[1] = binding.Y.Value;
				internalValue[2] = binding.Z.Value;
			}

			if (Manager.NativeManager.DragFloat3(id, internalValue))
			{
				FixValueInternal(isActive);
			}

			var isActive_Current = Manager.NativeManager.IsItemActive();

			if (isActive && !isActive_Current)
			{
				FixValue();
			}

			isActive = isActive_Current;
		}
	}
}
