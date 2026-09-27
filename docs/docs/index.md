# Welcome to difficalcy-performanceplus

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
curl "localhost:5000/api/calculators/osu/calculation?BeatmapId=658127"
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

!!! tip "Non-finite values"
    Sometimes, due to broken maps or bugs, the calculator can return non-finite values. These are serialised as `"NaN"`, `"Infinity"` or `"-Infinity"` strings.

## Image variants

difficalcy-performanceplus is published in two variants with the same API:

| Variant | Tags                                                      | Description                                                                                                                 |
| ------- | --------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------- |
| Slim    | `latest`, `vX.Y.Z` (`latest-slim`, `vX.Y.Z-slim` aliases) | Default lightweight image. Unnecessary game resources and dependencies are stripped.                                         |
| Full    | `latest-full`, `vX.Y.Z-full`                              | Complete unstripped image (5-6x larger). Use this if you run into issues with the slim image, and please submit a bug report! |

Both variants are tested with the same end-to-end suite, so calculation results are equivalent.

See [the difficalcy Getting Started page](https://difficalcy.syrin.me/getting-started) for a full example setup.
