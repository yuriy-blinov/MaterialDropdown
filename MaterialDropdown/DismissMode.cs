namespace MaterialDropdown
{
	///<summary>
	/// The dismiss mode for a drop down.
	/// </summary>
	public enum DismissMode
	{
		///<summary>
		///A tap outside the drop down is required to dismiss
		///</summary>
		OnTap,

		///<summary>
		///No tap is required to dismiss, it will dimiss when interacting with anything else.
		///</summary>
		Automatic,

		///<summary>
		///Not dismissable by the user.
		///</summary>
		Manual
	}
}