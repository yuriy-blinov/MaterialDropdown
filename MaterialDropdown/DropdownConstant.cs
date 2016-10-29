using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace MaterialDropdown
{
	internal static class DropdownConstant
	{
		internal static class KeyPath
		{
			internal static string Frame = "frame";
		}

		internal static class ReusableIdentifier
		{
			internal static NSString DropdownCell = new NSString("DropdownCell");
		}

		internal static class UI
		{
			internal static UIColor TextColor = UIColor.Black;
			internal static UIFont TextFont = UIFont.SystemFontOfSize(15);
			internal static UIColor BackgroundColor = new UIColor(white: 0.94f, alpha: 1);
			internal static UIColor SelectionBackgroundColor = new UIColor(white: 0.89f, alpha: 1);
			internal static UIColor SeparatorColor = UIColor.Clear;
			internal static nfloat CornerRadius = 2;
			internal static nfloat RowHeight = 44;
			internal static nfloat HeightPadding = 20;

			internal static class Shadow
			{
				internal static UIColor Color = UIColor.DarkGray;
				internal static CGSize Offset = CGSize.Empty;
				internal static float Opacity = 0.4f;
				internal static nfloat Radius = 8f;
			}
		}

		internal static class Animation
		{

			internal static float Duration = 0.15f;
			internal static UIViewAnimationOptions EntranceOptions = UIViewAnimationOptions.AllowUserInteraction |
			                                                UIViewAnimationOptions.CurveEaseOut;
			internal static UIViewAnimationOptions ExitOptions = UIViewAnimationOptions.AllowUserInteraction |
			                                            UIViewAnimationOptions.CurveEaseIn;
			internal static CGAffineTransform DownScaleTransform = new CGAffineTransform(0,0,0,0, 0.9f, 0.9f);
		}
	}
}
