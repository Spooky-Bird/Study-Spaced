namespace ServerApp.Services
{
    public class Interval
    {
        public int delay { get; set; }
        public int duration { get; set; }
        public Interval(int delay, int duration)
        {
            this.delay = delay;
            this.duration = duration;
        }
    }
}
