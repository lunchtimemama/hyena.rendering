// SelectableListRenderer.cs
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
using Cairo;
using Gtk;

namespace Test
{
    public class SelectableListRenderer<TRenderContext> : BaseListRenderer<TRenderContext>
        where TRenderContext : IThemeContext
    {
        private readonly ISelectableList list;
        private readonly IRowRenderer row_renderer;

        public SelectableListRenderer (IRowRenderer rowRenderer, ISelectableList list)
            : this (rowRenderer, list)
        {
        }
        
        public SelectableListRenderer (IRowRenderer rowRenderer, ISelectableList list,
                                      IListRenderer<TRenderContext> nextRenderer)
            : base (nextRenderer)
        {
            this.list = list;
            this.row_renderer = rowRenderer;
        }
        
        public override void RenderRows (IRenderContext<TRenderContext> context,
                                         int startIndex, int endIndex, int width)
        {
            Context cairo_context = context.CairoContext;
            cairo_context.Context.Save ();
            
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
                        context.ExtendedContext.Theme.RenderRowSelection (context, width, rowHeight, false);
                    }
                    if (selection_length > 0) {
                        RenderSelection (context, selection_start, selection_length, width);
                        selection_length = 0;
                    }
                    row_renderer.RenderRow (context, row_index, StatusType.Normal);
                }
                cairo_context.Translate (0, list.RowHeight);
            }
            if (selection_length > 0) {
                RenderSelection (context, selection_start, selection_length, width, list.RowHeight);
            }

            cairo_context.Restore ();

            base.RenderRows (context, startIndex, endIndex);
        }

        private void RenderSelection (IRenderContext<TRenderContext> context, int selection_start,
                                      int selection_length, int focusedIndex, int width)
        {
            int rowHeight = list.RowHeight;
            Theme theme = context.ExtendedContext.Theme;
            Context cairo_context = context.CairoContext;
            
            cairo_context.Translate (0, selection_length * rowHeight);
            theme.RenderRowSelection (context, width, selection_length * rowHeight);
            
            while (selection_length > 0) {
                if (selection_start == focusedIndex) {
                    theme.RenderRowSelection (context, width, rowHeight, false);
                }
                row_renderer (context, selection_start, StatusType.Selected, width, rowHeight);
                cairo_context.Translate (0, rowHeight);
                selection_start++;
                selection_length--;
            }
        }
    }
}
