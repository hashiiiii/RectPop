# RectPop

A Unity plugin for positioning layouts around a `RectTransform` using anchors and pivots. Perfect for tooltips, popovers, and contextual menus.

**Docs ([English](README.md), [日本語](README_JA.md))**

## Features

- Position UI elements relative to a target `RectTransform` using anchors and pivots.
- Ideal for creating tooltips, popovers, contextual menus, and other overlay UI elements.
- Easy integration into existing Unity projects.

## Installation

RectPop can be installed via Unity's Package Manager using a Git URL.

1. Open Unity and navigate to `Window` > `Package Manager`.
2. Click the `+` button and select `Add package from git URL...`.
3. Enter the following URL: `https://github.com/hashiiiii/RectPop.git#v1.0.0`

Ensure that the version tag (`#v1.0.0`) matches the version you intend to install.

4. Click `Add` to install the package.

For more information, refer to the [Unity Manual on Git URL installation](https://docs.unity3d.com/Manual/upm-ui-giturl.html).

## Usage

1. Add the `RectPop` component to the UI element you want to position.
2. Set the target `RectTransform` that the UI element should be positioned relative to.
3. Configure the anchor and pivot settings to achieve the desired positioning.
4. Optionally, adjust offset values to fine-tune the position.

## Example

Here's a simple example of how to use RectPop to position a tooltip relative to a button:

1. Create a UI Button in your scene.
2. Create a UI Panel to serve as the tooltip.
3. Add the `RectPop` component to the tooltip panel.
4. In the `RectPop` component, set the target to the Button's `RectTransform`.
5. Configure the anchors and pivots to position the tooltip appropriately.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE.md) file for details.