using System.Collections.Generic;

namespace AmbiBoxApiCaller
{

    public class LedCollection:List<LedZone>
    {
        public void AddZone(int ledNo, string rgbColor)
        {
            Add(new LedZone
            {
                LedNo = ledNo,
                RGBColour = rgbColor
            });
        }

        public string GenerateCommand()
        {
            string command = "setcolor:";
            foreach (var ledZone in this)
                command += $"{ledZone.LedNo}-{ledZone.RGBColour};";

            return command;
        }
    }

    public class LedZone
    {
        public int LedNo { get; set; }
        public string RGBColour { get; set; }
    }
}
