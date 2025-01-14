import Vector3 = ManagedMath.Vector3;

export default class CarScript {
    // Default Injections
    private self: Script;
    private parent: GameObject;

    // Prefab Injections
    private sprite1: AnimatedSprite;
    // private sprite2: Sprite2D;
    private camera: Camera;
    private rigidBody: RigidBody;

    // private counter: number;

    public onInit() {
        Logging.Append(Logging.LogSeverity.Info, "CarScript", "Test car created.");
        // this.sprite1.ShouldDraw = false;

        // this.counter = 0;
    }

    public onUpdate(deltaTime) {
        // if (this.rigidBody.Active || this.counter < 0) {
        //     return;
        // }
        //
        // this.counter += deltaTime;
        //
        // if (this.counter > 5) {
        //     this.counter = -1;
        //     this.rigidBody.Active = true;
        // }
    }

    public onRender(deltaTime) {
        // let rotation = this.camera.Transform.Rotation.Add(new Vector3(0, 0, 40 * deltaTime));
        // this.camera.Transform.Rotation = rotation;
        // let rotation = this.sprite2.Transform.Rotation.Add(new Vector3(0, 0, 40 * deltaTime));
        // this.sprite1.Transform.Rotation = rotation;
        // this.sprite2.Transform.Rotation = rotation;
    }
}
