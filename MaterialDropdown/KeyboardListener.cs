using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace MaterialDropdown
{
	public class KeyboardListener
	{

		public static KeyboardListener SharedInstance = new KeyboardListener();

		public bool IsVisible = false;
		public CGRect KeyboardFrame = CGRect.Empty;
		private bool isListening = false;
	
		~KeyboardListener() {
			StopListeningToKeyboard();
		}

	public void StartListeningToKeyboard() {
		if (isListening) {
				return;
		}

		isListening = true;

		NSNotificationCenter.DefaultCenter.AddObserver(
			UIKeyboard.WillShowNotification,
			KeyboardWillShow);
			
		NSNotificationCenter.DefaultCenter.AddObserver(
			UIKeyboard.WillHideNotification,
			KeyboardWillHide);
	}

	public void StopListeningToKeyboard() {
			NSNotificationCenter.DefaultCenter.RemoveObserver((NSObject)(this as object));
	}

	public void KeyboardWillShow(NSNotification notification) {
			IsVisible = true;
			KeyboardFrame = KeyboardFrameFromNotification(notification);
	}

	private void KeyboardWillHide(NSNotification notification) {
			IsVisible = false;
			KeyboardFrame = KeyboardFrameFromNotification(notification);
	}

	private CGRect KeyboardFrameFromNotification( NSNotification notification )
	{
			return ((notification as NSNotification).UserInfo[UIKeyboard.FrameEndUserInfoKey] as NSValue)?.CGRectValue ?? CGRect.Empty;
	}

}

}
