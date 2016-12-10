using System;
using System.Diagnostics;
using System.Linq;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using UIKit;
using static System.Math;

namespace MaterialDropdown
{
	public delegate void CellConfigurationAction(int index, object item, DropdownCell cell);

	public class Dropdown : UIView
	{
		private readonly static object[] EmtpyDatasource = new object[0];

		[DebuggerDisplay("x={x}, y={y}, width={width}, offscreenHeight={offscreenHeight}")]
		private struct ComputeLayoutTuple
		{
			public nfloat x, y, width, offscreenHeight;

			public ComputeLayoutTuple(nfloat x, nfloat y, nfloat width, nfloat offscreenHeight) : this()
			{
				this.x = x;
				this.y = y;
				this.width = width;
				this.offscreenHeight = offscreenHeight;
			}
		}

		/// <summary>
		/// Occurs when dropdown item selected by user.
		/// </summary>
		public event ItemSelectedEventHandler SelectedItemChanged;
		public event EventHandler Canceled;
		public event EventHandler WillBeShown;

		private static WeakReference<Dropdown> visibleDropDown;

		private static WeakReference<Dropdown> VisibleDropDown
		{
			get
			{
				return visibleDropDown;
			}

			set
			{
				visibleDropDown = value;
			}
		}

		internal UIView dismissableView = new UIView();
		internal UIView tableViewContainer = new UIView();
		internal UITableView tableView = new UITableView();
		internal DropdownCell templateCell;
		private AnchorView anchorView;

		public AnchorView AnchorView
		{
			get
			{
				return anchorView;
			}
			set
			{
				anchorView = value;
				NeedsUpdateConstraints();
			}
		}

		///<summary>
		///	The possible directions where the drop down will be showed.
		///	See `Direction` enum for more info.
		///</summary>
		private Direction direction = Direction.Any;

		public Direction Direction
		{
			get
			{
				return direction;
			}

			set
			{
				direction = value;
			}
		}

		private CGPoint topOffset;

		///<summary>
		///	The offset point relative to `anchorView` when the drop down is shown above the anchor view.
		///	By default, the drop down is showed onto the `anchorView` with the top
		///	left corner for its origin, so an offset equal to (0, 0).
		///	You can change here the default drop down origin.
		///</summary>
		public CGPoint TopOffset
		{
			get
			{
				return topOffset;
			}

			set
			{
				topOffset = value;
				NeedsUpdateConstraints();
			}
		}

		private CGPoint bottomOffset;

		///<summary>
		///	The offset point relative to `anchorView` when the drop down is shown below the anchor view.
		///	By default, the drop down is showed onto the `anchorView` with the top
		///	left corner for its origin, so an offset equal to (0, 0).
		///	You can change here the default drop down origin.
		///</summary>
		public CGPoint BottomOffset
		{
			get
			{
				return bottomOffset;
			}

			set
			{
				bottomOffset = value;
				NeedsUpdateConstraints();
			}
		}

		private nfloat? width;

		///<summary>
		///	The width of the drop down.
		///	Defaults to `anchorView.bounds.width - offset.x`.
		///</summary>
		public nfloat? Width
		{
			get
			{
				return width;
			}

			set
			{
				width = value;
				NeedsUpdateConstraints();
			}
		}

		private NSLayoutConstraint heightConstraint;
		private NSLayoutConstraint widthConstraint;
		private NSLayoutConstraint xConstraint;
		private NSLayoutConstraint yConstraint;

		//MARK: Appearance
		private nfloat cellHeight = DropdownConstant.UI.RowHeight;
		public nfloat CellHeight
		{
			get
			{
				return cellHeight;
			}

			set
			{
				tableView.RowHeight = value;
				cellHeight = value;
				ReloadAllComponents();
			}
		}

		private UIColor tableViewBackgoundColor = DropdownConstant.UI.BackgroundColor;

		public UIColor TableViewBackgoundColor
		{
			get
			{
				return tableViewBackgoundColor;
			}

			set
			{
				tableView.BackgroundColor = value;
				tableViewBackgoundColor = value;
			}
		}

