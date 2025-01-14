import Vector3 = ManagedMath.Vector3;

export default class TestScript {
    // Default Injections
    private self: Script;
    private parent: GameObject;

    // Prefab Injections
    private explosionSound: Sound;
    private spriteExplosion: AnimatedSprite;

    private counter: number;
    private reset: boolean;
    private initComplete: boolean;

    public onInit() {
        Logging.Append(Logging.LogSeverity.Info, "TestScript", "Current gameobject name: " + this.parent.Name);

        this.counter = 0;

        this.spriteExplosion.ShouldDraw = false;
        this.spriteExplosion.Active = true;

        this.parent.Transform.Position = new Vector3(-3.5, 0, 0);

        this.spriteExplosion.Frame = 0;
        this.spriteExplosion.ShouldDraw = true;

        SoundController.UnmuteAudio();

        this.explosionSound.Play();
        this.initComplete = true;
    }

    public onUpdate(deltaTime) {
        // console.log("Status of test sound is: " + this.sound.PlaybackStatus);
    }

    public onRender(deltaTime) {
        if (!this.initComplete || !this.spriteExplosion || !this.spriteExplosion.Active) {
            return;
        }

        if (this.parent.Transform.Position.X < 3.5) {
            this.parent.Transform.Position = this.parent.Transform.Position.Add(new Vector3(2 * deltaTime, 0, 0));
            this.counter = 0;
        } else {
            this.explosionSound.Play();
            this.parent.Transform.Position = new Vector3(-3.5, 0, 0);
        }
    }
}
