namespace XrmFramework.DeployUtils.Context
{
    /*
    public class FlatAssemblyContext : IFlatAssemblyContext
    {
        public string Name { get; }

        public PluginAssembly Assembly { get; set; }
        public ICollection<Plugin> Plugins { get; set; }
        public ICollection<Step> Steps { get; set; }
        public ICollection<StepImage> StepImages { get; set; }
        public ICollection<Plugin> Workflows { get; set; }
        public ICollection<CustomApi> CustomApis { get; set; }
        public ICollection<CustomApiRequestParameter> CustomApiRequestParameters { get; set; }
        public ICollection<CustomApiResponseProperty> CustomApiResponseProperties { get; set; }

        public IReadOnlyCollection<ICrmComponent> ComponentsOrderedPool
        {
            get
            {
                var res = new List<ICrmComponent>();
                res.Add(Assembly);
                res.AddRange(Plugins);
                res.AddRange(Steps);
                res.AddRange(StepImages);
                res.AddRange(Workflows);
                res.AddRange(CustomApis);
                res.AddRange(CustomApiRequestParameters);
                res.AddRange(CustomApiResponseProperties);
                return res;
            }
        }
    }
    */
}
