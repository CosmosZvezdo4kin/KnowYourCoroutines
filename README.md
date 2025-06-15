# KnowYourCoroutines
This is a simple plugin for LabAPI that adds some useful commands for debugging MEC coroutines
> **⚠️ Warning:** Familiarize yourself with the principles of MEC coroutines before using the commands.
# Installation
Put plugin dll from [releases](https://github.com/CosmosZvezdo4kin/KnowYourCoroutines/releases/latest) in `LabAPI/plugins/...`
# Commands
|Command              |Description                       |Required Permission               |
|---------------------|----------------------------------|----------------------------------|
|getcoroutines all    |Get list of all coroutines        |`kyc.getcoroutines.all`           |
|getcoroutines paused |Get list of all paused coroutines |`kyc.getcoroutines.paused`        |
|getcoroutines running|Get list of all running coroutines|`kyc.getcoroutines.running`       |
|killcoroutine        |Kills coroutine by Id             |`kyc.killcoroutine`               |
|pausecoroutine       |Pauses coroutine by Id            |`kyc.pausecoroutine`              |
|resumecoroutine      |Resumes coroutine by Id           |`kyc.resumecoroutine`             |
