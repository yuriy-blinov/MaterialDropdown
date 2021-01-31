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

        private NSObject _willShowNotification;
        private NSObject _willHideNotification;

        ~KeyboardListener()
        {
            StopListeningToKeyboard();
        }

        public void StartListeningToKeyboard()
        {
            if (isListening)
            {
                return;
            }

            isListening = true;

            _willShowNotification = NSNotificationCenter.DefaultCenter.AddObserver(
                UIKeyboard.WillShowNotification,
                KeyboardWillShow);

            _willHideNotification = NSNotificationCenter.DefaultCenter.AddObserver(
                UIKeyboard.WillHideNotification,
                KeyboardWillHide);
        }

        public void StopListeningToKeyboard()
        {
            if (_willShowNotification != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_willShowNotification);
                _willShowNotification = null;
            }

            if (_willHideNotification != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_willHideNotification);
                _willHideNotification = null;
            }

            isListening = false;
        }

        public void KeyboardWillShow(NSNotification notification)
        {
            IsVisible = true;
            KeyboardFrame = KeyboardFrameFromNotification(notification);
        }

        private void KeyboardWillHide(NSNotification notification)
        {
            IsVisible = false;
            KeyboardFrame = KeyboardFrameFromNotification(notification);
        }

        private CGRect KeyboardFrameFromNotification(NSNotification notification)
        {
            return ((notification as NSNotification).UserInfo[UIKeyboard.FrameEndUserInfoKey] as NSValue)?.CGRectValue ?? CGRect.Empty;
        }
    }
}