		public override UIColor BackgroundColor
		{
			get { return tableViewBackgoundColor; }
			set { TableViewBackgoundColor = value; }
		}

		///<summary>
		///	The background color of the selected cell in the drop down.
		///	Changing the background color automatically reloads the drop down.
		///</summary>
		public UIColor SelectionBackgroundColor = DropdownConstant.UI.SelectionBackgroundColor;

		///<summary>
		///The separator color between cells.
		///Changing the separator color automatically reloads the drop down.
		///</summary>
		private UIColor separatorColor = DropdownConstant.UI.SeparatorColor;

		public UIColor SeparatorColor
		{
			get
			{
				return separatorColor;
			}

			set
			{
				tableView.SeparatorColor = value;
				separatorColor = value;
				ReloadAllComponents();
			}
		}

		private nfloat cornerRadius = DropdownConstant.UI.CornerRadius;

		///<summary>
		///The corner radius of DropDown.
		///Changing the corner radius automatically reloads the drop down.
		///</summary>
		public nfloat CornerRadius
		{
			get
			{
				return cornerRadius;
			}

			set
			{
				tableViewContainer.Layer.CornerRadius = value;
				tableView.Layer.CornerRadius = value;
				cornerRadius = value;
				ReloadAllComponents();
			}
		}


		private UIColor shadowColor = DropdownConstant.UI.Shadow.Color;

		///<summary>
		///The color of the shadow.
		///Changing the shadow color automatically reloads the drop down.
		///</summary>
		public UIColor ShadowColor
		{
			get
			{
				return shadowColor;
			}

			set
			{
				tableViewContainer.Layer.ShadowColor = value.CGColor;
				shadowColor = value;
				ReloadAllComponents();
			}
		}


		private CGSize shadowOffset = DropdownConstant.UI.Shadow.Offset;

		///<summary>
		///	The offset of the shadow.
		///	Changing the shadow color automatically reloads the drop down.
		///</summary>
		public CGSize ShadowOffset
		{
			get
			{
				return shadowOffset;
			}

			set
			{
				tableViewContainer.Layer.ShadowOffset = value;
				shadowOffset = value;
				ReloadAllComponents();
			}
		}

		private float shadowOpacity = DropdownConstant.UI.Shadow.Opacity;

		///<summary>
		///The opacity of the shadow.
		///Changing the shadow opacity automatically reloads the drop down.
		///</summary>
		public float ShadowOpacity
		{
			get
			{
				return shadowOpacity;
			}

			set
			{
				tableViewContainer.Layer.ShadowOpacity = value;
				shadowOpacity = value;
				ReloadAllComponents();
			}
		}


		private nfloat shadowRadius = DropdownConstant.UI.Shadow.Radius;

		///<summary>
		///The radius of the shadow.
		///Changing the shadow radius automatically reloads the drop down.
		///</summary>
		public nfloat ShadowRadius
		{
			get
			{
				return shadowRadius;
			}

			set
			{
				tableViewContainer.Layer.ShadowRadius = value;
				shadowRadius = value;
				ReloadAllComponents();
			}
		}

		///<summary>
		///	The duration of the show/hide animation.
		///</summary>
		public float Animationduration = DropdownConstant.Animation.Duration;

		///<summary>
		///	The option of the show animation. Global change.
		///</summary>
		public static UIViewAnimationOptions GlobalAnimationEntranceOptions = DropdownConstant.Animation.EntranceOptions;

		///<summary>
		///The option of the hide animation. Global change.
		///</summary>
		public static UIViewAnimationOptions GlobalAnimationExitOptions = DropdownConstant.Animation.ExitOptions;

		///<summary>
		///The option of the show animation. Only change the caller. To change all drop down's use the static var.
		///</summary>
		public UIViewAnimationOptions AnimationEntranceOptions = GlobalAnimationEntranceOptions;

		///<summary>
		///The option of the hide animation. Only change the caller. To change all drop down's use the static var.
		///</summary>
		public UIViewAnimationOptions AnimationExitOptions = GlobalAnimationExitOptions;

