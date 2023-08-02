using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;


namespace Project.Lib {
	/// <summary>
	/// AddressableAssetsの補助クラス
	/// </summary>
	/// <remarks>
	/// アドレス登録してないアセットはローカルからロードするように補助する
	/// </remarks>
	public partial class AddressableAssist {
		//@todo 調整中
		/// <summary>
		/// カタログの更新
		/// </summary>
		public async static void LoadCatalogAsync(string url) {
			//var handle = Addressables.LoadContentCatalogAsync("http://localhost:8081/userContent/StandaloneWindows64/catalog_0.0.1.json");
			var handle = Addressables.LoadContentCatalogAsync(url);
			await handle.Task;

			Addressables.Release(handle);
		}
		/// <summary>
		/// ダウンロードが必要なアセットのサイズを計測
		/// </summary>
		public static void CalcDownloadSizeAsync(List<string> list, System.Action<int> complete = null, System.Action<string> error = null) {
			DownloadControl.CalcDownloadSizeAsync(list, complete, error);
		}
		/// <summary>
		/// ダウンロードの実行
		/// </summary>
		public static void StartDownload(List<string> list) {
			DownloadControl.StartDownload(list);
		}

		/// <summary>
		/// ダウンロード終了したか
		/// </summary>
		public static bool IsDownloading() {
			return DownloadControl.IsDownloading();
		}
		/// <summary>
		/// ダウンロードの進捗
		/// </summary>
		public static float DownloadProgress() {
			return DownloadControl.DownloadProgress();
		}

		/// <summary>
		/// アセットをロード
		/// </summary>
		public async static Task<Object> LoadAssetAsync(string address) {
			return await AssetControl.LoadAssetAsync(address);
		}

		/// <summary>
		/// アセットを複数ロード
		/// </summary>
		public async static Task<List<Object>> LoadAssetsAsync(List<string> addresses) {
			return await AssetControl.LoadAssetsAsync(addresses);
		}
		/// <summary>
		/// アセットのアンロード
		/// </summary>
		public static void UnLoadAsset(Object address) {
			AssetControl.UnLoadAsset(address);
		}

		/// <summary>
		/// アセットをcategoryでロード
		/// </summary>
		public async static Task<List<UnityEngine.Object>> LoadAssetGroupAsync(string label) {
			return await AssetGroupControl.LoadAssetGroupAsync(label);
		}
		/// <summary>
		/// アセットをcategoryでロード
		/// </summary>
		public async static Task<List<List<UnityEngine.Object>>> LoadAssetGroupAsync(List<string> labels) {
			return await AssetGroupControl.LoadAssetGroupAsync(labels);
		}
		/// <summary>
		/// categoryでロードしたアセットを解放
		/// </summary>
		public static void UnLoadAssetGroup(List<UnityEngine.Object> assets) {
			AssetGroupControl.UnLoadAssetGroup(assets);
		}

		/// <summary>
		/// シーンをロード
		/// </summary>
		public async static Task<SceneInstance> LoadSceneAsync(string address, UnityEngine.SceneManagement.LoadSceneMode mode) {
			return await SceneControl.LoadSceneAsync(address, mode);
		}

		/// <summary>
		/// シーンのアンロード
		/// </summary>
		public static void UnLoadSceneAsync(SceneInstance scene) {
			SceneControl.UnLoadSceneAsync(scene);
		}




#if UNITY_EDITOR
		//PC上のディレクトリパス
		static readonly string ADDRESSABLE_ASSET_PATH = "Assets/Application/Addressable/";
		static readonly string ADDRESSABLE_DIRECTORY = Application.dataPath + "/Application/Addressable/";
		/// <summary>
		/// ファイルの拡張子を取得
		/// </summary>
		/// <remarks>
		/// Addressableのパスは拡張子なしなのでファイルアクセスして拡張子を取得する
		/// </remarks>
		public static string GetExtension(string name) {
			//最後の'/'でファイル名とパスを分離
			int splitIndex = name.LastIndexOf('/');

			string path;
			string filename;
			if (splitIndex < 0) {
				path = "";
				filename = name;
			} else {
				path = name.Substring(0, splitIndex);
				filename = name.Substring(splitIndex + 1);
			}
			//ディレクトリに存在するファイル一覧を取得
			string[] files = System.IO.Directory.GetFiles(ADDRESSABLE_DIRECTORY + path);

			//ファイル一覧からファイル名が前方一致するものを探す
			string pname = System.Array.Find<string>(files, s => System.IO.Path.GetFileName(s).StartsWith(filename));
			//拡張子だけにして返す
			return System.IO.Path.GetExtension(pname);
		}
		/// <summary>
		/// ディレクトリの存在チェック
		/// </summary>
		public static bool IsExistDirectory(string path) {
			return System.IO.Directory.Exists(ADDRESSABLE_DIRECTORY + path);
		}

#endif

	}
}
