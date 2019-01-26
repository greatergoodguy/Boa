using System;

public class Clock {
    public float elapsedTime { get; private set; }
    public bool paused { get; private set; }

    const float ticksPerSecond = 20;

    public bool Tock(float delta) {
        if (paused) return false;

        elapsedTime += delta;

        return elapsedTime > 1 / ticksPerSecond;
    }

    public void Reset() {
        elapsedTime = 0;
        paused = false;
    }

    public void Pause() => paused = true;

    public void Unpause() => paused = false;
}
