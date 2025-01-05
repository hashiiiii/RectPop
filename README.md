<p align="center">
  <img width=600 src="Documentation/Images/logo.png" alt="RectPop">
</p>

# RectPop

[![license](https://img.shields.io/badge/LICENSE-MIT-green.svg)](LICENSE.md)

**Documentation ( [English](README.md), [日本語](README_JA.md) )**

Provides modules for easily implementing floating UIs such as popovers, tooltips, context menus, etc.

<p align="center">
  <img width="50%" src="Documentation/Images/multi_resolution.gif" alt="ConceptMovie">
</p>

## Table of Contents

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
<!-- param::title::Details:: -->
<details>
<summary>Details</summary>

- [Overview](#overview)
- [Features](#features)
  - [Easily unify floating UIs](#easily-unify-floating-uis)
  - [Place the UI within the rendering area](#place-the-ui-within-the-rendering-area)
  - [Floating Options](#floating-options)
    - [Modes](#modes)
    - [Offset](#offset)
  - [Multi-resolution support](#multi-resolution-support)
- [Setup](#setup)
  - [Installation](#installation)
- [Minimal usage](#minimal-usage)
- [Recommended usage](#recommended-usage)
- [License](#license)

</details>
<!-- END doctoc generated TOC please keep comment here to allow auto update -->

## Overview

When you pass an object that has a `RectTransform` and the `Canvas` on which that object is placed as a request, RectPop returns the settings necessary to display a floating UI.

## Features

### Easily unify floating UIs

As mentioned earlier, RectPop’s calculation logic does not require anything other than

> an object with a `RectTransform` and the `Canvas` on which the object is placed.

Furthermore, there are no constraints between the base object and the floating UI. Therefore, you can create only one floating UI and send requests from various objects.

We also provide a mechanism to achieve that. For an implementation example, please refer to [Example02Request.cs](Assets/RectPop/Examples/Sources/Example02Request.cs) and [Example02Result.cs](Assets/RectPop/Examples/Sources/Example02Result.cs).

### Place the UI within the rendering area

The response includes settings such as `Pivot` and `Anchor`. By applying these to the floating UI, you can display the UI on the screen in most cases.

> [!WARNING]
> If the floating UI is extremely large or if you add an excessive offset, it may be displayed partially or fully outside the screen.

We also provide the necessary methods to apply these settings. Refer to the `Apply` method in [PopController.cs](Assets/RectPop/Sources/Runtime/PopController.cs).

### Support for All Render Modes

The `Canvas.RenderMode` includes `ScreenSpaceOverlay`, `ScreenSpaceCamera`, and `WorldSpace`, and RectPop supports all of them.

### Floating Options

#### Modes

There are three modes:

> [!NOTE]
> You can change the default floating position.
> Inherit from `PopProviderBase` and override both `PopProviderBase.GetPopAnchorWorldPoint` and `PopProviderBase.GetPopPivotPosition`.

1. Inside

   Floats from the inside of the object.

<p align="center">
  <img width="50%" src="Documentation/Images/inside.png" alt="Inside">
</p>

2. OutsideVertical

   Floats above or below the object.

<p align="center">
  <img width="50%" src="Documentation/Images/outside_vertical.png" alt="OutsideVertical">
</p>

3. OutsideHorizontal

   Floats to the left or right of the object.

<p align="center">
  <img width="50%" src="Documentation/Images/outside_horizontal.png" alt="OutsideHorizontal">
</p>

#### Offset

You can add offsets to the top, bottom, left, or right.

<p align="center">
  <img width="80%" src="Documentation/Images/offset.gif" alt="ConceptMovie">
</p>

### Multi-resolution support

As you can see in the [GIF at the top](#rectpop), RectPop returns calculation results that consider the screen resolution. This allows you to handle any resolution. In cases where the resolution changes dynamically, you can display the floating UI in the correct position by recalculating.

## Setup

### Installation

You can install RectPop using Unity’s Package Manager.

1. Open Unity and select `Window` > `Package Manager`.
2. Click the `+` button in the top-left corner and select `Add package from git URL...`.
3. Enter the following URL:  
   `https://github.com/hashiiiii/RectPop.git?path=/Assets/RectPop/Sources`
4. Click `Add` to install the package.

For more details, please refer to [Installing from a Git URL](https://docs.unity3d.com/ja/2019.4/Manual/upm-ui-giturl.html) in the Unity manual.

## Minimal usage

> [!NOTE]
> An example scene is available at `Assets/RectPop/Examples/Example01.unity`. Refer to it as needed.

1. Create a Canvas and an object with a RectTransform.

   In the Unity Editor, prepare a `Canvas` and `RectTransform` which will be the base for your floating UI.

2. Obtain an instance of `PopController`.

   `PopController` is a handler for the calculation logic (`IPopProvider`).

    ```csharp
    public class Example01 : MonoBehaviour
    {
        private readonly PopController _controller = new();
    }
    ```

   The `PopController` instance requires an `IPopProvider`. By default, the parameterless constructor uses `DefaultPopProvider`. In most cases, this will meet your requirements.

    ```csharp
    public class PopController
    {
        // static
        private static readonly IPopProvider Default = new DefaultPopProvider();
      
        // dependency
        private readonly IPopProvider _provider;
      
        // constructor
        public PopController(IPopProvider provider)
        {
            _provider = provider;
        }
      
        public PopController() : this(Default)
        {
        }
    
        // ----- code omitted -----
    }
    ```

> [!NOTE]
> If you do not need to handle multiple `IPopProvider` instances at once, you can also treat the `PopController` instance as a singleton.

3. Run `PopController.RequestAndApply`.

   In this example, we display the floating UI when a button is clicked.

    ```csharp
    public class Example01 : MonoBehaviour
    {
        // base
        [SerializeField] private Canvas _baseCanvas;
        [SerializeField] private Button _button;
    
        // floating ui
        [SerializeField] private RectTransform _popRect;
        [SerializeField] private Canvas _popCanvas;
    
        private readonly PopController _controller = new();
    
        private void Awake()
        {
            _button.onClick.AddListener(() =>
            {
                // get base rect transform
                var baseRectTransform = _button.GetComponent<RectTransform>();
    
                // create request
                var request = new PopRequest(baseRectTransform, _baseCanvas);
    
                // send request and apply result to floating ui
                _controller.RequestAndApply(request, _popRect, _popCanvas);
    
                // show floating ui
                _popRect.gameObject.SetActive(true);
            });
        }
    }
    ```

## Recommended usage

In the [minimal usage](#minimal-usage) example, both the base UI and the floating UI references are implemented in the same file. However, in practice, you might want to **unify the floating UI to be used in multiple contexts**. Below is an example of how you can separate them into different files.

> [!NOTE]
> An example scene is available at `Assets/RectPop/Examples/Example02.unity`. Refer to it as needed.

1. Refer to steps 1 and 2 of the [minimal usage](#minimal-usage) section.

   It is essentially the same as before.

2. Run `PopController.Request`.

   This part is almost the same as step 3 in [minimal usage](#minimal-usage). Since the floating UI display is delegated to a different class, the implementation here is simpler.

    ```csharp
    public class Example02Request : MonoBehaviour
    {
        // base
        [SerializeField] private Canvas _baseCanvas;
        [SerializeField] private Button _button;
    
        private readonly PopController _controller = new();
    
        private void Awake()
        {
            _button.onClick.AddListener(() =>
            {
                // get base rect transform
                var baseRectTransform = _button.GetComponent<RectTransform>();
    
                // create request
                var request = new PopRequest(baseRectTransform, _baseCanvas);
    
                // send request
                _controller.Request(request);
            });
        }
    }
    ```

3. Run `PopController.Apply`.

   Here we create a class to display the floating UI. We subscribe to the `PopDispatcher.OnDispatched` event, receive the result, and display it.

    ```csharp
    public class Example02Result : MonoBehaviour
    {
        // floating ui
        [SerializeField] private RectTransform _floatingRect;
        [SerializeField] private Canvas _floatingCanvas;
    
        private readonly PopController _controller = new();
    
        // register event
        private void Awake()
        {
            PopDispatcher.OnDispatched += OnPopDispatched;
        }
    
        // unregister event
        private void OnDestroy()
        {
            PopDispatcher.OnDispatched -= OnPopDispatched;
        }
    
        // apply result to floating ui
        private void OnPopDispatched(PopDispatchedEvent ev)
        {
            _controller.Apply(ev.Result, _floatingRect, _floatingCanvas);
            _floatingRect.gameObject.SetActive(true);
        }
    }
    ```

> [!NOTE]
>
> `PopDispatcher` is implemented using an `event`. You can aim for a cleaner implementation by using other OSS methods like [R3.Observable.FromEvent](https://github.com/Cysharp/R3?tab=readme-ov-file#fromevent) (not tested yet, so no guarantee).
>
> If you actually replace it, you would need to substitute `PopDispatcher` with the new class, which uses `R3.Observable.FromEvent` or similar.
>
> In that case, create a class that inherits `PopController` and override `PopController.Dispatch`.
>
> Then, in your overridden `Dispatch` method, use the newly replaced `PopDispatcher`.

## License

This software is released under the MIT License.  
You are free to use it within the scope of the license. However, please note that the following copyright and license statements are required:

* [LICENSE.md](LICENSE.md)

Also, this document’s table of contents is created using the following software:

* [toc-generator](https://github.com/technote-space/toc-generator)

For more details about the toc-generator license, please refer to [Third Party Notices.md](Thirs%20Party%20Notices.md).