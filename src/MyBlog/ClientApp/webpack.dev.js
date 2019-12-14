const path = require("path");
const merge = require("webpack-merge");
const common = require("./webpack.common.js");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const WebpackPwaManifest = require("webpack-pwa-manifest");
const workboxPlugin = require("workbox-webpack-plugin");

module.exports = merge(common, {
  mode: "development",
  devtool: "inline-source-map",
  plugins: [
    new MiniCssExtractPlugin({
      filename: "[name].bundle.css"
    }),
    new WebpackPwaManifest({
      inject: true,
      fingerprints: false,
      name: "Code And Coffees",
      short_name: "Code And Coffees",
      description: "Code and Coffees is a blog about programming topics.",
      background_color: "#ffffff",
      display: "standalone",
      theme_color: "#FF7B39",
      ios: true,
      start_url: "/",
      icons: [
        {
          src: path.resolve("assets/images/icon.png"),
          sizes: [96, 128, 144, 192, 256, 384, 512, 1024],
          destination: path.join("pwa-icons")
        }
      ]
    }),
    new workboxPlugin.InjectManifest({
      swDest: "../sw.js",
      swSrc: "./sw.js",
      include: [/\.(css|js|html|ico|json)$/, /images\/.*\.(png|svg|jpg)$/]
    })
  ]
});
