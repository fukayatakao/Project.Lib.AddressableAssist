#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace Project.Lib {
	public static class AddressableGenerateGroup {
		static string AddressablePath = Application.dataPath + "/Application/Addressable/";
		static string AssetPath = Application.dataPath + "/Application/";

		//自動生成に含めないグループ
		static string[] ignoreList = new string[] { "Built In Data" };
		//ラベルを自動付与するグループ
		static string[] grantlabeGroup = new string[] { "Character-Animation", "Character-Action" };
		static string[] grantlabeGroupPrefix = new string[] { "Animation-", "Action-" };

		//[MenuItem("Editor/Addressable/Profile")]
		public static void SetProfile() {
			var settings = AddressableAssetSettingsDefaultObject.Settings;
			ProjectConfigData.PostProfilerEvents = true;

		}
		[MenuItem("Editor/Addressable/GenerateGroup")]
		public static void GenerateCRIGroup() {
			var settings = AddressableAssetSettingsDefaultObject.Settings;

			List<string> labels = settings.GetLabels();
			for(int i = 0; i < labels.Count; i++) {
				settings.RemoveLabel(labels[i]);
			}

			//AddressableAssetのグループ設定を検索
			for (int i = 0; i < ignoreList.Length; i++) {
				ignoreList[i] = ignoreList[i].ToLower();
			}
			for (int i = 0; i < grantlabeGroup.Length; i++) {
				grantlabeGroup[i] = grantlabeGroup[i].ToLower();
			}

			for (int i = 0; i < settings.groups.Count; i++) {
				//Built Inは無視
				string lower = settings.groups[i].Name.ToLower();
				if (System.Array.FindIndex(ignoreList, l => lower.StartsWith(l)) >= 0)
					continue;
				var group = settings.groups[i];

				List<AddressableAssetEntry> entryList = new List<AddressableAssetEntry>();
				//登録名を編集する
				foreach (var en in settings.groups[i].entries) {
					entryList.Add(en);
				}
				for(int j = 0;j < entryList.Count; j++) {
					group.RemoveAssetEntry(entryList[j]);
				}

				string[] split = settings.groups[i].Name.Split('-');
				string dirName = "";
				for (int j = 0; j < split.Length; j++) {
					dirName += split[j] + "/";
				}
				//グループ名がディレクトリ名になってないときは無視
				if (!System.IO.Directory.Exists(AddressablePath + dirName)){
					Debug.LogWarning("not found directory:" + dirName);
					continue;
				}
				string[] files = System.IO.Directory.GetFiles(AddressablePath + dirName, "*.meta", System.IO.SearchOption.TopDirectoryOnly);
				for (int j = 0; j < files.Length; j++) {
					string file = files[j].Substring(0, files[j].Length - ".meta".Length);
					var guid = AssetDatabase.AssetPathToGUID(file.Replace(Application.dataPath, "Assets"));
					var entry = settings.CreateOrMoveEntry(guid, group);

					entry.address = dirName + System.IO.Path.GetFileNameWithoutExtension(file);

					//ラベルを付けるグループの場合
					int index = System.Array.FindIndex(grantlabeGroup, l => lower.StartsWith(l));
					if (index >= 0) {
						string label = grantlabeGroupPrefix[index] + System.IO.Path.GetFileNameWithoutExtension(file);
						settings.AddLabel(label);
						entry.labels.Add(label);
					}

				}



			}

		}

	}
}
#endif
