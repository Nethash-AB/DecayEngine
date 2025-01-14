import Vector3 = ManagedMath.Vector3;

export default class TestScript {
    // Default Injections
    private self: Script;
    private parent: GameObject;

    // Prefab Injections
    private explosionSound: Sound;
    private introSound: Sound;

    private sprite: AnimatedSprite;
    private counter: number;
    private reset: boolean;
    private initComplete: boolean;

    public onInit() {
        Logging.Append(Logging.LogSeverity.Info, "TestScript", "Current gameobject name: " + this.parent.Name);

        this.sprite = this.parent.Components.find((value) => value.Type === ManagedObjectType.Sprite2D) as AnimatedSprite;
        this.counter = 0;

        let sceneScript = SceneController.ActiveScene.Components.find((value) => value.Type === ManagedObjectType.Script) as Script;
        if (sceneScript) {
            let testVar = sceneScript.GetProperty("testVar");
            console.log("Value of testVar from ActiveScene script is: " + testVar);

            sceneScript.SetProperty("testVar", "ayylmao");

            testVar = sceneScript.GetProperty("testVar");
            console.log("Value of testVar from ActiveScene script after change is: " + testVar);
        }

        this.sprite.ShouldDraw = false;
        this.sprite.Active = true;

        this.parent.Transform.Position = new Vector3(-3, 0, 0);

        // this.explosionSound = parent.Components.find((value) => value.Type === ManagedObjectType.Sound && value.Name === "explosion_sound") as Sound;
        if (!this.explosionSound) {
            Logging.Append(Logging.LogSeverity.Error, "TestScript", "Could not find explosion sound, aborting script.");
            return;
        }

        // this.introSound = parent.Components.find((value) => value.Type === ManagedObjectType.Sound && value.Name === "intro_sound") as Sound;
        if (!this.introSound) {
            Logging.Append(Logging.LogSeverity.Error, "TestScript", "Could not find intro sound, aborting script.");
            return;
        }

        this.introSound.OnTimelineMarkerReached.Subscribe((sound: Sound, name: string, position: number) => {
            console.log(`Sound (${sound.Name}) reached marker (${name}) at position (${position})`);
            switch (name) {
                case "TestMarker":
                    sound.TriggerCue();
                    break;
                case "FadeOut":
                    sound.OnTimelineMarkerReached.Clear();
                    this.sprite.Frame = 0;
                    this.sprite.ShouldDraw = true;
                    this.explosionSound.Play();
                    this.initComplete = true;
            }
        });

        SoundController.UnmuteAudio();

        this.introSound.Active = true;
    }

    public onUpdate(deltaTime) {
        // console.log("Status of test sound is: " + this.sound.PlaybackStatus);
    }

    public onRender(deltaTime) {
        if (!this.initComplete || !this.sprite || !this.sprite.Active) {
            return;
        }

        if (this.reset && this.explosionSound.PlaybackStatus !== SoundPlaybackStatus.Stopped) {
            return;
        }

        this.counter += deltaTime;

        if (this.counter > 0.025) {
            let parentPosition = this.parent.Transform.Position;
            // console.log(`Parent position: (${parentPosition.X}, ${parentPosition.Y}, ${parentPosition.Z})`);
            if (this.sprite.Frame < this.sprite.FrameCount - 1) {
                this.sprite.Frame++;
                this.parent.Transform.Position = this.parent.Transform.Position.Add(new Vector3(0.15, 0, 0));
                this.counter = 0;
            } else if (this.sprite.Frame === this.sprite.FrameCount - 1) {
                if (this.reset) {
                    this.sprite.ShouldDraw = true;
                    this.sprite.Frame = 0;
                    this.reset = false;
                    this.explosionSound.Play();
                } else {
                    this.sprite.ShouldDraw = false;
                    this.reset = true;
                    this.counter = -0.75;
                    // console.log("Resetting transform");
                    this.parent.Transform.Position = new Vector3(-3, 0, 0);
                }
            }
        }
    }
}