		///<summary>
		///The downScale transformation of the tableview when the DropDown is appearing
		///</summary>
		CGAffineTransform downScaleTransform = DropdownConstant.Animation.DownScaleTransform;
		public CGAffineTransform DownScaleTransform
		{
			get { return downScaleTransform; }
			set
			{
				tableViewContainer.Transform = value;
				downScaleTransform = value;
			}
		}

		///<summary>
		///The color of the text for each cells of the drop down.
		///Changing the text color automatically reloads the drop down.
		///</summary>
		private UIColor textColor = DropdownConstant.UI.TextColor;
		public UIColor TextColor
		{
			get { return textColor; }
			set
			{
				textColor = value;
				ReloadAllComponents();
			}
		}

		///<summary>
		///The font of the text for each cells of the drop down.
		///Changing the text font automatically reloads the drop down.
		///</summary>
		private UIFont textFont = DropdownConstant.UI.TextFont;
		public UIFont TextFont
		{
			get { return textFont; }
			set
			{
				textFont = value;
				ReloadAllComponents();
			}
		}

		private UINib cellNib = UINib.FromName("DropdownCell", NSBundle.MainBundle);

		///<summary>
		/// The NIB to use for DropDownCells
		///Changing the cell nib automatically reloads the drop down.
		///</summary>
		public UINib CellNib
		{
			get
			{
				return cellNib;
			}

			set
			{
				cellNib = value;
				tableView.RegisterNibForCellReuse(cellNib, DropdownConstant.ReusableIdentifier.DropdownCell);
				templateCell = null;
				ReloadAllComponents();
			}
		}
		private object[] dataSource = EmtpyDatasource;

		//MARK: Content

		///<summary>
		///The data source for the drop down.
		///Changing the data source automatically reloads the drop down.
		///</summary>
		public object[] DataSource
		{
			get
			{
				return dataSource;
			}

			set
			{
				if (value == null)
				{
					value = EmtpyDatasource;
				}

				dataSource = value;
				DeselectRowAt(SelectedRowIndex);
				ReloadAllComponents();
			}
		}

		// The index of the row after its seleciton.
		private int? SelectedRowIndex;

		private CellConfigurationAction customCellConfiguration;

		///<summary>
		/// A advanced formatter for the cells. Allows customization when custom cells are used
		/// Changing `customCellConfiguration` automatically reloads the drop down.
		///</summary>
		public CellConfigurationAction CustomCellConfiguration
		{
			get
			{
				return customCellConfiguration;
			}

			set
			{
				customCellConfiguration = value;
				ReloadAllComponents();
			}
		}

		private DismissMode dismissMode = DismissMode.OnTap;

		///<summary>The dismiss mode of the drop down. Default is `OnTap`.</summary> 
		public DismissMode DismissMode
		{
			get
			{
				return dismissMode;
			}

			set
			{
				if (value == DismissMode.OnTap)
				{
					var gestureRecognizer = new UITapGestureRecognizer(() => DismissableViewTapped());
					dismissableView.AddGestureRecognizer(gestureRecognizer);
				}
				else {
					var gestureRecognizers = dismissableView.GestureRecognizers?.SingleOrDefault();
					if (gestureRecognizers != null)
					{
						dismissableView.RemoveGestureRecognizer(gestureRecognizers);
					}
				}
				dismissMode = value;
			}
		}

		private nfloat minHeight
		{
			get { return tableView.RowHeight; }
		}

		private bool didSetupConstraints = false;

		~Dropdown()
		{
			StopListeningToNotifications();
		}

		///<summary>
		///	Creates a new instance of a drop down.
		///	Don't forget to setup the `dataSource`,
		///	the `anchorView` and the `selectionAction`
		///	at least before calling `show()`.
		///</summary>
		public Dropdown() : base()
		{
			Setup();
		}

