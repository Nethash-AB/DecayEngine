// ToDo: Redo this once collider resource is implemented.
//using DecayEngine.DecPakLib.Resource.RootElement.Collider;
//using DecayEngine.ModuleSDK.Component;
//using DecayEngine.ModuleSDK.Component.RigidBody;
//
//namespace DecayEngine.StubEngines.Physics.Component.RigidBody
//{
//    public class StubRigidBodyFactory : IComponentFactory<StubRigidBodyComponent, Collider2DResource>, IComponentFactory<IRigidBody, Collider2DResource>
//    {
//        IRigidBody IComponentFactory<IRigidBody, Collider2DResource>.CreateComponent(Collider2DResource resource)
//        {
//            return CreateComponent(resource);
//        }
//
//        public StubRigidBodyComponent CreateComponent(Collider2DResource resource)
//        {
//            return new StubRigidBodyComponent {Resource = resource};
//        }
//    }
//}