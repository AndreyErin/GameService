using System.Collections.Concurrent;

namespace GameService.Models.Inst
{
    //Сервис матчей
    public class InstancesService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        private ConcurrentDictionary<int , Instance> _instancesDictionary;

        public InstancesService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;    

            //комнаты по умолчанию при запуске
            _instancesDictionary = new ConcurrentDictionary<int, Instance>() 
            {
                [1] = new Instance(150, _scopeFactory),
                [2] = new Instance(250, _scopeFactory),
            };
        }

        public int Add(int bet) 
        {
            Instance inst = new(bet, _scopeFactory);

            bool result =  _instancesDictionary.TryAdd(inst.Id, inst);
            if (result) 
            {
                return inst.Id;
            }

            return 0;
        }

        public Instance? Get(int id) 
        {
            bool result = _instancesDictionary.TryGetValue(id, out Instance? inst);
            if (result) 
            {
                return inst;
            }
            return null;
        }

        public void Delete(int id)
        {
            bool result = _instancesDictionary.TryGetValue(id, out Instance? inst);
            if (inst != null) 
            {
                _instancesDictionary.TryRemove(inst.Id, out _);
            }
        }

        public List<Instance> GetFreeInsts() 
        {
            return _instancesDictionary.Values.Where(x => x.IsVisible == true).ToList();
        }

    }
}
