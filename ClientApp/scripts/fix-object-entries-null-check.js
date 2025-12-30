import { readFileSync, writeFileSync } from "fs";
import { dirname, join } from "path";
import { fileURLToPath } from "url";

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

const apiFilePath = join(__dirname, "../src/services/openapi/api.ts");

try {
    let content = readFileSync(apiFilePath, "utf8");

    // Fix Object.entries() null check issue
    // The generated code checks `if (variable !== undefined)` but the variable
    // can be `string | null`, so we need to also exclude null.
    // This regex matches the pattern and changes !== undefined to != null
    // (loose equality checks both undefined and null)
    content = content.replace(
        /if \((\w+) !== undefined\) \{\s+for \(const \[key, value\] of Object\.entries\(\1\)\)/g,
        "if ($1 != null) {\n                for (const [key, value] of Object.entries($1))"
    );

    // Fix Object.entries() on string enum parameters (like category)
    // The generator incorrectly uses Object.entries() on string enums,
    // which causes "Divertissement" to become "0=D&1=i&2=v..."
    // This replaces the Object.entries loop with direct assignment
    content = content.replace(
        /if \((\w+) != null\) \{\s+for \(const \[key, value\] of Object\.entries\(\1\)\) \{\s+localVarQueryParameter\[key\] = value;\s+\}\s+\}/g,
        'if ($1 !== undefined) {\n        localVarQueryParameter["$1"] = $1;\n      }'
    );

    writeFileSync(apiFilePath, content);
    console.log("Successfully fixed Object.entries() issues");
} catch (error) {
    console.error("Error fixing Object.entries():", error);
}