		///<summary>
		///	Creates a new instance of a drop down.
		///	- parameter anchorView:        The view to which the drop down will displayed onto.
		///	- parameter dataSource:        The data source for the drop down.
		///	- parameter topOffset:         The offset point relative to `anchorView` used when drop down is displayed on above the anchor view.
		///	- parameter bottomOffset:      The offset point relative to `anchorView` used when drop down is displayed on below the anchor view.
		///	- returns: A new instance of a drop down customized with the above parameters.
		///</summary>
		public Dropdown(AnchorView anchorView,
						object[] dataSource,
						CGPoint topOffset,
						CGPoint bottomOffset) : base()
		{

			this.anchorView = anchorView;
			this.dataSource = dataSource;
			this.topOffset = topOffset;
			this.bottomOffset = bottomOffset;
		}

		public Dropdown(CGRect frame) : base(frame)
		{
			Setup();
		}

		public Dropdown(NSCoder aDecoder) : base(aDecoder)
		{
			Setup();
		}

		private void Setup()
		{
			tableView.RegisterNibForCellReuse(cellNib, DropdownConstant.ReusableIdentifier.DropdownCell);
			DispatchQueue.MainQueue.DispatchAsync(() =>
			{
				UpdateConstraintsIfNeeded();
				SetupUI();
			});

			dismissMode = DismissMode.OnTap;

			tableView.Source = new DropdownDatasource(this);
			ListenToKeyboard();
			AccessibilityIdentifier = "drop_down";
		}

		private void SetupUI()
		{
			base.BackgroundColor = UIColor.Clear;

			tableViewContainer.Layer.MasksToBounds = false;
			tableViewContainer.Layer.CornerRadius = cornerRadius;
			tableViewContainer.Layer.ShadowColor = shadowColor.CGColor;
			tableViewContainer.Layer.ShadowOffset = shadowOffset;
			tableViewContainer.Layer.ShadowOpacity = shadowOpacity;
			tableViewContainer.Layer.ShadowRadius = shadowRadius;

			tableView.RowHeight = cellHeight;
			tableView.BackgroundColor = tableViewBackgoundColor;
			tableView.SeparatorColor = separatorColor;
			tableView.Layer.CornerRadius = cornerRadius;
			tableView.Layer.MasksToBounds = true;

			SetHiddentState();
			Hidden = true;
		}

		public override void UpdateConstraints()
		{
			if (!didSetupConstraints)
			{
				SetupConstraints();
			}

			didSetupConstraints = true;

			var layout = ComputeLayout();

			if (!layout.canBeDisplayed)
			{
				base.UpdateConstraints();
				Hide();
				return;
			}

			xConstraint.Constant = layout.x;
			yConstraint.Constant = layout.y;
			widthConstraint.Constant = layout.width;
			heightConstraint.Constant = layout.visibleHeight;

			tableView.ScrollEnabled = layout.offscreenHeight > 0;

			DispatchQueue.MainQueue.DispatchAsync(
				() => tableView.FlashScrollIndicators()
			);

			base.UpdateConstraints();
		}

		private void SetupConstraints()
		{
			TranslatesAutoresizingMaskIntoConstraints = false;

			// Dismissable view
			AddSubview(dismissableView);
			dismissableView.TranslatesAutoresizingMaskIntoConstraints = false;
			AddConstraints(NSLayoutConstraint.FromVisualFormat("H:|[dismissableView]|", 0, "dismissableView", dismissableView));
			AddConstraints(NSLayoutConstraint.FromVisualFormat("V:|[dismissableView]|", 0, "dismissableView", dismissableView));
			// Table view container
			AddSubview(tableViewContainer);
			tableViewContainer.TranslatesAutoresizingMaskIntoConstraints = false;

			xConstraint = NSLayoutConstraint.Create(
				tableViewContainer,
				NSLayoutAttribute.Leading,
				NSLayoutRelation.Equal,
				this,
				NSLayoutAttribute.Leading,
				multiplier: 1,
				constant: 0);
			AddConstraint(xConstraint);

			yConstraint = NSLayoutConstraint.Create(
				tableViewContainer,
				NSLayoutAttribute.Top,
				NSLayoutRelation.Equal,
				this,
				NSLayoutAttribute.Top,
				multiplier: 1,
				constant: 0);

			AddConstraint(yConstraint);

			widthConstraint = NSLayoutConstraint.Create(
				tableViewContainer,
				NSLayoutAttribute.Width,
				NSLayoutRelation.Equal,
				null,
				NSLayoutAttribute.NoAttribute,
				multiplier: 1,
				constant: 0);
			tableViewContainer.AddConstraint(widthConstraint);

			heightConstraint = NSLayoutConstraint.Create(
				tableViewContainer,
				NSLayoutAttribute.Height,
				NSLayoutRelation.Equal,
				null,
				NSLayoutAttribute.NoAttribute,
				multiplier: 1,
				constant: 0);
			tableViewContainer.AddConstraint(heightConstraint);

			// Table view
			tableViewContainer.AddSubview(tableView);
			tableView.TranslatesAutoresizingMaskIntoConstraints = false;

			tableViewContainer.AddConstraints(NSLayoutConstraint.FromVisualFormat("H:|[tableView]|", 0, "tableView", tableView));
			tableViewContainer.AddConstraints(NSLayoutConstraint.FromVisualFormat("V:|[tableView]|", 0, "tableView", tableView));
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			// When orientation changes, layoutSubviews is called
			// We update the constraint to update the position
			SetNeedsUpdateConstraints();
			var shadowPath = UIBezierPath.FromRoundedRect(tableViewContainer.Bounds, DropdownConstant.UI.CornerRadius);
			tableViewContainer.Layer.ShadowPath = shadowPath.CGPath;
		}

