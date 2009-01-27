// DoubleBufferedWidgetSurface.cs
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
using Gtk;
using Pango;

namespace Test
{
    public class DoubleBufferedWidgetSurface : DoubleBuffer<Context>, IWidgetLayoutProvider
    {
        private DoubleBuffer<Layout> layout_buffer;

        public DoubleBufferedWidgetSurface (Surface surface, int width, int height)
            : base (Surface, width, height)
        {
        }

        Layout IWidgetLayoutProvider.Layout {
            get { return layout_buffer.PrimaryBuffer; }
        }

        void IWidgetLayoutProvider.SetStyle (Widget widget)
        {
            layout_buffer = new DoubleBuffer<Layout> (
                CairoExtensions.CreateLayout (widget, PrimaryBuffer),
                CairoExtensions.CreateLayout (widget, SecondaryBuffer));
        }

        protected override SwapBuffersCore ()
        {
            base.SwapBuffersCore ();
            if (layout_buffer != null) {
                layout_buffer.SwapBuffers ();
            }
        }

        protected override void Dispose (bool disposing)
        {
            base.Dispose (disposing);
            if (disposing && layout_buffer != null) {
                layout_buffer.Dispose ();
            }
        }
    }
}
