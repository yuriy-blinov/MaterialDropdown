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
		private readonly UIView view;

		public UIAnchorView(UIView view)
		{
			if (view == null)
				throw new ArgumentNullException(nameof(view));
			
			this.view = view;
		}

		public UIView PlainView { get { return view;} }

		public static explicit operator UIAnchorView(UIView view){
			return new UIAnchorView(view);
		}
	}

	public sealed class UIBarButtonItemAnchorView : AnchorView
	{
		private readonly NSString ViewKey = new NSString("view");
		private readonly UIBarButtonItem view;

		public UIBarButtonItemAnchorView(UIBarButtonItem view)
		{
			if (view == null)
				throw new ArgumentNullException(nameof(view));

			this.view = view;
		}

		public UIView PlainView { get { return (UIView)view.ValueForKey(ViewKey); } }

		public static explicit operator UIBarButtonItemAnchorView(UIBarButtonItem view)
		{
			return new UIBarButtonItemAnchorView(view);
		}
	}
}
