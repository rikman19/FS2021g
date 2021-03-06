﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace TechnomediaLabs.Internal
{
	[InitializeOnLoad]
	public class AutoSaveFeature
	{
		private const string MenuItemName = "Zetcil/AutoSave on Play";

		private static bool IsEnabled
		{
			get { return TechnomediaLabsSettings.AutoSaveEnabled; }
			set { TechnomediaLabsSettings.AutoSaveEnabled = value; }
		}

		[MenuItem(MenuItemName, priority = 1)]
		private static void MenuItem()
		{
			IsEnabled = !IsEnabled;
		}

		[MenuItem(MenuItemName, true)]
		private static bool MenuItemValidation()
		{
			Menu.SetChecked(MenuItemName, IsEnabled);
			return true;
		}


		static AutoSaveFeature()
		{
			EditorApplication.playModeStateChanged += AutoSaveWhenPlaymodeStarts;
		}

		private static void AutoSaveWhenPlaymodeStarts(PlayModeStateChange obj)
		{
			if (!IsEnabled) return;

			if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
			{
				for (var i = 0; i < SceneManager.sceneCount; i++)
				{
					var scene = SceneManager.GetSceneAt(i);
					if (scene.isDirty) EditorSceneManager.SaveScene(scene);
				}

				AssetDatabase.SaveAssets();
			}
		}
	}
}
#endif