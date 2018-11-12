# CodeHubX [![Build Status](https://dev.azure.com/sicstechgithub/CodeHubX/_apis/build/status/Build)](https://dev.azure.com/sicstechgithub/CodeHubX/_build/latest?definitionId=2)
<span class="badge-patreon"><a href="https://www.patreon.com/aalok05" title="Donate to this project using Patreon"><img src="https://img.shields.io/badge/patreon-donate-yellow.svg" alt="Patreon donate button" /></a></span>
[![Twitter URL](https://img.shields.io/badge/tweet-%40rafsanulhasan-blue.svg?style=social&style=flat-square)](https://twitter.com/rafsanulhasan)

CodeHubX is an x-plat GitHub client that helps you keep up with the open source world.

<p align="center">
    <a href="https://www.microsoft.com/store/apps/9nblggh52tbd?ocid=badge"><img src="https://www.github.com/sics/codehubx/tree/dev/Codehubx/assets/images/unified storelogo.jpg" alt="Available at stores" width='200' /></a>
</p>

# Supported Platforms

<table style='text-align:center'>
    <thead>
        <tr>
            <td>Form-Factor/Platform</td>
            <td>Windows</td>
            <td>OSX</td>
            <td>Linux</td>
        </tr>
    <thead>
    <tr>
        <td>Mobile</td>
        <td>Universal*<br/><img src='https://dev.azure.com/sicstechgithub/CodeHubX/_apis/build/status/Windows.UWP'/></td>
        <td>Universal*<br/><img src='https://build.appcenter.ms/v0.1/apps/95af53c2-f347-483c-95a0-1f9c33fdb89d/branches/dev/badge'/></td>
        <td>Android<br/><img src='https://build.appcenter.ms/v0.1/apps/7965f46a-842c-4cce-b0bc-4ba627837cc4/branches/dev/badge'/></td></td>
    </tr>
    <tr>
        <td>Desktop</td>
        <td>WPF*</td>
        <td>Coming soon</td>
        <td>Desktop with GTK</td>
    </tr>
    <tr>
        <td>TV</td>
        <td>-</td>
        <td>Coming soon</td>
        <td>Coming Soon</td>
    </tr>
    <tr>
        <td>Watch</td>
        <td>Coming soon</td>
        <td>Coming soon</td>
        <td><img src='https://build.appcenter.ms/v0.1/apps/b2b123d2-65a1-4e0e-af48-6252bd4d335f/branches/dev/badge'/></td>
    </tr>
</table>

* Universal* in Apple Platforms means App Store App for iOS, iPod & iPad
* Universal* in Windows means Windows 10 Store App for Desktop, Mobile & Surface Book
* WPF* in Windows supports desktops with Windows 10 and earlier with more low level API access

## Features
* Trending repositories
* News Feed
* View code (with syntax highlighting), issues and comments.
* Create Issues
* Comment on Issues and PRs
* Choose from 9 different syntax highlighting styles
* Search repositories, users, issues and code
* Star, Watch and Fork repositories
* Follow users
* Edit profile

## Screenshots

|               |                   |
|:-------------:| :----------------:|
| ![screenshot](https://raw.githubusercontent.com/sicsbd/CodeHubX/dev/ScreenShots/repoView.PNG)  | ![screenshot](https://raw.githubusercontent.com/sicsbd/CodeHubX/dev/ScreenShots/trending.PNG) |


## Contributing
Do you want to contribute? Here are our [contribution guidelines](https://github.com/sicsbd/CodeHubX/blob/master/CONTRIBUTING.md).

## Setting up the project
* [Register](https://github.com/settings/developers) your OAuth application and paste your key and secret in the `app.config` file in the root of the project.

## URI Schemes
You can launch CodeHub and navigate to repositories and user profiles using custom URI schemes

Examples:
- _codehubx://repository/sicsbd/codehubx_
- _codehubx://user/rafsanulhasan_

## Troubleshooting

### I Can't Find My Organization

CodeHubX can see all organizations *if they are granted access*. GitHub, by default, disables [third-party access](https://help.github.com/articles/about-third-party-application-restrictions/) for new organizations. Because of this, CodeHub has no knowledge that those organizations even exist. GitHub keeps that information from the app. There are several ways to correct this. If you own the organization follow [these instructions](https://help.github.com/articles/enabling-third-party-application-restrictions-for-your-organization/). If you do not own the organization you can request access for CodeHub by following [these instructions](https://help.github.com/articles/requesting-organization-approval-for-third-party-applications/).

## Dependencies
I thank the makers of these libraries
* [Octokit](https://github.com/octokit/octokit.net)
* [UICompositionAnimations](https://github.com/Sergio0694/UICompositionAnimations)
* [MVVM Light](https://www.nuget.org/packages/MvvmLightLibs/)
* [UWP Community Toolkit](https://github.com/Microsoft/UWPCommunityToolkit)
* [HTML Agility Pack](https://www.nuget.org/packages/HtmlAgilityPack)
* [LocalNotifications](https://github.com/RavinduL/LocalNotifications)
* [UWP-Animated-SplashScreen](https://github.com/XamlBrewer/UWP-Animated-SplashScreen)
* [QueryString.Net](https://www.github.com/WindowsNotifications/QueryString.Net)
* [Xamarin](https://visualstudio.microsoft.com/xamarin/)
  - SkiaSharp   

## Gitter chat
* https://gitter.im/SynergyCodeHubX/Bugs
* https://gitter.im/CodehubUWP/Features
* https://gitter.im/CodehubUWP/Discussion
* https://gitter.im/CodehubUWP/CrashReports