		[DebuggerDisplay("x={x}, y={y}, width={width}, visibleHeight={visibleHeight}, offscreenHeight={offscreenHeight}")]
		private struct LayoutResult
		{
			public nfloat x;
			public nfloat y;
			public nfloat width;
			public nfloat offscreenHeight;
			public nfloat visibleHeight;
			public bool canBeDisplayed;
			public Direction direction;

			public LayoutResult(nfloat x, nfloat y, nfloat width, nfloat offscreenHeight, nfloat visibleHeight, bool canBeDisplayed, Direction direction)
			{
				this.x = x;
				this.y = y;
				this.width = width;
				this.offscreenHeight = offscreenHeight;
				this.visibleHeight = visibleHeight;
				this.canBeDisplayed = canBeDisplayed;
				this.direction = direction;
			}
		}

		private static UIWindow VisibleWindow()
		{
			var currentWindow = UIApplication.SharedApplication.KeyWindow;

			if (currentWindow == null)
			{
				var frontToBackWindows = UIApplication.SharedApplication.Windows.Reverse();

				foreach (var window in frontToBackWindows)
				{
					if (window.WindowLevel == UIWindowLevel.Normal)
					{
						currentWindow = window;
						break;
					}
				}
			}

			return currentWindow;
		}

		private LayoutResult ComputeLayout()
		{
			var layout = new ComputeLayoutTuple(0, 0, 0, 0);
			var dir = Direction;

			var window = VisibleWindow();

			if (window == null)
			{
				return new LayoutResult(0, 0, 0, 0, 0, false, dir);
			}

			if (anchorView != null)
			{
				var av = anchorView as UIBarButtonItemAnchorView;
				if (av != null)
				{
					var isRightBarButtonItem = anchorView.PlainView.Frame.GetMinX() > window.Frame.GetMidX();

					if (isRightBarButtonItem)
					{
						nfloat w = width ?? FittingWidth();
						var anchorViewWidth = anchorView.PlainView.Frame.Width;
						var x = -(w - anchorViewWidth);
						bottomOffset = new CGPoint(x, 0);
					}

				}
			}

			if (anchorView == null)
			{
				layout = ComputeLayoutBottomDisplay(window);
				Direction = Direction.Any;
			}
			else {
				switch (Direction)
				{
					case Direction.Any:
						layout = ComputeLayoutBottomDisplay(window);
						Direction = Direction.Bottom;

						if (layout.offscreenHeight > 0)
						{
							var topLayout = ComputeLayoutForTopDisplay(window);

							if (topLayout.offscreenHeight < layout.offscreenHeight)
							{
								layout = topLayout;
								Direction = Direction.Top;
							}
						}
						break;
					case Direction.Bottom:
						layout = ComputeLayoutBottomDisplay(window);
						Direction = Direction.Bottom;
						break;
					case Direction.Top:
						layout = ComputeLayoutForTopDisplay(window);
						Direction = Direction.Top;
						break;
				}
			}


			ConstraintWidthToFittingSizeIfNecessary(ref layout);
			ConstraintWidthToBoundsIfNecessary(ref layout, window);


			var visibleHeight = TableHeight - layout.offscreenHeight;
			var canBeDisplayed = visibleHeight >= minHeight;

			return new LayoutResult(layout.x, layout.y, layout.width, layout.offscreenHeight, visibleHeight, canBeDisplayed, Direction);

		}

