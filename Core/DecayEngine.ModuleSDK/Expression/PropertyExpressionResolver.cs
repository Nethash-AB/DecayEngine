using System.Linq;
using DecayEngine.DecPakLib.Resource.Expression.Property;
using DecayEngine.DecPakLib.Resource.Expression.Property.Reference;
using DecayEngine.DecPakLib.Resource.RootElement;

namespace DecayEngine.ModuleSDK.Expression
{
    public class PropertyExpressionResolver
    {
        private readonly IPropertyExpression _rootExpression;

        public PropertyExpressionResolver(IPropertyExpression rootExpression)
        {
            _rootExpression = rootExpression;
        }

        public TResource Resolve<TResource>()
            where TResource : class, IRootResource
        {
            return _rootExpression switch
            {
                ResourceReferenceExpression reference => ResolveReference<TResource>(reference.ReferenceId),
                _ => null
            };
        }

        private static TResource ResolveReference<TResource>(string referenceId)
            where TResource : class, IRootResource
        {
            return GameEngine.ResourceBundles
                .SelectMany(bundle => bundle.Resources.OfType<TResource>())
                .FirstOrDefault(resource => resource.Id == referenceId);
        }
    }
}