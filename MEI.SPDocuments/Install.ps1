param($installPath, $toolsPath, $package, $project)

$configItem = $project.ProjectItems.Item("spDocumentsSettings.json")

# set 'Copy To Output Directory' to 'Copy if newer'
$copyToOutput = $configItem.Properties.Item("CopyToOutputDirectory")
$copyToOutput.Value = 2

# set 'Build Action' to 'Content'
$buildAction = $configItem.Properties.Item("BuildAction")
$buildAction.Value = 2

