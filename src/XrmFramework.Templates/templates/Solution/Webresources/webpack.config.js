const path = require("path");
const fs = require("fs");
const CopyWebpackPlugin = require("copy-webpack-plugin");

const SRC_DIR = "scripts";
const srcDirPath = path.resolve(__dirname, SRC_DIR);

function getEntries(dirPath, entry = {}, baseDir = srcDirPath) {
  const fileList = fs.readdirSync(dirPath);

  fileList.forEach((fileName) => {
    const filePath = path.resolve(dirPath, fileName);

    if (fs.statSync(filePath).isDirectory()) {
      getEntries(filePath, entry, baseDir);
      return;
    }

    if (!fileName.endsWith(".ts")) {
      return;
    }

    const relativeFilePath = path.relative(baseDir, filePath);
    const entryName = relativeFilePath.replace(/\.ts$/, "").split(path.sep).join("/");
    entry[entryName] = filePath;
  });

  return entry;
}

module.exports = () => ({
  entry: {
    ...getEntries(srcDirPath),
  },
  output: {
    path: path.resolve(__dirname, "dist/scripts"),
    filename: "[name].js",
    clean: false,
  },
  module: {
    rules: [
      {
        test: /\.(ts)$/i,
        loader: "ts-loader",
        exclude: ["/node_modules/"],
      },
    ],
  },
  resolve: {
    extensions: [".ts", ".js", "..."],
  },
  plugins: [
    new CopyWebpackPlugin({
      patterns: [
        {
          from: path.resolve(__dirname, "assets"),
          to: path.resolve(__dirname, "dist/assets"),
        },
        {
          from: path.resolve(__dirname, "svg"),
          to: path.resolve(__dirname, "dist/svg"),
        },
        {
          from: path.resolve(__dirname, "js"),
          to: path.resolve(__dirname, "dist/js"),
        },
        {
          from: path.resolve(__dirname, "html"),
          to: path.resolve(__dirname, "dist/html"),
        },
      ],
    }),
  ],
});
