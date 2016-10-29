using CoreGraphics;
using UIKit;

namespace MaterialDropdown
{
	public static class UIViewExtensions
	{
		public static CGRect? WindowFrame(this UIView view)
		{
			return view.Superview?.ConvertRectToView(view.Frame, null);
		}
	}
}