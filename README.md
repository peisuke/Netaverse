# Netaverse

[僕の考えたメタバースの世界を実装してみる](https://qiita.com/peisuke/items/e876e7be3ba4b5fa5aa3)の記事のコードです。

# 使い方

## 以下の関連プロジェクトをデプロイ

- [この辺](https://qiita.com/romorimori/items/155bb1f5e4cced629ce8)を参考に、TruffleとGanacheの環境を容易。
- 下記プロジェクトをデプロイ
  - https://github.com/peisuke/ERC20-sample-token
  - https://github.com/peisuke/atomic-swap-backend
- デプロイした時の`contract address`を記録しておく

## 本プロジェクトをクローン

- 先程のAtomicSwapとUserInformationのcontract addressを[meta.js](https://github.com/peisuke/Netaverse/blob/master/Assets/WebGLTemplates/Tmpl/TemplateData/meta.js)の先頭に記載。
- 後はUnityでWebGLでビルドして、ホスティングすれば
