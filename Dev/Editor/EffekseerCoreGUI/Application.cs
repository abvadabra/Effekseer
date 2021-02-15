﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Effekseer.GUI;
using EfkN = Effekseer.swig.EffekseerNative;

namespace Effekseer
{
	public abstract class Application
	{
		/// <summary>
		/// Starting (current) directory
		/// </summary>
		public static string StartDirectory
		{
			get;
			private set;
		}

		/// <summary>
		/// Executable file directory.
		/// </summary>
		public static string EntryDirectory
		{
			get;
			private set;
		}

		public void Initialize(bool gui)
		{
			try
			{
				StartDirectory = System.IO.Directory.GetCurrentDirectory();
				EntryDirectory = GetEntryDirectory();
			}
			catch (Exception e)
			{
				ExportError(e);
				throw;
			}




			// Register UI
			GUI.Component.ParameterListComponentFactory.Register(typeof(Data.LanguageSelector), () => { return new GUI.Component.LanguageSelector(); });

			GUI.Component.ParameterListComponentFactory.Register(typeof(Data.ProcedualModelReference), () => {
				return new GUI.Component.ObjectReference<Data.ProcedualModelParameter>(Core.ProcedualModel.ProcedualModels);
			});

			// Debug
			bool isDebugMode = false;
#if DEBUG
			isDebugMode = true;
#endif
			if (System.IO.File.Exists(Path.Combine(EntryDirectory, "debug.txt")) || isDebugMode)
			{
				swig.Native.SetFileLogger(Path.Combine(GetEntryDirectory(), "Effekseer.log.txt"));
				Utils.Logger.LogPath = Path.Combine(GetEntryDirectory(), "Effekseer.Managed.log.txt");
			}

			LanguageTable.LoadTable(Path.Combine(EntryDirectory, "resources/languages/languages.txt"));

			var systemLanguage = EfkN.GetSystemLanguage();
			string language = null;

			if (systemLanguage != swig.SystemLanguage.Unknown)
			{
				if (systemLanguage == swig.SystemLanguage.Japanese)
				{
					language = "ja";
				}
				else if (systemLanguage == swig.SystemLanguage.English)
				{
					language = "en";
				}
			}
			else
			{
				language = "en";
			}

			Core.OnOutputMessage += new Action<string>(Core_OnOutputMessage);
			Core.Initialize(language);

			if (gui)
			{
				ChangeLanguage();
				LanguageTable.OnLanguageChanged += (o, e) => { ChangeLanguage(); };

				// Failed to compile script
				if (Core.ExportScripts.Count == 0)
				{
					Script.ExportScript efkpkgExporter = new Script.ExportScript(
						Script.ScriptPosition.External,
						Plugin.ExportEfkPkg.UniqueName,
						Plugin.ExportEfkPkg.Author,
						Plugin.ExportEfkPkg.Title,
						Plugin.ExportEfkPkg.Description,
						Plugin.ExportEfkPkg.Filter,
						Plugin.ExportEfkPkg.Call);
					Core.ExportScripts.Add(efkpkgExporter);

					Script.ExportScript defaultExporter = new Script.ExportScript(
						Script.ScriptPosition.External,
						Plugin.ExportDefault.UniqueName,
						Plugin.ExportDefault.Author,
						Plugin.ExportDefault.Title,
						Plugin.ExportDefault.Description,
						Plugin.ExportDefault.Filter,
						Plugin.ExportDefault.Call);
					Core.ExportScripts.Add(defaultExporter);

					Script.ImportScript efkpkgImporter = new Script.ImportScript(
						Script.ScriptPosition.External,
						Plugin.ImportEfkPkg.UniqueName,
						Plugin.ImportEfkPkg.Author,
						Plugin.ImportEfkPkg.Title,
						Plugin.ImportEfkPkg.Description,
						Plugin.ImportEfkPkg.Filter,
						Plugin.ImportEfkPkg.Call);
					Core.ImportScripts.Add(efkpkgImporter);
				}

				OnInitialize();
			}
		}

		public void Run()
		{
			while (GUI.GUIManager.NativeManager.DoEvents())
			{
				OnUpdate();
			}

			OnTerminate();
		}

