using System;
using UnityEngine;

namespace Assets.EditorFramework.Editor
{
    public abstract class GUIBase : IDisposable
    {
        protected bool mDisposed { get; private set; }
        protected Rect mPosition { get;  set; }

        public virtual void Dispose()
        {
            if (mDisposed) return;
            OnDispose();
            mDisposed = true;

        }

        public virtual void OnGUI(Rect position)
        {
            mPosition = position;
        }

        protected abstract void OnDispose();
    }
}
