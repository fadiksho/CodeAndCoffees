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
    devtool: "inline-source-map",
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
        start_url: "/",
        icons: [
          {
            src: path.resolve("Assets/images/icon.png"),
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
          test: /\.(html|png|jpg|gif|svg|woff|woff2|eot|ttf)$/,
          loader: "file-loader",
          options: {
            context: "Assets",
            name: "[path][name].[ext]"
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
