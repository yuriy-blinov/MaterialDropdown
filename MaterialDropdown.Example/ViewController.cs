using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace MaterialDropdown.Example
{
	public partial class ViewController : UIViewController
	{
		Dropdown amountDropdown = new Dropdown();
		Dropdown anchorlessDropdown = new Dropdown();
		Dropdown menuDropdown = new Dropdown();
		Dropdown customCellDropdown = new Dropdown();

		protected ViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.
			amountButton.TouchUpInside+= (sender, e) => amountDropdown.Show();
			anchorlessButton.TouchUpInside += (sender, e) => anchorlessDropdown.Show();
			customCellDropdownButton.TouchUpInside += (sender, e) => customCellDropdown.Show();

			this.NavigationItem.SetRightBarButtonItem(RightBarButtonItem, false);

			setupMenuDropDown();
			setupAmountDropDown();
			setupAnchorlessDropDown();
			setupCustomCellDropdown();
		}

		partial void RightBarButtonItem_Activated(UIBarButtonItem sender)
		{
			menuDropdown.Show();
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

		private void setupAmountDropDown()
		{
			amountDropdown.AnchorView = (UIAnchorView)amountButton;
			amountDropdown.DismissMode = DismissMode.Automatic;
			// By default, the dropdown will have its origin on the top left corner of its anchor view
			// So it will come over the anchor view and hide it completely
			// If you want to have the dropdown underneath your anchor view, you can do this:
			amountDropdown.BottomOffset = new CGPoint(x: 0, y: amountButton.Bounds.Height);
			amountDropdown.DataSource = new[]{
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
			amountDropdown.SelectedItemChanged += (sender, e) => this.amountButton.SetTitle(e.Item.ToString(), UIControlState.Normal);
		}

		private void setupAnchorlessDropDown()
		{
			anchorlessDropdown.DismissMode = DismissMode.Automatic;
			anchorlessDropdown.DataSource = new[]{
				"English",
				"German",
				"Ukrainian"
			};
		}

		private void setupMenuDropDown()
		{
			menuDropdown.AnchorView = (UIBarButtonItemAnchorView)RightBarButtonItem;
			menuDropdown.DismissMode = DismissMode.Automatic;
			menuDropdown.DataSource = new[]{
				"Menu 1",
				"Menu 2",
				"Menu 3",
				"Menu 4"
			};
		}

		private void setupCustomCellDropdown()
		{
			customCellDropdown.AnchorView = (UIAnchorView)customCellDropdownButton;
			customCellDropdown.DismissMode = DismissMode.Automatic;
			// By default, the dropdown will have its origin on the top left corner of its anchor view
			// So it will come over the anchor view and hide it completely
			// If you want to have the dropdown underneath your anchor view, you can do this:
			customCellDropdown.BottomOffset = new CGPoint(x: 0, y: customCellDropdownButton.Bounds.Height);

			customCellDropdown.DataSource = new object[]{
				new Currency("currency_dollar.png", "US Dollar", "USD"),
				new Currency("currency_euro.png", "Euro", "EUR"),
				new Currency("currency_pound.png", "British Pound", "GBP"),
				new Currency("currency_yuan.png", "Japanese Yen", "JPY")
			};

			customCellDropdown.CellNib = UINib.FromName("CustomDropdownCell", NSBundle.MainBundle);

			customCellDropdown.CustomCellConfiguration = (index, item, cell) => {
				var currency = item as Currency;
				var customCell = cell as CustomDropdownCell;

				var image = UIImage.FromFile(currency.icon);

				customCell.IconImage.Image = image;
				customCell.TitleLabel.Text = currency.name;
				customCell.SubtitleLabel.Text = currency.code;
			};
		}		

		public class Currency{
			public readonly string icon;
			public readonly string name;
			public readonly string code;

			public Currency(string icon, string name, string code)
			{
				this.code = code;
				this.name = name;
				this.icon = icon;
			}
		}
	}
}
