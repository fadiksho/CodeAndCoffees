const path = require("path");
const merge = require("webpack-merge");
const common = require("./webpack.common.js");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const WebpackPwaManifest = require("webpack-pwa-manifest");
const CompressionPlugin = require("compression-webpack-plugin");
const workboxPlugin = require("workbox-webpack-plugin");

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
    new WebpackPwaManifest({
      inject: false,
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
          sizes: [96, 128, 192, 256, 384, 512],
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
