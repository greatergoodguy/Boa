using System;

public class Clock {
    public float elapsedTime { get; private set; }
    public bool paused { get; private set; }

    const float ticksPerSecond = 10;

    public bool Tock(float delta) {
        if (paused) return false;

        elapsedTime += delta;

        return elapsedTime > 1 / ticksPerSecond;
    }

    public void Reset() {
        elapsedTime = 0;
        paused = false;
    }

    internal void Pause() {
        paused = true;
    }

    internal void Unpause() {
        paused = false;
    }
}
