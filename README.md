FontEffectsLib
==============

C# / XNA Font Effects Library - great for game titles, menus, and more

Core Types
---------------------------
These are the core interfaces, classes and extensions in this library. They are located in the FontEffectsLib.CoreTypes namespace. 

Core Interfaces:
----------------
* **ISprite**: Defines a basic drawable item in an XNA game
* **IStateful**: Allows fonts to define events and pass custom state change information via the **StateEventArgs** class. Use to add sound effects at the right time, trigger other fonts effects, and more.

### Core Classes:
* **BaseGameObject**: Abstract base type for both fonts and sprites in the library. Provides necessary definitions and functionality to position, scale, rotate, and draw a Game Object.
* **StateEventArgs**: Custom event arguments class for the **IStateful** interface. Allows passing custom state information via events.

### Core Extensions:
The extensions methods in the static **Extensions** class are generic helper methods for common tasks. 

#### Viewport:
* **Viewport.HalfWidth**: This static method returns center of the screen on the X-axis
* **Viewport.HalfHeight**: This static method returns center of the screen on the Y-axis


Game Objects
------------
These are the creatable types provided by the library. They are located in the FontEffectsLib.FontTypes and FontEffectsLib.SpriteTypes namespaces

### Fonts:
* **GameFont**: Basic font, no special effects, equivalent to using XNA's SpriteFont
* **ShadowFont**: Easily add a shadow to your text, making it look 3D. Shadow can be turned on or off. Inherits from **GameFont**, and is the base for all other fonts in this library.
* **DropInFont**: Start your text anywhere (like off-screen), and let it drop to a certain Y-position (like center screen). Once the text drops, there is a really cool "bounce effect"! This class implements **IStateful**, raising events on *Drop*, *Compress*, *Expand*, and *Done* state transitions.
* **FadingFont**: Fade your text in or out! Neat effect for title screens. This class implements **IStateful**, raising events on *NotFading*, *Fading*, and *TargetValueReached* state transitions.

### Sprites:
* **GameSprite**: Basic sprite; inherits from **BaseGameObject**
* **ComplexSprite**: *Under development*...

License
--------
Copyright (c) 2013 <a href="http://www.buildcoolrobots.com">Great Minds Robotics</a>

This software is released under the terms of the <a href="http://opensource.org/licenses/MIT">MIT License</a>


