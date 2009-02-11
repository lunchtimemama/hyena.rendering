// DoubleBufferedContext.cs
//
// Copyright (c) 2009 [copyright holders]
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//

using System;

namespace Test
{
    public class DoubleBufferedContext<T> : IDoubleBufferedContext<T>, IDisposable where T : IDisposable
    {
        private readonly T primary_context;
        private readonly T secondary_context;

        public DoubleBufferedContext (T primaryContext, T secondaryContext)
        {
            primary_context = primaryContext;
            secondary_context = secondaryContext;
        }

		T IContext<T>.Context {
			get { return primary_context; }
		}

        T IDoubleBuffer<T>.PrimaryContext {
            get { return primary_context; }
        }

        T IDoubleBuffer<T>.SecondaryContext {
            get { return secondary_context; }
        }

        void IDoubleBuffer<T>.SwapContexts ()
        {
            SwapContextsCore ();
        }

        protected virtual void SwapContextsCore ()
        {
            T temporary_context = primary_context;
            primary_context = secondary_context;
            secondary_context = temporary_context;
        }

        void IDisposable.Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        protected virtual void Dispose (bool disposing)
        {
            if (disposing) {
                if (primary_context != null) {
                    primary_context.Dispose ();
                }
                if (secondary_context != null) {
                    secondary_context.Dispose ();
                }
            }
        }
    }
}
