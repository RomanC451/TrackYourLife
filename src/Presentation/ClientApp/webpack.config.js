const path = require("path");
const HtmlWebpackPlugin = require("html-webpack-plugin");

module.exports = {
  entry: "./src/index.tsx",
  output: {
    filename: "main.js",
    path: path.resolve(__dirname, "build"),
    publicPath: "/"
  },
  module: {
    rules: [
      {
        test: /\.(js|jsx)$/,
        exclude: /node_modules/,
        use: {
          loader: "babel-loader",
          options: {
            presets: ["@babel/preset-env", "@babel/preset-react"],
            plugins: [
              "@babel/plugin-proposal-class-properties",
              [
                "babel-plugin-root-import",
                {
                  rootPathPrefix: "~",
                  rootPathSuffix: "src"
                }
              ]
            ]
          }
        }
      },
      {
        test: /\.(ts|tsx)$/,
        loader: "ts-loader"
      },
      // {
      //   test: /\.(png|jp(e*)g|svg|gif)$/,
      //   type: "asset/resource"
      // },
      {
        test: /\.svg$/,
        use: ["@svgr/webpack"]
      },
      {
        test: /\.(png|jpg|gif)$/i,
        use: [
          {
            loader: "file-loader"
          }
        ]
      },
      {
        test: /\.css$/,
        include: path.resolve(__dirname, "src"),
        use: ["style-loader", "css-loader", "postcss-loader"]
      }
    ]
  },

  resolve: {
    extensions: ["*", ".js", ".jsx", ".ts", ".tsx"],
    alias: {
      "~": path.resolve(__dirname, "src"),
      root: path.resolve(__dirname, "src")
    }
  },
  plugins: [
    new HtmlWebpackPlugin({
      template: path.join(__dirname, "public", "index.html")
    })
  ],
  devServer: {
    static: {
      directory: path.join(__dirname, "build")
    },
    https: true,
    hot: true,
    port: 44497,
    historyApiFallback: true
  },
  devtool: "source-map"
};
