export default class TestScript {
    // Default Injections
    private self: Script;
    private parent: GameObject;

    // Prefab Injections
    private explosionSprite: AnimatedSprite;

    private counter: number;

    public onInit() {
        Logging.Append(Logging.LogSeverity.Info, "TestScript", "Current gameobject name: " + this.parent.Name);

        this.counter = 0;
    }

    public onUpdate(deltaTime) {
        //
    }

    public onRender(deltaTime) {
        if (!this.explosionSprite || !this.explosionSprite.Active) {
            return;
        }

        this.counter += deltaTime;

        if (this.counter > 0.075) {
            if (this.explosionSprite.Frame < this.explosionSprite.FrameCount - 1) {
                this.explosionSprite.Frame++;
                this.counter = 0;
            } else if (this.explosionSprite.Frame === this.explosionSprite.FrameCount - 1) {
                this.explosionSprite.Frame = 0;
            }
        }
    }
}
