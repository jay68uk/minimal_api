## Build {{buildDetails.buildNumber}}
* **Branch**: {{buildDetails.sourceBranch}}
* **Tags**: {{buildDetails.tags}}
* **Completed**: {{buildDetails.finishTime}}
* **Previous Build**: {{compareBuildDetails.buildNumber}}

## List of WI returned by WIQL ({{queryWorkItems.length}})
{{#forEach queryWorkItems}}
*  **{{this.id}}** {{lookup this.fields 'System.Title'}}
{{/forEach}}
------
