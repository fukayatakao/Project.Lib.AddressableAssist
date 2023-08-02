using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Project.Lib {

    //@note アセットバンドルを使うようになったらこのクラスを切り替える？
	/// <summary>
	/// Resourcesに含まれているアセットへのアクセス管理
	/// </summary>
	public static partial class ResourcesAsset
	{

		static Dictionary<int, string> pathDict_;
		/// <summary>
		/// 初期化
		/// </summary>
		public static void Init(Dictionary<int, string> pathDict) {
			pathDict_ = pathDict;
		}

        /// <summary>
        /// リソースをロード
        /// </summary>
        public static T Load<T>(int dir, string resName)
            where T : UnityEngine.Object 
        {
            return Resources.Load<T>(pathDict_[dir] + resName);
        }

        /// <summary>
        /// リソースをフォルダ単位で全ロード
        /// </summary>
        public static T[] LoadAll<T> (int dir, string resName) 
			where T : UnityEngine.Object
		{
			return Resources.LoadAll<T> (pathDict_[dir] + resName);
		}

	}

}
