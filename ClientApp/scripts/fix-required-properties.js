#!/usr/bin/env node

/**
 * Post-generation script to fix required properties in polymorphic types.
 * This script ensures that non-nullable properties in derived polymorphic types are marked as required.
 */

import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

// Configuration
const API_FILE_PATH = path.join(__dirname, '../src/services/openapi/api.ts');
const BACKUP_SUFFIX = '.backup';

// Property mappings for each polymorphic type
const REQUIRED_PROPERTIES = {
    'WeightBasedExerciseS': ['reps', 'weight'],
    'TimeBasedExerciseSet': ['durationSeconds'],
    'BodyweightExerciseSet': ['reps'],
    'DistanceExerciseSet': ['distance', 'distanceUnit'],
    'CustomExerciseSet': ['customValue', 'customUnit'],
    'WeightBasedExerciseSetChange': ['weightChange', 'repsChange'],
    'TimeBasedExerciseSetChange': ['durationChangeSeconds'],
    'BodyweightExerciseSetChange': ['repsChange'],
    'DistanceExerciseSetChange': ['distanceChange', 'distanceUnitChange'],
    'CustomExerciseSetChange': ['valueChange']
};

function createBackup(filePath) {
    const backupPath = filePath + BACKUP_SUFFIX;
    if (fs.existsSync(filePath)) {
        fs.copyFileSync(filePath, backupPath);
        console.log(`✅ Created backup: ${backupPath}`);
    }
}

function fixRequiredProperties(filePath) {
    if (!fs.existsSync(filePath)) {
        console.error(`❌ Error: File not found at ${filePath}`);
        return false;
    }

    let content = fs.readFileSync(filePath, 'utf8');
    let changesMade = false;

    console.log(`🔧 Reading file: ${filePath}`);

    // Process each polymorphic type
    Object.entries(REQUIRED_PROPERTIES).forEach(([typeName, requiredProps]) => {
        console.log(`🔄 Processing ${typeName}...`);

        requiredProps.forEach(prop => {
            // Replace optional property with required property in the entire file
            const optionalRegex = new RegExp(`'${prop}'\\?:\\s*([^;]+);`, 'g');
            const requiredRegex = new RegExp(`'${prop}':\\s*([^;]+);`, 'g');

            if (optionalRegex.test(content)) {
                content = content.replace(optionalRegex, `'${prop}': $1;`);
                console.log(`  ✅ Made '${prop}' required in ${typeName}`);
                changesMade = true;
            } else if (!requiredRegex.test(content)) {
                console.log(`  ⚠️  Property '${prop}' not found in ${typeName}`);
            }
        });
    });

    if (changesMade) {
        fs.writeFileSync(filePath, content, 'utf8');
        console.log(`✅ Successfully updated required properties in: ${filePath}`);
    } else {
        console.log(`ℹ️ No changes needed for required properties in: ${filePath}`);
    }

    return changesMade;
}

function main() {
    console.log('\n🚀 Starting required properties fix...');
    console.log(`📁 Target file: ${API_FILE_PATH}`);
    console.log(`📁 File exists: ${fs.existsSync(API_FILE_PATH)}`);

    try {
        createBackup(API_FILE_PATH);
        fixRequiredProperties(API_FILE_PATH);
        console.log('\n✨ Required properties fix completed successfully!');
    } catch (error) {
        console.error('\n❌ Error fixing required properties:', error.message);
        process.exit(1);
    }
}

// Run the script
main();

export { fixRequiredProperties, REQUIRED_PROPERTIES };
