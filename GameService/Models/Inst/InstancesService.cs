namespace GameService.Models.Inst
{
    //Сервис матчей
    public class InstancesService
    {
        public List<Instance> instances;
        public InstancesService()
        {
            instances = new List<Instance>()
            {
                new(150),
                new(250)
            };
        }
    }
}
