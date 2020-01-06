using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Liberfy.Model;

namespace Liberfy
{
    /// <summary>
    /// IEntityのコレクションクラス
    /// </summary>
    internal class TextEntityCollection : ICollection<IEntity>
    {
        private readonly LinkedList<IEntity> _internalList = new LinkedList<IEntity>();

        /// <summary>
        /// 最初のエンティティ
        /// </summary>
        public IEntity FirstEntity => this._internalList.First?.Value;

        /// <summary>
        /// 最後のエンティティ
        /// </summary>
        public IEntity LastEntity => this._internalList.Last?.Value;

        /// <summary>
        /// 文字エンティティを追加する
        /// </summary>
        /// <param name="content"></param>
        public void AddText(string content)
        {
            if (this.LastEntity is PlainTextEntity textEntity)
            {
                textEntity.DisplayText += content;
            }
            else
            {
                this.Add(new PlainTextEntity(content));
            }
        }
        
        /// <summary>
        /// 改行を追加する
        /// </summary>
        public void AddNewLine()
        {
            this.AddText("\n");
        }

        /// <summary>
        /// 配列を生成する
        /// </summary>
        /// <returns>エンティティ一覧</returns>
        public IEntity[] ToArray()
        {
            var linkedList = this._internalList;
            var entities = new IEntity[linkedList.Count];
            int idx = 0;

            for(var node = linkedList.First; node != null; node = node.Next)
            {
                entities[idx++] = node.Value;
            }

            return entities;
        }

        #region Default Implements

        public int Count => this._internalList.Count;

        public bool IsReadOnly => ((ICollection<IEntity>)this._internalList).IsReadOnly;

        public void Add(IEntity item) => this._internalList.AddLast(item);

        public void Clear() => this._internalList.Clear();

        public bool Contains(IEntity item) => this._internalList.Contains(item);

        public void CopyTo(IEntity[] array, int arrayIndex) => this._internalList.CopyTo(array, arrayIndex);

        public IEnumerator<IEntity> GetEnumerator() => this._internalList.GetEnumerator();

        public bool Remove(IEntity item) => this._internalList.Remove(item);

        IEnumerator IEnumerable.GetEnumerator() => this._internalList.GetEnumerator();

        #endregion Default Implements
    }
}
