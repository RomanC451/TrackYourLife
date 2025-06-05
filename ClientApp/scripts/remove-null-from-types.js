import { readFileSync, writeFileSync } from "fs";
import { dirname, join } from "path";
import { fileURLToPath } from "url";

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

const apiFilePath = join(__dirname, "../src/services/openapi/api.ts");

try {
  let content = readFileSync(apiFilePath, "utf8");

  // Replace null with undefined in optional types
  // This regex matches patterns like:
  // 'property'?: type | null;
  // 'property'?: type | null | undefined;
  // property?: type | null;
  content = content.replace(/(['"]?\w+['"]?)\?: ([^;]+) \| null(?: \| undefined)?;/g, "$1?: $2 | undefined;");

  writeFileSync(apiFilePath, content);
  console.log("Successfully replaced null with undefined in optional types");
} catch (error) {
  console.error("Error replacing null with undefined:", error);
} 