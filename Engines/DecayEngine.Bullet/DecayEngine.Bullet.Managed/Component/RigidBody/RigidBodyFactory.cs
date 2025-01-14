// ToDo: Redo this once collider resource is implemented.
//using DecayEngine.DecPakLib.Resource.RootElement.Collider;
//using DecayEngine.ModuleSDK.Component;
//using DecayEngine.ModuleSDK.Component.RigidBody;
//
//namespace DecayEngine.Bullet.Managed.Component.RigidBody
//{
//    public class RigidBodyFactory : IComponentFactory<RigidBody2DComponent, Collider2DResource>, IComponentFactory<IRigidBody, Collider2DResource>
//    {
//        IRigidBody IComponentFactory<IRigidBody, Collider2DResource>.CreateComponent(Collider2DResource resource)
//        {
//            return CreateComponent(resource);
//        }
//
//        public RigidBody2DComponent CreateComponent(Collider2DResource resource)
//        {
//            return new RigidBody2DComponent {Resource = resource};
//        }
//    }
//}