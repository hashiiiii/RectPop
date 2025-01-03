<p align="center">
  <img width=800 src="Documentation/Images/logo.png" alt="RectPop">
</p>

# RectPop

[![license](https://img.shields.io/badge/LICENSE-MIT-green.svg)](LICENSE.md)
[![unity](https://img.shields.io/badge/Unity-2019.3+-black.svg)](###要件)

**ドキュメント ([English](README.md), [日本語](README_JA.md))**

ポップオーバー、ツールチップ、コンテキストメニューなどのフローティング UI を簡単に実装するためのモジュールを提供します。

<p align="center">
  <img width="80%" src="Documentation/Images/multi_resolution.gif" alt="ConceptMovie">
</p>

## 目次
<!-- START doctoc -->
<!-- END doctoc -->

## 概要
任意の `RectTransform` をリクエストとして渡すと、フローティング UI を表示するのにちょうど良い位置や設定を計算して返します。

## 特徴
### 描画領域内への UI 配置
レスポンスには `Pivot` や `Anchor` の設定が含まれています。これらをフローティング UI に適用することで、ほとんどのケースで画面内に UI を表示することができます。

> [!WARNING]
> フローティング UI そのものがあまりにも巨大な場合は画面外にはみ出します。

### 必要最小限のオプション
- Inside
- OutsideVertical
- OutsideHorizontal

以上の 3 つの配置モードがあります。

<p align="center">
  <img width="80%" src="Documentation/Images/multi_placement.gif" alt="ConceptMovie">
</p>

オフセットを追加することができます。

<p align="center">
  <img width="80%" src="Documentation/Images/offset.gif" alt="ConceptMovie">
</p>

### 複数解像度対応
GIF でも示されているように、端末の解像度を加味した計算結果を返します。動的に解像度が変わるようなケースにおいても、再計算さえすれば正しい位置に UI を表示することができます。

### 疎結合な実装
RectPop のロジックは UI に依存しません。それゆえ、単一のフローティング UI を複数個所で使いまわすような実装も簡単に行うことができます。
## セットアップ

### 要件
* Unity 2019.3 or later

### インストール

RectPop は、Unity のパッケージマネージャーを使用してインストールできます。

1. Unity を開き、`ウィンドウ` > `パッケージマネージャー` を選択します。
2. 左上の `+` ボタンをクリックし、`Git URL からパッケージを追加...` を選択します。
3. 以下の URL を入力します。：`https://github.com/hashiiiii/RectPop.git?path=/Assets/RectPop/Sources`
4. `追加` をクリックしてパッケージをインストールします。

詳細については、[Unity マニュアルの Git URL からのインストール](https://docs.unity3d.com/ja/2019.4/Manual/upm-ui-giturl.html)を参照してください。

## 基本的な使い方
TBD

## ライセンス

本ソフトウェアはMITライセンスで公開しています。ライセンスの範囲内で自由に使っていただけますが、著作権表示とライセンス表示が必須となります。詳細は [LICENSE](LICENSE.md) ファイルをご覧ください。