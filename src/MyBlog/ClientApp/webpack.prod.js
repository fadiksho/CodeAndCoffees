const path = require("path");
const merge = require("webpack-merge");
const common = require("./webpack.common.js");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const WebpackPwaManifest = require("webpack-pwa-manifest");
const CompressionPlugin = require("compression-webpack-plugin");
const workboxPlugin = require("workbox-webpack-plugin");
const WebappWebpackPlugin = require("webapp-webpack-plugin");

module.exports = merge(common, {
  mode: "production",
  devtool: "source-map",
  plugins: [
    new MiniCssExtractPlugin({
      filename: "[name].bundle.css"
    }),
    new CompressionPlugin({
      filename: "[path].gz[query]",
      algorithm: "gzip",
      test: /\.js$|\.css$|\.html$|\.eot?.+$|\.ttf?.+$|\.woff?.+$|\.svg?.+$/,
      threshold: 8192,
      minRatio: 0.8
    }),
    new WebappWebpackPlugin({
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
      include: [
        /\.(css|js|html)$/,
        /images\/.*\.(png|svg|jpg)$/,
        /favicon\.ico/,
        /manifest\.json/
      ],
      exclude: [/pwa-icons\/favicon\.ico/, /pwa-icons\/manifest\.json/]
    })
  ]
});
