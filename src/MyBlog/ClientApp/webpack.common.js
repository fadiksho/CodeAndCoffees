/* eslint-disable @typescript-eslint/no-var-requires */
const path = require("path");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

module.exports = {
  entry: {
    main: "./index.ts"
  },
  module: {
    rules: [
      {
        test: /\.ts?$/,
        use: "ts-loader",
        exclude: /node_modules/
      },
      {
        test: /\.(sa|sc|c)ss$/,
        use: [
          MiniCssExtractPlugin.loader,
          "css-loader",
          "resolve-url-loader",
          "sass-loader"
        ]
      },
      {
        test: /\.(html|png|jpg|gif|svg|woff|woff2|eot|ttf)$/,
        loader: "file-loader",
        options: {
          context: "assets",
          name: "[path][name].[ext]"
        }
      },
      {
        test: /\.(ico)$/,
        loader: "file-loader",
        options: {
          name: "../[name].[ext]"
        }
      },
      {
        test: /\.(json)$/,
        loader: "file-loader",
        type: "javascript/auto",
        options: {
          name: "../[name].[ext]"
        }
      }
    ]
  },
  plugins: [],
  resolve: {
    extensions: [".ts", ".js"]
  },
  output: {
    filename: "[name].bundle.js",
    path: path.resolve(__dirname, "../wwwroot/dist"),
    publicPath: "/dist/"
  }
};
