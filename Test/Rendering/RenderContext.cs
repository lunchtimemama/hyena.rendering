// RenderContext.cs
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
using Pango;

namespace Test
{
    public class RenderContext : IRenderContext
    {
        private readonly IContextProvider context_provider;
        private readonly ILayoutProvider layout_provider;
        private readonly IThemeProvider theme_provider;

        public WidgetRenderContext (IContextProvider contextProvider,
                                    ILayoutProvider layoutProvider,
                                    IThemeProvider themeProvider)
        {
            if (contextProvider == null) throw new ArgumentNullException ("contextProvider");
            if (layoutProvider == null) throw new ArgumentNullException ("layoutProvider");
            if (themeProvider == null) throw new ArgumentNullException ("themeProvider");
            
            this.context_provider = contextProvider;
            this.layout_provider = layoutProvider;
            this.theme_provider = themeProvider;
        }

        Context IRenderContext.Context {
            get { return context_provider.Context; }
        }

        Pango.Layout IRenderContext.Layout {
            get { return layout_provider.Layout; }
        }

        Theme IRenderContext.Theme {
            get { return theme_provider.Theme; }
        }
    }
}
