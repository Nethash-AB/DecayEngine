import Quaternion = ManagedMath.Quaternion;
import Vector3 = ManagedMath.Vector3;

export default class TestScript {
    // Default Injections
    private self: Script;
    private parent: GameObject;

    // Scene Injections
    private sphereGameObject: GameObject;
    private platformGameObject: GameObject;
    private testCarGameObject: GameObject;
    private debugTextLabelGameObject: GameObject;

    // Input Actions
    private moveXAction: InputAction;
    private moveYAction: InputAction;
    private moveZAction: InputAction;
    private rotateXAction: InputAction;
    private rotateYAction: InputAction;
    private rotateZAction: InputAction;
    private resetAction: InputAction;
    private changeObjectAction: InputAction;

    private selectedObject: GameObject;
    private debugTextLabel: TextSprite;

    private pitch: number;
    private yaw: number;
    private roll: number;
    private dirtyRotation: boolean;

    public onInit() {
        Logging.Append(Logging.LogSeverity.Info, "TestScript", "Current parent name: " + this.parent.Name);
        console.log(`sphereGameObject is: ${this.sphereGameObject}`);
        console.log(`platformGameObject is: ${this.platformGameObject}`);
        console.log(`testCarGameObject is: ${this.testCarGameObject}`);
        console.log(`debugTextLabelGameObject is: ${this.debugTextLabelGameObject}`);

        this.debugTextLabel = this.debugTextLabelGameObject.Components.find((value) => value.Type === ManagedObjectType.TextSprite) as TextSprite;
        console.log(`debugTextLabel is: ${this.debugTextLabel}`);

        this.pitch = 0;
        this.yaw = 0;
        this.roll = 0;

        this.selectedObject = this.testCarGameObject;

        this.setupActions();
    }

    public onUpdate(deltaTime) {
        if (!this.selectedObject || !this.selectedObject.Active) {
            return;
        }

        let rotateSpeed = 16 * deltaTime;

        let deltaPitch = this.rotateXAction.AnalogValue * rotateSpeed;
        if (deltaPitch > 0 || deltaPitch < 0) {
            this.pitch += deltaPitch;
            this.dirtyRotation = true;
        }

        let deltaYaw = this.rotateYAction.AnalogValue * rotateSpeed;
        if (deltaYaw > 0 || deltaYaw < 0) {
            this.yaw += deltaYaw;
            this.dirtyRotation = true;
        }
    }

    public onRender(deltaTime) {
        if (!this.selectedObject || !this.selectedObject.Active) {
            return;
        }

        let moveSpeed = 25 * deltaTime;
        this.selectedObject.Transform.Position = this.selectedObject.Transform.Position.Add(
            new Vector3(
                moveSpeed * this.moveXAction.AnalogValue,
                moveSpeed * this.moveYAction.AnalogValue,
                moveSpeed * this.moveZAction.AnalogValue,
            ),
        );

        if (this.dirtyRotation) {
            this.selectedObject.Transform.Rotation =
                Quaternion.Identity
                    .Multiply(Quaternion.FromAxisAngle(Vector3.UnitY, this.yaw))
                    .Multiply(Quaternion.FromAxisAngle(Vector3.UnitX, this.pitch));

            this.dirtyRotation = false;
            this.updateTextLabel();
        }
    }

