![](https://i.postimg.cc/Rh9gn8b2/cf34dbb9-1e62-4ba5-b228-5a733648c375.png)

# KnowYourCoroutines
This is a simple plugin for LabAPI that adds some useful commands for debugging MEC coroutines
> **⚠️ Warning:** Familiarize yourself with the principles of MEC coroutines before using the commands.
# Installation
Put plugin dll from [releases](https://github.com/CosmosZvezdo4kin/KnowYourCoroutines/releases/latest) in `LabAPI/plugins/...`
# Commands
| Command          | Description                        | Arguments                              | Required Permission |
|------------------|------------------------------------|----------------------------------------|---------------------|
| coroutine list   | Get list of coroutines by category | (All / Running / Paused) \<GetFields\> | `kyc.list`          |
| coroutine kill   | Kills coroutine by Id              | (Id)                                   | `kyc.kill`          |
| coroutine pause  | Pauses coroutine by Id             | (Id)                                   | `kyc.pause`         |
| coroutine resume | Resumes coroutine by Id            | (Id)                                   | `kyc.resume`        |

# Example of usage
* `coroutine list all` - gets a list of all coroutines
* `coroutine list valid` - get a list of all valid coroutines
* `coroutine list paused` - gets a list of all paused coroutines
* `coroutine list all true` - gets a list of all coroutines with their fields
* `coroutine kill 5` - kills coroutine with Id 5 if it is valid

# FAQ
> **How to get coroutine Id ?** 
> 
> Use `coroutines list <category here>`
>
> ![Id Location](https://i.postimg.cc/9QNgNLrq/NVIDIA-Overlay-ym7rzvgkr-B.png)

> **How to find out the name of the coroutine method from the `CancelWith` method ?** 
> 
> Use `coroutines list <category here> true`
> 
> ![Field Location](https://i.postimg.cc/D0dYjFX2/NVIDIA-Overlay-9-Mz-Eo1a4-X7.png)

> **What is this plugin for ?**
> 
> For more accurate debugging of MEC coroutines. And the purposes of such debugging can be as follows:
> * Check for unnecessary duplicate coroutines
> * Check the TPS costs of a coroutine