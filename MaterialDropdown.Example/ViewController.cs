using System;
using CoreGraphics;
using UIKit;

namespace MaterialDropdown.Example
{
	public partial class ViewController : UIViewController
	{
		Dropdown amountDropDown = new Dropdown();
		protected ViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.
			amountButton.TouchUpInside+= (sender, e) => amountDropDown.Show();
			setupAmountDropDown();
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

		private void setupAmountDropDown()
		{
			amountDropDown.AnchorView = (UIAnchorView)amountButton;
			amountDropDown.DismissMode = DismissMode.Automatic;
			// By default, the dropdown will have its origin on the top left corner of its anchor view
			// So it will come over the anchor view and hide it completely
			// If you want to have the dropdown underneath your anchor view, you can do this:
			amountDropDown.BottomOffset = new CGPoint(x: 0, y: amountButton.Bounds.Height);

			// You can also use localizationKeysDataSource instead. Check the docs.
			amountDropDown.DataSource = new[]{
			"10 €",
			"20 €",
			"30 €",
			"40 €",
			"50 €",
			"60 €",
			"70 €",
			"80 €",
			"90 €",
			"100 €",
			"110 €",
			"120 €"
			};

			// Action triggered on selection
			amountDropDown.ItemSelected += (sender, e) => this.amountButton.SetTitle(e.Item.ToString(), UIControlState.Normal);
		}
	}
}
