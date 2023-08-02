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
		/// ダウンロード制御処理クラス
		/// </summary>
		private static class DownloadControl {

			/// <summary>
			/// ダウンロードが必要なアセットのサイズを計測
			/// </summary>
			public async static void CalcDownloadSizeAsync(List<string> list, System.Action<int> complete = null, System.Action<string> error=null) {
				var handle = Addressables.GetDownloadSizeAsync((IEnumerable)list.ToArray());
				await handle.Task;
				//エラーだったときのハンドリング
				if (handle.Status != AsyncOperationStatus.Succeeded) {
					if (error != null)
						error(handle.OperationException.Message);

					Debug.Log(handle.OperationException.Message);
				}else{
					if (complete != null)
						complete((int)handle.Result);
					Debug.Log("download size = " + (int)handle.Result);
				}
				Addressables.Release(handle);
			}

			static AsyncOperationHandle downloadHandle_;
			/// <summary>
			/// ダウンロードの実行
			/// </summary>
			public static void StartDownload(List<string> list) {
				downloadHandle_ = Addressables.DownloadDependenciesAsync(list, Addressables.MergeMode.Union, true);
			}
			/// <summary>
			/// ダウンロード中か？
			/// </summary>
			public static bool IsDownloading() {
				//ダウンロードのインスタンスが消えていたら終わっていたとみなす
				return downloadHandle_.IsValid();
			}
			/// <summary>
			/// ダウンロードの進捗(0.0f～1.0f)
			/// </summary>
			public static float DownloadProgress() {
				if (!downloadHandle_.IsValid()){
					return 1f;
				}
				return downloadHandle_.PercentComplete;
			}

		}
	}
}
