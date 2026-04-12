<p align="center">
  <img src="Logo/256.png" alt="Releasarr" width="128" />
</p>

<h1 align="center">Releasarr</h1>

<p align="center">
  Monitor your Plex watchlist. Track downloads via Sonarr & Radarr. Get notified when content is ready.
</p>

---

Releasarr is an *arr-style application that bridges the gap between your Plex watchlist and your media automation stack. It monitors what you want to watch, tracks download progress through Sonarr and Radarr, and sends rich push notifications the moment content becomes available.

## Features

- **Plex Watchlist & Playlist Sync** -- Automatically imports items from your Plex watchlist and named playlists, parsing TMDB/TVDB/IMDB IDs for accurate matching
- **Sonarr & Radarr Integration** -- Matches tracked items to your existing Sonarr series and Radarr movies, monitors download queues and availability status
- **Rich Notifications** -- Episode-specific details (S01E01 - Episode Title), show overview, runtime, poster image, and direct links to IMDb/TMDb/TVDB
- **20 Notification Providers** -- Pushover (with image attachments), ntfy (with image + click URL), Discord (rich embeds with poster thumbnail), Telegram (HTML links), plus 16 more
- **Dashboard** -- At-a-glance summary of watchlisted, downloading, available, and notified content
- **Tracked Content View** -- Filterable table with poster art, title, content type, status badges, and timestamps

## How It Works

```
Plex Watchlist/Playlist
        |
        v
   Releasarr  <---->  Sonarr / Radarr
        |
        v
  Push Notification
  (with poster, episode info, links)
```

1. **Watchlist Sync** (every 15 min) -- Pulls items from Plex, creates tracked entries, matches to Sonarr/Radarr
2. **Status Check** (every 5 min) -- Polls Sonarr/Radarr APIs for download queue and file availability
3. **Notification** -- When content transitions to available, fires a rich notification with episode details, metadata, and external links

## Quick Start

### Docker

```yaml
services:
  releasarr:
    image: releasarr/releasarr:latest
    ports:
      - "9898:9898"
    volumes:
      - ./config:/config
    environment:
      - TZ=America/New_York
```

### Manual

```bash
dotnet run --project src/NzbDrone.Console/Releasarr.Console.csproj \
  --framework net8.0 -- --nobrowser --data=./config
```

Then open `http://localhost:9898` and configure:

1. **Settings > Media Servers** -- Add your Plex server (URL + auth token)
2. **Settings > Arr Clients** -- Add Sonarr and/or Radarr instances (URL + API key)
3. **Settings > Notifications** -- Configure notification providers and enable "On Content Available"

## Notification Providers

All providers support the Content Available event with rich metadata:

Apprise, CustomScript, Discord, Email, Gotify, Join, Notifiarr, ntfy, Prowl, PushBullet, Pushcut, Pushover, SendGrid, Signal, Simplepush, Telegram, Twitter, Webhook

## Configuration

| Setting | Default | Description |
|---|---|---|
| Port | 9898 | Web UI and API port |
| Watchlist Sync | 15 min | How often to poll Plex for new watchlist items |
| Download Status Check | 5 min | How often to check Sonarr/Radarr for availability |

## Built On

Forked from [Prowlarr](https://github.com/Prowlarr/Prowlarr)'s NzbDrone framework -- .NET 8, SQLite, React/Redux frontend.

## License

[GNU GPL v3](http://www.gnu.org/licenses/gpl.html)
