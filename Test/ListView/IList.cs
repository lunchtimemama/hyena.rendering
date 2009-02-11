// IListView.cs created with MonoDevelop
// User: scott at 11:55 PM 1/26/2009
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using Gdk;

namespace Test
{
    public interface IList : IScrollable
    {
        int RowHeight { get; }
        int RowsInView { get; }
        Rectangle Allocation { get; }
    }
}
