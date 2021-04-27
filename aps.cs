using System;
namespace SendPushTest
{
    public class Aps2
    {

        public string alert { get; set; }
        public string sound { get; set; } = "default";
        public int badge { get; set; }
    }

    public class Aps
    {
        public Aps2 aps { get; set; }
    }
}
