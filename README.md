# difficalcy-osu

This is a [difficalcy](https://github.com/Syriiin/difficalcy) calculator for the PP+ (PerformancePlus) algorithm.

The core PP+ algorithm was originally developed by [Drezi](https://osu.ppy.sh/users/3936645) as part of the [PP+ project](https://syrin.me/pp+/).

Later, with Drezi's permission, I ([Syrin](https://osu.ppy.sh/users/5701575)) [ported the algorithm to lazer](https://github.com/Syriiin/osu/tree/performanceplus) to release it open source.

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
  "accuracy": 1,
  "combo": 2402,
  "difficulty": {
    "aim": 3.627148161249041,
    "jumpAim": 3.3301542877140267,
    "flowAim": 3.585550004673542,
    "precision": 1.1184358822864646,
    "speed": 3.0511709685064643,
    "stamina": 3.2961703667829396,
    "accuracy": 1.0555997178136178,
    "total": 6.994179873428194
  },
  "performance": {
    "aim": 186.23100919283755,
    "jumpAim": 144.1282701053189,
    "flowAim": 179.89680893624967,
    "precision": 5.459959039032057,
    "speed": 110.45944769686339,
    "stamina": 139.26185924286102,
    "accuracy": 196.7275038620089,
    "total": 529.8565291447785
  }
}
```

See [the difficalcy Getting Started page](https://Syriiin.github.io/difficalcy/getting-started.md) for a full example setup.
