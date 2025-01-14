import Vector3 = ManagedMath.Vector3;

export default class TestCollisionScene {

    private static ContactPointsToString(contactPoints: Vector3[]): string {
        let contactPointStrings: string[] = [];
        contactPoints.forEach((cp) => contactPointStrings.push(`[${cp.X}, ${cp.Y}, ${cp.Z}]`));

        return contactPointStrings.join(", ");
    }

    // Default Injections
    private self: Script;
    private parent: Scene;

    // Prefab Injections
    private car1: GameObject;
    private car2: GameObject;

    // Class Fields
    private rigidbody1: RigidBody;
    private rigidbody2: RigidBody;
    private reset1: boolean;
    private reset2: boolean;

    public onInit() {
        Logging.Append(Logging.LogSeverity.Info, "TestTweenScene", "Current scene name: " + SceneController.ActiveScene.Name);

        this.rigidbody1 = this.car1.Components.find((value) => value.Type === ManagedObjectType.RigidBody) as RigidBody;
        this.rigidbody2 = this.car2.Components.find((value) => value.Type === ManagedObjectType.RigidBody) as RigidBody;

        if (this.rigidbody1) {
            console.log("Rigidbody for car 1 found.");
            this.rigidbody1.CollisionMask = 0b00000000000000000000000000000001; // 1
        }

        if (this.rigidbody2) {
            console.log("Rigidbody for car 2 found.");
            this.rigidbody2.CollisionMask = 0b00000000000000000000000000000001; // 1
        }

        if (this.rigidbody1 && this.rigidbody2) {
            this.rigidbody1.OnCollide.Subscribe(this.OnCollide);
            this.rigidbody2.OnCollide.Subscribe(this.OnCollide);

            this.rigidbody1.ApplyCentralForce(new Vector3(1, 0, 0).Multiply(100000));
            this.rigidbody2.ApplyCentralForce(new Vector3(-1, 0, 0).Multiply(100000));
        }
    }

    public onUpdate(deltaTime) {
        if (!this.reset1) {
            if ((this.rigidbody1.Parent as GameObject).Transform.Position.X < -2) {
                console.log("Resetting car1");
                this.rigidbody1.ClearForces();
                this.rigidbody1.ApplyCentralForce(new Vector3(1, 0, 0).Multiply(200000));
                this.reset1 = true;
            }
        } else {
            if ((this.rigidbody1.Parent as GameObject).Transform.Position.X > -1) {
                this.reset1 = false;
            }
        }

        if (!this.reset2) {
            if ((this.rigidbody2.Parent as GameObject).Transform.Position.X > 2) {
                console.log("Resetting car2");
                this.rigidbody2.ClearForces();
                this.rigidbody2.ApplyCentralForce(new Vector3(-1, 0, 0).Multiply(200000));
                this.reset2 = true;
            }
        } else {
            if ((this.rigidbody2.Parent as GameObject).Transform.Position.X < 1) {
                this.reset2 = false;
            }
        }
    }

    public onRender(deltaTime) {
        //
    }

    private OnCollide(collisionData: CollisionData) {
        console.log(`Car (${collisionData.SelfBody.Parent.Name}) collided with Car (${collisionData.OtherBody.Parent.Name}).`);
        console.log(`Contact points for ${collisionData.SelfBody.Parent.Name}: ${TestCollisionScene.ContactPointsToString(collisionData.ContactPointsSelf)}`);
        console.log(`Contact points for ${collisionData.OtherBody.Parent.Name}: ${TestCollisionScene.ContactPointsToString(collisionData.ContactPointsOther)}`);
    }
}
