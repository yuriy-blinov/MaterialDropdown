using System;
using Foundation;
using UIKit;

namespace MaterialDropdown
{
    public interface AnchorView
    {
        UIView PlainView { get; }
    }

    public sealed class UIAnchorView : AnchorView
    {
        private readonly WeakReference<UIView> view;

        public UIAnchorView(UIView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            this.view = new WeakReference<UIView>(view);
        }

        public UIView PlainView
        {
            get
            {
                bool _ = view.TryGetTarget(out UIView target);
                return target;
            }
        }

        public static explicit operator UIAnchorView(UIView view)
        {
            return new UIAnchorView(view);
        }
    }

    public sealed class UIBarButtonItemAnchorView : AnchorView
    {
        private readonly NSString ViewKey = new NSString("view");
        private readonly WeakReference<UIBarButtonItem> view;

        public UIBarButtonItemAnchorView(UIBarButtonItem view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            this.view = new WeakReference<UIBarButtonItem>(view);
        }

        public UIView PlainView
        {
            get
            {
                bool _ = view.TryGetTarget(out UIBarButtonItem target);
                return (UIView)target.ValueForKey(ViewKey);
            }
        }

        public static explicit operator UIBarButtonItemAnchorView(UIBarButtonItem view)
        {
            return new UIBarButtonItemAnchorView(view);
        }
    }
}
