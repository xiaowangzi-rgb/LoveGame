using System;
using System.Collections.Generic;
using Google.Protobuf.Collections;

namespace MM.Data {

    public class RepeatedFieldRecordList<T> : RecordList<T, RepeatedField<T>> {
        public RepeatedFieldRecordList(RepeatedField<T> data, Action<bool> action) : base(data, action) {
        }

        public override void AddRange(IList<T> l) {
            list.AddRange(l);
            Invoke();
        }

        public override void RemoveRange(int index, int count) {
            //list.RemoveRange(index, count);
            Invoke();
        }
    }

    public class ListRecordList<T> : RecordList<T, List<T>> {
        public ListRecordList(List<T> data, Action<bool> action) : base(data, action) {
        }

        public override void AddRange(IList<T> l) {
            list.AddRange(l);
            Invoke();
        }

        public override void RemoveRange(int index, int count) {
            list.RemoveRange(index, count);
            Invoke();
        }
    }

    /// <summary>
    /// 存档的List
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RecordList<T, K> where K : IList<T> {

        public RecordList(K data, Action<bool> action) {
            modifyAction = action;
            list = data;
        }

        /// <summary>
        /// 具体的数据
        /// </summary>
        protected K list { get; private set; }

        /// <summary>
        /// 数据改变时的回调
        /// </summary>
        private Action<bool> modifyAction;

        public void Add(T t) {
            if (t == null) {
                return;
            }
            list.Add(t);
            Invoke();
        }

        public void Add(T t, bool isSynEvent) {
            if (t == null) {
                return;
            }
            list.Add(t);
            Invoke(isSynEvent);
        }

        public void Remove(T t) {
            if (t == null) {
                return;
            }
            list.Remove(t);
            Invoke();
        }

        public abstract void RemoveRange(int index, int count);

        public void Remove(T t, bool isSynEvent) {
            if (t == null) {
                return;
            }
            list.Remove(t);
            Invoke(isSynEvent);
        }

        public void RemoveAt(int i) {
            list.RemoveAt(i);
            Invoke();
        }

        public void Clear() {
            list.Clear();
            Invoke();
        }

        public abstract void AddRange(IList<T> l);

        public bool Contains(T t) {
            return list.Contains(t);
        }

        public T Find(Predicate<T> match) {
            var index = FindIndex(match);
            if (index == -1) {
                return default(T);
            }
            return list[index];
        }

        public int FindIndex(Predicate<T> match) {
            for (int i = 0; i < list.Count; ++i) {
                if (match(list[i])) {
                    return i;
                }
            }
            return -1;
        }

        public T this[int index] {
            get { return list[index]; }
            set { list[index] = value; Invoke(); }
        }

        public int Count {
            get { return list.Count; }
        }

        public void Invoke() {
            modifyAction?.Invoke(true);
        }

        public void Invoke(bool isSynEvent) {
            modifyAction?.Invoke(isSynEvent);
        }

        public IList<T> GetList() {
            return list;
        }

        public IList<T> GetNewList() {
            return new List<T>(list);
        }
    }
}
