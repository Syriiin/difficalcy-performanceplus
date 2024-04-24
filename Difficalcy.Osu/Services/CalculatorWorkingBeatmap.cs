using System.IO;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.IO;
using osu.Game.Rulesets;
using osu.Game.Skinning;

namespace Difficalcy.Osu.Services
{
    public class CalculatorWorkingBeatmap : WorkingBeatmap
    {
        private readonly Beatmap _beatmap;

        public CalculatorWorkingBeatmap(Ruleset ruleset, Stream beatmapStream, string beatmapId) : this(ruleset, readFromStream(beatmapStream), beatmapId) { }

        private CalculatorWorkingBeatmap(Ruleset ruleset, Beatmap beatmap, string beatmapId) : base(beatmap.BeatmapInfo, null)
        {
            _beatmap = beatmap;

            _beatmap.BeatmapInfo.Ruleset = ruleset.RulesetInfo;
        }

        private static Beatmap readFromStream(Stream stream)
        {
            using var reader = new LineBufferedReader(stream);
            return Decoder.GetDecoder<Beatmap>(reader).Decode(reader);
        }

        protected override IBeatmap GetBeatmap() => _beatmap;
        protected override Texture GetBackground() => null;
        protected override Track GetBeatmapTrack() => null;
        protected override ISkin GetSkin() => null;
        public override Stream GetStream(string storagePath) => null;
    }
}
