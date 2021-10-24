using Livet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WpfMvvmToolkit;

namespace Liberfy.ViewModels
{
    /// <summary>
    /// ViewModelのベースクラス
    /// </summary>
    internal abstract class ViewModelBase : ViewModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ViewModelBase() : base()
        {
        }

        /// <summary>
        /// コマンド管理
        /// </summary>
        public ViewModelCommandManager Commands { get; set; }

        /// <summary>
        /// 複数のプロパティの変更を通知する。
        /// </summary>
        /// <param name="propertyNames"></param>
        protected void RaisePropertiesChanged(params string[] propertyNames)
        {
            this.RaisePropertiesChanged((IEnumerable<string>)propertyNames);
        }

        /// <summary>
        /// 複数のプロパティの変更を通知する。
        /// </summary>
        /// <param name="propertyNames"></param>
        protected void RaisePropertiesChanged(IEnumerable<string> propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                this.RaisePropertyChanged(propertyName);
            }
        }

        /// <summary>
        /// ViewModelにコマンドを登録する。登録したコマンドはViewModel破棄時に破棄される。
        /// </summary>
        /// <param name="command">登録するコマンド</param>
        /// <returns>Command</returns>
        public T RegisterCommand<T>(T command) where T: IDisposableCommand
            => this.Commands.Add(command);

        /// <summary>
        /// インスタンスを破棄する。
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this.Commands.Dispose();
        }
    }
}
