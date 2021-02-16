
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;
using EfkN = Effekseer.swig.EffekseerNative;

namespace Effekseer
{
	public class Images
	{
		public static swig.ImageResource Play;
		public static swig.ImageResource Stop;
		public static swig.ImageResource Pause;
		public static swig.ImageResource Step;
		public static swig.ImageResource BackStep;

		public static Dictionary<string, swig.ImageResource> Icons = new Dictionary<string, swig.ImageResource>();

		public static Dictionary<string, swig.ImageResource> tempImages = new Dictionary<string, swig.ImageResource>();

		public static swig.ImageResource Load(swig.Native native, string path, bool isRequiredToReload = false)
		{
			if (tempImages.ContainsKey(path) && !isRequiredToReload)
			{
				return tempImages[path];
			}
			else
			{
				if (tempImages.ContainsKey(path))
				{
					tempImages[path].Invalidate();
					tempImages[path].Validate();
					return tempImages[path];
				}

				var img = native.LoadImageResource(path);
				if (img != null)
				{
					tempImages.Add(path, img);
				}
				return img;
			}
		}

		public static swig.ImageResource LoadAppResource(swig.Native native, string path)
		{
			string appDirectory = Application.EntryDirectory;
			string fullPath = Path.Combine(appDirectory, path);
			return native.LoadImageResource(fullPath);
		}

		public static void LoadCommonIcons(swig.Native native, string appIconPath)
		{
			Play = LoadAppResource(native, "resources/Play.png");
			Stop = LoadAppResource(native, "resources/Stop.png");
			Pause = LoadAppResource(native, "resources/Pause.png");
			Step = LoadAppResource(native, "resources/Step.png");
			BackStep = LoadAppResource(native, "resources/BackStep.png");

			Icons["Copy"] = LoadAppResource(native, "resources/icons/Copy.png");
			Icons["Paste"] = LoadAppResource(native, "resources/icons/Paste.png");

			Icons["AppIcon"] = LoadAppResource(native, appIconPath);

			Icons["VisibleShow"] = LoadAppResource(native, "resources/icons/Visible_Show.png");
			Icons["VisibleHide"] = LoadAppResource(native, "resources/icons/Visible_Hide.png");

			Icons["EnlargeAnchor"] = LoadAppResource(native, "resources/icons/EnlargeAnchor.png");
			Icons["ShrinkAnchor"] = LoadAppResource(native, "resources/icons/ShrinkAnchor.png");

			Icons["EnlargeAnchor"] = LoadAppResource(native, "resources/icons/EnlargeAnchor.png");
			Icons["ShrinkAnchor"] = LoadAppResource(native, "resources/icons/ShrinkAnchor.png");

			Icons["AutoZoom_On"] = LoadAppResource(native, "resources/icons/AutoZoom_On.png");
			Icons["AutoZoom_Off"] = LoadAppResource(native, "resources/icons/AutoZoom_Off.png");

			Icons["FileViewer_Directory"] = LoadAppResource(native, "resources/icons/FileViewer_Directory.png");
			Icons["FileViewer_EffekseerProj"] = LoadAppResource(native, "resources/icons/FileViewer_EffekseerProj.png");

			Icons["ButtonMin"] = LoadAppResource(native, "resources/icons/Button_Min.png");
			Icons["ButtonMax"] = LoadAppResource(native, "resources/icons/Button_Max.png");
			Icons["ButtonMaxCancel"] = LoadAppResource(native, "resources/icons/Button_MaxCancel.png");
			Icons["ButtonClose"] = LoadAppResource(native, "resources/icons/Button_Close.png");
		}

		public static void Unload()
		{
			Play.Dispose();
			Stop.Dispose();
			Pause.Dispose();
			Step.Dispose();
			BackStep.Dispose();
			foreach (var keyValue in Icons)
			{
				if (keyValue.Value != null)
				{
					keyValue.Value.Dispose();
				}
			}

			foreach (var keyValue in tempImages)
			{
				if (keyValue.Value != null)
				{
					keyValue.Value.Dispose();
				}
			}
		}

		public static swig.ImageResource GetIcon(string resourceName)
		{
			return Icons.ContainsKey(resourceName) ? Icons[resourceName] : null;
		}
	}
}