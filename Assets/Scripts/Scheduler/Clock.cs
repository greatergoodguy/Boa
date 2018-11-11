public class Clock {
    public float elapsedTime { get; private set; }

    float ticksPerSecond = 10;

    public bool Tock(float delta) {
        elapsedTime += delta;

        return elapsedTime > 1 / ticksPerSecond;
    }

    public void Reset() {
        elapsedTime = 0;
    }
}
