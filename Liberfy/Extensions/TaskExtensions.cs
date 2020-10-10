using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Liberfy.Extensions
{
    internal static class TaskExtensions
    {
        public static Task ContinueWithRan(this Task task, Action action)
        {
            return task.ContinueWith(
                task => action(),
                TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public static Task ContinueWithRan<T>(this Task<T> task, Action<T> action)
        {
            return task.ContinueWith(
                task => action(task.Result),
                TaskContinuationOptions.OnlyOnRanToCompletion);
        }
        public static Task<TResult> ContinueWithRan<TResult>(this Task task, Func<TResult> action)
        {
            return task.ContinueWith(
                task => action(),
                TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public static Task<TResult> ContinueWithRan<T, TResult>(this Task<T> task, Func<T, TResult> action)
        {
            return task.ContinueWith(
                task => action(task.Result),
                TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}
