const https = require("https");
const fs = require("fs");
const path = require("path");

require("dotenv").config({ path: path.resolve(process.cwd(), ".env") });

const apiPath = process.env.VITE_API_PATH_SWAGGER.replace(/\/$/, ""); // remove trailing slash
const swaggerUrl = `${apiPath}/swagger/v1/swagger.json`;
const outputPath = "src/services/openapi/swagger.json";

const file = fs.createWriteStream(outputPath);

// Due to a self-signed certificate, we need to disable SSL verification for this request.
const options = {
  rejectUnauthorized: false,
};

https
  .get(swaggerUrl, options, (response) => {
    // Check if the response is successful
    if (response.statusCode !== 200) {
      console.error(
        `Failed to get swagger.json, status code: ${response.statusCode}`,
      );
      // It's important to exit with an error code so that the npm script stops.
      process.exit(1);
    }
    response.pipe(file);
    file.on("finish", () => {
      file.close();
      console.log("swagger.json downloaded successfully.");
    });
  })
  .on("error", (err) => {
    fs.unlink(outputPath); // Delete the file if an error occurs.
    console.error("Error downloading swagger.json:", err.message);
    process.exit(1);
  }); 