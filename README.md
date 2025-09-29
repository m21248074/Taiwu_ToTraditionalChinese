# Taiwu_ToTraditionalChinese 太吾繪卷 簡繁轉換模組

## Introduction 介紹

先前遊戲於EA階段，NGA論壇上有中國熱心網友提供[簡繁轉換模組](https://bbs.nga.cn/read.php?tid=15239374)
，但後來遊戲進入正式版後，程式架構改變，也沒有更新，故建立此專案，望造福繁體中文玩家。

### Features 特點

* 提供5種簡轉繁方式
* 以太吾繪卷來說，應該比 XUnity.AutoTranslator 好一些

## Installation 安裝

待補


目前不知道與其他模組是否有衝突，模組有任何問題，歡迎提出[Issue](https://github.com/m21248074/Taiwu_ToTraditionalChinese/issues)!

## 原理

根據下列兩種主要簡轉繁方式取得繁體文字後 使用 Harmony 對 Unity 的 TMP_Text.text.Setter 注入 Patch

### 查表

預設使用該方式做簡轉繁

跟EA階段時NGA上的模組實現方式相同 使用 kernel32.dll 的 LCMapString 函數

### OpenCC

使用 [OpenCC](https://github.com/BYVoid/OpenCC) 並根據其提供預設配置文件 模組提供4種簡轉繁

|種類|對應配置文件|
|----|----|
|OpenCC標準|s2t.json|
|臺灣(不使用常用詞彙)|s2tw.json|
|臺灣(使用常用詞彙)|s2twp.json|
|香港|s2hk.json|

ToTranditionalChinese.dll 內嵌 OpenCC 的設定(json)、字典(ocd2)與DLL檔案 會在加載本模組時 在遊戲根目錄建立 opencc 資料夾並將資源文件丟進去 所以移除模組後請記得刪除該資料夾

## License 許可協議

MIT License

## Third Party Library 第三方庫

* [OpenCC](https://github.com/BYVoid/OpenCC) Apache License 2.0