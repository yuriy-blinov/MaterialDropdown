using System;
using Foundation;
using UIKit;

namespace MaterialDropdown
{
	[Register("DropdownButton")]
	public class DropdownButton: UIButton
	{
		// Required for the Xamarin iOS Desinger
		public DropdownButton() : base() { }

		public DropdownButton(IntPtr handle) : base(handle){ }

		public UIView view;

		UIColor borderColor = new UIColor(red: 0.6494f, green: 0.8155f, blue: 1.0f, alpha: 1.0f);
		public UIColor BorderColor{
			get{
				return borderColor;
			}

			set{
				borderColor = value;
				if (view != null) { 
					view.BackgroundColor = borderColor;
				}
			}
		}

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();

			view = new UIView();
			view.BackgroundColor = borderColor;

			view.TranslatesAutoresizingMaskIntoConstraints = false;
			AddSubview(view);

			view.AddConstraint(NSLayoutConstraint.Create(
				view1: view,
				attribute1: NSLayoutAttribute.Height,
				relation: NSLayoutRelation.Equal,
				view2: null,
				attribute2: NSLayoutAttribute.Height,
				multiplier: 1,
				constant: 1
				)
			);

			AddConstraint(NSLayoutConstraint.Create(
				view1: view,
				attribute1: NSLayoutAttribute.Left,
				relation: NSLayoutRelation.Equal,
				view2: this,
				attribute2: NSLayoutAttribute.Left,
				multiplier: 1,
				constant: 0
				)
			);

			AddConstraint(NSLayoutConstraint.Create(
				view1: view,
				attribute1: NSLayoutAttribute.Right,
				relation: NSLayoutRelation.Equal,
				view2: this,
				attribute2: NSLayoutAttribute.Right,
				multiplier: 1,
				constant: 0
				)
			);

			AddConstraint(NSLayoutConstraint.Create(
				view1: view,
				attribute1: NSLayoutAttribute.Bottom,
				relation: NSLayoutRelation.Equal,
				view2: this,
				attribute2: NSLayoutAttribute.Bottom,
				multiplier: 1,
				constant: 0
				)
			);
		}
	}
}
