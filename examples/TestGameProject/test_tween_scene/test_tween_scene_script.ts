export default class TestTweenScene {
    // Default Injections
    private self: Script;
    private parent: Scene;

    // Prefab Injections
    private explosionGo: GameObject;

    // Class Fields
    private sprite: AnimatedSprite;
    private tweenPositionIn: Tween;
    private tweenPositionOut: Tween;
    private tweenFrameIn: Tween;
    private tweenFrameOut: Tween;

    public onInit() {
        Logging.Append(Logging.LogSeverity.Info, "TestTweenScene", "Current scene name: " + SceneController.ActiveScene.Name);

        this.sprite = this.explosionGo.Components.find((value) => value.Type === ManagedObjectType.Sprite2D) as AnimatedSprite;
        this.explosionGo.Transform.Position = new ManagedMath.Vector3(-3, -1.5, 0);

        this.createTweens();
        this.linkTweens();

        this.tweenPositionIn.Run(CoroutineContext.Render);
        this.tweenFrameIn.Run(CoroutineContext.Render);
    }

    public onUpdate(deltaTime) {
        //
    }

    public onRender(deltaTime) {
        //
    }

    private createTweens() {
        let targetTime = 2500;

        this.tweenPositionIn = new Tween(TweenEaseType.Linear, targetTime, this.explosionGo.Transform.Position, {
            X: 3,
            Y: 1.5,
        });

        this.tweenPositionOut = new Tween(TweenEaseType.Linear, targetTime, this.explosionGo.Transform.Position, {
            X: -3,
            Y: -1.5,
        });

        this.tweenFrameIn = new Tween(TweenEaseType.Linear, targetTime, this.sprite, {
            Frame: this.sprite.FrameCount - 1,
        });

        this.tweenFrameOut = new Tween(TweenEaseType.Linear, targetTime, this.sprite, {
            Frame: 0,
        });
    }

    private linkTweens() {
        this.tweenPositionIn.OnEnd.Subscribe(() => {
            this.tweenPositionOut.Run(CoroutineContext.Render);
        });

        this.tweenPositionOut.OnEnd.Subscribe(() => {
            this.tweenPositionIn.Run(CoroutineContext.Render);
        });

        this.tweenFrameIn.OnEnd.Subscribe(() => {
            this.tweenFrameOut.Run(CoroutineContext.Render);
        });

        this.tweenFrameOut.OnEnd.Subscribe(() => {
            this.tweenFrameIn.Run(CoroutineContext.Render);
        });
    }
}
