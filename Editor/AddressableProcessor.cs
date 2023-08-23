#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Project.Lib {
	public static class AddressableProcessor {
		/// <summary>
		/// アセットバンドルビルド
		/// </summary>
		[MenuItem("Editor/Addressable/Build")]
		public static void Build() {
			//アセットバンドルファイルを削除
			ClearServerDataImpl(EditorUserBuildSettings.activeBuildTarget.ToString());
			Debug.Log("BuildAddressablesProcessor.PreExport start");

			UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.CleanPlayerContent();
			UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.BuildPlayerContent();
			Debug.Log("BuildAddressablesProcessor.PreExport done");
		}




		/// <summary>
		/// 以前作成したアセットバンドルのファイルを削除
		/// </summary>
		[MenuItem("Editor/Addressable/ClearServerData")]
		public static void ClearServerData(){
			ClearServerDataImpl();
		}
		/// <summary>
		/// Unityがwindows上に作ったダウンロードアセットのキャッシュを削除
		/// </summary>
		[MenuItem("Editor/Addressable/ClearDownloadCache")]
		public static void ClearCache() {
			Caching.ClearCache();
		}


		/// <summary>
		/// すべてのアセットバンドルをRemote設定にする
		/// </summary>
		[MenuItem("Editor/Addressable/SetAllRemoteBuild")]
		public static void SetAllRemoteBuild() {
			//AddressableAssetの設定を取得
			//var settings = AssetDatabase.LoadAssetAtPath<UnityEditor.AddressableAssets.Settings.AddressableAssetSettings>("Assets/AddressableAssetsData/AddressableAssetSettings.asset");
			var settings = AddressableAssetSettingsDefaultObject.Settings;
			var build = settings.profileSettings.GetValueByName("Default", "RemoteBuildPath");
			var load = settings.profileSettings.GetValueByName("Default", "RemoteLoadPath");

			//AddressableAssetのグループ設定を検索
			string[] list = System.IO.Directory.GetFiles(Application.dataPath + "/AddressableAssetsData/AssetGroups/", "*.asset");


			for (int i = 0; i < list.Length; i++) {
				string path = list[i].Replace(Application.dataPath, "Assets");
				//Built Inは無視
				if (System.IO.Path.GetFileName(path).ToLower().StartsWith("built in")) {
					continue;
				}

				//グループのビルド設定をRemoteに設定
				var asset = AssetDatabase.LoadAssetAtPath<UnityEditor.AddressableAssets.Settings.AddressableAssetGroup>(path);
				var schema = asset.GetSchema<UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema>();

				schema.BuildPath.SetVariableById(settings, build);
				schema.LoadPath.SetVariableById(settings, load);

			}
		}
		/// <summary>
		/// すべてのアセットバンドルをRemote設定にする
		/// </summary>
		[MenuItem("Editor/Addressable/SetAllLocalBuild")]
		public static void SetAllLocalBuild() {
			//AddressableAssetの設定を取得
			//var settings = AssetDatabase.LoadAssetAtPath<UnityEditor.AddressableAssets.Settings.AddressableAssetSettings>("Assets/AddressableAssetsData/AddressableAssetSettings.asset");
			var settings = AddressableAssetSettingsDefaultObject.Settings;
			var build = settings.profileSettings.GetValueByName("Default", "LocalBuildPath");
			var load = settings.profileSettings.GetValueByName("Default", "LocalLoadPath");

			//AddressableAssetのグループ設定を検索
			string[] list = System.IO.Directory.GetFiles(Application.dataPath + "/AddressableAssetsData/AssetGroups/", "*.asset");


			for (int i = 0; i < list.Length; i++) {
				string path = list[i].Replace(Application.dataPath, "Assets");
				//Built Inは無視
				if (System.IO.Path.GetFileName(path).ToLower().StartsWith("built in")) {
					continue;
				}

				//グループのビルド設定をRemoteに設定
				var asset = AssetDatabase.LoadAssetAtPath<UnityEditor.AddressableAssets.Settings.AddressableAssetGroup>(path);
				var schema = asset.GetSchema<UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema>();

				schema.BuildPath.SetVariableById(settings, build);
				schema.LoadPath.SetVariableById(settings, load);

			}
		}
		//パス修正に含めないグループ
		static string[] ignoreList = new string[] { "built in", "effect" };
		//addressableに含めるアセットの置き場所
		static string addressablePath = "Assets/Application/Addressable/";
		[MenuItem("Editor/Addressable/CorrectPath")]
		public static void CorrectGroupPath() {
			//AddressableAssetのグループ設定を検索
			string[] list = System.IO.Directory.GetFiles(Application.dataPath + "/AddressableAssetsData/AssetGroups/", "*.asset");


			for (int i = 0; i < list.Length; i++) {
				string path = list[i].Replace(Application.dataPath, "Assets");
				//Built Inは無視
				string lower = System.IO.Path.GetFileName(path).ToLower();
				if (System.Array.FindIndex(ignoreList, l => lower.StartsWith(l)) >= 0)
					continue;

				//グループのビルド設定を取得
				var asset = AssetDatabase.LoadAssetAtPath<AddressableAssetGroup>(path);

				//登録名を編集する
				foreach (var en in asset.entries) {
					string address = en.AssetPath.Replace(addressablePath, "");
					en.address = System.IO.Path.GetDirectoryName(address).Replace('\\', '/') + "/" + System.IO.Path.GetFileNameWithoutExtension(address);
				}
			}
		}
		/// <summary>
		/// 以前作成したアセットバンドルのファイルを削除
		/// </summary>
		private static void ClearServerDataImpl(string subDir = "") {
			//前回のビルド結果を削除
			string archive = Application.dataPath + "/../" + "ServerData/" + subDir;

			//ファイルがある場合は削除
			if (System.IO.Directory.Exists(archive)) {
				System.IO.Directory.Delete(archive, true);
			}
		}



	}
}
#endif
