using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;


namespace Project.Lib {
	/// <summary>
	/// AddressableAssetsの補助クラス
	/// </summary>
	public static partial class AddressableAssist {
		/// <summary>
		/// シーンアセットの制御処理クラス
		/// </summary>
		private static class SceneControl {
#if UNITY_EDITOR
			//ローカルファイルを直接ロードしたのでAddressableを使わないで解放するリスト
			//シーンはSceneInstanceのSceneメンバから取得する
			static Dictionary<string, Scene> localAssetCache_ = new Dictionary<string, Scene>();
			static Dictionary<string, int> localAssetCounter_ = new Dictionary<string, int>();
#endif

			/// <summary>
			/// シーンをロード
			/// </summary>
			/// <remarks>
			/// ダウンロードされてないときはダウンロードも行う
			/// </remarks>
			public async static Task<SceneInstance> LoadSceneAsync(string address, LoadSceneMode mode) {
				return await LoadSceneImplAsync(address, mode);
			}


			/// <summary>
			/// ロード処理実体
			/// </summary>
			private async static Task<SceneInstance> LoadSceneImplAsync(string address, LoadSceneMode mode) {
				//ロードのリクエストを出して終了まで待機
				SceneInstance scene = await Addressables.LoadSceneAsync(address, mode).Task;
#if UNITY_EDITOR
				//var s = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(address, (OpenSceneMode)mode);
				


#endif


				Debug.Log("Load Scene:" + address);
				return scene;
			}
			/// <summary>
			/// シーンのアンロード
			/// </summary>
			public static void UnLoadSceneAsync(SceneInstance scene) {
#if UNITY_EDITOR
				//@todo ローカルロードはそのうち
#endif
				Addressables.UnloadSceneAsync(scene);
			}
		}
	}
}
