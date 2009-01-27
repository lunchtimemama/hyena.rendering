// IRowRenderer.cs created with MonoDevelop
// User: scott at 11:39 PM 1/26/2009
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using Gtk;

namespace Test
{
    public interface IRowRenderer
    {
        void RenderRow (IRenderContext context, int rowIndex, StateType statusType, int width, int height);
    }
}
