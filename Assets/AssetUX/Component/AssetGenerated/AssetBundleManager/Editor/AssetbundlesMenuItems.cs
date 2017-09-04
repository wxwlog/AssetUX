using UnityEngine;
using UnityEditor;
using System.Collections;
using AssetUX;

namespace AssetBundles
{
	public class AssetBundlesMenuItems
	{
		const string kSimulationMode = "AssetsUX/AssetBundles/Simulation Mode";
	
		[MenuItem(kSimulationMode)]
		public static void ToggleSimulationMode ()
		{
			AssetBundleManager.SimulateAssetBundleInEditor = !AssetBundleManager.SimulateAssetBundleInEditor;
		}
	
		[MenuItem(kSimulationMode, true)]
		public static bool ToggleSimulationModeValidate ()
		{
			Menu.SetChecked(kSimulationMode, AssetBundleManager.SimulateAssetBundleInEditor);
			return true;
		}
		
		[MenuItem ("AssetsUX/AssetBundles/Build AssetBundles")]
		static public void BuildAssetBundles ()
		{
			//BuildScript.BuildAssetBundles();  原语句;
            
            EditorUtility.SetDirty(Settings.Instance);
            Selection.activeObject = Settings.Instance;
           // ScriptableObject.CreateInstance<Settings>();
		}
	}
}