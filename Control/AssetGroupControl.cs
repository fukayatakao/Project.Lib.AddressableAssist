using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

using System.Threading.Tasks;


namespace Project.Lib {
	/// <summary>
	/// AddressableAssetsの補助クラス
	/// </summary>
	public static partial class AddressableAssist {
		/// <summary>
		/// label単位の制御処理クラス
		/// </summary>
		private static class AssetGroupControl {
#if UNITY_EDITOR
			//ローカルファイルを直接ロードしたのでAddressableを使わないで解放するリスト
			static Dictionary<string, List<UnityEngine.Object>> localAssetGroupCache_ = new Dictionary<string, List<UnityEngine.Object>>();
			static Dictionary<string, int> localAssetGroupCounter_ = new Dictionary<string, int>();
#endif
			//@note  AddressableAssetはロードした単位でしか解放できないようなのでラベルを使って複数のアセットを同時にロードした後に個別で開放はできない。
			/// <summary>
			/// アセットをlabelでロード
			/// </summary>
			public async static Task<List<UnityEngine.Object>> LoadAssetGroupAsync(string label) {
				return await LoadImplAsync(label);
			}



			/// <summary>
			/// アセットを複数ロード
			/// </summary>
			/// <remarks>
			/// ダウンロードされてないときはダウンロードも行う
			/// </remarks>
			public async static Task<List<List<UnityEngine.Object>>> LoadAssetGroupAsync(List<string> labels) {
				List<Task<List<UnityEngine.Object>>> arrayTask = new List<Task<List<UnityEngine.Object>>>();
				List<List<UnityEngine.Object>> assets = new List<List<UnityEngine.Object>>();

				for (int i = 0, max = labels.Count; i < max; i++) {
					arrayTask.Add(LoadImplAsync(labels[i]));
				}
				await Task.WhenAll(arrayTask);
				//エラーチェック
				//エラーチェック
				for (int i = 0, max = labels.Count; i < max; i++) {
					assets.Add(arrayTask[i].Result);
				}

				return assets;
			}
			/// <summary>
			/// ロード処理実体
			/// </summary>
			private async static Task<List<UnityEngine.Object>> LoadImplAsync(string label) {
				List<UnityEngine.Object> list = (List<UnityEngine.Object>)await Addressables.LoadAssetsAsync<UnityEngine.Object>(label, null).Task;

#if UNITY_EDITOR
				if (list == null) {
					Debug.LogWarning("use local asset load:" + label);
					if (!localAssetGroupCounter_.ContainsKey(label)) {
						localAssetGroupCounter_[label] = 0;
					}
					if (localAssetGroupCounter_[label] == 0) {

						string path = "Character/" + label.Replace('-', '/');
						List<UnityEngine.Object> assets;
						if (IsExistDirectory(path)) {
							assets = new List<UnityEngine.Object>(UnityEditor.AssetDatabase.LoadAllAssetsAtPath(ADDRESSABLE_ASSET_PATH + path));
						} else {
							assets = new List<Object>();
						}
						localAssetGroupCache_[label] = assets;
					}
					localAssetGroupCounter_[label]++;

				}
#endif
				//Debug.Log("Load label:" + label);

				return list;
			}


			/// <summary>
			/// labelでロードしたアセットを解放
			/// </summary>
			public static void UnLoadAssetGroup(List<UnityEngine.Object> assets) {
#if UNITY_EDITOR
				//ローカルからロードしたアセットかチェック
				foreach (KeyValuePair<string, List<UnityEngine.Object>> pair in localAssetGroupCache_) {
					if (pair.Value != assets)
						continue;
					//ローカルからロードしたアセットが見つかったら
					localAssetGroupCounter_[pair.Key]--;
					if (localAssetGroupCounter_[pair.Key] == 0) {
						localAssetGroupCounter_.Remove(pair.Key);
						localAssetGroupCache_.Remove(pair.Key);
					}
					return;
				}
#endif
				Addressables.Release(assets);
			}

		}
	}
}
