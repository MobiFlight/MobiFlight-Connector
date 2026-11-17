using Microsoft.Web.WebView2.Core;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.WebView
{
    /// <summary>
    /// Issue 3209: Reduces the default WebView2 context menu to clipboard items.
    /// Text fields get Cut, Copy, Paste and Select all, selected text gets Copy,
    /// everywhere else no menu is shown.
    /// </summary>
    internal class ClipboardContextMenuHandler
    {
        static readonly string[] EditableItemNames = ["cut", "copy", "paste", "selectAll"];
        static readonly string[] SelectionItemNames = ["copy"];

        internal void RegisterWithWebView(CoreWebView2 webView)
        {
            webView.ContextMenuRequested += ContextMenuRequested;
        }

        private void ContextMenuRequested(object sender, CoreWebView2ContextMenuRequestedEventArgs e)
        {
            var target = e.ContextMenuTarget;
            var allowedNames = GetAllowedItemNames(target.IsEditable, target.HasSelection);

            var menuItems = e.MenuItems;

            // Remove by index, the wrapper objects are not stable references; this also drops separators
            for (var i = menuItems.Count - 1; i >= 0; i--)
            {
                if (!allowedNames.Contains(menuItems[i].Name))
                {
                    menuItems.RemoveAt(i);
                }
            }

            var hasNoItemsLeft = menuItems.Count == 0;
            if (hasNoItemsLeft)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Returns the names of the default menu items to keep for a context menu target.
        /// </summary>
        internal static IReadOnlyCollection<string> GetAllowedItemNames(bool isEditable, bool hasSelection)
        {
            if (isEditable) return EditableItemNames;
            if (hasSelection) return SelectionItemNames;
            return [];
        }
    }
}
