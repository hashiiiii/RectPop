<p align="center">
  <img width=600 src="Documentation/Images/logo.png" alt="RectPop">
</p>

# RectPop

[![license](https://img.shields.io/badge/LICENSE-MIT-green.svg)](LICENSE.md)
[![unity](https://img.shields.io/badge/Unity-2019.3+-black.svg)](#requirements)

**Documentation ([English](README.md), [日本語](README_JA.md))**

RectPop provides modules to easily implement floating UIs such as popovers, tooltips, and context menus.

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
    - [UI Placement Within the Display Area](#ui-placement-within-the-display-area)
    - [Minimal Required Options](#minimal-required-options)
    - [Support for Multiple Resolutions](#support-for-multiple-resolutions)
    - [Loosely Coupled Implementation](#loosely-coupled-implementation)
- [Setup](#setup)
    - [Requirements](#requirements)
    - [Installation](#installation)
- [Basic Usage](#basic-usage)
- [Recommended Usage](#recommended-usage)
- [License](#license)

</details>
<!-- END doctoc generated TOC please keep comment here to allow auto update -->

## Overview
When you pass an arbitrary `RectTransform` as a request, RectPop calculates and returns optimal positioning and settings for displaying a floating UI.

## Features

### UI Placement Within the Display Area
The response contains settings such as `Pivot` and `Anchor`. By applying these settings to your floating UI, you can display the UI on the screen in most cases.

> [!WARNING]
> If the floating UI itself is extremely large, part of it may still end up off-screen.

### Minimal Required Options
RectPop supports three placement modes:

- **Inside**
- **OutsideVertical**
- **OutsideHorizontal**

<p align="center">
  <img width="80%" src="Documentation/Images/multi_placement.gif" alt="ConceptMovie">
</p>

You can also add offsets.

<p align="center">
  <img width="80%" src="Documentation/Images/offset.gif" alt="ConceptMovie">
</p>

### Support for Multiple Resolutions
As shown in the GIF, the calculation results take the device’s resolution into account. Even in scenarios where the resolution dynamically changes, re-running the calculation will allow you to display your UI in the correct position.

### Loosely Coupled Implementation
RectPop’s logic is not dependent on any specific UI setup. Consequently, it’s easy to reuse a single floating UI in multiple places.

## Setup

### Requirements
- Unity **2019.3** or later

### Installation

RectPop can be installed using the Unity Package Manager.

1. Open Unity, and go to **Window** > **Package Manager**.
2. Click the **+** button in the top-left corner, then select **Add package from git URL...**.
3. Enter the following URL: `https://github.com/hashiiiii/RectPop.git?path=/Assets/RectPop/Sources`
4. Click **Add** to install the package.

For more details, refer to the Unity Manual on [Installing from a Git URL](https://docs.unity3d.com/2019.4/Documentation/Manual/upm-ui-giturl.html).

## Basic Usage

> [!NOTE]
> You can find an example in `Assets/RectPop/Examples/Example01.unity`. Refer to it as needed.

1. Create a **Canvas** and an object with a **RectTransform**.

In the Unity Editor, prepare a `Canvas` and an object with a `RectTransform` that will serve as the base for your floating UI.

2. Obtain an instance of `PopController`.

`PopController` is a wrapper for the calculation logic (`IPopProvider`).

```csharp
public class Example01 : MonoBehaviour
{
   private readonly PopController _controller = new();
}
```

The `PopController` instance requires an `IPopProvider`. The default constructor uses `DefaultPopProvider`, which should be sufficient for most use cases.

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

> [!TIP]
> If handling multiple `IPopProvider` instances simultaneously is unnecessary, it might be beneficial to treat the `PopController` instance as a singleton.

3. Execute `PopController.RequestAndApply`.

In this example, the floating UI is displayed in response to a button click event.

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

## Recommended Usage

In the [Basic Usage](#basic-usage) section, the base UI and floating UI were implemented with references within the same file. However, in real-world applications, there is often a need to **standardize floating UI components**. This section introduces a method for implementing this by separating them into different files.

> [!NOTE]
> An example is available at `Assets/RectPop/Examples/Example02.unity` for reference.

1. Review steps 1 and 2 from [Basic Usage](#basic-usage).

These initial steps remain unchanged.

2. Execute `PopController.Request`.

This implementation is nearly identical to step 3 in [Basic Usage](#basic-usage). However, the display of the floating UI is delegated to a separate class, resulting in a simpler implementation.

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

3. Execute `PopController.Apply`.

This step involves creating a class responsible for displaying the floating UI. It subscribes to the `PopDispatcher.OnDispatched` event to receive and display the results.

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

> [!TIP]
> 1. While `PopDispatcher` uses an event-based implementation, you might achieve a more elegant solution using methods from other OSS libraries like [R3.Observable.FromEvent](https://github.com/Cysharp/R3?tab=readme-ov-file#fromevent) (though this hasn't been tested yet).
> 2. To implement this approach, you'll need to replace `PopDispatcher` with an alternative implementation such as `R3.Observable.FromEvent`.
> 3. Create a class that inherits from `PopController` and override `PopController.Dispatch`.
> 4. Use the replaced `PopDispatcher` class within the overridden Dispatch method.

## License

This software is released under the MIT License.  
While you are free to use it within the license terms, you must include the following copyright and license notices:

* [LICENSE.md](LICENSE.md)

Additionally, the table of contents in this document was generated using:

* [toc-generator](https://github.com/technote-space/toc-generator)

For details about the toc-generator license, please refer to [Third Party Notices.md](Third%20Party%20Notices.md).