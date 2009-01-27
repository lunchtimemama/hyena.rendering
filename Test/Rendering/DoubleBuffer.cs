// DoubleBufferedSurface.cs
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
using Cairo;

namespace Test
{
    public class DoubleBuffer<T> : IDoubleBuffer<T> where T : IDisposable
    {
        private readonly T primary_buffer;
        private readonly T secondary_buffer;

        public DoubleBuffer (T primaryBuffer, T secondaryBuffer)
        {
            this.primary_buffer = primaryBuffer;
            this.secondary_buffer = secondaryBuffer;
        }

        public T PrimaryBuffer {
            get { return primary_buffer; }
        }

        public T SecondaryBuffer {
            get { return secondary_buffer; }
        }

        public void SwapBuffers ()
        {
            SwaptBuffersCore ();
        }

        protected virtual void SwapBuffersCore ()
        {
            T temporary_buffer = primary_buffer;
            primary_buffer = secondary_buffer;
            secondary_buffer = temporary_buffer;
        }

        public void Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        protected virtual void Dispose (bool disposing)
        {
            if (disposing) {
                if (primary_buffer != null) {
                    primary_buffer.Dispose ();
                }
                if (secondary_buffer != null) {
                    secondary_buffer.Dispose ();
                }
            }
        }
    }
}
