namespace Neuroglia.K8s
{

    /// <summary>
    /// Exposes Kubernetes defaults 
    /// </summary>
    public static class KubernetesDefaults
    {

        /// <summary>
        /// Exposes default Kubernetes API versions
        /// </summary>
        public static class ApiVersions
        {

            /// <summary>
            /// Gets the 'v1' API version
            /// </summary>
            public const string V1 = "v1";

            /// <summary>
            /// Gets the 'apps/v1' API version
            /// </summary>
            public const string AppsV1 = "apps/v1";

        }

        /// <summary>
        /// Exposes constants about Kubernetes default Kinds
        /// </summary>
        public static class Kinds
        {
            /// <summary>
            /// Gets the kind of the 'Deployment' object
            /// </summary>
            public const string Deployment = "Deployment";

            /// <summary>
            /// Gets the kind of the 'Service' object
            /// </summary>
            public const string Service = "Service";
        }

        /// <summary>
        /// Exposes default Kubernetes service types
        /// </summary>
        public static class ServiceTypes
        {

            /// <summary>
            /// Gets the 'ExternalName' service type
            /// </summary>
            public const string ExternalName = "ExternalName";

        }

    }

}