		private ComputeLayoutTuple ComputeLayoutBottomDisplay(UIWindow window)
		{
			nfloat offscreenHeight = 0;
			var w = width ?? (anchorView?.PlainView.Bounds.Width ?? FittingWidth()) - bottomOffset.X;
			var anchorViewX = anchorView?.PlainView.WindowFrame()?.GetMinX() ?? window.Frame.GetMidX() - (w / 2);
			var anchorViewY = anchorView?.PlainView.WindowFrame()?.GetMinY() ?? window.Frame.GetMidY() - (TableHeight / 2);

			var x = anchorViewX + bottomOffset.X;
			var y = (anchorViewY > 0 ? anchorViewY : 0) + bottomOffset.Y;

			var maxY = y + TableHeight;
			var windowMaxY = window.Bounds.GetMaxY();

			var keyboardListener = KeyboardListener.SharedInstance;

			var keyboardMinY = keyboardListener.KeyboardFrame.GetMinY() - DropdownConstant.UI.HeightPadding;

			if (keyboardListener.IsVisible && maxY > keyboardMinY)
			{
				offscreenHeight = new nfloat(Abs(maxY - keyboardMinY));
			}
			else if (maxY > windowMaxY)
			{
				offscreenHeight = new nfloat(Abs(maxY - windowMaxY));
			}

			return new ComputeLayoutTuple(x, y, w, offscreenHeight);
		}

		private ComputeLayoutTuple ComputeLayoutForTopDisplay(UIWindow window)
		{
			nfloat offscreenHeight = 0;
			var anchorViewX = anchorView?.PlainView.WindowFrame()?.GetMinX() ?? 0;
			var anchorViewMaxY = anchorView?.PlainView.WindowFrame()?.GetMaxY() ?? 0;

			var x = anchorViewX + topOffset.X;
			var y = (anchorViewMaxY + topOffset.Y) - TableHeight;

			var windowY = window.Bounds.GetMinY() + DropdownConstant.UI.HeightPadding;

			if (y < windowY)
			{
				offscreenHeight = new nfloat(Abs(y - windowY));
				y = windowY;
			}

			var w = width ?? (anchorView?.PlainView.Bounds.Width ?? FittingWidth()) - topOffset.X;

			return new ComputeLayoutTuple(x, y, w, offscreenHeight);
		}

		private nfloat FittingWidth()
		{
			if (templateCell == null)
			{
				templateCell = cellNib.Instantiate(null, null)[0] as DropdownCell;
			}

			nfloat maxWidth = 0;

			for (int index = 0; index < dataSource.Count(); index++)
			{
				var ds = tableView.Source as DropdownDatasource;
				ds.ConfigureCellAt(templateCell, index);
				templateCell.Bounds = new CGRect(templateCell.Bounds.Location, new CGSize(templateCell.Bounds.Width, CellHeight));

				var w = templateCell.SystemLayoutSizeFittingSize(UILayoutFittingCompressedSize).Width;
				if (w > maxWidth)
				{
					maxWidth = w;
				}
			}

			return maxWidth;
		}

		private void ConstraintWidthToBoundsIfNecessary(ref ComputeLayoutTuple layout, UIWindow window)
		{
			var windowMaxX = window.Bounds.GetMaxX();
			var maxX = layout.x + layout.width;


			if (maxX > windowMaxX)
			{
				var delta = maxX - windowMaxX;
				var newOrigin = layout.x - delta;


				if (newOrigin > 0)
				{
					layout.x = newOrigin;
				}
				else
				{
					layout.x = 0;
					layout.width += newOrigin; // newOrigin is negative, so this operation is a substraction
				}
			}
		}

