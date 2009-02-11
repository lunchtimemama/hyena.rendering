// SelectableListScrollRenderer.cs
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
    public abstract class SelectableListScrollRenderer<TRenderContext, TDoubleBufferedContext>
        : ListScrollRenderer<TRenderContext, TDoubleBufferedContext>
        where TContext : IContext<Context>
        where TDoubleBufferedContext : TContext, IContext<IDoubleBufferedSurface>
    {
        private readonly ISelectableList list;
        
        public SelectionScrollRenderer(ISelectableList list, TDoubleBufferedContext doubleBufferedContext,
                                       IListRenderer<TRenderContext> nextRenderer)
            : base (rowRenderer, doubleBufferedContext, nextRenderer)
        {
            this.list = list;
        }

        protected override int GetTopRow (int topRow, int delta)
        {
            if (list.Selection == null) {
                return base.GetTopRow (topRow, delta);
            }
            
            if (delta > 0) {
                return topRow;
            } else {
                int previousBottomRow = topRow + delta + list.RowsInView - 1; // FIXME is this right?
                while (list.Selection.Contains (previousBottomRow) && previousBottomRow > topRow) {
                    previousBottomRow--;
                }
                return previousBottomRow;
            }
        }

        protected override int GetBottomRow (int bottomRow, int delta)
        {
            if (list.Selection == null) {
                return base.GetBottomRow (bottomRow, delta);
            }
            
            if (delta < 0) {
                return bottomRow;
            } else {
                int previousTopRow = bottomRow + delta - list.RowsInView + 1; // FIXME is this right?
                while (list.Selection.Contains (previousTopRow) && previousTopRow < bottomRow) {
                    previousTopRow++;
                }
                return previousTopRow;
            }
        }
    }
}
