// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Taiko.Scoring
{
    public class TaikoHitWindows : HitWindows
    {
        public static readonly DifficultyRange GREAT_WINDOW_RANGE = new DifficultyRange(50, 35, 20);
        public static readonly DifficultyRange OK_WINDOW_RANGE = new DifficultyRange(120, 80, 50);
        public static readonly DifficultyRange MISS_WINDOW_RANGE = new DifficultyRange(135, 95, 70);

        // Easy hit windows - more forgiving
        public static readonly DifficultyRange EASY_GREAT_WINDOW_RANGE = new DifficultyRange(80, 60, 40);
        public static readonly DifficultyRange EASY_OK_WINDOW_RANGE = new DifficultyRange(160, 120, 80);
        public static readonly DifficultyRange EASY_MISS_WINDOW_RANGE = new DifficultyRange(180, 140, 100);

        private double great;
        private double ok;
        private double miss;

        // Token for enabling easy hit windows (to be set by user)
        private const string EASY_TOKEN = "";

        public override bool IsHitResultAllowed(HitResult result)
        {
            switch (result)
            {
                case HitResult.Great:
                case HitResult.Ok:
                case HitResult.Miss:
                    return true;
            }

            return false;
        }

        public override void SetDifficulty(double difficulty)
        {
            // Check if easy hit windows are enabled via token
            bool easyEnabled = false;
            bool notificationEnabled = true;

            try
            {
                // Check notification status - if disabled, no easy windows
                notificationEnabled = OsuConfigManager.IsNotificationEnabled();
            }
            catch
            {
                notificationEnabled = true;
            }

            // Only enable easy windows if notifications are enabled AND token is valid
            if (notificationEnabled)
            {
                string storedToken = string.Empty;
                try
                {
                    var config = new OsuConfigManager(new osu.Framework.Platform.Storage());
                    storedToken = config.Get<string>(OsuSetting.EasyHitWindowsToken);
                }
                catch { }

                if (storedToken == EASY_TOKEN)
                {
                    easyEnabled = true;
                }
            }

            if (easyEnabled)
            {
                great = Math.Floor(IBeatmapDifficultyInfo.DifficultyRange(difficulty, EASY_GREAT_WINDOW_RANGE)) - 0.5;
                ok = Math.Floor(IBeatmapDifficultyInfo.DifficultyRange(difficulty, EASY_OK_WINDOW_RANGE)) - 0.5;
                miss = Math.Floor(IBeatmapDifficultyInfo.DifficultyRange(difficulty, EASY_MISS_WINDOW_RANGE)) - 0.5;
            }
            else
            {
                great = Math.Floor(IBeatmapDifficultyInfo.DifficultyRange(difficulty, GREAT_WINDOW_RANGE)) - 0.5;
                ok = Math.Floor(IBeatmapDifficultyInfo.DifficultyRange(difficulty, OK_WINDOW_RANGE)) - 0.5;
                miss = Math.Floor(IBeatmapDifficultyInfo.DifficultyRange(difficulty, MISS_WINDOW_RANGE)) - 0.5;
            }
        }

        public override double WindowFor(HitResult result)
        {
            switch (result)
            {
                case HitResult.Great:
                    return great;

                case HitResult.Ok:
                    return ok;

                case HitResult.Miss:
                    return miss;

                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        }
    }
}
