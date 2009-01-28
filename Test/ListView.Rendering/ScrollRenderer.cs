// ScrollRenderer.cs
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
    public abstract class ScrollRenderer<TRenderContext> : BaseRenderer<TRenderContext>
    {
        // TODO do we make IRenderContext mutable and get rid of this?
        private struct RenderContextShim : IRenderContext<IRenderContext>
        {
            private readonly Context context;
            private readonly IRenderContext<IRenderContext> source_context;
            
            public RenderContextShim (Context context, IRenderContext<IRenderContext> sourceContext)
            {
                this.context = context;
                this.source_context = sourceContext;
            }

            Context IRenderContext.Context {
                get { return context; }
            }

            TRenderContext IRenderContext<TRenderContext>.ExtendedContext {
                get { return source_context.ExtendedContext; }
            }
        }
        
        private readonly IList list;
        private readonly IListRenderer<TRenderContext> list_renderer;
        
        private int buffer_top_row;
        private int buffer_bottom_row;
        private bool render_everything;

        public ScrollRenderer(IList list, IListRenderer<TRenderContext> listRenderer)
            : this (list, listRenderer, null)
        {
        }
        
        public ScrollRenderer(IList list,IListRenderer<TRenderContext> listRenderer,
                              IRenderer<TRenderContext> nextRenderer)
            : base (nextRenderer)
        {
            this.list = list;
            this.list_renderer = listRenderer;
        }

        public void Render (IRenderContext<TRenderContext> context)
        {
            RenderRows (context);

            Context cairo_context = context.CairoContext;
            cairo_context.Rectangle (clip.X, clip.Y, clip.Width, clip.Height);
            cairo_context.Clip ();
            cairo_context.SetSourceSurface (
                SurfaceBuffer.PrimaryBuffer.Target,
                list.Allocation.X - list.HorizontalOffset,
                list.Allocation.Y - (list.VerticalOffset % list.RowHeight));
            cairo_context.Paint ();

            render_everything = false;
            base.RenderList (context);
        }

        private void RenderRows (IRenderContext<TRenderContext> context)
        {
            top = list.VerticalOffset / list.RowHeight;
            bottom = top_row + list.RowsInView;

            if (!render_everything && top == buffer_top_row && bottom == buffer_bottom_row) {
                return;
            }

            SurfaceDoubleBuffer.SwapBuffers ();

            Context cairo_context = DoubleBufferedSurface.Context;
            cairo_context.Operator = Operator.Source;
			cairo_context.Color = new Color (1, 1, 1);
            cairo_context.Paint ();
			cairo_context.Operator = Cairo.Operator.Over;

            RenderRows (context, top, bottom);

            buffer_top_row = top;
            buffer_bottom_row = bottom;
        }

        private void RenderRows (IRenderContext<TRenderContext> context, int topRow, int bottomRow)
        {
            int orginalTopRow = topRow;
            int row_height = ListRenderer.RowHeight;
            Context cairo_context = SurfaceDoubleBuffer.Context;
            cairo_context.Save ();
            
            if (render_everything) {
                int delta = buffer_top_row - topRow;
                if (Math.Abs (delta) < list.RowsInView) {
                    cairo_context.SetSourceSurface (DoubleBufferedSurface.SecondaryBuffer, 0, delta * row_height);
                    cairo_context.Paint ();
                    topRow = GetTopRow (topRow, buffer_bottom_row, delta);
                    bottomRow = GetBottomRow (bottomRow, buffer_top_row, delta);
                    if (delta < 0) {
                        cairo_context.Translate (0, (topRow - orginalTopRow) * list.RowHeight);
                    }
                }
            }
            
            list_renderer.RenderRows (new RenderContextShim (cairo_context, context),
                                      topRow, bottomRow,
                                      DoubleBufferedSurface.Width);
            cairo_context.Restore ();
        }

        protected virtual int GetTopRow (int topRow, int delta)
        {
            return delta > 0 ? topRow : buffer_bottom_row + 1;
        }

        protected virtual int GetBottomRow (int bottomRow, int delta)
        {
            return delta < 0 ? bottomRow : buffer_top_row - 1;
        }

        // TODO make this class render-target independant so the same logic can be used with pixbufs
        // TODO make sure the height jives
        protected abstract DoubleBufferedSurface DoubleBufferedSurface { get; }

        public void QueueFullRender ()
        {
            render_everything = true;
        }
    }
}
