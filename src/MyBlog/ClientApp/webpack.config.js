const path = require("path");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const CompressionPlugin = require("compression-webpack-plugin");
const HtmlWebpackPlugin = require("html-webpack-plugin");
const WebpackPwaManifest = require("webpack-pwa-manifest");
const workboxPlugin = require("workbox-webpack-plugin");

module.exports = (env = {}, argv = {}) => {
  const isProd = argv.mode === "production";

  const config = {
    mode: argv.mode || "development", // we default to development when no 'mode' arg is passed

    optimization: {
      minimize: true
    },
    entry: {
      main: "./index.js"
    },
    output: {
      filename: isProd ? "bundle-[chunkHash].js" : "[name].js",
      path: path.resolve(__dirname, "../wwwroot/dist"),
      publicPath: "/dist/"
    },
    plugins: [
      new MiniCssExtractPlugin({
        filename: isProd ? "style-[contenthash].css" : "style.css"
      }),
      new CompressionPlugin({
        filename: "[path].gz[query]",
        algorithm: "gzip",
        test: /\.js$|\.css$|\.html$|\.eot?.+$|\.ttf?.+$|\.woff?.+$|\.svg?.+$/,
        threshold: 10240,
        minRatio: 0.8
      }),
      new HtmlWebpackPlugin({
        template: "./Pages/_LayoutTemplate.cshtml",
        filename: "../../Views/Shared/_Layout.cshtml",
        inject: false
      }),
      new WebpackPwaManifest({
        name: "Code And Coffees",
        short_name: "Code And Coffees",
        description: "Code and Coffees is a blog about programming topics.",
        background_color: "#ffffff",
        display: "standalone",
        theme_color: "#FF7B39",
        ios: true,
        icons: [
          {
            src: path.resolve("Assets/images/icon.png"),
            sizes: [96, 128, 192, 256, 384, 512],
            destination: "/icons/"
          }
        ]
      }),
      new workboxPlugin.InjectManifest({
        swDest: "../sw.js",
        swSrc: "./sw.js",
        exclude: [/\.cshtml$/]
      })
    ],
    module: {
      rules: [
        {
          test: /\.js$/,
          exclude: /node_modules/,
          use: {
            loader: "babel-loader"
          }
        },
        {
          test: /\.(sa|sc|c)ss$/,
          use: [
            "style-loader",
            MiniCssExtractPlugin.loader,
            "css-loader",
            "sass-loader"
          ]
        },
        {
          test: /\.(png|jpg|gif|svg)$/,
          loader: "file-loader",
          options: {
            name: "images/[name].[ext]"
          }
        },
        {
          test: /\.(woff|woff2|eot|ttf)$/,
          loader: "file-loader",
          options: {
            name: "fonts/[name].[ext]"
          }
        },
        {
          test: /\.(ico)$/,
          loader: "file-loader",
          options: {
            name: "../[name].[ext]"
          }
        }
      ]
    }
  };
  return config;
};
