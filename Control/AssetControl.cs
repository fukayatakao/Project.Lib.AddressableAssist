using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace Project.Lib {
	/// <summary>
	/// AddressableAssetsの補助クラス
	/// </summary>
	public partial class AddressableAssist {

		/// <summary>
		/// 個別アセットの制御処理クラス
		/// </summary>
		public class AssetControl {
#if UNITY_EDITOR
			//ローカルファイルを直接ロードしたのでAddressableを使わないで解放するリスト
			static Dictionary<string, Object> localAssetCache_ = new Dictionary<string, Object>();
			static Dictionary<string, int> localAssetCounter_ = new Dictionary<string, int>();
#endif

			/// <summary>
			/// アセットをロード
			/// </summary>
			/// <remarks>
			/// ダウンロードされてないときはダウンロードも行う
			/// </remarks>
			public async static Task<Object> LoadAssetAsync(string address) {
				return await LoadImplAsync(address);
			}

			/// <summary>
			/// アセットを複数ロード
			/// </summary>
			/// <remarks>
			/// ダウンロードされてないときはダウンロードも行う
			/// </remarks>
			public async static Task<List<Object>> LoadAssetsAsync(List<string> addresses) {
				List<Task<Object>> arrayTask = new List<Task<Object>>();
				List<Object> assets = new List<Object>();

				for (int i = 0, max = addresses.Count; i < max; i++) {
					arrayTask.Add(LoadImplAsync(addresses[i]));
				}
				await Task.WhenAll(arrayTask);
				//エラーチェック
				for (int i = 0, max = addresses.Count; i < max; i++) {
					assets.Add(arrayTask[i].Result);
				}

				return assets;
			}
			/// <summary>
			/// ロード処理実体
			/// </summary>
			private async static Task<Object> LoadImplAsync(string address) {
				//ロードのリクエストを出して終了まで待機
				Object asset = await Addressables.LoadAssetAsync<Object>(address).Task;

#if UNITY_EDITOR
				//UnityEditorのときはaddressableでアセットが見つからなかったときはローカルのアセットを直接見に行く
				if (asset == null) {
					Debug.LogWarning("use local asset load:" + address);
					if (!localAssetCounter_.ContainsKey(address)) {
						localAssetCounter_[address] = 0;
					}
					if (localAssetCounter_[address] == 0) {
						asset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(ADDRESSABLE_ASSET_PATH + address + AddressableAssist.GetExtension(address));
						localAssetCache_[address] = asset;
					}
					localAssetCounter_[address]++;
				}
#endif
				//Debug.Log("Load Address:" + address);
				return asset;
			}
			/// <summary>
			/// アセットのアンロード
			/// </summary>
			public static void UnLoadAsset(Object asset) {
#if UNITY_EDITOR
				//ローカルからロードしたアセットかチェック
				foreach(KeyValuePair<string, Object> pair in localAssetCache_) {
					if (pair.Value != asset)
						continue;
					//ローカルからロードしたアセットが見つかったら
					localAssetCounter_[pair.Key]--;
					if (localAssetCounter_[pair.Key] == 0) {
						localAssetCounter_.Remove(pair.Key);
						localAssetCache_.Remove(pair.Key);
					}
					return;
				}
#endif
				Addressables.Release(asset);
			}
		}
	}
}
