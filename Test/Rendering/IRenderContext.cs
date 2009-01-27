// IRenderer.cs created with MonoDevelop
// User: scott at 11:45 PM 1/26/2009
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using Cairo;
using Pango;

namespace Test
{
    public interface IRenderContext
    {
        Context Context { get; }
        Layout Layout { get; }
        Theme Theme { get; }
    }
}