		protected virtual void OnInitialize()
		{
		}

		protected virtual void OnUpdate()
		{
		}

		protected virtual void OnTerminate()
		{
		}

		private static void ChangeLanguage()
		{
			MultiLanguageTextProvider.RootDirectory = GetEntryDirectory() + "/";
			MultiLanguageTextProvider.Reset();
			MultiLanguageTextProvider.LoadCSV("Base.csv");
			MultiLanguageTextProvider.LoadCSV("Effekseer.csv");
			MultiLanguageTextProvider.LoadCSV("Effekseer_Options.csv");
			MultiLanguageTextProvider.LoadCSV("Effekseer_BasicSettings.csv");
			MultiLanguageTextProvider.LoadCSV("Effekseer_Position.csv");
			MultiLanguageTextProvider.LoadCSV("Effekseer_Rotation.csv");
			MultiLanguageTextProvider.LoadCSV("Effekseer_Scale.csv");
			MultiLanguageTextProvider.LoadCSV("Effekseer_BasicRenderSettings.csv");
			MultiLanguageTextProvider.LoadCSV("Effekseer_RenderSettings.csv");
			MultiLanguageTextProvider.LoadCSV("Effekseer_LocalForceField.csv");
			MultiLanguageTextProvider.LoadCSV("Effekseer_AdvancedRenderCommon.csv");
			MultiLanguageTextProvider.LoadCSV("Effekseer_Environment.csv");
			MultiLanguageTextProvider.LoadCSV("Effekseer_ProcedualModel.csv");

			GUIManager.UpdateFont();
		}

		private static void Core_OnOutputMessage(string obj)
		{
			swig.GUIManager.show(obj, "Error", swig.DialogStyle.Error, swig.DialogButtons.OK);
		}

		public static void ExportError(Exception e)
		{
			string messageBase = "Error has been caused.";

			if (e is UnauthorizedAccessException)
			{
				if (Core.Language == Language.Japanese)
				{
					messageBase = "アクセスが拒否されエラーが発生しました。\n他のディレクトリにインストールしてください。\nもしくはアクセスできないファイルを選択しています。\n";
				}
				else
				{
					messageBase = "Access is denied and an error occurred. \nPlease install it in another directory. \nOr you have selected a file that you cannot access.\n";
				}
			}
			else
			{
				if (Core.Language == Language.Japanese)
				{
					messageBase = "エラーが発生しました。";
				}
			}

			DateTime dt = DateTime.Now;
			var filename = string.Format("error_{0:D4}_{1:D2}_{2:D2}_{3:D2}_{4:D2}_{5:D2}.txt", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
			var filepath = Path.Combine(EntryDirectory, filename);

			try
			{
				System.IO.File.WriteAllText(filepath, e.ToString());

				string message = messageBase + "Error log is written in " + filepath + "\nWe are glad if you send this error to Effekseer with a mail or twitter.\n";

				if (Core.Language == Language.Japanese)
				{
					message = messageBase + "エラーログが" + filepath + "に出力されました。\nもしエラーをメールやTwitterでEffekseerに送っていただけると助かります。\n";
				}
				swig.GUIManager.show(message, "Error", swig.DialogStyle.Error, swig.DialogButtons.OK);
			}
			catch (Exception e2)
			{
				string message = messageBase;

				message += e.ToString();
				message += "\n";
				message += e2.ToString();
				swig.GUIManager.show(message, "Error", swig.DialogStyle.Error, swig.DialogButtons.OK);
			}
		}

		/// <summary>
		/// Get a directory where this application is located.
		/// </summary>
		/// <returns></returns>
		private static string GetEntryDirectory()
		{
			var myAssembly = System.Reflection.Assembly.GetEntryAssembly();
			string path = myAssembly.Location;
			var dir = System.IO.Path.GetDirectoryName(path);

			// for mkbundle
			if (dir == string.Empty)
			{
				dir = System.IO.Path.GetDirectoryName(
				System.IO.Path.GetFullPath(
				Environment.GetCommandLineArgs()[0]));
			}

			return dir;
		}
	}
}
