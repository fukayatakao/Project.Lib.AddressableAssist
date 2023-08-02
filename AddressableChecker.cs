using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Project.Lib {
	/// <summary>
	/// Addressableの解放漏れチェック
	/// </summary>
	public class AddressableChecker {
		//シングルトンインスタンス
		static AddressableChecker singleton_;
		//チェックポイント時点の参照カウンタ記録
		Dictionary<string, Dictionary<IResourceLocation, int>> checkpoint_ = new Dictionary<string, Dictionary<IResourceLocation, int>>();
		Dictionary<IResourceLocation, int> lastCheckpoint_;
		//アセットの参照カウンタ情報
		Dictionary<IResourceLocation, int> refCounter_ = new Dictionary<IResourceLocation, int>();

		/// <summary>
		/// インスタンス生成
		/// </summary>
		private static void Create() {
			if (singleton_ != null)
				return;
			singleton_ = new AddressableChecker();
		}
		/// <summary>
		/// チェックポイント記録
		/// </summary>
		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		public static void CheckPoint(string pointName) {
			Create();
			if (singleton_.lastCheckpoint_ != null) {
				Dictionary<IResourceLocation, int> last = new Dictionary<IResourceLocation, int>(singleton_.lastCheckpoint_);
				Dictionary<IResourceLocation, int> current = singleton_.refCounter_;

				Debug.Log("check point:" + pointName);
				//前回記録時との差分を表示
				foreach (var pair in current) {
					//前回の記録がある
					if (last.ContainsKey(pair.Key)) {
						//参照カウンタが一致してない=解放漏れor過剰解放の可能性
						if (last[pair.Key] != pair.Value) {
							Debug.LogError("check counter:" + pair.Key.PrimaryKey + ":" + last[pair.Key] + "->" + pair.Value);
						}

						last.Remove(pair.Key);
					//前回の記録がない=解放漏れの可能性
					} else {
						Debug.LogError("leak: " + pair.Key.PrimaryKey);
					}
				}
				//前回記録よりもアセットが減ってる=過剰解放の可能性
				if(last.Count > 0) {
					foreach (var pair in last) {
						Debug.LogError("over release: " + pair.Key.PrimaryKey);
					}
				}

			}


			singleton_.checkpoint_[pointName] = new Dictionary<IResourceLocation, int>(singleton_.refCounter_);
			singleton_.lastCheckpoint_ = singleton_.checkpoint_[pointName];
		}
		/// <summary>
		/// インスタンス生成
		/// </summary>
		AddressableChecker() {
			singleton_ = this;
			// DiagnosticCallbackを登録
			Addressables.ResourceManager.RegisterDiagnosticCallback(DiagnosticCallback);
		}
		/// <summary>
		/// インスタンス破棄
		/// </summary>
		~AddressableChecker() {
			//SendProfileEventが有効になってないとコールバックの登録ができてなくてエラーが出る。
#if UNITY_EDITOR
			if (UnityEditor.AddressableAssets.Settings.ProjectConfigData.PostProfilerEvents)
#endif
			{
				// DiagnosticCallbackを登録解除
				Addressables.ResourceManager.UnregisterDiagnosticCallback(DiagnosticCallback);
			}
			singleton_ = null;
		}
		/// <summary>
		/// 参照カウントが変わった
		/// </summary>
		private void DiagnosticCallback(ResourceManager.DiagnosticEventContext context) {
			// 参照カウントの変更通知だけログ出力する
			if (context.Type == ResourceManager.DiagnosticEventType.AsyncOperationReferenceCount) {
				if (context.Location == null)
					return;
				refCounter_[context.Location] = context.EventValue;
				if (context.EventValue == 0)
					refCounter_.Remove(context.Location);

			}
		}
	}
}
