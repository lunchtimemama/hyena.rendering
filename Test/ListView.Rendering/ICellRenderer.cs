// ICellRenderer.cs created with MonoDevelop
// User: scott at 11:42 PM 1/26/2009
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;

namespace Test
{
    public interface ICellRenderer<TRenderContext, TModel>
    {
        void RenderCell (IRenderContext<TRenderContext> context, TModel item, int width, int height);
    }
}
