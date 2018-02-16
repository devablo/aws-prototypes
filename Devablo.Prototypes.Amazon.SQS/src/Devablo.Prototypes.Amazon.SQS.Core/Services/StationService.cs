using Devablo.Prototypes.Amazon.SQS.Core.Domain;

namespace Devablo.Prototypes.Amazon.SQS.Core.Services
{
    public interface IStationService
    {
        void Create(CreateStationCommand command);
    }
    public class StationService : IStationService
    {
        public void Create(CreateStationCommand command)
        {
            // Do Something
        }
    }
}