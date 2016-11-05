using System;

using Foundation;
using UIKit;

namespace MaterialDropdown.Example
{
	public partial class CustomDropdownCell : DropdownCell
	{
		public static readonly NSString Key = new NSString("CustomDropdownCell");
		public static readonly UINib Nib;

		static CustomDropdownCell()
		{
			Nib = UINib.FromName("CustomDropdownCell", NSBundle.MainBundle);
		}

		public override UILabel TitleLabel => titleLabel;
		public UILabel SubtitleLabel => subtitleLabel;
		public UIImageView IconImage => iconImage;

		protected CustomDropdownCell(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}
	}
}
