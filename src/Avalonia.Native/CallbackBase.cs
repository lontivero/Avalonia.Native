﻿using System;
using SharpGen.Runtime;

namespace Avalonia.Native
{
    public class CallbackBase : SharpGen.Runtime.IUnknown
    {
        uint _refCount;
        bool _disposed;
        object _lock = new object();
        private ShadowContainer _shadow;

        public CallbackBase()
        {
            _refCount = 1;
        }

        public ShadowContainer Shadow
        {
            get => _shadow;
            set
            {
                lock (_lock)
                {
                    if (_disposed && value != null)
                        throw new ObjectDisposedException("CallbackBase");
                    _shadow = value;
                }
            }
        }

        public uint AddRef()
        {
            lock (_lock)
                return ++_refCount;
        }

        public void Dispose()
        {
            lock (_lock)
                if (!_disposed)
                {
                    _disposed = true;
                    Release();
                }
        }

        public uint Release()
        {
            lock (_lock)
            {
                _refCount--;
                if (_refCount == 0)
                {
                    Shadow?.Dispose();
                    Shadow = null;
                    Destroyed();
                }
                return _refCount;
            }
        }

        protected virtual void Destroyed()
        {

        }
    }
}
