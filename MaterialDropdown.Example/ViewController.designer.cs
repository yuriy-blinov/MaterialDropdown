// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace MaterialDropdown.Example
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MaterialDropdown.DropdownButton amountButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MaterialDropdown.DropdownButton anchorlessButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MaterialDropdown.DropdownButton customCellDropdownButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem RightBarButtonItem { get; set; }

        [Action ("RightBarButtonItem_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void RightBarButtonItem_Activated (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (amountButton != null) {
                amountButton.Dispose ();
                amountButton = null;
            }

            if (anchorlessButton != null) {
                anchorlessButton.Dispose ();
                anchorlessButton = null;
            }

            if (customCellDropdownButton != null) {
                customCellDropdownButton.Dispose ();
                customCellDropdownButton = null;
            }

            if (RightBarButtonItem != null) {
                RightBarButtonItem.Dispose ();
                RightBarButtonItem = null;
            }
        }
    }
}