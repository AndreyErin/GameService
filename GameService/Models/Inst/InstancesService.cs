namespace GameService.Models.Inst
{
    //Сервис матчей
    public class InstancesService
    {
        private List<Instance> _instancesList;
        public InstancesService()
        {
            //комнаты по умолчанию при запуске
            _instancesList = new List<Instance>()
            {
                new(150),
                new(250)
            };       
        }

        public int Add(int bet) 
        {
            Instance inst = new(bet);
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
