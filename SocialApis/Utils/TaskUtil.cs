using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Utils
{
    /// <summary>
    /// <see cref="Task"/>に関するUtilクラス
    /// </summary>
    internal static class TaskUtil
    {
        /// <summary>
        /// タスクの終了まで待機し、戻り値を取得する。
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="task">待機するタスク</param>
        /// <returns></returns>
        public static T WaitResult<T>(this Task<T> task)
        {
            task.Wait();
            return task.Result;
        }
    }
}
