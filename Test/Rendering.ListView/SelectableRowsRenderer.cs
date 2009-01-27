// SelectableRowRenderer.cs
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
using System.Collections.Generic;
using Gtk;

namespace Test
{
    public class SelectableRowsRenderer : BaseListRenderer
    {
        private readonly ISelectableList list;
        private readonly IRowRenderer row_renderer;
        
        public SelectableRowRenderer (IListRenderer listRenderer, IRowRenderer rowRenderer, ISelectableList list)
            : base (listRenderer)
        {
            this.list = list;
            this.row_renderer = rowRenderer;
        }
        
        public override void RenderRows (IRenderContext context, int startIndex, int endIndex, int width)
        {
            context.Context.Save ();
            
            Selection selection = list.Selection;
            int row_height = list.RowHeight;
            int selection_start = 0;
            int selection_length = 0;
            int focused_index = -1;

            for (int row_index = startIndex; row_index < endIndex; row_index++) {
                if (selection != null && selection.Contains (row_index)) {
                    if (selection_length == 0) {
                        selection_start = row_index;
                    }
                    selection_length++;
                    if (selection.FocusedIndex == row_index) {
                        focused_index = row_index;
                    }
                } else {
                    if (selection != null && selection.FocusedIndex == row_index /* && HasFocus */) { // TODO
                        CairoCorners corners = CairoCorners.All;
                        if (selection_length > 0) {
                            corners &= ~(CairoCorners.TopLeft | CairoCorners.TopRight);
                        }
                        if (Selection.Contains (row_index + 1)) {
                            corners &= ~(CairoCorners.BottomLeft | CairoCorners.BottomRight);
                        }
                        context.Theme.RenderRowSelection (context, width, rowHeight, false);
                    }
                    if (selection_length > 0) {
                        RenderSelection (context, selection_start, selection_length, width);
                        selection_length = 0;
                    }
                    row_renderer.RenderRow (context, row_index, StatusType.Normal);
                }
                context.Context.Translate (0, list.RowHeight);
            }
            if (selection_length > 0) {
                RenderSelection (context, selection_start, selection_length, width, list.RowHeight);
            }

            context.Context.Restore ();

            base.RenderRows (context, startIndex, endIndex);
        }

        private void RenderSelection (IRenderContext context, int selection_start,
                                      int selection_length, int focusedIndex, int width)
        {
            int rowHeight = list.RowHeight;
            context.Context.Translate (0, selection_length * rowHeight);
            context.Theme.RenderRowSelection (context, width, selection_length * rowHeight);
            
            while (selection_length > 0) {
                if (selection_start == focusedIndex) {
                    context.Theme.RenderRowSelection (context, width, rowHeight, false);
                }
                row_renderer (context, selection_start, StatusType.Selected, width, rowHeight);
                context.Context.Translate (0, rowHeight);
                selection_start++;
                selection_length--;
            }
        }
    }
}
