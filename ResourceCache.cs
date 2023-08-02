using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Lib {
	//@todo これは要らないのでそのうち削除する
	/// <summary>
	/// リソース管理
	/// </summary>
	public static class ResourceCache {
		//ディレクトリ管理なしバージョン
		//リソースキャッシュ
		private static Dictionary<string, UnityEngine.Object> resource_ = new Dictionary<string, UnityEngine.Object>();
		/// <summary>
		/// 即時ロード
		/// </summary>
		public static T Load<T>(string resName, bool isCache = true) where T : UnityEngine.Object {

			T asset;
			//リソースのキャッシュはある場合
			if (resource_.ContainsKey(resName)) {
				asset = (T)resource_[resName];
				//正しくキャッシュされてない
				Debug.Assert(asset != null, "cache error:" + resName);
				//キャッシュされてないリソースを使おうとしたとき
			} else {
				asset = Resources.Load<T>(resName);
				if (isCache) {
					resource_[resName] = asset;
				}
				//アセットのロードに失敗している
				Debug.Assert(asset != null, "asset load error:" + resName);
			}

			return asset;
		}
		/// <summary>
		/// リソース解放
		/// </summary>
		public static void UnLoad(string resName) {
			resource_.Remove(resName);
		}


		//ディレクトリ管理ありバージョン（ディレクトリごとにDictionary分けたバージョン
		//リソースキャッシュ
		private static Dictionary<int, Dictionary<string, UnityEngine.Object>> resourceDict_ = new Dictionary<int, Dictionary<string, UnityEngine.Object>>();
        /// <summary>
        /// 即時ロード
        /// </summary>
        public static T Load<T>(int directory, string resName, bool isCache = true) where T : UnityEngine.Object    
        {
            int dir = (int)directory;
            //キャッシュDictionaryにkeyがないパスの中にあるリソースはキャッシュしない
            if (!resourceDict_.ContainsKey(dir) && isCache) {
				resourceDict_[dir] = new Dictionary<string, UnityEngine.Object>();
			}

            T asset;
            Dictionary<string, UnityEngine.Object> resourceDict = resourceDict_[dir];
            //リソースのキャッシュはある場合
            if (resourceDict != null && resourceDict.ContainsKey(resName)) {
                asset = (T)resourceDict[resName];
                //正しくキャッシュされてない
                Debug.Assert(asset != null, "cache error:" + resName);
            //キャッシュされてないリソースを使おうとしたとき
            } else {
                asset = ResourcesAsset.Load<T>(dir, resName);
                if (isCache) {
                    resourceDict[resName] = asset;
                }
                //アセットのロードに失敗している
                Debug.Assert(asset != null, "asset load error:" + resName);
            }

            return asset;
        }
        /// <summary>
        /// リソース解放
        /// </summary>
        public static void UnLoad(int dir, string resName) {
            resourceDict_[dir].Remove(resName);
        }

		public static void Clear() {
			resource_.Clear();
			resourceDict_.Clear();
			//使ってないリソースを解放
			Resources.UnloadUnusedAssets();
		}
	}
}
