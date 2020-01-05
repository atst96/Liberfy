using Liberfy.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    /// <summary>
    /// 表示用エンティティ生成用クラスのインタフェース
    /// </summary>
    internal interface ITextEntityBuilder
    {
        /// <summary>
        /// 表示用エンティティを生成する
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IEntity> Build();
    }
}
