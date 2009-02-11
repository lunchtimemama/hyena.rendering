// ListScrollRenderer.cs
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
    public abstract class ListScrollRenderer<TContext, TDoubleBufferedContext> : BaseListRenderer<TContext>
		where TContext : IContext<Context>
        where TDoubleBufferedContext : TContext, IContext<IDoubleBufferedSurface>
    {
        private readonly IList list;
        private readonly TDoubleBufferedContext double_buffered_context;
        
        private int buffer_top_row;
        private int buffer_bottom_row;
        private bool render_everything;
        
        private IDoubleBufferedSurface DoubleBuffer {
            get { return double_buffered_context.Context; }
        }
        
        public ListScrollRenderer(IList list, TDoubleBufferedContext doubleBufferedContext,
                              IListRenderer<TRenderContext> nextRenderer)
            : base (nextRenderer)
        {
            this.list = list;
            this.double_buffered_context = doubleBufferedContext;
        }
        
        protected override void RenderRows (TContext context, int startIndex, int endIndex, int width)
        {
            RenderRows (startIndex, endIndex);
            
            Context cairo_context = context.Context;
            cairo_context.SetSourceSurface (
                DoubleBuffer.PrimaryContext.Target,
                list.Allocation.X - list.HorizontalOffset,
                list.Allocation.Y - (list.VerticalOffset % list.RowHeight));
            cairo_context.Paint ();

            render_everything = false;
        }
        
        private void RenderRows (int startIndex, int endIndex)
        {
            if (!render_everything && startIndex == buffer_top_row && endIndex == buffer_bottom_row) {
                return;
            }
            
            DoubleBuffer.SwapBuffers ();
            
            Context cairo_context = DoubleBuffer.PrimaryContext;
            cairo_context.Operator = Operator.Source;
            cairo_context.Color = new Color (1, 1, 1);
            cairo_context.Paint ();
            cairo_context.Operator = Cairo.Operator.Over;
            
            int orginalStartIndex = startIndex;
            int row_height = list.RowHeight;
            
            cairo_context.Save ();
            
            if (!render_everything) {
                int delta = buffer_top_row - startIndex;
                if (Math.Abs (delta) < list.RowsInView) {
                    cairo_context.SetSourceSurface (DoubleBufferedSurface.SecondaryContext, 0, delta * row_height);
                    cairo_context.Paint ();
                    startIndex = GetTopRow (startIndex, buffer_bottom_row, delta);
                    endIndex = GetBottomRow (endIndex, buffer_top_row, delta);
                    if (delta < 0) {
                        cairo_context.Translate (0, (startIndex - orginalStartIndex) * row_height);
                    }
                }
            }
            
            base.RenderRows (double_buffered_context, startIndex, endIndex, DoubleBuffer.Width);
            
            cairo_context.Restore ();
            
            buffer_top_row = startIndex;
            buffer_bottom_row = endIndex;
        }

        protected virtual int GetTopRow (int topRow, int delta)
        {
            return delta > 0 ? topRow : buffer_bottom_row + 1;
        }

        protected virtual int GetBottomRow (int bottomRow, int delta)
        {
            return delta < 0 ? bottomRow : buffer_top_row - 1;
        }

        public void QueueFullRender ()
        {
            render_everything = true;
        }
    }
}
