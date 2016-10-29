namespace MaterialDropdown
{
	///<summary>
	///The direction where the drop down will show from the `anchorView`.
	///</summary>
	public enum Direction
	{
		///<summary>
		///The drop down will show below the anchor view when possible, otherwise above if there is more place than below.
		///</summary>
		Any,

		///<summary>
		///The drop down will show above the anchor view or will not be showed if not enough space.
		///</summary>
		Top,

		///<summary>
		///The drop down will show below or will not be showed if not enough space.
		///</summary>
		Bottom
	}
}