		private void ConstraintWidthToFittingSizeIfNecessary(ref ComputeLayoutTuple layout)
		{
			if (width != null) return;

			if (layout.width < FittingWidth())
			{
				layout.width = FittingWidth();
			}
		}

		private Dropdown GetVisibleDropdown()
		{
			Dropdown dd = null;
			VisibleDropDown?.TryGetTarget(out dd);
			return dd;
		}

		///<summary>
		///Shows the drop down if enough height.
		///- returns: Wether it succeed and how much height is needed to display all cells at once.
		///</summary>
		public Tuple<bool, nfloat> Show()
		{
			var visibleDropDown = GetVisibleDropdown();

			if (visibleDropDown != null)
			{
				visibleDropDown.Cancel();
			}

			WillBeShown?.Invoke(this, EventArgs.Empty);

			VisibleDropDown = new WeakReference<Dropdown>(this);
			SetNeedsUpdateConstraints();

			var visibleWindow = VisibleWindow();
			visibleWindow?.AddSubview(this);
			visibleWindow?.BringSubviewToFront(this);

			TranslatesAutoresizingMaskIntoConstraints = false;
			if (visibleWindow != null)
			{
				visibleWindow.AddConstraints(NSLayoutConstraint.FromVisualFormat("H:|[dropDown]|", 0, "dropDown", this));
				visibleWindow.AddConstraints(NSLayoutConstraint.FromVisualFormat("V:|[dropDown]|", 0, "dropDown", this));
			}

			var layout = ComputeLayout();

			if (!layout.canBeDisplayed)
			{
				Hide();
				return new Tuple<bool, nfloat>(layout.canBeDisplayed, layout.offscreenHeight);
			}

			Hidden = false;
			tableViewContainer.Transform = downScaleTransform;
			Animate(
					Animationduration,
					0,
					AnimationEntranceOptions,
					() => SetShowedState(),
					null);

			SelectRowAt(SelectedRowIndex);

			return new Tuple<bool, nfloat>(layout.canBeDisplayed, layout.offscreenHeight);
		}

		// Hides the drop down.
		public void Hide()
		{
			var visibleDropDown = GetVisibleDropdown();
			if (this == visibleDropDown)
			{
				/*
				If one drop down is showed and another one is not
				but we call `hide()` on the hidden one:
				we don't want it to set the `VisibleDropDown` to nil.
				*/
				VisibleDropDown = null;
			}

			if (Hidden)
			{
				return;
			}

			Animate(
				Animationduration,
				0,
				AnimationExitOptions,
				() => SetHiddentState(),
				() =>
				{
					Hidden = true;
					RemoveFromSuperview();
				});
		}

		internal void Cancel()
		{
			Hide();
			Canceled?.Invoke(this, EventArgs.Empty);
		}

		private void SetHiddentState()
		{
			Alpha = 0;
		}

		private void SetShowedState()
		{
			Alpha = 1;
			tableViewContainer.Transform = CGAffineTransform.MakeIdentity();
		}

		///<summary>
		///Reloads all the cells.
		///It should not be necessary in most cases because each change to
		///	`dataSource`, `textColor`, `textFont`, `selectionBackgroundColor`
		///	and `cellConfiguration` implicitly calls `reloadAllComponents()`.
		///</summary>
		public void ReloadAllComponents()
		{
			tableView.ReloadData();
			SetNeedsUpdateConstraints();
		}

		// (Pre)selects a row at a certain index.
		public void SelectRowAt(int? index)
		{
			if (index != null)
			{
				tableView.SelectRow(
					indexPath: NSIndexPath.FromRowSection(index.Value, section: 0),
					animated: false,
					scrollPosition: UITableViewScrollPosition.Middle);
			}
			else {
				DeselectRowAt(SelectedRowIndex);
			}

			SelectedRowIndex = index;
		}

		public void DeselectRowAt(int? index)
		{
			SelectedRowIndex = null;
			if (index == null || index.Value >= 0) return;

			tableView.DeselectRow(NSIndexPath.FromRowSection(index.Value, section: 0), animated: true);
		}

