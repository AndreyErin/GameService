namespace GameService.Models.Inst
{
    //Сервис матчей
    public class InstancesService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        private List<Instance> _instancesList;
        public InstancesService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;

            //комнаты по умолчанию при запуске
            _instancesList = new List<Instance>()
            {
                new(150, _scopeFactory),
                new(250, _scopeFactory)
            };       
        }

        public int Add(int bet) 
        {
            Instance inst = new(bet, _scopeFactory);
            _instancesList.Add(inst);

            return inst.Id;
        }

        public Instance? Get(int id) 
        {
            return _instancesList.FirstOrDefault(x=>x.Id == id);
        }

        public void Delete(int id)
        {
            Instance? inst = _instancesList.FirstOrDefault(x => x.Id == id);
            if (inst != null)
            {
                _instancesList.Remove(inst);
            }
        }

        public List<Instance> GetFreeInsts() 
        {
            return _instancesList.Where(x => x.IsVisible == true).ToList();
        }

    }
}
