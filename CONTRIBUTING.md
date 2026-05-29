# Contributing to MobiFlight

There are many ways to get involved with MobiFlight beyond writing code.

### Report a bug

Open a [bug report issue](https://github.com/MobiFlight/MobiFlight-Connector/issues/new?template=bug_report.md) on GitHub. A clear description and steps to reproduce help us diagnose and fix things faster. GitHub issues are preferred over Discord for bug reports — they are easier to track and less likely to get overlooked. That said, if you don't have a GitHub account, you can also share a bug report in the [Discord server](https://discord.gg/U28QeEJpBV) in #mobiflight channel for example, and someone can likely file an issue for you.

### Request a feature

Open a [feature request issue](https://github.com/MobiFlight/MobiFlight-Connector/issues/new?template=feature_request.md). Before filing, consider discussing the idea on [Discord](https://discord.gg/U28QeEJpBV) first — it helps shape the request and avoid duplicate effort. On discord there is #development channel that is used to discuss ideas related to development of MobiFlight.

### Code contributions

We use pull requests for code contributions. If you are new to this workflow, GitHub's [contributing to a project](https://docs.github.com/en/get-started/exploring-projects-on-github/contributing-to-a-project) guide is a good starting point.

For setting up the development environment and compiling MobiFlight, see the [frontend README](src/MobiFlightConnector/frontend/README.md), it covers the whole development setup.

### Translations

Translation files live in [`src/MobiFlightConnector/frontend/public/locales/`](src/MobiFlightConnector/frontend/public/locales/). Each language has its own folder with a `translation.json` file. The three core languages (`en`, `de`, `es`) must be complete before a PR can be merged.

### Controller definition files

Controller definitions are `.joystick.json` files in [`src/MobiFlightConnector/Joysticks/`](src/MobiFlightConnector/Joysticks/), organised by vendor. Adding support for a new USB HID controller means adding a definition file there.

### Winwing CDU script mappings

Python scripts that drive Winwing CDU hardware for specific aircraft add-ons live in [`src/MobiFlightConnector/Scripts/Winwing/`](src/MobiFlightConnector/Scripts/Winwing/). See the [README there](src/MobiFlightConnector/Scripts/README.md) for how they work and how to add support for a new aircraft.

### HubHop presets

[HubHop](https://hubhop.mobiflight.com/) is a community preset database for MSFS. Sharing your event and variable configurations there makes them available to all MobiFlight users — no code changes needed.

### Helping others

We support new users on the "Support topics" section on our discord server and do this as volunteers, because we find MobiFlight useful and appreciate our community. If you have learned your way around profiles and troubleshooting and creating profiles, and want to contribute back by helping others, your help is very much appreciated!

---

## Supporting MobiFlight

MobiFlight is free and open source, and contributions go beyond code. There are two ways to support the project financially:

**MobiFlight Club** — a membership program at [mobiflight.com/members](https://mobiflight.com/members) that helps keep the project running and the team motivated by supporting MobiFlight with an affordable monthly sponsorship fee. In return, we want to add more ways for members to weigh in on decisions and feature requests, to have their voice heard.

**GitHub Sponsors** — one-time or recurring sponsorships via [github.com/sponsors/MobiFlight](https://github.com/sponsors/MobiFlight).

### For businesses and hardware vendors

MobiFlight benefits the wider flight sim ecosystem: hardware vendors and cockpit builders use MobiFlight as a platform to bring products to market faster, without having to build simulator integration from scratch.

If your business relies on MobiFlight, there are several ways to collaborate beyond a standard sponsorship:

- **Partnership** — co-branding, joint announcements, or featured placement for hardware that works with MobiFlight
- **Consulting** — hands-on help integrating MobiFlight support into your product or manufacturing workflow
- **Collaboration** — co-development of features that benefit both your product and the wider MobiFlight community

To start the conversation, email [info@mobiflight.com](mailto:info@mobiflight.com).

---

## Discord

The MobiFlight [Discord server](https://discord.gg/U28QeEJpBV) is the hub for the community. A few pointers on where to go:

- **Development discussions** — explore ideas before opening an issue, ask questions while working on a PR, or discuss architecture and implementation
- **User support** — questions on how to use MobiFlight go in the support channels: **#mobiflight**, **#hardware**, and sim-specific channels like **#msfs2024** and **#xplane**
- **Bug reports without GitHub** — if you don't have a GitHub account, share your bug in Discord and someone can file it for you

If you've opened an issue or PR and haven't heard back, be patient, and if needed, give a polite nudge on Discord to get our attention.

---

## Patience

MobiFlight is maintained by a small volunteer team. Reviews and responses can sometimes take a while — we appreciate your patience.
