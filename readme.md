## Introduction

diggcordslash is Discord bot with a couple of slash commands.

## Download

Compiled downloads are not available.

## Compiling

To clone and run this application, you'll need [Git](https://git-scm.com) and [.NET](https://dotnet.microsoft.com/) installed on your computer. From your command line:

```
# Clone this repository
$ git clone https://github.com/btigi/diggcordslash

# Go into the repository
$ cd src

# Build  the app
$ dotnet build
```

## Usage

Set the required environment variables:
- SlashBotApplicationId - the application id of the Discord bot, available from the General Information tab of the bot's management page.
- SlashBotBotToken - the security token of the Discord bot, available from the Bot tab of the bot's management page.
- SlashBotDiscordPublicKey - the public key of the Discord bot, available from the General Information tab of the bot's management page.
- SlashBotSecurityId - a custom security key, to be passed on each request as a mechanism to prevent unauthorized calls.

The project has an OpenAPI specification to facilitate discovery of endpoints, though as a guide:
a) Call listavailablecommands to list available commands
b) Call registercommand to register a command with Discord
c) Call listregisteredcommands to list commands registered with Discord
d) Call deletecommand to delete a command from Discord

Additional commands can be added by creating a new class implementing the ICommand interace and adding the Command attribute.


## Licencing

diggcordslash is licenced under CC BY-NC-ND 4.0 https://creativecommons.org/licenses/by-nc-nd/4.0/ Full licence details are available in licence.md