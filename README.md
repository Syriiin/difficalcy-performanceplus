# difficalcy-osu

This is a [difficalcy](https://github.com/Syriiin/difficalcy) calculator for the PP+ (PerformancePlus) algorithm.

The core PP+ algorithm was originally developed by [Drezi](https://osu.ppy.sh/users/3936645) as part of the [PP+ project](https://syrin.me/pp+/).

Later, I ([Syrin](https://osu.ppy.sh/users/5701575)) [ported the algorithm to lazer](https://github.com/Syriiin/osu/tree/performanceplus) to release it open source.

This project can be used to run the PP+ calculator easily within your own projects.

## Basic usage

Run the server:

```sh
docker run -p 5000:80 ghcr.io/syriiin/difficalcy-performanceplus:latest
```

Call the API:

```sh
curl "localhost:5000/api/calculation?BeatmapId=658127"
```

Get your lazer powered calculations:

```json
{
  "difficulty": {
    "aim": 3.6287549779248565,
    "jumpAim": 3.330914733838406,
    "flowAim": 3.587152579074192,
    "precision": 1.1191203443788618,
    "speed": 3.0511709685023125,
    "stamina": 3.296170361440796,
    "accuracy": 1.0555997725980655,
    "total": 6.995950245446664
  },
  "performance": {
    "aim": 186.47861832774464,
    "jumpAim": 144.22702842513866,
    "flowAim": 180.1381333317369,
    "precision": 5.469989357009822,
    "speed": 110.45944769641248,
    "stamina": 139.26185856575086,
    "accuracy": 196.72751407194684,
    "total": 530.1091860753253
  }
}
```

See [the difficalcy Getting Started page](https://Syriiin.github.io/difficalcy/getting-started.md) for a full example setup.
