import { readFileSync, writeFileSync } from "fs";
import { dirname, join } from "path";
import { fileURLToPath } from "url";

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

const apiFilePath = join(__dirname, "../src/services/openapi/api.ts");

try {
  let content = readFileSync(apiFilePath, "utf8");

  // Regular expression to find DTO interfaces
  const dtoRegex = /export interface (\w+Dto) {/g;

  // Function to check if loading flags already exist in the interface
  const hasLoadingFlags = (interfaceContent) => {
    return interfaceContent.includes("isLoading") && interfaceContent.includes("isDeleting");
  };

  // Replace each DTO interface with the modified version
  content = content.replace(dtoRegex, (match, interfaceName) => {
    if (hasLoadingFlags(content)) {
      return match; // Skip if loading flags already exist
    }

    // Add loading flags properties right after the opening brace
    return `export interface ${interfaceName} {\n    /**\n     * Whether the data is currently loading\n     * @type {boolean}\n     * @memberof ${interfaceName}\n     */\n    'isLoading': boolean;\n\n    /**\n     * Whether the data is currently being deleted\n     * @type {boolean}\n     * @memberof ${interfaceName}\n     */\n    'isDeleting': boolean;\n`;
  });

  writeFileSync(apiFilePath, content);
  console.log("Successfully added isLoading and isDeleting to DTO interfaces");
} catch (error) {
  console.error("Error modifying DTOs:", error);
} 