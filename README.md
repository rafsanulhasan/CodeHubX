# CodeHubX
[![Twitter URL](https://img.shields.io/badge/tweet-%40rafsanulhasan-blue.svg?style=social&style=flat-square)](https://twitter.com/rafsanulhasan)

CodeHubX is an x-plat GitHub client that helps you keep up with the open source world.

<p align="center">
    <a href="https://www.microsoft.com/store/apps/9nblggh52tbd?ocid=badge"><img src="https://www.github.com/sics/codehubx/tree/dev/Codehubx/assets/images/unified storelogo.jpg" alt="Available at stores" width='200' /></a>
</p>

# Supported Platforms

<table>
    <thead>
        <tr>
            <td align='center'>Form-Factor / Platform</td>
            <td align='center'>Windows</td>
            <td align='center'>OSX</td>
            <td align='center'>Linux</td>
        </tr>
    <thead>
    <tr>
        <td align='center'>Mobile</td>
        <td align='center'>Universal<sup>1</sup><br/><img src='https://dev.azure.com/sicstechgithub/CodeHubX/_apis/build/status/Windows.UWP'/></td>
        <td align='center'>iOS Universal<sup>2</sup><br/><img src='https://dev.azure.com/sicstechgithub/CodeHubX/_apis/build/status/Apple.iOS'/></td>
        <td align='center'>Android</br>Mobile &amp; Tablet<br/><img src='https://dev.azure.com/sicstechgithub/CodeHubX/_apis/build/status/Android'/></td></td>
    </tr>
    <tr>
        <td align='center'>Desktop</td>
        <td align='center'>WPF<sup>3</sup><br/><img src='https://dev.azure.com/sicstechgithub/CodeHubX/_apis/build/status/Windows.WPF'/></td>
        <td align='center'>Coming soon</td>
        <td align='center'>Desktop<sup>4</sup><br/><img src='https://dev.azure.com/sicstechgithub/CodeHubX/_apis/build/status/Linux.Desktop'/></td>
    </tr>
    <tr>
        <td align='center'>TV</td>
        <td align='center'>-</td>
        <td align='center'>Coming soon</td>
        <td align='center'>Coming Soon</td>
    </tr>
    <tr>
        <td align='center'>Watch</td>
        <td align='center'>Coming soon</td>
        <td align='center'>Coming soon</td>
        <td align='center'>Coming soon</td>
    </tr>
</table>

1. Universal in Windows means Windows 10 Store App for Desktop, Mobile & Surface Book
2. Universal in Apple Platforms means App Store App for i, iPod & iPad
3. WPF in Windows supports desktops with Windows 10 and earlier with more low level API access
4. Must have GTK 2.0 and GTK Sharp installed in Linux

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
