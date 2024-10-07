# Website Down Tracker
WebsiteDownTracker is a background service running on .NET 8 This service can monitor given websites and notify via Telegram message.

[Check shell version](https://github.com/bariskisir/ShellWebsiteDownTracker)


## Usage
```sh
docker run -d --name website-down-tracker --restart always -e "Websites=https://www.google.com https://www.microsoft.com" -e "TelegramBotKey=BOT_KEY" -e "TelegramChatId=CHAT_ID" bariskisir/websitedowntracker
```

### Environment variables
"Websites" -> list of websites.

"TelegramBotKey" -> telegram bot key [Create telegram bot with @BotFather](https://t.me/botfather)

"TelegramChatId" -> chat id or user id [Get your user id with @userinfobot](https://t.me/userinfobot)

"Delay" -> check frequency in minute (default 5)

"Timeout" -> timeout for request in minute (default 2)

"DownCountLimit" -> Minimum allowed retry count (default 2)


[Dockerhub](https://hub.docker.com/r/bariskisir/websitedowntracker)
