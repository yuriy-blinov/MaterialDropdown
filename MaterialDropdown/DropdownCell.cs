using System;

using Foundation;
using UIKit;

namespace MaterialDropdown
{
	public partial class DropdownCell : UITableViewCell
	{
		private const bool WithoutAnimation = false;

		public double SelectedAnimationDuration = 0.3;

		public static readonly NSString Key = new NSString("DropdownCell");
		public static readonly UINib Nib;
		public UIColor SelectedColor { get; set; }

		static DropdownCell()
		{
			Nib = UINib.FromName("DropdownCell", NSBundle.MainBundle);
		}

		protected DropdownCell(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override bool Selected
		{
			get
			{
				return base.Selected;
			}
			set
			{
				SetSelected(value, WithoutAnimation);
				base.Selected = value;
			}
		}

		public override bool Highlighted
		{
			get
			{
				return base.Highlighted;
			}
			set
			{
				SetSelected(value, WithoutAnimation);
				base.Highlighted = value;
			}
		}

		public override void SetHighlighted(bool highlighted, bool animated)
		{
			SetSelected(highlighted, animated);
		}

		public override void SetSelected(bool selected, bool animated)
		{
			if (animated)
			{
				Animate(SelectedAnimationDuration, () => ExecuteSelection(selected));
			}
			else {
				ExecuteSelection(selected);
			}

		}

		protected void ExecuteSelection(bool selected)
		{
			if (SelectedColor != null && selected)
			{
				BackgroundColor = SelectedColor;
			}
			else {
				BackgroundColor = UIColor.Clear;
			}
		}

		public virtual UILabel TitleLabel{ get { return Title;}}
	}
}
