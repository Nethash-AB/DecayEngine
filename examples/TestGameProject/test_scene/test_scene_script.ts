export default class TestScene {
    // Property Exports
    public testVar: string;

    // Default Injections
    private self: Script;
    private parent: Scene;

    public onInit() {
        Logging.Append(Logging.LogSeverity.Info, "TestScene", "Current scene name: " + SceneController.ActiveScene.Name);
        this.testVar = "pepito";

        SoundController.MuteAudio();
    }

    public onUpdate(deltaTime) {
        //
    }

    public onRender(deltaTime) {
        //
    }
}
