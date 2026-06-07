/** @type {import('monocart-coverage-reports').CoverageReportOptions} */
export default {
  name: "TrackYourLife ClientApp Coverage",

  reports: ["console-details", "console-summary", "v8"],
  lcov: true,

  outputDir: "coverage",

  onEnd: (results) => {
    console.log(`\nMonocart coverage report: ${results.reportPath}`);
  },
};
