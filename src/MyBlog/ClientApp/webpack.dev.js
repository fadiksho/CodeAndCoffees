/* eslint-disable @typescript-eslint/no-var-requires */
const path = require("path");
const merge = require("webpack-merge");
const common = require("./webpack.common.js");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const workboxPlugin = require("workbox-webpack-plugin");
const FaviconsWebpackPlugin = require("favicons-webpack-plugin");

module.exports = merge(common, {
  mode: "development",
  devtool: "inline-source-map",
  plugins: [
    new MiniCssExtractPlugin({
      filename: "[name].bundle.css"
    }),
    new FaviconsWebpackPlugin({
      logo: path.resolve("assets/logo.svg"),
      prefix: "/pwa-icons",
      favicons: {
        icons: {
          coast: false,
          yandex: false,
          windows: false,
          firefox: false
        }
      }
    }),
    new workboxPlugin.InjectManifest({
      swDest: "../sw.js",
      swSrc: "./sw.js",
      include: [/\.(css|js|html)$/, /images\/.*\.(png|svg|jpg)$/]
    })
  ]
});