    private setupActions() {
        let deadZone = 0.25;
        let gamePadIndex = 0;

        this.moveXAction = InputController.CreateInputAction("moveX");
        this.moveXAction.AddKeyboardTrigger(KeyboardScanCode.D, 1);
        this.moveXAction.AddKeyboardTrigger(KeyboardScanCode.A, -1);
        this.moveXAction.AddGamePadAxisTrigger(GamePadAxisScanCode.LeftX, gamePadIndex, 0, deadZone, true);

        this.moveYAction = InputController.CreateInputAction("moveY");
        this.moveYAction.AddKeyboardTrigger(KeyboardScanCode.Q, 1);
        this.moveYAction.AddKeyboardTrigger(KeyboardScanCode.E, -1);
        this.moveYAction.AddGamePadButtonTrigger(GamePadButtonScanCode.R1, gamePadIndex, 1);
        this.moveYAction.AddGamePadButtonTrigger(GamePadButtonScanCode.L1, gamePadIndex, -1);

        this.moveZAction = InputController.CreateInputAction("moveZ");
        this.moveZAction.AddKeyboardTrigger(KeyboardScanCode.W, -1);
        this.moveZAction.AddKeyboardTrigger(KeyboardScanCode.S, 1);
        this.moveZAction.AddGamePadAxisTrigger(GamePadAxisScanCode.LeftY, gamePadIndex, 0, deadZone, true);

        this.rotateXAction = InputController.CreateInputAction("rotateX");
        this.rotateXAction.AddKeyboardTrigger(KeyboardScanCode.Numpad8, 1);
        this.rotateXAction.AddKeyboardTrigger(KeyboardScanCode.Numpad2, -1);
        this.rotateXAction.AddGamePadAxisTrigger(GamePadAxisScanCode.RightY, gamePadIndex, 0, deadZone, true);

        this.rotateYAction = InputController.CreateInputAction("rotateY");
        this.rotateYAction.AddKeyboardTrigger(KeyboardScanCode.Numpad4, 1);
        this.rotateYAction.AddKeyboardTrigger(KeyboardScanCode.Numpad6, -1);
        this.rotateYAction.AddGamePadAxisTrigger(GamePadAxisScanCode.RightX, gamePadIndex, 0, deadZone, true);

        this.rotateZAction = InputController.CreateInputAction("rotateZ");
        this.rotateZAction.AddKeyboardTrigger(KeyboardScanCode.Numpad9, 1);
        this.rotateZAction.AddKeyboardTrigger(KeyboardScanCode.Numpad7, -1);
        this.rotateZAction.AddGamePadButtonTrigger(GamePadButtonScanCode.X, gamePadIndex, -1);
        this.rotateZAction.AddGamePadButtonTrigger(GamePadButtonScanCode.B, gamePadIndex, 1);

        this.resetAction = InputController.CreateInputAction("reset");
        this.resetAction.AddKeyboardTrigger(KeyboardScanCode.Return, 1);
        this.resetAction.AddGamePadButtonTrigger(GamePadButtonScanCode.Start, gamePadIndex, 1);
        this.resetAction.OnDigitalDeactivate.Subscribe(() => {
            this.selectedObject.Transform.Position = new Vector3(0, 0, 0);
            this.pitch = 0;
            this.yaw = 0;
            this.roll = 0;
            this.dirtyRotation = true;
            this.updateTextLabel();
        });

        this.changeObjectAction = InputController.CreateInputAction("changeObject");
        this.changeObjectAction.AddKeyboardTrigger(KeyboardScanCode.Space, 1);
        this.changeObjectAction.AddGamePadButtonTrigger(GamePadButtonScanCode.Back, gamePadIndex, 1);
        this.changeObjectAction.OnDigitalDeactivate.Subscribe(() => {
            this.selectedObject.Active = false;
            if (this.selectedObject === this.sphereGameObject) {
                this.selectedObject = this.platformGameObject;
            } else if (this.selectedObject === this.platformGameObject) {
                this.selectedObject = this.testCarGameObject;
            } else if (this.selectedObject === this.testCarGameObject) {
                this.selectedObject = this.sphereGameObject;
            }
            this.selectedObject.Active = true;
        });
    }

    private updateTextLabel() {
        this.debugTextLabel.Text =
            `Selected GameObject: ${this.selectedObject.Name}` +
            "\n" +
            `Rotation: (P: ${this.pitch.toFixed(2)}, Y: ${this.yaw.toFixed(2)}, R: ${this.roll.toFixed(2)})`;
    }
}
