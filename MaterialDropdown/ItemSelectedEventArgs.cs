using System;

namespace MaterialDropdown
{
	public delegate void ItemSelectedEventHandler(Dropdown sender, ItemSelectedEventArgs e);
	public class ItemSelectedEventArgs : EventArgs
	{
		public ItemSelectedEventArgs(int index, object item)
		{
			Index = index;
			Item = item;
		}
		public int Index { get; private set; }
		public object Item { get; private set; }
	}
}