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
        /// <summary>
        /// Gets or sets a value that provides the horizontal coordinate (offset) of the mouse pointer in global (screen) coordinates.
        /// </summary>
        public long ScreenX { get; set; }

        /// <summary>
        /// Gets or sets a value that provides the vertical coordinate (offset) of the mouse pointer in global (screen) coordinates.
        /// </summary>
        public long ScreenY { get; set; }

        /// <summary>
        /// Gets or sets a value that provides the horizontal coordinate within the application's client area at which the event occurred (as opposed to the coordinates within the page). 
        /// For example, clicking in the top-left corner of the client area will always result in a mouse event with a clientX value of 0, regardless of whether the page is scrolled horizontally.
        /// </summary>
        public long ClientX { get; set; }

        /// <summary>
        /// Gets or sets the vertical coordinate within the application's client area at which the event occurred (as opposed to the coordinates within the page). 
        /// For example, clicking in the top-left corner of the client area will always result in a mouse event with a clientY value of 0, regardless of whether the page is scrolled vertically. 
        /// </summary>
        public long ClientY { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if the control key was pressed (true) or not (false) when the event occured.
        /// </summary>
        public bool CtrlKey { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if the shift key was pressed (true) or not (false) when the event occurred.
        /// </summary>
        public bool ShiftKey { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if the alt key was pressed (true) or not (false) when the event occurred.
        /// </summary>
        public bool AltKey { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if the Meta key was pressed (true) or not (false) when the event occured.
        /// </summary>
        public bool MetaKey { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates which button was pressed on the mouse to trigger the event.
        /// </summary>
        public short Button { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates which buttons are pressed on the mouse (or other input device) when the event is triggered.
        /// </summary>
        public short Buttons { get; set; }

        // ToDo: Not including support for 'relatedTarget' since we don't have a good way to represent it.
        //public UIEventTarget RelatedTarget { get; set; }

        /// <summary>
        /// The MouseEvent.region read-only property returns the id of the canvas hit region affected by the event. 
        /// If no hit region is affected, null is returned.
        /// </summary>
        public string Region { get; set; }
    }

    /// <summary>
    /// Supplies information about a mouse event that is being raised.
    /// </summary>
    public class UIPointerEventArgs : UIMouseEventArgs
    {
        /// <summary>
        /// Gets or sets a value that is assigned to a pointer event that is unique from the identifiers of all active pointer events at the time. 
        /// Authors cannot assume values convey any particular meaning other than an identifier for the pointer that is unique from all other active pointers.
        /// </summary>
        public long PointerId { get; set; }

        /// <summary>
        /// Gets or sets a value that represents the width of the pointer's contact geometry along the x-axis, measured in CSS pixels. 
        /// Depending on the source of the pointer device (such as a finger), for a given pointer, each event may produce a different value.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets a value that represents the height of the pointer's contact geometry, along the Y axis (in CSS pixels). 
        /// Depending on the source of the pointer device (for example a finger), for a given pointer, each event may produce a different value.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Gets or sets a value that represents the normalized pressure of the pointer input in the range of 0 to 1, where 0 and 1 represent the minimum and maximum pressure the hardware is capable of detecting, respectively. 
        /// For hardware that does not support pressure, including but not limited to mouse, the value MUST be 0.5 when the pointer is active and 0 otherwise.
        /// </summary>
        public float Pressure { get; set; }

        /// <summary>
        /// Gets or sets a value that represents the normalized tangential pressure of the pointer input (also known as barrel pressure or cylinder stress) in the range -1 to 1, where 0 is the neutral position of the control.
        /// </summary>
        public float TangentialPressure { get; set; }

        /// <summary>
        /// Gets or sets a value that represents the angle (in degrees) between the Y-Z plane of the pointer and the screen. 
        /// This property is typically only useful for a pen/stylus pointer type. 
        /// The range of values is -90 to 90 degrees and a positive value means a tilt to the right. 
        /// For devices that do not support this property, the value is 0.
        /// </summary>
        public long TiltX { get; set; }

        /// <summary>
        /// Gets or sets a value that represents the angle (in degrees) between the X-Z plane of the pointer and the screen. 
        /// This property is typically only useful for a pen/stylus pointer type. 
        /// The range of values is -90 to 90 degrees and a positive value is a tilt toward the user. 
        /// For devices that do not support this property, the value is 0.
        /// </summary>
        public long TiltY { get; set; }

        /// <summary>
        /// Gets or sets a value that represents the clockwise rotation of the transducer (e.g. pen stylus) around its major axis in degrees, with a value in the range 0 to 359.
        /// </summary>
        public long Twist { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates the device type that caused the pointer event. 
        /// The supported values are the following strings:
        /// - mouse: The event was generated by a mouse device.
        /// - pen: The event was generated by a pen or stylus device.
        /// - touch: The event was generated by a touch such as a finger.
        /// </summary>
        public string PointerType { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether or not the pointer device that created the event is the primary pointer.
        /// Returns true if the pointer that caused the event to be fired is the primary device and returns false otherwise.
        /// </summary>
        public bool IsPrimary { get; set; }
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
