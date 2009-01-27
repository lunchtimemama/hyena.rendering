// WidgetRenderContext.cs
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
    public class WidgetRenderContext : RenderContext
    {
        private readonly IWidgetLayoutProvider layout_provider;
        private readonly IWidgetThemeProvider theme_provider;
        
        public WidgetRenderContext (IContextProvider contextProvider,
                                    IWidgetLayoutProvider layoutProvider,
                                    IWidgetThemeProvider themeProvider)
            : base (contextProvider, layoutProvider, themeProvider)
        {
            this.layout_provider = layoutProvider;
            this.theme_provider = themeProvider;
        }

        public void SetStyle (Widget widget)
        {
            layout_provider.SetStyle (widget);
            theme_provider.SetStyle (widget);
        }
    }
}