		// Returns the index of the selected row.
		public int IndexForSelectedRow
		{
			get
			{
				var row = tableView.IndexPathForSelectedRow.Row;
				return row;
			}
		}

		// Returns the selected item.
		public object SelectedItem
		{
			get
			{
				var row = (tableView.IndexPathForSelectedRow).Row;

				return dataSource[row];
			}

			set{
				var index = Array.IndexOf(DataSource, value);
				if(index >= 0){
					SelectRowAt(index);
				}
			}
		}

		// Returns the height needed to display all cells.
		private nfloat TableHeight
		{
			get
			{
				return tableView.RowHeight * dataSource.Count();
			}
		}

		public override UIView HitTest(CGPoint point, UIEvent uievent)
		{
			var view = base.HitTest(point, uievent);

			if (dismissMode == DismissMode.Automatic && view == dismissableView)
			{
				Cancel();
				return null;
			}
			else {
				return view;
			}
		}

		private void DismissableViewTapped()
		{
			Cancel();
		}

		///<summary>
		///Starts listening to keyboard events.
		///Allows the drop down to display correctly when keyboard is showed.
		///</summary>
		public static void StartListeningToKeyboard()
		{
			KeyboardListener.SharedInstance.StartListeningToKeyboard();
		}

		private void ListenToKeyboard()
		{
			KeyboardListener.SharedInstance.StartListeningToKeyboard();

			NSNotificationCenter.DefaultCenter.AddObserver(
				UIKeyboard.WillShowNotification,
				KeyboardUpdate);

			NSNotificationCenter.DefaultCenter.AddObserver(
				UIKeyboard.WillHideNotification,
				KeyboardUpdate);
		}

		private void StopListeningToNotifications()
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver(this);
		}

		private void KeyboardUpdate(NSNotification notification)
		{
			SetNeedsUpdateConstraints();
		}

		public class DropdownDatasource : UITableViewSource
		{
			readonly Dropdown dropdown;

			public DropdownDatasource(Dropdown dropdown)
			{
				if (dropdown == null)
					throw new ArgumentNullException(nameof(dropdown));

				this.dropdown = dropdown;
			}

			public override nint RowsInSection(UITableView tableview, nint section)
			{
				return dropdown.dataSource.Count();
			}

			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell(DropdownConstant.ReusableIdentifier.DropdownCell, indexPath) as DropdownCell;
				var index = (indexPath as NSIndexPath).Row;

				ConfigureCellAt(cell, index);

				return cell;
			}

			internal void ConfigureCellAt(DropdownCell cell, int index)
			{
				Console.WriteLine("ConfigureCellAt " + index);
				if (index >= 0 && index < dropdown.dataSource.Count())
				{
					cell.AccessibilityIdentifier = dropdown.dataSource[index].ToString();
				}

				cell.TitleLabel.TextColor = dropdown.textColor;
				cell.TitleLabel.Font = dropdown.textFont;
				cell.SelectedColor = dropdown.SelectionBackgroundColor;

				cell.TitleLabel.Text = dropdown.dataSource[index].ToString();

				if (dropdown.customCellConfiguration != null)
					dropdown.customCellConfiguration(index, dropdown.dataSource[index], cell);
			}

			public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
			{
				cell.Selected = (indexPath as NSIndexPath).Row == dropdown.SelectedRowIndex;
			}

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				dropdown.SelectedRowIndex = (indexPath as NSIndexPath).Row;

				dropdown.SelectedItemChanged?.Invoke(dropdown,
											  new ItemSelectedEventArgs(
												  dropdown.SelectedRowIndex.Value,
												  dropdown.dataSource[dropdown.SelectedRowIndex.Value]));

				var avStrongRef = dropdown.AnchorView as UIBarButtonItem;
				if (avStrongRef != null)
				{
					// DropDown's from UIBarButtonItem are menus so we deselect the selected menu right after selection
					dropdown.DeselectRowAt(dropdown.SelectedRowIndex);
				}

				dropdown.Hide();
			}
		}
	}
}