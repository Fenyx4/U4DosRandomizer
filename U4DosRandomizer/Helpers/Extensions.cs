using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace U4DosRandomizer.Helpers
{
    public static class Extensions
    {
        // https://github.com/MrYossu/MazesForProgrammers/blob/1fae491a80240f80c334f2721189dd43a0146dec/Mazes.Models/Models/Extensions.cs
        public static T Rand<T>(this IEnumerable<T> items, Random r = null) =>
            items.Shuffle(r).First();

        // https://github.com/MrYossu/MazesForProgrammers/blob/1fae491a80240f80c334f2721189dd43a0146dec/Mazes.Models/Models/Extensions.cs
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items, Random r) =>
            items.OrderBy(n => r.Next());
    }
}
