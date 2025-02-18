import { readFileSync, writeFileSync } from "fs";
import { dirname, join } from "path";
import { fileURLToPath } from "url";

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

const baseFilePath = join(__dirname, "../src/services/openapi/base.ts");

try {
  let content = readFileSync(baseFilePath, "utf8");

  const newBasePath = `import { env } from "@/lib/env";\nexport const BASE_PATH =  env.VITE_API_PATH`;

  // Replace the existing BASE_PATH line with the new one
  content = content.replace(/export const BASE_PATH = .+;/, newBasePath);

  writeFileSync(baseFilePath, content);
  console.log(
    "Successfully updated BASE_PATH to use Vite environment variable",
  );
} catch (error) {
  console.error("Error updating BASE_PATH:", error);
}
