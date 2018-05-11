﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;

namespace Microsoft.AspNetCore.Blazor
{
    /// <summary>
    /// Supplies information about an event that is being raised.
    /// </summary>
    public class UIEventArgs
    {
        /// <summary>
        /// Gets or sets the type of the event.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the event bubbles up through the DOM or not.
        /// </summary>
        public bool Bubbles { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if the event can be canceled, and therefore prevented as if the event never happened. 
        /// If the event is not cancelable, then its cancelable property will be false and the event listener cannot stop the event from occurring.
        /// </summary>
        public bool Cancelable { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether or not the event will propagate across the shadow DOM boundary into the standard DOM.
        /// </summary>
        public bool Composed { get; set; }
    }

    /// <summary>
    /// Supplies information about an input change event that is being raised.
    /// </summary>
    public class UIChangeEventArgs : UIEventArgs
    {
        /// <summary>
        /// Gets or sets the new value of the input. This may be a <see cref="string"/>
        /// or a <see cref="bool"/>.
        /// </summary>
        public object Value { get; set; }
    }

    /// <summary>
    /// Supplies information about an clipboard event that is being raised.
    /// </summary>
    public class UIClipboardEventArgs : UIEventArgs
    {
    }

    /// <summary>
    /// Supplies information about an drag event that is being raised.
    /// </summary>
    public class UIDragEventArgs : UIEventArgs
    {
    }

    /// <summary>
    /// Supplies information about an error event that is being raised.
    /// </summary>
    public class UIErrorEventArgs : UIEventArgs
    {
    }

    /// <summary>
    /// Supplies information about a focus event that is being raised.
    /// </summary>
    public class UIFocusEventArgs : UIEventArgs
    {
        // Not including support for 'relatedTarget' since we don't have a good way to represent it.
        // see: https://developer.mozilla.org/en-US/docs/Web/API/FocusEvent
    }

    /// <summary>
    /// Supplies information about a keyboard event that is being raised.
    /// </summary>
    public class UIKeyboardEventArgs : UIEventArgs
    {
        /// <summary>
        /// Gets or sets the value of a key pressed by the user.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets a value that represents a physical key on the keyboard (as opposed to the character generated by pressing the key). 
        /// In other words, this property returns a value which isn't altered by keyboard layout or the state of the modifier keys.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets a value that representing the location of the key on the keyboard or other input device.
        /// </summary>
        public long Location { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if the control key was pressed (true) or not (false) when the event occured.
        /// </summary>
        public bool CtrlKey { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if the shift key was pressed (true) or (false) when the event occurred.
        /// </summary>
        public bool ShiftKey { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if the alt key (Option or ⌥ on OS X) was pressed (true) or not (false) when the event occured.
        /// </summary>
        public bool AltKey { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if the Meta key was pressed (true) or not (false) when the event occurred.
        /// Some operating systems may intercept the key so it is never detected.
        /// </summary>
        public bool MetaKey { get; set; }

        /// <summary>
        /// Gets or sets a value that is true if the key is being held down such that it is automatically repeating.
        /// </summary>
        public bool Repeat { get; set; }

        /// <summary>
        /// Gets or sets a value that indicating if the event is fired after compositionstart and before compositionend.
        /// </summary>
        public bool IsComposing { get; set; }
    }

    /// <summary>
    /// Supplies information about a mouse event that is being raised.
    /// </summary>
    public class UIMouseEventArgs : UIEventArgs
    {
    }

    /// <summary>
    /// Supplies information about a mouse event that is being raised.
    /// </summary>
    public class UIPointerEventArgs : UIMouseEventArgs
    {
    }

    /// <summary>
    /// Supplies information about a progress event that is being raised.
    /// </summary>
    public class UIProgressEventArgs : UIMouseEventArgs
    {
    }

    /// <summary>
    /// Supplies information about a touch event that is being raised.
    /// </summary>
    public class UITouchEventArgs : UIEventArgs
    {
    }

    /// <summary>
    /// Supplies information about a mouse wheel event that is being raised.
    /// </summary>
    public class UIWheelEventArgs : UIEventArgs
    {
    }
}
